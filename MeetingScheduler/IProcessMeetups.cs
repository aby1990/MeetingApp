using System.Collections.Generic;

namespace MeetingScheduler
{

    public interface IProcessMeetups
    {
        void GetBookedMeetups(List<string> input);
        void FinalMeetings(IEnumerable<Meetup> meetList);

    }
}