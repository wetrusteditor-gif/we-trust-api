using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeTrust.Api.Data;
using WeTrust.Api.Models;

namespace WeTrust.Api.Controllers
{
    [ApiController]
    [Route("account-heads")]
    public class AccountHeadsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountHeadsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /account-heads
        [HttpGet]
        [Produces("application/json")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<IEnumerable<AccountHead>>> GetAll()
        {
            Console.WriteLine($"[AccountHeads] GET all at {DateTime.UtcNow:o}");

            var list = await _context.AccountHeads
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine($"[AccountHeads] Returned {list.Count} rows at {DateTime.UtcNow:o}");

            return Ok(list); // always returns JSON body: [] or [ ... ]
        }

        // GET /account-heads/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccountHead>> GetById(int id)
        {
            var entity = await _context.AccountHeads
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountHeadId == id);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        // POST /account-heads
        [HttpPost]
        public async Task<ActionResult<AccountHead>> Create([FromBody] AccountHead model)
        {
            if (model == null)
                return BadRequest("Body is required.");

            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest("Name is required.");

            // Ensure EF treats this as a new row; DB will generate the ID
            model.AccountHeadId = 0;

            if (string.IsNullOrWhiteSpace(model.OpeningBalanceType))
                model.OpeningBalanceType = "D"; // default, adjust as you like

            _context.AccountHeads.Add(model);
            await _context.SaveChangesAsync();

            // Always return JSON body so Swagger and curl both see it
            return Ok(model);
        }

        // PUT /account-heads/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AccountHead>> Update(int id, [FromBody] AccountHead model)
        {
            if (model == null)
                return BadRequest("Body is required.");

            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest("Name is required.");

            var existing = await _context.AccountHeads
                .FirstOrDefaultAsync(a => a.AccountHeadId == id);

            if (existing == null)
                return NotFound();

            existing.Name = model.Name;
            existing.Category = model.Category;
            existing.IsActive = model.IsActive;
            existing.OpeningBalance = model.OpeningBalance;
            existing.OpeningBalanceType = model.OpeningBalanceType;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // DELETE /account-heads/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.AccountHeads
                .FirstOrDefaultAsync(a => a.AccountHeadId == id);

            if (existing == null)
                return NotFound();

            _context.AccountHeads.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
