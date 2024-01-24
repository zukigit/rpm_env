namespace CustomControls
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Drawing;
   using System.Globalization;
   using System.Linq;
   using System.Windows.Forms;

   /// <summary>
   /// A highly customizable and culture dependent month calendar control.
   /// </summary>
   [Designer(typeof(Design.MonthCalendarControlDesigner))]
   [DefaultEvent("DateSelected")]
   [DefaultProperty("ViewStart")]
   [ToolboxBitmap(typeof(System.Windows.Forms.MonthCalendar))]
    public class MonthCalendar : ScrollableControl
   {
      #region fields

      /// <summary>
      /// Redraw const.
      /// </summary>
      private const int SETREDRAW = 11;

      /// <summary>
      /// The list containing the bolded dates and their Category.
      /// </summary>
      private readonly BoldedDatesCollection boldDatesCollection;

      /// <summary>
      /// The list containing the bolded date types.
      /// </summary>
      private readonly BoldedDateCategoryCollection boldDateCategoryCollection;

      /// <summary>
      /// Use right to left layout.
      /// </summary>
      private bool rightToLeftLayout;

      /// <summary>
      /// Shows the footer.
      /// </summary>
      private bool showFooter;

      /// <summary>
      /// Selection started state.
      /// </summary>
      private bool selectionStarted;

      /// <summary>
      /// Indicates that an menu is currently displayed.
      /// </summary>
      private bool showingMenu;

      /// <summary>
      /// Indicates whether to show the week header.
      /// </summary>
      private bool showWeekHeader;

      /// <summary>
      /// Indicates whether the control is calculating it's sizes.
      /// </summary>
      private bool inUpdate;

      /// <summary>
      /// Indicates to use the shortest day names in the day header.
      /// </summary>
      private bool useShortestDayNames;

      /// <summary>
      /// Indicates whether to use the native digits in <see cref="NumberFormatInfo.NativeDigits"/>
      /// specified by <see cref="MonthCalendar.Culture"/>s <see cref="CultureInfo.NumberFormat"/>
      /// for day number display.
      /// </summary>
      private bool useNativeDigits;

      /// <summary>
      /// The height of the header.
      /// </summary>
      private int headerHeight;

      /// <summary>
      /// The width of a day.
      /// </summary>
      private int dayWidth;

      /// <summary>
      /// The height of a day.
      /// </summary>
      private int dayHeight;

      /// <summary>
      /// The height of the footer.
      /// </summary>
      private int footerHeight;

      /// <summary>
      /// The width of the week header.
      /// </summary>
      private int weekNumberWidth;

      /// <summary>
      /// The height of the day name header.
      /// </summary>
      private int dayNameHeight;

      /// <summary>
      /// The width of a single month element.
      /// </summary>
      private int monthWidth;

      /// <summary>
      /// The height of a single month element.
      /// </summary>
      private int monthHeight;

      /// <summary>
      /// The scroll change if clicked on an arrow.
      /// </summary>
      private int scrollChange;

      /// <summary>
      /// The max. selection count of days.
      /// </summary>
      private int maxSelectionCount;

      /// <summary>
      /// The renderer.
      /// </summary>
      private MonthCalendarRenderer renderer;

      /// <summary>
      /// The font for the header.
      /// </summary>
      private Font headerFont;

      /// <summary>
      /// The font for the footer.
      /// </summary>
      private Font footerFont;

      /// <summary>
      /// The font for the day names.
      /// </summary>
      private Font dayHeaderFont;

      /// <summary>
      /// The calendar dimensions.
      /// </summary>
      private Size calendarDimensions;

      /// <summary>
      /// The mouse location.
      /// </summary>
      private Point mouseLocation;

      /// <summary>
      /// The start date.
      /// </summary>
      private DateTime viewStart;

      /// <summary>
      /// The real start date.
      /// </summary>
      private DateTime realStart;

      /// <summary>
      /// The last visible date.
      /// </summary>
      private DateTime lastVisibleDate;

      /// <summary>
      /// The clicked year in a month header.
      /// </summary>
      private DateTime yearSelected;

      /// <summary>
      /// The clicked month name in a month header.
      /// </summary>
      private DateTime monthSelected;

      /// <summary>
      /// The current selection type.
      /// </summary>
      private MonthCalendarHitType currentHitType;

      /// <summary>
      /// Dates which are bolded.
      /// </summary>
      private List<DateTime> boldedDates;

      /// <summary>
      /// The footer rectangle.
      /// </summary>
      private Rectangle footerRect;

      /// <summary>
      /// The rectangle for the left arrow.
      /// </summary>
      private Rectangle leftArrowRect;

      /// <summary>
      /// The rectangle for the right arrow.
      /// </summary>
      private Rectangle rightArrowRect;

      /// <summary>
      /// The current bounds of the hit test result item.
      /// </summary>
      private Rectangle currentMoveBounds;

      /// <summary>
      /// The text align for the days.
      /// </summary>
      private ContentAlignment dayTextAlign;

      /// <summary>
      /// The selection start date.
      /// </summary>
      private DateTime selectionStart;

      /// <summary>
      /// The selection end date.
      /// </summary>
      private DateTime selectionEnd;

      /// <summary>
      /// The minimum date of the control.
      /// </summary>
      private DateTime minDate;

      /// <summary>
      /// The maximum date of the control.
      /// </summary>
      private DateTime maxDate;

      /// <summary>
      /// The months displayed.
      /// </summary>
      private MonthCalendarMonth[] months;

      /// <summary>
      /// The status information for mouse moving.
      /// </summary>
      private MonthCalendarMouseMoveFlags mouseMoveFlags;

      /// <summary>
      /// The selection start range if in week selection mode.
      /// </summary>
      private SelectionRange selectionStartRange;

      /// <summary>
      /// The selection range for backup purposes.
      /// </summary>
      private SelectionRange backupRange;

      /// <summary>
      /// The selection ranges.
      /// </summary>
      private List<SelectionRange> selectionRanges;

      /// <summary>
      /// The day selection mode.
      /// </summary>
      private MonthCalendarSelectionMode daySelectionMode;

      /// <summary>
      /// The non work days.
      /// </summary>
      private CalendarDayOfWeek nonWorkDays;

      /// <summary>
      /// The culture used by the control.
      /// </summary>
      private CultureInfo culture;

      /// <summary>
      /// The calendar used for displaying dates, months and everywhere else.
      /// </summary>
      private Calendar cultureCalendar;

      /// <summary>
      /// Interface field for handling month and day names.
      /// </summary>
      private ICustomFormatProvider formatProvider;

      /// <summary>
      /// The era date ranges.
      /// </summary>
      private MonthCalendarEraRange[] eraRanges;

      #region menu fields

      /// <summary>
      /// Context menu strip.
      /// </summary>
      private ContextMenuStrip monthMenu;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiJan;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiFeb;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiMar;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiApr;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiMay;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiJun;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiJul;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiAug;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiSep;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiOct;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiNov;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiDez;

      /// <summary>
      /// Context menu strip.
      /// </summary>
      private ContextMenuStrip yearMenu;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear1;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear2;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear3;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear4;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear5;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear6;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear7;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear8;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiYear9;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiA1;

      /// <summary>
      /// Menu item.
      /// </summary>
      private ToolStripMenuItem tsmiA2;

      /// <summary>
      /// Container for components.
      /// </summary>
      private IContainer components;

      private bool extendSelection;

      private PictureBox mFakeIt;


      #endregion

      #endregion

      #region constructor

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendar"/> class.
      /// </summary>
      public MonthCalendar()
      {
         SetStyle(
               ControlStyles.OptimizedDoubleBuffer
               | ControlStyles.ResizeRedraw
               | ControlStyles.Selectable
               | ControlStyles.AllPaintingInWmPaint
               | ControlStyles.UserPaint
               | ControlStyles.SupportsTransparentBackColor,
               true);
         AutoScroll = true;

         // initialize menus
         this.InitializeComponent();

         this.extendSelection = true;
         this.showFooter = false;
         this.showWeekHeader = true;
         this.mouseLocation = Point.Empty;
         this.yearSelected = DateTime.MinValue;
         this.monthSelected = DateTime.MinValue;
         //this.selectionStart = DateTime.Today;
         //this.selectionEnd = DateTime.Today;
         this.currentHitType = MonthCalendarHitType.None;
         this.boldedDates = new List<DateTime>();
         this.boldDatesCollection = new BoldedDatesCollection();
         this.boldDateCategoryCollection = new BoldedDateCategoryCollection(this);
         this.currentMoveBounds = Rectangle.Empty;
         this.mouseMoveFlags = new MonthCalendarMouseMoveFlags();
         this.selectionRanges = new List<SelectionRange>();
         this.daySelectionMode = MonthCalendarSelectionMode.Manual;
         this.nonWorkDays = CalendarDayOfWeek.Saturday | CalendarDayOfWeek.Sunday;
         this.culture = CultureInfo.CurrentUICulture;
         this.cultureCalendar = CultureInfo.CurrentUICulture.DateTimeFormat.Calendar;
         this.eraRanges = GetEraRanges(this.cultureCalendar);
         this.minDate = this.cultureCalendar.MinSupportedDateTime.Date < new DateTime(1900, 1, 1) ? new DateTime(1900, 1, 1) : this.cultureCalendar.MinSupportedDateTime.Date;
         this.maxDate = this.cultureCalendar.MaxSupportedDateTime.Date > new DateTime(9998, 12, 31) ? new DateTime(9998, 12, 31) : this.cultureCalendar.MaxSupportedDateTime.Date;
         this.formatProvider = new MonthCalendarFormatProvider(this.culture, null, this.culture.TextInfo.IsRightToLeft) { MonthCalendar = this };
         this.renderer = new MonthCalendarRenderer(this);
         this.calendarDimensions = new Size(4, 3);
         this.headerFont = new Font("Segoe UI", 8f, FontStyle.Regular);
         this.footerFont = new Font("Arial", 8f, FontStyle.Bold);
         this.dayHeaderFont = new Font("Segoe UI", 7f, FontStyle.Regular);
         this.dayTextAlign = ContentAlignment.MiddleCenter;

         // update year menu
         this.UpdateYearMenu(DateTime.Today.Year);

         // set start date
         this.SetStartDate(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));

         this.CalculateSize(false);
      }

      #endregion

      #region events

      /// <summary>
      /// Occurs when the user selects a date or a date range.
      /// </summary>
      [Category("Action")]
      [Description("Is raised, when the user selected a date or date range.")]
      public event EventHandler<DateRangeEventArgs> DateSelected;

      /// <summary>
      /// Occurs when the user changed the month or year.
      /// </summary>
      [Category("Action")]
      [Description("Is raised, when the user changed the month or year.")]
      public event EventHandler<DateRangeEventArgs> DateChanged;

      /// <summary>
      /// Is Raised when the mouse is over an date.
      /// </summary>
      [Category("Action")]
      [Description("Is raised when the mouse is over an date.")]
      public event EventHandler<ActiveDateChangedEventArgs> ActiveDateChanged;

      /// <summary>
      /// Is raised when the selection extension ended.
      /// </summary>
      [Category("Action")]
      [Description("Is raised when the selection extension ended.")]
      public event EventHandler SelectionExtendEnd;

      /// <summary>
      /// Is raised when a date was clicked.
      /// </summary>
      [Category("Action")]
      [Description("Is raised when a date in selection mode 'Day' was clicked.")]
      public event EventHandler<DateEventArgs> DateClicked;

      /// <summary>
      /// Is raises when a date was selected.
      /// </summary>
      internal event EventHandler<DateEventArgs> InternalDateSelected;

      #endregion

      #region properties

      #region public properties

      /// <summary>
      /// Gets or sets the start month and year.
      /// </summary>
      [Category("Appearance")]
      [Description("Sets the first displayed month and year.")]
      public DateTime ViewStart
      {
         get
         {
            return this.viewStart;
         }

         set
         {
            if (value == this.viewStart
               || value < this.minDate || value > this.maxDate)
            {
               return;
            }

            this.SetStartDate(value);

            this.UpdateMonths();

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets the last in-month date of the last displayed month.
      /// </summary>
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public DateTime ViewEnd
      {
          get
          {
              MonthCalendarDate dt = new MonthCalendarDate(this.cultureCalendar, this.viewStart)
                 .GetEndDateOfWeek(this.formatProvider).FirstOfMonth.AddMonths(this.months != null ? this.months.Length - 1 : 1).FirstOfMonth;

              int daysInMonth = dt.DaysInMonth;

              dt = dt.AddDays(daysInMonth - 1);

              return dt.Date;
          }
      }

      /// <summary>
      /// Gets the real start date of the month calendar.
      /// </summary>
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public DateTime RealStartDate
      {
         get { return this.realStart; }
      }

      /// <summary>
      /// Gets or sets the lower limit of the visible month and year.
      /// </summary>
      [Category("Behavior")]
      [Description("The viewable minimum month and year.")]
      public DateTime MinDate
      {
         get
         {
            return this.minDate;
         }

         set
         {
            if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime
               || value >= this.maxDate)
            {
               return;
            }

            value = this.GetMinDate(value);

            this.minDate = value.Date;

            this.UpdateMonths();

            int dim1 = Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height);
            int dim2 = this.months != null ? this.months.Length : 1;

            if (dim1 != dim2)
            {
               this.SetStartDate(new MonthCalendarDate(this.CultureCalendar, this.viewStart).AddMonths(dim2 - dim1).Date);
            }

            this.Invalidate();
         }
      }

      /// <summary>
      /// Gets or sets the upper limit of the visible month and year.
      /// </summary>
      [Category("Behavior")]
      [Description("The viewable maximum month and year.")]
      public DateTime MaxDate
      {
         get
         {
            return this.maxDate;
         }

         set
         {
            if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime
               || value <= this.minDate)
            {
               return;
            }

            value = this.GetMaxDate(value);

            this.maxDate = value.Date;

            this.UpdateMonths();

            int dim1 = Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height);
            int dim2 = this.months != null ? this.months.Length : 1;

            if (dim1 != dim2)
            {
               this.SetStartDate(new MonthCalendarDate(this.CultureCalendar, this.viewStart).AddMonths(dim2 - dim1).Date);
            }

            this.Invalidate();
         }
      }

      /// <summary>
      /// Gets or sets the calendar dimensions.
      /// </summary>
      [Category("Appearance")]
      [Description("The number of rows and columns of months in the calendar.")]
      [DefaultValue(typeof(Size), "1,1")]
      public Size CalendarDimensions
      {
         get
         {
            return this.calendarDimensions;
         }

         set
         {
            if (value == this.calendarDimensions || value.IsEmpty)
            {
               return;
            }

            // get number of months in a row
            value.Width = Math.Max(1, Math.Min(value.Width, 7));

            // get number of months in a column
            value.Height = Math.Max(1, Math.Min(value.Height, 7));

            // set new dimension
            this.calendarDimensions = value;

            // update width and height of control
            this.inUpdate = true;

            this.Width = value.Width * this.monthWidth;
            this.Height = (value.Height * this.monthHeight) + (this.showFooter ? this.footerHeight : 0);

            this.inUpdate = false;

            // adjust scroll change
            this.scrollChange = Math.Max(0, Math.Min(this.scrollChange, this.calendarDimensions.Width * this.calendarDimensions.Height));

            // calculate sizes
            this.CalculateSize(false);
         }
      }

      /// <summary>
      /// Gets or sets the header font.
      /// </summary>
      [Category("Appearance")]
      [Description("The font for the header.")]
      public Font HeaderFont
      {
         get
         {
            return this.headerFont;
         }

         set
         {
            if (value == this.headerFont || value == null)
            {
               return;
            }

            this.BeginUpdate();

            if (this.headerFont != null)
            {
               this.headerFont.Dispose();
            }

            this.headerFont = value;

            this.CalculateSize(false);

            this.EndUpdate();
         }
      }

      /// <summary>
      /// Gets or sets the footer font.
      /// </summary>
      [Category("Appearance")]
      [Description("The font for the footer.")]
      public Font FooterFont
      {
         get
         {
            return this.footerFont;
         }

         set
         {
            if (value == this.footerFont || value == null)
            {
               return;
            }

            this.BeginUpdate();

            if (this.footerFont != null)
            {
               this.footerFont.Dispose();
            }

            this.footerFont = value;

            this.CalculateSize(false);

            this.EndUpdate();
         }
      }

      /// <summary>
      /// Gets or sets the font for the day header.
      /// </summary>
      [Category("Appearance")]
      [Description("The font for the day header.")]
      public Font DayHeaderFont
      {
         get
         {
            return this.dayHeaderFont;
         }

         set
         {
            if (value == this.dayHeaderFont || value == null)
            {
               return;
            }

            this.BeginUpdate();

            if (this.dayHeaderFont != null)
            {
               this.dayHeaderFont.Dispose();
            }

            this.dayHeaderFont = value;

            this.CalculateSize(false);

            this.EndUpdate();
         }
      }

      /// <summary>
      /// Gets or sets the font used for the week header and days.
      /// </summary>
      public override Font Font
      {
         get
         {
            return base.Font;
         }

         set
         {
            this.BeginUpdate();

            base.Font = value;

            this.CalculateSize(false);

            this.EndUpdate();
         }
      }

      /// <summary>
      /// Gets or sets the size of the control.
      /// </summary>
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public new Size Size
      {
         get { return base.Size; }
         set { base.Size = value; }
      }

      /// <summary>
      /// Gets or sets the text alignment for the days.
      /// </summary>
      [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
      [Description("Determines the text alignment for the days.")]
      [Category("Appearance")]
      public ContentAlignment DayTextAlignment
      {
         get
         {
            return this.dayTextAlign;
         }

         set
         {
            if (value == this.dayTextAlign)
            {
               return;
            }

            this.dayTextAlign = value;

            this.Invalidate();
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to use a right to left layout.
      /// </summary>
      [DefaultValue(false)]
      [Category("Appearance")]
      [Description("Indicates whether to use the RTL layout.")]
      public bool RightToLeftLayout
      {
         get
         {
            return this.rightToLeftLayout;
         }

         set
         {
            if (value == this.rightToLeftLayout)
            {
               return;
            }

            this.rightToLeftLayout = value;

            this.formatProvider.IsRTLLanguage = this.UseRTL;

            Size calDim = this.calendarDimensions;

            this.UpdateMonths();

            this.CalendarDimensions = calDim;

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to show the footer.
      /// </summary>
      [DefaultValue(true)]
      [Category("Appearance")]
      [Description("Indicates whether to show the footer.")]
      public bool ShowFooter
      {
         get
         {
            return this.showFooter;
         }

         set
         {
            if (value == this.showFooter)
            {
               return;
            }

            this.showFooter = value;

            this.Height += value ? this.footerHeight : -this.footerHeight;

            this.Invalidate();
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to show the week header.
      /// </summary>
      [DefaultValue(true)]
      [Category("Appearance")]
      [Description("Indicates whether to show the week header.")]
      public bool ShowWeekHeader
      {
         get
         {
            return this.showWeekHeader;
         }

         set
         {
            if (this.showWeekHeader == value)
            {
               return;
            }

            this.showWeekHeader = value;

            this.CalculateSize(false);
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to use the shortest or the abbreviated day names in the day header.
      /// </summary>
      [DefaultValue(false)]
      [Category("Appearance")]
      [Description("Indicates whether to use the shortest or the abbreviated day names in the day header.")]
      public bool UseShortestDayNames
      {
         get
         {
            return this.useShortestDayNames;
         }

         set
         {
            this.useShortestDayNames = value;

            this.CalculateSize(false);
         }
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
         get { return this.useNativeDigits; }

         set
         {
            if (value == this.useNativeDigits)
            {
               return;
            }

            this.useNativeDigits = value;

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets or sets the list for bolded dates.
      /// </summary>
      [Description("The bolded dates in the month calendar.")]
      public List<DateTime> BoldedDates
      {
         get
         {
            return this.boldedDates;
         }

         set
         {
            this.boldedDates = value ?? new List<DateTime>();
         }
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
            return this.boldDatesCollection;
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
            return this.boldDateCategoryCollection;
         }
      }

      /// <summary>
      /// Gets or sets the selection start date.
      /// </summary>
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public DateTime SelectionStart
      {
         get
         {
            return this.selectionStart;
         }

         set
         {
            value = value.Date;

            // valid value ?
            if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime)
            {
               return;
            }

            if (value < this.minDate)
            {
               value = this.minDate;
            }
            else if (value > this.maxDate)
            {
               value = this.maxDate;
            }

            switch (this.daySelectionMode)
            {
               case MonthCalendarSelectionMode.Day:
                  {
                     this.selectionStart = value;
                     this.selectionEnd = value;

                     break;
                  }

               case MonthCalendarSelectionMode.WorkWeek:
               case MonthCalendarSelectionMode.FullWeek:
                  {
                     //MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, value).GetFirstDayInWeek(this.formatProvider);
                     //this.selectionStart = dt.Date;
                     //this.selectionEnd = dt.GetEndDateOfWeek(this.formatProvider).Date;

                     break;
                  }

               case MonthCalendarSelectionMode.Month:
                  {
                     //MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, value).FirstOfMonth;
                     //this.selectionStart = dt.Date;
                     //this.selectionEnd = dt.AddMonths(1).AddDays(-1).Date;

                     break;
                  }

               case MonthCalendarSelectionMode.Manual:
                  {
                     value = this.GetSelectionDate(this.selectionEnd, value);

                     if (value == DateTime.MinValue)
                     {
                        this.selectionEnd = value;
                        this.selectionStart = value;
                     }
                     else
                     {
                        this.selectionStart = value;

                        if (this.selectionEnd == DateTime.MinValue)
                        {
                           this.selectionEnd = value;
                        }
                     }

                     break;
                  }
            }

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets or sets the selection end date.
      /// </summary>
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public DateTime SelectionEnd
      {
         get
         {
            return this.selectionEnd;
         }

         set
         {
            value = value.Date;

            if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime
               || this.daySelectionMode != MonthCalendarSelectionMode.Manual)
            {
               return;
            }

            if (value < this.minDate)
            {
               value = this.minDate;
            }
            else if (value > this.maxDate)
            {
               value = this.maxDate;
            }

            value = this.GetSelectionDate(this.selectionStart, value);

            if (value == DateTime.MinValue || this.selectionStart == DateTime.MinValue)
            {
               this.selectionStart = value;
               this.selectionEnd = value;

               this.Refresh();

               return;
            }

            this.selectionEnd = value;

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets or sets the selection range of the selected dates.
      /// </summary>
      [Category("Behavior")]
      [Description("The selection range.")]
      public SelectionRange SelectionRange
      {
         get
         {
            return new SelectionRange(this.selectionStart, this.selectionEnd);
         }

         set
         {
            if (value == null)
            {
               this.ResetSelectionRange();

               return;
            }

            switch (this.daySelectionMode)
            {
               case MonthCalendarSelectionMode.Day:
               case MonthCalendarSelectionMode.WorkWeek:
               case MonthCalendarSelectionMode.FullWeek:
               case MonthCalendarSelectionMode.Month:
                  {
                     this.SelectionStart = this.selectionStart == value.Start ?
                        value.End : value.Start;

                     break;
                  }

               case MonthCalendarSelectionMode.Manual:
                  {
                     this.selectionStart = value.Start;
                     this.SelectionEnd = value.End;

                     break;
                  }
            }
         }
      }

      /// <summary>
      /// Gets the selection ranges.
      /// </summary>
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public List<SelectionRange> SelectionRanges
      {
         get
         {
            return this.selectionRanges;
         }
      }

      /// <summary>
      /// Gets or sets the scroll change if clicked on an arrow button.
      /// </summary>
      [DefaultValue(0)]
      [Category("Behavior")]
      [Description("The number of months the calendar is going for- or backwards if clicked on an arrow."
       + "A value of 0 indicates the last visible month is the first month (forwards) and vice versa.")]
      public int ScrollChange
      {
         get
         {
            return this.scrollChange;
         }

         set
         {
            if (value == this.scrollChange)
            {
               return;
            }

            this.scrollChange = value;
         }
      }

      /// <summary>
      /// Gets or sets the maximum selectable days.
      /// </summary>
      [DefaultValue(0)]
      [Category("Behavior")]
      [Description("The maximum selectable days. A value of 0 means no limit.")]
      public int MaxSelectionCount
      {
         get
         {
            return this.maxSelectionCount;
         }

         set
         {
            if (value == this.maxSelectionCount)
            {
               return;
            }

            this.maxSelectionCount = Math.Max(0, value);
         }
      }

      /// <summary>
      /// Gets or sets the day selection mode.
      /// </summary>
      [DefaultValue(MonthCalendarSelectionMode.Manual)]
      [Category("Behavior")]
      [Description("Sets the day selection mode.")]
      public MonthCalendarSelectionMode SelectionMode
      {
         get
         {
            return this.daySelectionMode;
         }

         set
         {
            if (value == this.daySelectionMode || !Enum.IsDefined(typeof(MonthCalendarSelectionMode), value))
            {
               return;
            }

            this.daySelectionMode = value;

            this.SetSelectionRange(value);

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets or sets the non working days.
      /// </summary>
      [DefaultValue(CalendarDayOfWeek.Saturday | CalendarDayOfWeek.Sunday)]
      [Category("Behavior")]
      [Description("Sets the non working days.")]
      [Editor(typeof(Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
      public CalendarDayOfWeek NonWorkDays
      {
         get
         {
            return this.nonWorkDays;
         }

         set
         {
            if (value == this.nonWorkDays)
            {
               return;
            }

            this.nonWorkDays = value;

            //if (this.daySelectionMode == MonthCalendarSelectionMode.WorkWeek)
            //{
               //this.Refresh();
            //}
         }
      }

      /// <summary>
      /// Gets or sets the used renderer.
      /// </summary>
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public MonthCalendarRenderer Renderer
      {
         get
         {
            return this.renderer;
         }

         set
         {
            if (value == null)
            {
               return;
            }

            this.renderer = value;

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets or sets the culture used by the <see cref="MonthCalendar"/>.
      /// </summary>
      [Category("Behavior")]
      [Description("The culture used by the MonthCalendar.")]
      [TypeConverter(typeof(Design.CultureInfoCustomTypeConverter))]
      public CultureInfo Culture
      {
         get
         {
            return this.culture;
         }

         set
         {
            if (value == null || value.IsNeutralCulture)
            {
               return;
            }

            this.culture = value;
            this.formatProvider.DateTimeFormat = value.DateTimeFormat;
            this.CultureCalendar = null;

            if (DateMethods.IsRTLCulture(value))
            {
               this.RightToLeft = RightToLeft.Yes;
               this.RightToLeftLayout = true;
            }
            else
            {
               this.RightToLeftLayout = false;
               this.RightToLeft = RightToLeft.Inherit;
            }

            this.formatProvider.IsRTLLanguage = this.UseRTL;
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
            return this.cultureCalendar;
         }

         set
         {
            if (value == null)
            {
               value = this.culture.Calendar;
            }

            this.cultureCalendar = value;
            this.formatProvider.Calendar = value;

            if (value.GetType() == typeof(PersianCalendar) && !value.IsReadOnly)
            {
               value.TwoDigitYearMax = 1410;
            }

            foreach (Calendar c in this.culture.OptionalCalendars)
            {
               if (value.GetType() == c.GetType())
               {
                  if (value.GetType() == typeof(GregorianCalendar))
                  {
                     GregorianCalendar g1 = (GregorianCalendar)value;
                     GregorianCalendar g2 = (GregorianCalendar)c;

                     if (g1.CalendarType != g2.CalendarType)
                     {
                        continue;
                     }
                  }

                  this.culture.DateTimeFormat.Calendar = c;
                  this.formatProvider.DateTimeFormat = this.culture.DateTimeFormat;
                  this.cultureCalendar = c;

                  value = c;

                  break;
               }
            }

            this.eraRanges = GetEraRanges(value);

            this.ReAssignSelectionMode();

            this.minDate = this.GetMinDate(value.MinSupportedDateTime.Date);
            this.maxDate = this.GetMaxDate(value.MaxSupportedDateTime.Date);

            this.SetStartDate(DateTime.Today);

            this.CalculateSize(false);
         }
      }

      /// <summary>
      /// Gets or sets the used color table.
      /// </summary>
      [Category("Appearance")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      [Description("The used colors for the month calendar.")]
      public MonthCalendarColorTable ColorTable
      {
         get
         {
            return this.renderer.ColorTable;
         }

         set
         {
            if (value == null)
            {
               return;
            }

            this.renderer.ColorTable = value;
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
         get { return this.formatProvider; }
         set { this.formatProvider = value ?? new MonthCalendarFormatProvider(this.culture, null, this.culture.TextInfo.IsRightToLeft); }
      }

      public new bool Enabled
      {
          get { return base.Enabled; }
          set
          {
              if (value)
              {
                  if (mFakeIt != null) mFakeIt.Dispose();
                  mFakeIt = null;
              }
              else
              {
                  mFakeIt = new PictureBox();
                  mFakeIt.Size = this.Size;
                  mFakeIt.Location = this.Location;
                  var bmp = new Bitmap(this.Width, this.Height);
                  this.DrawToBitmap(bmp, new Rectangle(Point.Empty, this.Size));
                  mFakeIt.Image = bmp;
                  this.Parent.Controls.Add(mFakeIt);
                  this.Parent.Controls.SetChildIndex(mFakeIt, 0);
              }
              base.Enabled = value;
          }
      }

      #endregion

      #region internal properties

      /// <summary>
      /// Gets the size of a single <see cref="MonthCalendarMonth"/>.
      /// </summary>
      internal Size MonthSize
      {
         get { return new Size(this.monthWidth, this.monthHeight); }
      }

      /// <summary>
      /// Gets the size of a single day.
      /// </summary>
      internal Size DaySize
      {
         get { return new Size(this.dayWidth, this.dayHeight); }
      }

      /// <summary>
      /// Gets the footer size.
      /// </summary>
      internal Size FooterSize
      {
         get { return new Size(this.Width, this.footerHeight); }
      }

      /// <summary>
      /// Gets the header size.
      /// </summary>
      internal Size HeaderSize
      {
         get { return new Size(this.monthWidth, this.headerHeight); }
      }

      /// <summary>
      /// Gets the size of the day names.
      /// </summary>
      internal Size DayNamesSize
      {
         get { return new Size(this.dayWidth * 7, this.dayNameHeight); }
      }

      /// <summary>
      /// Gets the size of the week numbers.
      /// </summary>
      internal Size WeekNumberSize
      {
         get { return new Size(this.weekNumberWidth, this.dayHeight * 7); }
      }

      /// <summary>
      /// Gets the date for the current day the mouse is over.
      /// </summary>
      internal DateTime MouseOverDay
      {
         get { return this.mouseMoveFlags.Day; }
      }

      /// <summary>
      /// Gets a value indicating whether the control is using the RTL layout.
      /// </summary>
      internal bool UseRTL
      {
         get { return this.RightToLeft == RightToLeft.Yes && this.rightToLeftLayout; }
      }

      /// <summary>
      /// Gets the current left button state.
      /// </summary>
      //internal ButtonState LeftButtonState
      //{
      //   get
      //   {
      //      return this.mouseMoveFlags.LeftArrow ? ButtonState.Pushed : ButtonState.Normal;
      //   }
      //}

      /// <summary>
      /// Gets the current right button state.
      /// </summary>
      //internal ButtonState RightButtonState
      //{
      //   get
      //   {
      //      return this.mouseMoveFlags.RightArrow ? ButtonState.Pushed : ButtonState.Normal;
      //   }
      //}

      /// <summary>
      /// Gets the current hit type result.
      /// </summary>
      internal MonthCalendarHitType CurrentHitType
      {
         get { return this.currentHitType; }
      }

      /// <summary>
      /// Gets the month menu.
      /// </summary>
      internal ContextMenuStrip MonthMenu
      {
         get { return this.monthMenu; }
      }

      /// <summary>
      /// Gets the year menu.
      /// </summary>
      internal ContextMenuStrip YearMenu
      {
         get { return this.yearMenu; }
      }

      /// <summary>
      /// Gets the era date ranges for the current calendar.
      /// </summary>
      internal MonthCalendarEraRange[] EraRanges
      {
         get { return this.eraRanges; }
      }

      #endregion

      #endregion

      #region methods

      #region public methods

      /// <summary>
      /// Prevents a drawing of the control until <see cref="EndUpdate"/> is called.
      /// </summary>
      public void BeginUpdate()
      {
         SendMessage(this.Handle, SETREDRAW, false, 0);
      }

      /// <summary>
      /// Ends the updating process and the control can draw itself again.
      /// </summary>
      public void EndUpdate()
      {
         SendMessage(this.Handle, SETREDRAW, true, 0);
         this.Refresh();
      }

      /// <summary>
      /// Performs a hit test with the specified <paramref name="location"/> as mouse location.
      /// </summary>
      /// <param name="location">The mouse location.</param>
      /// <returns>A <see cref="MonthCalendarHitTest"/> object.</returns>
      public MonthCalendarHitTest HitTest(Point location)
      {
         if (!this.ClientRectangle.Contains(location))
         {
            return MonthCalendarHitTest.Empty;
         }

         if (this.rightArrowRect.Contains(location))
         {
            return new MonthCalendarHitTest(this.GetNewScrollDate(false), MonthCalendarHitType.Arrow, this.rightArrowRect);
         }

         if (this.leftArrowRect.Contains(location))
         {
            return new MonthCalendarHitTest(this.GetNewScrollDate(true), MonthCalendarHitType.Arrow, this.leftArrowRect);
         }

         if (this.showFooter && this.footerRect.Contains(location))
         {
            return new MonthCalendarHitTest(DateTime.Today, MonthCalendarHitType.Footer, this.footerRect);
         }

         foreach (MonthCalendarMonth month in this.months)
         {
            MonthCalendarHitTest hit = month.HitTest(location);

            if (!hit.IsEmpty)
            {
               return hit;
            }
         }

         return MonthCalendarHitTest.Empty;
      }

      #endregion

      #region internal methods

      /// <summary>
      /// Gets all bolded dates.
      /// </summary>
      /// <returns>A generic List of type <see cref="DateTime"/> with the bolded dates.</returns>
      internal List<DateTime> GetBoldedDates()
      {
         List<DateTime> dates = new List<DateTime>();

         // remove all duplicate dates
         this.boldedDates.ForEach(date =>
         {
            if (!dates.Contains(date))
            {
               dates.Add(date);
            }
         });

         return dates;
      }

      /// <summary>
      /// Determines if the <paramref name="date"/> is selected.
      /// </summary>
      /// <param name="date">The date to determine if checked.</param>
      /// <returns>true if <paramref name="date"/> is selected; false otherwise.</returns>
      internal bool IsSelected(DateTime date)
      {
         // get if date is in first selection range
         bool selected = this.SelectionRange.Contains(date);

         // get if date is in all other selection ranges (only in WorkWeek day selection mode)
         this.selectionRanges.ForEach(range =>
         {
            selected |= range.Contains(date);
         });

         // if in WorkWeek day selection mode a date is only selected if a work day
         //if (this.daySelectionMode == MonthCalendarSelectionMode.WorkWeek)
         //{
         //   // get all work days
         //   List<DayOfWeek> workDays = DateMethods.GetWorkDays(this.nonWorkDays);

         //   // return if date is selected
         //   return workDays.Contains(date.DayOfWeek) && selected;
         //}

         // return if date is selected
         return selected;
      }

      /// <summary>
      /// Scrolls to the selection start.
      /// </summary>
      internal void EnsureSeletedDateIsVisible()
      {
         if (this.RealStartDate > this.selectionStart || this.selectionStart > this.lastVisibleDate)
         {
            this.SetStartDate(new DateTime(this.selectionStart.Year, this.selectionStart.Month, 1));

            this.UpdateMonths();
         }
      }

      /// <summary>
      /// Sets the bounds of the left arrow.
      /// </summary>
      /// <param name="rect">The bounds of the left arrow.</param>
      internal void SetLeftArrowRect(Rectangle rect)
      {
         // if in RTL mode
         if (this.UseRTL)
         {
            // the left arrow bounds are the right ones
            this.rightArrowRect = rect;
         }
         else
         {
            this.leftArrowRect = rect;
         }
      }

      /// <summary>
      /// Sets the bounds of the right arrow.
      /// </summary>
      /// <param name="rect">The bounds of the right arrow.</param>
      internal void SetRightArrowRect(Rectangle rect)
      {
         // if in RTL mode
         if (this.UseRTL)
         {
            // the right arrow bounds are the left ones
            this.leftArrowRect = rect;
         }
         else
         {
            this.rightArrowRect = rect;
         }
      }

      #endregion

      #region protected methods

      /// <summary>
      /// Releases the unmanaged resources used by the <see cref="MonthCalendar"/> control and optionally releases the managed resources.
      /// </summary>
      /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (this.headerFont != null)
            {
               this.headerFont.Dispose();
            }

            if (this.footerFont != null)
            {
               this.footerFont.Dispose();
            }

            if (this.dayHeaderFont != null)
            {
               this.dayHeaderFont.Dispose();
            }
            if (mFakeIt != null)
            {
                mFakeIt.Dispose();
            }

         }

         base.Dispose(disposing);
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.FontChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnFontChanged(EventArgs e)
      {
         base.OnFontChanged(e);

         this.CalculateSize(false);
      }

      /// <summary>
      /// Processes a dialog key.
      /// </summary>
      /// <param name="keyData">One of the <see cref="Keys"/> value that represents the key to process.</param>
      /// <returns>true if the key was processed by the control; otherwise, false.</returns>
      protected override bool ProcessDialogKey(Keys keyData)
      {
         if (this.daySelectionMode != MonthCalendarSelectionMode.Day)
         {
            return base.ProcessDialogKey(keyData);
         }

         MonthCalendarDate dt = new MonthCalendarDate(this.cultureCalendar, this.selectionStart);
         bool retValue = false;

         if (keyData == Keys.Left)
         {
            this.selectionStart = dt.AddDays(-1).Date;

            retValue = true;
         }
         else if (keyData == Keys.Right)
         {
            this.selectionStart = dt.AddDays(1).Date;

            retValue = true;
         }
         else if (keyData == Keys.Up)
         {
            this.selectionStart = dt.AddDays(-7).Date;

            retValue = true;
         }
         else if (keyData == Keys.Down)
         {
            this.selectionStart = dt.AddDays(7).Date;

            retValue = true;
         }

         if (retValue)
         {
            if (this.selectionStart < this.minDate)
            {
               this.selectionStart = this.minDate;
            }
            else if (this.selectionStart > this.maxDate)
            {
               this.selectionStart = this.maxDate;
            }

            this.SetSelectionRange(this.daySelectionMode);

            this.EnsureSeletedDateIsVisible();

            this.RaiseInternalDateSelected();

            this.Invalidate();

            return true;
         }

         return base.ProcessDialogKey(keyData);
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.KeyDown"/> event.
      /// </summary>
      /// <param name="e">A <see cref="System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
      protected override void OnKeyDown(KeyEventArgs e)
      {
         if (e.KeyCode == Keys.ControlKey)
         {
            this.extendSelection = true;
         }

         base.OnKeyDown(e);
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.KeyUp"/> event.
      /// </summary>
      /// <param name="e">A <see cref="System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
      protected override void OnKeyUp(KeyEventArgs e)
      {
         if (e.KeyCode == Keys.ControlKey)
         {
            //this.extendSelection = false;

            //this.RaiseSelectExtendEnd();
         }

         base.OnKeyUp(e);
      }

      /// <summary>
      /// Performs the work of setting the specified bounds of this control.
      /// </summary>
      /// <param name="x">The new <see cref="System.Windows.Forms.Control.Left"/> property value of the control.</param>
      /// <param name="y">The new <see cref="System.Windows.Forms.Control.Top"/> property value of the control.</param>
      /// <param name="width">The new <see cref="System.Windows.Forms.Control.Width"/> property value of the control.</param>
      /// <param name="height">The new <see cref="System.Windows.Forms.Control.Height"/> property value of the control.</param>
      /// <param name="specified">A bitwise combination of the <see cref="System.Windows.Forms.BoundsSpecified"/> values.</param>
      protected override void SetBoundsCore(
         int x,
         int y,
         int width,
         int height,
         BoundsSpecified specified)
      {
         base.SetBoundsCore(x, y, width, height, specified);

         if (this.Created || this.DesignMode)
         {
            this.CalculateSize(false);
         }
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.Paint"/> event.
      /// </summary>
      /// <param name="e">A <see cref="PaintEventArgs"/> that contains the event data.</param>
      protected override void OnPaint(PaintEventArgs e)
      {
         // return if in update mode of if nothing to draw
         if (this.calendarDimensions.IsEmpty || this.inUpdate)
         {
            return;
         }

         // set text rendering hint
         e.Graphics.TextRenderingHint =
            System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

         this.renderer.DrawControlBackground(e.Graphics);

         // loop through all months to draw
         foreach (MonthCalendarMonth month in this.months)
         {
            // if month is null or not in the clip rectangle - continue
            if (month == null || !e.ClipRectangle.IntersectsWith(month.Bounds))
            {
               continue;
            }

            MonthCalendarHeaderState state = this.GetMonthHeaderState(month.Date);

            // draw the title background
            this.renderer.DrawTitleBackground(e.Graphics, month, state);

            // draw the background for the month body
            this.renderer.DrawMonthBodyBackground(e.Graphics, month);

            // draw the background for the day header
            this.renderer.DrawDayHeaderBackground(e.Graphics, month);

            // draw the month header
            this.renderer.DrawMonthHeader(e.Graphics, month, state);

            // draw the day names header
            this.renderer.DrawDayHeader(e.Graphics, month.DayNamesBounds);

            // show week header ?
            if (this.showWeekHeader)
            {
               // draw the background of the week header
               this.renderer.DrawWeekHeaderBackground(e.Graphics, month);

               // loop through all week header elements
               foreach (MonthCalendarWeek week in month.Weeks)
               {
                  // if week not visible continue
                  if (!week.Visible)
                  {
                     continue;
                  }

                  // draw week header element
                  //this.renderer.DrawWeekHeaderItem(e.Graphics, week);
               }
            }

            // loop through all days in current month
            foreach (MonthCalendarDay day in month.Days)
            {
               // if day is not visible continue
               if (!day.Visible)
               {
                  continue;
               }

               // draw the day
               this.renderer.DrawDay(e.Graphics, day);
            }

            // draw week header separator line
            this.renderer.DrawWeekHeaderSeparator(e.Graphics, month.WeekBounds);
         }

         // if footer is shown
         if (this.showFooter)
         {
            // draw the background of the footer
            this.renderer.DrawFooterBackground(e.Graphics, this.footerRect, this.mouseMoveFlags.Footer);

            // draw the footer
            this.renderer.DrawFooter(e.Graphics, this.footerRect, this.mouseMoveFlags.Footer);
         }

         // draw the border
         using (Pen p = new Pen(MonthCalendarAbstractRenderer.GetGrayColor(this.Enabled, this.renderer.ColorTable.Border)))
         {
            Rectangle r = this.ClientRectangle;
            r.Width--;
            r.Height--;

            e.Graphics.DrawRectangle(p, r);
         }
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.MouseDown"/> event.
      /// </summary>
      /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
      protected override void OnMouseDown(MouseEventArgs e)
      {
         base.OnMouseDown(e);

         this.Focus();

         this.Capture = true;

         // reset the selection range where selection started
         this.selectionStartRange = null;

         if (e.Button == MouseButtons.Left)
         {
            // perform hit test
            MonthCalendarHitTest hit = this.HitTest(e.Location);

            // set current bounds
            this.currentMoveBounds = hit.Bounds;

            // set current hit type
            this.currentHitType = hit.Type;

            switch (hit.Type)
            {
               case MonthCalendarHitType.Day:
                  {
                     // save old selection range
                     SelectionRange oldRange = this.SelectionRange;

                     if (!this.extendSelection || this.daySelectionMode != MonthCalendarSelectionMode.Manual)
                     {
                        // clear all selection ranges
                        this.selectionRanges.Clear();
                     }

                     switch (this.daySelectionMode)
                     {
                        case MonthCalendarSelectionMode.Day:
                           {
                              this.OnDateClicked(new DateEventArgs(hit.Date));

                              // only single days are selectable
                              if (this.selectionStart != hit.Date)
                              {
                                 this.SelectionStart = hit.Date;

                                 this.RaiseDateSelected();
                              }

                              break;
                           }

                        case MonthCalendarSelectionMode.WorkWeek:
                           {
                               /*
                              // only single work week is selectable
                              // get first day of week
                              DateTime firstDay = new MonthCalendarDate(this.CultureCalendar, hit.Date).GetFirstDayInWeek(this.formatProvider).Date;

                              // get work days
                              List<DayOfWeek> workDays = DateMethods.GetWorkDays(this.nonWorkDays);

                              // reset selection start and end
                              this.selectionEnd = DateTime.MinValue;
                              this.selectionStart = DateTime.MinValue;

                              // current range
                              SelectionRange currentRange = null;

                              // build selection ranges for work days
                              for (int i = 0; i < 7; i++)
                              {
                                 DateTime toAdd = firstDay.AddDays(i);

                                 if (workDays.Contains(toAdd.DayOfWeek))
                                 {
                                    if (currentRange == null)
                                    {
                                       currentRange = new SelectionRange(DateTime.MinValue, DateTime.MinValue);
                                    }

                                    if (currentRange.Start == DateTime.MinValue)
                                    {
                                       currentRange.Start = toAdd;
                                    }
                                    else
                                    {
                                       currentRange.End = toAdd;
                                    }
                                 }
                                 else if (currentRange != null)
                                 {
                                    this.selectionRanges.Add(currentRange);

                                    currentRange = null;
                                 }
                              }

                              if (this.selectionRanges.Count >= 1)
                              {
                                 // set first selection range
                                 this.SelectionRange = this.selectionRanges[0];
                                 this.selectionRanges.RemoveAt(0);

                                 // if selection range changed, raise event
                                 if (this.SelectionRange != oldRange)
                                 {
                                    this.RaiseDateSelected();
                                 }
                              }
                              else
                              {
                                 this.Refresh();
                              }*/

                              break;
                           }

                        case MonthCalendarSelectionMode.FullWeek:
                           {
                               /*
                              // only a full week is selectable
                              // get selection start and end
                              MonthCalendarDate dt =
                                 new MonthCalendarDate(this.CultureCalendar, hit.Date).GetFirstDayInWeek(
                                    this.formatProvider);

                              this.selectionStart = dt.Date;
                              this.selectionEnd = dt.GetEndDateOfWeek(this.formatProvider).Date;

                              // if range changed, raise event
                              if (this.SelectionRange != oldRange)
                              {
                                 this.RaiseDateSelected();

                                 this.Refresh();
                              }*/

                              break;
                           }

                        case MonthCalendarSelectionMode.Month:
                           {
                               /*
                              // only a full month is selectable
                              MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, hit.Date).FirstOfMonth;

                              // get selection start and end
                              this.selectionStart = dt.Date;
                              this.selectionEnd = dt.AddMonths(1).AddDays(-1).Date;

                              // if range changed, raise event
                              if (this.SelectionRange != oldRange)
                              {
                                 this.RaiseDateSelected();

                                 this.Refresh();
                              }*/

                              break;
                           }

                        case MonthCalendarSelectionMode.Manual:
                           {
                               //if (this.extendSelection)
                               //{
                               //    var range = this.selectionRanges.Find(r => hit.Date >= r.Start && hit.Date <= r.End);

                               //    if (range != null)
                               //    {
                               //        this.selectionRanges.Remove(range);
                               //    }
                               //}

                              // manual mode - selection ends when user is releasing the left mouse button
                              this.selectionStarted = true;
                              this.backupRange = this.SelectionRange;
                              this.selectionEnd = DateTime.MinValue;
                              this.SelectionStart = hit.Date;

                              break;
                           }
                     }

                     break;
                  }

               case MonthCalendarHitType.Week:
                  {
                      /*
                     this.selectionRanges.Clear();

                     if (this.maxSelectionCount > 6 || this.maxSelectionCount == 0)
                     {
                        this.backupRange = this.SelectionRange;
                        this.selectionStarted = true;
                        this.selectionEnd = new MonthCalendarDate(this.CultureCalendar, hit.Date).GetEndDateOfWeek(this.formatProvider).Date;
                        this.SelectionStart = hit.Date;

                        this.selectionStartRange = this.SelectionRange;
                     }
                       */

                     break;
                  }

               case MonthCalendarHitType.MonthName:
                  {
                      /*
                     this.monthSelected = hit.Date;
                     this.mouseMoveFlags.HeaderDate = hit.Date;

                     this.Invalidate(hit.InvalidateBounds);
                     this.Update();

                     this.monthMenu.Tag = hit.Date;
                     this.UpdateMonthMenu(this.CultureCalendar.GetYear(hit.Date));

                     this.showingMenu = true;

                     // show month menu
                     this.monthMenu.Show(this, hit.Bounds.Right, e.Location.Y);
                       */

                     break;
                  }

               case MonthCalendarHitType.MonthYear:
                  {
                      /*
                     this.yearSelected = hit.Date;
                     this.mouseMoveFlags.HeaderDate = hit.Date;

                     this.Invalidate(hit.InvalidateBounds);
                     this.Update();

                     this.UpdateYearMenu(this.CultureCalendar.GetYear(hit.Date));

                     this.yearMenu.Tag = hit.Date;

                     this.showingMenu = true;

                     // show year menu
                     this.yearMenu.Show(this, hit.Bounds.Right, e.Location.Y);
                      */
                     break;
                  }

               case MonthCalendarHitType.Arrow:
                  {
                      /*
                     // an arrow was pressed
                     // set new start date
                     if (this.SetStartDate(hit.Date))
                     {
                        // update months
                        this.UpdateMonths();

                        // raise event
                        this.RaiseDateChanged();

                        this.mouseMoveFlags.HeaderDate = this.leftArrowRect.Contains(e.Location) ?
                           this.months[0].Date : this.months[this.calendarDimensions.Width - 1].Date;

                        this.Refresh();
                     }
                       */

                     break;
                  }

               case MonthCalendarHitType.Footer:
                  {
                      /*
                     // footer was pressed
                     this.selectionRanges.Clear();

                     bool raiseDateChanged = false;

                     SelectionRange range = this.SelectionRange;

                     // determine if date changed event has to be raised
                     if (DateTime.Today < this.months[0].FirstVisibleDate || DateTime.Today > this.lastVisibleDate)
                     {
                        // set new start date
                        if (this.SetStartDate(DateTime.Today))
                        {
                           // update months
                           this.UpdateMonths();

                           raiseDateChanged = true;
                        }
                        else
                        {
                           break;
                        }
                     }

                     // set new selection start and end values
                     this.selectionStart = DateTime.Today;
                     this.selectionEnd = DateTime.Today;

                     this.SetSelectionRange(this.daySelectionMode);

                     this.OnDateClicked(new DateEventArgs(DateTime.Today));

                     // raise events if necessary
                     if (range != this.SelectionRange)
                     {
                        this.RaiseDateSelected();
                     }

                     if (raiseDateChanged)
                     {
                        this.RaiseDateChanged();
                     }

                     this.Refresh();
                       */

                     break;
                  }

               case MonthCalendarHitType.Header:
                  {
                      /*
                     // header was pressed
                     this.Invalidate(hit.Bounds);
                     this.Update();
                       */

                     break;
                  }
            }
         }
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.MouseMove"/> event.
      /// </summary>
      /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
      protected override void OnMouseMove(MouseEventArgs e)
      {
         base.OnMouseMove(e);

         if (e.Location == this.mouseLocation)
         {
            return;
         }

         this.mouseLocation = e.Location;

         // backup and reset mouse move flags
         this.mouseMoveFlags.BackupAndReset();

         // perform hit test
         MonthCalendarHitTest hit = this.HitTest(e.Location);

         if (e.Button == MouseButtons.Left)
         {
            // if selection started - only in manual selection mode
            if (this.selectionStarted)
            {
               // if selection started with hit type Day and mouse is over new date
               if (hit.Type == MonthCalendarHitType.Day
                  && this.currentHitType == MonthCalendarHitType.Day
                  && this.currentMoveBounds != hit.Bounds)
               {
                  this.currentMoveBounds = hit.Bounds;

                  // set new selection end
                  this.SelectionEnd = hit.Date;
               }
               else if (hit.Type == MonthCalendarHitType.Week
                  && this.currentHitType == MonthCalendarHitType.Week)
               {

                  // set indicator that a week header element is selected
                  this.mouseMoveFlags.WeekHeader = true;

                  // get new end date
                  DateTime endDate = new MonthCalendarDate(this.CultureCalendar, hit.Date).AddDays(6).Date;

                  // if new week header element
                  if (this.currentMoveBounds != hit.Bounds)
                  {
                     this.currentMoveBounds = hit.Bounds;

                     // check if selection is switched
                     if (this.selectionStart == this.selectionStartRange.End)
                     {
                        // are we after the original end date?
                        if (endDate > this.selectionStart)
                        {
                           // set original start date
                           this.selectionStart = this.selectionStartRange.Start;

                           // set new end date
                           this.SelectionEnd = endDate;
                        }
                        else
                        {
                           // going backwards - set new "end" date - it's now the start date
                           this.SelectionEnd = hit.Date;
                        }
                     }
                     else
                     {
                        // we are after the start date
                        if (endDate > this.selectionStart)
                        {
                           // set end date
                           this.SelectionEnd = endDate;
                        }
                        else
                        {
                           // switch start and end
                           this.selectionStart = this.selectionStartRange.End;
                           this.SelectionEnd = hit.Date;
                        }
                     }
                  }
               }
            }
            else
            {
               switch (hit.Type)
               {
                  case MonthCalendarHitType.MonthName:
                     {
                        this.mouseMoveFlags.MonthName = hit.Date;
                        this.mouseMoveFlags.HeaderDate = hit.Date;

                        this.Invalidate(hit.InvalidateBounds);

                        break;
                     }

                  case MonthCalendarHitType.MonthYear:
                     {
                        this.mouseMoveFlags.Year = hit.Date;
                        this.mouseMoveFlags.HeaderDate = hit.Date;

                        this.Invalidate(hit.InvalidateBounds);

                        break;
                     }

                  case MonthCalendarHitType.Header:
                     {
                        this.mouseMoveFlags.HeaderDate = hit.Date;
                        this.Invalidate(hit.InvalidateBounds);

                        break;
                     }

                  case MonthCalendarHitType.Arrow:
                     {
                        bool useRTL = this.UseRTL;

                        if (this.leftArrowRect.Contains(e.Location))
                        {
                           this.mouseMoveFlags.LeftArrow = !useRTL;
                           this.mouseMoveFlags.RightArrow = useRTL;

                           this.mouseMoveFlags.HeaderDate = this.months[0].Date;
                        }
                        else
                        {
                           this.mouseMoveFlags.LeftArrow = useRTL;
                           this.mouseMoveFlags.RightArrow = !useRTL;

                           this.mouseMoveFlags.HeaderDate = this.months[this.calendarDimensions.Width - 1].Date;
                        }

                        this.Invalidate(hit.InvalidateBounds);

                        break;
                     }

                  case MonthCalendarHitType.Footer:
                     {
                        this.mouseMoveFlags.Footer = true;

                        this.Invalidate(hit.InvalidateBounds);

                        break;
                     }

                  default:
                     {
                        this.Invalidate();

                        break;
                     }
               }
            }
         }
         else if (e.Button == MouseButtons.None)
         {
            // no mouse button is pressed
            // set flags and invalidate corresponding region
            switch (hit.Type)
            {
               case MonthCalendarHitType.Day:
                  {
                     this.mouseMoveFlags.Day = hit.Date;

                     var bold = this.GetBoldedDates().Contains(hit.Date) || this.boldDatesCollection.Exists(d => d.Value.Date == hit.Date.Date);

                     this.OnActiveDateChanged(new ActiveDateChangedEventArgs(hit.Date, bold));

                     this.InvalidateMonth(hit.Date, true);

                     break;
                  }

               case MonthCalendarHitType.Week:
                  {
                     this.mouseMoveFlags.WeekHeader = true;

                     break;
                  }

               case MonthCalendarHitType.MonthName:
                  {
                     this.mouseMoveFlags.MonthName = hit.Date;
                     this.mouseMoveFlags.HeaderDate = hit.Date;

                     break;
                  }

               case MonthCalendarHitType.MonthYear:
                  {
                     this.mouseMoveFlags.Year = hit.Date;
                     this.mouseMoveFlags.HeaderDate = hit.Date;

                     break;
                  }

               case MonthCalendarHitType.Header:
                  {
                     this.mouseMoveFlags.HeaderDate = hit.Date;

                     break;
                  }

               case MonthCalendarHitType.Arrow:
                  {
                     bool useRTL = this.UseRTL;

                     if (this.leftArrowRect.Contains(e.Location))
                     {
                        this.mouseMoveFlags.LeftArrow = !useRTL;
                        this.mouseMoveFlags.RightArrow = useRTL;

                        this.mouseMoveFlags.HeaderDate = this.months[0].Date;
                     }
                     else if (this.rightArrowRect.Contains(e.Location))
                     {
                        this.mouseMoveFlags.LeftArrow = useRTL;
                        this.mouseMoveFlags.RightArrow = !useRTL;

                        this.mouseMoveFlags.HeaderDate = this.months[this.calendarDimensions.Width - 1].Date;
                     }

                     break;
                  }

               case MonthCalendarHitType.Footer:
                  {
                     this.mouseMoveFlags.Footer = true;

                     break;
                  }
            }

            // if left arrow state changed
            if (this.mouseMoveFlags.LeftArrowChanged)
            {
               this.Invalidate(this.UseRTL ? this.rightArrowRect : this.leftArrowRect);

               this.Update();
            }

            // if right arrow state changed
            if (this.mouseMoveFlags.RightArrowChanged)
            {
               this.Invalidate(this.UseRTL ? this.leftArrowRect : this.rightArrowRect);

               this.Update();
            }

            // if header state changed
            if (this.mouseMoveFlags.HeaderDateChanged)
            {
               this.Invalidate();
            }
            else if (this.mouseMoveFlags.MonthNameChanged || this.mouseMoveFlags.YearChanged)
            {
               // if state of month name or year in header changed
               SelectionRange range1 = new SelectionRange(this.mouseMoveFlags.MonthName, this.mouseMoveFlags.Backup.MonthName);

               SelectionRange range2 = new SelectionRange(this.mouseMoveFlags.Year, this.mouseMoveFlags.Backup.Year);

               this.Invalidate(this.months[this.GetIndex(range1.End)].TitleBounds);

               if (range1.End != range2.End)
               {
                  this.Invalidate(this.months[this.GetIndex(range2.End)].TitleBounds);
               }
            }

            // if day state changed
            if (this.mouseMoveFlags.DayChanged)
            {
               // invalidate current day
               this.InvalidateMonth(this.mouseMoveFlags.Day, false);

               // invalidate last day
               this.InvalidateMonth(this.mouseMoveFlags.Backup.Day, false);
            }

            // if footer state changed
            if (this.mouseMoveFlags.FooterChanged)
            {
               this.Invalidate(this.footerRect);
            }
         }

         // if mouse is over a week header, change cursor
         if (this.mouseMoveFlags.WeekHeaderChanged)
         {
            //this.Cursor = this.mouseMoveFlags.WeekHeader ? Cursors.UpArrow : Cursors.Default;
         }
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.MouseUp"/> event.
      /// </summary>
      /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
      protected override void OnMouseUp(MouseEventArgs e)
      {
          var cat = new CustomControls.BoldedDateCategory("NonOperation") { ForeColor = Color.Red };
         base.OnMouseUp(e);


         // if left mouse button is pressed and selection process was started
         if (e.Button == MouseButtons.Left && this.selectionStarted)
         {
             
             for (DateTime date = this.SelectionRange.Start; date <= this.SelectionRange.End; date = date.AddDays(1))
             {
                 //既に選択されたかチェック
                 var range = this.SelectionRanges.Find(r => date >= r.Start && date <= r.End);
                 //既に選択された場合は選択された日を非選択にする
                 if (range != null)
                 {
                     this.selectionRanges.Remove(range);
                 }
                 else
                 {
                     //未選択日の場合、稼働日かチェック
                     var boldDate = this.BoldedDatesCollection.Find(r => date >= r.Value && date <= r.Value && r.Category.Name.Equals("Operation"));
                     //稼働日の場合、非稼働日にする
                     if (!boldDate.IsEmpty)
                     {
                         this.BoldedDatesCollection.Remove(boldDate);
                         this.BoldedDatesCollection.Add(new CustomControls.BoldedDate { Category = cat, Value = boldDate.Value });
                     }
                     else
                     {
                         this.selectionRanges.Add(new SelectionRange(date, date));
                     }
                 }

             }

            // reset selection process
            this.selectionStarted = false;

            this.Refresh();

            // raise selected event if necessary
            if (this.backupRange.Start != this.SelectionRange.Start
               || this.backupRange.End != this.SelectionRange.End)
            {
               // raise date 
               this.RaiseDateSelected();
            }
         }

         // reset current hit type
         this.currentHitType = MonthCalendarHitType.None;

         this.Capture = false;
         this.SelectionRange= null;
      }

      private void controlSelectedDates()
      {

      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.MouseLeave"/> event.
      /// </summary>
      /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);

         // reset some of the mouse move flags
         this.mouseMoveFlags.LeftArrow = false;
         this.mouseMoveFlags.RightArrow = false;
         this.mouseMoveFlags.MonthName = DateTime.MinValue;
         this.mouseMoveFlags.Year = DateTime.MinValue;
         this.mouseMoveFlags.Footer = false;
         this.mouseMoveFlags.Day = DateTime.MinValue;

         if (!this.showingMenu)
         {
            this.mouseMoveFlags.HeaderDate = DateTime.MinValue;
         }

         this.Invalidate();
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.MouseWheel"/> event.
      /// </summary>
      /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
      protected override void OnMouseWheel(MouseEventArgs e)
      {
         base.OnMouseWheel(e);

         //年スクロールしないようにする
         //if (!this.showingMenu)
         //{
         //   this.Scroll(e.Delta > 0);
         //}
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.ParentChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnParentChanged(EventArgs e)
      {
         base.OnParentChanged(e);

         if (this.Parent != null && this.Created)
         {
            this.UpdateMonths();

            this.Invalidate();
         }
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.RightToLeftChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnRightToLeftChanged(EventArgs e)
      {
         base.OnRightToLeftChanged(e);

         this.formatProvider.IsRTLLanguage = this.UseRTL;

         this.UpdateMonths();

         this.Invalidate();
      }

      /// <summary>
      /// Raises the <see cref="System.Windows.Forms.Control.EnabledChanged"/> event.
      /// </summary>
      /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
      protected override void OnEnabledChanged(EventArgs e)
      {
         base.OnEnabledChanged(e);

         this.Refresh();
      }

      /// <summary>
      /// Raises the <see cref="DateSelected"/> event.
      /// </summary>
      /// <param name="e">The <see cref="DateRangeEventArgs"/> object that contains the event data.</param>
      protected virtual void OnDateSelected(DateRangeEventArgs e)
      {
         if (this.DateSelected != null)
         {
            this.DateSelected(this, e);
         }
      }

      /// <summary>
      /// Raises the <see cref="DateClicked"/> event.
      /// </summary>
      /// <param name="e">A <see cref="DateEventArgs"/> that contains the event data.</param>
      protected virtual void OnDateClicked(DateEventArgs e)
      {
         if (this.DateClicked != null)
         {
            this.DateClicked(this, e);
         }
      }

      /// <summary>
      /// Raises the <see cref="DateChanged"/> event.
      /// </summary>
      /// <param name="e">The <see cref="DateRangeEventArgs"/> object that contains the event data.</param>
      protected virtual void OnDateChanged(DateRangeEventArgs e)
      {
         if (this.DateChanged != null)
         {
            this.DateChanged(this, e);
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
      /// Builds an array of <see cref="MonthCalendarEraRange"/> to store the min and max date of the eras of the specified <see cref="System.Globalization.Calendar"/>.
      /// </summary>
      /// <param name="cal">The <see cref="System.Globalization.Calendar"/> to retrieve the era ranges for.</param>
      /// <returns>An array of type <see cref="MonthCalendarEraRange"/>.</returns>
      private static MonthCalendarEraRange[] GetEraRanges(Calendar cal)
      {
         if (cal.Eras.Length == 1)
         {
            return new[] { new MonthCalendarEraRange(cal.Eras[0], cal.MinSupportedDateTime.Date, cal.MaxSupportedDateTime.Date) };
         }

         List<MonthCalendarEraRange> ranges = new List<MonthCalendarEraRange>();

         DateTime date = cal.MinSupportedDateTime.Date;

         int currentEra = -1;

         while (date < cal.MaxSupportedDateTime.Date)
         {
            int era = cal.GetEra(date);

            if (era != currentEra)
            {
               ranges.Add(new MonthCalendarEraRange(era, date));

               if (currentEra != -1)
               {
                  ranges[ranges.Count - 2].MaxDate = cal.AddDays(date, -1);
               }

               currentEra = era;
            }

            date = cal.AddDays(date, 1);
         }

         ranges[ranges.Count - 1].MaxDate = date;

         return ranges.ToArray();
      }

      /// <summary>
      /// Sends a message.
      /// </summary>
      /// <param name="wnd">The handle to the window.</param>
      /// <param name="msg">The message.</param>
      /// <param name="param">Indicates a specified <paramref name="lparam"/> parameter.</param>
      /// <param name="lparam">An additional parameter.</param>
      /// <returns>An int value indicating success or failure.</returns>
      [System.Runtime.InteropServices.DllImport("user32.dll")]
      private static extern int SendMessage(IntPtr wnd, int msg, bool param, int lparam);

      /// <summary>
      /// Gets the era range for the specified era.
      /// </summary>
      /// <param name="era">The era to get the date range for.</param>
      /// <returns>A <see cref="MonthCalendarEraRange"/> object.</returns>
      private MonthCalendarEraRange GetEraRange(int era)
      {
         foreach (MonthCalendarEraRange e in this.eraRanges)
         {
            if (e.Era == era)
            {
               return e;
            }
         }

         return new MonthCalendarEraRange(
            this.CultureCalendar.GetEra(DateTime.Today),
            this.CultureCalendar.MinSupportedDateTime.Date,
            this.CultureCalendar.MaxSupportedDateTime.Date);
      }

      /// <summary>
      /// Gets the era range for the era the current date is in.
      /// </summary>
      /// <returns>A <see cref="MonthCalendarEraRange"/>.</returns>
      private MonthCalendarEraRange GetEraRange()
      {
         return this.GetEraRange(this.CultureCalendar.GetEra(DateTime.Today));
      }

      /// <summary>
      /// Calculates the various sizes of a single month view and the global size of the control.
      /// </summary>
      private void CalculateSize(bool changeDimension)
      {
         // if already calculating - return
         if (this.inUpdate)
         {
            return;
         }

         this.inUpdate = true;

         using (Graphics g = this.CreateGraphics())
         {
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular);

             // get sizes for different elements of the calendar
            SizeF daySize = g.MeasureString("30", this.Font);

            SizeF weekNumSize = g.MeasureString("59", this.Font);

            MonthCalendarDate date = new MonthCalendarDate(this.CultureCalendar, this.viewStart);

            SizeF monthNameSize = g.MeasureString(
               this.formatProvider.GetMonthName(date.Year, date.Month),
               this.headerFont);
            SizeF yearStringSize = g.MeasureString(
               this.viewStart.ToString("yyyy"), this.headerFont);
            SizeF footerStringSize = g.MeasureString(
               this.viewStart.ToShortDateString(), this.footerFont);

            // calculate the header height
            this.headerHeight = Math.Max(
               (int)Math.Max(monthNameSize.Height + 3, yearStringSize.Height) + 1, 15);

            // calculate the width of a single day
            this.dayWidth = Math.Max(12, (int)daySize.Width + 1) + 5;

            // calculate the height of a single day
            this.dayHeight = Math.Max(Math.Max(12, (int)weekNumSize.Height + 1), (int)daySize.Height + 1) + 1;

            // calculate the height of the footer
            this.footerHeight = Math.Max(12, (int)footerStringSize.Height + 1);

            // calculate the width of the week number header
            this.weekNumberWidth = this.showWeekHeader ? Math.Max(10, (int)weekNumSize.Width + 1) + 1 : 0;

            // set minimal height of the day name header
            this.dayNameHeight = 14;

            // loop through all day names
            foreach (string str in DateMethods.GetDayNames(this.formatProvider, this.useShortestDayNames ? 2 : 1))
            {
               // get the size of the name
               SizeF dayNameSize = g.MeasureString(str, this.dayHeaderFont);

               // adjust the width of the day and the day name header height
               this.dayWidth = Math.Max(this.dayWidth, (int)dayNameSize.Width + 1);
               this.dayNameHeight = Math.Max(
                  this.dayNameHeight,
                  (int)dayNameSize.Height + 1);
            }

            // calculate the width and height of a MonthCalendarMonth element
            this.monthWidth = this.weekNumberWidth + (this.dayWidth * 7) + 1;
            this.monthHeight = this.headerHeight
               + this.dayNameHeight + (this.dayHeight * 6) + 1;

            if (changeDimension)
            {
               // calculate the dimension of the control
               int calWidthDim = Math.Max(1, this.Width / this.monthWidth);
               int calHeightDim = Math.Max(1, this.Height / this.monthHeight);

               // set the dimensions
               this.CalendarDimensions = new Size(calWidthDim, calHeightDim);
            }

            // set the width and height of the control
            this.Height = (this.monthHeight * this.calendarDimensions.Height) + (this.showFooter ? this.footerHeight : 0);
            this.Width = this.monthWidth * this.calendarDimensions.Width;

            // calculate the footer bounds
            this.footerRect = new Rectangle(
               1,
               this.Height - this.footerHeight - 1,
               this.Width - 2,
               this.footerHeight);

            // update the months
            this.UpdateMonths();
         }

         this.inUpdate = false;

         this.Refresh();
      }

      /// <summary>
      /// Sets the start date.
      /// </summary>
      /// <param name="start">The start date.</param>
      /// <returns>true if <paramref name="start"/> is valid; false otherwise.</returns>
      private bool SetStartDate(DateTime start)
      {
         if (start < DateTime.MinValue.Date || start > DateTime.MaxValue.Date)
         {
            return false;
         }

         DayOfWeek firstDayOfWeek = this.formatProvider.FirstDayOfWeek;

         MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.maxDate);

         if (start > this.maxDate)
         {
            start = dt.AddMonths(1 - this.months.Length).FirstOfMonth.Date;
         }

         if (start < this.minDate)
         {
            start = this.minDate;
         }

         dt = new MonthCalendarDate(this.CultureCalendar, start);
         int length = this.months != null ? this.months.Length - 1 : 0;

         while (dt.Date > this.minDate && dt.Day != 1)
         {
            dt = dt.AddDays(-1);
         }

         MonthCalendarDate endDate = dt.AddMonths(length);
         MonthCalendarDate endDateDay = endDate.AddDays(endDate.DaysInMonth - 1 - (endDate.Day - 1));

         if (endDate.Date >= this.maxDate || endDateDay.Date >= this.maxDate)
         {
            dt = new MonthCalendarDate(this.CultureCalendar, this.maxDate).AddMonths(-length).FirstOfMonth;
         }

         this.viewStart = dt.Date;

         while (dt.Date > this.CultureCalendar.MinSupportedDateTime.Date && dt.DayOfWeek != firstDayOfWeek)
         {
            dt = dt.AddDays(-1);
         }

         this.realStart = dt.Date;

         return true;
      }

      /// <summary>
      /// Gets the index of the <see cref="MonthCalendarMonth"/> in the array of the specified monthYear datetime.
      /// </summary>
      /// <param name="monthYear">The date to search for.</param>
      /// <returns>The index in the array.</returns>
      private int GetIndex(DateTime monthYear)
      {
         return (from month in this.months where month != null where month.Date == monthYear select month.Index)
            .FirstOrDefault();
      }

      /// <summary>
      /// Gets the <see cref="MonthCalendarMonth"/> which contains the specified date.
      /// </summary>
      /// <param name="day">The day to search for.</param>
      /// <returns>An <see cref="MonthCalendarMonth"/> if day is valid; otherwise null.</returns>
      private MonthCalendarMonth GetMonth(DateTime day)
      {
         if (day == DateTime.MinValue)
         {
            return null;
         }

         return this.months.Where(month => month != null)
            .FirstOrDefault(month => month.ContainsDate(day));
      }

      /// <summary>
      /// Updates the shown months.
      /// </summary>
      public void UpdateMonths()
      {
         int x = 0, y = 0, index = 0;
         int calWidthDim = this.calendarDimensions.Width;
         int calHeightDim = this.calendarDimensions.Height;

         List<MonthCalendarMonth> monthList = new List<MonthCalendarMonth>();

         MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.viewStart);

         if (dt.GetEndDateOfWeek(this.formatProvider).Month != dt.Month)
         {
            dt = dt.GetEndDateOfWeek(this.formatProvider).FirstOfMonth;
         }

         if (this.UseRTL)
         {
            x = this.monthWidth * (calWidthDim - 1);

            for (int i = 0; i < calHeightDim; i++)
            {
               for (int j = calWidthDim - 1; j >= 0; j--)
               {
                  if (dt.Date >= this.maxDate)
                  {
                     break;
                  }

                  monthList.Add(new MonthCalendarMonth(this, dt.Date)
                  {
                     Location = new Point(x, y),
                     Index = index++
                  });

                  x -= this.monthWidth;
                  dt = dt.AddMonths(1);
               }

               x = this.monthWidth * (calWidthDim - 1);
               y += this.monthHeight;
            }
         }
         else
         {
            for (int i = 0; i < calHeightDim; i++)
            {
               for (int j = 0; j < calWidthDim; j++)
               {
                  if (dt.Date >= this.maxDate)
                  {
                     break;
                  }

                  monthList.Add(new MonthCalendarMonth(this, dt.Date)
                  {
                     Location = new Point(x, y),
                     Index = index++
                  });

                  x += this.monthWidth;
                  dt = dt.AddMonths(1);
               }

               x = 0;
               y += this.monthHeight;
            }
         }

         this.lastVisibleDate = monthList[monthList.Count - 1].LastVisibleDate;

         this.months = monthList.ToArray();
      }

      /// <summary>
      /// Updates the month menu.
      /// </summary>
      /// <param name="year">The year to calculate the months for.</param>
      private void UpdateMonthMenu(int year)
      {
         int i = 1;

         int monthsInYear = this.CultureCalendar.GetMonthsInYear(year);

         // set month names in menu
         foreach (ToolStripMenuItem item in this.monthMenu.Items)
         {
            if (i <= monthsInYear)
            {
               item.Tag = i;
               item.Text = this.formatProvider.GetMonthName(year, i++);
               item.Visible = true;
            }
            else
            {
               item.Tag = null;
               item.Text = string.Empty;
               item.Visible = false;
            }
         }
      }

      /// <summary>
      /// Updates the year menu.
      /// </summary>
      /// <param name="year">The year in the middle to display.</param>
      private void UpdateYearMenu(int year)
      {
         year -= 4;

         int maxYear = this.CultureCalendar.GetYear(this.maxDate);
         int minYear = this.CultureCalendar.GetYear(this.minDate);

         if (year + 8 > maxYear)
         {
            year = maxYear - 8;
         }
         else if (year < minYear)
         {
            year = minYear;
         }

         year = Math.Max(1, year);

         foreach (ToolStripMenuItem item in this.yearMenu.Items)
         {
            item.Text = DateMethods.GetNumberString(year, this.UseNativeDigits ? this.Culture.NumberFormat.NativeDigits : null, false);
            item.Tag = year;

            item.Font = year == this.CultureCalendar.GetYear(DateTime.Today) ?
               new Font("Tahoma", 8F, FontStyle.Bold)
               : new Font("Tahoma", 8F, FontStyle.Regular);
            item.ForeColor = year == this.CultureCalendar.GetYear(DateTime.Today) ?
               Color.FromArgb(251, 200, 79)
               : Color.Black;

            year++;
         }
      }

      /// <summary>
      /// Handles clicks in the month menu.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event data.</param>
      private void MonthClick(object sender, EventArgs e)
      {
         MonthCalendarDate currentMonthYear = new MonthCalendarDate(this.CultureCalendar, (DateTime)this.monthMenu.Tag);

         int monthClicked = (int)((ToolStripMenuItem)sender).Tag;

         if (currentMonthYear.Month != monthClicked)
         {
            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, new DateTime(currentMonthYear.Year, monthClicked, 1, this.CultureCalendar));
            DateTime newStartDate = dt.AddMonths(-this.GetIndex(currentMonthYear.Date)).Date;

            if (this.SetStartDate(newStartDate))
            {
               this.UpdateMonths();

               this.RaiseDateChanged();

               this.Focus();

               this.Refresh();
            }
         }
      }

      /// <summary>
      /// Handles clicks in the year menu.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event data.</param>
      private void YearClick(object sender, EventArgs e)
      {
         DateTime currentMonthYear = (DateTime)this.yearMenu.Tag;

         int yearClicked = (int)((ToolStripMenuItem)sender).Tag;

         MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, currentMonthYear);

         if (dt.Year != yearClicked)
         {
            MonthCalendarDate newStartDate = new MonthCalendarDate(this.CultureCalendar, new DateTime(yearClicked, dt.Month, 1, this.CultureCalendar))
               .AddMonths(-this.GetIndex(currentMonthYear));

            if (this.SetStartDate(newStartDate.Date))
            {
               this.UpdateMonths();

               this.RaiseDateChanged();

               this.Focus();

               this.Refresh();
            }
         }
      }

      /// <summary>
      /// Is called when the month menu was closed.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="ToolStripDropDownClosedEventArgs"/> that contains the event data.</param>
      private void MonthMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
      {
         this.monthSelected = DateTime.MinValue;
         this.showingMenu = false;

         this.Invalidate(this.months[this.GetIndex((DateTime)this.monthMenu.Tag)].TitleBounds);
      }

      /// <summary>
      /// Is called when the year menu was closed.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">A <see cref="ToolStripDropDownClosedEventArgs"/> that contains the event data.</param>
      private void YearMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
      {
         this.yearSelected = DateTime.MinValue;
         this.showingMenu = false;

         this.Invalidate(this.months[this.GetIndex((DateTime)this.yearMenu.Tag)].TitleBounds);
      }

      /// <summary>
      /// Calls <see cref="OnDateSelected"/>.
      /// </summary>
      private void RaiseDateSelected()
      {
         SelectionRange range = this.SelectionRange;

         DateTime selStart = range.Start;
         DateTime selEnd = range.End;

         if (selStart == DateTime.MinValue)
         {
            selStart = selEnd;
         }

         this.OnDateSelected(new DateRangeEventArgs(selStart, selEnd));
      }

      /// <summary>
      /// Calls <see cref="OnDateChanged"/>.
      /// </summary>
      private void RaiseDateChanged()
      {
         this.OnDateChanged(
            new DateRangeEventArgs(this.realStart, this.lastVisibleDate));
      }

      /// <summary>
      /// Raises the <see cref="SelectionExtendEnd"/> event.
      /// </summary>
      private void RaiseSelectExtendEnd()
      {
         var handler = this.SelectionExtendEnd;

         if (handler != null)
         {
            handler(this, EventArgs.Empty);
         }
      }

      /// <summary>
      /// Raises the <see cref="InternalDateSelected"/> event.
      /// </summary>
      private void RaiseInternalDateSelected()
      {
         if (this.InternalDateSelected != null)
         {
            this.InternalDateSelected(this, new DateEventArgs(this.selectionStart));
         }
      }

      /// <summary>
      /// Adjusts the currently displayed month by setting a new start date.
      /// </summary>
      /// <param name="up">true for scrolling upwards, false otherwise.</param>
      private void Scroll(bool up)
      {
         if (this.SetStartDate(this.GetNewScrollDate(up)))
         {
            this.UpdateMonths();

            this.RaiseDateChanged();

            this.Refresh();
         }
      }

      /// <summary>
      /// Gets the new date for the specified scroll direction.
      /// </summary>
      /// <param name="up">true for scrolling upwards, false otherwise.</param>
      /// <returns>The new start date.</returns>
      private DateTime GetNewScrollDate(bool up)
      {
         if ((this.lastVisibleDate == this.maxDate && !up) || (this.months[0].FirstVisibleDate == this.minDate && up))
         {
            return this.viewStart;
         }

         MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.viewStart);

         int monthsToAdd = (this.scrollChange == 0 ?
            Math.Max((this.calendarDimensions.Width * this.calendarDimensions.Height), 1)
            : this.scrollChange) * (up ? -1 : 1);

         int length = this.months == null ? Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height) : this.months.Length;

         MonthCalendarDate newStartMonthDate = dt.AddMonths(monthsToAdd);

         MonthCalendarDate lastMonthDate = newStartMonthDate.AddMonths(length - 1);

         MonthCalendarDate lastMonthEndDate = lastMonthDate.AddDays(lastMonthDate.DaysInMonth - 1 - lastMonthDate.Day);

         if (newStartMonthDate.Date < this.minDate)
         {
            newStartMonthDate = new MonthCalendarDate(this.CultureCalendar, this.minDate);
         }
         else if (lastMonthEndDate.Date >= this.maxDate || lastMonthDate.Date >= this.maxDate)
         {
            MonthCalendarDate maxdt = new MonthCalendarDate(this.CultureCalendar, this.maxDate).FirstOfMonth;
            newStartMonthDate = maxdt.AddMonths(1 - length);
         }

         return newStartMonthDate.Date;
      }

      /// <summary>
      /// Gets the current month header state.
      /// </summary>
      /// <param name="monthDate">The month date.</param>
      /// <returns>A <see cref="MonthCalendarHeaderState"/> value.</returns>
      private MonthCalendarHeaderState GetMonthHeaderState(DateTime monthDate)
      {
         MonthCalendarHeaderState state = MonthCalendarHeaderState.Default;

         if (this.monthSelected == monthDate)
         {
            state = MonthCalendarHeaderState.MonthNameSelected;
         }
         else if (this.yearSelected == monthDate)
         {
            state = MonthCalendarHeaderState.YearSelected;
         }
         else if (this.mouseMoveFlags.MonthName == monthDate)
         {
            state = MonthCalendarHeaderState.MonthNameActive;
         }
         else if (this.mouseMoveFlags.Year == monthDate)
         {
            state = MonthCalendarHeaderState.YearActive;
         }
         else if (this.mouseMoveFlags.HeaderDate == monthDate)
         {
            state = MonthCalendarHeaderState.Active;
         }

         return state;
      }

      /// <summary>
      /// Invalidates the region taken up by the month specified by the <paramref name="date"/>.
      /// </summary>
      /// <param name="date">The date specifying the <see cref="MonthCalendarMonth"/> to invalidate.</param>
      /// <param name="refreshInvalid">true for refreshing the whole control if invalid <paramref name="date"/> passed; otherwise false.</param>
      private void InvalidateMonth(DateTime date, bool refreshInvalid)
      {
         if (date == DateTime.MinValue)
         {
            if (refreshInvalid)
            {
               this.Refresh();
            }

            return;
         }

         MonthCalendarMonth month = this.GetMonth(date);

         if (month != null)
         {
            this.Invalidate(month.MonthBounds);
            this.Update();
         }
         else if (date > this.lastVisibleDate)
         {
            this.Invalidate(this.months[this.months.Length - 1].Bounds);
            this.Update();
         }
         else if (refreshInvalid)
         {
            this.Refresh();
         }
      }

      /// <summary>
      /// Checks if the <paramref name="newSelectionDate"/> is within bounds of the <paramref name="baseDate"/>
      /// and the <see cref="MaxSelectionCount"/>.
      /// </summary>
      /// <param name="baseDate">The base date from where to check.</param>
      /// <param name="newSelectionDate">The new selection date.</param>
      /// <returns>A valid new selection date if valid parameters, otherwise <c>DateTime.MinValue</c>.</returns>
      private DateTime GetSelectionDate(DateTime baseDate, DateTime newSelectionDate)
      {
         if (this.maxSelectionCount == 0 || baseDate == DateTime.MinValue)
         {
            return newSelectionDate;
         }

         if (baseDate >= this.CultureCalendar.MinSupportedDateTime && newSelectionDate >= this.CultureCalendar.MinSupportedDateTime
            && baseDate <= this.CultureCalendar.MaxSupportedDateTime && newSelectionDate <= this.CultureCalendar.MaxSupportedDateTime)
         {
            int days = (baseDate - newSelectionDate).Days;

            if (Math.Abs(days) >= this.maxSelectionCount)
            {

               newSelectionDate =
                  new MonthCalendarDate(this.CultureCalendar, baseDate).AddDays(days < 0
                                                                                ? this.maxSelectionCount - 1
                                                                                : 1 - this.maxSelectionCount).Date;
            }

            return newSelectionDate;
         }

         return DateTime.MinValue;
      }

      /// <summary>
      /// Returns the minimum date for the control.
      /// </summary>
      /// <param name="date">The date to set as min date.</param>
      /// <returns>The min date.</returns>
      private DateTime GetMinDate(DateTime date)
      {
         DateTime dt = new DateTime(1900, 1, 1);
         DateTime minEra = this.GetEraRange().MinDate;

         // bug in JapaneseLunisolarCalendar - JapaneseLunisolarCalendar.GetYear() with date parameter
         // between 1989/1/8 and 1989/2/6 returns 0 therefore make sure, the calendar
         // can display date range if ViewStart set to min date
         if (this.cultureCalendar.GetType() == typeof(JapaneseLunisolarCalendar))
         {
            minEra = new DateTime(1989, 4, 1);
         }

         DateTime mindate = minEra < dt ? dt : minEra;

         return date < mindate ? mindate : date;
      }

      /// <summary>
      /// Returns the maximum date for the control.
      /// </summary>
      /// <param name="date">The date to set as max date.</param>
      /// <returns>The max date.</returns>
      private DateTime GetMaxDate(DateTime date)
      {
         DateTime dt = new DateTime(9998, 12, 31);
         DateTime maxEra = this.GetEraRange().MaxDate;

         DateTime maxdate = maxEra > dt ? dt : maxEra;

         return date > maxdate ? maxdate : date;
      }

      /// <summary>
      /// If changing the used calendar, then this method reassigns the selection mode to set the correct selection range.
      /// </summary>
      private void ReAssignSelectionMode()
      {
         this.SelectionRange = null;

         MonthCalendarSelectionMode selMode = this.daySelectionMode;

         this.daySelectionMode = MonthCalendarSelectionMode.Manual;

         this.SelectionMode = selMode;
      }

      /// <summary>
      /// Sets the selection range for the specified <see cref="MonthCalendarSelectionMode"/>.
      /// </summary>
      /// <param name="selMode">The <see cref="MonthCalendarSelectionMode"/> value to set the selection range for.</param>
      private void SetSelectionRange(MonthCalendarSelectionMode selMode)
      {
         switch (selMode)
         {
            case MonthCalendarSelectionMode.Day:
               {
                  this.selectionEnd = this.selectionStart;

                  break;
               }

            case MonthCalendarSelectionMode.WorkWeek:
            case MonthCalendarSelectionMode.FullWeek:
               {
                   /*
                  MonthCalendarDate dt =
                     new MonthCalendarDate(this.CultureCalendar, this.selectionStart).GetFirstDayInWeek(
                        this.formatProvider);
                  this.selectionStart = dt.Date;
                  this.selectionEnd = dt.AddDays(6).Date;*/

                  break;
               }

            case MonthCalendarSelectionMode.Month:
               {
                   /*
                  MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.selectionStart).FirstOfMonth;
                  this.selectionStart = dt.Date;
                  this.selectionEnd = dt.AddMonths(1).AddDays(-1).Date;*/

                  break;
               }
         }
      }

      #region private design time methods

      /// <summary>
      /// Returns whether the property <see cref="Culture"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      internal bool ShouldSerializeCulture()
      {
         return this.culture.LCID != CultureInfo.CurrentUICulture.LCID;
      }

      /// <summary>
      /// Resets the property <see cref="Culture"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      internal void ResetCulture()
      {
         this.minDate = CultureInfo.CurrentUICulture.DateTimeFormat.Calendar.MinSupportedDateTime;

         if (this.minDate < new DateTime(1900, 1, 1))
         {
            this.minDate = new DateTime(1900, 1, 1);
         }

         this.maxDate = CultureInfo.CurrentUICulture.DateTimeFormat.Calendar.MaxSupportedDateTime.Date;

         if (this.maxDate > new DateTime(9998, 12, 31))
         {
            this.maxDate = new DateTime(9998, 12, 31);
         }

         this.Culture = CultureInfo.CurrentUICulture;
      }

      /// <summary>
      /// Returns whether the property <see cref="CultureCalendar"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      internal bool ShouldSerializeCultureCalendar()
      {
         if (this.CultureCalendar.GetType() == this.culture.Calendar.GetType() && this.CultureCalendar.GetType() == typeof(GregorianCalendar))
         {
            GregorianCalendar g1 = (GregorianCalendar)this.CultureCalendar;
            GregorianCalendar g2 = (GregorianCalendar)this.culture.Calendar;

            return this.CultureCalendar != this.culture.Calendar && g1.CalendarType != g2.CalendarType;
         }

         return this.CultureCalendar != this.culture.Calendar && this.CultureCalendar.GetType() != this.culture.Calendar.GetType();
      }

      /// <summary>
      /// Resets the property <see cref="CultureCalendar"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      internal void ResetCultureCalendar()
      {
         this.CultureCalendar = this.culture.Calendar;
      }

      /// <summary>
      /// Returns whether the property <see cref="MinDate"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      internal bool ShouldSerializeMinDate()
      {
         return this.minDate != this.GetMinDate(this.CultureCalendar.MinSupportedDateTime.Date);
      }

      /// <summary>
      /// Resets the property <see cref="MinDate"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      internal void ResetMinDate()
      {
         this.minDate = this.GetMinDate(this.CultureCalendar.MinSupportedDateTime.Date);
      }

      /// <summary>
      /// Returns whether the property <see cref="MaxDate"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      internal bool ShouldSerializeMaxDate()
      {
         return this.maxDate != this.GetMaxDate(this.CultureCalendar.MaxSupportedDateTime.Date);
      }

      /// <summary>
      /// Resets the property <see cref="MaxDate"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      internal void ResetMaxDate()
      {
         this.maxDate = this.GetMaxDate(this.CultureCalendar.MaxSupportedDateTime.Date);
      }

      /// <summary>
      /// Returns whether the property <see cref="ColorTable"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      internal bool ShouldSerializeColorTable()
      {
         MonthCalendarColorTable table = new MonthCalendarColorTable();
         MonthCalendarColorTable currentTable = this.ColorTable;

         return table.BackgroundGradientBegin != currentTable.BackgroundGradientBegin
            || table.BackgroundGradientEnd != currentTable.BackgroundGradientEnd
            || table.BackgroundGradientMode != currentTable.BackgroundGradientMode
            || table.Border != currentTable.Border
            || table.DayActiveGradientBegin != currentTable.DayActiveGradientBegin
            || table.DayActiveGradientEnd != currentTable.DayActiveGradientEnd
            || table.DayActiveGradientMode != currentTable.DayActiveGradientMode
            || table.DayActiveText != currentTable.DayActiveText
            || table.DayActiveTodayCircleBorder != currentTable.DayActiveTodayCircleBorder
            || table.DayHeaderGradientBegin != currentTable.DayHeaderGradientBegin
            || table.DayHeaderGradientEnd != currentTable.DayHeaderGradientEnd
            || table.DayHeaderGradientMode != currentTable.DayHeaderGradientMode
            || table.DayHeaderText != currentTable.DayHeaderText
            || table.DaySelectedGradientBegin != currentTable.DaySelectedGradientBegin
            || table.DaySelectedGradientEnd != currentTable.DaySelectedGradientEnd
            || table.DaySelectedGradientMode != currentTable.DaySelectedGradientMode
            || table.DaySelectedText != currentTable.DaySelectedText
            || table.DaySelectedTodayCircleBorder != currentTable.DaySelectedTodayCircleBorder
            || table.DayText != currentTable.DayText
            || table.DayTextBold != currentTable.DayTextBold
            || table.DayTodayCircleBorder != currentTable.DayTodayCircleBorder
            || table.DayTrailingText != currentTable.DayTrailingText
            || table.FooterActiveGradientBegin != currentTable.FooterActiveGradientBegin
            || table.FooterActiveGradientEnd != currentTable.FooterActiveGradientEnd
            || table.FooterActiveGradientMode != currentTable.FooterActiveGradientMode
            || table.FooterActiveText != currentTable.FooterActiveText
            || table.FooterGradientBegin != currentTable.FooterGradientBegin
            || table.FooterGradientEnd != currentTable.FooterGradientEnd
            || table.FooterGradientMode != currentTable.FooterGradientMode
            || table.FooterText != currentTable.FooterText
            || table.FooterTodayCircleBorder != currentTable.FooterTodayCircleBorder
            || table.HeaderActiveArrow != currentTable.HeaderActiveArrow
            || table.HeaderActiveGradientBegin != currentTable.HeaderActiveGradientBegin
            || table.HeaderActiveGradientEnd != currentTable.HeaderActiveGradientEnd
            || table.HeaderActiveGradientMode != currentTable.HeaderActiveGradientMode
            || table.HeaderActiveText != currentTable.HeaderActiveText
            || table.HeaderArrow != currentTable.HeaderArrow
            || table.HeaderGradientBegin != currentTable.HeaderGradientBegin
            || table.HeaderGradientEnd != currentTable.HeaderGradientEnd
            || table.HeaderGradientMode != currentTable.HeaderGradientMode
            || table.HeaderSelectedText != currentTable.HeaderSelectedText
            || table.HeaderText != currentTable.HeaderText
            || table.MonthBodyGradientBegin != currentTable.MonthBodyGradientBegin
            || table.MonthBodyGradientEnd != currentTable.MonthBodyGradientEnd
            || table.MonthBodyGradientMode != currentTable.MonthBodyGradientMode
            || table.MonthSeparator != currentTable.MonthSeparator
            || table.WeekHeaderGradientBegin != currentTable.WeekHeaderGradientBegin
            || table.WeekHeaderGradientEnd != currentTable.WeekHeaderGradientEnd
            || table.WeekHeaderGradientMode != currentTable.WeekHeaderGradientMode
            || table.WeekHeaderText != currentTable.WeekHeaderText;
      }

      /// <summary>
      /// Resets the property <see cref="ColorTable"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      internal void ResetColorTable()
      {
         this.ColorTable = new MonthCalendarColorTable();

         this.Invalidate();
      }

      /// <summary>
      /// Returns whether the property <see cref="ViewStart"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeViewStart()
      {
         return this.viewStart != new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      }

      /// <summary>
      /// Resets the property <see cref="ViewStart"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetViewStart()
      {
         this.ViewStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      }

      /// <summary>
      /// Returns whether the property <see cref="HeaderFont"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeHeaderFont()
      {
         return this.headerFont.Name != "Segoe UI"
            || !this.headerFont.Size.Equals(9f)
            || this.headerFont.Style != FontStyle.Regular;
      }

      /// <summary>
      /// Resets the property <see cref="HeaderFont"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetHeaderFont()
      {
         this.HeaderFont = new Font("Segoe UI", 9f, FontStyle.Regular);
      }

      /// <summary>
      /// Returns whether the property <see cref="FooterFont"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeFooterFont()
      {
         return this.footerFont.Name != "Arial"
            || !this.footerFont.Size.Equals(9f)
            || this.footerFont.Style != FontStyle.Bold;
      }

      /// <summary>
      /// Resets the property <see cref="FooterFont"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetFooterFont()
      {
         this.FooterFont = new Font("Arial", 9f, FontStyle.Bold);
      }

      /// <summary>
      /// Returns whether the property <see cref="DayHeaderFont"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeDayHeaderFont()
      {
         return this.dayHeaderFont.Name != "Segoe UI"
            || !this.dayHeaderFont.Size.Equals(8f)
            || this.dayHeaderFont.Style != FontStyle.Regular;
      }

      /// <summary>
      /// Resets the property <see cref="DayHeaderFont"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetDayHeaderFont()
      {
         this.DayHeaderFont = new Font("Segoe UI", 8f, FontStyle.Regular);
      }

      /// <summary>
      /// Returns whether the property <see cref="Font"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeFont()
      {
         return this.Font.Name != "Microsoft Sans Serif"
            || !this.Font.Size.Equals(8.25f)
            || this.Font.Style != FontStyle.Regular;
      }

      /// <summary>
      /// Resets the property <see cref="Font"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private new void ResetFont()
      {
         this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
      }

      /// <summary>
      /// Returns whether the property <see cref="BoldedDates"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeBoldedDates()
      {
         return this.boldedDates.Count != 0;
      }

      /// <summary>
      /// Resets the property <see cref="BoldedDates"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetBoldedDates()
      {
         this.boldedDates.Clear();

         this.Refresh();
      }

      /// <summary>
      /// Returns whether the property <see cref="SelectionRange"/> should be serialized.
      /// </summary>
      /// <returns>true or false.</returns>
      /// <remarks>Only used by the designer.</remarks>
      private bool ShouldSerializeSelectionRange()
      {
         return this.selectionStart != DateTime.Today
            || this.selectionStart != this.selectionEnd;
      }

      /// <summary>
      /// Resets the property <see cref="SelectionRange"/>.
      /// </summary>
      /// <remarks>Only used by the designer.</remarks>
      private void ResetSelectionRange()
      {
         //this.selectionEnd = DateTime.Today;
         //this.selectionStart = DateTime.Today;

          this.SelectionRange = new SelectionRange();

         this.Refresh();
      }

      /// <summary>
      /// Initializes the context menus.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new Container();
         this.monthMenu = new ContextMenuStrip(this.components);
         this.tsmiJan = new ToolStripMenuItem();
         this.tsmiFeb = new ToolStripMenuItem();
         this.tsmiMar = new ToolStripMenuItem();
         this.tsmiApr = new ToolStripMenuItem();
         this.tsmiMay = new ToolStripMenuItem();
         this.tsmiJun = new ToolStripMenuItem();
         this.tsmiJul = new ToolStripMenuItem();
         this.tsmiAug = new ToolStripMenuItem();
         this.tsmiSep = new ToolStripMenuItem();
         this.tsmiOct = new ToolStripMenuItem();
         this.tsmiNov = new ToolStripMenuItem();
         this.tsmiDez = new ToolStripMenuItem();
         this.tsmiA1 = new ToolStripMenuItem();
         this.tsmiA2 = new ToolStripMenuItem();
         this.yearMenu = new ContextMenuStrip(this.components);
         this.tsmiYear1 = new ToolStripMenuItem();
         this.tsmiYear2 = new ToolStripMenuItem();
         this.tsmiYear3 = new ToolStripMenuItem();
         this.tsmiYear4 = new ToolStripMenuItem();
         this.tsmiYear5 = new ToolStripMenuItem();
         this.tsmiYear6 = new ToolStripMenuItem();
         this.tsmiYear7 = new ToolStripMenuItem();
         this.tsmiYear8 = new ToolStripMenuItem();
         this.tsmiYear9 = new ToolStripMenuItem();
         this.monthMenu.SuspendLayout();
         this.yearMenu.SuspendLayout();
         this.SuspendLayout();
         // 
         // monthMenu
         // 
         this.monthMenu.Items.AddRange(new ToolStripItem[] {
            this.tsmiJan,
            this.tsmiFeb,
            this.tsmiMar,
            this.tsmiApr,
            this.tsmiMay,
            this.tsmiJun,
            this.tsmiJul,
            this.tsmiAug,
            this.tsmiSep,
            this.tsmiOct,
            this.tsmiNov,
            this.tsmiDez,
            this.tsmiA1,
            this.tsmiA2});
         this.monthMenu.Name = "monthMenu";
         this.monthMenu.ShowImageMargin = false;
         this.monthMenu.Size = new Size(54, 312);
         this.monthMenu.Closed += this.MonthMenuClosed;
         // 
         // tsmiJan
         // 
         this.tsmiJan.Size = new Size(78, 22);
         this.tsmiJan.Tag = 1;
         this.tsmiJan.Click += this.MonthClick;
         // 
         // tsmiFeb
         // 
         this.tsmiFeb.Size = new Size(78, 22);
         this.tsmiFeb.Tag = 2;
         this.tsmiFeb.Click += this.MonthClick;
         // 
         // tsmiMar
         // 
         this.tsmiMar.Size = new Size(78, 22);
         this.tsmiMar.Tag = 3;
         this.tsmiMar.Click += this.MonthClick;
         // 
         // tsmiApr
         // 
         this.tsmiApr.Size = new Size(78, 22);
         this.tsmiApr.Tag = 4;
         this.tsmiApr.Click += this.MonthClick;
         // 
         // tsmiMay
         // 
         this.tsmiMay.Size = new Size(78, 22);
         this.tsmiMay.Tag = 5;
         this.tsmiMay.Click += this.MonthClick;
         // 
         // tsmiJun
         // 
         this.tsmiJun.Size = new Size(78, 22);
         this.tsmiJun.Tag = 6;
         this.tsmiJun.Click += this.MonthClick;
         // 
         // tsmiJul
         // 
         this.tsmiJul.Size = new Size(78, 22);
         this.tsmiJul.Tag = 7;
         this.tsmiJul.Click += this.MonthClick;
         // 
         // tsmiAug
         // 
         this.tsmiAug.Size = new Size(78, 22);
         this.tsmiAug.Tag = 8;
         this.tsmiAug.Click += this.MonthClick;
         // 
         // tsmiSep
         // 
         this.tsmiSep.Size = new Size(78, 22);
         this.tsmiSep.Tag = 9;
         this.tsmiSep.Click += this.MonthClick;
         // 
         // tsmiOct
         // 
         this.tsmiOct.Size = new Size(78, 22);
         this.tsmiOct.Tag = 10;
         this.tsmiOct.Click += this.MonthClick;
         // 
         // tsmiNov
         // 
         this.tsmiNov.Size = new Size(78, 22);
         this.tsmiNov.Tag = 11;
         this.tsmiNov.Click += this.MonthClick;
         // 
         // tsmiDez
         // 
         this.tsmiDez.Size = new Size(78, 22);
         this.tsmiDez.Tag = 12;
         this.tsmiDez.Click += this.MonthClick;
         // 
         // tsmiA1
         // 
         this.tsmiA1.Size = new Size(78, 22);
         this.tsmiA1.Click += this.MonthClick;
         // 
         // tsmiA2
         // 
         this.tsmiA2.Size = new Size(78, 22);
         this.tsmiA2.Click += this.MonthClick;

         // 
         // yearMenu
         // 
         //this.yearMenu.Items.AddRange(new ToolStripItem[] {
         //   this.tsmiYear1,
         //   this.tsmiYear2,
         //   this.tsmiYear3,
         //   this.tsmiYear4,
         //   this.tsmiYear5,
         //   this.tsmiYear6,
         //   this.tsmiYear7,
         //   this.tsmiYear8,
         //   this.tsmiYear9});
         this.yearMenu.Name = "yearMenu";
         this.yearMenu.ShowImageMargin = false;
         this.yearMenu.ShowItemToolTips = false;
         this.yearMenu.Size = new Size(54, 202);
         this.yearMenu.Closed += this.YearMenuClosed;

         // 
         // tsmiYear1
         // 
         this.tsmiYear1.Size = new Size(53, 22);
         this.tsmiYear1.Click += this.YearClick;
         // 
         // tsmiYear2
         // 
         this.tsmiYear2.Size = new Size(53, 22);
         this.tsmiYear2.Click += this.YearClick;
         // 
         // tsmiYear3
         // 
         this.tsmiYear3.Size = new Size(53, 22);
         this.tsmiYear3.Click += this.YearClick;
         // 
         // tsmiYear4
         // 
         this.tsmiYear4.Size = new Size(53, 22);
         this.tsmiYear4.Click += this.YearClick;
         // 
         // tsmiYear5
         // 
         this.tsmiYear5.Size = new Size(53, 22);
         this.tsmiYear5.Click += this.YearClick;
         // 
         // tsmiYear6
         // 
         this.tsmiYear6.Size = new Size(53, 22);
         this.tsmiYear6.Click += this.YearClick;
         // 
         // tsmiYear7
         // 
         this.tsmiYear7.Size = new Size(53, 22);
         this.tsmiYear7.Click += this.YearClick;
         // 
         // tsmiYear8
         // 
         this.tsmiYear8.Size = new Size(53, 22);
         this.tsmiYear8.Click += this.YearClick;
         // 
         // tsmiYear9
         // 
         this.tsmiYear9.Size = new Size(53, 22);
         this.tsmiYear9.Click += this.YearClick;

         this.monthMenu.ResumeLayout(false);
         this.yearMenu.ResumeLayout(false);
         this.ResumeLayout(false);
      }

      #endregion

      #endregion

      #endregion
   }
}