using System.Collections.Generic;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Admins.Maps;

[Mapper]
public partial class AdminMapper
{
    public partial AdminDto Map(Admin admin);
    public partial IEnumerable<AdminDto> Map(IEnumerable<Admin> admins);

    public partial Shared.Messages.Admins.AdminDto MapToMessage(Admin admin);
}