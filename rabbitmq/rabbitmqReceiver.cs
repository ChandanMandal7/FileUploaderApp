using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using FileUploadApp.DataAccessLayer;
using Polly.Retry;
using Polly;
using FileUploadApp.CommonLayer.polly;

namespace FileUploadApp.rabbitmq
{
    public class rabbitmqReceiver
    {
        private readonly ConnectionFactory factory;
        // private readonly AsyncRetryPolicy rabbitMqRetryPolicy;

        public rabbitmqReceiver()
        {
            factory = new ConnectionFactory
            {
                HostName = "localhost",
            };

            // rabbitMqRetryPolicy = Policy
            //  .Handle<Exception>() // Handle all exceptions, you may want to refine this to specific exceptions
            // .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
            //  {
            //      Console.WriteLine($"Retry attempt {retryCount} File name subscriber pa");
            //  });

        }

        public void StartListening()
        {
            var rabbitMqRetryPolicy = pollyRetry.rabbitMqRetryPolicy();
            rabbitMqRetryPolicy.Execute(async () =>
            {
                Console.WriteLine("Connecting to khargosh");
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                Console.WriteLine("ho gya...");

                channel.QueueDeclare(queue: "fileAddress",
                                    durable: true,
                                    exclusive: false,
                                    arguments: null);

                Console.WriteLine(" Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
               {
                 var body = ea.Body.ToArray();
                 var message = Encoding.UTF8.GetString(body);

                 var res = await UploadFileDL.UploadCsvFile(message);

                 Console.WriteLine($" Received {message}");
             };

                channel.BasicConsume(queue: "fileAddress",
                                    autoAck: true,
                                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadKey();
                Console.WriteLine("Subscriber exited....");
                await Task.CompletedTask;
            });



        }
    }
}
