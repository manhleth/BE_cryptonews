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
                    UserAvartar = user.Avatar,
                    UserFullName = user.Fullname
                });
            }
            return listCommentResponse;
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

        public async Task<object> GetAllCommentAdmin()
        {
            var listComment = await _unitOfWork.Comment.GetAllObject();
            var listUserId = listComment.Select(x => x.UserId).Distinct().ToList();
            var listUserInfor = await _unitOfWork.User.FindAsync(x => listUserId.Contains(x.UserId));

            // Debug logs
            Console.WriteLine($"Found {listUserInfor.Count()} users for {listUserId.Count} unique user IDs");
            foreach (var user in listUserInfor)
            {
                Console.WriteLine($"User {user.UserId}: {user.Fullname} ({user.Username})");
            }

            // Lấy thông tin news
            var listNewsId = listComment.Select(x => x.NewsId).Distinct().ToList();
            var listNews = await _unitOfWork.News.FindAsync(x => listNewsId.Contains(x.NewsId));

            List<ListCommentResponseDto> listCommentResponse = new List<ListCommentResponseDto>();
            foreach (var item in listComment)
            {
                var user = listUserInfor.FirstOrDefault(x => x.UserId == item.UserId);

                // Xử lý tên user với nhiều fallback
                string userName = "Unknown User";
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Fullname))
                        userName = user.Fullname;
                    else if (!string.IsNullOrEmpty(user.Username))
                        userName = user.Username;
                    else
                        userName = $"User #{user.UserId}";
                }
                else
                {
                    userName = $"User #{item.UserId} (Not Found)";
                }

                Console.WriteLine($"Comment {item.CommentId} by UserId {item.UserId} -> {userName}");

                listCommentResponse.Add(new ListCommentResponseDto
                {
                    CommentId = item.CommentId,
                    UserId = item.UserId,
                    Content = item.Content,
                    UserAvartar = user?.Avatar ?? "",
                    UserFullName = userName,
                    NewsId = item.NewsId,
                    CreatedDate = item.CreatedDate ?? DateTime.Now
                });
            }

            return listCommentResponse;
        }
        public async Task<object> DeleteCommentByAdmin(int commentID)
        {
            var comment = await _unitOfWork.Comment.FindOnlyByCondition(x => x.CommentId == commentID);
            await _unitOfWork.Comment.DeleteAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return $"Delete comment {commentID} success";

        }
    }
}
