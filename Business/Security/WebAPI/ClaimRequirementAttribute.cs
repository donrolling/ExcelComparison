using Business.Services.Membership;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;

namespace Business.Security.WebAPI {
	public class ClaimRequirementAttribute : Attribute, IFilterMetadata {
		public Claim Claim { get; }

		public ClaimRequirementAttribute(NavigationSections navigationSections, ClaimActions claimActions) {
			this.Claim = new Claim(navigationSections.ToString(), claimActions.ToString());
		}

		public ClaimRequirementAttribute(string claimType, string claimValue) {
			this.Claim = new Claim(claimType, claimValue);
		}
	}
}