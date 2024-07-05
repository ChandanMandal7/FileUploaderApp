using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;

namespace FileUploadApp.CommonLayer.polly
{
    public class pollyRetry
    {
        public static RetryPolicy rabbitMqRetryPolicy()
        {
            try
            { 
                RetryPolicy rabbitMqRetryPolicy1 = Policy
                .Handle<BrokerUnreachableException>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry attempt {retryCount}");
                });

                return rabbitMqRetryPolicy1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }


    }
}