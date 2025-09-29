using Domain.Entities;

namespace Application.Services.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(User user);
}