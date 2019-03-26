using Business.Interfaces;
using Business.Security.WebAPI;
using Common.BaseClasses;
using Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Business.Security
{
	public class API_SecurityFilter : LoggingWorker, IActionFilter
	{
		public IHostingEnvironment HostingEnvironment { get; }
		private IWebAPIMembershipService WebAPIMembershipService { get; }

		public API_SecurityFilter(IHostingEnvironment hostingEnvironment, IWebAPIMembershipService webAPIMembershipService, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			HostingEnvironment = hostingEnvironment;
			WebAPIMembershipService = webAPIMembershipService;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
		}

		public void OnActionExecuting(ActionExecutingContext actionContext)
		{
			var claimRequirementAttributes = actionContext.ActionDescriptor.FilterDescriptors.Select(x => x.Filter).OfType<ClaimRequirementAttribute>();
			foreach (var claimRequirementAttribute in claimRequirementAttributes.Where(a => a != null)) {
				this.Logger.LogInformation($"API_SecurityFilter OnActionExecuting(): { claimRequirementAttribute.Claim.Type }|{ claimRequirementAttribute.Claim.Value }");
				var hasClaim = false;
				if (claimRequirementAttribute.Claim.Value == ClaimActions.None.ToString()) {
					return;
				}
				try {
					hasClaim = this.WebAPIMembershipService.HasClaim(claimRequirementAttribute.Claim).Result;
				} catch (Exception ex) {
					this.Logger.LogError(ex, $"OnActionExecuting() - Error running this.WebAPIMembershipService.HasClaim for claim: { claimRequirementAttribute.Claim.Type }|{ claimRequirementAttribute.Claim.Value }");
					var err = HttpErrorMessage.CreateError(this.HostingEnvironment, Messages.SecurityFilter_AccessDenied);
					actionContext.Result = new JsonResult(err);
					return;
				}
				if (!hasClaim) {
					this.Logger.LogError($"OnActionExecuting() - user did not have the claim: { claimRequirementAttribute.Claim.Type }|{ claimRequirementAttribute.Claim.Value }");
					var errorMessage2 = HttpErrorMessage.CreateUnauthorized(this.HostingEnvironment, Messages.SecurityFilter_AccessDenied);
					actionContext.Result = new JsonResult(errorMessage2);
					break;
				}
			}
		}
	}
}