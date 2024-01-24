namespace CustomControls
{
   using System;
   using System.Collections.Generic;
   using System.Globalization;
   using System.Linq;

   /// <summary>
   /// Class for calendar methods.
   /// </summary>
   internal static class DateMethods
   {
      /// <summary>
      /// Gets the days in month for the specified date.
      /// </summary>
      /// <param name="date">The <see cref="MonthCalendarDate"/> to get the days in month for.</param>
      /// <returns>The number of days in the month.</returns>
      public static int GetDaysInMonth(MonthCalendarDate date)
      {
         return GetDaysInMonth(date.Calendar, date.Year, date.Month);
      }

      /// <summary>
      /// Gets the days in month for the specified year and month using the specified <paramref name="cal"/>.
      /// </summary>
      /// <param name="cal">The <see cref="Calendar"/> to use.</param>
      /// <param name="year">The year value.</param>
      /// <param name="month">The month value.</param>
      /// <returns>The number of days in the month using the specified <see cref="Calendar"/>.</returns>
      public static int GetDaysInMonth(Calendar cal, int year, int month)
      {
         return (cal ?? CultureInfo.CurrentUICulture.Calendar).GetDaysInMonth(year, month);
      }

      /// <summary>
      /// Gets the week of the year for the specified date, culture and calendar.
      /// </summary>
      /// <param name="info">The <see cref="CultureInfo"/> to use.</param>
      /// <param name="cal">The <see cref="Calendar"/> to use.</param>
      /// <param name="date">The date value to get the week of year for.</param>
      /// <returns>The week of the year.</returns>
      public static int GetWeekOfYear(CultureInfo info, Calendar cal, DateTime date)
      {
         CultureInfo ci = info ?? CultureInfo.CurrentUICulture;
         Calendar c = cal ?? ci.Calendar;

         return c.GetWeekOfYear(date, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
      }

      /// <summary>
      /// Gets the abbreviated and day names, beginning with the first day provided by the <paramref name="provider"/>.
      /// </summary>
      /// <param name="provider">The <see cref="ICustomFormatProvider"/> to use.</param>
      /// <returns>The abbreviated and day names beginning with the first day.</returns>
      public static string[,] GetDayNames(ICustomFormatProvider provider)
      {
         List<string> abbDayNames = new List<string>(provider.AbbreviatedDayNames);
         List<string> shortestDayNames = new List<string>(provider.ShortestDayNames);
         List<string> dayNames = new List<string>(provider.DayNames);
         string firstDayName = provider.GetAbbreviatedDayName(provider.FirstDayOfWeek);

         int firstNameIndex = abbDayNames.IndexOf(firstDayName);

         string[,] names = new string[3, dayNames.Count];
         int j = 0;

         for (int i = firstNameIndex; i < abbDayNames.Count; i++, j++)
         {
            names[0, j] = dayNames[i];
            names[1, j] = abbDayNames[i];
            names[2, j] = shortestDayNames[i];
         }

         for (int i = 0; i < firstNameIndex; i++, j++)
         {
            names[0, j] = dayNames[i];
            names[1, j] = abbDayNames[i];
            names[2, j] = shortestDayNames[i];
         }

         return names;
      }

      /// <summary>
      /// Gets the abbreviated or day names, beginning with the first day of the week specified by the <paramref name="provider"/>.
      /// </summary>
      /// <param name="provider">The provider to use.</param>
      /// <param name="index">The index for the kind of name : 0 means full, 1 means abbreviated and 2 means shortest day names.</param>
      /// <returns>The (abbreviated) day names.</returns>
      public static string[] GetDayNames(ICustomFormatProvider provider, int index)
      {
         if (index < 0 || index > 2)
         {
            index = 0;
         }

         string[,] dayNames = GetDayNames(provider);

         string[] names = new string[dayNames.GetLength(1)];

         for (int i = 0; i < names.Length; i++)
         {
            names[i] = dayNames[index, i];
         }

         return names;
      }

      /// <summary>
      /// Gets from the specified <see cref="CalendarDayOfWeek"/> a generic list
      /// with the work days.
      /// </summary>
      /// <param name="nonWorkDays">The non working days.</param>
      /// <returns>A list with the working days.</returns>
      public static List<DayOfWeek> GetWorkDays(CalendarDayOfWeek nonWorkDays)
      {
         List<DayOfWeek> workDays = new List<DayOfWeek>();

         for (int i = 0; i < 7; i++)
         {
            workDays.Add((DayOfWeek)i);
         }

         GetSysDaysOfWeek(nonWorkDays).ForEach(day =>
            {
               if (workDays.Contains(day))
               {
                  workDays.Remove(day);
               }
            });

         return workDays;
      }

      /// <summary>
      /// Gets a value indicating whether the specified culture is a right to left culture.
      /// </summary>
      /// <param name="info">The culture.</param>
      /// <returns>true, if RTL, false otherwise.</returns>
      public static bool IsRTLCulture(CultureInfo info)
      {
         return info.TextInfo.IsRightToLeft;
      }

      /// <summary>
      /// Gets a value indicating whether the current UI culture is a right to left culture.
      /// </summary>
      /// <returns>true, if RTL, false otherwise.</returns>
      public static bool IsRTLCulture()
      {
         return CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
      }

      /// <summary>
      /// Gets a <see cref="string"/> representing the <paramref name="number"/>.
      /// If <paramref name="nativeDigits"/> is not <c>null</c> and valid the numbers in <paramref name="number"/>
      /// are converted using the native digits.
      /// </summary>
      /// <param name="number">The number to convert.</param>
      /// <param name="nativeDigits">The native digits, can be <c>null</c>.</param>
      /// <param name="prefixZero">if set to <c>true</c> prefixes the string with a zero.</param>
      /// <returns>A <see cref="string"/> representing the <paramref name="number"/> either in arabic numerals or if specified using the native digits.</returns>
      public static string GetNumberString(int number, string[] nativeDigits, bool prefixZero)
      {
         if (nativeDigits == null || nativeDigits.Length != 10)
         {
            return prefixZero ? number.ToString("##00") : number.ToString(CultureInfo.CurrentUICulture);
         }

         return GetNativeNumberString(number, nativeDigits, prefixZero);
      }

      /// <summary>
      /// Gets a string where each number in <paramref name="number"/> is replaced with the
      /// native number representation as specified by <paramref name="nativeDigits"/>.
      /// </summary>
      /// <param name="number">The number to convert.</param>
      /// <param name="nativeDigits">The native digits.</param>
      /// <param name="prefixZero">if set to <c>true</c> and the <paramref name="number"/> is greater 0 and less than 10, prefixes the string with the native zero.</param>
      /// <returns>
      /// A <see cref="string"/> representing the <paramref name="number"/> using the native digits specified by <paramref name="nativeDigits"/>.
      /// </returns>
      public static string GetNativeNumberString(int number, string[] nativeDigits, bool prefixZero)
      {
         if (nativeDigits == null || nativeDigits.Length != 10)
         {
            return String.Empty;
         }

         if (number > -1 && number < 10)
         {
            return prefixZero ? nativeDigits[0] + nativeDigits[number] : nativeDigits[number];
         }

         var list = new List<int>();

         while (number != 0)
         {
            list.Insert(0, number % 10);
            number = number / 10;
         }

         return list.ConvertAll(i => nativeDigits[i]).Aggregate((s1, s2) => s1 + s2);
      }

      /// <summary>
      /// Gets a generic list of <see cref="DayOfWeek"/> value from the
      /// specified <see cref="CalendarDayOfWeek"/> value.
      /// </summary>
      /// <param name="days">The <see cref="CalendarDayOfWeek"/> value.</param>
      /// <returns>A generic list of type <see cref="DayOfWeek"/>.</returns>
      private static List<DayOfWeek> GetSysDaysOfWeek(CalendarDayOfWeek days)
      {
         List<DayOfWeek> list = new List<DayOfWeek>();

         if ((days & CalendarDayOfWeek.Monday) == CalendarDayOfWeek.Monday)
         {
            list.Add(DayOfWeek.Monday);
         }

         if ((days & CalendarDayOfWeek.Tuesday) == CalendarDayOfWeek.Tuesday)
         {
            list.Add(DayOfWeek.Tuesday);
         }

         if ((days & CalendarDayOfWeek.Wednesday) == CalendarDayOfWeek.Wednesday)
         {
            list.Add(DayOfWeek.Wednesday);
         }

         if ((days & CalendarDayOfWeek.Thursday) == CalendarDayOfWeek.Thursday)
         {
            list.Add(DayOfWeek.Thursday);
         }

         if ((days & CalendarDayOfWeek.Friday) == CalendarDayOfWeek.Friday)
         {
            list.Add(DayOfWeek.Friday);
         }

         if ((days & CalendarDayOfWeek.Saturday) == CalendarDayOfWeek.Saturday)
         {
            list.Add(DayOfWeek.Saturday);
         }

         if ((days & CalendarDayOfWeek.Sunday) == CalendarDayOfWeek.Sunday)
         {
            list.Add(DayOfWeek.Sunday);
         }

         return list;
      }
   }
}