using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tests.Models {
	public class FakeHttpContextAccessor : IHttpContextAccessor {
		public HttpContext HttpContext {
			get {
				if (this._httpContext != null) { return this._httpContext; }
				this._httpContext = new DefaultHttpContext();
				this._httpContext.User = new ClaimsPrincipal();
				var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
				var claimsIdentity = new ClaimsIdentity(windowsIdentity.Claims);
				this._httpContext.User.AddIdentity(claimsIdentity);
				return this._httpContext;
			}
			set {
				this._httpContext = value;
			}
		}
		private HttpContext _httpContext = null;
	}
}