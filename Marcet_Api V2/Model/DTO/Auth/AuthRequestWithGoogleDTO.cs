namespace TestRest.Models.Dto
{
    public class AuthRequestWithGoogleDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? CompanyName { get; set; }
        public DateOnly JoinDate { get; set; }
    }
}
