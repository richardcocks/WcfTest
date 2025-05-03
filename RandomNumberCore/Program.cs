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

// Only available on Windows
builder.WebHost.UseNetNamedPipe(options =>
{
    options.Listen("net.pipe://localhost/Service");
});


var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    
    serviceBuilder.AddService<Service>();
    serviceBuilder.AddServiceEndpoint<Service, IService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), $"https://localhost:7151/Service.svc");
    serviceBuilder.AddServiceEndpoint<Service, IService>(new NetTcpBinding(SecurityMode.Transport),"net.tcp://localhost:808/Service/netTcp");
    
#pragma warning disable CA1416
    // Only available on windows
    serviceBuilder.AddServiceEndpoint<Service, IService>(new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport), "net.pipe://localhost/Service");
#pragma warning restore CA1416
    
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();
