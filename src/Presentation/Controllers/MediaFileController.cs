using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaFileController : ControllerBase
    {
        private readonly MediaFileService _mediaFileService;
        private readonly ILogger<MediaFileController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MediaFileController(MediaFileService mediaFileService, ILogger<MediaFileController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaFileService = mediaFileService;
        }
        [HttpGet]
        public IActionResult GetMediaFiles()
        {
            return Ok();
        }
        [HttpPost]
        public IActionResult CreateMediaFile()
        {
            return Ok();
        }
        [HttpPut]
        public IActionResult UpdateMediaFile()
        {
            return Ok();
        }
        [HttpDelete]
        public IActionResult DeleteMediaFile()
        {
            return Ok();
        }
    }
}
