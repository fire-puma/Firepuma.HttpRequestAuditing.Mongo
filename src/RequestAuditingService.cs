using System.Threading.Tasks;
using Firepuma.Api.Abstractions.Actor;
using Firepuma.MongoRepo.Generic;

namespace Firepuma.HttpRequestAuditing.Mongo
{
    public class RequestAuditingService<TActor> : IRequestAuditingService where TActor : IActorIdentity
    {
        private readonly IActorProviderHolder<TActor> _actorProviderHolder;
        private readonly IRemoteIpProvider _remoteIpProvider;
        private readonly IRepository<RequestAudit> _requestAuditRepository;

        public RequestAuditingService(
            IActorProviderHolder<TActor> actorProviderHolder,
            IRemoteIpProvider remoteIpProvider,
            IRepository<RequestAudit> requestAuditRepository)
        {
            _actorProviderHolder = actorProviderHolder;
            _remoteIpProvider = remoteIpProvider;
            _requestAuditRepository = requestAuditRepository;
        }

        public async Task Add(string action, string path, string query, string body)
        {
            var actor = await _actorProviderHolder.Provider.GetActor();
            var remoteIp = _remoteIpProvider.GetRemoteIp();

            var auditActor = new RequestAudit.AuditActor(actor.Id, actor.Email, actor.FullName);
            var record = new RequestAudit(auditActor, remoteIp, actor.FullName, action, path, query, body);

            await _requestAuditRepository.Add(record);
        }
    }
}