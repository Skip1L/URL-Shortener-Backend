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
        });
    }
}