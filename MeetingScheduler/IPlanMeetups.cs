using System;
using System.Collections.Generic;

namespace MeetingScheduler
{
    public interface IPlanMeetups
    {
        Meetup GetAppliedMeetups(string meet, out DateTime bookingDate);
        IEnumerable<Meetup> GetValidMeetups(IEnumerable<string> inputMeetup);
        OfficeHours GetOfficeHours(string inputMeetup);
        IEnumerable<string> TransformInputMeetups(List<string> inputMeetup);
        bool ValidateOfficeTimeLimits(Meetup objMeetup, OfficeHours officeHours);
        bool ValidateOverlappingMeetup(Meetup objMeetup, SortedList<DateTime,Meetup> sortList);
    }
    
}