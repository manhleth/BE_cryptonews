using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedController : ControllerBase
    {
        private readonly SavedService _savedService;
        private readonly ILogger<SavedController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SavedController(SavedService savedService, ILogger<SavedController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _savedService = savedService;
        }
        [HttpGet]
        public IActionResult GetSaved()
        {
            return Ok();
        }
        [HttpPost]
        public IActionResult CreateSaved()
        {
            return Ok();
        }
        [HttpPut]
        public IActionResult UpdateSaved()
        {
            return Ok();
        }
        [HttpDelete]
        public IActionResult DeleteSaved()
        {
            return Ok();
        }
    }
}
