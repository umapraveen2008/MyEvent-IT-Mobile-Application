using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEI
{
    public class EventCreation
    {

        public static async Task<int> GetDomainEventCount(string firmID)
        {
            int count = (await App.serverData.GetDomainEventData(firmID)).Count;
            return count;
        }

        public static async Task<ServerEvent> GetDomainEvent(string eventID)
        {
            return await App.serverData.GetSingleEventData(eventID);
        }

        public static async Task<List<ServerEvent>> GetDomainEvents(string firmID)
        {
            List<string> events = await App.serverData.GetDomainEventData(firmID);
            List<ServerEvent> currentDomainEvents = new List<ServerEvent>();
            for (int i = 0; i < events.Count; i++)
            {
                await ((HomeLayout)App.Current.MainPage).SetProgressBar(.3);
                await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Syncing event " + (i + 1).ToString() + " / " + events.Count.ToString() + " from current domain...");
                ServerEvent sevent = await App.serverData.GetSingleEventData(events[i]);
                if (string.Equals(sevent.eventPublishStatus, "published", StringComparison.OrdinalIgnoreCase))
                {
                    if (sevent.eventType == "Subscription")
                    {
                        if (sevent.eventSubscriptionIDStatus == "Approved")
                        {
                            if (sevent.eventUsers.Count == 0 || sevent.eventUsers.Contains(App.userID))
                                currentDomainEvents.Add(sevent);
                        }
                    }
                    else
                    {
                        if (sevent.eventUsers.Count == 0 || sevent.eventUsers.Contains(App.userID))
                            currentDomainEvents.Add(sevent);
                    }
                }
                await ((HomeLayout)App.Current.MainPage).SetProgressBar(.8);
            }
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "found " + (currentDomainEvents.Count).ToString() + " available event(s) from current domain...");
            await Task.Delay(500);
            return currentDomainEvents;
        }

        public static async Task<DomainEvent> SyncEventCatalog(DomainEvent dEvent, List<ServerInventoryItem> inventoryList)
        {
            DomainEvent currentEvent = dEvent;
            currentEvent.catalogList = await App.serverData.GetEventCatalogData(inventoryList, currentEvent.s_event.eventID);
            currentEvent.isSetted = true;
            return currentEvent;
        }

        public static async Task<DomainEvent> SyncEventSessions(DomainEvent dEvent)
        {
            DomainEvent currentEvent = dEvent;
            currentEvent.sessionList = await App.serverData.GetEventSessionData(currentEvent.s_event.eventID);
            return currentEvent;
        }

        public static async Task<DomainEvent> SyncEventSpeakers(DomainEvent dEvent)
        {
            DomainEvent currentEvent = dEvent;
            currentEvent.sessionList = await App.serverData.GetEventSessionData(currentEvent.s_event.eventID);
            List<string> speakerIDList = new List<string>();
            for (int i = 0; i < currentEvent.sessionList.Count; i++)
            {
                speakerIDList.AddRange(currentEvent.sessionList[i].sessionSpeakers);
            }
            speakerIDList = speakerIDList.Distinct().ToList();
            currentEvent.speakers.Clear();
            for (int i = 0; i < speakerIDList.Count; i++)
                currentEvent.speakers.Add(await App.serverData.GetOneSpeaker(speakerIDList[i]));
            return currentEvent;
        }

        public static async Task<DomainEvent> SyncEventSponsors(DomainEvent dEvent)
        {
            DomainEvent currentEvent = dEvent;
            currentEvent.sponsors = await App.serverData.GetEventSponsorData(dEvent.s_event.eventID);
            return currentEvent;
        }

        public static async Task<DomainEvent> SyncEventUsers(DomainEvent dEvent)
        {
            DomainEvent currentEvent = dEvent;
            currentEvent.rsvpUserList = await App.serverData.GetRSVPUserList(currentEvent.s_event.eventID);
            currentEvent.users.Clear();
            for (int k = 0; k < currentEvent.rsvpUserList.Count; k++)
            {
                currentEvent.users.Add(currentEvent.rsvpUserList[k].userID);
            }
            return currentEvent;
        }

        public static async Task<DomainGroup> SyncDomainUsers(DomainGroup dGroup)
        {
            DomainGroup currentDomainGroup = dGroup;
            List<string> firmUserIds = await App.serverData.GetDomainUsers(dGroup.domain.firmID);
            currentDomainGroup.users = new List<ServerUser>();
            for (int k = 0; k < firmUserIds.Count; k++)
            {
                currentDomainGroup.users.Add(await App.serverData.GetUserWithID(firmUserIds[k]));
            }
            return currentDomainGroup;
        }

        public static async Task<DomainEvent> SyncEventExhibitors(DomainEvent dEvent)
        {
            DomainEvent currentEvent = dEvent;
            currentEvent.exhibitors = await App.serverData.GetEventExhibitorData(currentEvent.s_event.eventID);
            return currentEvent;
        }

        public static async Task<DomainEvent> SyncEventPosts(DomainEvent dEvent)
        {
            DomainEvent currentEvent = dEvent;
            currentEvent.eventPostList.Clear();
            List<ServerEventPost> posts = await App.serverData.GetPostData(currentEvent.s_event.eventID);
            currentEvent.eventPostList = posts.Where(x => x.postUsers.Count == 0 || x.postUsers.Contains(App.userID)).ToList();
            return currentEvent;
        }

        public static async Task<DomainGroup> SyncDomainPosts(DomainGroup dEvent)
        {
            DomainGroup currentDomain = dEvent;
            currentDomain.userPosts.Clear();
            List<ServerEventPost> posts = await App.serverData.GetDomainUserPosts(dEvent.domain.firmID);
            currentDomain.userPosts = posts.Where(x => x.postUsers.Count == 0 || x.postUsers.Contains(App.userID)).ToList();
            return currentDomain;
        }

    }
}
