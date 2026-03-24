using System.Diagnostics;
using System.ServiceModel;
using Contracts;

Console.WriteLine("=== TCP CLIENT ===");
Console.WriteLine("Connecting to net.tcp://localhost:8090/DeviceService/tcp ...");
Console.WriteLine();

var binding = new NetTcpBinding(SecurityMode.None);
var endpoint = new EndpointAddress("net.tcp://localhost:8090/DeviceService/tcp");
var factory = new ChannelFactory<IDeviceManager>(binding, endpoint);
var client = factory.CreateChannel();

try
{
    // 1. GetDevice
    Console.WriteLine("--- GetDevice(5) ---");
    var device = client.GetDevice(5);
    Console.WriteLine($"  ID={device.Id}, Name={device.Name}, Online={device.IsOnline}, LastPing={device.LastPing:HH:mm:ss}");
    Console.WriteLine();

    // 2. PingDevice
    Console.WriteLine("--- PingDevice(5) ---");
    var pingResult = client.PingDevice(5);
    Console.WriteLine($"  Ping result: {pingResult}");
    Console.WriteLine();

    // 3. GetServiceStats
    Console.WriteLine("--- GetServiceStats ---");
    var stats = client.GetServiceStats();
    Console.WriteLine($"  Stats: {stats}");
    Console.WriteLine();

    // 4. Performance comparison
    Console.WriteLine("=== PERFORMANCE TEST ===");
    const int iterations = 100;

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
    {
        client.GetDevice(1);
    }
    sw.Stop();

    Console.WriteLine($"  {iterations} calls via TCP: {sw.ElapsedMilliseconds} ms");
    Console.WriteLine($"  Average: {sw.ElapsedMilliseconds / (double)iterations:F2} ms per call");

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
