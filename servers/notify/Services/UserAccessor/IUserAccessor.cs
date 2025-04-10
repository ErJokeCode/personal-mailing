using System.Threading.Tasks;
using Notify.Models;

namespace Notify.Services.UserAccessor;

public interface IUserAccessor
{
    public Task<Admin?> GetUserAsync();
}