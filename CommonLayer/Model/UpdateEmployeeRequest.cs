using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadApp.CommonLayer.Model
{
    public class UpdateEmployeeRequest
{
    public string EmailId { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string TelephoneNumber { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal GrossSalaryFY2019_20 { get; set; }
    public decimal GrossSalaryFY2020_21 { get; set; }
    public decimal GrossSalaryFY2021_22 { get; set; }
    public decimal GrossSalaryFY2022_23 { get; set; }
    public decimal GrossSalaryFY2023_24 { get; set; }
}
}
