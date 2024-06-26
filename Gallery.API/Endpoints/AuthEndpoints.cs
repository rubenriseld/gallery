﻿using Gallery.API.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Gallery.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth");

        group.MapPost("/login", Login);

        group.MapPost("/logout", Logout).RequireAuthorization();

        group.MapGet("/check", Check).RequireAuthorization();

    }
    public static async Task<IResult> Login(SignInManager<IdentityUser> signInManager, LoginDTO login)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;
        var signInResult = await signInManager.PasswordSignInAsync(login.Email, login.Password, true, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            return Results.BadRequest();
        }
        return Results.Ok();
    }
    public static async Task<IResult> Check()
    {
        return Results.Ok();
    }
    public static async Task<IResult> Logout(SignInManager<IdentityUser> signInManager)
    {
        await signInManager.SignOutAsync().ConfigureAwait(false);
        return Results.NoContent();
    }
}