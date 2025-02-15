using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class CategoryService : IBaseService<CategoryDto>
    {
        public Task<CategoryDto> CreateNewsAsync(CategoryDto newsDto)
        {
            throw new NotImplementedException();
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
