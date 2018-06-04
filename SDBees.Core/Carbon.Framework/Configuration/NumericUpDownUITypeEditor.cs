using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;

namespace Carbon.Configuration
{
	/// <summary>
	/// Defines a UITypeEditor that displays a NumericUpDown control for a PropertyGrid value.
	/// </summary>
	public class NumericUpDownUITypeEditor : UITypeEditor
	{
		public NumericUpDownUITypeEditor()
			: base()
		{

		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					NumericUpDown editor = new NumericUpDown();
					editor.DecimalPlaces = 0;
					decimal v = Convert.ToDecimal(value);
					editor.Minimum = 0;
					editor.Maximum = Decimal.MaxValue;
					editor.Value = v;
					edSvc.DropDownControl(editor);
					value = editor.Value;
					return value;
				}
			}

			return base.EditValue(context, provider, value);
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return false;
		}

        public override void PaintValue(PaintValueEventArgs e)
        {
            base.PaintValue(e);
        }
	}
}
