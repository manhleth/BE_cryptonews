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

            // Category mappings - UPDATED để đảm bảo mapping đúng
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName));
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

            // ChildrenCategory mappings - FIXED để đảm bảo mapping property name đúng
            CreateMap<ChildrenCategoryDto, ChildrenCategory>()
                .ForMember(dest => dest.ChildrenCategoryId, opt => opt.MapFrom(src => src.ChildrenCategoryId))
                .ForMember(dest => dest.ChildrenCategoryName, opt => opt.MapFrom(src => src.ChildrenCategoryName))
                .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategoryId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

            CreateMap<ChildrenCategory, ChildrenCategoryDto>()
                .ForMember(dest => dest.ChildrenCategoryId, opt => opt.MapFrom(src => src.ChildrenCategoryId))
                .ForMember(dest => dest.ChildrenCategoryName, opt => opt.MapFrom(src => src.ChildrenCategoryName))
                .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategoryId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

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
                .ForMember(dest => dest.TimeAgo, opt => opt.Ignore());

            CreateMap<SwapTransactionDto, Transaction>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => "SWAP"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "PENDING"));

            // Analytics mappings
            CreateMap<TrackPageViewDto, PageView>();
            CreateMap<PageView, PageViewResponseDto>();
            CreateMap<TrackActivityDto, UserActivity>();
            CreateMap<UserActivity, UserActivityResponseDto>()
                .ForMember(dest => dest.TimeAgo, opt => opt.Ignore());
            CreateMap<NewsAnalytics, NewsAnalyticsDto>()
                .ForMember(dest => dest.NewsTitle, opt => opt.Ignore());
            CreateMap<DailyStats, DailyStatsDto>();
        }
    }
}