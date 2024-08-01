namespace Footballers.DataProcessor
{
    using Boardgames.Helper;
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            ImportCoachDto[] coachesDto = XmlSerializationHelper.Deserialize<ImportCoachDto[]>(xmlString, "Coaches");

            List<Coach> validCoaches = new List<Coach>();
            
            foreach (var cDto in coachesDto)
            {
                if (!IsValid(cDto))
                {
                    sb.AppendLine(ErrorMessage); 
                    continue;
                }

                Coach coach = new Coach()
                {
                    Name = cDto.Name,
                    Nationality = cDto.Nationality,
                };

                foreach (var footballerDto in  cDto.Footballers)
                {
                    DateTime contractStart;
                    DateTime contractEnd;

                    if (!DateTime.TryParseExact(footballerDto.ContractStartDate, 
                        "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out contractStart)
                         || !DateTime.TryParseExact(footballerDto.ContractEndDate, 
                         "dd/MM/yyyy", CultureInfo.InvariantCulture, 
                         DateTimeStyles.None, out contractEnd))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (contractStart > contractEnd)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!IsValid(footballerDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Footballer footballer = new Footballer()
                    {
                        Name = footballerDto.Name,
                        ContractStartDate = contractStart,
                        ContractEndDate = contractEnd,
                        BestSkillType = (BestSkillType)footballerDto.BestSkillType,
                        PositionType = (PositionType)footballerDto.PositionType,
                    };

                    coach.Footballers.Add(footballer);
                }

                sb.AppendLine(string.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count));

                validCoaches.Add(coach);
            }

            context.Coaches.AddRange(validCoaches);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportTeamDto[] teamsDto = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);

            List<Team> validTeams = new List<Team>();

            foreach(var tDto in teamsDto)
            {
                if(!IsValid(tDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (tDto.Trophies <= 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Team team = new Team()
                {
                    Name = tDto.Name,
                    Nationality = tDto.Nationality,
                    Trophies = tDto.Trophies
                };

                var validFootballerIds = context.Footballers.Select(x => x.Id).ToList();

                foreach (var id in tDto.Footballers.Distinct())
                {
                    if (!validFootballerIds.Contains(id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    team.TeamsFootballers.Add(new TeamFootballer
                    {
                        FootballerId = id,
                        Team = team
                    });
                }

                validTeams.Add(team);
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count));
            }

            context.Teams.AddRange(validTeams);
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
