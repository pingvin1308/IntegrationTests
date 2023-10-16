using EasyNetQ;
using IntegrationTests.DataAccess;
using IntegrationTests.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IBus _bus;

    public UsersController(AppDbContext context, IBus bus)
    {
        _context = context;
        _bus = bus;
    }

    [HttpGet]
    public async Task<IEnumerable<User>> Get()
    {
        return await _context.Users.AsNoTracking().ToArrayAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        _context.Users.Add(user);
        var result = await _context.SaveChangesAsync();
        if (result <= 0)
        {
            return BadRequest();
        }

        var userCreatedEvent = new UserCreatedEvent
        {
            UserId = user.Id,
            UserName = user.Name,
            UserEmail = user.Email
        };
        await _bus.PubSub.PublishAsync(userCreatedEvent);
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User user)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(existingUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        _context.Users.Remove(existingUser);
        await _context.SaveChangesAsync();
        return Ok();
    }
}