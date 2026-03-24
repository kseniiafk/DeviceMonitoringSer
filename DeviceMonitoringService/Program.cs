using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Contracts;
using DeviceMonitoringService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

builder.WebHost.UseNetTcp(8090);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000);
});

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<DeviceService>();

    // HTTP endpoint (BasicHttpBinding)
    var httpBinding = new BasicHttpBinding();
    serviceBuilder.AddServiceEndpoint<DeviceService, IDeviceManager>(
        httpBinding,
        "/DeviceService/http");

    // TCP endpoint (NetTcpBinding)
    var tcpBinding = new NetTcpBinding(SecurityMode.None);
    serviceBuilder.AddServiceEndpoint<DeviceService, IDeviceManager>(
        tcpBinding,
        "/DeviceService/tcp");

    // Enable WSDL metadata
    var smb = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    smb.HttpGetEnabled = true;
});

app.MapGet("/", () => "DeviceMonitoringService is running. WSDL: /DeviceService/http?wsdl");
app.MapGet("/health", () => "OK");

Console.WriteLine("Service starting...");
Console.WriteLine("HTTP endpoint: http://localhost:5000/DeviceService/http");
Console.WriteLine("TCP endpoint:  net.tcp://localhost:8090/DeviceService/tcp");

app.Run();
