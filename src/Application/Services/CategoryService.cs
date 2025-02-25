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

        public Task<CategoryDto> DeleteNewsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDto> GetNewsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryDto>> SearchNewsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDto> UpdateNewsAsync(CategoryDto newsDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
