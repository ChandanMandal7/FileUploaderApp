using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Polly.Retry;
using Polly;
using RabbitMQ.Client.Exceptions;
using FileUploadApp.CommonLayer.polly;

namespace FileUploadApp.rabbitmq
{
    public class RabbitMQSubscriber
    {


        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "file_upload_exchange";
        private const string RoutingKey = "file_upload_queue";
        string connectionString = "server=localhost;user=root;password=;database=DataManagemnt;port=3306";
        private readonly MySqlConnection _mySqlConnection;
        // private readonly AsyncRetryPolicy retryPolicy;

        public RabbitMQSubscriber()
        {
            // var factory = new ConnectionFactory { HostName = "localhost" };
            // _connection = factory.CreateConnection();
            // _channel = _connection.CreateModel();
            // _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            // _channel.QueueDeclare(queue: "sqlquery",
            //                       durable: true,
            //                       exclusive: false,
            //                       autoDelete: false,
            //                       arguments: null);
            // _channel.QueueBind("sqlquery", ExchangeName, RoutingKey);


            _mySqlConnection = new MySqlConnection("server=localhost;user=root;password=;database=DataManagemnt;port=3306");

            // retryPolicy = Policy
            //             .Handle<BrokerUnreachableException>() // Handle all exceptions, you may want to refine this to specific exceptions
            //            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
            //            {
            //                Console.WriteLine($"Retry {retryCount} in {timeSpan.TotalSeconds} seconds");
            //            });



        }
        public void StartListening()
        {
            var policyworkforsub = pollyRetry.rabbitMqRetryPolicy();
            policyworkforsub.Execute(() =>
            {
                Console.WriteLine("RabbitMQ subscriber listening...");
                var factory = new ConnectionFactory { HostName = "localhost" };
                var _connection = factory.CreateConnection();
                var _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                _channel.QueueDeclare(queue: "sqlquery",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                _channel.QueueBind("sqlquery", ExchangeName, RoutingKey);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Execute database command
                    await ExecuteDatabaseCommandAsync(this, message);


                    // Acknowledge the message by subscriber
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                };

                _channel.BasicConsume(queue: "sqlquery",
                                      autoAck: false,
                                      consumer: consumer);
                Console.ReadKey();

                return Task.CompletedTask;
            });
        }


        private static async Task ExecuteDatabaseCommandAsync(RabbitMQSubscriber @this, string message)
        {
            try
            {
                MySqlConnection sql = new MySqlConnection(@this.connectionString);
                await sql.OpenAsync();



                string sqlCommandText = message;
                using (var sqlCommand = new MySqlCommand(sqlCommandText, sql))
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                    Console.WriteLine("Database command executed successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing database command: {ex.Message}");
            }

        }
    }
}
