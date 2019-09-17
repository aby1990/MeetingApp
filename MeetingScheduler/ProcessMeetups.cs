using System;
using System.Collections.Generic;

namespace MeetingScheduler
{
    public class ProcessMeetups : IProcessMeetups
    {
        private readonly IPlanMeetups _planMeetup;
        public ProcessMeetups(IPlanMeetups planMeetup)
        {
            _planMeetup = planMeetup;
        }
        /// <summary>
        /// Verify and then print finalised meetups
        /// </summary>
        /// <param name="input"></param>
        public void GetBookedMeetups(List<string> input)
        {
            IEnumerable<Meetup> meetList = _planMeetup.GetValidMeetups(input);
            FinalMeetings(meetList);
        }

        /// <summary>
        /// Print finalised meetings
        /// </summary>
        /// <param name="meetList"></param>
        public void FinalMeetings(IEnumerable<Meetup> meetList)
        {
            DateTime? tmpDate = null;
            foreach (var meet in meetList)
            {
                if (tmpDate == null || tmpDate?.Date != meet.MeetupStart.Date)
                    Console.WriteLine(meet.MeetupStart.Date.ToString("yyyy-MM-dd"));
                Console.WriteLine(meet.MeetupStart.ToString("HH:mm") + " " + meet.MeetupEnd.ToString("HH:mm") + " " + meet.EntityName);
                tmpDate = meet.MeetupStart.Date;
            }
        }
    }
}