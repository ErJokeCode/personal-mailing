using System.Threading.Tasks;
using Chatter.Models;

namespace Chatter.Abstractions.UserAccessor;

public interface IUserAccessor
{
    public Task<Admin?> GetUserAsync();
}