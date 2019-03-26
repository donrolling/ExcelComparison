using Business.Interfaces;
using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Application;
using System;

namespace Tests.Models {
	[TestClass]
	public class TestBase {
		public IOptions<AppSettings> AppSettings { get; }
		public ILogger Logger { get; }
		public ILoggerFactory LoggerFactory { get; }
		public IServiceProvider ServiceProvider { get; }
		public TestContext TestContext { get; set; }
		public string TestName {
			get {
				return this.TestContext.TestName;
			}
		}

		public TestBase() {
			this.ServiceProvider = new Startup().Setup();
			this.LoggerFactory = this.ServiceProvider.GetService<ILoggerFactory>();
			Logger = LogUtility.GetLogger(this.ServiceProvider, this.GetType());
			this.AppSettings = this.ServiceProvider.GetService<IOptions<AppSettings>>();
		}
	}
}