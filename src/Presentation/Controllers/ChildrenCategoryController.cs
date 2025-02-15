using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildrenCategoryController : ControllerBase
    {
        private readonly ChildrenCategoryService _childrenCategoryService;
        private readonly ILogger<ChildrenCategoryController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ChildrenCategoryController(ChildrenCategoryService childrenCategoryService, ILogger<ChildrenCategoryController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _childrenCategoryService = childrenCategoryService;
        }

        [HttpGet("GetChildrenCategories")]
        public IActionResult GetChildrenCategories()
        {
            return Ok();
        }
        [HttpPost("CreateChildrenCategory")]
        public IActionResult CreateChildrenCategory()
        {
            return Ok();
        }
        [HttpPut("UpdateChildrenCategory")]
        public IActionResult UpdateChildrenCategory()
        {
            return Ok();
        }
        [HttpDelete("DeleteChildrenCategory")]
        public IActionResult DeleteChildrenCategory()
        {
            return Ok();
        }

    }
}
