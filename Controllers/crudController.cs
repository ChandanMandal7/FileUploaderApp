using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileUploadApp.DataAccessLayer;
using FileUploadApp.CommonLayer.Model;

namespace FileUploadApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        private readonly CrudOperations _crudOperations;

        public CrudController(CrudOperations crudOperations)
        {
            _crudOperations = crudOperations;
        }

        // GET: api/crud/employees
        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<string>>> GetEmployees()
        {
            try
            {
                List<string> employees = await _crudOperations.Getemployees();
                return Ok(employees);
            }
            catch (System.Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{EmailId}")]
        public async Task<IActionResult> GetEmployeeById(String EmailId)
        {
            try
            {
                string employeeDetail = await _crudOperations.GetEmployeeById(EmailId);

                if (string.IsNullOrEmpty(employeeDetail))
                {
                    return NotFound($"Employee with Id {EmailId} not found");
                }

                return Ok(employeeDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        // DELETE: api/employee/{id}
        [HttpDelete("{EmailId}")]
        public async Task<IActionResult> DeleteEmployee(String EmailId)
        {
            try
            {
                // Check if employee exists
                string existingEmployee = await _crudOperations.GetEmployeeById(EmailId);

                if (string.IsNullOrEmpty(existingEmployee))
                {
                    return NotFound($"Employee with EmailId {EmailId} not found");
                }


                int affectedRow = await _crudOperations.DeleteEmployeeById(EmailId);

                if (affectedRow > 0)
                {
                    return Ok($"Employee with EmailId {EmailId} deleted successfully");
                }
                else
                {
                    return Ok($"Employee with EmailId {EmailId} not deleted");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // PUT: api/employee/{id}
        [HttpPut("{EmailId}")]
        public async Task<IActionResult> UpdateEmployee(String EmailId, [FromBody] UpdateEmployeeRequest model)
        {
            try
            {
                // Fetch existing employee details
                string existingEmployee = await _crudOperations.GetEmployeeById(EmailId);

                if (string.IsNullOrEmpty(existingEmployee))
                {
                    return NotFound($"Employee with EmailId {EmailId} not found");
                }

                int affectedRows = await _crudOperations.UpdateEmployee(EmailId, model);

                if (affectedRows > 0)
                {
                    return Ok($"Employee with EmailId {EmailId} updated successfully");
                }
                else
                {
                    return Ok($"Employee with EmailId {EmailId} not updated");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



         [HttpPost]
        public async Task<IActionResult> PostEmployee(UpdateEmployeeRequest model)
        {
            try
            {
              var postedValue= await _crudOperations.PostEmployee(model);
          
               if( postedValue > 0){
                return Ok("Employee added successfully");
               }else{
                return Ok("Employee not added");
               }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{LIMIT}/{pageNo}")]
        public async Task<IActionResult> GetEmployeeOnPage(int LIMIT, int pageNo){

        try{
            var result = await _crudOperations.GetEmployeesOnpage(LIMIT, pageNo);
            return Ok(result);
        }
        catch (Exception ex){
            return StatusCode(500, $"Internal server error: {ex}");
        }

        }
    }
}

