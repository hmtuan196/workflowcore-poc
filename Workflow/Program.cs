using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Workflow.Workflows;
using Workflow.Workflows.Data;
using WorkflowCore.Interface;

namespace Workflow
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs\\app.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var hostBuilder = CreateHostBuilder(args).Build();
            var serviceProvider = hostBuilder.Services;

            var host = serviceProvider.GetRequiredService<IWorkflowHost>();
            host.RegisterWorkflow<NhiemVuWorkflow, NhiemVuData>();
            host.Start();

            var appLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            appLifetime.ApplicationStopped.Register(() =>
            {
                host.Stop();
                Log.CloseAndFlush();
            });

            hostBuilder.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}