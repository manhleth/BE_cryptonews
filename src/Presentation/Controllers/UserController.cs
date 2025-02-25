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
        [Authorize]
        [HttpPost]
        public async Task<ResponseData> UpdateUser(UserRegisterDto userUpdate)
        {
            var user = await _userService.UpdateUserInfor(userUpdate);
            return new ResponseData { Data = user, StatusCode = 1 };
        }
        [HttpDelete]
        [Authorize]
        public IActionResult DeleteUser()
        {
            return Ok();
        }
    }
}
