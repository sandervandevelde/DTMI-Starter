using Microsoft.Azure.Devices.Client;
using System;
using System.Text;

namespace SimpleDeviceClientApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello simple device client!");

            var connectionString = "HostName=edgedemo-ih.azure-devices.net;DeviceId=pnpSimpleDevice;SharedAccessKey=SECRET";

            using var deviceClient = DeviceClient.CreateFromConnectionString(
                                                    connectionString,
                                                    new ClientOptions { ModelId = "dtmi:com:example:NotExisting;1" });

            //// open connection explicitly
            deviceClient.OpenAsync().Wait();

            //// send a single message
            SendSingleMessage(deviceClient);

            Console.WriteLine("Message is sent. Press a key to exit...");
            Console.ReadKey();
        }

        private static void SendSingleMessage(DeviceClient deviceClient)
        {
            string jsonData = "{ \"meaning\":42}";

            using var message = new Message(Encoding.UTF8.GetBytes(jsonData));

            message.Properties.Add("messagetype", "normal");

            deviceClient.SendEventAsync(message).Wait();

            Console.WriteLine("A single message is sent");
        }
    }
}