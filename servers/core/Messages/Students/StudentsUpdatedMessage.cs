using System.Collections.Generic;
using Core.Routes.Students.Dtos;

namespace Core.Messages.Students;

public class StudentsUpdatedMessage
{
    public required List<StudentDto> Students { get; set; } = [];
}