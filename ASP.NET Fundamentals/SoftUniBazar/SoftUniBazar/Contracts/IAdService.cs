using SoftUniBazar.Models.Ad;
using SoftUniBazar.Models.Category;

namespace SoftUniBazar.Contracts
{
    public interface IAdService
    {
        Task<IEnumerable<AdViewModel>> GetAllAsync();

        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync();

        Task AddAdAsync(AdFormViewModel model, string userId);

        Task<IEnumerable<AdViewModel>> GetAdsInUserCart(string userId);

        Task AddAdToCartAsync(int addId, string userId);

        Task<AdFormViewModel> GetAdByIdAsync(int id, string userId);

        Task EditAdAsync(int adId, AdFormViewModel model, string userId);
    }
}
