
using System.Collections;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class AdminMapper
{
#pragma warning disable RMG020 // Source member is not mapped to any target member
    public partial AdminDto Map(Admin admin);
#pragma warning restore RMG020 // Source member is not mapped to any target member

#pragma warning disable RMG020 // Source member is not mapped to any target member
    public partial IEnumerable<AdminDto> Map(IEnumerable<Admin> admins);
#pragma warning restore RMG020 // Source member is not mapped to any target member
}