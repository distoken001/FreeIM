using Com.Ctrip.Framework.Apollo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace imServer
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

        public IConfiguration Configuration;

        public void ConfigureServices(IServiceCollection services)
        {
             services
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
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.GetEncoding("GB2312");
            Console.InputEncoding = Encoding.GetEncoding("GB2312");
            
            app.UseDeveloperExceptionPage();
            //跨域设置
            app.UseCors("CorsPolicy");
            app.UseFreeImServer(new ImServerOptions
            {
                Redis = new FreeRedis.RedisClient(Configuration["ImServerOption:RedisClient"]),
                Servers = Configuration["ImServerOption:Servers"].Split(";"),
                Server = Configuration["ImServerOption:Server"]
            });
        }
    }
}
