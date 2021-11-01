using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await UserExists(dto.Username)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(dto);

            using var hmac = new HMACSHA512();
            user.UserName = dto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == dto.Username.ToLower());
            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            for (int i = 0; i < computedhash.Length; i++)
            {
                if (computedhash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}