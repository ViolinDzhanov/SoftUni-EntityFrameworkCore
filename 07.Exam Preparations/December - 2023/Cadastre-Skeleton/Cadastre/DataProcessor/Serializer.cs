﻿using Cadastre.Data;
using Cadastre.DataProcessor.ExportDtos;
using Cadastre.DataProcessor.ImportDtos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor
{
    public class Serializer
    {
        public static string ExportPropertiesWithOwners(CadastreContext dbContext)
        {
            var propertiesData = dbContext.Properties.AsNoTracking()
               .Where(p => p.DateOfAcquisition >= new DateTime(2000, 1, 1))
               .OrderByDescending(p => p.DateOfAcquisition)
               .ThenBy(p => p.PropertyIdentifier)
               .Select(p => new
               {
                   PropertyIdentifier = p.PropertyIdentifier,
                   Area = p.Area,
                   Address = p.Address,
                   DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy"),
                   Owners = p.PropertiesCitizens
                   .Select(pc => pc.Citizen)
                   .OrderBy(c => c.LastName)
                   .Select(c => new
                   {
                       LastName = c.LastName,
                       MaritalStatus = c.MaritalStatus.ToString(),
                   })
                   .ToList()
               })
               .ToList();

            return JsonConvert.SerializeObject(propertiesData, Formatting.Indented);
        }

        public static string ExportFilteredPropertiesWithDistrict(CadastreContext dbContext)
        {
            ExportPropertyDto[] properties = dbContext.Properties.AsNoTracking()
             .Where(p => p.Area >= 100)
            .OrderByDescending(p => p.Area)
             .ThenBy(p => p.DateOfAcquisition)
             .Select(p => new ExportPropertyDto
             {
                 PostalCode = p.District.PostalCode,
                 PropertyIdentifier = p.PropertyIdentifier,
                 Area = p.Area,
                 DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy")
             })
            .ToArray();
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportPropertyDto[]), new XmlRootAttribute("Properties"));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, properties, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
