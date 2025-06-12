using AutoMapper;
using Microsoft.Extensions.Options;
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
        private readonly OTPService _OTPService;
        private readonly OTPConfiguration _otpConfig;
        public UserService(IOptions<OTPConfiguration> otpConfig, OTPService oTPService,IUnitOfWork unitOfWork, IMapper mapper, ApiOptions apiOptions)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _OTPService = oTPService;
            _otpConfig = otpConfig.Value;
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
                user = _mapper.Map<UserLoginResponseDto>(user),
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

        public async Task<object> UpdateUserInfor(int userID, UpdateUserInfor userUpdate)
        {
            var findUser = await _unitOfWork.User.FindOnlyByCondition(x => x.UserId == userID);
            if (findUser != null)
            {
                findUser.Fullname = userUpdate.Fullname;
                findUser.Birthday = userUpdate.Birthday;
                findUser.Avatar = userUpdate.Avatar;
                findUser.Phonenumber = userUpdate.PhoneNumber;
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

        public async Task<object> GetUserInfor(int UserID)
        {
            return await _unitOfWork.User.FindOnlyByCondition(x => x.UserId == UserID);
        }
        public async Task<object> UpdateUserRole(int userId, int newRoleId)
        {
            try
            {
                var user = await _unitOfWork.User.FindOnlyByCondition(x => x.UserId == userId);
                if (user == null)
                {
                    return $"User with ID {userId} not found";
                }
                if (newRoleId != 1 && newRoleId != 0)
                {
                    return "Invalid role ID. Use 1 for Admin, 0 for User";
                }

                user.RoleId = newRoleId;
                user.ModifiedDate = DateTime.Now;

                await _unitOfWork.User.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new
                {
                    message = "User role updated successfully",
                    userId = user.UserId,
                    newRole = newRoleId == 1 ? "Admin" : "User",
                    user = _mapper.Map<UserLoginResponseDto>(user)
                };
            }
            catch (Exception ex)
            {
                return $"Error updating user role: {ex.Message}";
            }
        }

        public string GeneratePasswordResetToken(string otpToken)
        {
            return otpToken;
        }


        public async Task<string> RequestForgotPassword(string email)
        {
            var checkEmail = await _unitOfWork.User.FindOnlyByCondition(x => x.Email == email);
            if (checkEmail == null)
            {
                throw new UnauthorizedAccessException("Email không tồn tại");
            }
            var token = await _OTPService.GenerateAndSendOTPAsync(email);
            return token;
        }

        public async Task<bool> ResetPassword(string email, ResetPasswordModel model)
        {
            var checkEmail = await _unitOfWork.User.FindOnlyByCondition(x => x.Email == email);
            if (checkEmail == null)
            {
                throw new UnauthorizedAccessException("Email không tồn tại");
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                throw new UnauthorizedAccessException("Mật khẩu không khớp");
            }
            var passwordHash = (model.NewPassword);
            var nv = await _unitOfWork.User.FindOnlyByCondition(x => x.Email.Equals(email));
            if (nv == null)
            {
                throw new UnauthorizedAccessException("Email không tồn tại");
            }
            nv.Password = passwordHash;
            await _unitOfWork.User.UpdateAsync(nv);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public string ValidateAndGetEmailFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_otpConfig.Secret);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal.FindFirstValue(ClaimTypes.Email);
            }
            catch
            {
                return null;
            }
        }

        public bool VerifyOTP(string token, string otp)
        {
            var isValid = _OTPService.ValidateOTPAsync(token, otp);
            if (!isValid)
            {
                throw new UnauthorizedAccessException("Mã OTP không hợp lệ");
            }
            return isValid;
        }
        public async Task<object> ChangePassword(int userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (string.IsNullOrEmpty(changePasswordDto.CurrentPassword) ||
                    string.IsNullOrEmpty(changePasswordDto.NewPassword) ||
                    string.IsNullOrEmpty(changePasswordDto.ConfirmPassword))
                {
                    return new { success = false, message = "Tất cả các trường đều bắt buộc" };
                }

                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                {
                    return new { success = false, message = "Mật khẩu mới và xác nhận mật khẩu không khớp" };
                }

                if (changePasswordDto.NewPassword.Length < 8)
                {
                    return new { success = false, message = "Mật khẩu mới phải có ít nhất 8 ký tự" };
                }

                // Tìm user
                var user = await _unitOfWork.User.FindOnlyByCondition(x => x.UserId == userId);
                if (user == null)
                {
                    return new { success = false, message = "Không tìm thấy người dùng" };
                }

                // Kiểm tra mật khẩu hiện tại
                if (user.Password != changePasswordDto.CurrentPassword)
                {
                    return new { success = false, message = "Mật khẩu hiện tại không đúng" };
                }

                // Kiểm tra mật khẩu mới không trùng với mật khẩu cũ
                if (user.Password == changePasswordDto.NewPassword)
                {
                    return new { success = false, message = "Mật khẩu mới phải khác mật khẩu hiện tại" };
                }

                // Cập nhật mật khẩu mới
                user.Password = changePasswordDto.NewPassword; // Lưu ý: nên hash password trong thực tế
                user.ModifiedDate = DateTime.Now;

                await _unitOfWork.User.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new
                {
                    success = true,
                    message = "Đổi mật khẩu thành công",
                    data = new
                    {
                        userId = user.UserId,
                        username = user.Username,
                        updatedAt = user.ModifiedDate
                    }
                };
            }
            catch (Exception ex)
            {
                return new { success = false, message = $"Lỗi server: {ex.Message}" };
            }
        }

    }

}
