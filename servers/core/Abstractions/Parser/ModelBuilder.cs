using System;
using System.Text.Json;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Abstractions.Parser;

public static class ParserModelBuilder
{
    public static void BuildParserModels(this ModelBuilder builder)
    {
        System.Linq.Expressions.Expression<Func<ParserStudent, string>> convertToProviderExpression = v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!);
        System.Linq.Expressions.Expression<Func<string, ParserStudent>> convertFromProviderExpression = v => JsonSerializer.Deserialize<ParserStudent>(v, (JsonSerializerOptions)null!)!;

        ValueComparer<ParserStudent> valueComparer = new(
            (c1, c2) => false,
            c => c.GetHashCode(),
            c => c
        );

        builder.Entity<Student>().Property(s => s.Info).HasConversion(
            convertToProviderExpression,
            convertFromProviderExpression,
            valueComparer
        );
    }
}