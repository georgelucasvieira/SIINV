using System.Security.Claims;
using MediatR;

namespace AuthService.Api.Features.GetCurrentUser;

public static class GetCurrentUserEndpoint
{
    public static void MapGetCurrentUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/me", async (HttpContext context, IMediator mediator) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var query = new GetCurrentUserQuery { UsuarioId = userId };
            var result = await mediator.Send(query);

            if (result == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(result);
        })
        .WithName("GetCurrentUser")
        .WithTags("Auth")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
