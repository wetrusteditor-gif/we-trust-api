using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeTrust.Api.Data;
using WeTrust.Api.Models;

namespace WeTrust.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountHeadsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountHeadsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountHead>>> GetAll()
        {
            var list = await _context.AccountHeads
                .AsNoTracking()
                .ToListAsync();   // line 19 in your stack trace

            return Ok(list);
        }
    }
}
