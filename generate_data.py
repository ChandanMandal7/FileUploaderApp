import csv
import random
from faker import Faker
 
fake = Faker()
 
# Function to generate unique email
def generate_unique_email(used_emails):
    while True:
        email = fake.email()
        if email not in used_emails:
            used_emails.add(email)
            return email
 
# Function to generate random data for a single row
def generate_random_row(used_emails):
    name = fake.name()
    email = generate_unique_email(used_emails)
    country = fake.country()
    state = fake.state()
    city = fake.city()
    telephone = fake.random_number(digits=10)
    address_line1 = fake.street_address()
    address_line2 = fake.secondary_address()
    dob = fake.date_of_birth(minimum_age=18, maximum_age=80).strftime('%Y-%m-%d')
    gross_salaries = [random.randint(50000, 150000) for _ in range(5)]
 
    row = {
        "EmailId": email,
        "Name": name,
        "Country": country,
        "State": state,
        "City": city,
        "TelephoneNumber": telephone,
        "AddressLine1": address_line1,
        "AddressLine2": address_line2,
        "DateOfBirth": dob,
        "GrossSalaryFY2019_20": gross_salaries[0],
        "GrossSalaryFY2020_21": gross_salaries[1],
        "GrossSalaryFY2021_22": gross_salaries[2],
        "GrossSalaryFY2022_23": gross_salaries[3],
        "GrossSalaryFY2023_24": gross_salaries[4]
    }
    return row
 
# Function to generate random data for specified number of rows
def generate_random_data(num_rows):
    used_emails = set()
    data = []
    for _ in range(num_rows):
        row = generate_random_row(used_emails)
        data.append(row)
    return data
 
# Function to write data to CSV file
def write_to_csv(data, filename):
    fieldnames = data[0].keys()
    with open(filename, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(data)
    print(f"Data successfully written to {filename}")
 
# Main program
if __name__ == "__main__":
    try:
        num_rows = int(input("Enter number of rows to generate: "))
        if num_rows <= 0:
            raise ValueError("Number of rows must be greater than zero.")
 
        generated_data = generate_random_data(num_rows)
        csv_filename = "generated_data.csv"
        write_to_csv(generated_data, csv_filename)
 
    except ValueError as ve:
        print(f"Error: {ve}")
    except Exception as e:
        print(f"Error: {e}")