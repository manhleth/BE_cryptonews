using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController<T> : ControllerBase where T : BaseController<T>
    {
        public int UserIDLogined
        {
            get
            {
                return int.Parse(this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);
            }
        }

        public string UserNameLogined
        {
            get
            {
                return (this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value);
            }
        }

    }
}
