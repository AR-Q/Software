using System.Threading.Tasks;

namespace CloudHosting.Core.Interfaces
{
    public interface IIamService
    {
        Task<string> GetUserIdFromTokenAsync(string token);
    }
}