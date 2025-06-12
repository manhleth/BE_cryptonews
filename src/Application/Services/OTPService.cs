using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewsPaper.src.Application.Features;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NewsPaper.src.Application.Services
{
    public class OTPService
    {
        private readonly OTPConfiguration _otpConfig;
        private readonly EmailService _emailService;
        public OTPService(IOptions<OTPConfiguration> otpConfig, EmailService emailService)
        {
            _otpConfig = otpConfig.Value;
            _emailService = emailService;
        }

        public async Task<string> GenerateAndSendOTPAsync(string email)
        {
            string otp = GenerateRandomOTPAsync();
            string token = GenerateOTPToken(email, otp);
            string subject = "Mã xác thực đặt lại mật khẩu";
            //string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ResetPasswordOTP.html");?
            //string htmlTemplate = await File.ReadAllTextAsync(templatePath);
            string htmlContent = $"<h1>Mã xác thực mật khẩu của bạn là: {otp}</h1>";
            await _emailService.SendEmailAsync(email, subject, htmlContent);
            return token;
        }

        public string GenerateOTPToken(string email, string otp)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_otpConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Email, email),
            new Claim("otp", otp)
        }),
                Expires = DateTime.UtcNow.AddMinutes(_otpConfig.ExpireMinutes), // Token có hiệu lực trong 15 phút
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        public string GenerateRandomOTPAsync()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[4];
                rng.GetBytes(randomNumber);
                int value = Math.Abs(BitConverter.ToInt32(randomNumber, 0));
                return (value % 1000000).ToString("D6"); // Generate a 6-digit OTP
            }
        }

        public bool ValidateOTPAsync(string token, string otp)
        {
            try
            {
                // 1. Giải mã token
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

                // 2. Lấy thông tin từ claims
                var claims = principal.Claims;
                var storedOTP = claims.FirstOrDefault(c => c.Type == "otp")?.Value;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                // 3. Kiểm tra OTP có khớp không
                return storedOTP == otp;
            }
            catch
            {
                // Token không hợp lệ hoặc đã hết hạn
                return false;
            }
        }
    }
}
