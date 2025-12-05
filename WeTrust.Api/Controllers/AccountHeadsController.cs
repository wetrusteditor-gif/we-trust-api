using Microsoft.AspNetCore.Mvc;
using WeTrust.Api.Data;
using WeTrust.Api.Models;

namespace WeTrust.Api.Controllers
{
    [ApiController]
    [Route("account-heads")]
    public class AccountHeadsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AccountHeadsController(AppDbContext db) { _db = db; }

        [HttpGet]
        public IActionResult GetAll() => Ok(_db.AccountHeads.Where(a => a.IsActive).ToList());

        [HttpPost]
        public IActionResult Create(AccountHead a)
        {
            _db.AccountHeads.Add(a);
            _db.SaveChanges();
            return Ok(a);
        }
    }
}

