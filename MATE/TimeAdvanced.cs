using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATE
{
    public class TimeAdvanced
    {
        private int _hours;

        private int _minutes;

        private DateTime CurrentTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public int Hours
        {
            get
            {
                return _hours;
            }
            private set
            {
                _hours = value;
            }
        }

        public int Minutes
        {
            get
            {
                return _minutes;
            }
            private set
            {
                _minutes = value;
            }
        }

        public TimeAdvanced(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
        }

        public TimeAdvanced() { }

        public TimeAdvanced SetTime(DateTime dateTime)
        {
            return new TimeAdvanced(dateTime.Hour, dateTime.Minute);
        }

        public void ChangeTime(DateTime dateTime)
        {
            Hours = dateTime.Hour;
            Minutes = dateTime.Minute;
        }

        public void ChangeTime(TimeSpan timeSpan)
        {
            Hours += timeSpan.Hours;
            Minutes += timeSpan.Minutes;
        }

        public int GetTotalMinutes()
        {
            return Hours * 60 + Minutes;
        }

        public int GetSpanMinutes(DateTime dateTime)
        {
            var timeLeft = CurrentTime - dateTime;

            return (int)timeLeft.TotalMinutes;
        }

        public int GetSpanSeconds(DateTime dateTime)
        {
            var timeLeft = CurrentTime - dateTime;

            return (int)timeLeft.TotalSeconds;
        }

        public static TimeAdvanced operator -(TimeAdvanced time1, TimeAdvanced time2) 
            => new TimeAdvanced(time1.Hours - time2.Hours, time1.Minutes - time2.Minutes);
    }
}
