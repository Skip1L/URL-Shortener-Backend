using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IUrlService
    {
        Task<ShortUrl> CreateShortUrlAsync(string originalUrl, Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<ShortUrl>> GetAllUrlsAsync(CancellationToken cancellationToken);
        Task<ShortUrl> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<ShortUrl> GetUrlDetailsAsync(long id, CancellationToken cancellationToken);
        Task<bool> DeleteUrlAsync(long id, CancellationToken cancellationToken);
    }
}
