using System.Collections.Generic;
using Core.Features.Admins.DTOs;
using Core.Models;
using Riok.Mapperly.Abstractions;
using Shared.Context.Admins.Messages;
using Shared.Infrastructure.Extensions;

namespace Core.Routes.Admins.Maps;

[Mapper]
public partial class AdminMapper : IMapper
{
    public partial AdminDto Map(Admin admin);
    public partial IEnumerable<AdminDto> Map(IEnumerable<Admin> admins);

    public partial AdminCreatedMessage MapToMessage(Admin admin);
}