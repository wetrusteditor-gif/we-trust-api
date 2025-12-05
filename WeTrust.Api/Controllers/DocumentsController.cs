using Microsoft.AspNetCore.Mvc;
using WeTrust.Api.Data;
using WeTrust.Api.Models;

namespace WeTrust.Api.Controllers
{
    [ApiController]
    [Route("documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DocumentsController(AppDbContext db) { _db = db; }

        [HttpPost("receipt")]
        public IActionResult CreateReceipt([FromBody] CreateReceiptRequest req)
        {
            if (req.Amount <= 0) return BadRequest("Amount must be > 0");
            var doc = new Document {
                DocType = "Receipt",
                DocNumber = $"RCP-{DateTime.UtcNow.Year}-{new Random().Next(1000,999999):D6}",
                DocDate = req.DocDate,
                PaymentMode = req.PaymentMode,
                Description = req.Description,
                CreatedByUserId = null
            };
            _db.Documents.Add(doc);
            _db.SaveChanges();

            // Create simple journal lines (Debit = Income, Credit = Cash)
            var jl1 = new JournalLine { DocumentId = doc.DocumentId, AccountHeadId = req.AccountHeadId, Debit = req.Amount, Credit = 0 };
            var cashHead = _db.AccountHeads.SingleOrDefault(a => a.Name.ToLower() == "cash");
            var cashId = cashHead != null ? cashHead.AccountHeadId : 0;
            var jl2 = new JournalLine { DocumentId = doc.DocumentId, AccountHeadId = cashId, Debit = 0, Credit = req.Amount };
            _db.JournalLines.AddRange(jl1, jl2);
            _db.SaveChanges();

            return Ok(new { docNumber = doc.DocNumber, documentId = doc.DocumentId });
        }
    }

    public record CreateReceiptRequest(DateTime DocDate, int AccountHeadId, decimal Amount, string PaymentMode, string Description);
}

