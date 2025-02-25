using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedController : BaseController<SavedController>
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
        [HttpGet("GetYourListSaved")]
        public async Task<object> GetListSaved()
        {
            var saved = _savedService.GetListSavedByUser(UserIDLogined);
            return new ResponseData { Data = saved, StatusCode = 1};
        }
        [HttpPost("AddOrRemoveSaved")]
        public async Task<object> CreateSaved(SavedDto s)
        {
            var newSaved = _savedService.AddOrRemoveSaved(s);
            return new ResponseData { Data =newSaved, StatusCode = 1 };
        }
    }
}
