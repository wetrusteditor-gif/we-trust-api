namespace WeTrust.Api.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public string DocType { get; set; }
        public string DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public string? PaymentMode { get; set; }
        public int? RefDocumentId { get; set; }
        public string? Description { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsCancelled { get; set; } = false;
        public List<JournalLine>? JournalLines { get; set; }
    }
}

