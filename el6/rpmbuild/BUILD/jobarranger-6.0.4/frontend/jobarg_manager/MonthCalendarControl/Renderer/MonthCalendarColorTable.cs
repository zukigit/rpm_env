namespace CustomControls
{
   using System;
   using System.ComponentModel;
   using System.Drawing;
   using System.Drawing.Drawing2D;
   using System.Globalization;
   using System.Xml;
   using System.Xml.Schema;
   using System.Xml.Serialization;

   /// <summary>
   /// Class that hold mainly color information for the <see cref="MonthCalendar"/> renderer.
   /// </summary>
   [TypeConverter(typeof(Design.MonthCalendarColorTableTypeConverter))]
   public class MonthCalendarColorTable : IXmlSerializable
   {
      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MonthCalendarColorTable"/> class.
      /// </summary>
      public MonthCalendarColorTable()
      {
         this.BackgroundGradientBegin = Color.White;
         this.Border = Color.FromArgb(154, 198, 255);
         this.MonthSeparator = Color.FromArgb(154, 198, 255);
         //this.HeaderGradientBegin = Color.FromArgb(191, 219, 255);
         this.HeaderGradientBegin = Color.FromArgb(30, 144, 255);
         this.HeaderActiveGradientBegin = Color.FromArgb(191, 219, 255);
         this.HeaderText = Color.Black;
         this.HeaderActiveText = Color.Navy;
         this.HeaderSelectedText = Color.Navy;
         this.HeaderArrow = Color.FromArgb(111, 157, 217);
         this.HeaderActiveArrow = Color.Navy;
         this.FooterGradientBegin = Color.Transparent;
         this.FooterText = Color.Black;
         this.FooterActiveText = Color.Black;
         this.FooterTodayCircleBorder = Color.FromArgb(187, 85, 3);
         this.MonthBodyGradientBegin = Color.Transparent;
         this.DayActiveGradientBegin = Color.DarkOrange;
         this.DaySelectedGradientBegin = Color.FromArgb(251, 200, 79);
         this.DayText = Color.Black;
         this.DayTextBold = Color.Black;
         this.DayActiveText = Color.Black;
         this.DaySelectedText = Color.Black;
         this.DayTrailingText = Color.FromArgb(179, 179, 179);
         this.DayTodayCircleBorder = Color.FromArgb(187, 85, 3);
         this.DayActiveTodayCircleBorder = Color.FromArgb(187, 85, 3);
         this.DaySelectedTodayCircleBorder = Color.FromArgb(187, 85, 3);
         this.DayHeaderGradientBegin = Color.Transparent;
         this.DayHeaderText = Color.Black;
         this.WeekHeaderGradientBegin = Color.Transparent;
         this.WeekHeaderText = Color.FromArgb(179, 179, 179);
      }

      #endregion

      #region properties

      #region control properties

      /// <summary>
      /// Gets or sets the start color of the background of the <see cref="MonthCalendar"/> control.
      /// </summary>
      [DefaultValue(typeof(Color), "White")]
      [Description("Start color of the month calendar background.")]
      public virtual Color BackgroundGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the end color of the background of the <see cref="MonthCalendar"/> control.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("End color of the month calendar background.")]
      public virtual Color BackgroundGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the background of the <see cref="MonthCalendar"/> control.
      /// </summary>
      [DefaultValue(null)]
      [Description("Fill mode of the month calendar background.")]
      public LinearGradientMode? BackgroundGradientMode { get; set; }

      /// <summary>
      /// Gets or sets border color.
      /// </summary>
      [DefaultValue(typeof(Color), "154, 198, 255")]
      [Description("The color of the border for the month calendar.")]
      public virtual Color Border { get; set; }

      /// <summary>
      /// Gets or sets the separator color of the month body and week header and/or day header.
      /// </summary>
      [DefaultValue(typeof(Color), "154, 198, 255")]
      [Description("The color of the separator line between month body and/or day header.")]
      public virtual Color MonthSeparator { get; set; }

      #endregion

      #region month header properties

      /// <summary>
      /// Gets or sets the start color of the header background.
      /// </summary>
      [DefaultValue(typeof(Color), "191, 219, 255")]
      [Description("Start color of the header background.")]
      public virtual Color HeaderGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the end color of the header background.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("End color of the header background.")]
      public virtual Color HeaderGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the header background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Fill mode of the header background.")]
      public LinearGradientMode? HeaderGradientMode { get; set; }

      /// <summary>
      /// Gets or sets the active start color of the header background.
      /// </summary>
      [DefaultValue(typeof(Color), "191, 219, 255")]
      [Description("Active start color of the header background.")]
      public virtual Color HeaderActiveGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the active end color of the header background.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("Active end color of the header background.")]
      public virtual Color HeaderActiveGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the active fill mode of the header background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Active fill mode of the header background.")]
      public LinearGradientMode? HeaderActiveGradientMode { get; set; }

      /// <summary>
      /// Gets or sets the color of the header text.
      /// </summary>
      [DefaultValue(typeof(Color), "Black")]
      [Description("Header text color.")]
      public virtual Color HeaderText { get; set; }

      /// <summary>
      /// Gets or sets the color of the active header text.
      /// </summary>
      [DefaultValue(typeof(Color), "Navy")]
      [Description("Active text color of the header.")]
      public virtual Color HeaderActiveText { get; set; }

      /// <summary>
      /// Gets or sets the color of the selected header text.
      /// </summary>
      [DefaultValue(typeof(Color), "Navy")]
      [Description("Selected text color of the header.")]
      public virtual Color HeaderSelectedText { get; set; }

      /// <summary>
      /// Gets or sets the header arrow color.
      /// </summary>
      [DefaultValue(typeof(Color), "111, 157, 217")]
      [Description("Color of the header arrows.")]
      public virtual Color HeaderArrow { get; set; }

      /// <summary>
      /// Gets or sets the active header arrow color.
      /// </summary>
      [DefaultValue(typeof(Color), "Navy")]
      [Description("Active color of the header arrows.")]
      public virtual Color HeaderActiveArrow { get; set; }

      #endregion

      #region footer properties

      /// <summary>
      /// Gets or sets the start color of the footer background.
      /// </summary>
      [DefaultValue(typeof(Color), "Transparent")]
      [Description("Start color of the footer background.")]
      public virtual Color FooterGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the end color of the footer background.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("End color of the footer background.")]
      public virtual Color FooterGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the footer background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Fill mode of the footer background.")]
      public LinearGradientMode? FooterGradientMode { get; set; }

      /// <summary>
      /// Gets or sets the active start color of the footer background.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("Active start color of the footer background.")]
      public virtual Color FooterActiveGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the active end color of the footer background.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("Active end color of the footer background.")]
      public virtual Color FooterActiveGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the active footer background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Active fill mode of the footer background.")]
      public LinearGradientMode? FooterActiveGradientMode { get; set; }

      /// <summary>
      /// Gets or sets the color of the footer text.
      /// </summary>
      [DefaultValue(typeof(Color), "Black")]
      [Description("Footer text color.")]
      public virtual Color FooterText { get; set; }

      /// <summary>
      /// Gets or sets the color of the active footer text.
      /// </summary>
      [DefaultValue(typeof(Color), "Black")]
      [Description("Active color of the footer text.")]
      public virtual Color FooterActiveText { get; set; }

      /// <summary>
      /// Gets or sets the today circle border color of the footer.
      /// </summary>
      [DefaultValue(typeof(Color), "187, 85, 3")]
      [Description("Border color of the today circle in the footer.")]
      public virtual Color FooterTodayCircleBorder { get; set; }

      #endregion

      #region month body properties

      /// <summary>
      /// Gets or sets the start color of the month body background.
      /// </summary>
      [DefaultValue(typeof(Color), "Transparent")]
      [Description("Start color of the month body background.")]
      public virtual Color MonthBodyGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the end color of the month body background.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("End color of the month body background.")]
      public virtual Color MonthBodyGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the month body background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Fill mode of the month body background.")]
      public LinearGradientMode? MonthBodyGradientMode { get; set; }

      #endregion

      #region day properties

      /// <summary>
      /// Gets or sets the active start color of the day background.
      /// </summary>
      [DefaultValue(typeof(Color), "DarkOrange")]
      [Description("Active start color of the day background.")]
      public virtual Color DayActiveGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the active end color of the day background.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("Active end color of the day background.")]
      public virtual Color DayActiveGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the active day background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Active fill mode of the day background.")]
      public LinearGradientMode? DayActiveGradientMode { get; set; }

      /// <summary>
      /// Gets or sets the selected start color of the day background.
      /// </summary>
      [DefaultValue(typeof(Color), "251, 200, 79")]
      [Description("Start color of selected day background.")]
      public virtual Color DaySelectedGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the selected end color of the day background.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("End color of selected day background.")]
      public virtual Color DaySelectedGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the selected day background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Fill mode of selected day background.")]
      public LinearGradientMode? DaySelectedGradientMode { get; set; }

      /// <summary>
      /// Gets or sets the color of the day text.
      /// </summary>
      [DefaultValue(typeof(Color), "Black")]
      [Description("The color of the day text.")]
      public virtual Color DayText { get; set; }

      /// <summary>
      /// Gets or sets the text color for bolded dates.
      /// </summary>
      [DefaultValue(typeof(Color), "Black")]
      [Description("The text color for bolded dates.")]
      public virtual Color DayTextBold { get; set; }

      /// <summary>
      /// Gets or sets the active color of the day text.
      /// </summary>
      [DefaultValue(typeof(Color), "Black")]
      [Description("The active color of the day text.")]
      public virtual Color DayActiveText { get; set; }

      /// <summary>
      /// Gets or sets the selected color of the day text.
      /// </summary>
      [DefaultValue(typeof(Color), "Black")]
      [Description("The selection color of the day text.")]
      public virtual Color DaySelectedText { get; set; }

      /// <summary>
      /// Gets or sets the trailing color of the day text.
      /// </summary>
      [DefaultValue(typeof(Color), "179, 179, 179")]
      [Description("The trailing color of the day text.")]
      public virtual Color DayTrailingText { get; set; }

      /// <summary>
      /// Gets or sets the circle border color.
      /// </summary>
      [DefaultValue(typeof(Color), "187, 85, 3")]
      [Description("Color of the border of the today circle.")]
      public virtual Color DayTodayCircleBorder { get; set; }

      /// <summary>
      /// Gets or sets the active circle border color.
      /// </summary>
      [DefaultValue(typeof(Color), "187, 85, 3")]
      [Description("The color of the border of the active today circle.")]
      public virtual Color DayActiveTodayCircleBorder { get; set; }

      /// <summary>
      /// Gets or sets the selected circle border color.
      /// </summary>
      [DefaultValue(typeof(Color), "187, 85, 3")]
      [Description("The selection color of the border of the today circle.")]
      public virtual Color DaySelectedTodayCircleBorder { get; set; }

      #endregion

      #region day header properties

      /// <summary>
      /// Gets or sets the day header background start color.
      /// </summary>
      [DefaultValue(typeof(Color), "Transparent")]
      [Description("Start color of the day header background.")]
      public virtual Color DayHeaderGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the day header background end color.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("End color of the day header background.")]
      public virtual Color DayHeaderGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the day header background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Fill mode of the day header background.")]
      public LinearGradientMode? DayHeaderGradientMode { get; set; }

      /// <summary>
      /// Gets or sets the text color of the day header.
      /// </summary>
      [DefaultValue(typeof(Color), "Black")]
      [Description("The text color of the day header.")]
      public virtual Color DayHeaderText { get; set; }

      #endregion

      #region week header properties

      /// <summary>
      /// Gets or sets the week header background start color.
      /// </summary>
      [DefaultValue(typeof(Color), "Transparent")]
      [Description("Start color of the week header background.")]
      public virtual Color WeekHeaderGradientBegin { get; set; }

      /// <summary>
      /// Gets or sets the week header background end color.
      /// </summary>
      [DefaultValue(typeof(Color), "")]
      [Description("End color of the week header background.")]
      public virtual Color WeekHeaderGradientEnd { get; set; }

      /// <summary>
      /// Gets or sets the fill mode of the week header background.
      /// </summary>
      [DefaultValue(null)]
      [Description("Fill mode of the week header background.")]
      public LinearGradientMode? WeekHeaderGradientMode { get; set; }

      /// <summary>
      /// Gets or sets the text color of the week header.
      /// </summary>
      [DefaultValue(typeof(Color), "179, 179, 179")]
      [Description("Text color of the week header.")]
      public virtual Color WeekHeaderText { get; set; }

      #endregion

      #endregion

      #region methods

      /// <summary>
      /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable"/> 
      /// interface, you should return <c>null</c> (<c>Nothing</c> in Visual Basic) from this method, and instead, if specifying 
      /// a custom schema is required, apply the XmlSchemaProviderAttribute to the class.
      /// </summary>
      /// <returns>An <see cref="XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="WriteXml"/> method 
      /// and consumed by the <see cref="ReadXml"/> method.</returns>
      public virtual XmlSchema GetSchema()
      {
         return new XmlSchema();
      }

      /// <summary>
      /// Generates an object from its XML representation.
      /// </summary>
      /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/>-stream from which the object is deserialized.</param>
      public virtual void ReadXml(XmlReader reader)
      {
         XmlDocument doc = new XmlDocument();
         doc.Load(reader);

         if (doc.GetElementsByTagName("BackgroundGradientBegin").Count > 0)
         {
            this.BackgroundGradientBegin = GetColorFromString(doc.GetElementsByTagName("BackgroundGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("BackgroundGradientEnd").Count > 0)
         {
            this.BackgroundGradientEnd = GetColorFromString(doc.GetElementsByTagName("BackgroundGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("BackgroundGradientMode").Count > 0)
         {
            this.BackgroundGradientMode = GetModeFromString(doc.GetElementsByTagName("BackgroundGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("Border").Count > 0)
         {
            this.Border = GetColorFromString(doc.GetElementsByTagName("Border")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayActiveGradientBegin").Count > 0)
         {
            this.DayActiveGradientBegin = GetColorFromString(doc.GetElementsByTagName("DayActiveGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayActiveGradientEnd").Count > 0)
         {
            this.DayActiveGradientEnd = GetColorFromString(doc.GetElementsByTagName("DayActiveGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayActiveGradientMode").Count > 0)
         {
            this.DayActiveGradientMode = GetModeFromString(doc.GetElementsByTagName("DayActiveGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayActiveText").Count > 0)
         {
            this.DayActiveText = GetColorFromString(doc.GetElementsByTagName("DayActiveText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayActiveTodayCircleBorder").Count > 0)
         {
            this.DayActiveTodayCircleBorder = GetColorFromString(doc.GetElementsByTagName("DayActiveTodayCircleBorder")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayHeaderGradientBegin").Count > 0)
         {
            this.DayHeaderGradientBegin = GetColorFromString(doc.GetElementsByTagName("DayHeaderGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayHeaderGradientEnd").Count > 0)
         {
            this.DayHeaderGradientEnd = GetColorFromString(doc.GetElementsByTagName("DayHeaderGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayHeaderGradientMode").Count > 0)
         {
            this.DayHeaderGradientMode = GetModeFromString(doc.GetElementsByTagName("DayHeaderGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayHeaderText").Count > 0)
         {
            this.DayHeaderText = GetColorFromString(doc.GetElementsByTagName("DayHeaderText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DaySelectedGradientBegin").Count > 0)
         {
            this.DaySelectedGradientBegin = GetColorFromString(doc.GetElementsByTagName("DaySelectedGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DaySelectedGradientEnd").Count > 0)
         {
            this.DaySelectedGradientEnd = GetColorFromString(doc.GetElementsByTagName("DaySelectedGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DaySelectedGradientMode").Count > 0)
         {
            this.DaySelectedGradientMode = GetModeFromString(doc.GetElementsByTagName("DaySelectedGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DaySelectedText").Count > 0)
         {
            this.DaySelectedText = GetColorFromString(doc.GetElementsByTagName("DaySelectedText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DaySelectedTodayCircleBorder").Count > 0)
         {
            this.DaySelectedTodayCircleBorder = GetColorFromString(doc.GetElementsByTagName("DaySelectedTodayCircleBorder")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayText").Count > 0)
         {
            this.DayText = GetColorFromString(doc.GetElementsByTagName("DayText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayTextBold").Count > 0)
         {
            this.DayTextBold = GetColorFromString(doc.GetElementsByTagName("DayTextBold")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayTodayCircleBorder").Count > 0)
         {
            this.DayTodayCircleBorder = GetColorFromString(doc.GetElementsByTagName("DayTodayCircleBorder")[0].InnerText);
         }

         if (doc.GetElementsByTagName("DayTrailingText").Count > 0)
         {
            this.DayTrailingText = GetColorFromString(doc.GetElementsByTagName("DayTrailingText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterActiveGradientBegin").Count > 0)
         {
            this.FooterActiveGradientBegin = GetColorFromString(doc.GetElementsByTagName("FooterActiveGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterActiveGradientEnd").Count > 0)
         {
            this.FooterActiveGradientEnd = GetColorFromString(doc.GetElementsByTagName("FooterActiveGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterActiveGradientMode").Count > 0)
         {
            this.FooterActiveGradientMode = GetModeFromString(doc.GetElementsByTagName("FooterActiveGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterActiveText").Count > 0)
         {
            this.FooterActiveText = GetColorFromString(doc.GetElementsByTagName("FooterActiveText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterGradientBegin").Count > 0)
         {
            this.FooterGradientBegin = GetColorFromString(doc.GetElementsByTagName("FooterGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterGradientEnd").Count > 0)
         {
            this.FooterGradientEnd = GetColorFromString(doc.GetElementsByTagName("FooterGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterGradientMode").Count > 0)
         {
            this.FooterGradientMode = GetModeFromString(doc.GetElementsByTagName("FooterGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterText").Count > 0)
         {
            this.FooterText = GetColorFromString(doc.GetElementsByTagName("FooterText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("FooterTodayCircleBorder").Count > 0)
         {
            this.FooterTodayCircleBorder = GetColorFromString(doc.GetElementsByTagName("FooterTodayCircleBorder")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderActiveArrow").Count > 0)
         {
            this.HeaderActiveArrow = GetColorFromString(doc.GetElementsByTagName("HeaderActiveArrow")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderActiveGradientBegin").Count > 0)
         {
            this.HeaderActiveGradientBegin = GetColorFromString(doc.GetElementsByTagName("HeaderActiveGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderActiveGradientEnd").Count > 0)
         {
            this.HeaderActiveGradientEnd = GetColorFromString(doc.GetElementsByTagName("HeaderActiveGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderActiveGradientMode").Count > 0)
         {
            this.HeaderActiveGradientMode = GetModeFromString(doc.GetElementsByTagName("HeaderActiveGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderActiveText").Count > 0)
         {
            this.HeaderActiveText = GetColorFromString(doc.GetElementsByTagName("HeaderActiveText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderArrow").Count > 0)
         {
            this.HeaderArrow = GetColorFromString(doc.GetElementsByTagName("HeaderArrow")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderGradientBegin").Count > 0)
         {
            this.HeaderGradientBegin = GetColorFromString(doc.GetElementsByTagName("HeaderGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderGradientEnd").Count > 0)
         {
            this.HeaderGradientEnd = GetColorFromString(doc.GetElementsByTagName("HeaderGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderGradientMode").Count > 0)
         {
            this.HeaderGradientMode = GetModeFromString(doc.GetElementsByTagName("HeaderGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderSelectedText").Count > 0)
         {
            this.HeaderSelectedText = GetColorFromString(doc.GetElementsByTagName("HeaderSelectedText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("HeaderText").Count > 0)
         {
            this.HeaderText = GetColorFromString(doc.GetElementsByTagName("HeaderText")[0].InnerText);
         }

         if (doc.GetElementsByTagName("MonthBodyGradientBegin").Count > 0)
         {
            this.MonthBodyGradientBegin = GetColorFromString(doc.GetElementsByTagName("MonthBodyGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("MonthBodyGradientEnd").Count > 0)
         {
            this.MonthBodyGradientEnd = GetColorFromString(doc.GetElementsByTagName("MonthBodyGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("MonthBodyGradientMode").Count > 0)
         {
            this.MonthBodyGradientMode = GetModeFromString(doc.GetElementsByTagName("MonthBodyGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("MonthSeparator").Count > 0)
         {
            this.MonthSeparator = GetColorFromString(doc.GetElementsByTagName("MonthSeparator")[0].InnerText);
         }

         if (doc.GetElementsByTagName("WeekHeaderGradientBegin").Count > 0)
         {
            this.WeekHeaderGradientBegin = GetColorFromString(doc.GetElementsByTagName("WeekHeaderGradientBegin")[0].InnerText);
         }

         if (doc.GetElementsByTagName("WeekHeaderGradientEnd").Count > 0)
         {
            this.WeekHeaderGradientEnd = GetColorFromString(doc.GetElementsByTagName("WeekHeaderGradientEnd")[0].InnerText);
         }

         if (doc.GetElementsByTagName("WeekHeaderGradientMode").Count > 0)
         {
            this.WeekHeaderGradientMode = GetModeFromString(doc.GetElementsByTagName("WeekHeaderGradientMode")[0].InnerText);
         }

         if (doc.GetElementsByTagName("WeekHeaderText").Count > 0)
         {
            this.WeekHeaderText = GetColorFromString(doc.GetElementsByTagName("WeekHeaderText")[0].InnerText);
         }
      }

      /// <summary>
      /// Converts an object into its XML representation.
      /// </summary>
      /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/>-stream to which the object is serialized.</param>
      public virtual void WriteXml(XmlWriter writer)
      {
         writer.WriteElementString("BackgroundGradientBegin", GetColorString(this.BackgroundGradientBegin));
         writer.WriteElementString("BackgroundGradientEnd", GetColorString(this.BackgroundGradientEnd));
         writer.WriteElementString("BackgroundGradientMode", GetModeString(this.BackgroundGradientMode));
         writer.WriteElementString("Border", GetColorString(this.Border));
         writer.WriteElementString("DayActiveGradientBegin", GetColorString(this.DayActiveGradientBegin));
         writer.WriteElementString("DayActiveGradientEnd", GetColorString(this.DayActiveGradientEnd));
         writer.WriteElementString("DayActiveGradientMode", GetModeString(this.DayActiveGradientMode));
         writer.WriteElementString("DayActiveText", GetColorString(this.DayActiveText));
         writer.WriteElementString("DayActiveTodayCircleBorder", GetColorString(this.DayActiveTodayCircleBorder));
         writer.WriteElementString("DayHeaderGradientBegin", GetColorString(this.DayHeaderGradientBegin));
         writer.WriteElementString("DayHeaderGradientEnd", GetColorString(this.DayHeaderGradientEnd));
         writer.WriteElementString("DayHeaderGradientMode", GetModeString(this.DayHeaderGradientMode));
         writer.WriteElementString("DayHeaderText", GetColorString(this.DayHeaderText));
         writer.WriteElementString("DaySelectedGradientBegin", GetColorString(this.DaySelectedGradientBegin));
         writer.WriteElementString("DaySelectedGradientEnd", GetColorString(this.DaySelectedGradientEnd));
         writer.WriteElementString("DaySelectedGradientMode", GetModeString(this.DaySelectedGradientMode));
         writer.WriteElementString("DaySelectedText", GetColorString(this.DaySelectedText));
         writer.WriteElementString("DaySelectedTodayCircleBorder", GetColorString(this.DaySelectedTodayCircleBorder));
         writer.WriteElementString("DayText", GetColorString(this.DayText));
         writer.WriteElementString("DayTextBold", GetColorString(this.DayTextBold));
         writer.WriteElementString("DayTodayCircleBorder", GetColorString(this.DayTodayCircleBorder));
         writer.WriteElementString("DayTrailingText", GetColorString(this.DayTrailingText));
         writer.WriteElementString("FooterActiveGradientBegin", GetColorString(this.FooterActiveGradientBegin));
         writer.WriteElementString("FooterActiveGradientEnd", GetColorString(this.FooterActiveGradientEnd));
         writer.WriteElementString("FooterActiveGradientMode", GetModeString(this.FooterActiveGradientMode));
         writer.WriteElementString("FooterActiveText", GetColorString(this.FooterActiveText));
         writer.WriteElementString("FooterGradientBegin", GetColorString(this.FooterGradientBegin));
         writer.WriteElementString("FooterGradientEnd", GetColorString(this.FooterGradientEnd));
         writer.WriteElementString("FooterGradientMode", GetModeString(this.FooterGradientMode));
         writer.WriteElementString("FooterText", GetColorString(this.FooterText));
         writer.WriteElementString("FooterTodayCircleBorder", GetColorString(this.FooterTodayCircleBorder));
         writer.WriteElementString("HeaderActiveArrow", GetColorString(this.HeaderActiveArrow));
         writer.WriteElementString("HeaderActiveGradientBegin", GetColorString(this.HeaderActiveGradientBegin));
         writer.WriteElementString("HeaderActiveGradientEnd", GetColorString(this.HeaderActiveGradientEnd));
         writer.WriteElementString("HeaderActiveGradientMode", GetModeString(this.HeaderActiveGradientMode));
         writer.WriteElementString("HeaderActiveText", GetColorString(this.HeaderActiveText));
         writer.WriteElementString("HeaderArrow", GetColorString(this.HeaderArrow));
         writer.WriteElementString("HeaderGradientBegin", GetColorString(this.HeaderGradientBegin));
         writer.WriteElementString("HeaderGradientEnd", GetColorString(this.HeaderGradientEnd));
         writer.WriteElementString("HeaderGradientMode", GetModeString(this.HeaderGradientMode));
         writer.WriteElementString("HeaderSelectedText", GetColorString(this.HeaderSelectedText));
         writer.WriteElementString("HeaderText", GetColorString(this.HeaderText));
         writer.WriteElementString("MonthBodyGradientBegin", GetColorString(this.MonthBodyGradientBegin));
         writer.WriteElementString("MonthBodyGradientEnd", GetColorString(this.MonthBodyGradientEnd));
         writer.WriteElementString("MonthBodyGradientMode", GetModeString(this.MonthBodyGradientMode));
         writer.WriteElementString("MonthSeparator", GetColorString(this.MonthSeparator));
         writer.WriteElementString("WeekHeaderGradientBegin", GetColorString(this.WeekHeaderGradientBegin));
         writer.WriteElementString("WeekHeaderGradientEnd", GetColorString(this.WeekHeaderGradientEnd));
         writer.WriteElementString("WeekHeaderGradientMode", GetModeString(this.WeekHeaderGradientMode));
         writer.WriteElementString("WeekHeaderText", GetColorString(this.WeekHeaderText));
      }

      /// <summary>
      /// Converts the specified <see cref="Color"/> to an string representation.
      /// </summary>
      /// <param name="c">The <see cref="Color"/> value to convert.</param>
      /// <returns>A <see cref="string"/> representation of the color specified by <paramref name="c"/>.</returns>
      private static string GetColorString(Color c)
      {
         if (c.IsNamedColor || c.IsKnownColor || c.IsSystemColor)
         {
            return c.Name;
         }

         if (c.IsEmpty)
         {
            return string.Empty;
         }

         return c.A + "," + c.R + "," + c.G + "," + c.B;
      }

      /// <summary>
      /// Converts the specified <see cref="string"/> to a color value.
      /// </summary>
      /// <param name="c">The <see cref="string"/> which holds the color name or the ARGB values.</param>
      /// <returns>A <see cref="Color"/> value.</returns>
      private static Color GetColorFromString(string c)
      {
         if (c.IndexOf(',') > 0)
         {
            string[] parts = c.Split(',');

            return Color.FromArgb(Convert.ToInt32(parts[0].Trim()), Convert.ToInt32(parts[1].Trim()),
                                  Convert.ToInt32(parts[2].Trim()),
                                  Convert.ToInt32(parts[3].Trim()));
         }

         if (string.IsNullOrEmpty(c.Trim()))
         {
            return Color.Empty;
         }

         return Color.FromName(c);
      }

      /// <summary>
      /// Converts the specified <see cref="Nullable{LinearGradientMode}"/> to a string representation.
      /// </summary>
      /// <param name="mode">The <see cref="Nullable{LinearGradientMode}"/> to convert.</param>
      /// <returns>A <see cref="string"/> representation of the <paramref name="mode"/>.</returns>
      private static string GetModeString(LinearGradientMode? mode)
      {
         return mode.HasValue ? ((int)mode.Value).ToString(CultureInfo.InvariantCulture) : (-1).ToString(CultureInfo.InvariantCulture);
      }

      /// <summary>
      /// Converts the specified <see cref="string"/> value to an <see cref="Nullable{LinearGradientMode}"/> value.
      /// </summary>
      /// <param name="mode">The <see cref="Nullable{LinearGradientMode}"/> to convert.</param>
      /// <returns>A <see cref="Nullable{LinearGradientMode}"/> value.</returns>
      private static LinearGradientMode? GetModeFromString(string mode)
      {
         if (mode == "-1")
         {
            return null;
         }

         return (LinearGradientMode)Convert.ToInt32(mode);
      }

      #endregion
   }
}