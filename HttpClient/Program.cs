using System.ServiceModel;
using Contracts;

Console.WriteLine("=== HTTP CLIENT ===");
Console.WriteLine("Connecting to http://localhost:5000/DeviceService/http ...");
Console.WriteLine();

var binding = new BasicHttpBinding();
var endpoint = new EndpointAddress("http://localhost:5000/DeviceService/http");
var factory = new ChannelFactory<IDeviceManager>(binding, endpoint);
var client = factory.CreateChannel();

try
{
    // 1. GetAllDevices
    Console.WriteLine("--- GetAllDevices ---");
    var devices = client.GetAllDevices();
    Console.WriteLine($"Total devices: {devices.Count}");
    foreach (var d in devices)
    {
        Console.WriteLine($"  ID={d.Id}, Name={d.Name}, Online={d.IsOnline}, LastPing={d.LastPing:HH:mm:ss}");
    }
    Console.WriteLine();

    // 2. GetDevice
    Console.WriteLine("--- GetDevice(1) ---");
    var device = client.GetDevice(1);
    Console.WriteLine($"  ID={device.Id}, Name={device.Name}, Online={device.IsOnline}");
    Console.WriteLine();

    // 3. PingDevice
    Console.WriteLine("--- PingDevice(3) ---");
    var pingResult = client.PingDevice(3);
    Console.WriteLine($"  Ping result: {pingResult}");
    Console.WriteLine();

    // 4. GetServiceStats
    Console.WriteLine("--- GetServiceStats ---");
    var stats = client.GetServiceStats();
    Console.WriteLine($"  Stats: {stats}");

    ((IClientChannel)client).Close();
    factory.Close();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
