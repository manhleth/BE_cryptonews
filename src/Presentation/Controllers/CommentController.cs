using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;
        private readonly ILogger<CommentController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CommentController(CommentService commentService, ILogger<CommentController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commentService = commentService;
        }
        [HttpGet]
        public IActionResult GetComments()
        {
            return Ok();
        }
        [HttpPost]
        public IActionResult CreateComment()
        {
            return Ok();
        }
        [HttpPut]
        public IActionResult UpdateComment()
        {
            return Ok();
        }
        [HttpDelete]
        public IActionResult DeleteComment()
        {
            return Ok();
        }

    }
}
