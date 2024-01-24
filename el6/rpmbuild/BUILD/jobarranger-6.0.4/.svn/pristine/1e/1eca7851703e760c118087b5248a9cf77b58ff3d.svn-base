namespace CustomControls.Design
{
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Globalization;

   /// <summary>
   /// Custom <see cref="CultureInfo"/> converter that only gives non-neutral cultures back.
   /// </summary>
   internal class CultureInfoCustomTypeConverter : CultureInfoConverter
   {
      /// <summary>
      /// The non-neutral cultures.
      /// </summary>
      private StandardValuesCollection values;

      /// <summary>
      /// Gets a collection of standard values for a <see cref="CultureInfo"/> object using the specified context.
      /// </summary>
      /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
      /// <returns>A <see cref="TypeConverter.StandardValuesCollection"/> containing a standard set of valid values,
      /// or null if the data type does not support a standard set of values.</returns>
      public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
         base.GetStandardValues(context);

         if (this.values == null)
         {
            List<CultureInfo> list = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));

            list.RemoveAll(c => c.IsNeutralCulture);

            list.Sort((c1, c2) =>
            {
               if (c1 == null)
               {
                  return c2 == null ? 0 : -1;
               }

               if (c2 == null)
               {
                  return 1;
               }

               return CultureInfo.CurrentCulture.CompareInfo.Compare(c1.DisplayName, c2.DisplayName, CompareOptions.StringSort);
            });

            this.values = new StandardValuesCollection(list);
         }

         return this.values;
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
      {
         var retValue = base.ConvertTo(context, culture, value, destinationType);

         //if (destinationType == typeof(string) && value is CultureInfo)
         //{
         //   var ci = (CultureInfo)value;

         //   var name = ci.DisplayName;

         //   if (string.IsNullOrEmpty(name))
         //   {
         //      name = ci.Name;
         //   }

         //   return name;
         //}

         return retValue;
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
      {
         return base.ConvertFrom(context, culture, value);
      }
   }
}