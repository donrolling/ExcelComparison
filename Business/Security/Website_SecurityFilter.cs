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
	public class Website_SecurityFilter : LoggingWorker, IActionFilter
	{
		public IHostingEnvironment HostingEnvironment { get; }
		private IMembershipService MembershipService { get; }
		public IHostingEnvironment Environment { get; }

		public Website_SecurityFilter(IMembershipService membershipService, IHostingEnvironment environment, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			MembershipService = membershipService;
			Environment = environment;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
		}

		public void OnActionExecuting(ActionExecutingContext actionContext)
		{
			var claimRequirementAttributes = actionContext.ActionDescriptor.FilterDescriptors.Select(x => x.Filter).OfType<ClaimRequirementAttribute>();
			//not equals null just because...not sure that would ever happen
			foreach (var claimRequirementAttribute in claimRequirementAttributes.Where(a => a != null)) {
				var hasClaim = false;
				try {
					hasClaim = this.MembershipService.HasClaim(claimRequirementAttribute.Claim).Result;
				} catch (Exception ex) {
					this.Logger.LogError(ex, $"OnActionExecuting() - Error running this.MembershipService.HasClaim for claim: { claimRequirementAttribute.Claim.Type }|{ claimRequirementAttribute.Claim.Value }");
					var err = HttpErrorMessage.CreateError(this.HostingEnvironment, Messages.SecurityFilter_AccessDenied);
					actionContext.Result = new JsonResult(err);
					return;
				}
				if (!hasClaim) {
					var errorMessage2 = HttpErrorMessage.CreateUnauthorized(this.HostingEnvironment, Messages.SecurityFilter_AccessDenied);
					actionContext.Result = new JsonResult(errorMessage2);
					break;
				}
			}
		}
	}
}