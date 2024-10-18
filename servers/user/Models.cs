using Microsoft.EntityFrameworkCore;

namespace User.Models;

public class UserDb : DbContext
{
    public DbSet<Student> Students => Set<Student>();

    public UserDb(DbContextOptions<UserDb> options) : base(options)
    {
    }
}

public class UserCourse
{
    public string _id { get; set; }
    public string name { get; set; }
    public string sername { get; set; }
    public string patronymic { get; set; }
}

public class User
{
    public string _id { get; set; }
    public string personal_number { get; set; }
}

public class Student
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PersonalNumber { get; set; }
    public string ChatId { get; set; }

    public string UserId { get; set; }
    public string UserCourseId { get; set; }
}
