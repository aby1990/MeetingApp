using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

namespace MeetingScheduler.Test
{
    public class MeetupTest
    {
        ILoggerFactory _logger;
        IPlanMeetups _planMeetup;
        public MeetupTest()
        {
            _logger = new LoggerFactory();
            _planMeetup = new PlanMeetups(_logger);
        }

        /// <summary>
        /// Validate transformation from input to office hours
        /// </summary>
        [Fact]
        public void ValidateGetOfficeHours()
        {
            string inp = "0900 1730";
            OfficeHours meet = _planMeetup.GetOfficeHours(inp);
            OfficeHours expected = new OfficeHours { StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 30, 0) };
            var obj1Str = JsonConvert.SerializeObject(meet);
            var obj2Str = JsonConvert.SerializeObject(expected);
            Assert.Equal(obj1Str, obj2Str);
        }

        /// <summary>
        /// transforming multiple lines corresponding to one meeting to single line per meeting 
        /// </summary>
        [Fact]
        public void ValidateTransformInput()
        {
            List<string> input = new List<string>() { "0900 1730", "2011-03-17 10:17:06 EMP001", "2011-03-21 09:00 1" };
            var res = _planMeetup.TransformInputMeetups(input);
            var expected = new string[] { "2011-03-17 10:17:06 EMP001 2011-03-21 09:00 1" };
            Assert.Equal(res, expected);
        }
        
        /// <summary>
        /// transforming single meeting into meetup object
        /// </summary>
        [Fact]
        public void ValidateAppliedMeetups()
        {
            string input ="2011-03-17 10:17:06 EMP001 2011-03-21 09:00 3";
            DateTime bookingDate;
            Meetup meet = _planMeetup.GetAppliedMeetups(input, out bookingDate);
            Meetup meetExp = new Meetup{MeetupStart=new DateTime(2011, 03, 21, 09, 00, 0),MeetupEnd=new DateTime(2011, 03, 21, 12, 0, 0),EntityName="EMP001" };
            var obj1Str = JsonConvert.SerializeObject(meet);
            var obj2Str = JsonConvert.SerializeObject(meetExp);
            Assert.Equal(obj1Str,obj2Str);
        }

        /// <summary>
        /// validate meetups for overlappping comparing already added meetings with new meeting
        /// </summary>
        [Fact]
        public void ValidateOverlappingMeetups()
        {
            SortedList<DateTime,Meetup> sort = new SortedList<DateTime, Meetup>();
            sort.Add(new DateTime(2012,11,25,10,37,12),new Meetup{MeetupStart=new DateTime(2011, 03, 21, 09, 00, 0),MeetupEnd=new DateTime(2011, 03, 21, 10, 0, 0),EntityName="EMP001" });
            sort.Add(new DateTime(2012,11,25,10,34,12),new Meetup{MeetupStart=new DateTime(2011, 03, 21, 10, 00, 0),MeetupEnd=new DateTime(2011, 03, 21, 12, 0, 0),EntityName="EMP001" });
            sort.Add(new DateTime(2012,11,25,10,33,12),new Meetup{MeetupStart=new DateTime(2011, 03, 21, 13, 00, 0),MeetupEnd=new DateTime(2011, 03, 21, 16, 0, 0),EntityName="EMP001" });
            Meetup newMeet = new Meetup{MeetupStart=new DateTime(2011, 03, 21, 16, 00, 0),MeetupEnd=new DateTime(2011, 03, 21, 17, 0, 0),EntityName="EMP001" };
            Assert.Equal(_planMeetup.ValidateOverlappingMeetup(newMeet,sort),false);
        }

        /// <summary>
        /// validate if meeting comes in office hours or not
        /// </summary>
        [Fact]
        public void ValidateIsMeetInOfficeHours()
        {
            Meetup newMeet = new Meetup{MeetupStart=new DateTime(2011, 03, 21, 16, 00, 0),MeetupEnd=new DateTime(2011, 03, 21, 17, 0, 0),EntityName="EMP001" };
            OfficeHours offHours = new OfficeHours { StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 30, 0) };
            Assert.Equal(_planMeetup.ValidateOfficeTimeLimits(newMeet,offHours),true);
        }
    }
}
