using System;

namespace DocuSign.MyHR.Domain
{
    public class UserDetails : User
    {
        public UserDetails()
        {
        }

        public UserDetails(string id, string name, string email, string firstName, string lastName, string hireDate, string profileImageUri, Address address) : base(id, name)
        {
            Address = address;
            FirstName = firstName;
            LastName = lastName;
            HireDate = hireDate;
            ProfileImageUri = profileImageUri;
            Email = email;
        }
        public string Email { get; set; }
        public Address Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HireDate { get; set; }
        public string ProfileImageUri { get; set; }
    }
}