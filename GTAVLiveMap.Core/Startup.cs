using FluentMigrator.Runner;
using GTAVLiveMap.Core.Hubs;
using GTAVLiveMap.Core.Infrastructure.Attributes;
using GTAVLiveMap.Core.Infrastructure.Authorization;
using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Core.Infrastructure.Mapper.Base;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using GTAVLiveMap.Core.Infrastructure.Services;
using GTAVLiveMap.Core.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace GTAVLiveMap.Core
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
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c
                    .AddPostgres11_0()
                    .WithGlobalConnectionString(Configuration.GetConnectionString("PostgreSQLDocker") + $"Database={Configuration["PostgreConfig:DataBaseName"]}")
                    .ScanIn(typeof(InitDataBase).Assembly).For.Migrations())
                .AddLogging(l => l.AddFluentMigratorConsole());

            services.AddSingleton<DbContext>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IMapRepository, MapRepository>();
            services.AddSingleton<ISessionKeyRepository, SessionKeyRepository>();
            services.AddSingleton<IMapMemberRepository, MapMemberRepository>();
            services.AddSingleton<IInviteRepository, InviteRepository>();

            services.AddSingleton<IGoogleService, GoogleService>();

            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationOptions, AuthenticationHandler>("Basic", null);

            services.AddAutoMapper(Assembly.GetAssembly(typeof(MapperConfigurationBase)));

            services.AddControllers();
            services.AddSignalR();
            services.AddSwaggerDocument(c => c.PostProcess = d =>
            {
                d.Info.Version = "v1";
                d.Info.Title = "GTAV-Live-Map API";
                d.Info.Description = "A web API for gtavlivemap.com";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IMapMemberRepository memberRepository,
            IUserRepository userRepository,
            ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.Use(next => async context =>
            {
                try
                {
                    var endpoint = context.GetEndpoint();

                    var controllerDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                    var controllerType = controllerDescriptor.ControllerTypeInfo;

                    var actionType = controllerType.GetMethod(controllerDescriptor.ActionName);

                    var atribute = actionType.GetCustomAttribute<ScopesAttribute>();

                    if (int.TryParse(context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value, out int userId))
                    {
                        var routeData = context.GetRouteData();
                        var mapId = routeData.Values["id"];

                        var member = await memberRepository.GetByMapAndUserId(Guid.Parse((string)mapId), userId);

                        if (member == null)
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("Member not found");
                            return;
                        }

                        if (atribute == null)
                        {
                            await next.Invoke(context);
                            return;
                        }

                        var scopes = atribute.Scopes.Split(';').ToHashSet();
                        var memberScopes = member.Scopes.Split(';').ToHashSet();

                        var IsSuperset = scopes.IsSubsetOf(memberScopes);

                        if (!IsSuperset)
                        {
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync($"For access this path, you need to have {string.Join(',' , scopes)} scope");
                            return;
                        }
                    }

                    await next.Invoke(context);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed validate scopes => {ex.GetType().Name}:{ex.Message}");
                    await next.Invoke(context);
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MapHub>("/ws/map");
            });
        }
    }
}
