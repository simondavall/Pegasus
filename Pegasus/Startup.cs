using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pegasus.Library.Api;
using Pegasus.Library.Api.Extensions;
using Pegasus.Library.JwtAuthentication.Extensions;
using Pegasus.Library.Services.Http;
using Pegasus.Services;

namespace Pegasus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITaskFilterService, TaskFilterService>();

            services.AddApiEndpoints();

            services.AddHttpContextAccessor();
            services.AddScoped<IHttpContextWrapper, HttpContextWrapper>();
            services.AddSingleton<IApiHelper, ApiHelper>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IMarketingService, MarketingService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();


            services.AddJwtAuthenticationWithProtectedCookie(Configuration);

            services.AddScoped<ISignInManager, SignInManager>();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pegasus v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=TaskList}/{action=Index}/{id?}");
            });
        }
    }
}
