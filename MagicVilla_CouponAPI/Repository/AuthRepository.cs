using AutoMapper;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Migrations;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_CouponAPI.Repository
{
    public class AuthRepository:IAuthRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private string secretKey;
        public AuthRepository(ApplicationDBContext dbContext, IMapper mapper, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
            secretKey = _configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user=_dbContext.LocalUsers.FirstOrDefault(x=>x.UserName== loginRequestDTO.UserName && x.Password==loginRequestDTO.Password);
            if (user == null)
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key=Encoding.ASCII.GetBytes(secretKey); //to convert secret key in bytes
            var tokenDescriptor = new SecurityTokenDescriptor //describe what is inside the token
            {
                Subject=new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(ClaimTypes.Role,user.Role),
                }),
                Expires=DateTime.UtcNow.AddDays(7),
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new()
            {
                User = _mapper.Map<UserDTO>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token), //token is generated
            };
            return loginResponseDTO;
        }

        public bool IsUniqueUser(string username)
        {
            var user=_dbContext.LocalUsers.FirstOrDefault(x=>x.UserName== username);
            if(user==null)
                return true;
            return false;
        }

        public async Task<UserDTO> Register(RegistrationRequestDTO requestDTO)
        {
            LocalUser obj = new()
            {
                UserName= requestDTO.UserName,
                Name= requestDTO.Name,
                Password= requestDTO.Password,
                Role="Customer"
            };
            _dbContext.LocalUsers.Add(obj);
            _dbContext.SaveChanges();
            obj.Password = ""; //empty the password
            return _mapper.Map<UserDTO>(obj);
        }
    }
}
