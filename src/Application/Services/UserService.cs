using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<UserDto> UserLogin(UserDto newsDto)
        {
            var user = await _unitOfWork.User.FindOnlyByCondition(x => x.Email == newsDto.Email && x.Password == newsDto.password);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<UserDto>(user);
        }
    }
    
}
