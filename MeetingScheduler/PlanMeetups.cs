using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MeetingScheduler
{
    public class PlanMeetups : IPlanMeetups
    {
        private readonly ILogger<PlanMeetups> _logger;
        public PlanMeetups(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PlanMeetups>();
        }

        /// <summary>
        /// Get final meetings after various checks
        /// </summary>
        /// <param name="inputMeetup"></param>
        /// <returns> IEnumerable </returns>
        public IEnumerable<Meetup> GetValidMeetups(List<string> inputMeetup)
        {
            SortedList<DateTime, Meetup> sortList = new SortedList<DateTime, Meetup>();
            Meetup objMeetup = null;
            DateTime bookingDate;
            OfficeHours officeHours = GetOfficeHours(inputMeetup[0]);

            foreach (var meet in TransformInputMeetups(inputMeetup))
            {
                objMeetup = GetAppliedMeetups(meet, out bookingDate);
                if (objMeetup == new Meetup())
                    return Enumerable.Empty<Meetup>();

                if (ValidateOfficeTimeLimits(objMeetup, officeHours))
                {
                    if (ValidateOverlappingMeetup(objMeetup, sortList))
                    {
                        IEnumerable<Meetup> tmpObj = sortList.Values.Where(w => w.MeetupStart >= objMeetup.MeetupStart && w.MeetupEnd <= objMeetup.MeetupEnd);
                        int counter = tmpObj.Count();
                        List<DateTime> lstKeys = new List<DateTime>();
                        while (counter > 0)
                        {
                            int index = sortList.IndexOfValue(tmpObj.ElementAt(counter - 1));
                            if (bookingDate < sortList.Keys.ElementAt(index))
                                lstKeys.Add(sortList.Keys.ElementAt(index));
                            counter--;
                        }
                        if (lstKeys.Count() == tmpObj.Count())
                        {
                            for (int i = 0; i < lstKeys.Count(); i++)
                                sortList.Remove(lstKeys[i]);
                        }
                        else
                            continue;
                    }
                    sortList.Add(bookingDate, objMeetup);
                }
            }
            return sortList.Values.OrderBy(o => o.MeetupStart);
        }

        /// <summary>
        /// transforming input to keep single meeting details into single line 
        /// </summary>
        /// <param name="_meetupInput"></param>
        /// <returns>IEnumerable</returns>

        public IEnumerable<string> TransformInputMeetups(List<string> _meetupInput)
        {
            List<string> finalText = new List<string>();
            try
            {
                for (int i = 1; i < _meetupInput.Count; i = i + 2)
                {
                    finalText.Add($"{_meetupInput[i]} {_meetupInput[i + 1]}");
                }
                return finalText;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Mapping of data to Meetup object with bookingDate as out param
        /// </summary>
        /// <param name="meet"></param>
        /// <param name="bookingDate"></param>
        /// <returns>Meetup object</returns>
        public Meetup GetAppliedMeetups(string meet, out DateTime bookingDate)
        {
            Meetup objMeetup = new Meetup();
            bookingDate = new DateTime();
            try
            {
                List<string> tempString = meet.Split(" ").ToList();
                bookingDate = DateTime.Parse($"{tempString[0]} {tempString[1]}");
                objMeetup.EntityName = tempString[2];
                objMeetup.MeetupStart = DateTime.Parse($"{tempString[3]} {tempString[4]}");
                objMeetup.MeetupEnd = objMeetup.MeetupStart.AddHours(Convert.ToInt32(tempString[5]));
                return objMeetup;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new Meetup();
            }
        }

        /// <summary>
        /// Fetching office hours from first line of input string
        /// </summary>
        /// <param name="_meetupInput"></param>
        /// <returns> Officehours object </returns>
        public OfficeHours GetOfficeHours(string _meetupInput)
        {
            OfficeHours officeHours = new OfficeHours();
            try
            {
                officeHours.StartTime = TimeSpan.Parse(DateTime.ParseExact(_meetupInput.Split(" ")[0], "HHmm", CultureInfo.InvariantCulture).ToString("HH:mm"));
                officeHours.EndTime = TimeSpan.Parse(DateTime.ParseExact(_meetupInput.Split(" ")[1], "HHmm", CultureInfo.InvariantCulture).ToString("HH:mm"));
                return officeHours;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new OfficeHours();
            }
        }

        /// <summary>
        /// verify if meetup object lies in the offic hours
        /// </summary>
        /// <param name="objMeetup"></param>
        /// <param name="officeHours"></param>
        /// <returns> boolean </returns>
        public bool ValidateOfficeTimeLimits(Meetup objMeetup, OfficeHours officeHours)
        {
            try
            {
                return (objMeetup.MeetupStart.TimeOfDay >= officeHours.StartTime && objMeetup.MeetupEnd.TimeOfDay <= officeHours.EndTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// verify if meetup overlaps with any other meetings saved in the final list
        /// </summary>
        /// <param name="objMeetup"></param>
        /// <param name="sortList"></param>
        /// <returns> bool </returns>
        public bool ValidateOverlappingMeetup(Meetup objMeetup, SortedList<DateTime, Meetup> sortList)
        {
            try
            {
                return sortList.Count > 0 && sortList.Values.Any(a => a.MeetupStart >= objMeetup.MeetupStart && a.MeetupEnd <= objMeetup.MeetupEnd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }

    public class OfficeHours
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}