using Homies.Contracts;
using Homies.Data;
using Homies.Data.Models;
using Homies.Models;
using Homies.Models.Event;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Xml.Linq;

namespace Homies.Services
{
    public class EventService : IEventService
    {
        private readonly HomiesDbContext context;

        public EventService(HomiesDbContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<EventViewShortModel>> GetAll()
        {
            return await context.Events
                .Select(e => new EventViewShortModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Organiser = e.Organiser.UserName,
                    Start = e.Start.ToString("yyyy-MM-dd H:mm"),
                    Type = e.Type.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EventViewShortModel>> GetUserJoinedEvents(string userId)
        {
            return await context.EventsParticipants
                .Where(ep => ep.HelperId == userId)
                .Select(ep => new EventViewShortModel()
                {
                    Id = ep.Event.Id,
                    Name = ep.Event.Name,
                    Organiser = ep.Event.Organiser.UserName,
                    Start = ep.Event.Start.ToString("yyyy-MM-dd H:mm"),
                    Type = ep.Event.Type.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TypeViewModel>> GetAllEventTypes()
        {
            return await context.Types
                .Select(t => new TypeViewModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .ToListAsync();
        }

        public async Task AddNewEvent(EventFormModel model, string userId)
        {
            var eventToAdd = new Event()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedOn = DateTime.Now,
                TypeId = model.TypeId,
                OrganiserId = userId,
                Start = model.Start,
                End = model.End
            };

            await context.Events.AddAsync(eventToAdd);
            await context.SaveChangesAsync();
        }

        public async Task<EventFormModel> GetEventById(int id, string userId)
        {
            var eventToEdit = await context.Events.FindAsync(id);

            if (eventToEdit is null)
            {
                throw new BadHttpRequestException("Something went wrong!");
            }

            if (eventToEdit.OrganiserId != userId)
            {
                throw new BadHttpRequestException("You are unauthorized!");
            }

            var model = new EventFormModel()
            {
                Description = eventToEdit.Description,
                End = eventToEdit.End,
                Name = eventToEdit.Name,
                Start = eventToEdit.Start,
                TypeId= eventToEdit.TypeId,
                Types = await GetAllEventTypes()
            };

            return model;
        }

        public async Task EditEvent(int eventId, EventFormModel model, string userId)
        {
            var eventToEdit = await context.Events.FindAsync(eventId);

            if (eventToEdit?.OrganiserId != userId)
            {
                throw new BadHttpRequestException("You are unauthorized!");
            }

            eventToEdit.Name = model.Name;
            eventToEdit.Description = model.Description;
            eventToEdit.Start = model.Start;
            eventToEdit.End = model.End;
            eventToEdit.TypeId = model.TypeId;

            await context.SaveChangesAsync();
        }

        public async Task AddToJoined(string userId, int eventId)
        {
            var searchedEvent = await context.Events.FindAsync(eventId);

            if (searchedEvent is null)
            {
                throw new BadHttpRequestException("Something went wrong!");
            }

            var entry = new EventParticipant()
            {
                EventId = eventId,
                HelperId = userId,
            };

            if (await context.EventsParticipants.ContainsAsync(entry))
            {
                throw new ArgumentException();
            }

            await context.EventsParticipants.AddAsync(entry);
            await context.SaveChangesAsync();
        }

        public async Task RemoveEventHelper(int eventId, string userId)
        {
            var eventToLeave = await context.Events.FindAsync(eventId);

            if (eventToLeave is null)
            {
                throw new BadHttpRequestException("Something went wrong!");
            }

            var entry = await context.EventsParticipants
                .FirstOrDefaultAsync(ep => ep.HelperId == userId &&
                                           ep.EventId == eventId);

            context.EventsParticipants.Remove(entry);
            await context.SaveChangesAsync();
        }

        public async Task<EventViewDetailsModel> GetEventDetails(int eventId)
        {
            var eventToDisplay = await context
               .Events
               .Where(e => e.Id == eventId)
               .Select(e => new EventViewDetailsModel()
               {
                   Id = e.Id,
                   Name = e.Name,
                   Start = e.Start.ToString("dd/MM/yyyy H:mm"),
                   End = e.End.ToString("dd/MM/yyyy H:mm"),
                   Organiser = e.Organiser.UserName,
                   Type = e.Type.Name,
                   Description = e.Description,
                   CreatedOn = e.CreatedOn.ToString("dd/MM/yyyy H:mm")
               })
               .FirstOrDefaultAsync();

            if (eventToDisplay == null)
            {
                throw new BadHttpRequestException("Something went wrong!");
            }

            return eventToDisplay;
        }
    }
}
