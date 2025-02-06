using System.Threading.Tasks;
using Core.Routes;
using Core.Routes.Admins.Commands;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public class AdminRoute : IRouteDefinition
{
    public void RegisterRoutes(WebApplication app)
    {
        var group = app.MapGroup("/admin");

        group.MapPost("/login", LoginAdmin);
        group.MapPost("/register", RegisterAdmin);
        group.MapPost("/signout", SignoutAdmin);
    }

    public async Task<IResult> LoginAdmin(LoginAdminCommand command, IMediator mediator)
    {
        await mediator.Send(command);
        return Results.Ok();
    }

    public async Task<IResult> RegisterAdmin(RegisterAdminCommand command, IMediator mediator)
    {
        await mediator.Send(command);
        return Results.Ok();
    }

    public async Task<IResult> SignoutAdmin(IMediator mediator)
    {
        var command = new SignoutAdminCommand();
        await mediator.Send(command);
        return Results.Ok();
    }
}