using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController<UserController>
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserController(UserService userService, ILogger<UserController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        [HttpPost("UserLogin")]
        [AllowAnonymous]
        public async Task<ResponseData> UserLogin(UserDto userdto)
        {
            var user = await _userService.UserLogin(userdto);
            return new ResponseData { Data = user, StatusCode = 1 };
        }
        [HttpPost("UserRegister")]
        [AllowAnonymous]
        public async Task<ResponseData> UserRegister(UserRegisterDto newUser)
        {
            var user = await _userService.UserRegister(newUser);
            return new ResponseData { Data = user, StatusCode = 1 };
        }
        [HttpPost("UpdateUserInfor")]
        public async Task<ResponseData> UpdateUser(UpdateUserInfor userUpdate)
        {
            var user = await _userService.UpdateUserInfor(UserIDLogined, userUpdate);
            return new ResponseData { Data = user, StatusCode = 1 };
        }
        [HttpDelete("DeleUserByAdmin")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> DeleteUser(int userID)
        {
            var user = await _userService.DeleteUser(userID);
            return new ResponseData { Data = user, StatusCode = 1 };
        }

        [HttpGet("GetAllUserByAdmin")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> GetAllUser()
        {
            var user = await _userService.GetListUser();
            return new ResponseData { Data = user, StatusCode = 1 };
        }

        [HttpGet("GetUserInfor")]
        public async Task<ResponseData> GetUserById()
        {
            var user = await _userService.GetUserInfor(UserIDLogined);
            return new ResponseData { Data = user, StatusCode = 1 };
        }
        [HttpPost("SetAdminRole")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> SetAdminRole(int UserID, int RoleChange)
        {
            try
            {
                int newRoleId = RoleChange;

                var result = await _userService.UpdateUserRole(UserID, newRoleId);
                if (result is string errorMessage && errorMessage.Contains("not found"))
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }

                if (result is string error && error.Contains("Error"))
                {
                    return new ResponseData { Data = error, StatusCode = -1 };
                }

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user role");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }


        [HttpPost("Request-forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestForgotPassword(string email)
        {
            try
            {
                var checkToken = await _userService.RequestForgotPassword(email);
                if (checkToken == null)
                    return BadRequest("Email không tồn tại");
                // Lưu token vào cookie
                Response.Cookies.Append("reset_token", checkToken, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15) // Token có hiệu lực trong 15 phút
                });
                return Ok(new { message = "Email đã được gửi" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPVerificationModel dto)
        {
            try
            {
                string token = Request.Cookies["reset_token"];
                var isValid = _userService.VerifyOTP(token, dto.OTP);
                if (isValid)
                {
                    string resetToken = _userService.GeneratePasswordResetToken(token);
                    Response.Cookies.Append("password_reset_token", resetToken, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        MaxAge = TimeSpan.FromMinutes(15)
                    });
                    return Ok(new { message = "Mã OTP hợp lệ" });
                }
                else
                {
                    return BadRequest(new { message = "Mã OTP không hợp lệ" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel dto)
        {
            try
            {
                string resetToken = Request.Cookies["password_reset_token"];
                var email = _userService.ValidateAndGetEmailFromToken(resetToken);
                if (email == null)
                    return BadRequest("Token không hợp lệ");
                var result = await _userService.ResetPassword(email, dto);
                if (result)
                {
                    Response.Cookies.Delete("reset_token", new CookieOptions
                    {
                        Domain = "localhost",
                        Path = "/",
                        SameSite = SameSiteMode.None,
                        HttpOnly = false,
                        Secure = true,
                    });
                    Response.Cookies.Delete("password_reset_token", new CookieOptions
                    {
                        Domain = "localhost",
                        Path = "/",
                        SameSite = SameSiteMode.None,
                        HttpOnly = false,
                        Secure = true,
                    });
                    return Ok(new { message = "Đặt lại mật khẩu thành công" });
                }
                else
                    return BadRequest("Đặt lại mật khẩu thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ChangePassword")]
        public async Task<ResponseData> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var result = await _userService.ChangePassword(UserIDLogined, changePasswordDto);

                // Check if result contains success property
                if (result is object resultObj)
                {
                    var resultType = resultObj.GetType();
                    var successProperty = resultType.GetProperty("success");

                    if (successProperty != null)
                    {
                        bool success = (bool)successProperty.GetValue(resultObj);
                        if (success)
                        {
                            return new ResponseData { Data = result, StatusCode = 1 };
                        }
                        else
                        {
                            return new ResponseData { Data = result, StatusCode = -1 };
                        }
                    }
                }

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing password");
                return new ResponseData
                {
                    Data = new { success = false, message = "Có lỗi xảy ra khi đổi mật khẩu" },
                    StatusCode = -1
                };
            }
        }

    }
}
