using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Web.Helpers;

public static class EntitySeeder
{
    public static async Task InitializeDbForRoles(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        var roleManager = serviceScope?.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>()
                          ?? throw new Exception("An error occured while seeding Roles data");

        var rolesToCreate = typeof(UserRoles).GetFields()
            .Select(field => field.GetValue(null)?.ToString())
            .Where(roleName => !string.IsNullOrEmpty(roleName))
            .ToList();

        foreach (var roleName in rolesToCreate)
            if (!await roleManager.RoleExistsAsync(roleName!))
            {
                var identityRole = new IdentityRole<Guid> { Name = roleName };
                await roleManager.CreateAsync(identityRole);
            }
    }
}