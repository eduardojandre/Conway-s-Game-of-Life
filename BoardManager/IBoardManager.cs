namespace BoardManager
{
    public interface IBoardManager
    {
        public Task<DTOs.Generation> AddGeneration(string boardId);
        public Task<IEnumerable<DTOs.Board>> GetAllBoardsAsync(int pageSize, int pageNumber);
        public Task<DTOs.Board> GetBoardAsync(string id);
        public Task<int> GetBoardsCountAsync();
        public Task<DTOs.Generation> GetCurrentGeneration(string boardId);
        public Task<DTOs.Generation> CalculateFinalGeneration(string boardId);
        public Task<DTOs.Generation> CalculateGenerationNumber(string boardId, int generationNumber);
        public Task<DTOs.Generation> GetGenerationAsync(string boardId, int generationNumber);
        public Task<DTOs.Board> InsertBoardAsync(DTOs.NewBoardRequest request);        
    }
}
