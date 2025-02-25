namespace NewsPaper.src.Domain.Entities
{
    public class User : BaseEntity
    {
        public int UserId { get; private set; }
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }   
        public string Fullname { get; set; }

        public string Phonenumber { get; set; }

        public DateTime? Birthday { get; set; }

        public string Avatar { get; set; }  

        public int RoleId { get; set; }

    }
}
