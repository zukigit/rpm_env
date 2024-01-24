namespace CustomControls.Design
{
   using System;
   using System.Collections;
   using System.ComponentModel;
   using System.ComponentModel.Design;
   using System.IO;
   using System.Text;
   using System.Windows.Forms;
   using System.Windows.Forms.Design;
   using System.Xml;
   using System.Xml.Serialization;

   /// <summary>
   /// The <see cref="MonthCalendar"/> designer.
   /// </summary>
   internal class MonthCalendarControlDesigner : ControlDesigner
   {
      #region properties

      /// <summary>
      /// Gets the <see cref="SelectionRules"/> for the control.
      /// </summary>
      public override SelectionRules SelectionRules
      {
         get
         {
            return SelectionRules.BottomSizeable
               | SelectionRules.RightSizeable
               | SelectionRules.Moveable
               | SelectionRules.Visible;
         }
      }

      /// <summary>
      /// Gets the design time action list collection which is supported by the designer.
      /// </summary>
      public override DesignerActionListCollection ActionLists
      {
         get
         {
            return new DesignerActionListCollection { new MonthCalendarControlDesignerActionList(this.Component) };
         }
      }

      #endregion

      #region methods

      /// <summary>
      /// Prefilters the properties.
      /// </summary>
      /// <param name="properties">The property dictionary.</param>
      protected override void PreFilterProperties(IDictionary properties)
      {
         base.PreFilterProperties(properties);

         properties.Remove("BackgroundImage");
         properties.Remove("ForeColor");
         properties.Remove("Text");
         properties.Remove("ImeMode");
         properties.Remove("Padding");
         properties.Remove("BackgroundImageLayout");
         properties.Remove("BackColor");
      }

      #endregion

      #region nested classes

      /// <summary>
      /// Provides the designer action list for the <see cref="CustomControls.MonthCalendar"/> or <see cref="DatePicker"/> controls.
      /// </summary>
      internal class MonthCalendarControlDesignerActionList : DesignerActionList
      {
         #region fields

         private const string FileFilter = "XML File (*.xml)|*.xml";

         /// <summary>
         /// The <see cref="CustomControls.MonthCalendar"/> control.
         /// </summary>
         private readonly CustomControls.MonthCalendar cal;

         /// <summary>
         /// The <see cref="IComponentChangeService"/>.
         /// </summary>
         private readonly IComponentChangeService iccs;

         /// <summary>
         /// The <see cref="DesignerActionUIService"/>.
         /// </summary>
         private readonly DesignerActionUIService designerUISvc;

         #endregion

         #region constructor

         /// <summary>
         /// Initializes a new instance of the <see cref="MonthCalendarControlDesignerActionList"/> class.
         /// </summary>
         /// <param name="component">The component.</param>
         public MonthCalendarControlDesignerActionList(IComponent component)
            : base(component)
         {
            Type compType = component.GetType();

            if (component == null || (compType != typeof(CustomControls.MonthCalendar) && compType != typeof(DatePicker)))
            {
               throw new InvalidOperationException("MonthCalendarDesigner : component is null or not of the correct type.");
            }

            if (compType == typeof(DatePicker))
            {
               this.cal = ((DatePicker)component).Picker;
            }
            else
            {
               this.cal = (CustomControls.MonthCalendar)component;
            }

            this.iccs = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            this.designerUISvc = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
         }

         #endregion

         #region properties

         /// <summary>
         /// Gets or sets a value indicating whether to show the smart tag area while creating.
         /// </summary>
         public override bool AutoShow
         {
            get { return true; }
            set { base.AutoShow = true; }
         }

         #endregion

         #region methods

         /// <summary>
         /// Returns the collection of <see cref="DesignerActionItem"/>.
         /// </summary>
         /// <returns>A collection of <see cref="DesignerActionItem"/>.</returns>
         public override DesignerActionItemCollection GetSortedActionItems()
         {
            DesignerActionItemCollection actionItems = new DesignerActionItemCollection
                                 {
                                    new DesignerActionMethodItem(this, "LoadColorTable", "Load the color table",
                                                                 "", "Loads the color table from an XML-file",  true),
                                    new DesignerActionMethodItem(this, "SaveColorTable", "Save the color table", "",
                                                                 "Saves the color table to an XML-file", true)
                                 };

            return actionItems;
         }

         /// <summary>
         /// Saves the color table.
         /// </summary>
         internal void SaveColorTable()
         {
            // hide smart tag area if visible
            this.designerUISvc.HideUI(this.Component);

            // create save dialog
            using (SaveFileDialog dlg = new SaveFileDialog { Filter = FileFilter })
            {
               // show dialog
               if (dlg.ShowDialog() == DialogResult.OK)
               {
                  // create writer
                  using (XmlWriter writer = new XmlTextWriter(dlg.FileName, Encoding.UTF8))
                  {
                     // serialize the color table
                     new XmlSerializer(typeof(MonthCalendarColorTable)).Serialize(writer, this.cal.ColorTable);

                     // empty buffer and close writer
                     writer.Flush();
                     writer.Close();
                  }
               }
            }
         }

         /// <summary>
         /// Loads the color table.
         /// </summary>
         internal void LoadColorTable()
         {
            // hide smart tag area if visible
            this.designerUISvc.HideUI(this.Component);

            // create file open dialog
            using (OpenFileDialog dlg = new OpenFileDialog { Filter = FileFilter })
            {
               // show dialog
               if (dlg.ShowDialog() == DialogResult.OK)
               {
                  // create file stream
                  using (var fs = new FileStream(dlg.FileName, FileMode.Open))
                  {
                     // get new color table
                     MonthCalendarColorTable colorTable = (MonthCalendarColorTable)new XmlSerializer(typeof(MonthCalendarColorTable)).Deserialize(fs);

                     // save old color table
                     MonthCalendarColorTable oldTable = this.cal.ColorTable;

                     // set new color table
                     this.cal.ColorTable = colorTable;

                     // get property name of the color table in current component
                     string propName = this.Component.GetType() == typeof(DatePicker) ? "PickerColorTable" : "ColorTable";

                     // inform the IComponentChangeService, that the color table property has changed
                     this.iccs.OnComponentChanged(this.Component, TypeDescriptor.GetProperties(this.Component)[propName], oldTable, colorTable);

                     // invalidate the month calendar
                     this.cal.Invalidate();
                  }
               }
            }
         }

         #endregion
      }

      #endregion
   }
}