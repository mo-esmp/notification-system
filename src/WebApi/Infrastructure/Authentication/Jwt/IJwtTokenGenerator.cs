using WebApi.Domain.Users;

namespace WebApi.Infrastructure.Authentication.Jwt
{
    public interface IJwtTokenGenerator
    {
        string Generate(User user, string department);
    }
}