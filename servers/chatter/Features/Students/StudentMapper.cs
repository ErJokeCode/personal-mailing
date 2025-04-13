using Chatter.Models;
using Riok.Mapperly.Abstractions;
using Shared.Context.Students.Messages;
using Shared.Infrastructure.Extensions;

namespace Chatter.Features.Students;

[Mapper]
public partial class StudentMapper : IMapper
{
    public partial Student Map(StudentAuthedMessage studentAuthedMessage);
    public partial Student Map(StudentUpdatedMessage studentUpdatedMessage);
}
