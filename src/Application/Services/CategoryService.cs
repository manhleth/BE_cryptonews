using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class CategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<object> CreateCategory(CategoryDto categoryDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryDto.CategoryName))
                {
                    return "Tên danh mục không được để trống";
                }

                var findCategory = await _unitOfWork.Category.FindOnlyByCondition(x =>
                    x.CategoryName.ToLower().Trim() == categoryDto.CategoryName.ToLower().Trim());
                if (findCategory != null)
                    return "Tên danh mục đã tồn tại";

                var category = new Category
                {
                    CategoryName = categoryDto.CategoryName.Trim(),
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.Category.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return new
                {
                    Message = "Tạo danh mục thành công",
                    Data = _mapper.Map<CategoryDto>(category)
                };
            }
            catch (Exception ex)
            {
                return $"Lỗi khi tạo danh mục: {ex.Message}";
            }
        }

        public async Task<object> DelteCateGory(int id)
        {
            try
            {
                var category = await _unitOfWork.Category.FindOnlyByCondition(x => x.CategoryId == id);
                if (category == null)
                    return $"Không tìm thấy danh mục với ID: {id}";

                // Check if there are any news using this category
                var newsUsingThisCategory = await _unitOfWork.News.FindAsync(x => x.CategoryId == id);
                if (newsUsingThisCategory.Any())
                {
                    return $"Không thể xóa danh mục này vì có {newsUsingThisCategory.Count()} bài viết đang sử dụng";
                }

                // Check if there are children categories
                var childrenCategories = await _unitOfWork.ChildrenCategory.FindAsync(x => x.ParentCategoryId == id);
                if (childrenCategories.Any())
                {
                    // Option 1: Prevent deletion if has children
                    return $"Không thể xóa danh mục này vì có {childrenCategories.Count()} danh mục con. Vui lòng xóa các danh mục con trước.";

                    // Option 2: Auto delete children (uncomment if you want cascade delete)
                    /*
                    foreach (var child in childrenCategories)
                    {
                        // Check if any news using children categories
                        var newsUsingChild = await _unitOfWork.News.FindAsync(x => x.ChildrenCategoryId == child.ChildrenCategoryId);
                        if (newsUsingChild.Any())
                        {
                            return $"Không thể xóa danh mục này vì danh mục con '{child.ChildrenCategoryName}' có {newsUsingChild.Count()} bài viết đang sử dụng";
                        }
                    }
                    
                    // Delete all children categories first
                    foreach (var child in childrenCategories)
                    {
                        await _unitOfWork.ChildrenCategory.DeleteAsync(child);
                    }
                    */
                }

                await _unitOfWork.Category.DeleteAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return "Xóa danh mục thành công";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi xóa danh mục: {ex.Message}";
            }
        }

        public async Task<object> GetTopCategory()
        {
            try
            {
                return await _unitOfWork.Category.GetTopNews(4);
            }
            catch (Exception ex)
            {
                return $"Lỗi khi lấy danh mục top: {ex.Message}";
            }
        }

        public async Task<object> GetAllCategory()
        {
            try
            {
                return await _unitOfWork.Category.GetAllObject();
            }
            catch (Exception ex)
            {
                return $"Lỗi khi lấy tất cả danh mục: {ex.Message}";
            }
        }
    }
}