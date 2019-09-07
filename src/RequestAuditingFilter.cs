using System;
using System.Linq;
using System.Threading.Tasks;
using Firepuma.Api.Abstractions.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Firepuma.HttpRequestAuditing.Mongo
{
    public class RequestAuditingFilter : IAsyncActionFilter
    {
        private readonly string[] _allowedMethods;
        private readonly IRequestAuditingService _requestAuditingService;
        private readonly IErrorReportingService _errorReportingService;

        public RequestAuditingFilter(
            IOptions<RequestAuditingFilterOptions> options,
            IRequestAuditingService requestAuditingService,
            IErrorReportingService errorReportingService)
        {
            _allowedMethods = options.Value?.AllowedMethods ?? new[] {"POST", "PUT", "PATCH", "DELETE"};
            _requestAuditingService = requestAuditingService;
            _errorReportingService = errorReportingService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                await AddRequestAuditRecord(context);
            }
            catch (Exception exception)
            {
                _errorReportingService.CaptureException(exception);
            }

            await next();
        }

        private async Task AddRequestAuditRecord(ActionExecutingContext context)
        {
            if (!IsMethodAllowed(context))
            {
                return;
            }

            var actionName = context.ActionDescriptor?.DisplayName;
            var path = context.HttpContext?.Request?.Path.Value;
            var method = context.HttpContext?.Request?.Method;
            var query = context.HttpContext?.Request?.QueryString.Value;

            var bodyParameters = context.ActionDescriptor?.Parameters?.Where(x => x.BindingInfo.BindingSource.Id == "Body").ToArray();
            if (bodyParameters == null || !bodyParameters.Any())
            {
                const string bodyString = (string) null;
                await _requestAuditingService.Add(actionName, path, method, query, bodyString);
            }
            else
            {
                foreach (var bodyParameter in bodyParameters)
                {
                    var bodyString = GetRequestBodyOrDefault(context, bodyParameter);
                    await _requestAuditingService.Add(actionName, path, method, query, bodyString);
                }
            }
        }

        private bool IsMethodAllowed(ActionContext context)
        {
            var method = context.HttpContext?.Request?.Method?.ToUpper();
            return _allowedMethods.Contains(method);
        }

        private static string GetRequestBodyOrDefault(ActionExecutingContext context, ParameterDescriptor bodyParameter)
        {
            if (string.IsNullOrWhiteSpace(bodyParameter.Name))
            {
                return null;
            }

            if (!context.ActionArguments.TryGetValue(bodyParameter.Name, out var value))
            {
                return null;
            }

            return JsonConvert.SerializeObject(value);
        }
    }
}