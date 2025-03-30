﻿using BoardAccess.Models;

namespace BoardAccess
{
    public interface IRepository<T>
        where T : DbEntity
    {
        public Task<List<T>> GetAllAsync();
        public Task<T> GetByIdAsync(Guid id);
        public Task<T> AddAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
    }
}
