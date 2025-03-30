using Microsoft.EntityFrameworkCore;

namespace BoardAccess
{
    public class BoardDbContext : DbContext
    {
        public DbSet<Models.Board> Boards { get; set; }
        public DbSet<Models.Generation> Generations { get; set; }
        public BoardDbContext(DbContextOptions<BoardDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
