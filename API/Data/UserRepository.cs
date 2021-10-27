using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserRepository(DataContext dataContext, IMapper mapper) {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<MemberDto> GetMemberByUsernameAsync(string name)
        {
            return await _dataContext.Users.Where(x => x.UserName == name).ProjectTo<MemberDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _dataContext.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _dataContext.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string name)
        {
            return await _dataContext.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == name.ToLower());
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _dataContext.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _dataContext.Entry(user).State = EntityState.Modified;
        }
    }
}