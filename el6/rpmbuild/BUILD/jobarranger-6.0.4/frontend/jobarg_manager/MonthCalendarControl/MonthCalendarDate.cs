namespace CustomControls
{
   using System;
   using System.Globalization;
   using System.Text;

   /// <summary>
   /// Class for date handling in different calendars.
   /// </summary>
   internal class MonthCalendarDate : ICloneable
   {
      #region fields

      /// <summary>
      /// The calendar to use.
      /// </summary>
      private Calendar calendar = CultureInfo.CurrentUICulture.Calendar;

      /// <summary>
      /// The gregorian date this object represents.
      /// </summary>
      private DateTime date = DateTime.MinValue;

      /// <summary>
      /// The gregorian date for the first day in the month and year specified by the <see cref="date"/>.
      /// </summary>
      private MonthCalendarDate firstOfMonth;

      #endregion

      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarDate"/> class.
      /// </summary>
      /// <param name="cal">The calendar to use.</param>
      /// <param name="date">The gregorian date.</param>
      /// <exception cref="ArgumentNullException">If <paramref name="cal"/> is <c>null</c>.</exception>
      public MonthCalendarDate(Calendar cal, DateTime date)
      {
         if (cal == null)
         {
            throw new ArgumentNullException("cal", "parameter 'cal' cannot be null.");
         }

         if (date < cal.MinSupportedDateTime)
         {
            date = cal.MinSupportedDateTime;
         }

         if (date > cal.MaxSupportedDateTime)
         {
            date = cal.MaxSupportedDateTime;
         }

         this.Year = cal.GetYear(date);
         this.Month = cal.GetMonth(date);
         this.Day = cal.GetDayOfMonth(date);
         this.Era = cal.GetEra(date);
         this.calendar = cal;
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets the gregorian date this object represents.
      /// </summary>
      public DateTime Date
      {
         get
         {
            if (this.date == DateTime.MinValue)
            {
               this.date = this.calendar.ToDateTime(this.Year, this.Month, this.Day, 0, 0, 0, 0, this.Era);
            }

            return this.date;
         }
      }

      /// <summary>
      /// Gets the day in the <see cref="Calendar"/>.
      /// </summary>
      public int Day { get; private set; }

      /// <summary>
      /// Gets the month in the <see cref="Calendar"/>.
      /// </summary>
      public int Month { get; private set; }

      /// <summary>
      /// Gets the year in the <see cref="Calendar"/>.
      /// </summary>
      public int Year { get; private set; }

      /// <summary>
      /// Gets the era of the date represented by this instance.
      /// </summary>
      public int Era { get; private set; }

      /// <summary>
      /// Gets the <see cref="DayOfWeek"/> value this date represents in the <see cref="Calendar"/>.
      /// </summary>
      public DayOfWeek DayOfWeek
      {
         get { return this.calendar.GetDayOfWeek(this.Date); }
      }

      /// <summary>
      /// Gets the first date in the month as gregorian date.
      /// </summary>
      public MonthCalendarDate FirstOfMonth
      {
         get
         {
            if (this.firstOfMonth == null)
            {
               if (this.Date == this.calendar.MinSupportedDateTime.Date)
               {
                  this.firstOfMonth = this.Clone() as MonthCalendarDate;
               }
               else
               {
                  this.firstOfMonth = new MonthCalendarDate(this.calendar, this.calendar.ToDateTime(this.Year, this.Month, 1, 0, 0, 0, 0, this.Era));
               }
            }

            return this.firstOfMonth;
         }
      }

      /// <summary>
      /// Gets the number of days in the month specified by <see cref="Date"/> using the <see cref="Calendar"/>.
      /// </summary>
      public int DaysInMonth
      {
         get { return DateMethods.GetDaysInMonth(this); }
      }

      /// <summary>
      /// Gets or sets the <see cref="System.Globalization.Calendar"/> to use
      /// in converting the gregorian date this object represents.
      /// </summary>
      public Calendar Calendar
      {
         get
         {
            return this.calendar;
         }

         set
         {
            if (value != null)
            {
               this.calendar = value;
            }
         }
      }

      #endregion

      #region methods

      /// <summary>
      /// Gets the first day in the week.
      /// </summary>
      /// <param name="provider">The format provider.</param>
      /// <returns>The first day in the week specified by this instance.</returns>
      public MonthCalendarDate GetFirstDayInWeek(ICustomFormatProvider provider)
      {
         DayOfWeek firstDayOfWeek = provider.FirstDayOfWeek;
         MonthCalendarDate dt = (MonthCalendarDate)this.Clone();

         while (dt.DayOfWeek != firstDayOfWeek && dt.Date > this.calendar.MinSupportedDateTime)
         {
            dt = dt.AddDays(-1);
         }

         return dt;
      }

      /// <summary>
      /// Gets the last date of the week specified by this instance.
      /// </summary>
      /// <param name="provider">The format provider.</param>
      /// <returns>The last date in the week.</returns>
      public MonthCalendarDate GetEndDateOfWeek(ICustomFormatProvider provider)
      {
         DayOfWeek currentDayOfWeek = this.DayOfWeek;
         DayOfWeek firstDayOfWeek = provider.FirstDayOfWeek;

         if (currentDayOfWeek == firstDayOfWeek)
         {
            return this.AddDays(6);
         }

         int d = (int)currentDayOfWeek;
         int daysToAdd = -1;

         while (currentDayOfWeek != firstDayOfWeek)
         {
            daysToAdd++;

            if (++d > 6)
            {
               d = 0;
            }

            currentDayOfWeek = (DayOfWeek)d;
         }

         return this.AddDays(daysToAdd);
      }

      /// <summary>
      /// Adds the specified months to the date represented by this instance.
      /// </summary>
      /// <param name="months">The months to add.</param>
      /// <returns>Either the resulting date or the min/max date of the current calendar if the adding of months resulted in an invalid date for the current calendar.</returns>
      public MonthCalendarDate AddMonths(int months)
      {
         return new MonthCalendarDate(this.calendar, this.InternalAddMonths(months));
      }

      /// <summary>
      /// Adds the specified days to the date represented by this instance.
      /// </summary>
      /// <param name="days">The days to add.</param>
      /// <returns>The resulting date or the min/max date of the current calendar if the resulting date is out of bounds for the current calendar.</returns>
      public MonthCalendarDate AddDays(int days)
      {
         DateTime dt = this.Date;
         int sign = Math.Sign(days);
         days = Math.Abs(days);
         int daysAdded = 0;

         while (((dt > this.calendar.MinSupportedDateTime && sign == -1) || (dt < this.calendar.MaxSupportedDateTime.Date && sign == 1)) && days != daysAdded)
         {
            dt = this.calendar.AddDays(dt, sign);
            daysAdded++;
         }

         return new MonthCalendarDate(this.calendar, dt);
      }

      /// <summary>
      /// Returns a string representation of the native <see cref="Calendar"/> dependent date using the current UI's <see cref="DateTimeFormatInfo.ShortDatePattern"/>.
      /// </summary>
      /// <returns>A string representation of this object.</returns>
      public override string ToString()
      {
         return this.ToString(
            CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern,
            CultureInfo.CurrentUICulture.DateTimeFormat
            );
      }

      /// <summary>
      /// Creates a new object that is a copy of the current instance.
      /// </summary>
      /// <returns>A new object that is a copy of this instance.</returns>
      public object Clone()
      {
         return this.MemberwiseClone();
      }

      /// <summary>
      /// Returns a string representation of the native <see cref="Calendar"/> dependent date using the <see cref="IFormatProvider"/> <see cref="DateTimeFormatInfo"/>
      /// objects <see cref="DateTimeFormatInfo.ShortDatePattern"/>.
      /// </summary>
      /// <param name="fp">The <see cref="IFormatProvider"/> to use for formatting.</param>
      /// <returns>A string representation of this object.</returns>
      public string ToString(IFormatProvider fp)
      {
         return this.ToString(null, fp);
      }

      /// <summary>
      /// Returns a string representation of the native <see cref="Calendar"/> dependent date using the <see cref="IFormatProvider"/> and <paramref name="format"/>
      /// for formatting.
      /// </summary>
      /// <param name="format">A string which holds the format pattern.</param>
      /// <param name="fp">The <see cref="IFormatProvider"/> to use for formatting.</param>
      /// <param name="nameProvider">The day and month name provider.</param>
      /// <param name="nativeDigits">If not <c>null</c> and valid, uses for number representation the specified native digits.</param>
      /// <returns>
      /// A string representation of this object.
      /// </returns>
      public string ToString(string format, IFormatProvider fp, ICustomFormatProvider nameProvider = null, string[] nativeDigits = null)
      {
         if (fp != null)
         {
            DateTimeFormatInfo dtfi = DateTimeFormatInfo.GetInstance(fp);

            if (dtfi != null)
            {
               return this.ToString(format, dtfi, nameProvider, nativeDigits);
            }
         }

         return this.ToString(format, null, nameProvider, nativeDigits);
      }

      /// <summary>
      /// Returns the gregorian date for the first day in the month.
      /// </summary>
      /// <param name="cal">The <see cref="System.Globalization.Calendar"/> to use. If null the date <paramref name="year"/>.<paramref name="month"/>.<c>1</c> is returned.</param>
      /// <param name="year">The year to use.</param>
      /// <param name="month">The month to use.</param>
      /// <returns>The gregorian date for the first date in the month specified by <paramref name="year"/>, <paramref name="month"/> and <paramref name="cal"/>.</returns>
      public static DateTime GetFirstOfMonth(Calendar cal, int year, int month)
      {
         if (cal == null)
         {
            return new DateTime(year, month, 1);
         }

         return new DateTime(year, month, 1, cal);
      }

      /// <summary>
      /// Calculates the date after adding the specified number of months.
      /// </summary>
      /// <param name="months">The months to add.</param>
      /// <returns>The resulting <see cref="DateTime"/> value.</returns>
      private DateTime InternalAddMonths(int months)
      {
         try
         {
            DateTime dt = this.Date;
            int monthsToAdd = Math.Abs(months);
            int sign = Math.Sign(months);
            int counter = 0;

            while (counter != monthsToAdd)
            {
               dt = this.calendar.AddMonths(dt, sign);
               counter++;
            }

            return dt;
         }
         catch
         {
         }

         return months < 0 ? this.calendar.MinSupportedDateTime.Date : this.calendar.MaxSupportedDateTime.Date;
      }

      /// <summary>
      /// Returns a string representation of this object.
      /// </summary>
      /// <param name="format">The format pattern.</param>
      /// <param name="dtfi">The <see cref="DateTimeFormatInfo"/> to use for formatting.</param>
      /// <param name="nameProvider">The day and month name provider.</param>
      /// <param name="nativeDigits">If not <c>null</c> and valid, uses for number representation the specified native digits.</param>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      private string ToString(string format, DateTimeFormatInfo dtfi, ICustomFormatProvider nameProvider, string[] nativeDigits = null)
      {
         if (dtfi == null)
         {
            dtfi = CultureInfo.CurrentUICulture.DateTimeFormat;
         }

         if (string.IsNullOrEmpty(format))
         {
            format = nameProvider != null ? nameProvider.ShortDatePattern : dtfi.ShortDatePattern;
         }
         else if (format.Length == 1)
         {
            switch (format[0])
            {
               case 'D':
                  {
                     format = nameProvider != null ? nameProvider.LongDatePattern : dtfi.LongDatePattern;

                     break;
                  }

               case 'm':
               case 'M':
                  {
                     format = nameProvider != null ? nameProvider.MonthDayPattern : dtfi.MonthDayPattern;

                     break;
                  }

               case 'y':
               case 'Y':
                  {
                     format = nameProvider != null ? nameProvider.YearMonthPattern : dtfi.YearMonthPattern;

                     break;
                  }

               default:
                  {
                     format = nameProvider != null ? nameProvider.ShortDatePattern : dtfi.ShortDatePattern;

                     break;
                  }
            }
         }

         format = format.Replace(nameProvider != null ? nameProvider.DateSeparator : dtfi.DateSeparator, "/");

         StringBuilder sb = new StringBuilder();

         Calendar c = nameProvider != null ? nameProvider.Calendar : dtfi.Calendar;

         int i = 0;

         while (i < format.Length)
         {
            int tokLen;
            char ch = format[i];

            switch (ch)
            {
               case 'd':
                  {
                     tokLen = CountChar(format, i, ch);

                     sb.Append(tokLen <= 2
                        ? DateMethods.GetNumberString(this.Day, nativeDigits, tokLen == 2)
                        : GetDayName(c.GetDayOfWeek(this.Date), dtfi, nameProvider, tokLen == 3));

                     break;
                  }

               case 'M':
                  {
                     tokLen = CountChar(format, i, ch);
                     
                     sb.Append(tokLen <= 2
                        ? DateMethods.GetNumberString(this.Month, nativeDigits, tokLen == 2)
                        : GetMonthName(this.Month, this.Year, dtfi, nameProvider, tokLen == 3));

                     break;
                  }

               case 'y':
                  {
                     tokLen = CountChar(format, i, ch);

                     sb.Append(tokLen <= 2
                        ? DateMethods.GetNumberString(this.Year % 100, nativeDigits, true)
                        : DateMethods.GetNumberString(this.Year, nativeDigits, false));

                     break;
                  }

               case 'g':
                  {
                     tokLen = CountChar(format, i, ch);

                     sb.Append(nameProvider != null
                        ? nameProvider.GetEraName(c.GetEra(this.Date))
                        : dtfi.GetEraName(c.GetEra(this.Date)));

                     break;
                  }

               case '/':
                  {
                     tokLen = CountChar(format, i, ch);

                     sb.Append(nameProvider != null
                        ? nameProvider.DateSeparator
                        : dtfi.DateSeparator);

                     break;
                  }

               case '\'':
                  {
                     tokLen = 1;

                     break;
                  }

               default:
                  {
                     tokLen = 1;

                     sb.Append(ch.ToString(CultureInfo.CurrentUICulture));

                     break;
                  }
            }

            i += tokLen;
         }

         return sb.ToString();
      }

      /// <summary>
      /// Counts the specified <paramref name="c"/> at the position specified by <paramref name="p"/> in the string specified by <paramref name="fmt"/>.
      /// </summary>
      /// <param name="fmt">The string value to search.</param>
      /// <param name="p">The position start at.</param>
      /// <param name="c">The char value to count.</param>
      /// <returns>The count of the char <paramref name="c"/> at the specified location.</returns>
      private static int CountChar(string fmt, int p, char c)
      {
         int l = fmt.Length;
         int i = p + 1;

         while ((i < l) && (fmt[i] == c))
         {
            i++;
         }

         return i - p;
      }

      /// <summary>
      /// Gets the abbreviated or full day name for the specified <see cref="DayOfWeek"/> value using,
      /// if not null, the <paramref name="nameProvider"/> or the <see cref="DateTimeFormatInfo"/> specified by <paramref name="info"/>.
      /// </summary>
      /// <param name="dayofweek">The <see cref="DayOfWeek"/> value to get the day name for.</param>
      /// <param name="info">The <see cref="DateTimeFormatInfo"/> to get the name.</param>
      /// <param name="nameProvider">The <see cref="ICustomFormatProvider"/> to get the name.
      /// This parameter has precedence before the <paramref name="info"/>. Can be <c>null</c>.</param>
      /// <param name="abbreviated">true to get the abbreviated day name; false otherwise.</param>
      /// <returns>The full or abbreviated day name specified by <paramref name="dayofweek"/> value.</returns>
      private static string GetDayName(DayOfWeek dayofweek, DateTimeFormatInfo info, ICustomFormatProvider nameProvider, bool abbreviated)
      {
         if (nameProvider != null)
         {
            return abbreviated ? nameProvider.GetAbbreviatedDayName(dayofweek) : nameProvider.GetDayName(dayofweek);
         }

         return abbreviated ? info.GetAbbreviatedDayName(dayofweek) : info.GetDayName(dayofweek);
      }

      /// <summary>
      /// Gets the abbreviated or full month name for the specified <paramref name="month"/> and <paramref name="year"/> value,
      /// using the <paramref name="nameProvider"/> if not null or the <see cref="DateTimeFormatInfo"/> specified by <paramref name="info"/>.
      /// </summary>
      /// <param name="month">The month value to get the month name for.</param>
      /// <param name="year">The year value to get the month name for.</param>
      /// <param name="info">The <see cref="DateTimeFormatInfo"/> value to use.</param>
      /// <param name="nameProvider">The <see cref="ICustomFormatProvider"/> to get the name.
      /// This parameter has precedence before the <paramref name="info"/>. Can be <c>null</c>.</param>
      /// <param name="abbreviated">true to get the abbreviated month name; false otherwise.</param>
      /// <returns>The full or abbreviated month name specified by <paramref name="month"/> and <paramref name="year"/>.</returns>
      private static string GetMonthName(int month, int year, DateTimeFormatInfo info, ICustomFormatProvider nameProvider, bool abbreviated)
      {
         if (nameProvider != null)
         {
            return abbreviated
                      ? nameProvider.GetAbbreviatedMonthName(year, month)
                      : nameProvider.GetMonthName(year, month);
         }

         return abbreviated ? info.GetAbbreviatedMonthName(month) : info.GetMonthName(month);
      }

      #endregion
   }
}