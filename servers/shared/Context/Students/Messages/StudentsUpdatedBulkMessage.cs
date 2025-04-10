using System.Collections.Generic;

namespace Shared.Context.Students.Messages;

public class StudentsUpdatedBulkMessage : IMessage
{
    public static string TopicName => "students-updated-bulk";

    public required List<StudentUpdatedMessage> Students { get; set; } = [];
}