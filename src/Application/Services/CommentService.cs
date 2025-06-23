using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class CommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<object> CreateNewComment(CommentDto newComment, int UserID)
        {
            var comment = _mapper.Map<Comment>(newComment);
            comment.UserId = UserID;
            await _unitOfWork.Comment.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return comment;
        }

        // FIXED: Thêm CreatedDate vào response
        public async Task<object> GetCommentInPost(int newsID)
        {
            var listComment = await _unitOfWork.Comment.FindAsync(x => x.NewsId == newsID);
            var listUSer = listComment.Select(x => x.UserId).ToList();
            var listUserInfor = await _unitOfWork.User.FindAsync(x => listUSer.Contains(x.UserId));
            List<ListCommentResponseDto> listCommentResponse = new List<ListCommentResponseDto>();

            foreach (var item in listComment)
            {
                var user = listUserInfor.FirstOrDefault(x => x.UserId == item.UserId);
                listCommentResponse.Add(new ListCommentResponseDto
                {
                    CommentId = item.CommentId,
                    UserId = item.UserId,
                    Content = item.Content,
                    UserAvartar = user?.Avatar ?? "/default-avatar.png",
                    UserFullName = user?.Fullname ?? user?.Username ?? "Ẩn danh",
                    CreatedDate = item.CreatedDate,
                    NewsId = item.NewsId // Thêm NewsId
                });
            }

            // FIXED: Sắp xếp theo thứ tự mới nhất
            return listCommentResponse.OrderByDescending(x => x.CreatedDate).ToList();
        }

        public async Task<object> DeleteComment(int commentID, int userId)
        {
            var comment = await _unitOfWork.Comment.FindOnlyByCondition(x => x.CommentId == commentID && x.UserId == userId);
            if (comment == null)
                return $"Can't not find comment with id: {commentID}";
            await _unitOfWork.Comment.DeleteAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CommentDto>(comment);
        }

        // FIXED: GetAllCommentAdmin - Trả về đầy đủ thông tin
        public async Task<object> GetAllCommentAdmin()
        {
            try
            {
                // Lấy tất cả comments
                var listComment = await _unitOfWork.Comment.GetAllObject();

                // Lấy tất cả users
                var userIds = listComment.Select(x => x.UserId).Distinct().ToList();
                var listUserInfor = await _unitOfWork.User.FindAsync(x => userIds.Contains(x.UserId));

                // Lấy tất cả news để map title
                var newsIds = listComment.Select(x => x.NewsId).Distinct().ToList();
                var listNews = await _unitOfWork.News.FindAsync(x => newsIds.Contains(x.NewsId));

                List<AdminCommentResponseDto> listCommentResponse = new List<AdminCommentResponseDto>();

                foreach (var item in listComment)
                {
                    var user = listUserInfor.FirstOrDefault(x => x.UserId == item.UserId);
                    var news = listNews.FirstOrDefault(x => x.NewsId == item.NewsId);

                    listCommentResponse.Add(new AdminCommentResponseDto
                    {
                        CommentId = item.CommentId,
                        UserId = item.UserId,
                        Content = item.Content,
                        UserAvartar = user?.Avatar ?? "/default-avatar.png",
                        UserFullName = user?.Fullname ?? user?.Username ?? "Ẩn danh",
                        CreatedDate = item.CreatedDate,
                        NewsId = item.NewsId,
                        NewsTitle = news?.Title ?? news?.Header ?? "Không xác định"
                    });
                }

                // Sắp xếp theo thứ tự mới nhất
                return listCommentResponse.OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                // Log error và trả về empty list
                Console.WriteLine($"Error in GetAllCommentAdmin: {ex.Message}");
                return new List<AdminCommentResponseDto>();
            }
        }

        public async Task<object> DeleteCommentByAdmin(int commentID)
        {
            var comment = await _unitOfWork.Comment.FindOnlyByCondition(x => x.CommentId == commentID);
            if (comment == null)
                return $"Comment with id {commentID} not found";

            await _unitOfWork.Comment.DeleteAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return $"Delete comment {commentID} success";
        }
    }
}