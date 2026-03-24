using System.Collections.Concurrent;
using CoreWCF;
using Contracts;

namespace DeviceMonitoringService;

public class DeviceService : IDeviceManager
{
    private static readonly ConcurrentDictionary<int, DeviceInfo> _devices = new();
    private static int _httpCallCount;
    private static int _tcpCallCount;

    static DeviceService()
    {
        for (int i = 1; i <= 10; i++)
        {
            _devices[i] = new DeviceInfo
            {
                Id = i,
                Name = $"Device_{i}",
                LastPing = DateTime.UtcNow.AddMinutes(-i),
                IsOnline = i % 2 == 0
            };
        }
    }

    public List<DeviceInfo> GetAllDevices()
    {
        IncrementCallCounter();
        return _devices.Values.ToList();
    }

    public DeviceInfo GetDevice(int id)
    {
        IncrementCallCounter();
        return _devices.TryGetValue(id, out var device) ? device : new DeviceInfo();
    }

    public bool PingDevice(int id)
    {
        IncrementCallCounter();
        if (_devices.TryGetValue(id, out var device))
        {
            device.LastPing = DateTime.UtcNow;
            device.IsOnline = true;
            return true;
        }
        return false;
    }

    public string GetServiceStats()
    {
        IncrementCallCounter();
        return $"HTTP calls: {_httpCallCount}, TCP calls: {_tcpCallCount}";
    }

    private void IncrementCallCounter()
    {
        var context = OperationContext.Current;
        if (context != null)
        {
            var scheme = context.IncomingMessageProperties.Via?.Scheme;
            if (scheme == "http" || scheme == "https")
                Interlocked.Increment(ref _httpCallCount);
            else if (scheme == "net.tcp")
                Interlocked.Increment(ref _tcpCallCount);
        }
    }
}
