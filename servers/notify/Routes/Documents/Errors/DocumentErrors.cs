using System;

namespace Notify.Routes.Documents.Errors;

public static class DocumentErrors
{
    public static string NotFound(Guid id) => $"Документ с айди {id} не был найден";

}