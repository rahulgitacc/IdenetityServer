using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
            services.AddAuthentication(option =>
            {
                // tell the system to use open id connect by default when we need to authenticate any user
                option.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                // use for actual signin
                option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // tells system to authenticate incoming request using cookie authentication
                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddOpenIdConnect(options =>
                {
                    //tells the system to use open id connect endpoint which is given below
                    options.Authority = "https://login.microsoftonline.com/rahulidentitydemo.onmicrosoft.com";
                    // setting the name of the application
                    options.ClientId = "a0992cf3-4cb3-4ae9-ad53-5f841db62dc7";
                    // setting response type
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    //callback path/ reply url
                    options.CallbackPath = "/auth/signin-callback";
                    options.SignedOutRedirectUri = "https://localhost:44341/";
                    options.TokenValidationParameters.NameClaimType = "name";
                })
                .AddCookie();
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
