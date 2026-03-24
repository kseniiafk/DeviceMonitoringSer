using System.ServiceModel;

namespace Contracts;

[ServiceContract]
public interface IDeviceManager
{
    [OperationContract]
    List<DeviceInfo> GetAllDevices();

    [OperationContract]
    DeviceInfo GetDevice(int id);

    [OperationContract]
    bool PingDevice(int id);

    [OperationContract]
    string GetServiceStats();
}
