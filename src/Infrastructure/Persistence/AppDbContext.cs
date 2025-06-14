// src/Infrastructure/Persistence/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using NewsPaper.src.Domain.Entities;

namespace NewsPaper.src.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<ChildrenCategory> ChildrenCategories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Saved> Saveds { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }

        // Thêm DbSet mới
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PageView> PageViews { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<NewsAnalytics> NewsAnalytics { get; set; }
        public DbSet<DailyStats> DailyStats { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Category>().Property(c => c.Description).IsRequired(false);

            // ChildrenCategory
            modelBuilder.Entity<ChildrenCategory>().HasKey(cc => cc.ChildrenCategoryId);
            modelBuilder.Entity<ChildrenCategory>().Property(cc => cc.Description).IsRequired(false);

            // comment
            modelBuilder.Entity<Comment>().HasKey(c => c.CommentId);
            //modelBuilder.Entity<Comment>().Property(c => c.ImagesId).IsRequired(false);
            //modelBuilder.Entity<Comment>().Property(c => c.ChildCommentId).IsRequired(false);

            // MediaFile
            modelBuilder.Entity<MediaFile>().HasKey(mf => mf.MediaFileId);
            modelBuilder.Entity<MediaFile>().Property(mf => mf.ImageUrl).IsRequired(false);
            modelBuilder.Entity<MediaFile>().Property(mf => mf.VideoUrl).IsRequired(false);

            // News
            modelBuilder.Entity<News>().HasKey(n => n.NewsId);
            modelBuilder.Entity<News>().Property(n => n.Header).IsRequired(false);
            modelBuilder.Entity<News>().Property(n => n.Title).IsRequired(false);
            modelBuilder.Entity<News>().Property(n => n.Footer).IsRequired(false);
            modelBuilder.Entity<News>().Property(n => n.Links).IsRequired(false);
            //modelBuilder.Entity<News>().Property(n => n.TimeReading).IsRequired(false);
            //modelBuilder.Entity<News>().Property(n => n.ImagesId).IsRequired(false);
            //modelBuilder.Entity<News>().Property(n => n.CategoryId).IsRequired(false);

            //Role
            modelBuilder.Entity<Role>().HasKey(r => r.RoleId);

            // User
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<User>().Property(u => u.Avatar).IsRequired(false);
            //modelBuilder.Entity<User>().Property(u => u.Birthday).IsRequired(false);
            modelBuilder.Entity<User>().Property(u => u.Fullname).IsRequired(false);
            modelBuilder.Entity<User>().HasIndex(u => new { u.Email, u.Phonenumber }).IsUnique();

            // saved
            modelBuilder.Entity<Saved>().HasKey(s => s.SavedId);

            // Watchlist Configuration
            modelBuilder.Entity<Watchlist>().HasKey(w => w.WatchlistId);
            modelBuilder.Entity<Watchlist>().Property(w => w.CoinId).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Watchlist>().Property(w => w.CoinSymbol).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Watchlist>().Property(w => w.CoinName).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Watchlist>().Property(w => w.CoinImage).IsRequired(false).HasMaxLength(500);
            modelBuilder.Entity<Watchlist>().HasIndex(w => new { w.UserId, w.CoinId }).IsUnique();

            // Transaction configuration
            modelBuilder.Entity<Transaction>().HasKey(t => t.TransactionId);
            modelBuilder.Entity<Transaction>().Property(t => t.TransactionHash).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Transaction>().Property(t => t.FromAddress).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Transaction>().Property(t => t.ToAddress).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Transaction>().Property(t => t.FromToken).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Transaction>().Property(t => t.ToToken).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Transaction>().Property(t => t.FromAmount).HasPrecision(28, 18);
            modelBuilder.Entity<Transaction>().Property(t => t.ToAmount).HasPrecision(28, 18);
            modelBuilder.Entity<Transaction>().Property(t => t.TransactionType).IsRequired().HasMaxLength(10).HasDefaultValue("SEND");
            modelBuilder.Entity<Transaction>().Property(t => t.Status).IsRequired().HasMaxLength(10).HasDefaultValue("PENDING");
            modelBuilder.Entity<Transaction>().Property(t => t.GasUsed).HasPrecision(28, 18);
            modelBuilder.Entity<Transaction>().Property(t => t.GasPrice).HasPrecision(28, 18);

            // Relationship
            modelBuilder.Entity<Transaction>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for Transaction
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.TransactionHash)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.UserId);
            // PageView Configuration
            modelBuilder.Entity<PageView>().HasKey(p => p.PageViewId);
            modelBuilder.Entity<PageView>().Property(p => p.PageUrl).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<PageView>().Property(p => p.PageTitle).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<PageView>().Property(p => p.SessionDuration).HasDefaultValue(0);
            modelBuilder.Entity<PageView>().HasIndex(p => new { p.UserId, p.NewsId, p.ViewDate });

            // UserActivity Configuration  
            modelBuilder.Entity<UserActivity>().HasKey(u => u.UserActivityId);
            modelBuilder.Entity<UserActivity>().Property(u => u.ActivityType).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<UserActivity>().HasIndex(u => new { u.UserId, u.ActivityDate });
            modelBuilder.Entity<UserActivity>().HasIndex(u => u.ActivityType);

            // NewsAnalytics Configuration
            modelBuilder.Entity<NewsAnalytics>().HasKey(n => n.NewsAnalyticsId);
            modelBuilder.Entity<NewsAnalytics>().Property(n => n.ViewCount).HasDefaultValue(0);
            modelBuilder.Entity<NewsAnalytics>().Property(n => n.UniqueViewCount).HasDefaultValue(0);
            modelBuilder.Entity<NewsAnalytics>().Property(n => n.SaveCount).HasDefaultValue(0);
            modelBuilder.Entity<NewsAnalytics>().Property(n => n.CommentCount).HasDefaultValue(0);
            modelBuilder.Entity<NewsAnalytics>().Property(n => n.AverageReadTime).HasPrecision(10, 2).HasDefaultValue(0);
            modelBuilder.Entity<NewsAnalytics>().HasIndex(n => n.NewsId).IsUnique();

            // DailyStats Configuration
            modelBuilder.Entity<DailyStats>().HasKey(d => d.DailyStatsId);
            modelBuilder.Entity<DailyStats>().Property(d => d.TotalPageViews).HasDefaultValue(0);
            modelBuilder.Entity<DailyStats>().Property(d => d.UniqueVisitors).HasDefaultValue(0);
            modelBuilder.Entity<DailyStats>().Property(d => d.NewUsers).HasDefaultValue(0);
            modelBuilder.Entity<DailyStats>().Property(d => d.NewPosts).HasDefaultValue(0);
            modelBuilder.Entity<DailyStats>().Property(d => d.NewComments).HasDefaultValue(0);
            modelBuilder.Entity<DailyStats>().HasIndex(d => d.Date).IsUnique();
            // Relationships
            modelBuilder.Entity<PageView>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PageView>()
                .HasOne<News>()
                .WithMany()
                .HasForeignKey(p => p.NewsId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserActivity>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserActivity>()
                .HasOne<News>()
                .WithMany()
                .HasForeignKey(u => u.RelatedNewsId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<NewsAnalytics>()
                .HasOne<News>()
                .WithMany()
                .HasForeignKey(n => n.NewsId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}