using NewsPaper.src.Domain.Entities;

namespace NewsPaper.src.Application.DTOs
{
    public class UserRegisterDto
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
        public string Fullname { get; set; }

        public string Phonenumber { get; set; }

        public DateTime? Birthday { get; set; }

        public string Avatar { get; set; }

    }
}
