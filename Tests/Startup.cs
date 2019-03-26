using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices;
using Business.Services.Membership;
using Common.Interfaces;
using Common.IO;
using Common.Services;
using Common.Web.Interfaces;
using Common.Web.Services;
using Data.Repository.Dapper;
using Data.Repository.Interfaces;
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
			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddJsonOptions(options =>
					options.SerializerSettings.ContractResolver = new DefaultContractResolver()
				);
			//AppCache
			services.AddMemoryCache();
			//SessionCache
			services.AddDistributedMemoryCache();
			services.AddSession(options => {
				options.Cookie.HttpOnly = true;
			});
			services.AddMvc(options => {
				options.Filters.Add(new RequireHttpsAttribute());
			}).AddJsonOptions(options =>
				options.SerializerSettings.ContractResolver = new DefaultContractResolver()
			);
			services.AddSingleton<IHttpContextAccessor, FakeHttpContextAccessor>();

			//our services
			services.AddTransient<IAppCacheService, AppCacheService>();
			services.AddTransient<ISessionCacheService, SessionCacheService>();
			services.AddTransient<IMembershipService, Test_MembershipService>();

			//GENERATED
			services.AddTransient<IPermissionRepository, PermissionDapperRepository>();
			services.AddTransient<IPermissionService, PermissionService>();
			services.AddTransient<IRoleRepository, RoleDapperRepository>();
			services.AddTransient<IRoleService, RoleService>();
			services.AddTransient<IRolePermissionRepository, RolePermissionDapperRepository>();
			services.AddTransient<IRolePermissionService, RolePermissionService>();
			services.AddTransient<IUserRepository, UserDapperRepository>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IUserRoleRepository, UserRoleDapperRepository>();
			services.AddTransient<IUserRoleService, UserRoleService>();

			//windows service
			var serviceProvider = services.BuildServiceProvider();
			var appSettings = serviceProvider.GetService<IOptions<AppSettings>>();

			return services.BuildServiceProvider();
		}
	}
}