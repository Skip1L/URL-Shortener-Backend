namespace Domain.Entities;

public class ShortUrl : IBaseEntity
{
    public required string OriginalUrl { get; set; }
    public required string ShortCode { get; set; }
    public Guid CreatedById { get; set; }
    public virtual User CreatedBy { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastUpdatedAt { get; set; }
}