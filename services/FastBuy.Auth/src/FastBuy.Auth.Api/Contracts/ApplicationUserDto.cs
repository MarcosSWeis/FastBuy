namespace FastBuy.Auth.Api.Contracts
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public short DocumentType { get; set; }
        public string DocumentNumber { get; set; } = default!;

    }

    public class ApplicationUserUpdateDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public short DocumentType { get; set; }
        public string DocumentNumber { get; set; } = default!;

    }
}
