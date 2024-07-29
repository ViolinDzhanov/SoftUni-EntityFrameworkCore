namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using AutoMapper.Execution;
    using Boardgames.Helper;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();


            ICollection<Client> clients = new List<Client>();

            ImportClientDto[] importClientDtos = XmlSerializationHelper.Deserialize<ImportClientDto[]>(xmlString, "Clients");

            foreach (var clientDto in importClientDtos)
            {
                if (!IsValid(clientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                ICollection<Address> addressesToImport = new List<Address>();

                foreach (var addressDto in clientDto.Addresses)
                {
                    if (!IsValid(addressDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address address = new Address()
                    {
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country,
                    };

                    addressesToImport.Add(address);
                }
                Client client = new Client()
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat,
                    Addresses = addressesToImport
                };

                clients.Add(client);
                sb.AppendLine(string.Format(SuccessfullyImportedClients, clientDto.Name));
            }

            context.Clients.AddRange(clients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            ICollection<Invoice> invoicesToImport = new List<Invoice>();

            ImportInvoiceDto[] deserializedInvoices =
                JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString)!;
            foreach (ImportInvoiceDto invoiceDto in deserializedInvoices)
            {
                if (!IsValid(invoiceDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isIssueDateValid = DateTime
                    .TryParse(invoiceDto.IssueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime issueDate);
                bool isDueDateValid = DateTime
                    .TryParse(invoiceDto.DueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate);
                if (isIssueDateValid == false || isDueDateValid == false ||
                    DateTime.Compare(dueDate, issueDate) < 0)
                {
                    // DateTime.Compare(t1, t2)
                    //  -> -1 - t1 is before t2
                    //  -> 0 - t1 is t2
                    //  -> 1 - t1 is after t2
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!context.Clients.Any(cl => cl.Id == invoiceDto.ClientId))
                {
                    // Non-existing client
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice newInvoice = new Invoice()
                {
                    Number = invoiceDto.Number,
                    IssueDate = issueDate,
                    DueDate = dueDate,
                    Amount = invoiceDto.Amount,
                    CurrencyType = (CurrencyType)invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId
                };

                invoicesToImport.Add(newInvoice);
                sb.AppendLine(String.Format(SuccessfullyImportedInvoices, invoiceDto.Number));
            }

            context.Invoices.AddRange(invoicesToImport);
            context.SaveChanges();

            return sb.ToString();
        }


    public static string ImportProducts(InvoicesContext context, string jsonString)
    {
            var sb = new StringBuilder();
            
            var productsDto = JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString);

            ICollection<Product> validProducts = new List<Product>();

            foreach (var product in productsDto)
            {
                if (!IsValid(product))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Product newProduct = new Product()
                {
                    Name = product.Name,
                    Price = product.Price,
                    CategoryType = (CategoryType)product.CategoryType,
                };

                ICollection<ProductClient> productClients = new List<ProductClient>();

                var validIds = context.Clients
                    .Select(c => c.Id)
                    .ToList();

                foreach (var id in product.Clients.Distinct())
                {
                    if (!validIds.Contains(id))
                    {
                        sb .AppendLine(ErrorMessage); 
                        continue;
                    }

                    ProductClient newProductClient = new ProductClient()
                    {
                        Product = newProduct,
                        ClientId = id,
                    };

                    productClients.Add(newProductClient);
                }

                validProducts.Add(newProduct);

                sb.AppendLine(string.Format(SuccessfullyImportedProducts, product.Name, productClients.Count));
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
    }

    public static bool IsValid(object dto)
    {
        var validationContext = new ValidationContext(dto);
        var validationResult = new List<ValidationResult>();

        return Validator.TryValidateObject(dto, validationContext, validationResult, true);
    }
} 
}
