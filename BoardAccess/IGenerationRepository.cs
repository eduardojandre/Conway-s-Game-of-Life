using BoardAccess.Models;

namespace BoardAccess
{
    public interface IGenerationRepository : IRepository<Generation>
    {
        public Task<Generation> GetLastByBoardIdAsync(Guid boardId);
        public Task<Generation> GetByBoardIdAndNumberAsync(Guid boardId, int number);
    }
}
