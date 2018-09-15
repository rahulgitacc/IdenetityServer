using System.Threading.Tasks;

namespace AspNetCore.Auth.Web.Services
{
    public interface IUserService
    {
        Task<bool> ValidateCredentials(string username, string password, out User user);
    }

    public class User
    {
        public User(string userName)
        {
            UserName = userName;
        }
        public string UserName { get; set; }
    }
}
