namespace CustomControls.Design
{
   using System;
   using System.ComponentModel;

   /// <summary>
   /// The type converter class for <see cref="MonthCalendarFormatProvider"/>.
   /// </summary>
   internal class MonthCalendarNamesProviderTypeConverter : ExpandableObjectConverter
   {
      /// <summary>
      /// Gets a value indicating whether this converter can convert to the specified <paramref name="destinationType"/>.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="destinationType">The destination type.</param>
      /// <returns>true if this converter can convert to the specified <paramref name="destinationType"/>, false otherwise.</returns>
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
         if (destinationType == typeof(string))
         {
            return true;
         }

         return base.CanConvertTo(context, destinationType);
      }

      /// <summary>
      /// Converts the specified value in the given context and culture to the specified <paramref name="destinationType"/>.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="culture">The culture.</param>
      /// <param name="value">The object to convert.</param>
      /// <param name="destinationType">The destination type.</param>
      /// <returns>An object of the specified <paramref name="destinationType"/> if successful, otherwise null.</returns>
      public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
      {
         if (destinationType == typeof(string))
         {
            return "";
         }

         return base.ConvertTo(context, culture, value, destinationType);
      }
   }
}