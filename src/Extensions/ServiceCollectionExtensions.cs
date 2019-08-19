using System;
using Firepuma.Api.Abstractions.Actor;
using Firepuma.Api.Abstractions.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.HttpRequestAuditing.Mongo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRequestAuditingService<TActor>(this IServiceCollection services) where TActor : IActorIdentity
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                if (scope.ServiceProvider.GetService<IErrorReportingService>() == null)
                {
                    throw new Exception($"Please register IErrorReportingService service before calling {nameof(AddRequestAuditingService)}");
                }
                if (scope.ServiceProvider.GetService<IActorProvider<TActor>>() == null)
                {
                    throw new Exception($"Please register IActorProvider<TActor> service before calling {nameof(AddRequestAuditingService)}");
                }

                services.AddScoped<IRequestAuditingService, RequestAuditingService<TActor>>();
            }
        }
    }
}