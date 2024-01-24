namespace CustomControls
{
   using System;
   using System.Collections.Generic;
   using System.Linq;

   /// <summary>
   /// The list for bolded dates.
   /// </summary>
   public class BoldedDateCategoryCollection : List<BoldedDateCategory>
   {
      private readonly MonthCalendar parent;

      /// <summary>
      /// Initializes a new instance of the <see cref="BoldedDateCategoryCollection"/> class.
      /// </summary>
      /// <param name="parent">The parent.</param>
      public BoldedDateCategoryCollection(MonthCalendar parent)
      {
         this.parent = parent;
      }

      /// <summary>
      /// Adds the specified Category to the collection.
      /// </summary>
      /// <param name="category">The Category to add.</param>
      public new void Add(BoldedDateCategory category)
      {
         if (!this.CanAddItem(category))
         {
            return;
         }

         base.Add(category);
      }

      /// <summary>
      /// Adds the specified items to the collection.
      /// </summary>
      /// <param name="types">The types to add.</param>
      public new void AddRange(IEnumerable<BoldedDateCategory> types)
      {
         if (types == null)
         {
            return;
         }

         types.ToList().ForEach(this.Add);
      }

      /// <summary>
      /// Removes the specified item.
      /// </summary>
      /// <param name="item">The item.</param>
      /// <returns></returns>
      public new bool Remove(BoldedDateCategory item)
      {
         if (base.Remove(item))
         {
            this.parent.BoldedDatesCollection.RemoveAll(d => d.Category.Equals(item));

            return true;
         }

         return false;
      }

      /// <summary>
      /// Removes the item at the specified index.
      /// </summary>
      /// <param name="index">The index.</param>
      public new void RemoveAt(int index)
      {
         if (index < 0 || index >= this.Count)
         {
            return;
         }

         this.Remove(this[index]);
      }

      /// <summary>
      /// Removes all matching items.
      /// </summary>
      /// <param name="match">The match predicate.</param>
      /// <returns>The number of removed items.</returns>
      public new int RemoveAll(Predicate<BoldedDateCategory> match)
      {
         if (match == null)
         {
            return 0;
         }

         var matches = this.FindAll(match);

         if (matches.Count != 0)
         {
            matches.ForEach(t => this.Remove(t));
         }

         return matches.Count;
      }

      /// <summary>
      /// Removes a range of elements from this list.
      /// </summary>
      /// <param name="index">The index.</param>
      /// <param name="count">The count of items.</param>
      public new void RemoveRange(int index, int count)
      {
         if (index < 0 || index + count > this.Count)
         {
            return;
         }

         var items = this.GetRange(index, count);

         items.ForEach(t => this.Remove(t));
      }

      /// <summary>
      /// Clears this instance.
      /// </summary>
      public new void Clear()
      {
         this.parent.BoldedDatesCollection.Clear();
         base.Clear();
      }

      /// <summary>
      /// Inserts the specified item at the specified index.
      /// </summary>
      /// <param name="index">The index.</param>
      /// <param name="item">The item.</param>
      public new void Insert(int index, BoldedDateCategory item)
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
      public new void InsertRange(int index, IEnumerable<BoldedDateCategory> items)
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

      private bool CanAddItem(BoldedDateCategory category)
      {
         return !category.IsEmpty && !this.Exists(t => string.Compare(category.Name, t.Name, StringComparison.OrdinalIgnoreCase) == 0);
      }
   }
}