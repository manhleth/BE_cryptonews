using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class UserService : IBaseService<UserDto>
    {
        public Task<UserDto> CreateNewsAsync(UserDto newsDto)
        {
            throw new NotImplementedException();
        }
        public Task<UserDto> DeleteNewsAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<UserDto> GetNewsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<List<UserDto>> SearchNewsAsync()
        {
            throw new NotImplementedException();
        }
        public Task<UserDto> UpdateNewsAsync(UserDto newsDto, int id)
        {
            throw new NotImplementedException();
        }
    }
    
}
