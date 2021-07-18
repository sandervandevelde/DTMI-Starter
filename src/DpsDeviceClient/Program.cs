using System;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System.Text;

namespace DpsDeviceClient
{
    internal class Program
    {
        /// <summary>
        /// Sample taken from https://github.com/Azure-Samples/azure-iot-samples-csharp/blob/master/provisioning/Samples/device/SymmetricKeySample/Program.cs
        /// </summary>
        /// <param name="args"></param>
        public static async Task<int> Main(string[] args)
        {
            var parameters = new Parameters
            {
                GlobalDeviceEndpoint = "global.azure-devices-provisioning.net",
                IdScope = "0ne0003BB8B",
                Id = "PnPDeviceViaDps",
                PrimaryKey = "[Enrollment Primary key]",
            };

            await RunSampleAsync(parameters);

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();

            return 0;
        }

        private static async Task RunSampleAsync(Parameters parameters)
        {
            Console.WriteLine($"Initializing the device provisioning client...");

            // For individual enrollments, the first parameter must be the registration Id, where in the enrollment
            // the device Id is already chosen. However, for group enrollments the device Id can be requested by
            // the device, as long as the key has been computed using that value.
            // Also, the secondary could be included, but was left out for the simplicity of this sample.
            using var security = new SecurityProviderSymmetricKey(
                parameters.Id,
                parameters.PrimaryKey,
                null);

            using var transportHandler = new ProvisioningTransportHandlerMqtt(TransportFallbackType.TcpOnly);

            ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(
                parameters.GlobalDeviceEndpoint,
                parameters.IdScope,
                security,
                transportHandler);

            Console.WriteLine($"Initialized for registration Id {security.GetRegistrationID()}.");

            Console.WriteLine("Registering with the device provisioning service...");
            DeviceRegistrationResult result = await provClient.RegisterAsync();

            Console.WriteLine($"Registration status: {result.Status}.");

            if (result.Status != ProvisioningRegistrationStatusType.Assigned)
            {
                Console.WriteLine($"Registration status did not assign a hub, so exiting this sample.");
                return;
            }

            Console.WriteLine($"Device {result.DeviceId} registered to {result.AssignedHub}.");

            Console.WriteLine("Creating symmetric key authentication for IoT Hub...");

            IAuthenticationMethod auth = new DeviceAuthenticationWithRegistrySymmetricKey(
                result.DeviceId,
                security.GetPrimaryKey());

            Console.WriteLine($"Testing the provisioned device with IoT Hub...");

            using DeviceClient iotClient = DeviceClient.Create(
                result.AssignedHub,
                auth,
                new ClientOptions { ModelId = "dtmi:com:example:NotExisting;2" });

            Console.WriteLine("Sending a telemetry message...");

            string jsonData = "{ \"meaning\":43}";

            using var message = new Message(Encoding.UTF8.GetBytes(jsonData));

            await iotClient.SendEventAsync(message);

            Console.WriteLine("Finished.");
        }
    }

    internal class Parameters
    {
        public string IdScope { get; set; }

        public string Id { get; set; }

        public string PrimaryKey { get; set; }

        public string GlobalDeviceEndpoint { get; set; }
    }
}