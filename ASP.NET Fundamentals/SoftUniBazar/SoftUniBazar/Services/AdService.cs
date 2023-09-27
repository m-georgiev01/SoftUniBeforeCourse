using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Contracts;
using SoftUniBazar.Data;
using SoftUniBazar.Data.Models;
using SoftUniBazar.Models.Ad;
using SoftUniBazar.Models.Category;
using System.Net;

namespace SoftUniBazar.Services
{
    public class AdService : IAdService
    {
        private readonly BazarDbContext context;

        public AdService(BazarDbContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<AdViewModel>> GetAllAsync()
        {
            return await context.Ads
                .AsNoTracking()
                .Select(a => new AdViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Category = a.Category.Name,
                    ImageUrl = a.ImageUrl,
                    CreatedOn = a.CreatedOn.ToString("yyyy-MM-dd H:mm"),
                    Price = a.Price,
                    Owner = a.Owner.UserName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync()
        {
            return await context.Categories
                .AsNoTracking()
                .Select(a => new CategoryViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                })
                .ToListAsync();
        }

        public async Task AddAdAsync(AdFormViewModel model, string userId)
        {
            var ad = new Ad()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                CreatedOn = DateTime.Now,
                OwnerId = userId,
                CategoryId = model.CategoryId,
            };

            await context.Ads.AddAsync(ad);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AdViewModel>> GetAdsInUserCart(string userId)
        {
            return await context.AdsBuyers
                .Where(ab => ab.BuyerId == userId)
                 .Select(ab => new AdViewModel()
                 {
                     Id = ab.Ad.Id,
                     Name = ab.Ad.Name,
                     Description = ab.Ad.Description,
                     CreatedOn = ab.Ad.CreatedOn.ToString("yyyy-MM-dd H:mm"),
                     Category = ab.Ad.Category.Name,
                     Price = ab.Ad.Price,
                     ImageUrl = ab.Ad.ImageUrl,
                     Owner = ab.Ad.Owner.UserName
                 })
                .ToListAsync();
        }

        public async Task AddAdToCartAsync(int addId, string userId)
        {
            var adToAdd = await context.Ads.FindAsync(addId);

            if (adToAdd is null)
            {
                throw new BadHttpRequestException("Something went wrong!");
            }

            var entry = new AdBuyer()
            {
                AdId = addId,
                BuyerId = userId,
            };

            if (await context.AdsBuyers.ContainsAsync(entry))
            {
                throw new ArgumentException();
            }

            await context.AdsBuyers.AddAsync(entry);
            await context.SaveChangesAsync();
        }

        public async Task<AdFormViewModel> GetAdByIdAsync(int id, string userId)
        {
            var ad = await context.Ads.FindAsync(id);

            if (ad is null)
            {
                throw new BadHttpRequestException("Something went wrong!");
            }

            if (userId != ad.OwnerId)
            {
                throw new HttpRequestException(null, null, HttpStatusCode.Unauthorized);
            }

            var model = new AdFormViewModel()
            {
                Name = ad.Name,
                Description = ad.Description,
                Price = ad.Price,
                CategoryId = ad.CategoryId,
                Categories = await GetCategoriesAsync(),
                ImageUrl = ad.ImageUrl,
            };

            return model;
        }

        public async Task EditAdAsync(int adId, AdFormViewModel model, string userId)
        {
            var adToEdit = await context.Ads.FindAsync(adId);

            if (adToEdit is null)
            {
                throw new BadHttpRequestException("There is no Ad with the given ID!");
            }

            if (userId != adToEdit.OwnerId)
            {
                throw new HttpRequestException(null, null, HttpStatusCode.Unauthorized);
            }

            adToEdit.Name = model.Name;
            adToEdit.Description = model.Description;
            adToEdit.ImageUrl = model.ImageUrl;
            adToEdit.Price = model.Price;
            adToEdit.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();
        }
    }
}
