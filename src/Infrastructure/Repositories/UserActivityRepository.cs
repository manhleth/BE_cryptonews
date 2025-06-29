﻿using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class UserActivityRepository : BaseRepository<UserActivity>
    {
        public UserActivityRepository(AppDbContext context) : base(context)
        {
        }
    }
}