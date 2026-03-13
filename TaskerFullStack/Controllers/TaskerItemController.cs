using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskerFullStack.Client.Models;
using TaskerFullStack.Data;
using TaskerFullStack.Models;


namespace TaskerFullStack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskerItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskerItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET: api/TaskerItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskerItem>>> GetDbTaskerItems()
        {
            string userId = _userManager.GetUserId(User)!;

            List<DbTaskerItem> items = await _context.TaskerItems
                                                        .Where(t => t.UserId == userId)
                                                        .ToListAsync();
            return items;
        }

        //GET: api/TaskerItem/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskerItem>> GetTaskerItem([FromRoute] Guid id)
        {
            string userId = _userManager.GetUserId(User)!;

            DbTaskerItem? dbTaskerItem = await _context.TaskerItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (dbTaskerItem is null)
            {
                return NotFound();
            }

            return dbTaskerItem;
        }

        //POST: api/TaskerItem
        [HttpPost]
        public async Task<ActionResult<TaskerItem>> PostDbTaskerItem([FromBody] TaskerItem taskerItem)
        {
            DbTaskerItem dbTaskerItem = new();

            //dbTaskerItem.Id = taskerItem.Id;
            dbTaskerItem.Name = taskerItem.Name;
            dbTaskerItem.IsComplete = taskerItem.IsComplete;

            string userId = _userManager.GetUserId(User)!;
            dbTaskerItem.UserId = userId;

            _context.TaskerItems.Add(dbTaskerItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskerItem", new {id =dbTaskerItem.Id}, dbTaskerItem);
        }

        //PUT: api/TaskerItem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDbTaskerItem([FromRoute] Guid id, [FromBody] TaskerItem taskerItem)
        {
            if (id != taskerItem.Id)
            {
                return BadRequest();
            }

            string userId = _userManager.GetUserId(User)!;

            DbTaskerItem? dbTaskerItem = await _context.TaskerItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (dbTaskerItem is null)
            {
                return NotFound();
            }
            else
            {
                dbTaskerItem.Name = taskerItem.Name;
                dbTaskerItem.IsComplete = taskerItem.IsComplete;

                _context.Update(dbTaskerItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        //DELETE: api/TaskerItem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDbTaskerItem([FromRoute] Guid id)
        {
            string userId = _userManager.GetUserId(User)!;

            DbTaskerItem? dbTaskerItem = await _context.TaskerItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (dbTaskerItem is null)
            {
                return NotFound();
            }
            else
            {
                _context.TaskerItems.Remove(dbTaskerItem);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
