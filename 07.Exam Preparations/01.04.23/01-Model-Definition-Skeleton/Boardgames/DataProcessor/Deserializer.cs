namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var xmlHelper = new XmlHelper();

            var creatorsDto = xmlHelper
                .Deserialize<ImportCreatorDto[]>(xmlString, "Creators");

            List<Creator> validCreators = new List<Creator>();

            foreach (var crDto in creatorsDto)
            {
                if (!IsValid(crDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Creator creator = new Creator()
                {
                    FirstName = crDto.FirstName,
                    LastName = crDto.LastName,
                };

                foreach (var gameDto in crDto.Boardgame)
                {
                    if (!IsValid(gameDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame boardgame = new Boardgame()
                    {
                        Name = gameDto.Name,
                        Rating = gameDto.Rating,
                        YearPublished = gameDto.YearPublished,
                        CategoryType = (CategoryType)gameDto.CategoryType,
                        Mechanics = gameDto.Mechanics,
                    };
                    creator.Boardgames.Add(boardgame);
                }

                validCreators.Add(creator);
                sb.AppendLine(string.Format
                    (SuccessfullyImportedCreator, creator.FirstName, creator.LastName, creator.Boardgames.Count));
            }

            context.Creators.AddRange(validCreators);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var sellersDto = JsonConvert.DeserializeObject<ImportSellersDto[]>(jsonString);

            List<Seller> validSellers = new List<Seller>();

            var validGamesIds = context.Boardgames
                .Select(x => x.Id)
                .ToList();

            foreach (var sDto in sellersDto)
            {
                if (!IsValid(sDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var seller = new Seller()
                {
                    Name = sDto.Name,
                    Address = sDto.Address,
                    Country = sDto.Country,
                    Website = sDto.Website,
                };

                foreach (var id in sDto.BoardGamesId.Distinct())
                {
                    if (!validGamesIds.Contains(id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    BoardgameSeller boardgameSeller = new BoardgameSeller()
                    {
                        Seller = seller,
                        BoardgameId = id
                    };
                    seller.BoardgamesSellers.Add(boardgameSeller);
                }

                validSellers.Add(seller);

                sb.AppendLine(string.Format(SuccessfullyImportedSeller, seller.Name, seller.BoardgamesSellers.Count()));
            }

            context.Sellers.AddRange(validSellers);
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
