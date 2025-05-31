
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

        // Thêm coin vào watchlist
        public async Task<object> AddToWatchlist(int userId, AddWatchlistDto addWatchlistDto)
        {
            try
            {
                // Kiểm tra coin đã có trong watchlist chưa
                var existingItem = await _unitOfWork.Watchlist.FindOnlyByCondition(
                    x => x.UserId == userId && x.CoinId == addWatchlistDto.CoinId && x.IsActive == true
                );

                if (existingItem != null)
                {
                    return "Coin already exists in watchlist";
                }

                // Lấy order cao nhất hiện tại
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
                    IsActive = true
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

        // Toggle coin trong watchlist (thêm nếu chưa có, xóa nếu đã có)
        public async Task<object> ToggleWatchlist(int userId, AddWatchlistDto addWatchlistDto)
        {
            try
            {
                var existingItem = await _unitOfWork.Watchlist.FindOnlyByCondition(
                    x => x.UserId == userId && x.CoinId == addWatchlistDto.CoinId && x.IsActive == true
                );

                if (existingItem != null)
                {
                    // Nếu đã có thì xóa (soft delete)
                    existingItem.IsActive = false;
                    existingItem.ModifiedDate = DateTime.Now;
                    await _unitOfWork.Watchlist.UpdateAsync(existingItem);
                    await _unitOfWork.SaveChangesAsync();

                    return new { action = "removed", message = "Coin removed from watchlist" };
                }
                else
                {
                    // Nếu chưa có thì thêm
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
                        IsActive = true
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
    }
}