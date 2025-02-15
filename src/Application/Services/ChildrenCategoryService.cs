using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Interfaces;
using NewsPaper.src.Domain.Entities;

namespace NewsPaper.src.Application.Services
{
    public class ChildrenCategoryService : IBaseService<ChildrenCategoryDto>
    {
        public Task<ChildrenCategoryDto> CreateNewsAsync(ChildrenCategoryDto newsDto)
        {
            throw new NotImplementedException();
        }
        public Task<ChildrenCategoryDto> DeleteNewsAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<ChildrenCategoryDto> GetNewsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<List<ChildrenCategoryDto>> SearchNewsAsync()
        {
            throw new NotImplementedException();
        }
        public Task<ChildrenCategoryDto> UpdateNewsAsync(ChildrenCategoryDto newsDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
