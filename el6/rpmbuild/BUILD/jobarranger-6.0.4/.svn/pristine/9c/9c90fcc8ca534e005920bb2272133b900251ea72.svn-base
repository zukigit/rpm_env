namespace CustomControls
{
   using System;

   /// <summary>
   /// Provides data for the <see cref="MonthCalendar.ActiveDateChanged"/> or <see cref="DatePicker.ActiveDateChanged"/> events.
   /// </summary>
   public class ActiveDateChangedEventArgs : DateEventArgs
   {
      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="ActiveDateChangedEventArgs"/> class.
      /// </summary>
      /// <param name="date">The corresponding date.</param>
      /// <param name="boldDate">true if the <paramref name="date"/> value is a bolded date.</param>
      public ActiveDateChangedEventArgs(DateTime date, bool boldDate)
         : base(date)
      {
         this.IsBoldDate = boldDate;
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets a value indicating whether the <see cref="DateEventArgs.Date"/> value is a bolded date.
      /// </summary>
      public bool IsBoldDate { get; private set; }

      #endregion
   }
}