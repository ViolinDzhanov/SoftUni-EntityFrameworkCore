namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Metrics;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        private static XmlHelper? xmlHelper;

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportPatientDto[] patientsDto = JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString);
            ICollection<Patient> validPatients = new List<Patient>();

            foreach (var pDto in patientsDto)
            {
                if (!IsValid(pDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Patient patient = new Patient()
                {
                    FullName = pDto.FullName,
                    AgeGroup = (AgeGroup)pDto.AgeGroup,
                    Gender = (Gender)pDto.Gender,
                };

                foreach (var medId in pDto.Medicines)
                {
                    if (patient.PatientsMedicines.Any(m => m.MedicineId == medId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    PatientMedicine patientMedicine = new PatientMedicine()
                    {
                        Patient = patient,
                        MedicineId = medId,
                    };
                    patient.PatientsMedicines.Add(patientMedicine);
                }
                validPatients.Add(patient);
                sb.AppendLine(string.Format(SuccessfullyImportedPatient,
                    patient.FullName, patient.PatientsMedicines.Count));
            }

            context.AddRange(validPatients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            int medicinesCounter = 0;
            xmlHelper = new XmlHelper();

            ImportPharmacyDto[] pharmacyDtos = xmlHelper.Deserialize<ImportPharmacyDto[]>(xmlString, "Pharmacies");

            ICollection<Pharmacy> validPharmacies = new List<Pharmacy>();

            foreach (var pDtos in pharmacyDtos)
            {
                if (!IsValid(pDtos))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Pharmacy pharmacy = new Pharmacy()
                {
                    Name = pDtos.Name,
                    PhoneNumber = pDtos.PhoneNumber,
                    IsNonStop = bool.Parse(pDtos.IsNonStop)
                };

                foreach (var mDto in pDtos.Medicines)
                {
                    if (!IsValid(mDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime productionDate;
                    bool isProductionDateValid = DateTime
                        .TryParseExact(mDto.ProductionDate, "yyyy-MM-dd", CultureInfo
                        .InvariantCulture, DateTimeStyles.None, out productionDate);
                    
                    if (!isProductionDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime expireDate;
                    bool isExpireDateValid = DateTime
                        .TryParseExact(mDto.ExpiryDate, "yyyy-MM-dd", CultureInfo
                        .InvariantCulture, DateTimeStyles.None, out expireDate);

                    if (!isExpireDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (productionDate >= expireDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (pharmacy.Medicines.Any(x =>  x.Name == mDto.Name && x.Producer == mDto.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Medicine medicine = new Medicine()
                    {
                        Name = mDto.Name,
                        Price = (decimal)mDto.Price,
                        Category = (Category)mDto.Category,
                        ProductionDate = productionDate,
                        ExpiryDate = expireDate,
                        Producer = mDto.Producer,
                    };

                    medicinesCounter++;
                    pharmacy.Medicines.Add(medicine);
                }

                validPharmacies.Add(pharmacy);
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count));
            }

            context.Pharmacies.AddRange(validPharmacies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
