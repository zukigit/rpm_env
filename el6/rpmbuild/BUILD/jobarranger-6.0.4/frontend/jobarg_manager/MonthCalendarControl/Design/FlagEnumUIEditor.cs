namespace CustomControls.Design
{
   using System;
   using System.ComponentModel;
   using System.Drawing.Design;
   using System.Windows.Forms;
   using System.Windows.Forms.Design;

   /// <summary>
   /// UITypeEditor for flag enums.
   /// </summary>
   /// <remarks>
   /// From CodeProject : http://www.codeproject.com/KB/edit/flagenumeditor.aspx
   /// </remarks>
   internal class FlagEnumUIEditor : UITypeEditor
   {
      /// <summary>
      /// The checklistbox.
      /// </summary>
      private readonly FlagCheckedListBox flagEnumCheckBox;

      /// <summary>
      /// Initializes a new instance of the <see cref="FlagEnumUIEditor"/> class.
      /// </summary>
      public FlagEnumUIEditor()
      {
         this.flagEnumCheckBox = new FlagCheckedListBox { BorderStyle = BorderStyle.None };
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
         if (context != null && context.Instance != null && provider != null)
         {
            IWindowsFormsEditorService editorSvc = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

            if (editorSvc != null)
            {
               Enum e = (Enum) Convert.ChangeType(value, context.PropertyDescriptor.PropertyType);

               this.flagEnumCheckBox.EnumValue = e;

               editorSvc.DropDownControl(this.flagEnumCheckBox);

               return this.flagEnumCheckBox.EnumValue;
            }
         }

         return null;
      }

      /// <summary>
      /// Gets the editor style used by the <see cref="UITypeEditor"/><c>.EditValue(<see cref="IServiceProvider"/>, <see cref="Object"/>)</c> method.
      /// </summary>
      /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
      /// <returns>The <see cref="UITypeEditorEditStyle"/><c>.DropDown</c>.</returns>
      public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
      {
         return UITypeEditorEditStyle.DropDown;
      }
   }
}