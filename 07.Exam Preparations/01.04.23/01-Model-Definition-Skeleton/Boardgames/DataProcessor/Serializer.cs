namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.DataProcessor.ExportDto;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var xmlHelper = new XmlHelper();
            var creators = context.Creators
                .Where(c => c.Boardgames.Any())
                .Select(c => new ExportCreatorDto()
                {
                    CreatorName = string.Join(" ", c.FirstName, c.LastName),
                    BoardgamesCount = c.Boardgames.Count(),
                    Boardgames = c.Boardgames
                        .Select(bg => new ExportGameDto()
                        {
                            BoardgameName = bg.Name,
                            BoardgameYearPublished = bg.YearPublished
                        })
                        .OrderBy(bg => bg.BoardgameName)
                        .ToArray()
                })
                .OrderByDescending(c => c.BoardgamesCount)
                .ThenBy(c => c.CreatorName)
                .ToArray();

            return xmlHelper
                .Serialize(creators, "Creators");

        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                .Where(s => s.BoardgamesSellers
                .Any(bg => bg.Boardgame.YearPublished >= year
                    && bg.Boardgame.Rating <= rating))
                .Select(bgs => new
                {
                    Name = bgs.Name,
                    Website = bgs.Website,
                    Boardgames = bgs.BoardgamesSellers
                        .Where(bg => bg.Boardgame.YearPublished >= year
                        && bg.Boardgame.Rating <= rating)
                        .Select(bg => new
                        {
                            Name = bg.Boardgame.Name,
                            Rating = bg.Boardgame.Rating,
                            Mechanics = bg.Boardgame.Mechanics,
                            Category = bg.Boardgame.CategoryType.ToString()
                        })
                        .OrderByDescending(bg => bg.Rating)
                        .ThenBy(bg => bg.Name)
                        .ToList()
                })
                .OrderByDescending(bgs => bgs.Boardgames.Count)
                .ThenBy(bgs => bgs.Name)
                .Take(5)
                .ToList();

            return JsonConvert.SerializeObject(sellers, Formatting.Indented);
        }
    }
}