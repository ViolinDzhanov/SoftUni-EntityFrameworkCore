namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            Console.WriteLine(GetTotalProfitByCategory(db));

            //IncreasePrices(db);
        }

        //02.Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {

            var bookTitles = context.Books
                .AsEnumerable()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //03.Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenEditionBooks = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .Select(b => new
                {
                    Id = b.BookId,
                    Title = b.Title,
                })
                .OrderBy(t => t.Id)
                .ToList();

            return string.Join(Environment.NewLine, goldenEditionBooks.Select(b => b.Title));
        }

        //04.Books by Price

        public static string GetBooksByPrice(BookShopContext context)
        {
            var expensiveBooks = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price,
                })
                .OrderByDescending(t => t.Price)
                .ToList();

            return string.Join(Environment.NewLine, expensiveBooks.Select(a => $"{a.Title} - ${a.Price:f2}"));
        }

        //05.Not Released In

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var book = context.Books
                .Where(b => b.ReleaseDate.HasValue
                && b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    Id = b.BookId,
                    Title = b.Title,
                })
                .OrderBy(t => t.Id)
                .ToList();

            return string.Join(Environment.NewLine, book.Select(b => b.Title));
        }

        //06.Book Titles by Category

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var booksByCategory = context.BooksCategories
                .Where(bc =>
                    categories.Contains(bc.Category.Name.ToLower()))
                .Select(bc => bc.Book.Title)
                .OrderBy(t => t)
                .ToArray();

            return string.Join(Environment.NewLine, booksByCategory);
        }

        //07.Released Before Date

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate < dt)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    Title = b.Title,
                    Edition = b.EditionType,
                    Price = b.Price
                })
                .ToArray();

            return string.Join(Environment.NewLine,
                books.Select(b => $"{b.Title} - {b.Edition} - ${b.Price:f2}"));
        }

        //08.Author Search

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
              .Where(a => a.FirstName.EndsWith(input))
              .Select(a => $"{a.FirstName} {a.LastName}")
              .ToArray()
              .OrderBy(n => n);

            return string.Join(Environment.NewLine, authors);
        }

        //09.Book Search

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var inputToLower = input.ToLower();

            var bookTitles = context.Books
                .Where(b => b.Title.ToLower().Contains(inputToLower))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //10.Book Search by Author

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var inputToLower = input.ToLower();

            var autorsWithBooks = context.Books
                .Where(a => a.Author.LastName.ToLower().StartsWith(inputToLower))
                .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
                .ToArray();

            return string.Join(Environment.NewLine, autorsWithBooks);
        }

        //11.Count Books

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Count(b => b.Title.Length > lengthCheck);
        }

        //12.Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var books = context.Authors
                .Select(a => new
                {
                    FullName = $"{a.FirstName} {a.LastName}",
                    BooksCount = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.BooksCount)
                .ToArray();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.FullName} - {b.BooksCount}"));
        }

        //13.	Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitCategory = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Profit = c.CategoryBooks
                        .Sum(cb => cb.Book.Price * cb.Book.Copies)
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.CategoryName)
                .ToArray();

            return string.Join(Environment.NewLine, profitCategory.Select(c => $"{c.CategoryName} ${c.Profit:f2}"));
        }

        //14.Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var mostRecentBooks = context.Categories
                 .Select(c => new
                 {
                     c.Name,
                     NewestBooks = c.CategoryBooks
                        .OrderByDescending(b => b.Book.ReleaseDate)
                        .Take(3)
                        .Select(b => $"{b.Book.Title} ({b.Book.ReleaseDate.Value.Year})")
                 }).ToArray()
                .OrderBy(c => c.Name)
                .ToArray();

            StringBuilder sb = new();

            foreach (var c in mostRecentBooks)
            {
                sb.AppendLine($"--{c.Name}");

                foreach (var nb in c.NewestBooks)
                {
                    sb.AppendLine(nb);
                }
            }

            return sb.ToString().TrimEnd();
        }

        //15.Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var b in books)
            {
                b.Price += 5;
            }

            context.SaveChanges();
        }

        //16.Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.RemoveRange(booksToRemove);
            context.SaveChanges();

            return booksToRemove.Length;
        }
    }
}


