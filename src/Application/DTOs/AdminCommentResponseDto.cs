namespace NewsPaper.src.Application.DTOs
{
    public class AdminCommentResponseDto
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public string UserFullName { get; set; }
        public string UserAvartar { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int NewsId { get; set; }
        public string NewsTitle { get; set; }
    }
}