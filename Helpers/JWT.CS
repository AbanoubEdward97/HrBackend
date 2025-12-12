namespace HrBackend.Helpers
{
    public class JWT
    {
        public required string key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string DurationInDays { get; set; }
    } 
}