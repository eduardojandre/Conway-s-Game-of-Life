using BoardAccess.Models;

namespace BoardAccess
{
    public interface IGenerationRepository : IRepository<Generation>
    {
        public Task<Generation> GetLastGenerationByBoardIdAllAsync(Guid boardId);
    }
}
