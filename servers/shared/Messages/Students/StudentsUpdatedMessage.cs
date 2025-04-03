using System.Collections.Generic;

namespace Shared.Messages.Students;

public class StudentsUpdatedMessage
{
    public static string TopicName => "students-updated";

    public required List<StudentDto> Students { get; set; } = [];
}