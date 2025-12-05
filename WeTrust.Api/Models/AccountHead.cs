namespace WeTrust.Api.Models
{
    public class AccountHead
    {
        public int AccountHeadId { get; set; }
        public string Name { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; }
        public decimal OpeningBalance { get; set; }
        public string OpeningBalanceType { get; set; }
    }

}

