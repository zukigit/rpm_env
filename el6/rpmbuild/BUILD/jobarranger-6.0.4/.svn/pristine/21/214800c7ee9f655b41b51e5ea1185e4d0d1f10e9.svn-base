namespace CustomControls
{
   using System;
   using System.Globalization;

   /// <summary>
   /// Interface to provide culture dependent day and month names.
   /// </summary>
   public interface ICustomFormatProvider
   {
      #region properties

      /// <summary>
      /// Gets or sets the used <see cref="DateTimeFormatInfo"/> object.
      /// </summary>
      DateTimeFormatInfo DateTimeFormat { get; set; }

      /// <summary>
      /// Gets or sets the used <see cref="System.Globalization.Calendar"/> object.
      /// </summary>
      Calendar Calendar { get; set; }

      /// <summary>
      /// Gets or sets an array of type <see cref="string"/> containing the day names.
      /// </summary>
      string[] DayNames { get; set; }

      /// <summary>
      /// Gets or sets an array of type <see cref="string"/> containing the abbreviated day names.
      /// </summary>
      string[] AbbreviatedDayNames { get; set; }

      /// <summary>
      /// Gets or sets an array of type <see cref="string"/> containing the shortest day names.
      /// </summary>
      string[] ShortestDayNames { get; set; }

      /// <summary>
      /// Gets or sets the first day of the week.
      /// </summary>
      DayOfWeek FirstDayOfWeek { get; set; }

      /// <summary>
      /// Gets or sets the month names.
      /// </summary>
      string[] MonthNames { get; set; }

      /// <summary>
      /// Gets or sets the abbreviated month names.
      /// </summary>
      string[] AbbreviatedMonthNames { get; set; }

      /// <summary>
      /// Gets or sets the string that separates the day components, that is the year, month and day.
      /// </summary>
      string DateSeparator { get; set; }

      /// <summary>
      /// Gets or sets the short date pattern.
      /// </summary>
      string ShortDatePattern { get; set; }

      /// <summary>
      /// Gets or sets the long date pattern.
      /// </summary>
      string LongDatePattern { get; set; }

      /// <summary>
      /// Gets or sets the month day pattern.
      /// </summary>
      string MonthDayPattern { get; set; }

      /// <summary>
      /// Gets or sets the year month pattern.
      /// </summary>
      string YearMonthPattern { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether the provider belongs to an RTL language.
      /// </summary>
      bool IsRTLLanguage { get; set; }

      #endregion

      #region methods

      /// <summary>
      /// Returns the day name of the specified <see cref="DayOfWeek"/> value.
      /// </summary>
      /// <param name="dayofweek">The <see cref="DayOfWeek"/> value to get the day name for.</param>
      /// <returns>A <see cref="string"/> value representing the day name specified by <paramref name="dayofweek"/>.</returns>
      string GetDayName(DayOfWeek dayofweek);

      /// <summary>
      /// Returns the abbreviated day name of the specified <see cref="DayOfWeek"/> value.
      /// </summary>
      /// <param name="dayofweek">The <see cref="DayOfWeek"/> value to get the abbreviated day name for.</param>
      /// <returns>A <see cref="string"/> value representing the abbreviated day name specified by <paramref name="dayofweek"/>.</returns>
      string GetAbbreviatedDayName(DayOfWeek dayofweek);

      /// <summary>
      /// Returns the shortest day name of the specified <see cref="DayOfWeek"/> value.
      /// </summary>
      /// <param name="dayofweek">The <see cref="DayOfWeek"/> value to get the shortest day name for.</param>
      /// <returns>A <see cref="string"/> value representing the shortest day name specified by <paramref name="dayofweek"/>.</returns>
      string GetShortestDayName(DayOfWeek dayofweek);

      /// <summary>
      /// Gets the month name as string for the specified month.
      /// </summary>
      /// <param name="year">The year for which to retrieve the month name.</param>
      /// <param name="month">The month to get the name for.</param>
      /// <returns>The string value of the month.</returns>
      string GetMonthName(int year, int month);

      /// <summary>
      /// Gets the abbreviated month name as string for the specified month.
      /// </summary>
      /// <param name="year">The year for which to retrieve the abbreviated month name.</param>
      /// <param name="month">The month to get the name for.</param>
      /// <returns>A string representing the abbreviated month name.</returns>
      string GetAbbreviatedMonthName(int year, int month);

      /// <summary>
      /// Gets the number of months for the specified year.
      /// </summary>
      /// <param name="year">The year to get the number of months for.</param>
      /// <returns>The number of months is the year.</returns>
      int GetMonthsInYear(int year);

      /// <summary>
      /// Returns the string representation of the specified era.
      /// </summary>
      /// <param name="era">The era as <see cref="int"/> value to get the name for.</param>
      /// <returns>The era name.</returns>
      string GetEraName(int era);

      #endregion
   }
}