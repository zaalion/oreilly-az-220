using Microsoft.Azure.Devices;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ServiceClientApp
{
    class Program
    {
        private static ServiceClient _iotHubClient;

        private const string _serviceConnectionString =
            "HostName=iot-hub-or-demo01.azure-devices.net;SharedAccessKeyName=service;" +
            "SharedAccessKey=IOZDaEjoUfRwFKL2wZSE3Hh102FryfboYNpiDPys/xM=";

        static void Main(string[] args)
        {
            _iotHubClient = ServiceClient.CreateFromConnectionString(_serviceConnectionString);

            Console.WriteLine("Press a key to start the service app ...");
            Console.ReadLine();

            //SendC2DMessage();

            //CallDirectMethod();

            // Console.WriteLine("Sent a message to device01!");

            // var registryManager = RegistryManager.CreateFromConnectionString(_serviceConnectionString);
            // UpdateDesiredTwin(registryManager);

            Console.WriteLine("Press a key to terminate the service app ...");
            Console.ReadLine();
        }

        /// <summary>
        /// Updates the device desired twin properties. This will trigger a callback in the device app.
        /// </summary>
        /// <param name="registryManager"></param>
        private static void UpdateDesiredTwin(RegistryManager registryManager)
        {
            var deviceTwin = registryManager.GetTwinAsync("device01").Result;

            deviceTwin.Properties.Desired["sampleFrequency"] = "10";

            registryManager.UpdateTwinAsync("device01", deviceTwin, deviceTwin.ETag).Wait();
        }

        /// <summary>
        /// This will call a direct method in the device app
        /// </summary>
        private static void CallDirectMethod()
        {
            var method = new CloudToDeviceMethod("sayHi");
            method.SetPayloadJson("'Say greetings...'");

            var response = _iotHubClient.InvokeDeviceMethodAsync("device01", method).Result;

            Console.WriteLine($"Response status: {response.Status}, payload: {response.GetPayloadAsJson()}");
        }

        /// <summary>
        /// Sends a cloud to device message using the SDK
        /// </summary>
        static void SendC2DMessage()
        {
            var payload = "'Hello device01!'";
            var commandMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(payload))
            {
                MessageId = Guid.NewGuid().ToString()
            };

            _iotHubClient.SendAsync("device01", commandMessage).Wait();
        }
    }
}