namespace Models.Dto.Auth
{
    public class AuthUserResponseWithGoogleDTO
    {
        public Guid UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public DateOnly? JoinDate { get; set; }
    }
}
