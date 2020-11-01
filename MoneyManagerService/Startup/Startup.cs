using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using MoneyManagerService.Models.Settings;
using MoneyManagerService.Startup.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using MoneyManagerService.Startup.ApplicationBuilderExtensions;
using MoneyManagerService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MoneyManagerService.Core;
using Microsoft.AspNetCore.Identity;
using MoneyManagerService.Models.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using AutoMapper;
using MoneyManagerService.Data.Repositories;
using Microsoft.AspNetCore.HttpOverrides;
using MoneyManagerService.Middleware;

namespace MoneyManagerService.Startup
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
            services.AddControllers(options =>
            {
                // This allows for global authorization. No need to have [Authorize] attribute on controllers with this.
                // This is what requires tokens for all endpoints. Add [AllowAnonymous] to any endpoint not requiring tokens (like login)
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = ctx => new ValidationProblemDetailsResult();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // Add the MySQL database context with connection info set in appsettings.json
            services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection"), options => options.EnableRetryOnFailure()));

            // Configure identity
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
            });

            services.Configure<AuthenticationSettings>(Configuration.GetSection("Authentication"));
            services.Configure<SwaggerSettings>(Configuration.GetSection("Swagger"));

            ConfigureAuthentication(services);
            ConfigureHealthChecks(services);
            services.AddHangfireServices(Configuration);
            services.AddSwaggerServices(Configuration);

            services.AddCors();

            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfiles());
            });
            services.AddSingleton(mapperConfiguration.CreateMapper());

            //Must add repos here so they can be injected. Interface => concrete implementation
            services.AddScoped<UserRepository>();

            services.AddLogging();

            services.AddTransient<Seeder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAndConfigureSwagger(env);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(header =>
                header.WithOrigins(Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "X-Token-Expired" })
            );
            app.UseExceptionHandler(builder => builder.UseMiddleware<GlobalExceptionHandlerMiddleware>());
            app.UseHsts();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/health/db", new HealthCheckOptions()
                {
                    Predicate = check => check.Tags.Contains("db"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard(new DashboardOptions
                {
                    Authorization = new[] { new DashboardAuthorizationFilter(Configuration) }
                });
            });

            // backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            var authSettings = Configuration.GetSection("Authentication").Get<AuthenticationSettings>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Set token validation options. These will be used when validating all tokens.
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.APISecrect)),
                        RequireSignedTokens = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidAudience = authSettings.TokenAudience,
                        ValidIssuers = new string[] { authSettings.TokenIssuer }
                    };

                    options.Events = new JwtBearerEvents
                    {
                        // Add custom responses when token validation fails.
                        OnChallenge = context =>
                        {
                            // Skip the default logic.
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            string errorMessage = string.IsNullOrWhiteSpace(context.ErrorDescription) ? context.Error : $"{context.Error}. {context.ErrorDescription}.";

                            var problem = new ProblemDetailsWithErrors(errorMessage ?? "Invalid token", 401, context.Request);

                            var settings = new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            };

                            return context.Response.WriteAsync(JsonConvert.SerializeObject(problem, settings));
                        },
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("X-Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        private void ConfigureHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<DataContext>(tags: new[] { "db" });
        }

        private class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
        {
            private readonly IConfiguration configuration;

            public DashboardAuthorizationFilter(IConfiguration configuration)
            {
                this.configuration = configuration;
            }

            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                string authHeader = httpContext.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Get the encoded username and password
                    var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

                    // Decode from Base64 to string
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    // Split username and password
                    var username = decodedUsernamePassword.Split(':', 2)[0];
                    var password = decodedUsernamePassword.Split(':', 2)[1];

                    // Check if login is correct
                    if (username == configuration["Hangfire:DashboardUsername"] && password == configuration["Hangfire:DashboardPassword"])
                    {
                        return true;
                    }
                }

                // Return authentication type (causes browser to show login dialog)
                httpContext.Response.Headers["WWW-Authenticate"] = "Basic";

                // Return unauthorized
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return false;
            }
        }
    }
}
