using System.Collections.Generic;
using Core.Models;
using Core.Routes.Students.Dtos;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class StudentMapper
{
    public partial StudentDto Map(Student student);
    public partial IEnumerable<StudentDto> Map(IEnumerable<Student> students);
}