namespace CustomControls
{
   using System;

   /// <summary>
   /// Stores information regarding the element currently under the mouse cursor.
   /// </summary>
   internal class MonthCalendarMouseMoveFlags
   {
      #region fields

      /// <summary>
      /// The backup of the states.
      /// </summary>
      private MonthCalendarMouseMoveFlags backup;

      #endregion

      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarMouseMoveFlags"/> class.
      /// </summary>
      public MonthCalendarMouseMoveFlags()
      {
         this.Reset();
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets or sets a value indicating whether the mouse is over the left arrow.
      /// </summary>
      public bool LeftArrow { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether the mouse is over the right arrow.
      /// </summary>
      public bool RightArrow { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether the mouse is over a week header element.
      /// </summary>
      public bool WeekHeader { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether the mouse is over the footer.
      /// </summary>
      public bool Footer { get; set; }

      /// <summary>
      /// Gets or sets the date of the month name in a header the mouse is currently over.
      /// </summary>
      public DateTime MonthName { get; set; }

      /// <summary>
      /// Gets or sets the date of the year in a header the mouse is currently over.
      /// </summary>
      public DateTime Year { get; set; }

      /// <summary>
      /// Gets or sets the date of the month if the mouse is over a month header.
      /// </summary>
      public DateTime HeaderDate { get; set; }

      /// <summary>
      /// Gets or sets the date if the mouse is over an <see cref="MonthCalendarDay"/>.
      /// </summary>
      public DateTime Day { get; set; }

      /// <summary>
      /// Gets the backup flags.
      /// </summary>
      public MonthCalendarMouseMoveFlags Backup
      {
         get { return this.backup ?? (this.backup = new MonthCalendarMouseMoveFlags()); }
      }

      /// <summary>
      /// Gets a value indicating whether the left arrow state changed.
      /// </summary>
      public bool LeftArrowChanged
      {
         get { return this.LeftArrow != this.Backup.LeftArrow; }
      }

      /// <summary>
      /// Gets a value indicating whether the right arrow state changed.
      /// </summary>
      public bool RightArrowChanged
      {
         get { return this.RightArrow != this.Backup.RightArrow; }
      }

      /// <summary>
      /// Gets a value indicating whether the week header state changed.
      /// </summary>
      public bool WeekHeaderChanged
      {
         get { return this.WeekHeader != this.Backup.WeekHeader; }
      }

      /// <summary>
      /// Gets a value indicating whether the month name state changed.
      /// </summary>
      public bool MonthNameChanged
      {
         get { return this.MonthName != this.Backup.MonthName; }
      }

      /// <summary>
      /// Gets a value indicating whether the year state changed.
      /// </summary>
      public bool YearChanged
      {
         get { return this.Year != this.Backup.Year; }
      }

      /// <summary>
      /// Gets a value indicating whether the header date state changed.
      /// </summary>
      public bool HeaderDateChanged
      {
         get { return this.HeaderDate != this.Backup.HeaderDate; }
      }

      /// <summary>
      /// Gets a value indicating whether the day state changed.
      /// </summary>
      public bool DayChanged
      {
         get { return this.Day != this.Backup.Day; }
      }

      /// <summary>
      /// Gets a value indicating whether the footer state changed.
      /// </summary>
      public bool FooterChanged
      {
         get { return this.Footer != this.Backup.Footer; }
      }

      #endregion

      #region methods

      /// <summary>
      /// Resets the flags.
      /// </summary>
      public void Reset()
      {
         this.LeftArrow = this.RightArrow = this.WeekHeader = this.Footer = false;
         this.MonthName = this.Year = this.HeaderDate = this.Day = DateTime.MinValue;
      }

      /// <summary>
      /// Saves the current values to <see cref="Backup"/> and resets then the current values.
      /// </summary>
      public void BackupAndReset()
      {
         this.Backup.LeftArrow = this.LeftArrow;
         this.Backup.RightArrow = this.RightArrow;
         this.Backup.WeekHeader = this.WeekHeader;
         this.Backup.MonthName = this.MonthName;
         this.Backup.Year = this.Year;
         this.Backup.HeaderDate = this.HeaderDate;
         this.Backup.Day = this.Day;
         this.Backup.Footer = this.Footer;

         this.Reset();
      }

      #endregion
   }
}