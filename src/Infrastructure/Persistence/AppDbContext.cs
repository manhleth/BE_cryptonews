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
            modelBuilder.Entity<User>().HasIndex(u => new {u.Email, u.Phonenumber}).IsUnique();

            // saved
            modelBuilder.Entity<Saved>().HasKey(s => s.SavedId);
            //Watchlist Configuration
            modelBuilder.Entity<Watchlist>().HasKey(w => w.WatchlistId);
            modelBuilder.Entity<Watchlist>().Property(w => w.CoinId).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Watchlist>().Property(w => w.CoinSymbol).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Watchlist>().Property(w => w.CoinName).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Watchlist>().Property(w => w.CoinImage).IsRequired(false).HasMaxLength(500);
            modelBuilder.Entity<Watchlist>().HasIndex(w => new { w.UserId, w.CoinId }).IsUnique();
        }
    }
}
