using System.Collections.Generic;
using Core.Models;
using Core.Routes.Students.Dtos;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Students.Maps;

[Mapper]
public partial class StudentMapper
{
#pragma warning disable RMG020 // Source member is not mapped to any target member
    public partial StudentDto Map(Student student);
#pragma warning restore RMG020 // Source member is not mapped to any target member

    public partial IEnumerable<StudentDto> Map(IEnumerable<Student> students);
}