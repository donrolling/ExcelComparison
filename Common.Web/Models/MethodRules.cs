using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System;

namespace Common.Web.Models {
	public class MethodRules {

		public static void RedirectXMLRequests(RewriteContext context) {
			var request = context.HttpContext.Request;

			// Because we're redirecting back to the same app, stop
			// processing if the request has already been redirected
			if (request.Path.StartsWithSegments(new PathString("/xmlfiles"))) {
				return;
			}

			if (request.Path.Value.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) {
				var response = context.HttpContext.Response;
				response.StatusCode = StatusCodes.Status301MovedPermanently;
				context.Result = RuleResult.EndResponse;
				response.Headers[HeaderNames.Location] = "/xmlfiles" + request.Path + request.QueryString;
			}
		}
	}
}