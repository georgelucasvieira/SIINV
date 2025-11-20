using MediatR;

namespace AuthService.Api.Features.Register;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (RegisterCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);

            if (!result.Sucesso)
            {
                return Results.BadRequest(result);
            }

            return Results.Created("/api/auth/me", result);
        })
        .WithName("Register")
        .WithTags("Auth")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .AllowAnonymous();
    }
}
