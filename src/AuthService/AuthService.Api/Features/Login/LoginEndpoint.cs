using MediatR;

namespace AuthService.Api.Features.Login;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (LoginCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);

            if (!result.Sucesso)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        })
        .WithName("Login")
        .WithTags("Auth")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .AllowAnonymous();
    }
}
