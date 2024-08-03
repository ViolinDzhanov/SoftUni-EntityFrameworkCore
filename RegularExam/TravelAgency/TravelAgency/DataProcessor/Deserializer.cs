using Boardgames.Helper;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using TravelAgency.Data;
using TravelAgency.Data.Models;
using TravelAgency.DataProcessor.ImportDtos;

namespace TravelAgency.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedCustomer = "Successfully imported customer - {0}";
        private const string SuccessfullyImportedBooking = "Successfully imported booking. TourPackage: {0}, Date: {1}";

        public static string ImportCustomers(TravelAgencyContext context, string xmlString)
        {
            ImportCustomerDto[] customersDto = XmlSerializationHelper
                .Deserialize<ImportCustomerDto[]>(xmlString, "Customers");

            List<Customer> validCustomers = new List<Customer>();

            StringBuilder sb = new StringBuilder();

            foreach (var cDto in customersDto)
            {
                if (!IsValid(cDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (validCustomers.Any(c => c.FullName == cDto.FullName 
                    || c.Email == cDto.Email 
                    || c.PhoneNumber == cDto.PhoneNumber))
                {
                    sb.AppendLine(DuplicationDataMessage);
                    continue;
                }

                Customer customer = new Customer()
                {
                    FullName = cDto.FullName,
                    Email = cDto.Email,
                    PhoneNumber = cDto.PhoneNumber,
                };

                validCustomers.Add(customer);
                sb.AppendLine(string.Format(SuccessfullyImportedCustomer, customer.FullName));
            }

            context.Customers.AddRange(validCustomers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportBookings(TravelAgencyContext context, string jsonString)
        {
            ImportBookingsDto[] bookingsDto = JsonConvert.DeserializeObject<ImportBookingsDto[]>(jsonString);
            StringBuilder sb = new StringBuilder();
            List<Booking> validBookings = new List<Booking>();

            foreach (var bDto in bookingsDto)
            {
                if (!IsValid(bDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Customer customer = context.Customers
                .Where(c => c.FullName == bDto.CustomerName)
                .FirstOrDefault();

                TourPackage tourPackages = context.TourPackages
                    .Where(tp => tp.PackageName == bDto.TourPackageName)
                    .FirstOrDefault();

                bool isDateValid = DateTime
                    .TryParseExact(bDto.BookingDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime bookingDate);

                if (!isDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }           

                Booking booking = new Booking()
                {
                    BookingDate = bookingDate,
                    CustomerId = customer.Id,
                    TourPackageId = tourPackages.Id
                };

                validBookings.Add(booking);
                sb.AppendLine(string.Format(SuccessfullyImportedBooking,
                    bDto.TourPackageName, booking.BookingDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }

            context.Bookings.AddRange(validBookings);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validateContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                string currValidationMessage = validationResult.ErrorMessage;
            }

            return isValid;
        }
    }
}
