using Application.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class ShortUrlRepository(ApplicationContext repositoryContext) : GenericRepository<ShortUrl, long>(repositoryContext), IShortUrlRepository
    {
        public async Task<ShortUrl> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await FirstOrDefaultAsync(u => u.ShortCode == code, cancellationToken);
        }

        public async Task<IEnumerable<ShortUrl>> GetAllUrls(CancellationToken cancellationToken = default)
        {
            return await GetAll(cancellationToken);
        }

        public async Task<ShortUrl> GetByOriginalUrlAsync(string originalUrl, CancellationToken cancellationToken = default)
        {
            return await FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl, cancellationToken);
        }
    }
}
