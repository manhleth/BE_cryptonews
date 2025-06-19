using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : BaseController<NewsController>
    {
        private readonly NewsService _newsService;
        private readonly ILogger<NewsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public NewsController(NewsService newsService, ILogger<NewsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _newsService = newsService;
        }
        [HttpGet("GetNewsByIdAsync")]
        [AllowAnonymous]
        public async Task<ResponseData> GetNewsByIdAsync(int id)
        {
            try
            {
                var news = await _newsService.GetNewsByIdAsync(id);
                if (news is string errorMessage)
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }
                return new ResponseData { Data = news, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by ID: {Id}", id);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        [HttpGet("GetNewest")]
        [AllowAnonymous]
        public async Task<ResponseData> GetNewestAsycn()
        {
            var news = await _newsService.GetNewestAsycn();
            return new ResponseData { Data = news, StatusCode = 1 };
        }

        [HttpGet("GetNewsByCategory")]
        public async Task<ResponseData> GetNewsByCategory(int category)
        {
            var news = await _newsService.GetNewsByCategory(category);
            return new ResponseData { Data = news, StatusCode = 1 };
        }

        [HttpPost("CreateNewPost")]
        public async Task<object> CreateNewsAsync([FromForm] CreateNewsDto newsDto)
        {
            try
            {
                if (string.IsNullOrEmpty(newsDto.Footer) || newsDto.Footer == "null" || newsDto.Footer == "")
                {
                    newsDto.Footer = null;
                }
                var news = await _newsService.CreateNewsAsync(newsDto);
                return new ResponseData { Data = news, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                return new ResponseData { Data = ex, StatusCode = -1 };
            }
        }

        [HttpGet("GetYourPost")]
        public async Task<ResponseData> GetYourPost()
        {
            var news = await _newsService.GetYourCreatePost(UserIDLogined);
            return new ResponseData { Data = news, StatusCode = 1 };
        }


        [HttpGet("GetNewsByCategoryTop")]
        [AllowAnonymous]
        public async Task<ResponseData> GetNewsByCategoryTop(int category)
        {
            var news = await _newsService.GetNewsByCategoryTop(category);
            return new ResponseData { Data = news, StatusCode = 1 };
        }

        [HttpPut("UpdateNews")]
        public async Task<ResponseData> UpdateNewsAsync([FromBody] UpdateNewsDto updateNewsDto)
        {
            try
            {     var newsDto = new NewsDto
                {
                    NewsId = updateNewsDto.NewsId,
                    Header = updateNewsDto.Header,
                    Title = updateNewsDto.Title,
                    Content = updateNewsDto.Content,
                    Footer = updateNewsDto.Footer,
                    TimeReading = updateNewsDto.TimeReading,
                    Links = updateNewsDto.Links,
                    CategoryId = updateNewsDto.CategoryId,
                    ChildrenCategoryId = updateNewsDto.ChildrenCategoryId,
                    ImagesLink = updateNewsDto.ImagesLink
                };

                var result = await _newsService.UpdateNewsAsync(newsDto);

                if (result is string errorMessage)
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating news");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }
        [HttpPut("UpdateNewsForm")]
        public async Task<ResponseData> UpdateNewsFormAsync([FromForm] UpdateNewsFormDto formDto)
        {
            try
            {
                var result = await _newsService.UpdateNewsFromForm(
                    formDto.NewsId,
                    formDto.Header,
                    formDto.Title,
                    formDto.Content,
                    formDto.Footer,
                    formDto.TimeReading,
                    formDto.Links,
                    formDto.CategoryId,
                    formDto.ChildrenCategoryId,
                    formDto.ImagesLink
                );

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating news from form");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }
        [HttpDelete("DeleteNewsByID")]
        public async Task<object> DeleteNewsAsync(int id)
        {
            try
            {
                await _newsService.DeleteNewsAsync(UserIDLogined, id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting news");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("AdminDelele")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> AdminDeleteNews(int id)
        {
            try
            {
                var news = await _newsService.DeleteNewsAsyncByAdmin(id);
                return new ResponseData { Data = news, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting news");
                return new ResponseData { Data = ex.ToString(), StatusCode = -1 };
            }
        }

        [HttpGet("GetAllNewAdmin")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> GetAllNews()
        {
            var news = await _newsService.GetAllNews();
            return new ResponseData { Data = news, StatusCode = 1 };
        }

        [HttpGet("GetNewsByChildrenCategoryId")]
        [AllowAnonymous]
        public async Task<ResponseData> GetNewsByChildrenCategory(int categoryID)
        {
            var news = await _newsService.GetNewsByChildrenCategoryID(categoryID);
            return new ResponseData { Data = news, StatusCode = 1 };
        }

        [HttpGet("GetNewsByKeyWord")]
        [AllowAnonymous]
        public async Task<ResponseData> GetNewsByKeyWord(string keyWord)
        {
            var news = await _newsService.FindNewsByKeyWord(keyWord);
            return new ResponseData { Data = news, StatusCode = 1 };
        }
        [HttpGet("GetFeaturedNews")]
        [AllowAnonymous]
        public async Task<ResponseData> GetFeaturedNews()
        {
            var news = await _newsService.GetFeaturedNews();
            return new ResponseData { Data = news, StatusCode = 1 };
        }

        [HttpPost("SetFeaturedNews")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> SetFeaturedNews(int newsId, bool isFeatured)
        {
            var result = await _newsService.SetFeaturedNews(newsId, isFeatured);
            if (result is string && result.ToString().Contains("Đã đạt giới hạn"))
            {
                return new ResponseData { Data = result, StatusCode = -1 };
            }
            return new ResponseData { Data = result, StatusCode = 1 };
        }

        [HttpPost("UpdateFeaturedOrder")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> UpdateFeaturedOrder([FromBody] List<int> newsIds)
        {
            var result = await _newsService.UpdateFeaturedOrder(newsIds);
            return new ResponseData { Data = result, StatusCode = 1 };
        }
    }
}
