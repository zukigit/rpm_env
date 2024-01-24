namespace CustomControls.Design
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Drawing.Design;
   using System.Globalization;
   using System.Windows.Forms;
   using System.Windows.Forms.Design;

   /// <summary>
   /// UI editor for the property <see cref="CustomControls.MonthCalendar.CultureCalendar"/> to choose between
   /// optional calendars of the culture specified by <see cref="CustomControls.MonthCalendar.Culture"/>.
   /// </summary>
   internal class MonthCalendarCalendarUIEditor : UITypeEditor
   {
      #region fields

      /// <summary>
      /// The internal list box holding the optional usable calendar types.
      /// </summary>
      private readonly ListBox calendarListBox;

      /// <summary>
      /// The editor service.
      /// </summary>
      private IWindowsFormsEditorService editorSvc;

      #endregion

      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarCalendarUIEditor"/> class.
      /// </summary>
      public MonthCalendarCalendarUIEditor()
      {
         this.calendarListBox = new ListBox
         {
            BorderStyle = BorderStyle.None,
            SelectionMode = SelectionMode.One
         };

         this.calendarListBox.Click += ListBoxClick;
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

         if (context != null && context.Instance != null && provider != null)
         {
            this.editorSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (this.editorSvc != null)
            {
               CustomControls.MonthCalendar cal = context.Instance.GetType() == typeof(CustomControls.MonthCalendar)
                                      ? (CustomControls.MonthCalendar)context.Instance
                                      : ((DatePicker)context.Instance).Picker;

               if (cal != null)
               {
                  CalendarItem currentItem = null;

                  this.calendarListBox.Items.Clear();

                  bool addPersian = true, addHebrew = true;

                  foreach (Calendar c in cal.Culture.OptionalCalendars)
                  {
                     CalendarItem it = new CalendarItem(c);

                     if (c.GetType() == typeof(PersianCalendar))
                     {
                        addPersian = false;
                     }

                     if (c.GetType() == typeof(HebrewCalendar))
                     {
                        addHebrew = false;
                     }

                     this.calendarListBox.Items.Add(it);

                     if (c == cal.CultureCalendar)
                     {
                        currentItem = it;
                     }
                  }

                  if (currentItem != null)
                  {
                     this.calendarListBox.SelectedItem = currentItem;
                  }

                  List<CalendarItem> items = new List<CalendarItem>();

                  if (addPersian)
                  {
                     items.Add(new CalendarItem(new PersianCalendar(), false));
                  }

                  if (addHebrew)
                  {
                     items.Add(new CalendarItem(new HebrewCalendar(), false));
                  }

                  items.Add(new CalendarItem(new JulianCalendar(), false));
                  items.Add(new CalendarItem(new ChineseLunisolarCalendar(), false));
                  items.Add(new CalendarItem(new JapaneseLunisolarCalendar(), false));
                  items.Add(new CalendarItem(new KoreanLunisolarCalendar(), false));
                  items.Add(new CalendarItem(new TaiwanLunisolarCalendar(), false));

                  foreach (var item in items)
                  {
                     this.calendarListBox.Items.Add(item);

                     if (item.Item.GetType() == cal.CultureCalendar.GetType())
                     {
                        this.calendarListBox.SelectedItem = item;
                     }
                  }

                  this.editorSvc.DropDownControl(this.calendarListBox);

                  if (this.calendarListBox.SelectedItem != null)
                  {
                     return ((CalendarItem)this.calendarListBox.SelectedItem).Item;
                  }

                  return cal.Culture.DateTimeFormat.Calendar;
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

      #region nested classes

      /// <summary>
      /// Class to hold a <see cref="Calendar"/> and overridden ToString method.
      /// </summary>
      private class CalendarItem
      {
         #region constructors

         /// <summary>
         /// Initializes a new instance of the <see cref="CalendarItem"/> class.
         /// </summary>
         /// <param name="cal">The calendar.</param>
         /// <param name="isCultureCalendar">Indicates whether the specified calendar is an optional calendar for the culture.</param>
         public CalendarItem(Calendar cal, bool isCultureCalendar = true)
         {
            this.IsCultureCalendar = isCultureCalendar;
            this.Item = cal;
         }

         #endregion

         /// <summary>
         /// Gets or sets the calendar.
         /// </summary>
         public Calendar Item { get; private set; }

         /// <summary>
         /// Gets or sets a value indicating whether the calendar is an optional calendar for the culture.
         /// </summary>
         public bool IsCultureCalendar { get; set; }

         /// <summary>
         /// Returns a string representation of the object.
         /// </summary>
         /// <returns>The string representing the object.</returns>
         public override string ToString()
         {
            if (this.Item != null)
            {
               string addString = string.Empty;

               if (this.Item.GetType() == typeof(GregorianCalendar))
               {
                  addString = " " + ((GregorianCalendar)this.Item).CalendarType;
               }

               if (!this.IsCultureCalendar)
               {
                  addString += " not optional";
               }

               return this.Item.ToString().Replace("System.Globalization.", string.Empty).Replace("Calendar", string.Empty) + addString;
            }

            return string.Empty;
         }
      }

      #endregion
   }
}