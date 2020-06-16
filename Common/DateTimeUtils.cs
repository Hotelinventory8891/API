using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class DateTimeUtils
    {
        public static bool IsWeekEnd(this DateTime dateTime)
        {

            bool isWeekEnd = false;

            switch (dateTime.DayOfWeek)
            {

                case DayOfWeek.Sunday:

                case DayOfWeek.Saturday:

                    isWeekEnd = true;

                    break;

            }

            return isWeekEnd;

        }

        public static int FindNumberOfBusinessDays(this  DateTime fromDate, DateTime toDate, List<DateTime> Holidays)
        {
            int totalDays = 0;
            for (var date = fromDate.AddDays(1); date <= toDate; date = date.AddDays(1))
            {
                if (!date.IsWeekEnd() && !Holidays.Exists(x => x.Date == date.Date))
                    totalDays++;
            }

            return totalDays;
        }

        public static int FindNumberOfBusinessDays(this  DateTime fromDate, DateTime toDate)
        {
            return fromDate.FindNumberOfBusinessDays(toDate, null);
        }

        public static DateTime FindBusinessDay(this  DateTime fromDate, int numberOfBusinessDays, List<DateTime> Holidays)
        {

            //This is used to count the number of business days

            int businessDays = 0;

            int noOfDays = numberOfBusinessDays;

            for (int i = 1; i <= numberOfBusinessDays; i++)
            {

                //if current date is the WeekEnd increase the
                //numberOfBusinessDays with one

                //this is because we need one more loop ocurrrence

                if (IsWeekEnd(fromDate) || Holidays.Exists(x => x.Date == fromDate.Date))

                    numberOfBusinessDays++;

                else

                    businessDays++;



                //When businessDays is not equal to noOfDays,

                //add one day in the current date.

                if (businessDays != noOfDays)
                {

                    fromDate = fromDate.AddDays(1);

                }

                else
                {

                    break;

                }

            }

            return fromDate;

        }
        public static DateTime FindBusinessDay(this  DateTime fromDate, int numberOfBusinessDays)
        {

            return fromDate.FindBusinessDay(numberOfBusinessDays, null);

        }
    }
}
