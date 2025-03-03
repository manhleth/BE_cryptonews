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
        public async Task<ResponseData> GetNewsByIdAsync(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            return new ResponseData { Data = news, StatusCode = 1 };
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
        public async Task<ResponseData> UpdateNewsAsync([FromForm] NewsDto newsDto, int id)
        {
            try
            {
                var user = await _newsService.UpdateNewsAsync(newsDto, id);
                return new ResponseData { Data = user, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating news");
                return new ResponseData { Data = ex.ToString(), StatusCode = -1 };
            }
        }

        [HttpDelete("DeleteNewsByID")]
        public async Task<object> DeleteNewsAsync(int id)
        {
            try
            {
                await _newsService.DeleteNewsAsync(UserIDLogined,id);
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
    }
}
