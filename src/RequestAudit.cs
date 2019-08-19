using System;
using Firepuma.MongoRepo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Firepuma.HttpRequestAuditing.Mongo
{
    public class RequestAudit : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime Timestamp { get; set; }
        public AuditActor Actor { get; set; }
        public string ActorIp { get; set; }
        public string PersonName { get; set; }

        public string Action { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string Body { get; set; }

        public DateTime? Updated { get; set; }

        public RequestAudit(AuditActor actor, string actorIp, string personName, string action, string path, string query, string body)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Timestamp = DateTime.UtcNow;
            Actor = actor;
            ActorIp = actorIp;
            PersonName = personName;
            Action = action;
            Path = path;
            Query = query;
            Body = body;
        }

        public class AuditActor
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }

            public AuditActor(string id, string email, string fullName)
            {
                Id = id;
                Email = email;
                FullName = fullName;
            }
        }
    }
}