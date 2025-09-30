using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>, IBaseEntity<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public virtual ICollection<ShortUrl>? ShortUrls { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}