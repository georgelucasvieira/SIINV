using MediatR;

namespace AuthService.Api.Features.RefreshToken;

public static class RefreshTokenEndpoint
{
    public static void MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh", async (RefreshTokenCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);

            if (!result.Sucesso)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        })
        .WithName("RefreshToken")
        .WithTags("Auth")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .AllowAnonymous();
    }
}
