using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildrenCategoryController : BaseController<ChildrenCategoryController>
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

        [HttpGet("GetChildrenCategoriesByParenID")]
        [AllowAnonymous]
        public async Task<ResponseData> GetChildrenCategoriesByParenID(int ParentID)
        {
            var childrenCategories = await _childrenCategoryService.GetChildrenCategoryByParentCategory(ParentID);
            return new ResponseData { Data = childrenCategories, StatusCode = 1 };
        }
        [Authorize(Roles ="1")]
        [HttpPost("CreateChildrenCategory")]
        public async Task<ResponseData> CreateChildrenCategory(ChildrenCategoryDto s)
        {
            var newChildrenCategory = await _childrenCategoryService.CreateNewChildrenCategory(s);
            return new ResponseData { Data = newChildrenCategory, StatusCode = 1 };
        }
        [HttpGet("GetListChildrenCategory")]
        [AllowAnonymous]
        public async Task<ResponseData> GetListChildrenCategory()
        {
            var listchildrentCategory = await _childrenCategoryService.GetAllChildrenCategory();
            return new ResponseData { Data = listchildrentCategory, StatusCode = 1 };
        }
        [Authorize(Roles ="1")]
        [HttpDelete("DeleteChildrenCategory")]
        public async Task<ResponseData> DeleteChildrenCategory(int id)
        {
            var children = await _childrenCategoryService.DeleteChildrenCategory(id);
            return new ResponseData { Data = children, StatusCode = 1 };
        }

    }
}
