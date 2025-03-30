namespace BoardManager
{
    public interface IBoardManager
    {
        public Task<DTOs.Generation> AddGeneration(string boardId);
        public Task<DTOs.Board> GetBoardAsync(string id);
        public Task<DTOs.Generation> GetGenerationAsync(string boardId, int generationNumber);
        public Task<DTOs.Board> InsertBoardAsync(DTOs.NewBoardRequest request);
    }
}
