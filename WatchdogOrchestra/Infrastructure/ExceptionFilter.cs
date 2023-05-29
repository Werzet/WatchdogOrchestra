using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WatchdogOrchestra.Controllers.Login;

namespace WatchdogOrchestra.Infrastructure
{
	public class ExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			HttpStatusCode statusCode;
			string message;

			if (context.Exception is LoginException loginExc)
			{
				statusCode = HttpStatusCode.Unauthorized;
				message = loginExc.Message;
			}
			else
			{
				string req = context.HttpContext.Request.Method + " " + UriHelper.GetDisplayUrl(context.HttpContext.Request);

				message = $"Ошибка при выполнении запроса {req}. {Environment.NewLine}{context.Exception.Message}";
				statusCode = HttpStatusCode.InternalServerError;
			}

			context.Result = new ObjectResult(message)
			{
				StatusCode = (int)statusCode
			};

			context.HttpContext.Response.StatusCode = (int)statusCode;
		}
	}
}
