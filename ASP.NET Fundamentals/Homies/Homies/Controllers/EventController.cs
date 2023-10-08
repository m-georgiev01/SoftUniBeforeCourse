using Homies.Contracts;
using Homies.Data.Models;
using Homies.Models;
using Homies.Models.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Xml.Linq;

namespace Homies.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly IEventService eventService;

        public EventController(IEventService _eventService)
        {
            eventService = _eventService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await eventService.GetAll();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await eventService.GetUserJoinedEvents(userId);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new EventFormModel()
            {
                Types = await eventService.GetAllEventTypes()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await eventService.AddNewEvent(model, currUserId);

            return RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var currUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var model = await eventService.GetEventById(id, currUserId);

                return View(model);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EventFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var currUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await eventService.EditEvent(id, model, currUserId);

                return RedirectToAction("All");
            }
            catch(BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await eventService.AddToJoined(userId, id);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException)
            {
                return RedirectToAction("Joined");
            }

            return RedirectToAction("Joined");
        }

        [HttpGet]
        public async Task<IActionResult> Leave(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await eventService.RemoveEventHelper(id, userId);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }

            return RedirectToAction("All", "Event");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var model = await eventService.GetEventDetails(id);

                return View(model);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}
