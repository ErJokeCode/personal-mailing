using System.Threading.Tasks;
using Core.Models;

namespace Core.Services.UserAccessor;

public interface IUserAccessor
{
    public Task<Admin?> GetUserAsync();
}