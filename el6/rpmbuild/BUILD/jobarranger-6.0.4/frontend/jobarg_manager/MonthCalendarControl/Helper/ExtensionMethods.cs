namespace CustomControls
{
   using System;

   /// <summary>
   /// Class that holds date extension methods.
   /// </summary>
   internal static class ExtensionMethods
   {
      /// <summary>
      /// Determines if the specified <paramref name="date"/> is contained within the <paramref name="range"/>.
      /// </summary>
      /// <param name="range">The <see cref="System.Windows.Forms.SelectionRange"/>.</param>
      /// <param name="date">The <see cref="DateTime"/> to determine if it is contained in the <paramref name="range"/>.</param>
      /// <returns>true if <paramref name="date"/> is contained within <paramref name="range"/>; otherwise false.</returns>
      public static bool Contains(this System.Windows.Forms.SelectionRange range, DateTime date)
      {
         date = date.Date;

         if (range.Start == DateTime.MinValue)
         {
            return date == range.End;
         }

         if (range.End == DateTime.MinValue)
         {
            return date == range.Start;
         }

         return date >= range.Start && date <= range.End;
      }
   }
}