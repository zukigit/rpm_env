namespace CustomControls
{
   using System;
   using System.Collections.Generic;
   using System.Drawing;

   /// <summary>
   /// Class that represents a single month in the <see cref="MonthCalendar"/>.
   /// </summary>
   public class MonthCalendarMonth
   {
      #region fields

      /// <summary>
      /// The location of the month.
      /// </summary>
      private Point location;

      /// <summary>
      /// The first visible date.
      /// </summary>
      private DateTime firstVisibleDate = DateTime.MinValue;

      /// <summary>
      /// The last visible date.
      /// </summary>
      private DateTime lastVisibleDate = DateTime.MinValue;

      #endregion

      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarMonth"/> class.
      /// </summary>
      /// <param name="monthCal">The <see cref="MonthCalendar"/> which hosts the <see cref="MonthCalendarMonth"/> instance.</param>
      /// <param name="date">The date that represents the month and year which is displayed in this instance.</param>
      public MonthCalendarMonth(MonthCalendar monthCal, DateTime date)
      {
         this.MonthCalendar = monthCal;
         this.Date = date;
         this.location = new Point(0, 0);

         MonthCalendarDate dt = new MonthCalendarDate(monthCal.CultureCalendar, date).FirstOfMonth.GetFirstDayInWeek(monthCal.FormatProvider);

         List<MonthCalendarDay> dayList = new List<MonthCalendarDay>();
         List<MonthCalendarWeek> weekList = new List<MonthCalendarWeek>();

         int dayAdjust = 0;

         while (dt.AddDays(dayAdjust).DayOfWeek != monthCal.FormatProvider.FirstDayOfWeek)
         {
            dayAdjust++;
         }

         int d = dayAdjust != 0 ? 8 - dayAdjust : 0;

         for (int i = dayAdjust; i < 42 + dayAdjust; i++, dt = dt.AddDays(1))
         {
            MonthCalendarDay day = new MonthCalendarDay(this, dt.Date);

            dayList.Add(day);

            if (day.Visible)
            {  

               if (this.firstVisibleDate == DateTime.MinValue)
               {
                  this.firstVisibleDate = dt.Date;
               }

               if (!day.TrailingDate)
               {
                   this.lastVisibleDate = dt.Date;
               }
            }

            if (i == dayAdjust || ((i - d) % 7) == 0)
            {
               DateTime weekEnd = dt.GetEndDateOfWeek(monthCal.FormatProvider).Date;

               int weekNumEnd = DateMethods.GetWeekOfYear(monthCal.Culture, monthCal.CultureCalendar, weekEnd);

               weekList.Add(new MonthCalendarWeek(this, weekNumEnd, dt.Date, weekEnd));
            }

            if (dt.Date == monthCal.CultureCalendar.MaxSupportedDateTime.Date)
            {
               break;
            }
         }

         this.Days = dayList.ToArray();
         this.Weeks = weekList.ToArray();
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets or sets the index of the month.
      /// </summary>
      public int Index { get; set; }

      /// <summary>
      /// Gets the bounds of this instance.
      /// </summary>
      public Rectangle Bounds
      {
         get { return new Rectangle(this.location, this.Size); }
      }

      /// <summary>
      /// Gets the <see cref="MonthCalendar"/> which hosts the month.
      /// </summary>
      public MonthCalendar MonthCalendar { get; private set; }

      /// <summary>
      /// Gets the date which specifies the displayed month and year.
      /// </summary>
      public DateTime Date { get; private set; }

      /// <summary>
      /// Gets or sets the location of the this month.
      /// </summary>
      public Point Location
      {
         get
         {
            return this.location;
         }

         set
         {
            this.location = value;

            this.CalculateProportions(value);
         }
      }

      /// <summary>
      /// Gets the <see cref="MonthCalendarDay"/> instances that are being hosted.
      /// </summary>
      public MonthCalendarDay[] Days { get; private set; }

      /// <summary>
      /// Gets or sets the bounds of the days.
      /// </summary>
      public Rectangle DayNamesBounds { get; set; }

      /// <summary>
      /// Gets or sets the weeks displayed.
      /// </summary>
      public MonthCalendarWeek[] Weeks { get; set; }

      /// <summary>
      /// Gets or sets the title bounds where the month and year is displayed.
      /// </summary>
      public Rectangle TitleBounds { get; set; }

      /// <summary>
      /// Gets or sets the bounds for the month in the header.
      /// </summary>
      public Rectangle TitleMonthBounds { get; set; }

      /// <summary>
      /// Gets or sets the bounds for the year in the header.
      /// </summary>
      public Rectangle TitleYearBounds { get; set; }

      /// <summary>
      /// Gets or sets the week bounds.
      /// </summary>
      public Rectangle WeekBounds { get; set; }

      /// <summary>
      /// Gets or sets the bounds in which the days are drawn.
      /// </summary>
      public Rectangle MonthBounds { get; set; }

      /// <summary>
      /// Gets the size of this instance of <see cref="MonthCalendarMonth"/>.
      /// </summary>
      public Size Size
      {
         get { return this.MonthCalendar.MonthSize; }
      }

      /// <summary>
      /// Gets the first visible date.
      /// </summary>
      public DateTime FirstVisibleDate
      {
         get { return this.firstVisibleDate; }
      }

      /// <summary>
      /// Gets the last visible date.
      /// </summary>
      public DateTime LastVisibleDate
      {
         get { return this.lastVisibleDate; }
      }

      /// <summary>
      /// Gets a value indicating whether to draw the left button.
      /// </summary>
      //internal bool DrawLeftButton
      //{
      //   get
      //   {
      //      return this.MonthCalendar.UseRTL ?
      //         this.Index == this.MonthCalendar.CalendarDimensions.Width - 1 : this.Index == 0;
      //   }
      //}

      /// <summary>
      /// Gets a value indicating whether to draw the right button.
      /// </summary>
      //internal bool DrawRightButton
      //{
      //   get
      //   {
      //      return this.MonthCalendar.UseRTL ?
      //         this.Index == 0 : this.Index == this.MonthCalendar.CalendarDimensions.Width - 1;
      //   }
      //}

      #endregion

      #region methods

      /// <summary>
      /// Performs a hit test on the specified position.
      /// </summary>
      /// <param name="loc">The position to perform the hit test for.</param>
      /// <returns>An <see cref="MonthCalendarHitTest"/> that contains the hit test data.</returns>
      public MonthCalendarHitTest HitTest(Point loc)
      {
         if (this.TitleBounds.Contains(loc))
         {
            DateTime dt = this.MonthCalendar.CultureCalendar.GetEra(this.Date) !=
                          this.MonthCalendar.CultureCalendar.GetEra(this.firstVisibleDate)
                             ? this.firstVisibleDate
                             : this.Date;

            if (this.TitleMonthBounds.Contains(loc))
            {
               return new MonthCalendarHitTest(dt, MonthCalendarHitType.MonthName, this.TitleMonthBounds, this.TitleBounds);
            }

            if (this.TitleYearBounds.Contains(loc))
            {
               return new MonthCalendarHitTest(dt, MonthCalendarHitType.MonthYear, this.TitleYearBounds, this.TitleBounds);
            }

            return new MonthCalendarHitTest(this.Date, MonthCalendarHitType.Header, this.TitleBounds);
         }

         if (this.WeekBounds.Contains(loc))
         {
            foreach (MonthCalendarWeek week in this.Weeks)
            {
               if (week.Visible && week.Bounds.Contains(loc))
               {
                  return new MonthCalendarHitTest(week.Start, MonthCalendarHitType.Week, week.Bounds);
               }
            }
         }
         else if (this.MonthBounds.Contains(loc))
         {
            foreach (MonthCalendarDay day in this.Days)
            {
               if (day.Bounds.Contains(loc) && day.Visible)
               {
                  return new MonthCalendarHitTest(day.Date, MonthCalendarHitType.Day, day.Bounds);
               }
            }
         }
         else if (this.DayNamesBounds.Contains(loc))
         {
            int dayWidth = this.MonthCalendar.DaySize.Width;
            Rectangle dayNameBounds = this.DayNamesBounds;
            dayNameBounds.Width = dayWidth;

            for (int i = 0; i < 7; i++)
            {
               if (dayNameBounds.Contains(loc))
               {
                  return new MonthCalendarHitTest(this.Days[i].Date, MonthCalendarHitType.DayName, dayNameBounds);
               }

               dayNameBounds.X += dayWidth;
            }
         }

         return MonthCalendarHitTest.Empty;
      }

      /// <summary>
      /// Determines if the specified date is contained within this <see cref="MonthCalendarMonth"/> instance.
      /// </summary>
      /// <param name="date">The date to be determined.</param>
      /// <returns>true if the specified date is visible; otherwise false.</returns>
      public bool ContainsDate(DateTime date)
      {
         return date >= this.firstVisibleDate && date <= this.lastVisibleDate;
      }

      /// <summary>
      /// Calculates the proportions of this instance of <see cref="MonthCalendarMonth"/>.
      /// </summary>
      /// <param name="loc">The top left corner of the month.</param>
      private void CalculateProportions(Point loc)
      {
         // set title bounds
         this.TitleBounds = new Rectangle(loc, this.MonthCalendar.HeaderSize);

         // set helper variables
         bool useRTL = this.MonthCalendar.UseRTL;
         int adjustX = this.MonthCalendar.WeekNumberSize.Width;
         int dayWidth = this.MonthCalendar.DaySize.Width;
         int dayHeight = this.MonthCalendar.DaySize.Height;
         int weekRectAdjust = 0;

         // if RTL mode
         if (useRTL)
         {
            // set new values
            weekRectAdjust = dayWidth * 7;
            adjustX = 0;
         }

         // calculate day names header bounds
         this.DayNamesBounds = new Rectangle(
            new Point(loc.X + adjustX, loc.Y + this.TitleBounds.Height),
            this.MonthCalendar.DayNamesSize);

         // calculate week number header bounds
         Rectangle weekNumberRect = new Rectangle(
            loc.X + weekRectAdjust,
            loc.Y + this.TitleBounds.Height + this.DayNamesBounds.Height,
            this.MonthCalendar.WeekNumberSize.Width,
            dayHeight);

         // save week header bounds
         Rectangle weekBounds = weekNumberRect;

         // calculate month body bounds
         Rectangle monthRect = new Rectangle(
            loc.X + adjustX,
            loc.Y + this.TitleBounds.Height + this.DayNamesBounds.Height,
            dayWidth * 7,
            dayHeight * 6);
         // get start position at where to draw
         int startX = monthRect.X;
         adjustX = dayWidth;

         // if in RTL mode adjust start position and advancing width
         if (useRTL)
         {
            startX = monthRect.Right - dayWidth;
            adjustX = -dayWidth;
         }

         int x = startX;
         int y = monthRect.Y;
         int j = 0;

         if (this.Days.Length > 0 && new MonthCalendarDate(this.MonthCalendar.CultureCalendar, this.Days[0].Date).DayOfWeek != this.MonthCalendar.FormatProvider.FirstDayOfWeek)
         {
            DayOfWeek currentDayOfWeek = this.MonthCalendar.FormatProvider.FirstDayOfWeek;
            DayOfWeek dayOfWeek = new MonthCalendarDate(this.MonthCalendar.CultureCalendar, this.Days[0].Date).DayOfWeek;

            while (currentDayOfWeek != dayOfWeek)
            {
               x += adjustX;

               if ((j + 1) % 7 == 0)
               {
                  x = startX;
                  y += dayHeight;
               }

               int nextDay = (int)currentDayOfWeek + 1;

               if (nextDay > 6)
               {
                  nextDay = 0;
               }

               currentDayOfWeek = (DayOfWeek)nextDay;

               j++;
            }
         }

         // loop through all possible 42 days
         for (int i = 0; i < this.Days.Length; i++, j++)
         {
            // set bounds of the day
            this.Days[i].Bounds = new Rectangle(x, y, dayWidth, dayHeight);

            // if at the beginning of a row
            if (i % 7 == 0)
            {
               // check if any day in the week is visible
               bool visible = this.Days[i].Visible || this.Days[Math.Min(i + 6, this.Days.Length - 1)].Visible;

               // set the week number header element bounds and if it's visible
               this.Weeks[i / 7].Bounds = weekNumberRect;
               this.Weeks[i / 7].Visible = visible;

               // adjust bounds of week header
               if (visible)
               {
                  weekBounds = Rectangle.Union(weekBounds, weekNumberRect);
               }

               // adjust top position of the week number header bounds
               weekNumberRect.Y += dayHeight;
            }

            // adjust left position of the next day
            x += adjustX;

            // if last day in the row
            if ((j + 1) % 7 == 0)
            {
               // reset to start drawing position
               x = startX;

               // and advance a row
               y += dayHeight;
            }
         }

         // adjust the month body height and top position
         monthRect.Y = weekBounds.Y;
         monthRect.Height = weekBounds.Height;

         // set month body bounds
         this.MonthBounds = monthRect;

         // set week number header bounds
         this.WeekBounds = weekBounds;
      }

      #endregion
   }
}