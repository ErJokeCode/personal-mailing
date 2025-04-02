using System.Collections.Generic;

namespace Notify.Messages.Students;

public class StudentsUpdatedMessage
{
    public required List<StudentDto> Students { get; set; }
}
