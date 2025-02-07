using System;
using Core.Routes.Admins.Dtos;
using FluentResults;
using MediatR;

namespace Core.Routes.Admins.Queries;

public class GetAdminByIdQuery : IRequest<Result<AdminDto>>
{
    public Guid AdminId { get; init; }

    public GetAdminByIdQuery(Guid adminId)
    {
        AdminId = adminId;
    }
}