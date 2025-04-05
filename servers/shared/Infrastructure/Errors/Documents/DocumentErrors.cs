using System;

namespace Shared.Infrastructure.Errors.Documents;

public static class DocumentErrors
{
    public static string NotFound(Guid id) => $"Документ с айди {id} не был найден";

}