using MatchingApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace MatchingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchingController : ControllerBase
    {
        private readonly ILogger<MatchingController> _logger;

        public MatchingController(ILogger<MatchingController> logger)
        {
            _logger = logger;
        }
    }
    [HttpGet("GetUsers")]
public async Task<IActionResult> GetUsers([FromQuery] Filters filters)
{
    var currentUserId = /* Logic to get the current user ID */;

    var usersQuery = _context.Users
        .Where(u => u.Active) // Return only active users
        .AsQueryable();

    // Apply filters if any (e.g., age range, gender, etc.)
    if (filters.AgeMin.HasValue)
    {
        usersQuery = usersQuery.Where(u => u.Age >= filters.AgeMin.Value);
    }

    if (filters.AgeMax.HasValue)
    {
        usersQuery = usersQuery.Where(u => u.Age <= filters.AgeMax.Value);
    }

    if (!string.IsNullOrEmpty(filters.Gender))
    {
        usersQuery = usersQuery.Where(u => u.Gender == filters.Gender);
    }

    // Fetch users who liked the current user
    var likedByCurrentUser = await _context.Matches
        .Where(m => m.LikedId == currentUserId && m.IsMutual)
        .Select(m => m.LikerId)
        .ToListAsync();

    // Get the top 100 users who liked the current user and shuffle them
    var topUsers = await usersQuery
        .Where(u => likedByCurrentUser.Contains(u.Id))
        .OrderBy(r => Guid.NewGuid()) // Shuffle
        .Take(100)
        .ToListAsync();

    // Get the remaining users
    var otherUsers = await usersQuery
        .Where(u => !likedByCurrentUser.Contains(u.Id))
        .ToListAsync();

    // Combine the two lists
    var allUsers = topUsers.Concat(otherUsers).ToList();

    return Ok(allUsers);
}

[HttpPost("Match")]
public async Task<IActionResult> Match(int likedUserId, bool like)
{
    var currentUserId = /* Logic to get the current user ID */;
    
    var existingMatch = await _context.Matches
        .FirstOrDefaultAsync(m => m.LikerId == currentUserId && m.LikedId == likedUserId);

    if (like)
    {
        if (existingMatch == null)
        {
            // If no match exists, add a new one
            var newMatch = new Match
            {
                LikerId = currentUserId,
                LikedId = likedUserId,
                IsMutual = false,
                DateLiked = DateTime.Now
            };

            _context.Matches.Add(newMatch);
        }

        // Check if the liked user has already liked the current user
        var reciprocalMatch = await _context.Matches
            .FirstOrDefaultAsync(m => m.LikerId == likedUserId && m.LikedId == currentUserId);

        if (reciprocalMatch != null)
        {
            // If so, update both records to be mutual
            existingMatch.IsMutual = true;
            reciprocalMatch.IsMutual = true;
            return Ok("It's a match!");
        }
    }
    else
    {
        // If the current user dislikes the liked user
        if (existingMatch != null)
        {
            _context.Matches.Remove(existingMatch);
        }
    }

    await _context.SaveChangesAsync();
    return Ok();
}
[HttpPost("SendMessage")]
public async Task<IActionResult> SendMessage(int receiverId, [FromBody] string messageContent)
{
    var currentUserId = /* Logic to get the current user ID */;

    // Check if the current user is matched with the receiver
    var isMatched = await _context.Matches
        .AnyAsync(m => (m.LikerId == currentUserId && m.LikedId == receiverId && m.IsMutual) ||
                       (m.LikerId == receiverId && m.LikedId == currentUserId && m.IsMutual));

    if (!isMatched)
    {
        return BadRequest("You can only send messages to matched users.");
    }

    var message = new Message
    {
        SenderId = currentUserId,
        ReceiverId = receiverId,
        Content = messageContent,
        DateSent = DateTime.Now,
        IsRead = false
    };

    _context.Messages.Add(message);
    await _context.SaveChangesAsync();

    return Ok("Message sent successfully.");
}


    
    
}