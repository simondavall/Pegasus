using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pegasus.Domain.ProjectTask;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication.Extensions;
using Pegasus.Library.Models;
using TokenOptions = Pegasus.Library.JwtAuthentication.Models.TokenOptions;

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

            services.AddTransient<IProjectsEndpoint, ProjectsEndpoint>();
            services.AddTransient<ITasksEndpoint, TasksEndpoint>();
            services.AddTransient<ICommentsEndpoint, CommentsEndpoint>();

            services.AddSingleton<IApiHelper, ApiHelper>();

            //TODO See if these options can be moved to the JwtAuth method, and pass the configuration
            var tokenOptions = new TokenOptions(
                Configuration["Token:Audience"],
                Configuration["Token:Issuer"],
                Configuration["Token:SigningKey"]);

            services.AddJwtAuthenticationWithProtectedCookie(tokenOptions);

            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
