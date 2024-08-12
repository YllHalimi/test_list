using MatchingApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ApplicationDbContext _context;

        public UsersController(ILogger<UsersController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("TopActiveUsers")]
        public async Task<IActionResult> GetTopActiveUsers(int n = 5)
        {
            var topUsers = await _context.Users
                .Where(u => u.Active)
                .OrderByDescending(u => u.Credits)
                .Take(n)
                .ToListAsync();

            return Ok(topUsers);
        }

        [HttpGet("AverageCreditsByGender")]
        public async Task<IActionResult> GetAverageCreditsByGender()
        {
            var averageCreditsByGender = await _context.Users
                .Where(u => u.Active)
                .GroupBy(u => u.Gender)
                .Select(g => new
                {
                    Gender = g.Key,
                    AverageCredits = g.Average(u => u.Credits)
                })
                .ToListAsync();

            return Ok(averageCreditsByGender);
        }

        [HttpGet("YoungestAndOldestActiveUsers")]
        public async Task<IActionResult> GetYoungestAndOldestActiveUsers()
        {
            var youngestUser = await _context.Users
                .Where(u => u.Active)
                .OrderBy(u => u.Age)
                .FirstOrDefaultAsync();

            var oldestUser = await _context.Users
                .Where(u => u.Active)
                .OrderByDescending(u => u.Age)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                YoungestUser = youngestUser,
                OldestUser = oldestUser
            });
        }

        [HttpGet("TotalCreditsByAgeGroup")]
        public async Task<IActionResult> GetTotalCreditsByAgeGroup()
        {
            var totalCreditsByAgeGroup = await _context.Users
                .Where(u => u.Active)
                .GroupBy(u =>
                    u.Age <= 15 ? "0-15" :
                    u.Age <= 30 ? "15-30" :
                    u.Age <= 45 ? "30-45" :
                    u.Age <= 60 ? "45-60" :
                    u.Age <= 75 ? "60-75" :
                    u.Age <= 90 ? "75-90" : "90-105")
                .Select(g => new
                {
                    AgeGroup = g.Key,
                    TotalCredits = g.Sum(u => u.Credits)
                })
                .ToListAsync();

            return Ok(totalCreditsByAgeGroup);
        }
    }
}
