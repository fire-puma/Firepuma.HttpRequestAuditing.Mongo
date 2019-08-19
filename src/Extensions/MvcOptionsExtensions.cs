using Microsoft.AspNetCore.Mvc;

namespace Firepuma.HttpRequestAuditing.Mongo.Extensions
{
    public static class MvcOptionsExtensions
    {
        public static void ConfigureMvcRequestAuditing(this MvcOptions options)
        {
            options.Filters.Add(typeof(RequestAuditingFilter));
        }
    }
}