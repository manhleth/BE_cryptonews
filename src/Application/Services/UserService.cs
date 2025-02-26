using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;
using NewsPaper.src.Domain.ValueObjects;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace NewsPaper.src.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApiOptions _apiOptions;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ApiOptions apiOptions)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _apiOptions = apiOptions;
        }
        public async Task<object> UserLogin(UserDto newsDto)
        {
            var user = await _unitOfWork.User.FindOnlyByCondition(x => x.Email == newsDto.Email && x.Password == newsDto.password);
            if (user == null)
            {
                return null;
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiOptions.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _apiOptions.ValidIssuer,
                audience: _apiOptions.ValidAudience,
                claims: claims,
                expires: DateTime.Now.Add(TimeSpan.FromDays(1)),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new
            {
                tokenGen = tokenString,
                user = user
            };
        }

        public async Task<object> UserRegister(UserRegisterDto userRegisterDto)
        {
            var checkUser = await _unitOfWork.User.FindOnlyByCondition(User => User.Email == userRegisterDto.Email && User.Username == userRegisterDto.Username);
            if (checkUser != null)
            {
                return null;
            }
            var user = _mapper.Map<User>(userRegisterDto);
            await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task<object> UpdateUserInfor(UserRegisterDto userUpdate)
        {
            var findUser = await _unitOfWork.User.FindOnlyByCondition(User => User.Email == userUpdate.Email);
            if(findUser != null)
            {
                findUser.Fullname = userUpdate.Fullname;
                findUser.Birthday = userUpdate.Birthday;
                findUser.Avatar = userUpdate.Avatar;
                findUser.Phonenumber = userUpdate.Phonenumber;
                findUser.Password = userUpdate.Password;
                findUser.ModifiedDate = DateTime.Now;
                await _unitOfWork.SaveChangesAsync();
                return findUser;
            }
            return null;
        }


        public async Task<object> DeleteUser(int userId)
        {
            var findUser = await _unitOfWork.User.FindOnlyByCondition(User => User.UserId == userId);
            if (findUser != null)
            {
                await _unitOfWork.User.DeleteAsync(findUser);
                await _unitOfWork.SaveChangesAsync();
                return findUser;
            }
            return null;
        }

        public async Task<object> GetListUser()
        {
            return await _unitOfWork.User.GetAllObject();
        }
    }
    
}
