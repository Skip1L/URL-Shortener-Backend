using Application.Helpers;
using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class UrlService(IShortUrlRepository shortUrlRepository) : IUrlService
    {
        public async Task<ShortUrl> CreateShortUrlAsync(string originalUrl, Guid userId, CancellationToken cancellationToken)
        {
            var existing = await shortUrlRepository.GetByOriginalUrlAsync(originalUrl, cancellationToken);
            if (existing != null) return null;

            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                ShortCode = ""
            };

            await shortUrlRepository.CreateAsync(shortUrl, cancellationToken);
            await shortUrlRepository.SaveChangesAsync(cancellationToken);

            shortUrl.ShortCode = Base62Encoder.Encode(shortUrl.Id);

            await shortUrlRepository.SaveChangesAsync(cancellationToken);

            return shortUrl;
        }

        public async Task<ShortUrl> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            return await shortUrlRepository.GetByCodeAsync(code, cancellationToken);
        }
    }
}
