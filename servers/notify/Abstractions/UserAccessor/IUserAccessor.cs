using System.Threading.Tasks;
using Notify.Models;

namespace Notify.Abstractions.UserAccessor;

public interface IUserAccessor
{
    public Task<Admin?> GetUserAsync();
}