using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;

namespace NewsPaper.src.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig() { 
            CreateMap<News, NewsDto>();
            CreateMap<NewsDto, News>();
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
        }
    }
}
