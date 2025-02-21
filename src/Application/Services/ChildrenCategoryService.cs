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
