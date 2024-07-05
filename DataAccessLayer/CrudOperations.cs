using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FileUploadApp.CommonLayer.Model;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx;
namespace FileUploadApp.DataAccessLayer
{

    public class CrudOperations
    {

        private readonly IConfiguration _configuration;
        private readonly MySqlConnection _mySqlConnection;

        public CrudOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            _mySqlConnection = new MySqlConnection(_configuration["ConnectionStrings:MySqlDBConnectionString"]);
        }


        public void IsConnected()
        {
            if (_mySqlConnection.State != ConnectionState.Open)
            {
                _mySqlConnection.Open();

            }
        }


        //get all the employesss-------------------------------------------------

        public async Task<List<string>> Getemployees()
        {
            List<string> employees = new List<string>();

            string query = " SELECT * FROM userrecords LIMIT 10";

            IsConnected();
            MySqlCommand command = new MySqlCommand(query, _mySqlConnection);

            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {

                string emailId = reader["EmailId"].ToString();
                string name = reader["Name"].ToString();
                string country = reader["Country"].ToString();
                string state = reader["State"].ToString();
                string city = reader["City"].ToString();
                string telephoneNumber = reader["TelephoneNumber"].ToString();
                string addressLine1 = reader["AddressLine1"].ToString();
                string addressLine2 = reader["AddressLine2"].ToString();
                DateTime dateOfBirth = (DateTime)reader["DateOfBirth"];
                decimal grossSalaryFY2019_20 = (decimal)reader["GrossSalaryFY2019_20"];
                decimal grossSalaryFY2020_21 = (decimal)reader["GrossSalaryFY2020_21"];
                decimal grossSalaryFY2021_22 = (decimal)reader["GrossSalaryFY2021_22"];
                decimal grossSalaryFY2022_23 = (decimal)reader["GrossSalaryFY2022_23"];
                decimal grossSalaryFY2023_24 = (decimal)reader["GrossSalaryFY2023_24"];

                // Constructing a string with all values for presenting it
                string employeeInfo = $"EmailId: {emailId}, Name: {name}, Country: {country}, State: {state}, City: {city}, " +
                                      $"Telephone Number: {telephoneNumber}, Address Line 1: {addressLine1}, Address Line 2: {addressLine2}, " +
                                      $"Date of Birth: {dateOfBirth}, " +
                                      $"Gross Salary FY2019_20: {grossSalaryFY2019_20}, Gross Salary FY2020_21: {grossSalaryFY2020_21}, " +
                                      $"Gross Salary FY2021_22: {grossSalaryFY2021_22}, Gross Salary FY2022_23: {grossSalaryFY2022_23}, " +
                                      $"Gross Salary FY2023_24: {grossSalaryFY2023_24}";

                // Adding employee information to the list
                employees.Add(employeeInfo);
            }

            reader.Close();
            return employees;
        }


        //get employee by id wala logic-----------------------------------
        public async Task<string> GetEmployeeById(string emailId)
        {
            string employeeDetail = string.Empty;

            try
            {
                string query = "SELECT * FROM userrecords WHERE EmailId = @EmailId";

                IsConnected();

                MySqlCommand command = new MySqlCommand(query, _mySqlConnection);

                command.Parameters.AddWithValue("@EmailId", emailId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    // Read employee details from the reader
                    string name = reader["Name"].ToString();
                    string country = reader["Country"].ToString();
                    string state = reader["State"].ToString();
                    string city = reader["City"].ToString();
                    string telephoneNumber = reader["TelephoneNumber"].ToString();
                    string addressLine1 = reader["AddressLine1"].ToString();
                    string addressLine2 = reader["AddressLine2"].ToString();
                    DateTime dateOfBirth = (DateTime)reader["DateOfBirth"];
                    decimal grossSalaryFY2019_20 = (decimal)reader["GrossSalaryFY2019_20"];
                    decimal grossSalaryFY2020_21 = (decimal)reader["GrossSalaryFY2020_21"];
                    decimal grossSalaryFY2021_22 = (decimal)reader["GrossSalaryFY2021_22"];
                    decimal grossSalaryFY2022_23 = (decimal)reader["GrossSalaryFY2022_23"];
                    decimal grossSalaryFY2023_24 = (decimal)reader["GrossSalaryFY2023_24"];

                    // Construct a string of the employee details to return
                    employeeDetail = $"EmailId: {emailId}, Name: {name}, Country: {country}, State: {state}, City: {city}, " +
                                     $"Telephone Number: {telephoneNumber}, Address Line 1: {addressLine1}, Address Line 2: {addressLine2}, " +
                                     $"Date of Birth: {dateOfBirth}, " +
                                     $"Gross Salary FY2019_20: {grossSalaryFY2019_20}, Gross Salary FY2020_21: {grossSalaryFY2020_21}, " +
                                     $"Gross Salary FY2021_22: {grossSalaryFY2021_22}, Gross Salary FY2022_23: {grossSalaryFY2022_23}, " +
                                     $"Gross Salary FY2023_24: {grossSalaryFY2023_24}";
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return employeeDetail;
        }




        //delete employee from record --logic---------------------------------------------------------------------------------------------------
        public async Task<Int32> DeleteEmployeeById(String EmailId)
        {

            string query = "DELETE FROM userrecords WHERE EmailId = @EmailId";


            IsConnected(); // Assuming this method ensures the connection is open

            MySqlCommand command = new MySqlCommand(query, _mySqlConnection);
            command.Parameters.AddWithValue("@EmailId", EmailId);
            
            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected;


        }
[HttpPut]
        //update the employee wala section--logic------------------------------------------------------------------------------
        public async Task<Int32> UpdateEmployee(String EmailId, UpdateEmployeeRequest model)
        {
            try
            {

                string dateOfBirthString = model.DateOfBirth.ToString("yyyy-MM-dd");

                string query = @"UPDATE  userrecords 
                                 SET   Name = @Name, 
                                    Country = @Country, 
                                    State = @State, 
                                    City = @City, 
                                    TelephoneNumber = @TelephoneNumber, 
                                    AddressLine1 = @AddressLine1, 
                                    AddressLine2 = @AddressLine2, 
                                    DateOfBirth = @DateOfBirth, 
                                    GrossSalaryFY2019_20 = @GrossSalaryFY2019_20, 
                                    GrossSalaryFY2020_21 = @GrossSalaryFY2020_21, 
                                    GrossSalaryFY2021_22 = @GrossSalaryFY2021_22, 
                                    GrossSalaryFY2022_23 = @GrossSalaryFY2022_23, 
                                    GrossSalaryFY2023_24 = @GrossSalaryFY2023_24 
                                WHERE EmailId = @EmailId";


                IsConnected();

                MySqlCommand command = new MySqlCommand(query, _mySqlConnection);
                command.Parameters.AddWithValue("@EmailId", EmailId);

               

                command.Parameters.AddWithValue("@Name", model.Name);
                command.Parameters.AddWithValue("@Country", model.Country);
                command.Parameters.AddWithValue("@State", model.State);
                command.Parameters.AddWithValue("@City", model.City);
                command.Parameters.AddWithValue("@TelephoneNumber", model.TelephoneNumber);
                command.Parameters.AddWithValue("@AddressLine1", model.AddressLine1);
                command.Parameters.AddWithValue("@AddressLine2", model.AddressLine2);
                command.Parameters.AddWithValue("@DateOfBirth", dateOfBirthString);
                command.Parameters.AddWithValue("@GrossSalaryFY2019_20", model.GrossSalaryFY2019_20);
                command.Parameters.AddWithValue("@GrossSalaryFY2020_21", model.GrossSalaryFY2020_21);
                command.Parameters.AddWithValue("@GrossSalaryFY2021_22", model.GrossSalaryFY2021_22);
                command.Parameters.AddWithValue("@GrossSalaryFY2022_23", model.GrossSalaryFY2022_23);
                command.Parameters.AddWithValue("@GrossSalaryFY2023_24", model.GrossSalaryFY2023_24);
              


                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return 0;

            }
        }





        //post the employ logic----------------------------------------------------------------------------
        [HttpPost]
        public async Task<Int32> PostEmployee(UpdateEmployeeRequest model)
        {
            try
            {
                string dateOfBirthString = model.DateOfBirth.ToString("yyyy-MM-dd");

                string query = @"INSERT INTO userrecords 
                        (EmailId, Name, Country, State, City, TelephoneNumber, 
                         AddressLine1, AddressLine2, DateOfBirth, 
                         GrossSalaryFY2019_20, GrossSalaryFY2020_21, GrossSalaryFY2021_22, 
                         GrossSalaryFY2022_23, GrossSalaryFY2023_24)
                        VALUES
                        (@EmailId, @Name, @Country, @State, @City, @TelephoneNumber, 
                         @AddressLine1, @AddressLine2, @DateOfBirth, 
                         @GrossSalaryFY2019_20, @GrossSalaryFY2020_21, @GrossSalaryFY2021_22, 
                         @GrossSalaryFY2022_23, @GrossSalaryFY2023_24)";

                IsConnected();
                MySqlCommand command = new MySqlCommand(query, _mySqlConnection);
                // Add values to the command parameters
                command.Parameters.AddWithValue("@EmailId", model.EmailId);
                command.Parameters.AddWithValue("@Name", model.Name);
                command.Parameters.AddWithValue("@Country", model.Country);
                command.Parameters.AddWithValue("@State", model.State);
                command.Parameters.AddWithValue("@City", model.City);
                command.Parameters.AddWithValue("@TelephoneNumber", model.TelephoneNumber);
                command.Parameters.AddWithValue("@AddressLine1", model.AddressLine1);
                command.Parameters.AddWithValue("@AddressLine2", model.AddressLine2);
                command.Parameters.AddWithValue("@DateOfBirth", dateOfBirthString);
                command.Parameters.AddWithValue("@GrossSalaryFY2019_20", model.GrossSalaryFY2019_20);
                command.Parameters.AddWithValue("@GrossSalaryFY2020_21", model.GrossSalaryFY2020_21);
                command.Parameters.AddWithValue("@GrossSalaryFY2021_22", model.GrossSalaryFY2021_22);
                command.Parameters.AddWithValue("@GrossSalaryFY2022_23", model.GrossSalaryFY2022_23);
                command.Parameters.AddWithValue("@GrossSalaryFY2023_24", model.GrossSalaryFY2023_24);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

       


public async Task<List<String>> GetEmployeesOnpage(int LIMIT ,int pageNo){

    List<String > employees=new List<string>();

     IsConnected();
     
     int offSet = (pageNo-1)*LIMIT;
     
     String query=$"SELECT * FROM userrecords LIMIT {LIMIT} OFFSET {offSet}";

        MySqlCommand command=new MySqlCommand(query,_mySqlConnection);
        
       var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {

                string emailId = reader["EmailId"].ToString();
                string name = reader["Name"].ToString();
                string country = reader["Country"].ToString();
                string state = reader["State"].ToString();
                string city = reader["City"].ToString();
                string telephoneNumber = reader["TelephoneNumber"].ToString();
                string addressLine1 = reader["AddressLine1"].ToString();
                string addressLine2 = reader["AddressLine2"].ToString();
                DateTime dateOfBirth = (DateTime)reader["DateOfBirth"];
                decimal grossSalaryFY2019_20 = (decimal)reader["GrossSalaryFY2019_20"];
                decimal grossSalaryFY2020_21 = (decimal)reader["GrossSalaryFY2020_21"];
                decimal grossSalaryFY2021_22 = (decimal)reader["GrossSalaryFY2021_22"];
                decimal grossSalaryFY2022_23 = (decimal)reader["GrossSalaryFY2022_23"];
                decimal grossSalaryFY2023_24 = (decimal)reader["GrossSalaryFY2023_24"];

                // Constructing a string with all values for presenting it
                string employeeInfo = $"EmailId: {emailId}, Name: {name}, Country: {country}, State: {state}, City: {city}, " +
                                      $"Telephone Number: {telephoneNumber}, Address Line 1: {addressLine1}, Address Line 2: {addressLine2}, " +
                                      $"Date of Birth: {dateOfBirth}, " +
                                      $"Gross Salary FY2019_20: {grossSalaryFY2019_20}, Gross Salary FY2020_21: {grossSalaryFY2020_21}, " +
                                      $"Gross Salary FY2021_22: {grossSalaryFY2021_22}, Gross Salary FY2022_23: {grossSalaryFY2022_23}, " +
                                      $"Gross Salary FY2023_24: {grossSalaryFY2023_24}";

                // Adding employee information to the list
                employees.Add(employeeInfo);
            }

            reader.Close();
            return employees;
        

}







    }
}



