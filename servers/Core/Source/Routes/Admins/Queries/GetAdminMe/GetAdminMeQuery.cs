using Core.Routes.Admins.Dtos;
using FluentResults;
using MediatR;

namespace Core.Routes.Admins.Queries;

public class GetAdminMeQuery : IRequest<Result<AdminDto>>;