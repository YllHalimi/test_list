using CsvHelper;
using MatchingApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MatchingApp.Data.Seed
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DbInitializer(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Initialize()
        {
            try
            {
                if (_applicationDbContext.Database.GetPendingMigrations().Any())
                {
                    _applicationDbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, possibly log them
            }

            var dataExisting = _applicationDbContext.Users.Any();
            if (!dataExisting)
            {
                var dataToBeSeed = ReadData("C:\\Path\\To\\Your\\ApplicationData.csv"); // Provide the correct path to your CSV file

                if (dataToBeSeed != null && dataToBeSeed.Count > 0)
                {
                    _applicationDbContext.Users.AddRange(dataToBeSeed);
                    _applicationDbContext.SaveChanges();
                }
            }
        }

        public List<User> ReadData(string path)
        {
            List<User> records = new();

            try
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Reading the data from the CSV file and converting it to a list of User objects
                    records = csv.GetRecords<User>().ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions related to file reading or parsing
            }

            return records;
        }
    }
}
