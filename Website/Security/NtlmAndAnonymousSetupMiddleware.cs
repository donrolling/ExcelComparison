using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Website.Security
{
	public class NtlmAndAnonymousSetupMiddleware
	{
		private readonly RequestDelegate next;

		public NtlmAndAnonymousSetupMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			if (context.User.Identity.IsAuthenticated) {
				await next(context);
				return;
			}

			var path = context.Request.Path.ToString();
			if (path.StartsWith("/api/")) {
				await next(context);
				return;
			}

			await context.ChallengeAsync("Windows");
		}
	}
}