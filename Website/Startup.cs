using Business.Interfaces;
using Business.Security;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices;
using Business.Services.Membership;
using Common.Interfaces;
using Common.Services;
using Common.Web.Interfaces;
using Common.Web.Services;
using Data.Repository.Dapper;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Models.Application;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog.Web;
using Website.Security;

namespace Website
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		private IHostingEnvironment _hostingEnvironment;

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
			_hostingEnvironment = env;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddNLog();
			env.ConfigureNLog("nlog.config");
			var logger = loggerFactory.CreateLogger($"Website.Startup");
			logger.LogInformation($"EnvironmentName: { env.EnvironmentName }");

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseSession();

			app.UseMiddleware<NtlmAndAnonymousSetupMiddleware>();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				routes.MapRoute(
					name: "defaultapi",
					template: "api/{controller}/{action}/{id?}");
			});
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<FormOptions>(options => options.BufferBody = true);
			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
			services.AddSingleton<IFileProvider>(_hostingEnvironment.ContentRootFileProvider);
			services.AddAuthentication(Website_MembershipService.AuthCookie)
						.AddCookie(Website_MembershipService.AuthCookie, options =>
						{
							options.AccessDeniedPath = "/unauthorized";
							options.LoginPath = "/unauthorized";
						});
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddJsonOptions(options =>
					options.SerializerSettings.ContractResolver = new DefaultContractResolver()
				);
			services.AddSignalR();
			//AppCache
			services.AddMemoryCache();
			//SessionCache
			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				//options.Cookie.HttpOnly = true makes the cookie inaccesible to JS. This is good.
				options.Cookie.HttpOnly = true;
			});
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<API_SecurityFilter>();
			services.AddScoped<Website_SecurityFilter>();

			//base services
			services.AddScoped<IAppCacheService, AppCacheService>();
			services.AddScoped<ISessionCacheService, SessionCacheService>();
			services.AddScoped<IJWTService, JWTService>();
			//IWebsiteMembershipService and IWebAPIMembershipService both
			//	inherit from IMembershipService
			//	this allows me to replace IWebAPIMembershipService for IWebsiteMembershipService
			//	when I need to
			//I'm going to default IMembershipService to Website_MembershipService so that
			//	I don't have to change anything much
			services.AddScoped<IMembershipService, Website_MembershipService>();
			services.AddScoped<IWebsiteMembershipService, Website_MembershipService>();
			services.AddScoped<IWebAPIMembershipService, WebAPI_MembershipService>();

			//GENERATED
			services.AddScoped<IPermissionRepository, PermissionDapperRepository>();
			services.AddScoped<IPermissionService, PermissionService>();
			services.AddScoped<IRoleRepository, RoleDapperRepository>();
			services.AddScoped<IRoleService, RoleService>();
			services.AddScoped<IRolePermissionRepository, RolePermissionDapperRepository>();
			services.AddScoped<IRolePermissionService, RolePermissionService>();
			services.AddScoped<IUserRepository, UserDapperRepository>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IUserManagementService, UserManagementService>();
			services.AddScoped<IUserRoleRepository, UserRoleDapperRepository>();
			services.AddScoped<IUserRoleService, UserRoleService>();
			
			//special
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IUserRepository, UserDapperRepository>();

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
		}
	}
}