using Swashbuckle.AspNetCore.Filters;
using WebApi.Apis.V1;

namespace WebApi.Infrastructure.Swagger
{
    public class AccountLoginExampleValue : IExamplesProvider<LoginCommand>
    {
        public LoginCommand GetExamples()
        {
            return new LoginCommand("user1@sample.com", "123@qwe");
        }
    }
}