namespace CustomControls.Design
{
   using System;
   using System.ComponentModel;
   using System.Linq;
   using System.Windows.Forms;

   /// <summary>
   /// Class for displaying a flags enumeration in the designer.
   /// </summary>
   /// <remarks>From CodeProject : http://www.codeproject.com/KB/edit/flagenumeditor.aspx</remarks>
   [ToolboxItem(false)]
   internal class FlagCheckedListBox : CheckedListBox
   {
      #region fields

      /// <summary>
      /// Indicates whether currently updating the check states.
      /// </summary>
      private bool isUpdatingCheckStates;

      /// <summary>
      /// The type of the enum.
      /// </summary>
      private Type enumType;

      /// <summary>
      /// The enum value.
      /// </summary>
      private Enum enumValue;

      #endregion

      #region constructor

      /// <summary>
      /// Initializes a new instance of the <see cref="FlagCheckedListBox"/> class.
      /// </summary>
      public FlagCheckedListBox()
      {
         this.CheckOnClick = true;
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets or sets the enum value.
      /// </summary>
      [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
      public Enum EnumValue
      {
         get
         {
            object e = Enum.ToObject(this.enumType, this.GetCurrentValue());

            return (Enum)e;
         }

         set
         {
            Items.Clear();

            // Store the current enum value
            this.enumValue = value;

            // Store enum type
            this.enumType = value.GetType();

            // Add items for enum members
            this.FillEnumMembers();

            // Check/uncheck items depending on enum value
            this.ApplyEnumValue();
         }
      }

      #endregion

      #region methods

      /// <summary>
      /// Handles the raising of the <see cref="CheckedListBox.ItemCheck"/> event.
      /// </summary>
      /// <param name="e">An <see cref="ItemCheckEventArgs"/> instance that contains the event data.</param>
      protected override void OnItemCheck(ItemCheckEventArgs e)
      {
         base.OnItemCheck(e);

         if (this.isUpdatingCheckStates)
         {
            return;
         }

         // Get the checked/unchecked item
         FlagCheckedListBoxItem item = Items[e.Index] as FlagCheckedListBoxItem;

         // Update other items
         this.UpdateCheckedItems(item, e.NewValue);
      }

      /// <summary>
      /// Adds an integer value and its associated description.
      /// </summary>
      /// <param name="v">The value to add.</param>
      /// <param name="c">The description string to add.</param>
      /// <returns>The <see cref="FlagCheckedListBoxItem"/> item that was added.</returns>
      private void Add(int v, string c)
      {
         FlagCheckedListBoxItem item = new FlagCheckedListBoxItem(v, c);

         Items.Add(item);
      }

      /// <summary>
      /// Gets the current bit value corresponding to all checked items.
      /// </summary>
      /// <returns>The bit value.</returns>
      private int GetCurrentValue()
      {
         return (from object t in this.Items select t as FlagCheckedListBoxItem)
            .Where((item, i) => this.GetItemChecked(i) && item != null)
            .Aggregate(0, (current, item) => current | item.Value);
      }

      /// <summary>
      /// Checks or unchecks items depending on the given bit value.
      /// </summary>
      /// <param name="value">The bit value.</param>
      private void UpdateCheckedItems(int value)
      {
         this.isUpdatingCheckStates = true;

         // Iterate over all items
         for (int i = 0; i < Items.Count; i++)
         {
            FlagCheckedListBoxItem item = Items[i] as FlagCheckedListBoxItem;

            if (item == null)
            {
               continue;
            }

            if (item.Value == 0)
            {
               SetItemChecked(i, value == 0);
            }
            else
            {
               SetItemChecked(i, (item.Value & value) == item.Value && item.Value != 0);
            }
         }

         this.isUpdatingCheckStates = false;
      }

      /// <summary>
      /// Updates items in the checklistbox.
      /// </summary>
      /// <param name="composite">The item that was checked/unchecked.</param>
      /// <param name="cs">The check state of that item.</param>
      private void UpdateCheckedItems(FlagCheckedListBoxItem composite, CheckState cs)
      {
         if (composite == null)
         {
            return;
         }

         // If the value of the item is 0, call directly.
         if (composite.Value == 0)
         {
            this.UpdateCheckedItems(0);
         }

         // Get the total value of all checked items
         int sum = (from object t in this.Items select t as FlagCheckedListBoxItem)
            .Where((item, i) => item != null && this.GetItemChecked(i))
            .Aggregate(0, (current, item) => current | item.Value);

         // If the item has been unchecked, remove its bits from the sum
         if (cs == CheckState.Unchecked)
         {
            sum = sum & (~composite.Value);
         }
         else
         {
            // If the item has been checked, combine its bits with the sum
            sum |= composite.Value;
         }

         // Update all items in the checklistbox based on the final bit value
         this.UpdateCheckedItems(sum);
      }

      /// <summary>
      /// Adds items to the checklistbox based on the members of the enum.
      /// </summary>
      private void FillEnumMembers()
      {
         foreach (string name in Enum.GetNames(this.enumType))
         {
            object val = Enum.Parse(this.enumType, name);

            int intVal = (int)Convert.ChangeType(val, typeof(int));

            this.Add(intVal, name);
         }

         this.Height = this.GetItemHeight(0) * 8;
      }

      /// <summary>
      /// Checks/unchecks items based on the current value of the enum variable.
      /// </summary>
      private void ApplyEnumValue()
      {
         int intVal = (int)Convert.ChangeType(this.enumValue, typeof(int));

         this.UpdateCheckedItems(intVal);
      }

      #endregion

      #region nested classes

      /// <summary>
      /// Class that represents an item in the <see cref="FlagCheckedListBox"/>.
      /// </summary>
      /// <remarks>From CodeProject.</remarks>
      private class FlagCheckedListBoxItem
      {
         /// <summary>
         /// Initializes a new instance of the <see cref="FlagCheckedListBoxItem"/> class.
         /// </summary>
         /// <param name="value">The value of the item.</param>
         /// <param name="caption">The caption of the item.</param>
         public FlagCheckedListBoxItem(int value, string caption)
         {
            this.Value = value;
            this.Caption = caption;
         }

         /// <summary>
         /// Gets or sets the value of the item.
         /// </summary>
         public int Value { get; set; }

         /// <summary>
         /// Gets or sets the caption of the item.
         /// </summary>
         public string Caption { get; set; }

         /// <summary>
         /// Returns a string which represents the object.
         /// </summary>
         /// <returns>A string which represents the object.</returns>
         public override string ToString()
         {
            return this.Caption;
         }
      }

      #endregion
   }
}