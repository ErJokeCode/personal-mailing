using Core.Routes.Students.Dtos;

namespace Core.Messages.Students;

public class StudentAuthedMessage
{
    public required StudentDto Student { get; set; }
}