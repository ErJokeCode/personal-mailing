using System;

namespace Shared.Context.Documents;

public static class DocumentErrors
{
    public static string NotFound(Guid id) => $"Документ с айди {id} не был найден";
}