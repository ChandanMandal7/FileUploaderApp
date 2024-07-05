using RabbitMQ.Client;
using System;
using System.Text;
using Polly;
using Polly.Retry;
using FileUploadApp.CommonLayer.polly;

namespace FileUploadApp.rabbitmq
{
    public class RabbitMQPublisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "file_upload_exchange";
        private const string RoutingKey = "file_upload_queue";

    //    private readonly RetryPolicy rabbitMqRetryPolicy;

        public RabbitMQPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queue: "sqlquery",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.QueueBind("sqlquery", ExchangeName, RoutingKey);

            // Configure Polly wait and retry policy
            // rabbitMqRetryPolicy = Policy
            //     .Handle<Exception>() // Handle all exceptions, you may want to refine this to specific exceptions
            //     .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
            //     {
            //         Console.WriteLine($"Retry attempt {retryCount} query publisher k andar");
            //     // Console.WriteLine($"Exception message: {exception.Message}");
            //     });
        }

        public void PublishMessage(string message)
        {
            var rabbitMqRetryPolicy = pollyRetry.rabbitMqRetryPolicy();
            rabbitMqRetryPolicy.Execute(() =>
            {
                var body = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(exchange: ExchangeName,
                                      routingKey: RoutingKey,
                                      basicProperties: null,
                                      body: body);

             //  Console.WriteLine(" [x] Sent {0}", message);
               return Task.CompletedTask;
            });
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
