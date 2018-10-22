using System;
using System.Threading.Tasks;
using AppName.Web.GraphQL;
using AppName.Web.Models;
using AppName.Web.Providers;
using AppName.Web.Repositories;
using AppName.Web.Services;
using AppName.Web.Transport;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace AppName.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));
//            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));

//            var auth0Settings = Configuration.GetSection("Auth0");

            // Add authentication services
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect("Auth0", options =>
                {
                    // Set the authority to your Auth0 domain
                    options.Authority = $"https://{Configuration["Auth0:Domain"]}";

                    // Configure the Auth0 Client ID and Client Secret
                    options.ClientId = Configuration["Auth0:ClientId"];
                    options.ClientSecret = Configuration["Auth0:ClientSecret"];

                    // Set response type to code
                    options.ResponseType = "code";

                    // Configure the scope
                    options.Scope.Clear();
                    options.Scope.Add("openid email profile");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                    };

                    // Set the callback path, so Auth0 will call back to http://localhost:3000/signin-auth0
                    // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
                    options.CallbackPath = new PathString("/signin-auth0");

                    // Configure the Claims Issuer to be Auth0
                    options.ClaimsIssuer = "Auth0";

                    // Saves tokens to the AuthenticationProperties
                    options.SaveTokens = true;

                    options.Events = new OpenIdConnectEvents
                    {
                        // handle the logout redirection 
                        OnRedirectToIdentityProviderForSignOut = (context) =>
                        {
                            var logoutUri =
                                $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

                            var postLogoutUri = context.Properties.RedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = context.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase +
                                                    postLogoutUri;
                                }

                                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                            }

                            context.Response.Redirect(logoutUri);
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

//            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//                .AddCookie(options =>
//                {
//                    options.AccessDeniedPath = "/Common/AccessDenied";
//                    options.LoginPath = "/account/login";
//                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("GraphQL", policy =>
                    policy.Requirements.Add(new NoOpRequirement()));
            });

            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<AccrualFrequencyEnum>();
            services.AddSingleton<EndingEnum>();
            services.AddSingleton<AccrualActionEnum>();
            services.AddSingleton<AccrualGraphType>();
            services.AddSingleton<AirportGraphType>();
            services.AddSingleton<HealthCheckGraphType>();
            services.AddSingleton<RoleGraphType>();
            services.AddSingleton<UserGraphType>();
            services.AddSingleton<AccrualActionRecordInputType>();
            services.AddSingleton<AccrualActionRecordGraphType>();
            services.AddSingleton<AccrualRowGraphType>();
            services.AddSingleton<AccrualInputType>();
            services.AddSingleton<AirportInputType>();
            services.AddSingleton<ISchema, AppSchema>();
            services.AddSingleton<AccrualService>();
            services.AddSingleton<AppQuery>();
            services.AddSingleton<AppMutation>();
            services.AddSingleton<IAirportRepository, AirportRepository>();
            services.AddSingleton<IDashboardRepository, MongoDashboardRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IAuthorizationHandler, NoOpHandler>();

            services.AddSingleton<IUserRepository, MongoUserRepository>();
            services.AddScoped<IRoleRepository, MongoRoleRepository>();

            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
            });
            //.AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User });

//            services.AddSingleton<IUserStore<MongoIdentityUser>>(provider =>
//            {
//                var database = provider.GetService<IMongoDatabase>();
//                return new MongoUserStore<MongoIdentityUser>(database);
//            });

            services.AddSingleton<IMongoDatabase>(provider =>
            {
                var pack = new ConventionPack {new CamelCaseElementNameConvention()};
                ConventionRegistry.Register("camelcase", pack, t => true);

                var options = provider.GetService<IOptions<MongoDbSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var database = client.GetDatabase(options.Value.DatabaseName);

                return database;
            });

//            services.AddScoped<MongoRoleRepository>(provider =>
//            {
//                var database = provider.GetService<IMongoDatabase>();
//                var repository = new MongoRoleRepository(database);
//
//                return repository;
//            });

            services.AddSingleton<IRoleRepository, MongoRoleRepository>();

            services.AddSingleton<IMigrationRepository, MongoMigrationRepository>();
            services.AddSingleton<IMigrationDiscoverService, DefaultMigrationDiscoverService>();
            services.AddSingleton<IDotNetProvider, DefaultDotNetProvider>();
            services.AddSingleton<IMigrationService, MongoMigrationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMigrationService migrationService)
        {
            migrationService.ApplyMigrationsAsync().GetAwaiter().GetResult();

            if (env.IsDevelopment())
            {
                // When the app runs in the Development environment:
                //   Use the Developer Exception Page to report app runtime errors.
                //   Use the Database Error Page to report database runtime errors.
                app.UseDeveloperExceptionPage();
//                app.UseDatabaseErrorPage();
            }
            else
            {
                // When the app doesn't run in the Development environment:
                //   Enable the Exception Handler Middleware to catch exceptions
                //     thrown in the following middlewares.
                //   Use the HTTP Strict Transport Security Protocol (HSTS)
                //     Middleware.
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Use HTTPS Redirection Middleware to redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Return static files and end the pipeline.
            app.UseStaticFiles();

            // Use Cookie Policy Middleware to conform to EU General Data 
            // Protection Regulation (GDPR) regulations.
            app.UseCookiePolicy();

            // Authenticate before the user accesses secure resources.
            app.UseAuthentication();


            //// If the app uses session state, call Session Middleware after Cookie 
            //// Policy Middleware and before MVC Middleware.
            //app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // require an authenticated user for anything else in the pipeline after this point
            // this verifies that the requester is authenticated and satisfies the policy named GraphQL
            // for now, this protects the /graphql and /ui/playground routes
            app.Use(async (context, next) =>
            {
                if (context.User?.Identity?.IsAuthenticated == false)
                {
                    string redirect = "/account/login";
                    if (context.Request.Path == "/ui/playground")
                    {
                        redirect += "?ReturnUrl=%2Fui%2Fplayground";
                    }

                    context.Response.Redirect(redirect, false);
                }
                else
                {
                    if (context.Request.Path == "/graphql")
                    {
                        await next();
                    }
                    else
                    {
                        var authService = context.RequestServices.GetRequiredService<IAuthorizationService>();

                        AuthorizationResult result = await authService.AuthorizeAsync(context.User, context, "GraphQL");
                        if (result.Succeeded)
                        {
                            await next();
                        }
                        else
                        {
                            await context.Response.WriteAsync("Unauthorized.");

//                        string redirect = "/account/login";
//                        if (context.Request.Path == "/ui/playground")
//                        {
//                            redirect += "?ReturnUrl=%2Fui%2Fplayground";
//                        }
//
//                        context.Response.Redirect(redirect, false);

                            //throw new InvalidOperationException("Policy is missing.");
                        }
                    }
                }
            });

            // add http for Schema at default url /graphql
            app.UseGraphQL<ISchema>("/graphql");

            // use graphql-playground at default url /ui/playground
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                Path = "/ui/playground"
            });

//            var section = Configuration.GetSection("RECAPTCHA_PRIVATE");
//            Console.WriteLine($"{section.Key} --- {section.Value}");

//            foreach(var kvp in Configuration.GetChildren())
//            {
//                Console.WriteLine($"{kvp.Key}:{ kvp.Value}");
//            }
        }
    }
}