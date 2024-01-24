namespace CustomControls.Design
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Drawing.Design;
   using System.Linq;
   using System.Windows.Forms;
   using System.Windows.Forms.Design;

   /// <summary>
   /// UI editor for the property <see cref="CustomControls.MonthCalendar.BoldedDatesCollection"/>.
   /// </summary>
   internal class MonthCalendarBoldedDatesUIEditor : UITypeEditor
   {
      #region fields

      /// <summary>
      /// The internal list box holding the optional usable calendar types.
      /// </summary>
      private ListBox calendarListBox;

      /// <summary>
      /// The editor service.
      /// </summary>
      private IWindowsFormsEditorService editorSvc;

      #endregion

      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarBoldedDatesUIEditor"/> class.
      /// </summary>
      public MonthCalendarBoldedDatesUIEditor()
      {
         this.calendarListBox = new ListBox
            {
               BorderStyle = BorderStyle.None,
               SelectionMode = SelectionMode.One
            };

         this.calendarListBox.Click += this.ListBoxClick;
      }

      #endregion

      #region methods

      /// <summary>
      /// Gets the editor style used by the <see cref="UITypeEditor"/><c>.EditValue(<see cref="IServiceProvider"/>, <see cref="Object"/>)</c> method.
      /// </summary>
      /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
      /// <returns>The <see cref="UITypeEditorEditStyle"/><c>.DropDown</c>.</returns>
      public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
      {
         return UITypeEditorEditStyle.DropDown;
      }

      /// <summary>
      /// Edits the specified object's value using the editor style indicated by the
      /// <see cref="UITypeEditor"/><c>.GetEditStyle()</c> method.
      /// </summary>
      /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain
      /// additional context information.</param>
      /// <param name="provider">An <see cref="IServiceProvider"/> that this editor can use to obtain services.</param>
      /// <param name="value">The object to edit.</param>
      /// <returns>The new value of the object. If the value of the object has not changed,
      /// this should return the same object it was passed.</returns>
      public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
      {
         this.editorSvc = null;

         if (context != null && context.Instance != null && provider != null && value is BoldedDate)
         {
            var boldedDate = (BoldedDate)value;

            this.editorSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (this.editorSvc != null)
            {
               CustomControls.MonthCalendar cal = context.Instance.GetType() == typeof(CustomControls.MonthCalendar)
                                                     ? (CustomControls.MonthCalendar)context.Instance
                                                     : ((DatePicker)context.Instance).Picker;

               if (cal != null && cal.BoldedDateTypeCollection.Count != 0)
               {
                  this.calendarListBox.Items.Clear();

                  this.calendarListBox.Items.AddRange(cal.BoldedDateTypeCollection.Cast<object>().ToArray());

                  if (!boldedDate.Type.IsEmpty)
                  {
                     this.calendarListBox.SelectedItem = boldedDate.Type;
                  }

                  this.editorSvc.DropDownControl(this.calendarListBox);

                  if (this.calendarListBox.SelectedItem != null)
                  {
                     return ((BoldedDate)this.calendarListBox.SelectedItem).Type;
                  }

                  return cal.BoldedDateTypeCollection[0];
               }
            }
         }

         return null;
      }

      /// <summary>
      /// Handles the <see cref="ListBox.Click"/> event.
      /// </summary>
      /// <param name="sender">The sending listbox.</param>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      private void ListBoxClick(object sender, EventArgs e)
      {
         if (this.editorSvc != null)
         {
            this.editorSvc.CloseDropDown();
         }
      }

      #endregion
   }
}