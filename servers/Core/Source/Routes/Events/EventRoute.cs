using System.Threading.Tasks;
using Core.Routes.Events.Commands;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Core.Routes.Events;

public class EventRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/events")
            .RequireAuthorization();

        group.MapPost("/upload", UploadEvent)
            .WithDescription("Обновляет информацию о студентах новыми данными из парсера");
    }

    public async Task<Ok> UploadEvent(IMediator mediator)
    {
        var command = new UploadEventCommand();

        await mediator.Send(command);

        return TypedResults.Ok();
    }
}