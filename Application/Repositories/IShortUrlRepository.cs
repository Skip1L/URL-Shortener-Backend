using Domain.Entities;

namespace Application.Repositories
{
    public interface IShortUrlRepository : IGenericRepository<ShortUrl, long>
    {
        Task<ShortUrl> GetByOriginalUrlAsync(string originalUrl, CancellationToken cancellationToken = default);
        Task<ShortUrl> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
