using FileUploadApp.CommonLayer.Model;
using FileUploadApp.DataAccessLayer;
using FileUploadApp.rabbitmq;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UploadFileController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadCsvFile(UploadFileRequest request)
        {
          
            DataAccessLayer.UploadFileResponse response = new DataAccessLayer.UploadFileResponse();
            response.IsSuccess = true;
            response.Message = "File Uploaded SucessFully !";
            string path = "Upload/" + request.File.FileName;

            try
            {
               
                if (request.File.FileName.ToLower().EndsWith(".csv"))
                {
                    //agar pahela sa hai to purna wala delete kro
                    if (System.IO.File.Exists(path))
                    {

                        System.IO.File.Delete(path);
                    }

                   
                    using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                    {
                        await request.File.CopyToAsync(stream);
                    }
                    
                    //rabbit mq publish done------------------------------------------------
                    Csv_Rabbitmq_Config csv_Rabbitmq_Config = new Csv_Rabbitmq_Config();
                    csv_Rabbitmq_Config.rabbitMQPublisher(path);


                    Console.WriteLine(path+" path received");

                    return Ok(response.Message);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid file type. Only CSV files are allowed.";

                }
            }
            catch (Exception ex)
            {       
                Console.WriteLine(ex);
                response.Message="Kuch to Gadbad hai daya!";

            }
            return Ok(response);
        }
    }
}
