namespace CustomControls
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Drawing;
   using System.Drawing.Drawing2D;
   using System.Globalization;
   using System.Windows.Forms;

   /// <summary>
   /// A custom date time picker control.
   /// </summary>
   [Designer(typeof(Design.DatePickerControlDesigner))]
   [ToolboxBitmap(typeof(DateTimePicker))]
   [DefaultEvent("ValueChanged")]
   public class DatePicker : Control
   {
      #region fields

      /// <summary>
      /// The combobox button color information.
      /// </summary>
      private static readonly ButtonColors[] ColorArray;

      /// <summary>
      /// the tool strip control host which hosts the <see cref="MonthCalendar"/> control.
      /// </summary>
      private readonly ToolStripControlHost monthCalendarHost;

      /// <summary>
      /// The month calendar control which is displayed in the toolstrip drop down.
      /// </summary>
      private readonly MonthCalendar monthCalendar;

      /// <summary>
      /// The date text box.
      /// </summary>
      private readonly DatePickerDateTextBox dateTextBox;

      /// <summary>
      /// The toolstrip drop down which is displayed when pressing the combobox button.
      /// </summary>
      private CustomToolStripDropDown dropDown;

      /// <summary>
      /// The border color of the control.
      /// </summary>
      private Color borderColor;

      /// <summary>
      /// The border color in focused state.
      /// </summary>
      private Color focusedBorderColor;

      /// <summary>
      /// The bounds of the button.
      /// </summary>
      private Rectangle buttonBounds;

      /// <summary>
      /// The start combobox button state.
      /// </summary>
      private ComboButtonState buttonState;

      /// <summary>
      /// Indicates whether the toolstrip drop down is visible.
      /// </summary>
      private bool isDropped;

      /// <summary>
      /// Indicates whether to close the toolstrip drop down control.
      /// </summary>
      private bool cancelClosing;

      /// <summary>
      /// Indicates whether the control is focused.
      /// </summary>
      private bool isFocused;

      #endregion

      #region constructors

      /// <summary>
      /// Initializes static members of the <see cref="DatePicker"/> class.
      /// </summary>
      static DatePicker()
      {
         ColorArray = new[]
         {
            new ButtonColors
            {
               TL = Color.FromArgb(209, 224, 253),
               BL = Color.FromArgb(171, 193, 244),
               BB = Color.FromArgb(183, 198, 241),
               BR = Color.FromArgb(176, 197, 242),
               TRR = Color.FromArgb(188, 204, 243),
               TR = Color.FromArgb(175, 197, 244),
               BS = Color.FromArgb(225, 234, 254),
               BE = Color.FromArgb(174, 200, 247)
            },
            new ButtonColors
            {
               TL = Color.FromArgb(180, 199, 235),
               BL = Color.FromArgb(135, 160, 222),
               BB = Color.FromArgb(147, 167, 223),
               BR = Color.FromArgb(140, 167, 222),
               TRR = Color.FromArgb(155, 175, 224),
               TR = Color.FromArgb(138, 166, 227),
               BS = Color.FromArgb(253, 255, 255),
               BE = Color.FromArgb(185, 218, 251)
            },
            new ButtonColors
            {
               TL = Color.FromArgb(162, 172, 220),
               BL = Color.FromArgb(115, 129, 217),
               BB = Color.FromArgb(185, 201, 243),
               BR = Color.FromArgb(176, 179, 242),
               TRR = Color.FromArgb(188, 204, 243),
               TR = Color.FromArgb(119, 134, 217),
               BS = Color.FromArgb(110, 142, 241),
               BE = Color.FromArgb(210, 222, 235)
            }
         };
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="DatePicker"/> class.
      /// </summary>
      public DatePicker()
      {
         this.SetStyle(
            ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.UserPaint
            | ControlStyles.ResizeRedraw
            | ControlStyles.Selectable,
            true);

         this.borderColor = Color.FromArgb(127, 157, 185);
         this.focusedBorderColor = Color.Blue;

         this.Width = 101;
         this.Height = 22;
         this.AllowPromptAsInput = false;

         this.monthCalendar = new MonthCalendar
         {
            SelectionMode = MonthCalendarSelectionMode.Day
         };

         this.monthCalendar.KeyPress += this.MonthCalendarKeyPress;

         this.dateTextBox = new DatePickerDateTextBox(this)
         {
            Font = this.Font,
            Location = new Point(2, 2),
            Date = DateTime.Today,
            Width = this.Width - 19,
            Height = 18,
            MinDate = this.monthCalendar.MinDate,
            MaxDate = this.monthCalendar.MaxDate
         };

         this.dateTextBox.CheckDate += this.DateTextBoxCheckDate;
         this.dateTextBox.GotFocus += this.FocusChanged;
         this.dateTextBox.LostFocus += this.FocusChanged;

         this.Controls.Add(this.dateTextBox);

         this.monthCalendar.DateSelected += this.MonthCalendarDateSelected;
         this.monthCalendar.ActiveDateChanged += this.MonthCalendarActiveDateChanged;
         this.monthCalendar.DateClicked += this.MonthCalendarDateClicked;
         this.monthCalendar.InternalDateSelected += this.MonthCalendarInternalDateSelected;
         this.monthCalendar.GotFocus += this.FocusChanged;
         this.monthCalendar.LostFocus += this.FocusChanged;

         this.monthCalendarHost = new ToolStripControlHost(this.monthCalendar);
         this.monthCalendarHost.LostFocus += this.MonthCalendarHostLostFocus;
         this.monthCalendarHost.GotFocus += this.FocusChanged;

         this.dropDown = new CustomToolStripDropDown
          {
             DropShadowEnabled = true
          };

         this.dropDown.Closing += this.DropDownClosing;
         this.dropDown.GotFocus += this.FocusChanged;
         this.dropDown.LostFocus += this.FocusChanged;

         this.dropDown.Items.Add(this.monthCalendarHost);

         this.monthCalendar.MonthMenu.OwnerItem = this.monthCalendarHost;
         this.monthCalendar.YearMenu.OwnerItem = this.monthCalendarHost;

         this.monthCalendar.MonthMenu.ItemClicked += this.MenuItemClicked;
         this.monthCalendar.YearMenu.ItemClicked += this.MenuItemClicked;

         this.BackColor = SystemColors.Window;
         this.ClosePickerOnDayClick = true;
      }

      #endregion

      #region events

      /// <summary>
      /// Raised when a date was selected.
      /// </summary>
      [Category("Action")]
      [Description("Is Raised when the date value changed.")]
      public event EventHandler<CheckDateEventArgs> ValueChanged;

      /// <summary>
      /// Is raised when the mouse is over an date.
      /// </summary>
      [Category("Action")]
      [Description("Is raised when the mouse is over an date.")]
      public event EventHandler<ActiveDateChangedEventArgs> ActiveDateChanged;

      #endregion

      #region enums

      /// <summary>
      /// Enum to enumerate the combo button states.
      /// </summary>
      private enum ComboButtonState
      {
         /// <summary>
         /// Indicates a normal combo button state.
         /// </summary>
         Normal = 0,

         /// <summary>
         /// Indicates a hot combo button state.
         /// </summary>
         Hot,

         /// <summary>
         /// Indicates a pressed combo button state.
         /// </summary>
         Pressed,

         /// <summary>
         /// Indicates an invalid combo button state.
         /// </summary>
         None
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets or sets the border color.
      /// </summary>
      [Description("Sets the border color.")]
      [DefaultValue(typeof(Color), "127,157,185")]
      [Category("Appearance")]
      public Color BorderColor
      {
         get
         {
            return this.borderColor;
         }

         set
         {
            if (value == this.borderColor || value.IsEmpty)
            {
               return;
            }

            this.borderColor = value;

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets or sets the selected date value.
      /// </summary>
      [Description("The currently selected date.")]
      [Category("Behavior")]
      public DateTime Value
      {
         get
         {
            return this.monthCalendar.SelectionRange.Start;
         }

         set
         {
            if (this.monthCalendar.SelectionStart == value || value < this.MinDate || value > this.MaxDate)
            {
               return;
            }

            this.monthCalendar.SelectionStart = value;
            this.dateTextBox.Date = value;

            this.monthCalendar.EnsureSeletedDateIsVisible();
         }
      }

      /// <summary>
      /// Gets or sets the minimum selectable date.
      /// </summary>
      [Description("The minimum selectable date.")]
      [Category("Behavior")]
      public DateTime MinDate
      {
         get
         {
            return this.monthCalendar.MinDate;
         }

         set
         {
            this.monthCalendar.MinDate = value;
            this.dateTextBox.MinDate = this.monthCalendar.MinDate;
         }
      }

      /// <summary>
      /// Gets or sets the maximum selectable date.
      /// </summary>
      [Description("The maximum selectable date.")]
      [Category("Behavior")]
      public DateTime MaxDate
      {
         get
         {
            return this.monthCalendar.MaxDate;
         }

         set
         {
            this.monthCalendar.MaxDate = value;
            this.dateTextBox.MaxDate = this.monthCalendar.MaxDate;
         }
      }

      /// <summary>
      /// Gets or sets the background color for invalid dates in the text field portion of the control.
      /// </summary>
      [Category("Appearance")]
      [Description("The backcolor for invalid dates in the text portion.")]
      [DefaultValue(typeof(Color), "Red")]
      public Color InvalidBackColor
      {
         get { return this.dateTextBox.InvalidBackColor; }
         set { this.dateTextBox.InvalidBackColor = value; }
      }

      /// <summary>
      /// Gets or sets the text color for invalid dates in the text field portion of the control.
      /// </summary>
      [Category("Appearance")]
      [Description("The text color for invalid dates in the text portion.")]
      public Color InvalidForeColor
      {
         get { return this.dateTextBox.InvalidForeColor; }
         set { this.dateTextBox.InvalidForeColor = value; }
      }

      /// <summary>
      /// Gets or sets the border color in focused state.
      /// </summary>
      [Category("Appearance")]
      [Description("The border color if the control is focused.")]
      [DefaultValue(typeof(Color), "Blue")]
      public Color FocusedBorderColor
      {
         get
         {
            return this.focusedBorderColor;
         }

         set
         {
            if (value == this.focusedBorderColor || value.IsEmpty)
            {
               return;
            }

            this.focusedBorderColor = value;

            if (this.Focused)
            {
               this.Invalidate();
            }
         }
      }

      /// <summary>
      /// Gets or sets the color table for the picker part.
      /// </summary>
      [Category("Appearance")]
      [Description("The color table for the picker part.")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      public MonthCalendarColorTable PickerColorTable
      {
         get
         {
            return this.monthCalendar.ColorTable;
         }

         set
         {
            this.monthCalendar.ColorTable = value;
         }
      }

      /// <summary>
      /// Gets or sets the font for the days in the picker.
      /// </summary>
      [Category("Appearance")]
      [Description("The font for the days in the picker.")]
      public Font PickerDayFont
      {
         get { return this.monthCalendar.Font; }
         set { this.monthCalendar.Font = value; }
      }

      /// <summary>
      /// Gets or sets the picker header font.
      /// </summary>
      [Category("Appearance")]
      [Description("The font for the picker header.")]
      public Font PickerHeaderFont
      {
         get
         {
            return this.monthCalendar.HeaderFont;
         }

         set
         {
            this.monthCalendar.HeaderFont = value;
         }
      }

      /// <summary>
      /// Gets or sets the picker footer font.
      /// </summary>
      [Category("Appearance")]
      [Description("The font for the picker footer.")]
      public Font PickerFooterFont
      {
         get { return this.monthCalendar.FooterFont; }
         set { this.monthCalendar.FooterFont = value; }
      }

      /// <summary>
      /// Gets or sets the font for the picker day header.
      /// </summary>
      [Category("Appearance")]
      [Description("The font for the picker day header.")]
      public Font PickerDayHeaderFont
      {
         get { return this.monthCalendar.DayHeaderFont; }
         set { this.monthCalendar.DayHeaderFont = value; }
      }

      /// <summary>
      /// Gets or sets the text alignment for the days in the picker.
      /// </summary>
      [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
      [Description("Determines the text alignment for the days in the picker.")]
      [Category("Appearance")]
      public ContentAlignment PickerDayTextAlignment
      {
         get { return this.monthCalendar.DayTextAlignment; }
         set { this.monthCalendar.DayTextAlignment = value; }
      }

      /// <summary>
      /// Gets or sets the list for bolded dates in the picker.
      /// </summary>
      [Description("The bolded dates in the picker.")]
      public List<DateTime> PickerBoldedDates
      {
         get { return this.monthCalendar.BoldedDates; }
         set { this.monthCalendar.BoldedDates = value; }
      }

      /// <summary>
      /// Gets the bolded dates.
      /// </summary>
      [Description("The bolded dates in the calendar.")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public BoldedDatesCollection BoldedDatesCollection
      {
         get
         {
            return this.monthCalendar.BoldedDatesCollection;
         }
      }

      /// <summary>
      /// Gets a collection holding the defined categories of bold dates.
      /// </summary>
      [Description("The bold date categories in the calendar.")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public BoldedDateCategoryCollection BoldedDateCategoryCollection
      {
         get
         {
            return this.monthCalendar.BoldedDateCategoryCollection;
         }
      }

      /// <summary>
      /// Gets or sets the culture used by the <see cref="DatePicker"/>.
      /// </summary>
      [Category("Behavior")]
      [Description("The culture used by the DatePicker.")]
      [TypeConverter(typeof(Design.CultureInfoCustomTypeConverter))]
      public CultureInfo Culture
      {
         get
         {
            return this.monthCalendar.Culture;
         }

         set
         {
            if (value == null || value.IsNeutralCulture)
            {
               return;
            }

            this.monthCalendar.Culture = value;
            this.MinDate = this.monthCalendar.MinDate;
            this.MaxDate = this.monthCalendar.MaxDate;

            this.RightToLeft = this.monthCalendar.UseRTL ? RightToLeft.Yes : RightToLeft.Inherit;

            this.Invalidate();
         }
      }

      /// <summary>
      /// Gets or sets the used calendar.
      /// </summary>
      [Category("Behavior")]
      [Description("The calendar used by the MonthCalendar.")]
      [Editor(typeof(Design.MonthCalendarCalendarUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
      [TypeConverter(typeof(Design.MonthCalendarCalendarTypeConverter))]
      public Calendar CultureCalendar
      {
         get
         {
            return this.monthCalendar.CultureCalendar;
         }

         set
         {
            this.monthCalendar.CultureCalendar = value;
            this.MinDate = this.monthCalendar.MinDate;
            this.MaxDate = this.monthCalendar.MaxDate;

            this.Invalidate();
         }
      }

      /// <summary>
      /// Gets or sets the interface for day name handling.
      /// </summary>
      [TypeConverter(typeof(Design.MonthCalendarNamesProviderTypeConverter))]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      [Category("Behavior")]
      [Description("Culture dependent settings for month/day names and date formatting.")]
      public ICustomFormatProvider FormatProvider
      {
         get { return this.monthCalendar.FormatProvider; }
         set { this.monthCalendar.FormatProvider = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to show the week header in the picker.
      /// </summary>
      [Category("Appearance")]
      [Description("Show the week header in the picker.")]
      [DefaultValue(true)]
      public bool ShowPickerWeekHeader
      {
         get { return this.monthCalendar.ShowWeekHeader; }
         set { this.monthCalendar.ShowWeekHeader = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to close the picker when clicking a day.
      /// </summary>
      [Category("Behavior")]
      [Description("Whether to close the picker on clicking a day or not (regardless whether the day is already selected).")]
      [DefaultValue(true)]
      public bool ClosePickerOnDayClick { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether to use the shortest day names.
      /// </summary>
      [DefaultValue(false)]
      [Category("Appearance")]
      [Description("Indicates whether to use the shortest or the abbreviated day names in the day header of the picker.")]
      public bool UseShortestDayNames
      {
         get { return this.monthCalendar.UseShortestDayNames; }
         set { this.monthCalendar.UseShortestDayNames = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to use the native digits in <see cref="NumberFormatInfo.NativeDigits"/>
      /// specified by <see cref="MonthCalendar.Culture"/>s <see cref="CultureInfo.NumberFormat"/>
      /// for number display.
      /// </summary>
      [DefaultValue(false)]
      [Category("Appearance")]
      [Description("Indicates whether to use the native digits as specified by the current Culture property.")]
      public bool UseNativeDigits
      {
         get { return this.monthCalendar.UseNativeDigits; }
         set { this.monthCalendar.UseNativeDigits = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to allow the input to be the current date separator.
      /// After editing is finished, tries to parse the input as specified by the ShortDatePattern.
      /// </summary>
      [DefaultValue(false)]
      [Category("Behavior")]
      [Description("Allows the input to be the current date separator and tries to parse the date after the editing of the date finished.")]
      public bool AllowPromptAsInput { get; set; }

      /// <summary>
      /// Gets or sets the picker dimensions.
      /// </summary>
      [Category("Appearance")]
      [Description("The picker dimension.")]
      [DefaultValue(typeof(Size), "1,1")]
      public Size PickerDimension
      {
         get { return this.monthCalendar.CalendarDimensions; }
         set { this.monthCalendar.CalendarDimensions = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the control has input focus.
      /// </summary>
      public override bool Focused
      {
         get { return base.Focused || this.dateTextBox.Focused || this.monthCalendar.Focused || this.monthCalendarHost.Focused || this.dropDown.Focused; }
      }

      /// <summary>
      /// Gets or sets the background color for the control.
      /// </summary>
      public sealed override Color BackColor
      {
         get
         {
            return base.BackColor;
         }

         set
         {
            base.BackColor = value;
            this.dateTextBox.BackColor = value;
         }
      }

      /// <summary>
      /// Gets or sets the used font.
      /// </summary>
      public sealed override Font Font
      {
         get { return base.Font; }
         set { base.Font = value; }
      }

      /// <summary>
      /// Gets the picker calendar.
      /// </summary>
      internal MonthCalendar Picker
      {
         get { return this.monthCalendar; }
      }

      #endregion

      #region methods

      #region internal methods

      /// <summary>
      /// Shows or closes the picker according to the current picker state.
      /// </summary>
      internal void SwitchPickerState()
      {
         if (this.isDropped)
         {
            this.buttonState = ComboButtonState.Hot;
            this.isDropped = false;
            this.dropDown.Close(ToolStripDropDownCloseReason.CloseCalled);

            this.Focus();
         }
         else
         {
            if (this.buttonState == ComboButtonState.Pressed)
            {
               this.buttonState = ComboButtonState.Hot;
            }
            else if (this.buttonState == ComboButtonState.None)
            {
               this.buttonState = ComboButtonState.Hot;
            }
            else
            {
               this.buttonState = ComboButtonState.Pressed;

               this.Refresh();

               this.ShowDropDown();
            }
         }
      }

      #endregion

      #region protected methods

      /// <summary>
      /// Disposes the control.
      /// </summary>
      /// <param name="disposing">true for releasing both managed and unmanaged resources, false for releasing only managed resources.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (this.dropDown != null)
            {
               this.dropDown.Closing -= this.DropDownClosing;
               this.dropDown.GotFocus -= this.FocusChanged;
               this.dropDown.LostFocus -= this.FocusChanged;
               this.dropDown.Dispose();
               this.dropDown = null;
            }

            this.monthCalendar.DateSelected -= this.MonthCalendarDateSelected;
            this.monthCalendar.InternalDateSelected -= this.MonthCalendarInternalDateSelected;
            this.monthCalendar.ActiveDateChanged -= this.MonthCalendarActiveDateChanged;
            this.monthCalendar.DateClicked -= this.MonthCalendarDateClicked;
            this.monthCalendar.GotFocus -= this.FocusChanged;
            this.monthCalendar.LostFocus -= this.FocusChanged;
            this.monthCalendar.MonthMenu.ItemClicked -= this.MenuItemClicked;
            this.monthCalendar.YearMenu.ItemClicked -= this.MenuItemClicked;
            this.monthCalendarHost.LostFocus -= this.MonthCalendarHostLostFocus;
            this.monthCalendarHost.GotFocus -= this.FocusChanged;

            this.monthCalendar.Dispose();

            this.monthCalendarHost.Dispose();

            this.dateTextBox.CheckDate -= this.DateTextBoxCheckDate;
            this.dateTextBox.GotFocus -= this.FocusChanged;
            this.dateTextBox.LostFocus -= this.FocusChanged;

            this.dateTextBox.Dispose();
         }

         base.Dispose(disposing);
      }

      /// <summary>
      /// Sets the bounds of the control
      /// </summary>
      /// <param name="x">The x coordinate.</param>
      /// <param name="y">The y coordinate.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <param name="specified">true, if bounds where specified.</param>
      protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
      {
         if (width < 19)
         {
            width = 19;
         }

         height = this.MeasureControlSize();

         if (this.dateTextBox != null)
         {
            this.dateTextBox.Size = new Size(Math.Max(width - 20, 0), height - 4);
         }

         base.SetBoundsCore(x, y, width, height, specified);
      }

      /// <summary>
      /// Processes a dialog key.
      /// </summary>
      /// <param name="keyData">One of the <see cref="Keys"/> value that represents the key to process.</param>
      /// <returns>true if the key was processed by the control; otherwise, false.</returns>
      protected override bool ProcessDialogKey(Keys keyData)
      {
         if (keyData == Keys.Space && !this.dateTextBox.InEditMode)
         {
            this.SwitchPickerState();

            return true;
         }

         return base.ProcessDialogKey(keyData);
      }

      /// <summary>
      /// Paints the control.
      /// </summary>
      /// <param name="e">The event args.</param>
      protected override void OnPaint(PaintEventArgs e)
      {
         Rectangle rect = this.ClientRectangle;
         rect.Width--;
         rect.Height--;

         e.Graphics.Clear(this.Enabled ? this.BackColor : SystemColors.Window);

         using (Pen p = new Pen(this.Focused ? this.focusedBorderColor : MonthCalendarAbstractRenderer.GetGrayColor(this.Enabled, this.borderColor)))
         {
            e.Graphics.DrawRectangle(p, rect);
         }

         Rectangle btnRect = rect;
         btnRect.X = rect.Right - 16;
         btnRect.Width = 14;
         btnRect.Y += 2;
         btnRect.Height = rect.Height - 4;

         this.buttonBounds = btnRect;

         DrawButton(e.Graphics, btnRect, this.buttonState, this.Enabled);
      }

      /// <summary>
      /// Raises the mouse enter event.
      /// </summary>
      /// <param name="e">The event args.</param>
      protected override void OnMouseEnter(EventArgs e)
      {
         if (!this.isDropped)
         {
            this.buttonState = ComboButtonState.Hot;
            this.Refresh();
         }

         base.OnMouseEnter(e);
      }

      /// <summary>
      /// Raises the mouse leave event.
      /// </summary>
      /// <param name="e">The event args.</param>
      protected override void OnMouseLeave(EventArgs e)
      {
         if (!this.isDropped)
         {
            this.buttonState = ComboButtonState.Normal;

            this.Invalidate();
         }

         base.OnMouseLeave(e);
      }

      /// <summary>
      /// Raises the mouse down event.
      /// </summary>
      /// <param name="e">The event args.</param>
      protected override void OnMouseDown(MouseEventArgs e)
      {
         this.Focus();

         if (e.Button == MouseButtons.Left && this.buttonBounds.Contains(e.Location))
         {
            this.SwitchPickerState();

            this.Refresh();
         }

         base.OnMouseDown(e);
      }

      /// <summary>
      /// Raises the <see cref="Control.MouseMove"/> event.
      /// </summary>
      /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
      protected override void OnMouseMove(MouseEventArgs e)
      {
         base.OnMouseMove(e);

         if (e.Button == MouseButtons.None && !this.isDropped)
         {
            ComboButtonState st = this.buttonState;

            this.buttonState = this.buttonBounds.Contains(e.Location) ?
               ComboButtonState.Hot : ComboButtonState.Normal;

            if (st != this.buttonState)
            {
               this.Invalidate();
            }
         }
      }

      /// <summary>
      /// Raises the <see cref="Control.LostFocus"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnLostFocus(EventArgs e)
      {
         if (!this.isDropped)
         {
            this.buttonState = ComboButtonState.Normal;

            this.Invalidate();
         }

         if (!this.Focused)
         {
            base.OnLostFocus(e);
         }
      }

      /// <summary>
      /// Raises the <see cref="Control.GotFocus"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnGotFocus(EventArgs e)
      {
         base.OnGotFocus(e);

         this.Invalidate();
      }

      /// <summary>
      /// Raises the <see cref="Control.FontChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnFontChanged(EventArgs e)
      {
         base.OnFontChanged(e);

         this.dateTextBox.Font = this.Font;

         this.Height = Math.Max(22, this.MeasureControlSize());
      }

      /// <summary>
      /// Raises the <see cref="Control.ForeColorChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnForeColorChanged(EventArgs e)
      {
         if (this.dateTextBox != null)
         {
            this.dateTextBox.ForeColor = this.ForeColor;
         }

         base.OnForeColorChanged(e);
      }

      /// <summary>
      /// Raises the <see cref="Control.EnabledChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnEnabledChanged(EventArgs e)
      {
         base.OnEnabledChanged(e);

         this.Invalidate();
      }

      /// <summary>
      /// Raises the <see cref="Control.RightToLeftChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnRightToLeftChanged(EventArgs e)
      {
         base.OnRightToLeftChanged(e);

         this.dateTextBox.RightToLeft = this.RightToLeft;
         this.dateTextBox.Refresh();
      }

      /// <summary>
      /// Raises the <see cref="ValueChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected virtual void OnValueChanged(CheckDateEventArgs e)
      {
         if (this.ValueChanged != null)
         {
            this.ValueChanged(this, e);
         }
      }

      /// <summary>
      /// Raises the <see cref="ActiveDateChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="ActiveDateChangedEventArgs"/> that contains the event data.</param>
      protected virtual void OnActiveDateChanged(ActiveDateChangedEventArgs e)
      {
         if (this.ActiveDateChanged != null)
         {
            this.ActiveDateChanged(this, e);
         }
      }

      #endregion

      #region private methods

      /// <summary>
      /// Draws the combobox button.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="rect">The <see cref="Rectangle"/> in which to draw.</param>
      /// <param name="buttonState">The <see cref="ComboButtonState"/>.</param>
      /// <param name="enabled">true if the button should be drawn enabled; false otherwise.</param>
      private static void DrawButton(Graphics g, Rectangle rect, ComboButtonState buttonState, bool enabled)
      {
         if (g == null || rect.IsEmpty || rect.Width != 14)
         {
            return;
         }

         ButtonColors btnColors = ColorArray[0];

         switch (buttonState)
         {
            case ComboButtonState.Hot:
               {
                  btnColors = ColorArray[1];

                  break;
               }

            case ComboButtonState.Pressed:
               {
                  btnColors = ColorArray[2];

                  break;
               }
         }

         using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.TL),
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.BL),
            LinearGradientMode.Vertical))
         {
            using (Pen p = new Pen(brush))
            {
               g.DrawLine(p, rect.X, rect.Y, rect.X, rect.Bottom);
            }
         }

         using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.BB),
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.BR),
            LinearGradientMode.Horizontal))
         {
            using (Pen p = new Pen(brush))
            {
               g.DrawLine(p, rect.X + 1, rect.Bottom, rect.Right, rect.Bottom);
            }
         }

         using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.TRR),
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.BR),
            LinearGradientMode.Vertical))
         {
            using (Pen p = new Pen(brush))
            {
               g.DrawLine(p, rect.Right, rect.Y, rect.Right, rect.Bottom);
            }
         }

         using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.TL),
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.TR),
            LinearGradientMode.Horizontal))
         {
            using (Pen p = new Pen(brush))
            {
               g.DrawLine(p, rect.X, rect.Y, rect.Right - 1, rect.Y);
            }
         }

         Rectangle r = rect;
         r.X++;
         r.Y++;
         r.Width--;
         r.Height--;

         using (LinearGradientBrush brush = new LinearGradientBrush(
            r,
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.BS),
            MonthCalendarAbstractRenderer.GetGrayColor(enabled, btnColors.BE),
            45f))
         {
            g.FillRectangle(brush, r);
         }

         r.X += 2;
         r.Y += (rect.Height / 2) - 3;
         r.Width = 9;
         r.Height = 6;

         using (Bitmap arrow = MonthCalendarControl.Properties.MonthCalendarResources.ComboArrowDown)
         {
            if (enabled)
            {
               g.DrawImage(arrow, r);
            }
            else
            {
               ControlPaint.DrawImageDisabled(g, arrow, r.X, r.Y, Color.Transparent);
            }
         }
      }

      /// <summary>
      /// Is raised when the toolstrip drop down is closing.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="ToolStripDropDownClosingEventArgs"/> instance which holds the event data.</param>
      private void DropDownClosing(object sender, ToolStripDropDownClosingEventArgs e)
      {
         if (this.cancelClosing)
         {
            this.cancelClosing = false;

            e.Cancel = true;
         }
         else
         {
            if (e.CloseReason == ToolStripDropDownCloseReason.CloseCalled)
            {
               this.buttonState = ComboButtonState.Hot;

               this.Invalidate();
            }
            else
            {
               this.isDropped = false;
            }
         }
      }

      /// <summary>
      /// Handles the <see cref="ToolStrip.ItemClicked"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="ToolStripItemClickedEventArgs"/> that contains the event data.</param>
      private void MenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
      {
         this.cancelClosing = true;
      }

      /// <summary>
      /// Handles the <see cref="ToolStripControlHost.LostFocus"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      private void MonthCalendarHostLostFocus(object sender, EventArgs e)
      {
         if (this.isDropped)
         {
            this.buttonState = ComboButtonState.None;
            this.dropDown.Close(ToolStripDropDownCloseReason.AppFocusChange);
         }

         this.FocusChanged(this, EventArgs.Empty);
      }

      /// <summary>
      /// Handles the <see cref="MonthCalendar.DateSelected"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="DateRangeEventArgs"/> that contains the event data.</param>
      private void MonthCalendarDateSelected(object sender, DateRangeEventArgs e)
      {
         this.buttonState = ComboButtonState.Normal;
         this.dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
         this.dateTextBox.Date = e.Start;
      }

      /// <summary>
      /// Handles the <see cref="MonthCalendar.InternalDateSelected"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="DateEventArgs"/> that contains the event data.</param>
      private void MonthCalendarInternalDateSelected(object sender, DateEventArgs e)
      {
         this.dateTextBox.Date = e.Date;
      }

      /// <summary>
      /// Handles the <see cref="MonthCalendar.ActiveDateChanged"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="ActiveDateChangedEventArgs"/> that contains the event data.</param>
      private void MonthCalendarActiveDateChanged(object sender, ActiveDateChangedEventArgs e)
      {
         this.OnActiveDateChanged(e);
      }

      /// <summary>
      /// Handles the <see cref="MonthCalendar.DateClicked"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="DateEventArgs"/> that contains the event data.</param>
      private void MonthCalendarDateClicked(object sender, DateEventArgs e)
      {
         if (this.ClosePickerOnDayClick)
         {
            this.buttonState = ComboButtonState.Normal;
            this.dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
         }
      }

      /// <summary>
      /// Handles the <see cref="DatePickerDateTextBox.CheckDate"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="CheckDateEventArgs"/> that contains the event data.</param>
      private void DateTextBoxCheckDate(object sender, CheckDateEventArgs e)
      {
         this.monthCalendar.SelectionRange = new SelectionRange(e.Date, e.Date);

         this.monthCalendar.EnsureSeletedDateIsVisible();

         CheckDateEventArgs newArgs = new CheckDateEventArgs(e.Date, this.IsValidDate(e.Date));

         this.OnValueChanged(newArgs);

         e.IsValid = newArgs.IsValid;
      }

      /// <summary>
      /// Handles the <see cref="Control.KeyPress"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="KeyPressEventArgs"/> that contains the event data.</param>
      private void MonthCalendarKeyPress(object sender, KeyPressEventArgs e)
      {
         if (e.KeyChar == (char)Keys.Space)
         {
            this.SwitchPickerState();

            e.Handled = true;
         }
      }

      /// <summary>
      /// Handles the <see cref="Control.GotFocus"/> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      private void FocusChanged(object sender, EventArgs e)
      {
         if (this.isFocused != this.Focused)
         {
            this.isFocused = this.Focused;

            this.Invalidate();
         }
      }

      /// <summary>
      /// Shows the toolstrip drop down.
      /// </summary>
      private void ShowDropDown()
      {
         if (this.dropDown != null)
         {
            this.isDropped = true;
            this.monthCalendar.EnsureSeletedDateIsVisible();

            int x = 0, y = this.Height;

            if (this.RightToLeft == RightToLeft.Yes)
            {
               x = this.monthCalendar.Size.Width + Math.Abs(this.monthCalendar.Size.Width - this.Width);
            }

            this.dropDown.Show(this, x, y);
            this.monthCalendar.Focus();
         }
      }

      /// <summary>
      /// Checks if the specified date is valid in the current context.
      /// </summary>
      /// <param name="date">The <see cref="DateTime"/> value to determine.</param>
      /// <returns>true if it is a valid date; otherwise false.</returns>
      private bool IsValidDate(DateTime date)
      {
         return date >= this.MinDate && date <= this.MaxDate;
      }

      /// <summary>
      /// Measures the height of the control.
      /// </summary>
      /// <returns>The height in pixel.</returns>
      private int MeasureControlSize()
      {
         using (Graphics g = this.CreateGraphics())
         {
            return this.MeasureControlSize(g);
         }
      }

      /// <summary>
      /// Measures the height of the control with the specified <paramref name="g"/> object.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object to measure with.</param>
      /// <returns>The height of the control in pixel.</returns>
      private int MeasureControlSize(Graphics g)
      {
         if (g == null)
         {
            return 22;
         }

         return Size.Round(g.MeasureString(DateTime.Today.ToShortDateString(), this.Font)).Height + 8;
      }

      #region design time methods

      /// <summary>
      /// Returns whether the property <see cref="Culture"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeCulture()
      {
         return this.Picker.ShouldSerializeCulture();
      }

      /// <summary>
      /// Resets the property <see cref="Culture"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetCulture()
      {
         this.Picker.ResetCulture();
      }

      /// <summary>
      /// Returns whether the property <see cref="CultureCalendar"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeCultureCalendar()
      {
         return this.Picker.ShouldSerializeCultureCalendar();
      }

      /// <summary>
      /// Resets the property <see cref="CultureCalendar"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetCultureCalendar()
      {
         this.Picker.ResetCultureCalendar();
      }

      /// <summary>
      /// Returns whether the property <see cref="MinDate"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeMinDate()
      {
         return this.Picker.ShouldSerializeMinDate();
      }

      /// <summary>
      /// Resets the property <see cref="MinDate"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetMinDate()
      {
         this.Picker.ResetMinDate();
         this.dateTextBox.MinDate = this.monthCalendar.MinDate;
      }

      /// <summary>
      /// Returns whether the property <see cref="MaxDate"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeMaxDate()
      {
         return this.Picker.ShouldSerializeMaxDate();
      }

      /// <summary>
      /// Resets the property <see cref="MaxDate"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetMaxDate()
      {
         this.Picker.ResetMaxDate();
         this.dateTextBox.MaxDate = this.monthCalendar.MaxDate;
      }

      /// <summary>
      /// Determines if the property <see cref="Value"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeValue()
      {
         return this.Value != DateTime.Today;
      }

      /// <summary>
      /// Resets the property <see cref="Value"/> to the default value.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetValue()
      {
         this.Value = DateTime.Today;
      }

      /// <summary>
      /// Determines if the property <see cref="InvalidForeColor"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeInvalidForeColor()
      {
         return this.InvalidForeColor != this.ForeColor;
      }

      /// <summary>
      /// Resets the property <see cref="InvalidForeColor"/> to the default value.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetInvalidForeColor()
      {
         this.InvalidForeColor = this.ForeColor;
      }

      /// <summary>
      /// Determines if the <see cref="PickerColorTable"/> property should be serialized.
      /// </summary>
      /// <returns>true if it should be serialized; false otherwise.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializePickerColorTable()
      {
         return this.monthCalendar.ShouldSerializeColorTable();
      }

      /// <summary>
      /// Resets the property <see cref="PickerColorTable"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetPickerColorTable()
      {
         this.monthCalendar.ResetColorTable();

         this.Invalidate();
      }

      /// <summary>
      /// Determines whether the <see cref="BackColor"/> property should be serialized.
      /// </summary>
      /// <returns>true if it should be serialized; false otherwise.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeBackColor()
      {
         return this.BackColor != SystemColors.Window;
      }

      /// <summary>
      /// Resets the <see cref="BackColor"/> property to the default value.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private new void ResetBackColor()
      {
         this.BackColor = SystemColors.Window;
      }

      /// <summary>
      /// Returns whether the property <see cref="PickerHeaderFont"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializePickerHeaderFont()
      {
         return this.PickerHeaderFont.Name != "Segoe UI"
            || !this.PickerHeaderFont.Size.Equals(9f)
            || this.PickerHeaderFont.Style != FontStyle.Regular;
      }

      /// <summary>
      /// Resets the property <see cref="PickerHeaderFont"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetPickerHeaderFont()
      {
         this.PickerHeaderFont.Dispose();
         this.PickerHeaderFont = new Font("Segoe UI", 9f, FontStyle.Regular);
      }

      /// <summary>
      /// Returns whether the property <see cref="PickerFooterFont"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializePickerFooterFont()
      {
         return this.PickerFooterFont.Name != "Arial"
            || !this.PickerFooterFont.Size.Equals(9f)
            || this.PickerFooterFont.Style != FontStyle.Bold;
      }

      /// <summary>
      /// Resets the property <see cref="PickerFooterFont"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetPickerFooterFont()
      {
         this.PickerFooterFont.Dispose();
         this.PickerFooterFont = new Font("Arial", 9f, FontStyle.Bold);
      }

      /// <summary>
      /// Returns whether the property <see cref="PickerDayHeaderFont"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializePickerDayHeaderFont()
      {
         return this.PickerDayHeaderFont.Name != "Segoe UI"
            || !this.PickerDayHeaderFont.Size.Equals(8f)
            || this.PickerDayHeaderFont.Style != FontStyle.Regular;
      }

      /// <summary>
      /// Resets the property <see cref="PickerDayHeaderFont"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetPickerDayHeaderFont()
      {
         this.PickerDayHeaderFont.Dispose();
         this.PickerDayHeaderFont = new Font("Segoe UI", 8f, FontStyle.Regular);
      }

      /// <summary>
      /// Returns whether the property <see cref="PickerDayFont"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializePickerDayFont()
      {
         return this.PickerDayFont.Name != "Microsoft Sans Serif"
            || !this.PickerDayFont.Size.Equals(8.25f)
            || this.PickerDayFont.Style != FontStyle.Regular;
      }

      /// <summary>
      /// Resets the property <see cref="PickerDayFont"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetPickerDayFont()
      {
         this.PickerDayFont.Dispose();
         this.PickerDayFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
      }

      /// <summary>
      /// Returns whether the property <see cref="PickerBoldedDates"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializePickerBoldedDates()
      {
         return this.PickerBoldedDates.Count != 0;
      }

      /// <summary>
      /// Resets the property <see cref="PickerBoldedDates"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetPickerBoldedDates()
      {
         this.PickerBoldedDates = null;

         if (this.isDropped)
         {
            this.monthCalendar.Refresh();
         }
      }

      #endregion

      #endregion

      #endregion

      #region structs

      /// <summary>
      /// Struct which holds color information about the displayed colors.
      /// </summary>
      private struct ButtonColors
      {
         /// <summary>
         /// Gets or sets the top left color.
         /// </summary>
         public Color TL { get; set; }

         /// <summary>
         /// Gets or sets the bottom left color.
         /// </summary>
         public Color BL { get; set; }

         /// <summary>
         /// Gets or sets the bottom bottom color.
         /// </summary>
         public Color BB { get; set; }

         /// <summary>
         /// Gets or sets the bottom right color.
         /// </summary>
         public Color BR { get; set; }

         /// <summary>
         /// Gets or sets the top right right color.
         /// </summary>
         public Color TRR { get; set; }

         /// <summary>
         /// Gets or sets the top right color.
         /// </summary>
         public Color TR { get; set; }

         /// <summary>
         /// Gets or sets the background start color.
         /// </summary>
         public Color BS { get; set; }

         /// <summary>
         /// Gets or sets the background end color.
         /// </summary>
         public Color BE { get; set; }
      }

      #endregion

      #region custom ToolStripDropDown class

      /// <summary>
      /// Custom <see cref="ToolStripDropDown"/> class to remove the padding.
      /// </summary>
      private class CustomToolStripDropDown : ToolStripDropDown
      {
         /// <summary>
         /// Gets the default padding.
         /// </summary>
         protected override Padding DefaultPadding
         {
            get { return new Padding(0, 0, 0, -2); }
         }
      }

      #endregion
   }
}