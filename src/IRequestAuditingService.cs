using System.Threading.Tasks;

namespace Firepuma.HttpRequestAuditing.Mongo
{
    public interface IRequestAuditingService
    {
        Task Add(string action, string path, string query, string body);
    }
}
