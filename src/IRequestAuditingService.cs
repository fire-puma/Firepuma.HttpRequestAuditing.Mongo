using System.Threading.Tasks;

namespace Firepuma.HttpRequestAuditing.Mongo
{
    public interface IRequestAuditingService
    {
        Task Add(string action, string path, string method, string query, string body);
    }
}
