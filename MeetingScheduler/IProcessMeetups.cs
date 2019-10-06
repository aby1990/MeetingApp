using System.Collections.Generic;

namespace MeetingScheduler
{

    public interface IProcessMeetups
    {
        void GetBookedMeetups(IEnumerable<string> input);
        void FinalMeetings(IEnumerable<Meetup> meetList);

    }
}