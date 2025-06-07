namespace NewsPaper.src.Application.DTOs
{
    public class UpdateRoleDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? Reason { get; set; }
    }
}