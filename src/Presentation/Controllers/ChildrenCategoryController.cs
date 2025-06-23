using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

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
            try
            {
                var childrenCategories = await _childrenCategoryService.GetChildrenCategoryByParentCategory(ParentID);

                if (childrenCategories is string errorMessage)
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }

                return new ResponseData { Data = childrenCategories, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting children categories by parent ID: {ParentID}", ParentID);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        [Authorize(Roles = "1")]
        [HttpPost("CreateChildrenCategory")]
        public async Task<ResponseData> CreateChildrenCategory(ChildrenCategoryDto dto)
        {
            try
            {
                var result = await _childrenCategoryService.CreateNewChildrenCategory(dto);

                // Check if result is error message
                if (result is string errorMessage && (errorMessage.Contains("Lỗi") || errorMessage.Contains("không") || errorMessage.Contains("đã tồn tại")))
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating children category");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        [HttpGet("GetListChildrenCategory")]
        [AllowAnonymous]
        public async Task<ResponseData> GetListChildrenCategory()
        {
            try
            {
                var listchildrentCategory = await _childrenCategoryService.GetAllChildrenCategory();

                if (listchildrentCategory is string errorMessage)
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }

                return new ResponseData { Data = listchildrentCategory, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all children categories");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        [Authorize(Roles = "1")]
        [HttpPost("AdminDelete")]
        public async Task<ResponseData> AdminDelete(int id)
        {
            try
            {
                var result = await _childrenCategoryService.DeleteChildrenCategory(id);

                if (result is string message && (message.Contains("Lỗi") || message.Contains("Không thể") || message.Contains("Không tìm thấy")))
                {
                    return new ResponseData { Data = result, StatusCode = -1 };
                }

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting children category with ID: {Id}", id);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        // DEPRECATED - Keep for backward compatibility
        [Authorize(Roles = "1")]
        [HttpDelete("DeleteChildrenCategory")]
        public async Task<ResponseData> DeleteChildrenCategory(int id)
        {
            _logger.LogWarning("DeleteChildrenCategory endpoint is deprecated. Use AdminDelete instead.");
            return await AdminDelete(id);
        }
    }
}