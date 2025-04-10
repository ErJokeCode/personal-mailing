using Notify.Models;
using Riok.Mapperly.Abstractions;
using Shared.Context.Students.Messages;

namespace Notify.Features.Students;

[Mapper]
public partial class StudentMapper
{
    public partial Student Map(StudentAuthedMessage studentAuthedMessage);
    public partial Student Map(StudentUpdatedMessage studentUpdatedMessage);
}
