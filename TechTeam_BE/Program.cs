using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core;
using Core.Interfaces.Services;
using Core.Services;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TechTeam_BE;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
// builder.Services.AddAuthorization();

var jwtManager = new JwtManager();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = jwtManager.GetTokenValidationParameters();
});
builder.Services.AddAuthorization(o =>
{
    o.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});


var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();



app.MapPost("/login", async (LoginUser credential) =>
{
    if (credential.password != "Password1!")
    {
        return Results.Unauthorized();
    }
    
    // db

    var token = jwtManager.SignIn(credential.username);
    return Results.Ok(token);
}).AllowAnonymous().Produces<TokenResponse>();


app.MapPost("/refresh", (RefreshModel model) =>
{
    return Results.Ok(jwtManager.Refresh(model.refreshToken));
}).AllowAnonymous();

app.MapGet("/users", async ([FromServices] IUserService repo) =>
{
    var users = await repo.GetAllUserAsync();
    return Results.Ok(users);
}).Produces<IEnumerable<User>>();

app.MapGet("/users/{id}", async ([FromServices] IUserService repo, Guid id) =>
{
    var user = await repo.GetUserByIdAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
}).Produces<User>();

app.MapPost("/users", async ([FromServices] IUserService repo, User user) =>
{
    var created = await repo.CreateUserAsync(user);
    return Results.Created($"/users/{created.Id}", created);
}).Produces<User>();

app.MapPut("/users/{id}", async ([FromServices] IUserService repo, User user, Guid id) =>
{
    if (!(await repo.CheckUserExistAsync(id)))
    {
        return Results.NotFound();
    }
    
    user.Id = id;
    await repo.UpdateUserAsync(user);
    var updated = await repo.GetUserByIdAsync(id);
    return Results.Created($"/users/{updated!.Id}", updated);
}).Produces<User>();

app.MapDelete("/users/{id}", async ([FromServices] IUserService repo, Guid id) =>
{
    if (!(await repo.CheckUserExistAsync(id)))
    {
        return Results.NotFound();
    }
    await repo.DeleteUser(id);
    return Results.Ok();
});


app.Run();

record RefreshModel(string refreshToken);
record LoginUser(string username, string password);


