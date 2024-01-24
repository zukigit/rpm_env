namespace CustomControls
{
   using System;
   using System.Collections.Generic;
   using System.Drawing;
   using System.Drawing.Drawing2D;
   using System.Globalization;
   using System.Windows.Forms;

   /// <summary>
   /// The MonthCalendar control renderer.
   /// </summary>
   public class MonthCalendarRenderer : MonthCalendarAbstractRenderer
   {
      #region fields

      /// <summary>
      /// The month calendar control.
      /// </summary>
      private readonly MonthCalendar monthCal;

      #endregion

      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarRenderer"/> class.
      /// </summary>
      /// <param name="calendar">The corresponding <see cref="MonthCalendar"/>.</param>
      public MonthCalendarRenderer(MonthCalendar calendar)
         : base(calendar)
      {
         this.monthCal = calendar;
      }

      #endregion

      #region properties

      /// <summary>
      /// Gets the corresponding <see cref="MonthCalendar"/>.
      /// </summary>
      public MonthCalendar MonthCalendar
      {
         get { return this.monthCal; }
      }

      #endregion

      #region methods

      /// <summary>
      /// Draws the header of a <see cref="MonthCalendarMonth"/>.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="calMonth">The <see cref="MonthCalendarMonth"/> to draw the header for.</param>
      /// <param name="state">The <see cref="MonthCalendarHeaderState"/>.</param>
      public override void DrawMonthHeader(
         Graphics g,
         MonthCalendarMonth calMonth,
         MonthCalendarHeaderState state)
      {
         if (calMonth == null || !CheckParams(g, calMonth.TitleBounds))
         {
            return;
         }

         // get title bounds
         Rectangle rect = calMonth.TitleBounds;

         MonthCalendarDate date = new MonthCalendarDate(monthCal.CultureCalendar, calMonth.Date);
         MonthCalendarDate firstVisible = new MonthCalendarDate(monthCal.CultureCalendar, calMonth.FirstVisibleDate);

         string month;
         int year;

         // gets the month name for the month the MonthCalendarMonth represents and the year string
         if (firstVisible.Era != date.Era)
         {
            month = this.monthCal.FormatProvider.GetMonthName(firstVisible.Year, firstVisible.Month);
            year = firstVisible.Year;
         }
         else
         {
            month = this.monthCal.FormatProvider.GetMonthName(date.Year, date.Month);
            year = date.Year;
         }

         string yearString = this.monthCal.UseNativeDigits
            ? DateMethods.GetNativeNumberString(year, this.monthCal.Culture.NumberFormat.NativeDigits, false)
            : year.ToString(CultureInfo.CurrentUICulture);

         // get used font
         Font headerFont = this.monthCal.HeaderFont;

         // create bold font
         Font boldFont = new Font(headerFont.FontFamily, headerFont.SizeInPoints, FontStyle.Bold);

         // measure sizes
         SizeF monthSize = g.MeasureString(month, boldFont);

         SizeF yearSize = g.MeasureString(yearString, boldFont);

         float maxHeight = Math.Max(monthSize.Height, yearSize.Height);

         // calculates the width and the starting position of the arrows
         int width = (int)monthSize.Width + (int)yearSize.Width + 7;
         int arrowLeftX = rect.X + 6;
         int arrowRightX = rect.Right - 6;
         int arrowY = rect.Y + (rect.Height / 2) - 4;

         int x = Math.Max(0, rect.X + (rect.Width / 2) + 1 - (width / 2));
         int y = Math.Max(
            0,
            rect.Y + (rect.Height / 2) + 1 - (((int)maxHeight + 1) / 2));

         // set the title month name bounds
         calMonth.TitleMonthBounds = new Rectangle(
            x,
            y,
            (int)monthSize.Width + 1,
            (int)maxHeight + 1);

         // set the title year bounds
         calMonth.TitleYearBounds = new Rectangle(
            x + calMonth.TitleMonthBounds.Width + 7,
            y,
            (int)yearSize.Width + 1,
            (int)maxHeight + 1);

         // generate points for the left and right arrow
         Point[] arrowLeft = new[]
         {
            new Point(arrowLeftX, arrowY + 4),
            new Point(arrowLeftX + 4, arrowY),
            new Point(arrowLeftX + 4, arrowY + 8),
            new Point(arrowLeftX, arrowY + 4)
         };

         Point[] arrowRight = new[]
         {
            new Point(arrowRightX, arrowY + 4),
            new Point(arrowRightX - 4, arrowY),
            new Point(arrowRightX - 4, arrowY + 8),
            new Point(arrowRightX, arrowY + 4)
         };

         // get color table
         MonthCalendarColorTable colorTable = this.ColorTable;

         // get brushes for normal, mouse over and selected state
         using (SolidBrush brush = new SolidBrush(colorTable.HeaderText),
            brushOver = new SolidBrush(colorTable.HeaderActiveText),
            brushSelected = new SolidBrush(colorTable.HeaderSelectedText))
         {
            // get title month name and year bounds
            Rectangle monthRect = calMonth.TitleMonthBounds;
            Rectangle yearRect = calMonth.TitleYearBounds;

            // set used fonts
            Font monthFont = headerFont;
            Font yearFont = headerFont;

            // set used brushes
            SolidBrush monthBrush = brush, yearBrush = brush;

            // adjust brush and font if year selected
            if (state == MonthCalendarHeaderState.YearSelected)
            {
               yearBrush = brushSelected;
               yearFont = boldFont;
               yearRect.Width += 4;
            }
            else if (state == MonthCalendarHeaderState.YearActive)
            {
               // adjust brush if mouse over year
               yearBrush = brushOver;
            }

            // adjust brush and font if month name is selected
            if (state == MonthCalendarHeaderState.MonthNameSelected)
            {
               monthBrush = brushSelected;
               monthFont = boldFont;
               monthRect.Width += 4;
            }
            else if (state == MonthCalendarHeaderState.MonthNameActive)
            {
               // adjust brush if mouse over month name
               monthBrush = brushOver;
            }

            // draws the month name and year string
            g.DrawString(month, monthFont, monthBrush, monthRect);
            g.DrawString(yearString, yearFont, yearBrush, yearRect);
         }

         boldFont.Dispose();

         // if left arrow has to be drawn
         //if (calMonth.DrawLeftButton)
         //{
         //   // get arrow color
         //   Color arrowColor = this.monthCal.LeftButtonState == ButtonState.Normal ?
         //      GetGrayColor(this.monthCal.Enabled, colorTable.HeaderArrow) : colorTable.HeaderActiveArrow;

         //   // set left arrow rect
         //   this.monthCal.SetLeftArrowRect(new Rectangle(rect.X, rect.Y, 15, rect.Height));

         //   // draw left arrow
         //   using (GraphicsPath path = new GraphicsPath())
         //   {
         //      path.AddLines(arrowLeft);

         //      using (SolidBrush brush = new SolidBrush(arrowColor))
         //      {
         //         g.FillPath(brush, path);
         //      }

         //      using (Pen p = new Pen(arrowColor))
         //      {
         //         g.DrawPath(p, path);
         //      }
         //   }
         //}

         // if right arrow has to be drawn
      //   if (calMonth.DrawRightButton)
      //   {
      //      // get arrow color
      //      Color arrowColor = this.monthCal.RightButtonState == ButtonState.Normal ?
      //         GetGrayColor(this.monthCal.Enabled, colorTable.HeaderArrow) : colorTable.HeaderActiveArrow;

      //      // set right arrow rect
      //      this.monthCal.SetRightArrowRect(new Rectangle(rect.Right - 15, rect.Y, 15, rect.Height));

      //      // draw arrow
      //      using (GraphicsPath path = new GraphicsPath())
      //      {
      //         path.AddLines(arrowRight);

      //         using (SolidBrush brush = new SolidBrush(arrowColor))
      //         {
      //            g.FillPath(brush, path);
      //         }

      //         using (Pen p = new Pen(arrowColor))
      //         {
      //            g.DrawPath(p, path);
      //         }
      //      }
      //   }
      }

      /// <summary>
      /// Draws a day in the month body of the calendar control.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="day">The <see cref="MonthCalendarDay"/> to draw.</param>
      public override void DrawDay(Graphics g, MonthCalendarDay day)
      {
         if (!CheckParams(g, day.Bounds))
         {
            return;
         }

         // get color table
         MonthCalendarColorTable colors = this.ColorTable;

         // get the bounds of the day
         Rectangle rect = day.Bounds;

         var boldDate = this.monthCal.BoldedDatesCollection.Find(d => d.Value.Date == day.Date.Date);

         // if day is selected or in mouse over state
         if (day.MouseOver)
         {
            FillBackground(
               g,
               rect,
               colors.DayActiveGradientBegin,
               colors.DayActiveGradientEnd,
               colors.DayActiveGradientMode);
         }
         else if (day.Selected)
         {
            this.FillBackgroundInternal(
               g,
               rect,
               colors.DaySelectedGradientBegin,
               colors.DaySelectedGradientEnd,
               colors.DaySelectedGradientMode);
         }
         else if (!boldDate.IsEmpty && boldDate.Category.BackColorStart != Color.Empty && boldDate.Category.BackColorStart != Color.Transparent)
         {
            FillBackground(
               g,
               rect,
               boldDate.Category.BackColorStart,
               boldDate.Category.BackColorEnd.IsEmpty || boldDate.Category.BackColorEnd == Color.Transparent ? boldDate.Category.BackColorStart : boldDate.Category.BackColorEnd,
               boldDate.Category.GradientMode);
         }

         // get bolded dates
         List<DateTime> boldedDates = this.monthCal.GetBoldedDates();

         bool bold = boldedDates.Contains(day.Date) || !boldDate.IsEmpty;

         // draw the day
         using (StringFormat format = GetStringAlignment(this.monthCal.DayTextAlignment))
         {
            Color textColor = bold ? (boldDate.IsEmpty || boldDate.Category.ForeColor == Color.Empty || boldDate.Category.ForeColor == Color.Transparent ? colors.DayTextBold : boldDate.Category.ForeColor)
               : (day.Selected ? colors.DaySelectedText
               : (day.MouseOver ? colors.DayActiveText
               : (day.TrailingDate ? colors.DayTrailingText
               : colors.DayText)));

            using (SolidBrush brush = new SolidBrush(textColor))
            {
               using (Font font = new Font(
                  this.monthCal.Font.FontFamily,
                  this.monthCal.Font.SizeInPoints,
                  FontStyle.Bold))
               {
                  // adjust width
                  Rectangle textRect = day.Bounds;
                  textRect.Width -= 2;

                  // determine if to use bold font
                  //bool useBoldFont = day.Date == DateTime.Today || bold;
                  bool useBoldFont = bold;

                  var calDate = new MonthCalendarDate(monthCal.CultureCalendar, day.Date);

                  string dayString = this.monthCal.UseNativeDigits
                                        ? DateMethods.GetNativeNumberString(calDate.Day, this.monthCal.Culture.NumberFormat.NativeDigits, false)
                                        : calDate.Day.ToString(this.monthCal.Culture);

                  //if (!day.TrailingDate)
                  //{

                      if (this.monthCal.Enabled)
                      {
                          g.DrawString(
                             dayString,
                             (useBoldFont ? font : this.monthCal.Font),
                             brush,
                             textRect,
                             format);
                      }
                      else
                      {
                          ControlPaint.DrawStringDisabled(
                             g,
                             dayString,
                             (useBoldFont ? font : this.monthCal.Font),
                             Color.Transparent,
                             textRect,
                             format);
                      }
                  //}
               }
            }
         }

         // if today, draw border
         //if (day.Date == DateTime.Today)
         //{
         //   rect.Height -= 1;
         //   rect.Width -= 2;
         //   Color borderColor = day.Selected ? colors.DaySelectedTodayCircleBorder
         //      : (day.MouseOver ? colors.DayActiveTodayCircleBorder : colors.DayTodayCircleBorder);

         //   using (Pen p = new Pen(borderColor))
         //   {
         //      g.DrawRectangle(p, rect);

         //      rect.Offset(1, 0);

         //      g.DrawRectangle(p, rect);
         //   }
         //}
      }

      /// <summary>
      /// Draws the day header.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
      public override void DrawDayHeader(Graphics g, Rectangle rect)
      {
         // get day width
         int dayWidth = this.monthCal.DaySize.Width;

         if (!CheckParams(g, rect) || dayWidth <= 0)
         {
            return;
         }

         // get abbreviated day names
         List<string> names = new List<string>(DateMethods.GetDayNames(this.monthCal.FormatProvider, this.monthCal.UseShortestDayNames ? 2 : 1));

         // if in RTL mode, reverse order
         if (this.monthCal.UseRTL)
         {
            names.Reverse();
         }

         // get bounds for a single element
         Rectangle dayRect = rect;
         dayRect.Width = dayWidth;

         // draw day names
         using (StringFormat format =
            new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap)
            {
               Alignment = StringAlignment.Center,
               LineAlignment = StringAlignment.Center
            })
         {
            using (SolidBrush brush = new SolidBrush(this.ColorTable.DayHeaderText))
            {
               names.ForEach(day =>
                  {
                     if (this.monthCal.Enabled)
                     {
                        g.DrawString(day, this.monthCal.DayHeaderFont, brush, dayRect, format);
                     }
                     else
                     {
                        ControlPaint.DrawStringDisabled(
                           g,
                           day,
                           this.monthCal.DayHeaderFont,
                           Color.Transparent,
                           dayRect,
                           format);
                     }

                     dayRect.X += dayWidth;
                  });
            }
         }

         // draw separator line
         using (Pen p = new Pen(GetGrayColor(this.monthCal.Enabled, this.ColorTable.MonthSeparator)))
         {
            g.DrawLine(p, rect.X, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
         }
      }

      /// <summary>
      /// Draws the footer.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
      /// <param name="active">true if the footer is in mouse over state; otherwise false.</param>
      public override void DrawFooter(Graphics g, Rectangle rect, bool active)
      {
         if (!CheckParams(g, rect))
         {
            return;
         }

         string dateString = new MonthCalendarDate(this.monthCal.CultureCalendar, DateTime.Today).ToString(
            null,
            null,
            this.monthCal.FormatProvider,
            this.monthCal.UseNativeDigits ? this.monthCal.Culture.NumberFormat.NativeDigits : null);

         // get date size
         SizeF dateSize = g.MeasureString(dateString, this.monthCal.FooterFont);

         // get today rectangle and adjust position
         Rectangle todayRect = rect;
         todayRect.X += 2;

         // if in RTL mode, adjust position
         if (this.monthCal.UseRTL)
         {
            todayRect.X = rect.Right - 20;
         }

         // adjust bounds of today rectangle
         todayRect.Y = rect.Y + (rect.Height / 2) - 5;
         todayRect.Height = 11;
         todayRect.Width = 18;

         // draw the today rectangle
         using (Pen p = new Pen(this.ColorTable.FooterTodayCircleBorder))
         {
            g.DrawRectangle(p, todayRect);
         }

         // get top position to draw the text at
         int y = rect.Y + (rect.Height / 2) - ((int)dateSize.Height / 2);

         Rectangle dateRect;

         // if in RTL mode
         if (this.monthCal.UseRTL)
         {
            // get date bounds
            dateRect = new Rectangle(
               rect.X + 1,
               y,
               todayRect.Left - rect.X,
               (int)dateSize.Height + 1);
         }
         else
         {
            // get date bounds
            dateRect = new Rectangle(
               todayRect.Right + 2,
               y,
               rect.Width - todayRect.Width,
               (int)dateSize.Height + 1);
         }

         // draw date string
         using (StringFormat format = GetStringAlignment(this.monthCal.UseRTL ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft))
         {
            using (SolidBrush brush = new SolidBrush(active ? this.ColorTable.FooterActiveText
               : GetGrayColor(this.monthCal.Enabled, this.ColorTable.FooterText)))
            {
               g.DrawString(dateString, this.monthCal.FooterFont, brush, dateRect, format);
            }
         }
      }

      /// <summary>
      /// Draws a single week header element.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
      /// <param name="week">The <see cref="MonthCalendarWeek"/> to draw.</param>
      public override void DrawWeekHeaderItem(Graphics g, MonthCalendarWeek week)
      {
         if (!CheckParams(g, week.Bounds))
         {
            return;
         }

         var weekString = this.monthCal.UseNativeDigits
            ? DateMethods.GetNativeNumberString(week.WeekNumber, this.monthCal.Culture.NumberFormat.NativeDigits, false)
            : week.WeekNumber.ToString(CultureInfo.CurrentUICulture);

         // draw week header element
         using (StringFormat format = GetStringAlignment(this.monthCal.DayTextAlignment))
         {
            // set alignment
            format.Alignment = StringAlignment.Center;

            // draw string
            using (SolidBrush brush = new SolidBrush(this.ColorTable.WeekHeaderText))
            {
               if (this.monthCal.Enabled)
               {
                  g.DrawString(
                     weekString,
                     this.monthCal.Font,
                     brush,
                     week.Bounds,
                     format);
               }
               else
               {
                  ControlPaint.DrawStringDisabled(
                     g,
                     weekString,
                     this.monthCal.Font,
                     Color.Transparent,
                     week.Bounds,
                     format);
               }
            }
         }
      }

      /// <summary>
      /// Draws the separator line between week header and month body.
      /// </summary>
      /// <param name="g">The <see cref="Graphics"/> used to draw.</param>
      /// <param name="rect">The bounds of the week header.</param>
      public override void DrawWeekHeaderSeparator(Graphics g, Rectangle rect)
      {
         if (!CheckParams(g, rect) || !this.monthCal.ShowWeekHeader)
         {
            return;
         }

         // draw separator line
         using (Pen p = new Pen(GetGrayColor(this.monthCal.Enabled, this.ColorTable.MonthSeparator)))
         {
            if (this.monthCal.UseRTL)
            {
               g.DrawLine(p, rect.X, rect.Y - 1, rect.X, rect.Bottom - 1);
            }
            else
            {
               g.DrawLine(p, rect.Right - 1, rect.Y - 1, rect.Right - 1, rect.Bottom - 1);
            }
         }
      }

      #endregion
   }
}