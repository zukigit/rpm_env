namespace CustomControls
{
   using System;
   using System.Drawing;

   /// <summary>
   /// Class that represents a week number in the <see cref="MonthCalendarMonth"/>.
   /// </summary>
   public class MonthCalendarWeek
   {
      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarWeek"/> class.
      /// </summary>
      /// <param name="month">The corresponding <see cref="MonthCalendarMonth"/> that contains this instance.</param>
      /// <param name="weekNumber">The corresponding week number.</param>
      /// <param name="start">The start date of the week.</param>
      /// <param name="end">The end date of the week.</param>
      public MonthCalendarWeek(MonthCalendarMonth month, int weekNumber, DateTime start, DateTime end)
      {
         this.WeekNumber = weekNumber;
         this.Start = start;
         this.End = end;
         this.Month = month;
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets the hosting <see cref="MonthCalendarMonth"/> instance.
      /// </summary>
      public MonthCalendarMonth Month { get; private set; }

      /// <summary>
      /// Gets the hosting <see cref="MonthCalendar"/> instance.
      /// </summary>
      public MonthCalendar MonthCalendar
      {
         get { return this.Month.MonthCalendar; }
      }

      /// <summary>
      /// Gets the week number.
      /// </summary>
      public int WeekNumber { get; private set; }

      /// <summary>
      /// Gets the start date of the week.
      /// </summary>
      public DateTime Start { get; private set; }

      /// <summary>
      /// Gets the end date of the week.
      /// </summary>
      public DateTime End { get; private set; }

      /// <summary>
      /// Gets or sets the bounds of the week number.
      /// </summary>
      public Rectangle Bounds { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether this week number is visible.
      /// </summary>
      public bool Visible { get; set; }

      #endregion
   }
}