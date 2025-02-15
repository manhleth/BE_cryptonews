using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Interfaces;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
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
        public async Task<NewsDto> GetNewsByIdAsync(int id)
        {
            return await _newsService.GetNewsByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewsAsync([FromBody] CreateNewsDto newsDto)
        {
            try
            {
                var news = await _newsService.CreateNewsAsync(newsDto);
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating news");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNewsAsync([FromBody] NewsDto newsDto, int id)
        {
            try
            {
                await _newsService.UpdateNewsAsync(newsDto, id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating news");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsAsync(int id)
        {
            try
            {
                await _newsService.DeleteNewsAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting news");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
