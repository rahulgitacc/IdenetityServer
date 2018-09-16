using AspNetCore.Auth.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace AspNetCore.Auth.Web
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
            services.AddMvc(options =>
            {
                // ensure that the application should use HTTPS
                options.Filters.Add(new RequireHttpsAttribute());
            });
            var users = new Dictionary<string, string> { { "Rahul", "Password" }, { "Chris", "Password" } };
            services.AddSingleton<IUserService>(new DummyUserService(users));
            services.AddAuthentication(options =>
            {
                // specify that for authentication we need to use cookie
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // by default after the authentication we will authenticate the user using cookie
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // if the user ever need to authenticate it will ask for the cookie to do it
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/auth/signin";
                    options.Cookie.Name = "IdentityServer";
                });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
            // Rewrite rule for redirect the user to https if anyone request for http
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(301, 44341));
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
