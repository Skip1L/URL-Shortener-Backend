using System.Security.Claims;
using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using LoginRequest = Application.DTOs.LoginRequest;
using RegisterRequest = Application.DTOs.RegisterRequest;

namespace Web.Endpoints;

public static class Endpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/account/register", async (
                RegisterRequest model,
                UserManager<User> userManager,
                RoleManager<IdentityRole<Guid>> roleManager,
                IMapper mapper) =>
            {
                if (!await roleManager.RoleExistsAsync(model.Role))
                    return Results.BadRequest(new { error = $"Role '{model.Role}' does not exist." });

                var userExists = await userManager.FindByEmailAsync(model.Email);
                if (userExists != null) return Results.Conflict(new { error = "User with this email already exists." });

                var user = mapper.Map<User>(model);

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, model.Role);
                    return Results.Ok(
                        new { message = $"User '{user.Email}' created and assigned role '{model.Role}'." });
                }

                return Results.BadRequest(new
                {
                    error = "User creation failed.",
                    details = result.Errors.Select(e => e.Description)
                });
            }).AllowAnonymous()
            .WithTags("Account")
            .WithName("RegisterUser");

        app.MapPost("/api/account/login", async (
            LoginRequest model,
            UserManager<User> userManager,
            ITokenService tokenService) =>
        {
            var user = await userManager.FindByNameAsync(model.UserName);

            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                return Results.Unauthorized();

            var token = await tokenService.CreateToken(user);

            return Results.Ok(new { Username = user.UserName, Token = token });
        })
            .WithTags("Account")
            .WithName("LoginUser"); ;

        app.MapGroup("/api/urls")
            .RequireAuthorization()
            .WithTags("ShortUrls")
            .MapPost("/", async (
                ShortenUrlRequest request,
                IUrlService urlService,
                HttpContext context) =>
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Results.Unauthorized();
                }

                var shortUrl = await urlService.CreateShortUrlAsync(
                    request.OriginalUrl,
                    userId,
                    context.RequestAborted
                );

                if (shortUrl == null)
                {
                    return Results.Conflict(new { error = "This URL has already been shortened." });
                }

                var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
                var fullShortUrl = $"{baseUrl}/{shortUrl.ShortCode}";

                return Results.Created(fullShortUrl, new
                {
                    id = shortUrl.Id,
                    originalUrl = shortUrl.OriginalUrl,
                    shortCode = shortUrl.ShortCode,
                    shortUrl = fullShortUrl
                });
            })
            .WithName("CreateShortUrl");

        app.MapGet("/{shortCode}", async (
                string shortCode,
                IUrlService urlService,
                CancellationToken cancellationToken) =>
            {
                var shortUrl = await urlService.GetByCodeAsync(shortCode, cancellationToken);

                if (shortUrl == null)
                {
                    return Results.NotFound(new { error = $"Short URL '{shortCode}' not found." });
                }

                return Results.Redirect(shortUrl.OriginalUrl, permanent: true);
            })
            .AllowAnonymous()
            .WithName("RedirectToOriginalUrl");
    }
}