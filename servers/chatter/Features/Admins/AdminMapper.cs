using Chatter.Models;
using Riok.Mapperly.Abstractions;
using Shared.Context.Admins.Messages;
using Shared.Infrastructure.Extensions;

namespace Chatter.Features.Admins;

[Mapper]
public partial class AdminMapper : IMapper
{
    public partial Admin Map(AdminCreatedMessage adminCreatedMessage);
}