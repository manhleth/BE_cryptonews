using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class ChildrenCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ChildrenCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<object> CreateNewChildrenCategory(ChildrenCategoryDto childrenCategoryDto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(childrenCategoryDto.ChildrenCategoryName))
                {
                    return "Tên danh mục con không được để trống";
                }

                if (childrenCategoryDto.ParentCategoryId <= 0)
                {
                    return "ID danh mục cha không hợp lệ";
                }

                // Check if parent category exists
                var parentCategory = await _unitOfWork.Category.FindOnlyByCondition(x => x.CategoryId == childrenCategoryDto.ParentCategoryId);
                if (parentCategory == null)
                {
                    return $"Không tìm thấy danh mục cha với ID: {childrenCategoryDto.ParentCategoryId}";
                }

                // Check duplicate name within the same parent category
                var checkChildrenCategory = await _unitOfWork.ChildrenCategory.FindOnlyByCondition(x =>
                    x.ChildrenCategoryName.ToLower().Trim() == childrenCategoryDto.ChildrenCategoryName.ToLower().Trim() &&
                    x.ParentCategoryId == childrenCategoryDto.ParentCategoryId);

                if (checkChildrenCategory != null)
                    return $"Danh mục con '{childrenCategoryDto.ChildrenCategoryName}' đã tồn tại trong danh mục cha này";

                // Create new children category
                var childrenCategory = new ChildrenCategory
                {
                    ChildrenCategoryName = childrenCategoryDto.ChildrenCategoryName.Trim(),
                    ParentCategoryId = childrenCategoryDto.ParentCategoryId,
                    Description = childrenCategoryDto.Description?.Trim(),
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.ChildrenCategory.AddAsync(childrenCategory);
                await _unitOfWork.SaveChangesAsync();

                return new
                {
                    Message = "Tạo danh mục con thành công",
                    Data = _mapper.Map<ChildrenCategoryDto>(childrenCategory)
                };
            }
            catch (Exception ex)
            {
                return $"Lỗi khi tạo danh mục con: {ex.Message}";
            }
        }

        public async Task<object> GetChildrenCategoryByParentCategory(int parentCategory)
        {
            try
            {
                var childrenCategory = await _unitOfWork.ChildrenCategory.FindAsync(x => x.ParentCategoryId == parentCategory);
                return _mapper.Map<List<ChildrenCategoryDto>>(childrenCategory);
            }
            catch (Exception ex)
            {
                return $"Lỗi khi lấy danh mục con: {ex.Message}";
            }
        }

        public async Task<object> GetAllChildrenCategory()
        {
            try
            {
                var data = await _unitOfWork.ChildrenCategory.GetAllObject();
                return _mapper.Map<List<ChildrenCategoryDto>>(data);
            }
            catch (Exception ex)
            {
                return $"Lỗi khi lấy tất cả danh mục con: {ex.Message}";
            }
        }

        public async Task<object> DeleteChildrenCategory(int id)
        {
            try
            {
                var childrenCategory = await _unitOfWork.ChildrenCategory.FindOnlyByCondition(x => x.ChildrenCategoryId == id);
                if (childrenCategory == null)
                    return $"Không tìm thấy danh mục con với ID: {id}";

                // Check if there are any news using this children category
                var newsUsingThisCategory = await _unitOfWork.News.FindAsync(x => x.ChildrenCategoryId == id);
                if (newsUsingThisCategory.Any())
                {
                    return $"Không thể xóa danh mục con này vì có {newsUsingThisCategory.Count()} bài viết đang sử dụng";
                }

                await _unitOfWork.ChildrenCategory.DeleteAsync(childrenCategory);
                await _unitOfWork.SaveChangesAsync();
                return "Xóa danh mục con thành công";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi xóa danh mục con: {ex.Message}";
            }
        }
    }
}
