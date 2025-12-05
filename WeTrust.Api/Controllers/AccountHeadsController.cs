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
        public IActionResult GetAll()
        {
            try
            {
                var list = _db.AccountHeads.Where(a => a.IsActive).ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                // Log to console (Render will capture it)
                Console.Error.WriteLine("Exception in /account-heads: " + ex.ToString());
                // Return the exception detail temporarily for debugging (remove later)
                return Problem(title: "exception in account-heads", detail: ex.ToString());
            }
        }

        [HttpPost]
        public IActionResult Create(AccountHead a)
        {
            try
            {
                _db.AccountHeads.Add(a);
                _db.SaveChanges();
                return Ok(a);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception in POST /account-heads: " + ex.ToString());
                return Problem(title: "exception creating account-head", detail: ex.ToString());
            }
        }
    }
}
