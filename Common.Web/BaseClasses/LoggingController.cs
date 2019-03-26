using Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Common.Web.BaseClasses {
	public class LoggingController : Controller {
		public IHostingEnvironment HostingEnvironment { get; }
		public ILogger Logger { get; }
		public ILoggerFactory LoggerFactory { get; set; }

		public LoggingController(ILoggerFactory loggerFactory, IHostingEnvironment hostingEnvironment) {
			this.LoggerFactory = loggerFactory;
			this.HostingEnvironment = hostingEnvironment;
			Logger = LogUtility.GetLogger(this.LoggerFactory, this.GetType());
		}
	}
}