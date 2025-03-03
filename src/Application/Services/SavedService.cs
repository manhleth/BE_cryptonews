using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class SavedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly NewsService _newsService;

        public SavedService(IUnitOfWork unitOfWork, IMapper mapper, NewsService newsServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _newsService = newsServices;

        }

        public async Task<object> AddOrRemoveSaved(int newsID, int UserID)
        {
            var checkNews = await _newsService.GetNewsByIdAsync(newsID);
            if (checkNews == null)
            {
                return "can't find post";
            }
            var findSavedPost = await _unitOfWork.Saved.FindOnlyByCondition(x => x.UserId == UserID && x.NewsId == newsID);
            if (findSavedPost == null)
            {
                SavedDto savedDto = new SavedDto
                {
                    NewsID = newsID,
                    UserID = UserID,
                    Status = 1
                };
                var newSavedObject = _mapper.Map<Saved>(savedDto);
                await _unitOfWork.Saved.AddAsync(newSavedObject);
                await _unitOfWork.SaveChangesAsync();
                return newSavedObject;
            }
            else
            {
                if(findSavedPost.Status == 1)
                    findSavedPost.Status = 0;
                    await _unitOfWork.SaveChangesAsync();
                    return "Remove saved post status successfully";
                findSavedPost.Status = 1;
                await _unitOfWork.SaveChangesAsync();
                return "Add saved post status successfully";
            }    
        }

        public async Task<object> GetListSavedByUser(int userId)
        {
            return await _unitOfWork.Saved.FindAsync(x => x.UserId == userId && x.Status == 1);
        }

        public async Task<object> GetListSavedPostByUser(int userId, int categoryID)
        {
            var listsaved = await _unitOfWork.Saved.FindAsync(x => x.UserId == userId && x.Status == 1);
            var listNewsID = listsaved.Select(x => x.NewsId).ToList();
            var news = await _unitOfWork.News.FindAsync(x => listNewsID.Contains(x.NewsId) && x.CategoryId == categoryID);
            List<ListNewsDtoResponse> listNewsSaved = new List<ListNewsDtoResponse>();
            foreach(var item in news)
            {
                ListNewsDtoResponse items = new ListNewsDtoResponse();
                items.NewsID = item.NewsId;
                items.Header = item.Header;
                items.Title = item.Title;
                items.Links = item.Links;
                items.TimeReading = item.TimeReading.ToString() + " minutes to read";
                items.ImagesLink = item.ImagesLink;
                var hourago = (DateTime.Now.Hour - item.CreatedDate.Value.Hour);
                var timeAgo = "";
                if (hourago > 0)
                {
                    timeAgo = hourago.ToString() + " Hour ago";
                }
                else
                {
                    timeAgo = (DateTime.Now.Day - item.CreatedDate.Value.Day).ToString() + " Day ago";
                }
                items.TimeAgo = timeAgo;
                var users = await _unitOfWork.User.FindOnlyByCondition(x => x.UserId == item.UserId);
                items.UserName = users.Username;
                items.UserAvartar = users.Avatar;
                listNewsSaved.Add(items);

            }
            return new
            {
                listSaved = listNewsSaved,
                total = news.Count()
            };
        }
    }
    
}
