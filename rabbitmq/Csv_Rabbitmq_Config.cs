using System;
using System.Text;
using FileUploadApp.CommonLayer.polly;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace FileUploadApp.rabbitmq
{
    public class Csv_Rabbitmq_Config
    {
        private readonly ConnectionFactory factory;

        // private readonly RetryPolicy rabbitMqRetryPolicy;

        public Csv_Rabbitmq_Config()
        {
            factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            // Configure Polly retry policy for RabbitMQ publishing
            // rabbitMqRetryPolicy = Policy
            //     .Handle<BrokerUnreachableException>() // Handle all exceptions, you may want to refine this to specific exceptions
            //     .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, timespan, retryCount, context) =>
            //     {
            //         Console.WriteLine($"Retry attempt {retryCount} filename publisher ka andaer");
            //     });
        }

        public void rabbitMQPublisher(string file)
        {
            
            var rabbitMqRetryPolicy = pollyRetry.rabbitMqRetryPolicy();
            rabbitMqRetryPolicy.Execute(() =>
            {
                try
                {
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();
                    channel.QueueDeclare(

                        queue: "fileAddress",
                        durable: true,
                        exclusive: false,
                        arguments: null
                    );

                    var body = Encoding.UTF8.GetBytes(file);

                    channel.BasicPublish(
                        exchange: string.Empty,
                        routingKey: "fileAddress",
                        basicProperties: null,
                        body: body
                    );

                    Console.WriteLine(" [x] Sent {0}", file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" [!] Exception: {0}", ex.Message);
                    throw; // Ensure the exception is propagated for Polly to handle retries
                }
                return Task.CompletedTask;
            });
        }
    }
}
