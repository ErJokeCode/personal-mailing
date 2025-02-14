using System.Threading.Tasks;
using Core.Models;

namespace Core.Abstractions.UserAccesor;

public interface IUserAccessor
{
    public Task<Admin?> GetUserAsync();
}