using Notify.Models;
using Riok.Mapperly.Abstractions;
using Shared.Context.Admins.Messages;

namespace Notify.Features.Admins;

[Mapper]
public partial class AdminMapper
{
    public partial Admin Map(AdminCreatedMessage adminCreatedMessage);
}