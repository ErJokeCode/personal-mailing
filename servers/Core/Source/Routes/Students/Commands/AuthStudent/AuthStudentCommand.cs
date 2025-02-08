
using Core.Routes.Students.Dtos;
using FluentResults;
using MediatR;

namespace Core.Routes.Students.Commands;

public class AuthStudentCommand : IRequest<Result<StudentDto>>
{
    public required string Email { get; set; }
    public required string PersonalNumber { get; set; }
    public required string ChatId { get; set; }
}