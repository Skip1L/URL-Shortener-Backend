using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IGenericRepository<TEntity, in TKey> where TEntity : class, IBaseEntity<TKey>
    {
    Task<List<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> filter = null,
        CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetPagedAsync(int pageSize, int pageNumber, Expression<Func<TEntity, bool>> filter = null,
        CancellationToken cancellationToken = default);

    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null,
        CancellationToken cancellationToken = default);

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
