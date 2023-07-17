using Infra.Registers;
using Job;
using Quartz;

IConfiguration configuration = new ConfigurationBuilder()
                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                  .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables()
                  .Build();


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //services.AddHostedService<WorkeServicer>();

        services.AddServiceInjections(configuration);
      
        services.AddQuartz(quartzConfigs =>
        {
            quartzConfigs.UseMicrosoftDependencyInjectionScopedJobFactory();

            quartzConfigs.AddJobAndTrigger<ObservadorInterfacesJob>(configuration);
        });
        
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    })
    .Build();

await host.RunAsync();
