using AutoMapper;
using BoardAccess;
using BoardManager.DTOs;
using BoardManager.Exceptions;

namespace BoardManager
{
    public class BoardManager : IBoardManager
    {
        private const int MaximunNumberOfIterations = 5000;
        private IMapper _mapper;
        private IRepository<BoardAccess.Models.Board> _boardRepository;
        private IGenerationRepository _generationRepository;
        public BoardManager(IMapper mapper, IRepository<BoardAccess.Models.Board> boardRepository, IGenerationRepository generationRepository) { 
            _mapper = mapper;
            _boardRepository = boardRepository;
            _generationRepository = generationRepository;
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
            var nextState = CalculateNextState(currentState);
            
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
        private bool[][] CalculateNextState(bool[][] currentState) {
            var newBoardState = new bool[currentState.Length][];
            for (int i = 0; i < newBoardState.Length; i++)
            {
                newBoardState[i] = new bool[currentState[i].Length];
                for (int j = 0; j < currentState[i].Length; j++) {
                    var isAlive = IsAlive(currentState, i, j);
                    var neighboursAlive = CalculateLivingNeighbours(currentState, i, j);
                    newBoardState[i][j] = CalculateNextCellState(isAlive,neighboursAlive);
                }
            }
            return newBoardState;
        }

        private bool CalculateNextCellState(bool currentStateIsAlive, int neighboursAlive) {
            if (currentStateIsAlive && neighboursAlive < 2)
            {
                return false; //1. Any live cell with fewer than two live neighbours dies, as if by underpopulation.
            }
            if (currentStateIsAlive && neighboursAlive > 3)
            {
                return false; //3. Any live cell with more than three live neighbours dies, as if by overpopulation.
            }
            if (currentStateIsAlive) {
                return true; //2. Any live cell with two or three live neighbours lives on to the next generation.
            }
            if (neighboursAlive == 3) {
                return true;//4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            }
            return false;
        }

        private int CalculateLivingNeighbours(bool[][] currentState, int row, int column) {

            int countAlive = 0;
            for (int i = row - 1; i < row + 2; i++) {
                for (int j = column - 1; j < column + 2; j++) {
                    if (!(row == i && column == j))
                    {
                        var currentCellIsAlive = IsAlive(currentState, i, j);
                        if (currentCellIsAlive)
                        {
                            countAlive++;
                        }
                    }                    
                }
            }
            return countAlive;
        }
        private bool IsAlive(bool[][] currentState, int row, int column) {
            if (row < 0)
                return false;
            if (column < 0)
                return false;
            if(row >= currentState.Length)
                return false;
            if (column >= currentState[row].Length)
                return false;
            return currentState[row][column];
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
