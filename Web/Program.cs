using System.Reflection;
using System.Text;
using Application.Mapping;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Web.Endpoints;
using Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options
        .UseSqlServer(builder.Configuration.GetValue<string>("DBConnectionString"))
        .UseLazyLoadingProxies();
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = AuthOptionsHelper.GetIssuer(),
            ValidAudience = AuthOptionsHelper.GetAudience(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptionsHelper.GetSecretKey())),
            RequireExpirationTime = true
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await app.InitializeDbForRoles();

Endpoints.Map(app);

app.Run();