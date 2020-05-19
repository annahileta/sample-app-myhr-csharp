namespace DocuSign.MyHR.Domain
{
    public class UserDetails : User
    {
        public UserDetails()
        {
                
        }
        public UserDetails(string id, string name, string email, Address address) : base(id, name)
        {
            Address = address;
            Email = email;
        }
        public string Email { get; set; }
        public Address Address { get; set; }
    }
}