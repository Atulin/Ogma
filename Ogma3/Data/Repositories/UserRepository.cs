using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        
        public UserRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<ProfileBar> GetProfileBar(string name)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetProfileBar)} -> {name}")
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpper())
                .ProjectTo<ProfileBar>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        
        public async Task<ProfileBar> GetProfileBar(long id)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetProfileBar)} -> {id}")
                .Where(u => u.Id == id)
                .ProjectTo<ProfileBar>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfileDto> GetUserData(string name)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetUserData)} -> {name}")
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpper())
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfileDto> GetUserData(long id)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetUserData)} -> {id}")
                .Where(u => u.Id == id)
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}