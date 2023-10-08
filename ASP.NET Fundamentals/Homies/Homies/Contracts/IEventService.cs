using Homies.Models;
using Homies.Models.Event;
using Microsoft.AspNetCore.Mvc;

namespace Homies.Contracts
{
    public interface IEventService
    {
        Task<IEnumerable<EventViewShortModel>> GetAll();

        Task<IEnumerable<EventViewShortModel>> GetUserJoinedEvents(string userId);

        Task<IEnumerable<TypeViewModel>> GetAllEventTypes();

        Task AddNewEvent(EventFormModel model, string userId);

        Task<EventFormModel> GetEventById (int id, string userId);

        Task EditEvent(int eventId, EventFormModel model, string userId);

        Task AddToJoined(string userId, int eventId);

        Task RemoveEventHelper(int eventId, string userId);

        Task<EventViewDetailsModel> GetEventDetails(int eventId);
    }
}
