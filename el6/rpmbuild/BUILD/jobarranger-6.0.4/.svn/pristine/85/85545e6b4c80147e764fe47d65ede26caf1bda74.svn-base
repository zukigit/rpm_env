namespace CustomControls
{
   using System;

   /// <summary>
   /// Struct to store a bolded date.
   /// </summary>
   public struct BoldedDate
   {
      #region properties

      /// <summary>
      /// Gets or sets the category of the bolded date.
      /// </summary>
      public BoldedDateCategory Category { get; set; }

      /// <summary>
      /// Gets or sets the <see cref="DateTime"/> value.
      /// </summary>
      public DateTime Value { get; set; }

      /// <summary>
      /// Gets a value indicating whether this instance is empty/invalid.
      /// </summary>
      public bool IsEmpty
      {
         get { return this.Category.IsEmpty || Value == DateTime.MinValue; }
      }

      #endregion
   }
}