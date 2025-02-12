namespace NewsPaper.src.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public int CommentId { get; set; }

        public int UserId { get; set; } 

        public int? ImagesId { get; set; }

        public int NewsId { get; set; }

        public string Content { get; set; } 

        public int? ChildCommentId { get; set; }
    }
}
