using System.Diagnostics;
using Microsoft.AspNetCore;
using RandomNumberFramework;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ExampleAspNetCoreFramework
{

    public static class Program {
        public static void Main(string[] args){
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
            
            builder.Build().Run();
            
            
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            
            app.UseServiceModel(serviceBuilder =>
            {
    
                serviceBuilder.AddService<Service>();
                serviceBuilder.AddService<StreamingService>();
    
                serviceBuilder.AddServiceEndpoint<Service, IService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), $"https://localhost:7151/Service.svc");
    
                serviceBuilder.AddServiceEndpoint<StreamingService, IStreamingService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport){TransferMode = TransferMode.Streamed}, $"https://localhost:7151/StreamingService.svc");
    
                serviceBuilder.AddServiceEndpoint<Service, IService>(new NetTcpBinding(SecurityMode.Transport),"net.tcp://localhost:808/Service/netTcp");
    
#pragma warning disable CA1416
                // Only available on windows
                serviceBuilder.AddServiceEndpoint<Service, IService>(new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport), "net.pipe://localhost/Service");
#pragma warning restore CA1416
    
                var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
                serviceMetadataBehavior.HttpsGetEnabled = true;
            });
            
        }
    }
}




public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
            
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseMvc();
    }
}