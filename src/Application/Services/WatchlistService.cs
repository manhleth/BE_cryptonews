using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class WatchlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WatchlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Lấy danh sách watchlist của user
        public async Task<object> GetUserWatchlist(int userId)
        {
            try
            {
                var watchlist = await _unitOfWork.Watchlist.FindAsync(
                    x => x.UserId == userId && x.IsActive == true
                );
                var orderedWatchlist = watchlist.OrderBy(x => x.Order).ToList();
                return _mapper.Map<List<WatchlistDto>>(orderedWatchlist);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // FIXED: Thêm coin vào watchlist với proper handling
        public async Task<object> AddToWatchlist(int userId, AddWatchlistDto addWatchlistDto)
        {
            try
            {
                // FIXED: Kiểm tra coin đã từng có trong watchlist chưa (bao gồm cả IsActive = false)
                var existingItem = await _unitOfWork.Watchlist.FindOnlyByCondition(
                    x => x.UserId == userId && x.CoinId == addWatchlistDto.CoinId
                );

                if (existingItem != null)
                {
                    if (existingItem.IsActive)
                    {
                        return "Coin already exists in watchlist";
                    }

                    // FIXED: Nếu coin đã có nhưng IsActive = false, thì reactive lại
                    existingItem.IsActive = true;
                    existingItem.ModifiedDate = DateTime.Now;

                    // Update thông tin coin (có thể đã thay đổi)
                    existingItem.CoinSymbol = addWatchlistDto.CoinSymbol;
                    existingItem.CoinName = addWatchlistDto.CoinName;
                    existingItem.CoinImage = addWatchlistDto.CoinImage;

                    // FIXED: Set lại order cho coin được reactive
                    var currentActiveWatchlist = await _unitOfWork.Watchlist.FindAsync(
                        x => x.UserId == userId && x.IsActive == true
                    );
                    existingItem.Order = currentActiveWatchlist.Count() + 1;

                    await _unitOfWork.Watchlist.UpdateAsync(existingItem);
                    await _unitOfWork.SaveChangesAsync();

                    return _mapper.Map<WatchlistDto>(existingItem);
                }

                // Tạo mới nếu chưa từng có
                var currentWatchlist = await _unitOfWork.Watchlist.FindAsync(
                    x => x.UserId == userId && x.IsActive == true
                );
                var maxOrder = currentWatchlist.Any() ? currentWatchlist.Max(x => x.Order) : 0;

                var watchlistItem = new Watchlist
                {
                    UserId = userId,
                    CoinId = addWatchlistDto.CoinId,
                    CoinSymbol = addWatchlistDto.CoinSymbol,
                    CoinName = addWatchlistDto.CoinName,
                    CoinImage = addWatchlistDto.CoinImage,
                    Order = maxOrder + 1,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                await _unitOfWork.Watchlist.AddAsync(watchlistItem);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<WatchlistDto>(watchlistItem);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Xóa coin khỏi watchlist
        public async Task<object> RemoveFromWatchlist(int userId, string coinId)
        {
            try
            {
                var watchlistItem = await _unitOfWork.Watchlist.FindOnlyByCondition(
                    x => x.UserId == userId && x.CoinId == coinId && x.IsActive == true
                );

                if (watchlistItem == null)
                {
                    return "Coin not found in watchlist";
                }

                // Soft delete
                watchlistItem.IsActive = false;
                watchlistItem.ModifiedDate = DateTime.Now;

                await _unitOfWork.Watchlist.UpdateAsync(watchlistItem);
                await _unitOfWork.SaveChangesAsync();

                return "Coin removed from watchlist successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // IMPROVED: Toggle coin trong watchlist
        public async Task<object> ToggleWatchlist(int userId, AddWatchlistDto addWatchlistDto)
        {
            try
            {
                // Check cả IsActive = true và false
                var existingItem = await _unitOfWork.Watchlist.FindOnlyByCondition(
                    x => x.UserId == userId && x.CoinId == addWatchlistDto.CoinId
                );

                if (existingItem != null && existingItem.IsActive)
                {
                    // Nếu đang active thì deactivate
                    existingItem.IsActive = false;
                    existingItem.ModifiedDate = DateTime.Now;
                    await _unitOfWork.Watchlist.UpdateAsync(existingItem);
                    await _unitOfWork.SaveChangesAsync();

                    return new { action = "removed", message = "Coin removed from watchlist" };
                }
                else if (existingItem != null && !existingItem.IsActive)
                {
                    // Nếu đã có nhưng inactive thì reactive
                    existingItem.IsActive = true;
                    existingItem.ModifiedDate = DateTime.Now;

                    // Update thông tin coin
                    existingItem.CoinSymbol = addWatchlistDto.CoinSymbol;
                    existingItem.CoinName = addWatchlistDto.CoinName;
                    existingItem.CoinImage = addWatchlistDto.CoinImage;

                    // Set lại order
                    var currentActiveWatchlist = await _unitOfWork.Watchlist.FindAsync(
                        x => x.UserId == userId && x.IsActive == true
                    );
                    existingItem.Order = currentActiveWatchlist.Count() + 1;

                    await _unitOfWork.Watchlist.UpdateAsync(existingItem);
                    await _unitOfWork.SaveChangesAsync();

                    return new
                    {
                        action = "added",
                        message = "Coin re-added to watchlist",
                        data = _mapper.Map<WatchlistDto>(existingItem)
                    };
                }
                else
                {
                    // Tạo mới nếu chưa từng có
                    var currentWatchlist = await _unitOfWork.Watchlist.FindAsync(
                        x => x.UserId == userId && x.IsActive == true
                    );
                    var maxOrder = currentWatchlist.Any() ? currentWatchlist.Max(x => x.Order) : 0;

                    var watchlistItem = new Watchlist
                    {
                        UserId = userId,
                        CoinId = addWatchlistDto.CoinId,
                        CoinSymbol = addWatchlistDto.CoinSymbol,
                        CoinName = addWatchlistDto.CoinName,
                        CoinImage = addWatchlistDto.CoinImage,
                        Order = maxOrder + 1,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    await _unitOfWork.Watchlist.AddAsync(watchlistItem);
                    await _unitOfWork.SaveChangesAsync();

                    return new
                    {
                        action = "added",
                        message = "Coin added to watchlist",
                        data = _mapper.Map<WatchlistDto>(watchlistItem)
                    };
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Kiểm tra coin có trong watchlist không
        public async Task<object> IsInWatchlist(int userId, string coinId)
        {
            try
            {
                var existingItem = await _unitOfWork.Watchlist.FindOnlyByCondition(
                    x => x.UserId == userId && x.CoinId == coinId && x.IsActive == true
                );

                return new { isInWatchlist = existingItem != null };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Lấy danh sách coinId trong watchlist của user
        public async Task<List<string>> GetUserWatchlistCoinIds(int userId)
        {
            try
            {
                var watchlist = await _unitOfWork.Watchlist.FindAsync(
                    x => x.UserId == userId && x.IsActive == true
                );
                return watchlist.Select(x => x.CoinId).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        // BONUS: Method để cleanup duplicate entries (chạy 1 lần để fix data cũ)
        public async Task<object> CleanupDuplicateEntries(int userId)
        {
            try
            {
                var allWatchlistItems = await _unitOfWork.Watchlist.FindAsync(
                    x => x.UserId == userId
                );

                var groupedItems = allWatchlistItems
                    .GroupBy(x => x.CoinId)
                    .Where(g => g.Count() > 1)
                    .ToList();

                int cleanedCount = 0;

                foreach (var group in groupedItems)
                {
                    var items = group.OrderByDescending(x => x.ModifiedDate).ToList();
                    var keepItem = items.First(); // Giữ item mới nhất

                    // Xóa các items còn lại
                    for (int i = 1; i < items.Count; i++)
                    {
                        await _unitOfWork.Watchlist.DeleteAsync(items[i]);
                        cleanedCount++;
                    }
                }

                if (cleanedCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                }

                return $"Cleaned up {cleanedCount} duplicate entries";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}