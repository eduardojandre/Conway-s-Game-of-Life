using AutoMapper;
using BoardAccess;
using BoardManager.DTOs;

namespace BoardManager
{
    public class BoardManager : IBoardManager
    {
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
            var accessBoard = await _boardRepository.GetByIdAsync(guid);
            var board = _mapper.Map<Board>(accessBoard);
            if (board == null)
            {
                throw new Exceptions.BusineesException("Board id not found");
            }
            var accessGeneration = await _generationRepository.GetLastGenerationByBoardIdAllAsync(guid);
            var lastGeneration = _mapper.Map<Generation>(accessGeneration);
            if (lastGeneration == null) {

                lastGeneration = new Generation
                {
                    Board = board,
                    IsFinalState = false,
                    Number = 0,
                    State = board.InitialState
                };
            }
            var nextState = lastGeneration.State;
            if (!lastGeneration.IsFinalState) {
                nextState = CalculateNextState(lastGeneration.State);
            }
            var newGeneration = new NewGenerationRequest
            {
                State = nextState,
                IsFinalState = Enumerable.SequenceEqual(nextState, lastGeneration.State),
                BoardId = board.Id,
                Number = lastGeneration.Number + 1
            };
            var accessNewGenerationRequest = _mapper.Map<BoardAccess.Models.Generation>(newGeneration);
            var accessNewGeneration = await _generationRepository.AddAsync(accessNewGenerationRequest);

            var result = _mapper.Map<Generation>(accessNewGeneration);
            result.Board = board;
            return result;            
        }

        public async Task<Board> GetBoardAsync(string id)
        {
            var guid = GetGuidFromString(id);
            var accessBoard = await _boardRepository.GetByIdAsync(guid);
            var board = _mapper.Map<Board>(accessBoard);
            return board;
        }

        public async Task<Generation> GetGenerationAsync(string boardId, int generationNumber)
        {
            var guid = GetGuidFromString(boardId);
            var accessGeneration = await _generationRepository.GetLastGenerationByBoardIdAllAsync(guid);
            var result = _mapper.Map<Generation>(accessGeneration);
            return result;
        }

        public async Task<Board> InsertBoardAsync(NewBoardRequest request)
        {
            var accessBoardRequest = _mapper.Map<BoardAccess.Models.Board>(request);
            var accessBoardResult = await _boardRepository.AddAsync(accessBoardRequest);
            var result = _mapper.Map<Board>(accessBoardResult);
            return result;
        }

        private Guid GetGuidFromString(string id) { 
            Guid.TryParse(id, out var guid);
            return guid;
        
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
    }
}
