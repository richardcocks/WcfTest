using System.Diagnostics;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.WebHost.UseKestrel(options =>
{
    options.ListenLocalhost(7151, listenOptions =>
    {
        listenOptions.UseHttps();
        if (Debugger.IsAttached)
        {
            listenOptions.UseConnectionLogging();
        }
    });
});

builder.WebHost.UseNetTcp();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    
    serviceBuilder.AddService<Service>();
    serviceBuilder.AddServiceEndpoint<Service, IService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), $"https://localhost:7151/Service.svc");
    serviceBuilder.AddServiceEndpoint<Service, IService>(new NetTcpBinding(SecurityMode.Transport),"net.tcp://localhost:808/Service/netTcp");
    
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();
