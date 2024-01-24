namespace CustomControls
{
   /// <summary>
   /// The selection modes.
   /// </summary>
   public enum MonthCalendarSelectionMode
   {
      /// <summary>
      /// Indicates that only single days can be selected.
      /// </summary>
      Day,

      /// <summary>
      /// Indicates that a work week is selected.
      /// </summary>
      WorkWeek,

      /// <summary>
      /// Indicates that a full week is selected.
      /// </summary>
      FullWeek,

      /// <summary>
      /// Indicates that a full month is selected.
      /// </summary>
      Month,

      /// <summary>
      /// Indicates manual selection.
      /// </summary>
      Manual
   }
}