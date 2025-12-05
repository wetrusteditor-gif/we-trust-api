namespace WeTrust.Api.Models
{
    public class OrganizationSetting
    {
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
    }
}

