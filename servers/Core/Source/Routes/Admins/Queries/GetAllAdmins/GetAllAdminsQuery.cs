using System.Collections;
using System.Collections.Generic;
using Core.Routes.Admins.Dtos;
using MediatR;

namespace Core.Routes.Admins.Queries;

public class GetAllAdminsQuery : IRequest<IEnumerable<AdminDto>>;