# DeviceMonitoringService — CoreWCF с HTTP и TCP транспортом

## Описание

Сервис мониторинга устройств, реализованный на CoreWCF и развёрнутый в ASP.NET Core. Поддерживает одновременную работу по двум транспортным протоколам: **HTTP** (BasicHttpBinding) и **TCP** (NetTcpBinding).

## Структура проекта

    DeviceMonitoringService.sln
    ├── Contracts/              — контракты данных и служб (общая библиотека)
    │   ├── DeviceInfo.cs       — [DataContract] модель устройства
    │   └── IDeviceManager.cs   — [ServiceContract] интерфейс службы
    ├── DeviceMonitoringService/ — серверное приложение (ASP.NET Core + CoreWCF)
    │   ├── Program.cs          — настройка хоста, HTTP и TCP эндпоинтов
    │   └── DeviceService.cs    — реализация службы
    ├── HttpClient/             — консольный клиент (BasicHttpBinding)
    │   └── Program.cs
    └── TcpClient/              — консольный клиент (NetTcpBinding)
        └── Program.cs

## Контракт службы

    [ServiceContract]
    public interface IDeviceManager
    {
        [OperationContract] List<DeviceInfo> GetAllDevices();
        [OperationContract] DeviceInfo GetDevice(int id);
        [OperationContract] bool PingDevice(int id);
        [OperationContract] string GetServiceStats();
    }

## Эндпоинты

| Протокол | Binding | Адрес |
|----------|--------|-------|
| HTTP | BasicHttpBinding | `http://localhost:5000/DeviceService/http` |
| TCP | NetTcpBinding | `net.tcp://localhost:8090/DeviceService/tcp` |
| WSDL | Metadata | `http://localhost:5000/DeviceService/http?wsdl` |

## Запуск

### Требования

- .NET 8.0 SDK

### 1. Сборка

    dotnet build

### 2. Запуск сервиса

    dotnet run --project DeviceMonitoringService

### 3. Запуск HTTP-клиента (в отдельном терминале)

    dotnet run --project HttpClient

### 4. Запуск TCP-клиента (в отдельном терминале)

    dotnet run --project TcpClient

## Примеры работы

### Запуск сервиса

    Service starting...
    HTTP endpoint: http://localhost:5000/DeviceService/http
    TCP endpoint:  net.tcp://localhost:8090/DeviceService/tcp
    Now listening on: http://0.0.0.0:8090
    Now listening on: http://localhost:5000

### HTTP-клиент

    === HTTP CLIENT ===
    Connecting to http://localhost:5000/DeviceService/http ...

    --- GetAllDevices ---
    Total devices: 10
      ID=1, Name=Device_1, Online=False, LastPing=11:05:42
      ID=2, Name=Device_2, Online=True, LastPing=11:04:42
      ...

    --- GetDevice(1) ---
      ID=1, Name=Device_1, Online=False

    --- PingDevice(3) ---
      Ping result: True

    --- GetServiceStats ---
      Stats: HTTP calls: 4, TCP calls: 0

### TCP-клиент

    === TCP CLIENT ===
    Connecting to net.tcp://localhost:8090/DeviceService/tcp ...

    --- GetDevice(5) ---
      ID=5, Name=Device_5, Online=False, LastPing=11:01:42

    --- PingDevice(5) ---
      Ping result: True

    --- GetServiceStats ---
      Stats: HTTP calls: 4, TCP calls: 3

    === PERFORMANCE TEST ===
      100 calls via TCP: 28 ms
      Average: 0.28 ms per call

## Особенности реализации

- **Хранение данных**: `ConcurrentDictionary<int, DeviceInfo>` со статическими тестовыми данными (10 устройств)
- **Подсчёт вызовов**: `OperationContext.Current` определяет тип транспорта (HTTP/TCP) и ведёт раздельную статистику
- **DI**: регистрация сервиса выполняется через CoreWCF `AddService<DeviceService>()`
- **Безопасность**: отключена для упрощения (`SecurityMode.None`)

## Используемые пакеты

| Проект | Пакеты |
|--------|--------|
| Contracts | System.ServiceModel.Primitives |
| DeviceMonitoringService | CoreWCF.Primitives, CoreWCF.Http, CoreWCF.NetTcp |
| HttpClient | System.ServiceModel.Http |
| TcpClient | System.ServiceModel.NetTcp |
