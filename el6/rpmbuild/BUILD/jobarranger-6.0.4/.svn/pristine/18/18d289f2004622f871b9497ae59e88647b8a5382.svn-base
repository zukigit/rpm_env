namespace CustomControls
{
   using System;
   using System.Drawing;

   /// <summary>
   /// Represents a day in the <see cref="MonthCalendar"/>.
   /// </summary>
   public class MonthCalendarDay
   {
      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarDay"/> class.
      /// </summary>
      /// <param name="month">The <see cref="MonthCalendarMonth"/> in which the day is in.</param>
      /// <param name="date">The <see cref="DateTime"/> the <see cref="MonthCalendarDay"/> represents.</param>
      public MonthCalendarDay(MonthCalendarMonth month, DateTime date)
      {
         this.Month = month;
         this.Date = date;
         this.MonthCalendar = month.MonthCalendar;
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets or sets the bounds of the day.
      /// </summary>
      public Rectangle Bounds { get; set; }

      /// <summary>
      /// Gets the date the <see cref="MonthCalendarDay"/> represents.
      /// </summary>
      public DateTime Date { get; private set; }

      /// <summary>
      /// Gets the <see cref="MonthCalendarMonth"/> the day is in.
      /// </summary>
      public MonthCalendarMonth Month { get; private set; }

      /// <summary>
      /// Gets the <see cref="MonthCalendar"/> the <see cref="MonthCalendarMonth"/> is in.
      /// </summary>
      public MonthCalendar MonthCalendar { get; private set; }

      /// <summary>
      /// Gets a value indicating whether the represented date is selected.
      /// </summary>
      public bool Selected
      {
         get { return this.MonthCalendar.IsSelected(this.Date); }
      }

      /// <summary>
      /// Gets a value indicating whether the mouse is over the represented date.
      /// </summary>
      public bool MouseOver
      {
         get
         {
            return this.Date == this.MonthCalendar.MouseOverDay;
         }
      }

      /// <summary>
      /// Gets a value indicating whether the represented date is a trailing one.
      /// </summary>
      public bool TrailingDate
      {
         get
         {
            return this.MonthCalendar.CultureCalendar.GetMonth(this.Date) != this.MonthCalendar.CultureCalendar.GetMonth(this.Month.Date);
         }
      }

      /// <summary>
      /// Gets a value indicating whether the represented date is visible.
      /// </summary>
      public bool Visible
      {
         get
         {
            if (this.Date == this.MonthCalendar.ViewStart && this.MonthCalendar.ViewStart == this.MonthCalendar.MinDate)
            {
               return true;
            }

            //return this.Date >= this.MonthCalendar.MinDate && this.Date <= this.MonthCalendar.MaxDate && !(this.TrailingDate
            //   && this.Date >= this.MonthCalendar.ViewStart
            //   && this.Date <= this.MonthCalendar.ViewEnd);
            return this.Date >= this.MonthCalendar.MinDate && this.Date <= this.MonthCalendar.MaxDate && !this.TrailingDate;
         }
      }

      #endregion
   }
}