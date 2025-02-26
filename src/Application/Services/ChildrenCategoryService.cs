using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;
using System.Formats.Asn1;

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
            var checkChildrenCategory = await _unitOfWork.ChildrenCategory.FindOnlyByCondition(x => x.ChildrenCategoryName == childrenCategoryDto.ChildrenCategoryName);
            if(checkChildrenCategory != null)
                return $"Children category name {childrenCategoryDto.ChildrenCategoryName} is already exist";
            var childrenCategory = _mapper.Map<ChildrenCategory>(childrenCategoryDto);
            await _unitOfWork.ChildrenCategory.AddAsync(childrenCategory);
            await _unitOfWork.SaveChangesAsync();
            return childrenCategoryDto;
        }

        public async Task<object> GetChildrenCategoryByParentCategory(int parentCategory)
        {
            var childrenCategory = await _unitOfWork.ChildrenCategory.FindAsync(x => x.ParentCategoryId == parentCategory);
            return _mapper.Map<List<ChildrenCategoryDto>>(childrenCategory);
        }

        public async Task<object> DeleteChildrenCategory(int id)
        {
            var childrenCategory = await _unitOfWork.ChildrenCategory.FindOnlyByCondition(x => x.ChildrenCategoryId == id);
            if (childrenCategory == null)
                return $"Can't not find children category with id: {id}";
            await _unitOfWork.ChildrenCategory.DeleteAsync(childrenCategory);
            await _unitOfWork.SaveChangesAsync();
            return "Delete children category successfully";
        }

        public async Task<object> GetAllChildrenCategory()
        {
            return await _unitOfWork.ChildrenCategory.GetAllObject();
        }
    }
}
