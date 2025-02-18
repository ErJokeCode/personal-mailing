using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Core.Routes.Admins.Errors;

public static class AdminErrors
{
    public static string NotFound() => $"Админ не был найден";

    public static string NotFound(Guid id) => $"Админ с айди {id} не был найден";

    public static string AlreadyExists(string email) => $"Админ с почтой {email} уже существует";

    public static string LoginError() => $"Почта или пароль не совпадают";

    public static string RegisterError(IEnumerable<IdentityError> errors)
        => $"Произошла ошибка при регистриции: {string.Join(", ", errors.Select(e => e.Description))}";
}