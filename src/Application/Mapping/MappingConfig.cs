// src/Application/Mapping/MappingConfig.cs
using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;

namespace NewsPaper.src.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // News mappings
            CreateMap<NewsDto, News>();
            CreateMap<ListNewsDtoResponse, News>();
            CreateMap<News, YourPostDto>();
            CreateMap<News, ListNewsDtoResponse>();
            CreateMap<CreateNewsDto, News>();
            CreateMap<News, NewsDto>();

            // User mappings
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<UpdateUserInfor, User>();
            CreateMap<User, UserLoginResponseDto>();

            // Category mappings
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<ChildrenCategoryDto, ChildrenCategory>();
            CreateMap<ChildrenCategory, ChildrenCategoryDto>();

            // Comment mappings
            CreateMap<CommentDto, Comment>();
            CreateMap<Comment, ListCommentResponseDto>();

            // Saved mappings
            CreateMap<SavedDto, Saved>();
            CreateMap<Saved, SavedDto>();

            // Watchlist mappings
            CreateMap<WatchlistDto, Watchlist>();
            CreateMap<Watchlist, WatchlistDto>();
            CreateMap<AddWatchlistDto, Watchlist>();

            // Transaction mappings
            CreateMap<TransactionDto, Transaction>();
            CreateMap<Transaction, TransactionDto>();
            CreateMap<Transaction, TransactionResponseDto>()
                .ForMember(
                    dest => dest.TimeAgo,
                    opt => opt.Ignore()
                );

            CreateMap<SwapTransactionDto, Transaction>()
                .ForMember(
                    dest => dest.TransactionType,
                    opt => opt.MapFrom(src => "SWAP")
                )
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => "PENDING")
                );
        }
    }
}