using System.Threading.Tasks;
using Chatter.Models;

namespace Chatter.Services.UserAccessor;

public interface IUserAccessor
{
    public Task<Admin?> GetUserAsync();
}