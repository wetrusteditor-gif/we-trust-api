namespace WeTrust.Api.Models
{
    public class AccountHead
    {
        public int AccountHeadId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal OpeningBalance { get; set; }
        public char OpeningBalanceType { get; set; } = 'D';
    }
}

