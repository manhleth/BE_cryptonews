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

        public async Task<object> AddOrRemoveSaved(SavedDto savedDto)
        {
            var checkNews = _newsService.GetNewsByIdAsync(savedDto.NewsID);
            if (checkNews == null)
            {
                return "can't find post";
            }
            var findSavedPost = await _unitOfWork.Saved.FindOnlyByCondition(x => x.UserId == savedDto.UserID && x.NewsId == savedDto.NewsID);
            if (findSavedPost == null)
            {
                //SavedDto savedDtos = new SavedDto();
                //savedDto.UserID = UserID;
                //savedDto.NewsID = newsID;
                var newSavedObject = _mapper.Map<Saved>(savedDto);
                await _unitOfWork.Saved.AddAsync(newSavedObject);
                await _unitOfWork.SaveChangesAsync();
                return newSavedObject;
            }
            else
            {
                findSavedPost.Status = 0;
                await _unitOfWork.SaveChangesAsync();
                return "Remove saved post successfully";
            }    
        }

        public async Task<object> GetListSavedByUser(int userId)
        {
            return await _unitOfWork.Saved.FindAsync(x => x.UserId == userId && x.Status == 1);
        }
    }
    
}
