using MoviesApi.Entities;

namespace MoviesApi.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegesterAsync(RegisterModel registerModel);
        Task<AuthModel> GetokenAsync(TokenRequestModel registerModel);
        Task<string> AddRoleAsync(AddRoleModel addRoleModel);

    }
}
