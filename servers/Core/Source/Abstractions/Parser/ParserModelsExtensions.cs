using System.Collections.Generic;
using System.Text.Json;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Abstractions.Parser;

// TODO add a compare operator on conversion, to get rid of the warning

public static class ParserModelsExtensions
{
    public static void BuildParserModels(this ModelBuilder builder)
    {
        builder.Entity<Student>().OwnsOne(s => s.Info, i =>
        {
            i.OwnsMany(i => i.OnlineCourse, o =>
            {
                o.Property(x => x.Scores).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null!)!
                );

            });
        });

        builder.Entity<Student>().OwnsOne(s => s.Info, i =>
        {
            i.OwnsMany(i => i.Subjects, s =>
            {
                s.OwnsOne(s => s.OnlineCourse, o =>
                {
                    o.Property(o => o.Scores).HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null!)!
                    );
                });
            });
        });
    }
}
