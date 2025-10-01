using System.Linq.Expressions;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey>(ApplicationContext repositoryContext)
        : IGenericRepository<TEntity, TKey> where TEntity : class, IBaseEntity<TKey>
    {
        protected readonly ApplicationContext _repositoryContext = repositoryContext;

        public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _repositoryContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _repositoryContext.Attach(entity);
            _repositoryContext.Entry(entity).State = EntityState.Modified;
            await _repositoryContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _repositoryContext
                .Set<TEntity>()
                .Where(e => e.Id!.Equals(id))
                .ExecuteDeleteAsync(cancellationToken) > 0;
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            var set = _repositoryContext.Set<TEntity>();

            if (filter != null)
            {
                return (await set.AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken))!;
            }

            return (await set.AsNoTracking().FirstOrDefaultAsync(cancellationToken))!;
        }

        public async Task<List<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            IQueryable<TEntity> query = _repositoryContext.Set<TEntity>();

            query = query.Where(filter);

            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetPagedAsync(int pageSize, int pageNumber, Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _repositoryContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _repositoryContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _repositoryContext.Set<TEntity>().ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
