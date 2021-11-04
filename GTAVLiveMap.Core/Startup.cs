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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Schema;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

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
            services.AddSingleton<IScopeRepository, ScopeRepository>();
            services.AddSingleton<IMapLabelRepository, MapLabelRepository>();
            services.AddSingleton<IConnectionRepository, ConnectionRepository>();
            services.AddSingleton<IMapActionsRepository, MapActionsRepository>();
            services.AddSingleton<IMapConfigRepository, MapConfigRepository>();

            services.AddSingleton<IGoogleService, GoogleService>();
            services.AddSingleton<IUserUIService, UserUIService>();
            services.AddSingleton<IMapApiKeyService, MapApiKeyService>();
            services.AddSingleton<IMapLabelService, MapLabelService>();

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
            IMapRepository mapRepository,
            IMapApiKeyService mapApiKeyService,
            ILogger<Startup> logger)
        {
            logger.LogInformation($"Env: {env.EnvironmentName}");

            app.UseCors("CorsPolicy");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.Use(next => async context =>
            {
                try
                {
                    var currentDitectory = Directory.GetCurrentDirectory();

                    var logsDirectoryName = "logs";
                    var pathToLogsFile = currentDitectory + "/" + logsDirectoryName + "/requests.txt";

                    if (!Directory.Exists(logsDirectoryName))
                        Directory.CreateDirectory(logsDirectoryName);
                    
                    context.Request.EnableBuffering();

                    using var bodyReader = new StreamReader(context.Request.Body , Encoding.UTF8 , true, -1 , true);

                    var body = await bodyReader.ReadToEndAsync();

                    context.Request.Body.Position = 0;

                    object bodyObject = null;

                    try
                    {
                        bodyObject = Newtonsoft.Json.JsonConvert.DeserializeObject(body);
                    }
                    catch (Exception)
                    {
                        bodyObject = body;
                    }


                    var log = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        Path = context.Request.Path,
                        Method = context.Request.Method,
                        QueryString = context.Request.QueryString.ToString(),
                        Headers = context.Request.Headers.ToList(),
                        Body = bodyObject,
                        IP = context.Connection.RemoteIpAddress.MapToIPv4().ToString()
                    });

                    await File.AppendAllTextAsync(pathToLogsFile, log + Environment.NewLine);
                }
                catch (Exception)
                {
                    logger.LogError("Failed write log");
                }

                await next.Invoke(context);
            });

            app.Use(next => async context =>
            {
                try
                {
                    if (!context.Request.Path.Value.Contains("ws"))
                    {
                        await next.Invoke(context);
                        return;
                    }

                    var endpoint = context.GetEndpoint();

                    if (endpoint == null)
                    {
                        await next.Invoke(context);
                        return;
                    }

                    var hubMetadata = (HubMetadata) endpoint.Metadata.FirstOrDefault(p => p.GetType().IsEquivalentTo(typeof(HubMetadata)));

                    if (hubMetadata == null)
                    {
                        await next.Invoke(context);
                        return;
                    }

                    var hubType = hubMetadata.HubType;

                    var atribute = hubType.GetCustomAttribute<MapApiKeyValidateAttribute>();

                    if (atribute == null)
                    {
                        await next.Invoke(context);
                        return;
                    }

                    var key = context.Request.Query["apiKey"].FirstOrDefault();
                    
                    if (!(await mapApiKeyService.IsValid($"{key}")))
                    {
                        await context.Response.WriteAsync("Invalid api key");
                        await next.Invoke(context);
                        return;
                    }

                    await next.Invoke(context);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed validate map apiKey => {ex.GetType().Name}:{ex.Message}");
                    await next.Invoke(context);
                }
            });

            app.Use(next => async context =>
            {
                try
                {
                    var endpoint = context.GetEndpoint();
                        
                    if (endpoint == null)
                    {
                        await next.Invoke(context);
                        return;
                    }

                    var controllerDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                    if (controllerDescriptor == null)
                    {
                        await next.Invoke(context);
                        return;
                    }

                    var controllerType = controllerDescriptor.ControllerTypeInfo;

                    var actionType = controllerType.GetMethod(controllerDescriptor.ActionName);

                    var atribute = actionType.GetCustomAttribute<ScopesAttribute>();
                    var allowApiKeyAtribute = actionType.GetCustomAttribute<AllowApiKeyAttribute>();

                    if (int.TryParse(context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value, out int userId))
                    {
                        var routeData = context.GetRouteData();
                        var mapId = routeData.Values["id"]?.ToString();
                        var apiKey = context.Request.Headers["ApiKey"].ToString();

                        var map = string.IsNullOrEmpty(apiKey) && (string.IsNullOrEmpty(apiKey) || allowApiKeyAtribute == null) ? await mapRepository.GetById(new Guid(mapId)) : await mapRepository.GetByApiKey(apiKey);

                        if (map == null)
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("Map not found");
                            return;
                        }

                        if (!string.IsNullOrEmpty(apiKey) && allowApiKeyAtribute != null)
                        {
                            await next.Invoke(context);
                            return;
                        }

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
