namespace CustomControls
{
   using System;

   /// <summary>
   /// Class for storing information about era date ranges.
   /// </summary>
   internal class MonthCalendarEraRange
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarEraRange"/> class.
      /// </summary>
      /// <param name="era">The era.</param>
      /// <param name="minDate">The min date of the era specified by <paramref name="era"/>.</param>
      public MonthCalendarEraRange(int era, DateTime minDate)
      {
         this.Era = era;
         this.MinDate = minDate;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarEraRange"/> class.
      /// </summary>
      /// <param name="era">The era value.</param>
      /// <param name="minDate">The lower bound of the era specified by <paramref name="era"/>.</param>
      /// <param name="maxDate">The upper bound of the era specified by <paramref name="era"/>.</param>
      public MonthCalendarEraRange(int era, DateTime minDate, DateTime maxDate)
         : this(era, minDate)
      {
         this.MaxDate = maxDate;
      }

      /// <summary>
      /// Gets or sets the era.
      /// </summary>
      public int Era { get; set; }

      /// <summary>
      /// Gets or sets the min date of the <see cref="Era"/>.
      /// </summary>
      public DateTime MinDate { get; set; }

      /// <summary>
      /// Gets or sets the max date of the <see cref="Era"/>.
      /// </summary>
      public DateTime MaxDate { get; set; }
   }
}