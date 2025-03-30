using BoardAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardAccess
{
    public class GenerationRepository : Repository<Generation>, IGenerationRepository
    {
        private readonly BoardDbContext _dbContext;
        public GenerationRepository(BoardDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Generation> GetLastGenerationByBoardIdAllAsync(Guid boardId)
        {
            var entity = await _dbContext.Set<Generation>()
                .OrderBy(g => g.Number)
                .LastOrDefaultAsync(g => g.BoardId == boardId);
            return entity;
        }
    }
}
