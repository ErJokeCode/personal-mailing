using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Infrastructure.Rest;
using Core.Models;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Students.Queries;

public class GetAllStudentsQuery : IRequest<PagedList<StudentDto>>
{
    public string? Search { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public string? Group { get; set; }
    public int? CourseNumber { get; set; }
    public string? TypeOfCost { get; set; }
    public string? TypeOfEducation { get; set; }
    public string? OnlineCourse { get; set; }
    public string? Subject { get; set; }
    public string? Team { get; set; }
}

public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, PagedList<StudentDto>>
{
    private readonly AppDbContext _db;
    private readonly StudentMapper _studentMapper;

    public GetAllStudentsQueryHandler(AppDbContext db)
    {
        _db = db;
        _studentMapper = new StudentMapper();
    }

    public async Task<PagedList<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _db.Students
            .Where(s => s.Active)
            .ToListAsync();

        students = FilterStudents(students, request).ToList();

        return PagedList<StudentDto>.Create(_studentMapper.Map(students), request.Page, request.PageSize);
    }

    public IEnumerable<Student> FilterStudents(IEnumerable<Student> students, GetAllStudentsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            var getFullName = (Student s) => $"{s.Info.Surname} {s.Info.Name} {s.Info.Patronymic ?? ""}";

            students = students
                .Where(s =>
                    FuzzySearch.Contains(s.Email, request.Search)
                    || FuzzySearch.Contains(getFullName(s), request.Search)
                );
        }

        if (!string.IsNullOrEmpty(request.Group))
        {
            students = students
                .Where(s => s.Info.Group.Number.Contains(request.Group, StringComparison.CurrentCultureIgnoreCase));
        }

        if (request.CourseNumber is not null && request.CourseNumber > 0)
        {
            students = students
                .Where(s => s.Info.Group.NumberCourse == request.CourseNumber);
        }

        if (!string.IsNullOrEmpty(request.TypeOfCost))
        {
            students = students
                .Where(s => s.Info.TypeOfCost != null && s.Info.TypeOfCost.Contains(request.TypeOfCost, StringComparison.CurrentCultureIgnoreCase));
        }

        if (!string.IsNullOrEmpty(request.TypeOfEducation))
        {
            students = students
                .Where(s => s.Info.TypeOfEducation != null && s.Info.TypeOfEducation.Contains(request.TypeOfEducation, StringComparison.CurrentCultureIgnoreCase));
        }

        if (!string.IsNullOrEmpty(request.OnlineCourse))
        {
            students = students
                .Where(s => s.Info.OnlineCourse.Any(o => o.Name.Contains(request.OnlineCourse, StringComparison.CurrentCultureIgnoreCase)));
        }

        if (!string.IsNullOrEmpty(request.Subject))
        {
            students = students
                .Where(s => s.Info.Subjects.Any(s => s.FullName.Contains(request.Subject, StringComparison.CurrentCultureIgnoreCase)));
        }

        if (!string.IsNullOrEmpty(request.Team))
        {
            students = students
                .Where(s => s.Info.Subjects.Any(s => s.Teams.Any(t => t.Name.Contains(request.Team, StringComparison.CurrentCultureIgnoreCase))));
        }

        return students;
    }
}