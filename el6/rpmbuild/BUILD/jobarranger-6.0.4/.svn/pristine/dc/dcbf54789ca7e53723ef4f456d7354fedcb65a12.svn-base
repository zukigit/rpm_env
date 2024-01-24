namespace CustomControls
{
   using System;
   using System.Drawing;
   using System.Drawing.Drawing2D;

   /// <summary>
   /// The base renderer class for the <see cref="MonthCalendar"/><c>.</c><see cref="MonthCalendar.Renderer"/>.
   /// </summary>
   public abstract class MonthCalendarAbstractRenderer
   {
      #region fields

      /// <summary>
      /// The corresponding <see cref="MonthCalendar"/>.
      /// </summary>
      private readonly MonthCalendar calendar;

      #endregion

      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarAbstractRenderer"/> class.
      /// </summary>
      /// <param name="calendar">The corresponding <see cref="MonthCalendar"/>.</param>
      /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is null.</exception>
      protected MonthCalendarAbstractRenderer(MonthCalendar calendar)
      {
         if (calendar == null)
         {
            throw new ArgumentNullException("calendar", "Parameter 'calendar' cannot be null.");
         }

         this.calendar = calendar;

         this.ColorTable = new MonthCalendarColorTable();
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets the associated <see cref="MonthCalendar"/> control.
      /// </summary>
      public MonthCalendar Calendar
      {
         get { return this.calendar; }
      }

      /// <summary>
      /// Gets or sets the used color table.
      /// </summary>
      public MonthCalendarColorTable ColorTable { get; set; }

      #endregion

      #region methods

      #region static methods

      /// <summary>
      /// Fills the specified <paramref name="path"/> either with a gradient background or a solid one.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="path">The <see cref="GraphicsPath"/> to fill.</param>
      /// <param name="colorStart">The start color.</param>
      /// <param name="colorEnd">The end color if drawing a gradient background.</param>
      /// <param name="mode">The <see cref="LinearGradientMode"/> to use.</param>
      /// <exception cref="ArgumentNullException">If <paramref name="path"/> is null.</exception>
      public static void FillBackground(
         Graphics g,
         GraphicsPath path,
         Color colorStart,
         Color colorEnd,
         LinearGradientMode? mode)
      {
         if (path == null)
         {
            throw new ArgumentNullException("path", "parameter 'path' cannot be null.");
         }

         RectangleF rect = path.GetBounds();

         if (!CheckParams(g, path.GetBounds()) || colorStart == Color.Empty)
         {
            return;
         }

         if (colorEnd == Color.Empty)
         {
            if (colorStart != Color.Transparent)
            {
               using (SolidBrush brush = new SolidBrush(colorStart))
               {
                  g.FillPath(brush, path);
               }
            }
         }
         else
         {
            rect.Height += 2;
            rect.Y--;

            using (LinearGradientBrush brush = new LinearGradientBrush(
               rect,
               colorStart,
               colorEnd,
               mode.GetValueOrDefault(LinearGradientMode.Vertical)))
            {
               g.FillPath(brush, path);
            }
         }
      }

      /// <summary>
      /// Checks the parameters of methods.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
      /// <returns>true, if all is ok, false otherwise.</returns>
      /// <exception cref="ArgumentNullException">If <paramref name="g"/> is null.</exception>
      protected static bool CheckParams(Graphics g, RectangleF rect)
      {
         if (g == null)
         {
            throw new ArgumentNullException("g");
         }

         if (rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
         {
            return false;
         }

         return true;
      }

      /// <summary>
      /// Gets the string format for the specified <see cref="ContentAlignment"/> value.
      /// </summary>
      /// <param name="align">The text align value.</param>
      /// <returns>A <see cref="StringFormat"/> object.</returns>
      protected static StringFormat GetStringAlignment(ContentAlignment align)
      {
         StringFormat format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap);

         switch (align)
         {
            case ContentAlignment.TopLeft:
               {
                  format.Alignment = StringAlignment.Near;
                  format.LineAlignment = StringAlignment.Near;

                  break;
               }

            case ContentAlignment.TopCenter:
               {
                  format.Alignment = StringAlignment.Center;
                  format.LineAlignment = StringAlignment.Near;

                  break;
               }

            case ContentAlignment.TopRight:
               {
                  format.Alignment = StringAlignment.Far;
                  format.LineAlignment = StringAlignment.Near;

                  break;
               }

            case ContentAlignment.MiddleLeft:
               {
                  format.Alignment = StringAlignment.Near;
                  format.LineAlignment = StringAlignment.Center;

                  break;
               }

            case ContentAlignment.MiddleCenter:
               {
                  format.Alignment = StringAlignment.Center;
                  format.LineAlignment = StringAlignment.Center;

                  break;
               }

            case ContentAlignment.MiddleRight:
               {
                  format.Alignment = StringAlignment.Far;
                  format.LineAlignment = StringAlignment.Center;

                  break;
               }

            case ContentAlignment.BottomLeft:
               {
                  format.Alignment = StringAlignment.Near;
                  format.LineAlignment = StringAlignment.Far;

                  break;
               }

            case ContentAlignment.BottomCenter:
               {
                  format.Alignment = StringAlignment.Center;
                  format.LineAlignment = StringAlignment.Far;

                  break;
               }

            case ContentAlignment.BottomRight:
               {
                  format.Alignment = StringAlignment.Far;
                  format.LineAlignment = StringAlignment.Far;

                  break;
               }
         }

         return format;
      }

      /// <summary>
      /// Fills the specified <paramref name="rect"/> either with a gradient background or a solid one.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="rect">The <see cref="Rectangle"/> to fill.</param>
      /// <param name="colorStart">The start color.</param>
      /// <param name="colorEnd">The end color if drawing a gradient background.</param>
      /// <param name="mode">The <see cref="LinearGradientMode"/> to use.</param>
      protected static void FillBackground(
         Graphics g,
         Rectangle rect,
         Color colorStart,
         Color colorEnd,
         LinearGradientMode? mode)
      {
         using (GraphicsPath path = new GraphicsPath())
         {
            path.AddRectangle(rect);

            FillBackground(g, path, colorStart, colorEnd, mode);
         }
      }

      #endregion

      #region virtual methods

      /// <summary>
      /// Draws the background of the <see cref="MonthCalendar"/>.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <exception cref="ArgumentNullException">If <paramref name="g"/> is null.</exception>
      public virtual void DrawControlBackground(Graphics g)
      {
         if (g == null)
         {
            throw new ArgumentNullException("g");
         }

         FillBackground(
            g,
            this.calendar.ClientRectangle,
            this.ColorTable.BackgroundGradientBegin,
            this.ColorTable.BackgroundGradientEnd,
            this.ColorTable.BackgroundGradientMode);
      }

      /// <summary>
      /// Draws the title background for the specified <paramref name="month"/>.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="month">The <see cref="MonthCalendarMonth"/> to draw the title for.</param>
      /// <param name="state">The <see cref="MonthCalendarHeaderState"/> of the title.</param>
      public virtual void DrawTitleBackground(Graphics g, MonthCalendarMonth month, MonthCalendarHeaderState state)
      {
         if (!CheckParams(g, month.TitleBounds))
         {
            return;
         }

         Color backStart, backEnd;
         LinearGradientMode? mode;

        backStart = this.ColorTable.HeaderGradientBegin;
        backEnd = this.ColorTable.HeaderGradientEnd;
        mode = this.ColorTable.HeaderGradientMode;

         this.FillBackgroundInternal(g, month.TitleBounds, backStart, backEnd, mode);
      }

      /// <summary>
      /// Draws the background of the month body.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> used to draw.</param>
      /// <param name="month">The <see cref="MonthCalendarMonth"/> to draw the body background for.</param>
      public virtual void DrawMonthBodyBackground(Graphics g, MonthCalendarMonth month)
      {
         if (!CheckParams(g, month.MonthBounds))
         {
            return;
         }

         FillBackground(
            g,
            month.MonthBounds,
            this.ColorTable.MonthBodyGradientBegin,
            this.ColorTable.MonthBodyGradientEnd,
            this.ColorTable.MonthBodyGradientMode);
      }

      /// <summary>
      /// Draws the background of the day header.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> used to draw.</param>
      /// <param name="month">The <see cref="MonthCalendarMonth"/> to draw the day header background for.</param>
      public virtual void DrawDayHeaderBackground(Graphics g, MonthCalendarMonth month)
      {
         if (!CheckParams(g, month.DayNamesBounds))
         {
            return;
         }

         FillBackground(
            g,
            month.DayNamesBounds,
            this.ColorTable.DayHeaderGradientBegin,
            this.ColorTable.DayHeaderGradientEnd,
            this.ColorTable.DayHeaderGradientMode);
      }

      /// <summary>
      /// Draws the background of the week header.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="month">The <see cref="MonthCalendarMonth"/> to draw the week header for.</param>
      public virtual void DrawWeekHeaderBackground(Graphics g, MonthCalendarMonth month)
      {
         if (!CheckParams(g, month.WeekBounds))
         {
            return;
         }

         FillBackground(
            g,
            month.WeekBounds,
            this.ColorTable.WeekHeaderGradientBegin,
            this.ColorTable.WeekHeaderGradientEnd,
            this.ColorTable.WeekHeaderGradientMode);
      }

      /// <summary>
      /// Draws the background of the footer.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="footerBounds">The bounds of the footer.</param>
      /// <param name="active">true if the footer is in mouse over state, otherwise false.</param>
      public virtual void DrawFooterBackground(Graphics g, Rectangle footerBounds, bool active)
      {
         if (!CheckParams(g, footerBounds))
         {
            return;
         }

         MonthCalendarColorTable colors = this.ColorTable;

         if (active)
         {
            FillBackground(
               g,
               footerBounds,
               colors.FooterActiveGradientBegin,
               colors.FooterActiveGradientEnd,
               colors.FooterActiveGradientMode);
         }
         else
         {
            FillBackground(
               g,
               footerBounds,
               colors.FooterGradientBegin,
               colors.FooterGradientEnd,
               colors.FooterGradientMode);
         }
      }

      #endregion

      #region abstract methods

      /// <summary>
      /// Draws the header of a <see cref="MonthCalendarMonth"/>.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="calMonth">The <see cref="MonthCalendarMonth"/> to draw the header for.</param>
      /// <param name="state">The <see cref="MonthCalendarHeaderState"/>.</param>
      public abstract void DrawMonthHeader(
         Graphics g,
         MonthCalendarMonth calMonth,
         MonthCalendarHeaderState state);

      /// <summary>
      /// Draws a day in the month body of the calendar control.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="day">The <see cref="MonthCalendarDay"/> to draw.</param>
      public abstract void DrawDay(Graphics g, MonthCalendarDay day);

      /// <summary>
      /// Draws the day names.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
      public abstract void DrawDayHeader(Graphics g, Rectangle rect);

      /// <summary>
      /// Draws the footer.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
      /// <param name="active">true if the footer is in mouse over state; otherwise false.</param>
      public abstract void DrawFooter(Graphics g, Rectangle rect, bool active);

      /// <summary>
      /// Draws a single week header element.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="week">The <see cref="MonthCalendarWeek"/> to draw.</param>
      public abstract void DrawWeekHeaderItem(Graphics g, MonthCalendarWeek week);

      /// <summary>
      /// Draws the separator line between week header and month body.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> used to draw.</param>
      /// <param name="rect">The bounds of the week header.</param>
      public abstract void DrawWeekHeaderSeparator(Graphics g, Rectangle rect);

      #endregion

      #region internal methods

      /// <summary>
      /// Gets the gray scaled color of the specified <paramref name="baseColor"/>.
      /// </summary>
      /// <param name="enabled">true if the control is enabled; false otherwise.</param>
      /// <param name="baseColor">The base color.</param>
      /// <returns>The gray scaled color.</returns>
      internal static Color GetGrayColor(bool enabled, Color baseColor)
      {
         if (baseColor.IsEmpty || enabled)
         {
            return baseColor;
         }

         float lumi = (.3F * baseColor.R) + (.59F * baseColor.G) + (.11F * baseColor.B);

         return Color.FromArgb((int)lumi, (int)lumi, (int)lumi);
      }

      /// <summary>
      /// Fills the specified <paramref name="rect"/> with the given colors.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="rect">The rectangle to fill.</param>
      /// <param name="start">The start color.</param>
      /// <param name="end">The end color.</param>
      /// <param name="mode">The fill mode.</param>
      internal void FillBackgroundInternal(
         Graphics g,
         Rectangle rect,
         Color start,
         Color end,
         LinearGradientMode? mode)
      {
         if (!this.Calendar.Enabled)
         {
            float lumiStart = (.3F * start.R) + (.59F * start.G) + (.11F * start.B);
            float lumiEnd = (.3F * end.R) + (.59F * end.G) + (.11F * end.B);

            if (!start.IsEmpty)
            {
               start = Color.FromArgb((int)lumiStart, (int)lumiStart, (int)lumiStart);
            }

            if (!end.IsEmpty)
            {
               end = Color.FromArgb((int)lumiEnd, (int)lumiEnd, (int)lumiEnd);
            }
         }

         FillBackground(g, rect, start, end, mode);
      }

      #endregion

      #endregion
   }
}