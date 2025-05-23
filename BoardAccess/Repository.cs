﻿using Microsoft.EntityFrameworkCore;
using BoardAccess.Models;

namespace BoardAccess
{
    public class Repository<T> : IRepository<T>
          where T : DbEntity
    {
        private readonly BoardDbContext _dbContext;
        public Repository(BoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(int PageSize, int PageNumber)
        {
            var skip = (PageNumber - 1) * PageSize;
            var result = await _dbContext.Set<T>().Skip(skip).Take(PageSize).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Set<T>().CountAsync();
        }
        public async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            if (entity != null) {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }            
            return entity;
        }
        public async Task<T> AddAsync(T entity)
        {
            var modelEntity = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return (T)modelEntity.Entity;
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbContext.ChangeTracker.Clear();
            var modelEntity = await GetByIdAsync(entity.Id);
            if (modelEntity != null)
            {
                _dbContext.Entry(modelEntity).CurrentValues.SetValues(entity);
                await _dbContext.SaveChangesAsync();
            }
            else
                throw new Exception("Entity Not Found");

        }
        public async Task DeleteAsync(T entity)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
