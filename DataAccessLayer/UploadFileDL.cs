using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using MySql.Data.MySqlClient;
using FileUploadApp.rabbitmq;

namespace FileUploadApp.DataAccessLayer
{
    public class UploadFileDL
    {
        // private static readonly string _connectionString = "server=localhost;user=root;password=;database=DataManagemnt;port=3306";
        // private static readonly MySqlConnection _mySqlConnection = new MySqlConnection(_connectionString);

        public static async Task<UploadFileResponse> UploadCsvFile(string path)
        {
            Console.WriteLine("Uploading file");
            UploadFileResponse response = new UploadFileResponse();
            response.IsSuccess = true;
            response.Message = "Successfully";

            try
            {
                if (!path.ToLower().EndsWith(".csv"))
                {
                    throw new ArgumentException("Invalid file type. Only CSV files are allowed.");
                }

                Console.WriteLine("Starting CSV file processing...");

                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using (IExcelDataReader reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                    {
                        FallbackEncoding = Encoding.GetEncoding(1252)
                    }))
                    {
                        DataSet dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            UseColumnDataType = false,
                            ConfigureDataTable = tableReader => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });

                        var dataTable = dataSet.Tables[0];
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        stopwatch.Start();

                        int batchSize = 10000;

                        int totalRows = dataTable.Rows.Count;
                        int batches = (int)Math.Ceiling((double)totalRows / batchSize);

                        // Initialize RabbitMQ publisher outside the loop
                        using (var rabbitMQPublisher = new RabbitMQPublisher())
                        {
                            for (int batchIndex = 0; batchIndex < batches; batchIndex++)
                            {
                                int startRow = batchIndex * batchSize;
                                int endRow = Math.Min(startRow + batchSize, totalRows);
                                int rowsInBatch = endRow - startRow;

                                if (rowsInBatch <= 0)
                                {
                                    continue;
                                }

                                Console.WriteLine($"Processing batch {batchIndex + 1} of {batches}...");

                                StringBuilder batchInsertCommand = new StringBuilder();

                                batchInsertCommand.Append("INSERT IGNORE INTO userrecords (EmailId, Name, Country, State, City, TelephoneNumber, AddressLine1, AddressLine2,DateOfBirth, GrossSalaryFY2019_20, GrossSalaryFY2020_21, GrossSalaryFY2021_22,GrossSalaryFY2022_23, GrossSalaryFY2023_24) VALUES");


                                for (int i = startRow; i < endRow; i++)
                                {
                                    DataRow row = dataTable.Rows[i];

                                    // Appending each row values to the batch command
                                    batchInsertCommand.Append($"('{EscapeString(row["EmailId"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["Name"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["Country"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["State"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["City"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["TelephoneNumber"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["AddressLine1"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["AddressLine2"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["DateOfBirth"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["GrossSalaryFY2019_20"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["GrossSalaryFY2020_21"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["GrossSalaryFY2021_22"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["GrossSalaryFY2022_23"].ToString())}', ");
                                    batchInsertCommand.Append($"'{EscapeString(row["GrossSalaryFY2023_24"].ToString())}'), ");
                                }

                                // Remove the trailing comma and space

                                if (batchInsertCommand.Length > 0)

                                {
                                    batchInsertCommand.Length -= 2; // Remove the last ", "
                                }


                                // Construct the final SQL command for the batch
                                string finalBatchInsertCommand = batchInsertCommand.ToString();


                                // Publish the final batch insert command to RabbitMQ for every single batch -------
                                rabbitMQPublisher.PublishMessage(finalBatchInsertCommand);    
                              

                            }

                            stopwatch.Stop();
                            Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
                           

                            response.IsSuccess = true;
                            response.Message = "CSV file processed successfully";



                            
                            //   if (response.IsSuccess==true )
                            //     {
                            //         try
                            //         {
                            //             Console.WriteLine(response.IsSuccess);
                            //             System.IO.File.Delete(path);
                            //         }
                            //         catch (System.Exception ex)
                            //         {
                            //             Console.WriteLine(ex);
                            //         }
                            //     }
                            
                               
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
            }


            return response;
        }

        private static string EscapeString(string value)
        {
            // Replace single quotes with double single quotes to escape them
            return value.Replace("'", "''");
        }
    }

    public class UploadFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
