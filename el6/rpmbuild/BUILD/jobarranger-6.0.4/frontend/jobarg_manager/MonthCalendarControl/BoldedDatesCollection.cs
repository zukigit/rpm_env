namespace CustomControls
{
   using System.Collections.Generic;
   using System.Linq;

   /// <summary>
   /// The list for bolded dates.
   /// </summary>
   public class BoldedDatesCollection : List<BoldedDate>
   {
      /// <summary>
      /// Adds the specified date.
      /// </summary>
      /// <param name="date">The date.</param>
      public new void Add(BoldedDate date)
      {
         if (!this.CanAddItem(date))
         {
            return;
         }

         base.Add(date);
      }

      /// <summary>
      /// Adds the range.
      /// </summary>
      /// <param name="dates">The dates.</param>
      public new void AddRange(IEnumerable<BoldedDate> dates)
      {
         if (dates == null)
         {
            return;
         }

         dates.ToList().ForEach(this.Add);
      }

      /// <summary>
      /// Inserts the specified item at the specified index.
      /// </summary>
      /// <param name="index">The index.</param>
      /// <param name="item">The item.</param>
      public new void Insert(int index, BoldedDate item)
      {
         if (!this.CanAddItem(item))
         {
            return;
         }

         base.Insert(index, item);
      }

      /// <summary>
      /// Inserts the specified items at the specified index to the collection.
      /// </summary>
      /// <param name="index">The index.</param>
      /// <param name="items">The items.</param>
      public new void InsertRange(int index, IEnumerable<BoldedDate> items)
      {
         if (items == null)
         {
            return;
         }

         var list = items.ToList();

         if (list.Any(d => !this.CanAddItem(d)))
         {
            return;
         }

         base.InsertRange(index, list);
      }

      private bool CanAddItem(BoldedDate date)
      {
         return !date.Category.IsEmpty && !this.Exists(d => d.Value.Date == date.Value.Date);
      }
   }
}