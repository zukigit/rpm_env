namespace CustomControls.Design
{
   using System;
   using System.ComponentModel;
   using System.Globalization;

   /// <summary>
   /// Custom type converter for the <see cref="MonthCalendar.CultureCalendar"/> property.
   /// </summary>
   internal class MonthCalendarCalendarTypeConverter : TypeConverter
   {
      /// <summary>
      /// Returns whether this converter can convert the object to the specified type, using the specified context.
      /// </summary>
      /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
      /// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
      /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
         if (destinationType == typeof(string))
         {
            return true;
         }

         return base.CanConvertTo(context, destinationType);
      }

      /// <summary>
      /// Converts the given value object to the specified type, using the specified context and culture information.
      /// </summary>
      /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
      /// <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.</param>
      /// <param name="value">The <see cref="Object"/> to convert.</param>
      /// <param name="destinationType">The <see cref="Type"/> to convert the value parameter to.</param>
      /// <returns>An <see cref="Object"/> that represents the converted value.</returns>
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
         if (destinationType == typeof(string) && value != null && value is Calendar)
         {
            string addString = string.Empty;

            Calendar cal = (Calendar)value;

            if (cal.GetType() == typeof(GregorianCalendar))
            {
               addString = " " + ((GregorianCalendar)cal).CalendarType;
            }

            return cal.ToString().Replace("System.Globalization.", "").Replace("Calendar", "") + addString;
         }

         return base.ConvertTo(context, culture, value, destinationType);
      }
   }
}