namespace DocuSign.MyHR.Domain
{
    public class User
    {
        public User()
        {
                
        }
        public User(string id, string name)
        {
            Id = id;
            Name = name; 
        }
        public string Id { get; set; }
        public string Name { get; set; }
       
    }
}