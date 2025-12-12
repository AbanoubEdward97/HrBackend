namespace HrBackend.Helpers
{
    public class JWT
    {
        public string? key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? DurationInDays { get; set; }
    } 
}