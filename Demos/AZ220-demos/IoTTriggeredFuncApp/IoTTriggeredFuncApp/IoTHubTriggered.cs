using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace IoTTriggeredFuncApp
{
    public static class IoTHubTriggered
    {
        private static HttpClient client = new HttpClient();

        /// <summary>
        /// Use Event Hub-compatible endpoint in your connection string
        /// </summary>
        /// <param name="message"></param>
        /// <param name="log"></param>
        [FunctionName("IoTHubTriggered")]
        public static void Run([IoTHubTrigger("messages/events", 
            Connection = "IotHubConnection")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: " +
                $"{Encoding.UTF8.GetString(message.Body.Array)}");
        }
    }
}