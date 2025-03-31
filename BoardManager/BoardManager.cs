using AutoMapper;
using BoardAccess;
using BoardManager.DTOs;
using BoardManager.Exceptions;

namespace BoardManager
{
    public class BoardManager : IBoardManager
    {
        private const int MaximunNumberOfIterations = 1000;
        private IMapper _mapper;
        private IRepository<BoardAccess.Models.Board> _boardRepository;
        private IGenerationRepository _generationRepository;
        private IBoardRulesEngine _boardRulesEngine;
        public BoardManager(IMapper mapper, IRepository<BoardAccess.Models.Board> boardRepository, IGenerationRepository generationRepository, IBoardRulesEngine boardRulesEngine) { 
            _mapper = mapper;
            _boardRepository = boardRepository;
            _generationRepository = generationRepository;
            _boardRulesEngine = boardRulesEngine;
        }

        public async Task<Generation> AddGeneration(string boardId)
        {
            var guid = GetGuidFromString(boardId);
            var board = await GetBoard(guid);
            var accessGeneration = await _generationRepository.GetLastByBoardIdAsync(guid);
            var lastGeneration = _mapper.Map<Generation>(accessGeneration);
            var currentState = lastGeneration?.State ?? board.InitialState;
            var currentNumber = lastGeneration?.Number ?? 0;
            var newGenerationRequest = CalculateNextGeneration(boardId, currentState, currentNumber);
            var accessNewGenerationRequest = _mapper.Map<BoardAccess.Models.Generation>(newGenerationRequest);
            var accessNewGeneration = await _generationRepository.AddAsync(accessNewGenerationRequest);

            var result = _mapper.Map<Generation>(accessNewGeneration);
            result.Board = board;
            return result;            
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync(int pageSize, int pageNumber)
        {
            var accessBoards = await _boardRepository.GetAllAsync(pageSize, pageNumber);
            var boards = _mapper.Map<IEnumerable<Board>>(accessBoards);
            return boards;
        }

        public async Task<Board> GetBoardAsync(string id)
        {
            var guid = GetGuidFromString(id);
            var accessBoard = await _boardRepository.GetByIdAsync(guid);
            var board = _mapper.Map<Board>(accessBoard);
            return board;
        }

        public async Task<int> GetBoardsCountAsync()
        {
            var result = await _boardRepository.GetCountAsync();
            return result;
        }

        public async Task<Generation> GetCurrentGeneration(string boardId)
        {
            var guid = GetGuidFromString(boardId);
            var board = await GetBoard(guid);
            var accessGeneration = await _generationRepository.GetLastByBoardIdAsync(guid);
            var result = _mapper.Map<Generation>(accessGeneration);
            if (result != null) {
                result.Board = board;
            }            
            return result;
        }

        public async Task<Generation> CalculateFinalGeneration(string boardId)
        {
            var finalGeneration = await CalculateGenerationN(boardId, MaximunNumberOfIterations);

            if (!finalGeneration.IsFinalState)
            {
                throw new BusineesException($"The board exceeded the maximum number of interactions ({MaximunNumberOfIterations}) without reaching a final state.");
            }
            var result = await GetCurrentGeneration(boardId);
            return result;
        }

        public async Task<Generation> CalculateGenerationNumber(string boardId, int number)
        {
            var generation = await CalculateGenerationN(boardId, number);
            return generation;
        }

        public async Task<Generation> GetGenerationAsync(string boardId, int generationNumber)
        {
            var guid = GetGuidFromString(boardId);
            var board = await GetBoard(guid);
            var accessGeneration = await _generationRepository.GetByBoardIdAndNumberAsync(guid, generationNumber);
            var result = _mapper.Map<Generation>(accessGeneration);
            if (result != null)
            {
                result.Board = board;
            }
            return result;
        }

        public async Task<Board> InsertBoardAsync(NewBoardRequest request)
        {
            var accessBoardRequest = _mapper.Map<BoardAccess.Models.Board>(request);
            var accessBoardResult = await _boardRepository.AddAsync(accessBoardRequest);
            var result = _mapper.Map<Board>(accessBoardResult);
            return result;
        }

        private async Task<Board> GetBoard(Guid guid) {

            var accessBoard = await _boardRepository.GetByIdAsync(guid);
            var board = _mapper.Map<Board>(accessBoard);
            if (board == null)
            {
                throw new Exceptions.BusineesException("The board id was not found");
            }
            return board;
        }

        private Guid GetGuidFromString(string id) { 
            Guid.TryParse(id, out var guid);
            return guid;
        
        }
        public NewGenerationRequest CalculateNextGeneration(string boardId, bool[][] currentState, int currentNumber)
        {
            var nextState = _boardRulesEngine.CalculateNextState(currentState);
            
            var newGeneration = new NewGenerationRequest
            {
                State = nextState,
                IsFinalState = AreBoardStatesEquivalent(nextState, currentState),
                BoardId = boardId,
                Number = currentNumber + 1
            };
            return newGeneration;
        }

        private async Task<Generation> CalculateGenerationN(string boardId, int n) {
            var guid = GetGuidFromString(boardId);
            var board = await GetBoard(guid);
            var currentGeneration = await GetCurrentGeneration(boardId);
            var lastIsFinal = currentGeneration?.IsFinalState ?? false;
            var lastGenerationNumber = currentGeneration?.Number ?? 0;
            var lastState = currentGeneration?.State ?? board.InitialState;
            var generationsToAdd = new List<NewGenerationRequest>();
            if (lastIsFinal)
            {
                return currentGeneration;
            }
            if (lastGenerationNumber>=n)
            {
                return await GetGenerationAsync(boardId, n);
            }

            while (!lastIsFinal && lastGenerationNumber < n)
            {
                var newGeneration = CalculateNextGeneration(boardId, lastState, lastGenerationNumber);
                lastIsFinal = newGeneration.IsFinalState;
                lastGenerationNumber = newGeneration.Number;
                lastState = newGeneration.State;
                generationsToAdd.Add(newGeneration);
            }

            var generationsToAddAccess = _mapper.Map<IEnumerable<BoardAccess.Models.Generation>>(generationsToAdd);
            await _generationRepository.AddRangeAsync(generationsToAddAccess);

            currentGeneration = await GetCurrentGeneration(boardId);
            return currentGeneration;
        }
        private bool AreBoardStatesEquivalent(bool[][] state1, bool[][] state2) {
            for (int i = 0; i < state1.Length; i++) {
                if (!Enumerable.SequenceEqual(state1[i], state2[i])){ 
                    return false;
                }
            }
            return true;
        }
    }
}
