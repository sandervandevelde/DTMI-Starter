using Microsoft.Azure.Devices;
using System;
using System.Threading.Tasks;

namespace RetrieveModelIdentificationApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Retrieve Model Identification!");

            var manager = RegistryManager.CreateFromConnectionString("[IOT HUB Connectionstring]");

            var twin = await manager.GetTwinAsync("PnPDeviceViaDps"); // this is not the registration ID

            var modelId = twin.ModelId;

            Console.WriteLine($"The current model is {modelId}");

            Console.WriteLine("Press a key to exit");

            Console.ReadKey();
        }
    }
}