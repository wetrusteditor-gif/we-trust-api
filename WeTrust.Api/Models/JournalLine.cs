namespace WeTrust.Api.Models
{
    public class JournalLine
    {
        public int LineId { get; set; }
        public int DocumentId { get; set; }
        public int AccountHeadId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? LineNarration { get; set; }
    }
}

