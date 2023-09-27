using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using SoftUniBazar.Contracts;
using SoftUniBazar.Models.Ad;
using System.Security.Claims;
using System.Xml.Linq;

namespace SoftUniBazar.Controllers
{
    [Authorize]
    public class AdController : Controller
    {
        private readonly IAdService adService;

        public AdController(IAdService _adService)
        {
            adService = _adService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await adService.GetAllAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AdFormViewModel()
            {
                Categories = await adService.GetCategoriesAsync(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await adService.AddAdAsync(model, userId);

            return RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await adService.GetAdsInUserCart(userId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await adService.AddAdToCartAsync(id, userId);
            }
            catch (BadHttpRequestException)
            {
                return BadRequest();
            }
            catch(ArgumentException)
            {
                return RedirectToAction("Cart", "Ad");
            }

            return RedirectToAction("Cart", "Ad");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var model = await adService.GetAdByIdAsync(id, userId);

                return View(model);
            }
            catch (BadHttpRequestException)
            {
                return BadRequest();
            }
            catch (HttpRequestException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AdFormViewModel model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await adService.EditAdAsync(id, model, userId);
                return RedirectToAction("All", "Ad");
            }
            catch (BadHttpRequestException)
            {
                return BadRequest();
            }
            catch (HttpRequestException)
            {
                return Unauthorized();
            }
        }
    }
}
