using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;

namespace NewsPaper.src.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig() { 
            CreateMap<NewsDto, News>();
            CreateMap<CreateNewsDto, News>();   
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<SavedDto, Saved>();   
            CreateMap<Saved, SavedDto>();
            CreateMap<News, NewsDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<CommentDto, Comment>();
            CreateMap<Comment, ListCommentResponseDto>();
            CreateMap<ChildrenCategoryDto, ChildrenCategory>();
            CreateMap<ChildrenCategory, ChildrenCategoryDto>();
        }
    }
}
