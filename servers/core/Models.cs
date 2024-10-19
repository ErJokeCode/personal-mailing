using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Shared.Core;

namespace Core.Models;

public class CoreDb : DbContext
{
    public DbSet<Student> Students => Set<Student>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }
}

public class AuthDetails
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("personal_number")]
    public string PersonalNumber { get; set; }
    [JsonPropertyName("chat_id")]
    public string ChatId { get; set; }
}
