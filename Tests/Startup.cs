using Business.Interfaces;
using Common.Interfaces;
using Common.IO;
using Common.Services;
using Common.Web.Interfaces;
using Common.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using System;
using Tests.Models;
using Tests.Services;
using Tests.Utilities;

namespace Tests
{
	public class Startup
	{

		internal IServiceProvider Setup()
		{
			var pathToNLogConfig = FileUtility.GetFullPath_FromRelativePath<Startup>("nlog.config");
			var pathToAppSettingsConfig = FileUtility.GetFullPath_FromRelativePath<Startup>("appsettings.json");
			var provider = new PhysicalFileProvider(pathToNLogConfig.path);
			LogManager.Configuration = new XmlLoggingConfiguration(pathToNLogConfig.filePath);
			var config = new ConfigurationBuilder().AddJsonFile(pathToAppSettingsConfig.filePath).Build();
			var services = new ServiceCollection();
			services.Configure<AppSettings>(config.GetSection("AppSettings"));
			var loggerFactory = new LoggerFactory().AddNLog();
			services.AddSingleton<ILoggerFactory>(loggerFactory);
			services.AddSingleton<IFileProvider>(provider);			

			return services.BuildServiceProvider();
		}
	}
}