using Com.Ctrip.Framework.Apollo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Text;

namespace web
{

    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            this.Configuration = configuration;
            //输出debug日志在控制台，方便查找问题
            Com.Ctrip.Framework.Apollo.Logging.LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Debug);
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddApollo(configuration.GetSection("apollo"))
                .AddNamespace("backend.share")
                .AddDefault();
            Configuration = builder.Build();
        }
        public static ServiceProvider privider;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services
             .AddHttpClient()
             //.AddSingleton(Configuration)
             //.AddSingleton<ApolloConfigs>()
             .AddCors(options =>
             {
                 options.AddPolicy("CorsPolicy", builder =>
                 {
                     builder.SetIsOriginAllowed((x) => true)
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
                 });
             });
            //Swagger 配置
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Web API"
                });
            })
             .ConfigureSwaggerGen(options =>
             {
                 options.IncludeXmlComments(System.IO.Path.Combine(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath, "WebApi.xml"));
             });
            privider = services.BuildServiceProvider();

        }

        public void Configure(IApplicationBuilder app)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.GetEncoding("GB2312");
            Console.InputEncoding = Encoding.GetEncoding("GB2312");

            app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            ImHelper.Initialization(new ImClientOptions
            {
                Redis = new FreeRedis.RedisClient("127.0.0.1:6379,poolsize=10"),
                Servers = new[] { "127.0.0.1:6001" }
            });

            ImHelper.Instance.OnSend += (s, e) =>
                Console.WriteLine($"ImClient.SendMessage(server={e.Server},data={JsonConvert.SerializeObject(e.Message)})");

            ImHelper.EventBus(
                t =>
                {
                    Console.WriteLine(t.clientId + "上线了");
                    var onlineUids = ImHelper.GetClientListByOnline();
                    ImHelper.SendMessage(t.clientId, onlineUids, $"用户{t.clientId}上线了");
                },
                t => Console.WriteLine(t.clientId + "下线了"));
            //Swagger 配置
            if (Configuration["Env"] == "dev" || Configuration["Env"] == "test")
            {
                app.UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "WebApi");
            });
            }
            //Swagger 配置
            app.UseRouting();
            app.UseEndpoints(routes => routes.MapControllers());

        }
    }
}
