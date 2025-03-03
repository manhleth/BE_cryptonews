namespace NewsPaper.src.Application.DTOs
{
    public class ListCommentResponseDto
    {
        public int CommentId { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; }

        public string UserFullName { get; set; }

        public string UserAvartar { get; set; }

    }
}
