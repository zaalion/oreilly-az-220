using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        private static DeviceClient _iotDevice;
        private static TwinCollection _reportedTwinProperties;

        private const string _deviceConnectionString =
            "HostName=iot-hub-or-demo01.azure-devices.net;DeviceId=device01;" +
            "SharedAccessKey=0bCrs6t0ISouwVwxLAoyWj3PaE9AA5ckm/Kasu93zoU=";

        static void Main(string[] args)
        {
            _reportedTwinProperties = new TwinCollection();
            _iotDevice = DeviceClient.CreateFromConnectionString(_deviceConnectionString);

            _iotDevice.SetDesiredPropertyUpdateCallbackAsync(DesiredTwinUpdatedCallback, null);

            //_iotDevice.OpenAsync();
            //_iotDevice.SetMethodHandlerAsync("sayHi", SayHi, null);

            //SendD2CMessage();

            //Receive().Wait();

            //UpdateTwin();

            SendToBlobAsync();

            Console.WriteLine("Press a key to terminate the device app...");
            Console.ReadLine();
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-file-upload
        /// </summary>
        private static async void SendToBlobAsync()
        {
            string fileName = "image.jpg";
            Console.WriteLine("Uploading file: {0}", fileName);
            var watch = System.Diagnostics.Stopwatch.StartNew();

            using (var sourceData = new FileStream(@"D:\Repo\oreilly-az-220\Demos\AZ220-demos\Device\trees.jpg", FileMode.Open))
            {
                await _iotDevice.UploadToBlobAsync(fileName, sourceData);
            }

            watch.Stop();
            Console.WriteLine("Time to upload file: {0}ms\n", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Fires when IoT Hub updates one or more desired twin property
        /// </summary>
        /// <param name="desiredProperties"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        private static Task DesiredTwinUpdatedCallback(TwinCollection desiredProperties,
            object userContext)
        
        {
            var desiredSamplingFrequency = (string)desiredProperties["sampleFrequency"];

            Console.WriteLine("Making changes to the device and updating the reported device twin (if needed)...");

            _reportedTwinProperties["sampleFrequency"] = desiredSamplingFrequency;

            _iotDevice.UpdateReportedPropertiesAsync(_reportedTwinProperties).Wait();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates a reported twin property
        /// </summary>
        private static void UpdateTwin()
        {
            _reportedTwinProperties["sampleFrequency"] = "40";

            _iotDevice.UpdateReportedPropertiesAsync(_reportedTwinProperties).Wait();
        }

        /// <summary>
        /// A direct method which can be called from IoT Hub
        /// </summary>
        /// <param name="methodRequest"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        private static Task<MethodResponse> SayHi(MethodRequest methodRequest,
            object userContext)
        {
            Console.WriteLine("Got a direct call from IoT Hub!");
            Console.WriteLine(methodRequest.DataAsJson);

            var payload = Encoding.ASCII.GetBytes("{\"response\": \"Hi from device01!\"}");

            return Task.FromResult(new MethodResponse(payload, 200));
        }

        /// <summary>
        /// Sends a device to cloud message
        /// </summary>
        private static void SendD2CMessage()
        {
            var d2CMessage = new D2CMessage
            {
                Humidity = "Low",
                Temprature = 12.5F
            };

            var payload = JsonConvert.SerializeObject(d2CMessage);

            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(payload));

            _iotDevice.SendEventAsync(message).Wait();
        }

        /// <summary>
        /// Keeps listening for the messages comming from the device queue
        /// </summary>
        /// <returns></returns>
        private async static Task Receive()
        {
            while (true)
            {
                Console.WriteLine("Listenning...");

                var message = await _iotDevice.ReceiveAsync();

                if (message == null)
                {
                    continue;
                }

                var messageBody = message.GetBytes();

                var payload = Encoding.ASCII.GetString(messageBody);

                Console.WriteLine($"Received message from cloud: '{payload}'");

                await _iotDevice.CompleteAsync(message);
                //await _iotDevice.AbandonAsync(message);
                //await _iotDevice.RejectAsync(message);
            }
        }
    }
}