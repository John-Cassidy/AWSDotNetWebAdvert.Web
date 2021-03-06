using AWSDotNetWebAdvert.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AWSDotNetWebAdvert.Web.ServiceClients;
using Polly;
using System.Net.Http;
using Polly.Extensions.Http;
using System.Net;

namespace AWSDotNetWebAdvert.Web {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddCognitoIdentity(config => {
                config.Password = new Microsoft.AspNetCore.Identity.PasswordOptions {
                    RequireDigit = false,
                    RequiredLength = 6,
                    RequiredUniqueChars = 0,
                    RequireLowercase = false,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false
                };
            });

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Accounts/Login";
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddTransient<IFileUploader, S3FileUploader>();

            services.AddHttpClient<IAdvertApiClient, AdvertApiClient>().AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPatternPolicy());

            services.AddHttpClient<ISearchApiClient, SearchApiClient>().AddPolicyHandler(GetRetryPolicy())
                            .AddPolicyHandler(GetCircuitBreakerPatternPolicy());

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
            return HttpPolicyExtensions.HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
        private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPatternPolicy() {
            return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }
    }
}
