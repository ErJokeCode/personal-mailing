using System.Collections.Generic;

namespace Core.Models;

public interface IMappable<TDto, TOrig>
{
    public static abstract TDto Map(TOrig orig);
    public static abstract List<TDto> Maps(List<TOrig> origs);
}

public abstract class BaseStatus
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
}

public static class BaseStatusExtensions
{
    public static void SetLost(this BaseStatus status)
    {
        status.Code = -1;
        status.Short = "Lost";
    }

    public static void SetSent(this BaseStatus status)
    {
        status.Code = 0;
        status.Short = "Sent";
    }

    public static void SetRead(this BaseStatus status)
    {
        status.Code = 1;
        status.Short = "Read";
    }

    public static bool Set(this BaseStatus status, int code)
    {
        switch (code)
        {
            case -1:
                status.Short = "Lost";
                break;

            case 0:
                status.Short = "Sent";
                break;

            case 1:
                status.Short = "Read";
                break;

            default:
                return false;
        }

        status.Code = code;
        return true;
    }
}
