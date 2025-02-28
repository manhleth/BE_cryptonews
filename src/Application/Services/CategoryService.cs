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
            var findCategory = await _unitOfWork.Category.FindOnlyByCondition(x => x.CategoryName == categoryDto.CategoryName);
            if (findCategory != null)
                return null;
            var category = _mapper.Map<Category>(categoryDto);
            await _unitOfWork.Category.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return categoryDto;
        }

        public async Task<object> DelteCateGory(int id)
        {
            var category = await _unitOfWork.Category.FindOnlyByCondition(x => x.CategoryId == id);
            if (category == null)
                return $"Can't not find category with id: {id}";
            await _unitOfWork.Category.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<object> GetTopCategory()
        {
            return await _unitOfWork.Category.GetTopNews(4);
        }

        public async Task<object> GetAllCategory()
        {
            return await _unitOfWork.Category.GetAllObject();
        }
    }
}
