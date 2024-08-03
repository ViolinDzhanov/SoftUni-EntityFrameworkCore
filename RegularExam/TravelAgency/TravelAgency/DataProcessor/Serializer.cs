using Boardgames.Helper;
using Newtonsoft.Json;
using TravelAgency.Data;
using TravelAgency.Data.Models;
using TravelAgency.Data.Models.Enums;
using TravelAgency.DataProcessor.ExportDtos;

namespace TravelAgency.DataProcessor
{
    public class Serializer
    {
        public static string ExportGuidesWithSpanishLanguageWithAllTheirTourPackages(TravelAgencyContext context)
        {
            var guides = context.Guides
                .Where(g => g.Language.ToString() == "Spanish")
                .Select(g => new ExportGuideDto()
                {
                    FullName = g.FullName,
                    TourPackages = g.TourPackagesGuides
                    .Select(p => new ExportTourPackageDto()
                    {
                        Name = p.TourPackage.PackageName,
                        Description = p.TourPackage.Description,
                        Price = p.TourPackage.Price,
                    })
                    .OrderByDescending(p => p.Price)
                    .ThenBy(p => p.Name)
                    .ToArray()
                })
                .OrderByDescending(g => g.TourPackages.Count())
                .ThenBy(g => g.FullName)
                .ToArray();

            return XmlSerializationHelper.Serialize(guides, "Guides");
        }

        public static string ExportCustomersThatHaveBookedHorseRidingTourPackage(TravelAgencyContext context)
        {
            var customers = context.Customers
                .Where(c => c.Bookings.Any(b => b.TourPackage.PackageName == "Horse Riding Tour"))
                .Select(c => new
                {
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Bookings = c.Bookings
                    .Where(b => b.TourPackage.PackageName == "Horse Riding Tour")
                    .Select(b => new
                    {
                        TourPackageName = b.TourPackage.PackageName,
                        Date = b.BookingDate.ToString("yyyy-MM-dd")
                    })
                    .OrderBy(b => b.Date)
                    .ToArray(),
                })
                .OrderByDescending(c => c.Bookings.Count())
                .ThenBy (c => c.FullName)
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }
    }
}
