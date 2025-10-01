using System.Security.Claims;
using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LoginRequest = Application.DTOs.LoginRequest;
using RegisterRequest = Application.DTOs.RegisterRequest;

namespace Web.Endpoints;

public static class Endpoints
{
    private static string aboutText =
        """This URL Shortener uses a Base62 encoding algorithm based on a sequential unique ID for each URL. When a new URL is added, the system assigns it a unique numeric ID(long), converts it to a short Base62 string, and stores it in the database. Short URLs are unique and deterministic, ensuring that each URL maps to exactly one shortened version.""";
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
            .WithName("LoginUser");

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

        var urlsGroup = app.MapGroup("/api/urls")
            .WithTags("ShortUrls");

        urlsGroup.MapGet("/info/{shortCode}", async (
                string shortCode,
                IUrlService urlService,
                CancellationToken cancellationToken) =>
            {
                var shortUrl = await urlService.GetByCodeAsync(shortCode, cancellationToken);

                if (shortUrl == null)
                    return Results.NotFound(new { error = "URL not found." });

                return Results.Ok(new
                {
                    shortUrl.OriginalUrl,
                    shortUrl.ShortCode,
                    CreatedBy = shortUrl.CreatedBy.UserName, // або email
                    shortUrl.CreatedAt
                });
            })
            .RequireAuthorization() // анонімам доступ заборонено
            .WithName("GetUrlInfo");

        urlsGroup.MapGet("/", async (
                IUrlService urlService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var urls = await urlService.GetAllUrlsAsync(cancellationToken);

                    var urlsDto = urls.Select(u => new ShortUrlDto
                    {
                        Id = u.Id,
                        OriginalUrl = u.OriginalUrl,
                        ShortCode = u.ShortCode,
                        CreatedBy = u.CreatedBy?.UserName,
                        CreatedAt = u.CreatedAt
                    });

                    return Results.Ok(urlsDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    return Results.Problem("An unexpected error occurred while fetching URLs.", statusCode: 500);
                }
            })
            .AllowAnonymous()
            .WithName("GetUrls");

        urlsGroup.MapPost("/", [Authorize] async (
                ShortenUrlRequest request,
                IUrlService urlService,
                HttpContext httpContext) =>
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Results.Unauthorized();
                }

                var shortUrl = await urlService.CreateShortUrlAsync(
                    request.OriginalUrl,
                    userId,
                    httpContext.RequestAborted
                );

                if (shortUrl == null)
                {
                    return Results.Conflict(new { error = "This URL has already been shortened." });
                }

                var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                var fullShortUrl = $"{baseUrl}/{shortUrl.ShortCode}";

                return Results.Created(fullShortUrl, new
                {
                    id = shortUrl.Id,
                    originalUrl = shortUrl.OriginalUrl,
                    shortCode = shortUrl.ShortCode,
                    shortUrl = fullShortUrl
                });
            })
            .RequireAuthorization() // Requires JWT token
            .WithName("AddUrl");

        urlsGroup.MapDelete("/{id:long}", [Authorize] async (
                long id,
                IUrlService urlService,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = httpContext.User.IsInRole("Admin");

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Results.Unauthorized();
                }

                try
                {
                    var shortUrl = await urlService.GetUrlDetailsAsync(id, cancellationToken);
                    if (isAdmin || shortUrl.CreatedBy.Id == userId)
                    {
                        var isDeleted = await urlService.DeleteUrlAsync(id, cancellationToken);
                        return Results.NoContent(); // 204 Success, nothing to return
                    }
                    else
                    {
                        // This could mean not found, or user is not the creator AND not an admin
                        return Results.Forbid(); // 403 Forbidden
                    }
                }
                catch (Exception)
                {
                    return Results.Problem("Failed to delete URL.", statusCode: 500);
                }
            })
            .RequireAuthorization()
            .WithName("DeleteUrl");

        app.MapGet("/api/about", () =>
        {
            var text = aboutText;
            return Results.Ok(text);
        }).AllowAnonymous();

        app.MapPost("/api/about", (string text) =>
        {
            aboutText = text;
            return Results.Ok();
        }).RequireAuthorization("Admin"); // тільки для адмінів
    }
}