namespace DocuSign.MyHR.Domain
{
    public class UserDetails : User
    {
        public UserDetails(string id, string name, string email, Address address) : base(id, name)
        {
            Address = address;
            Email = email;
        }
        public string Email { get; }
        public Address Address { get; }
    }
}