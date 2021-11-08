using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection IServices, IConfiguration config) {
            IServices.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            IServices.AddScoped<ITokenService, TokenService>();
            IServices.AddScoped<IPhotoService, PhotoService>();
            
            IServices.AddScoped<LogUserActivity>();

            IServices.AddScoped<IUserRepository, UserRepository>();
            IServices.AddScoped<ILikesRepository, LikesRepository>();
            IServices.AddScoped<IMessageRepository, MessageRepository>();

            IServices.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            
            IServices.AddDbContext<DataContext>(options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            
            return IServices;
        }
    }
}