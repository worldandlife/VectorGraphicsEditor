﻿using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EditorModel.Selections;

namespace SimpleEditor.Controls
{
    public partial class BorderStyleEditor : UserControl, IEditor<Selection>
    {
        private Selection selection;
        private int updating;

        public event EventHandler<ChangingEventArgs> StartChanging = delegate { };
        public event EventHandler<EventArgs> Changed = delegate { };

        public BorderStyleEditor()
        {
            InitializeComponent();
            cbPattern.Items.Clear();
            cbPattern.Items.AddRange(GetPenPatternNames()); // получение всех имён доступных типов линий
            cbPattern.SelectedIndex = 0;
        }

        static readonly DashStyle[] DashStyleArray = (DashStyle[])Enum.GetValues(typeof(DashStyle));

        static readonly int DashStyleCount = DashStyleArray.Length - 1;

        public static object[] GetPenPatternNames()
        {
            var dashNameArray = Enum.GetNames(typeof(DashStyle));
            var names = new object[DashStyleCount];
            for (var i = 0; i < DashStyleCount; i++)
                names[i] = dashNameArray[i];
            return names;
        }

        public void Build(Selection selection)
        {
            // check visibility
            Visible = selection.ForAll(f => f.Style.BorderStyle != null); // show the editor only if all figures contain BorderStyle
            if (!Visible) return; // do not build anything

            // remember editing object
            this.selection = selection;

            // get list of objects
            var borderStyles = selection.Select(f => f.Style.BorderStyle).ToList();

            // copy properties of object to GUI
            updating++;

            cbPattern.SelectedIndex = (int)borderStyles.GetProperty(f => f.DashStyle);
            nudWidth.Value = (decimal)borderStyles.GetProperty(f => f.Width, 1);
            lbColor.BackColor = borderStyles.GetProperty(f => f.Color);
            cbVisible.Checked = borderStyles.GetProperty(f => f.IsVisible);

            updating--;
        }

        private void UpdateObject()
        {
            if (updating > 0) return; // we are in updating mode

            // fire event
            StartChanging(this, new ChangingEventArgs("Border Style"));

            // get list of objects
            var borderStyles = selection.Select(f => f.Style.BorderStyle).ToList();

            // send values back from GUI to object
            borderStyles.SetProperty(f => f.DashStyle = (DashStyle)cbPattern.SelectedIndex);
            borderStyles.SetProperty(f => f.Width = (float)nudWidth.Value);
            borderStyles.SetProperty(f => f.Color = lbColor.BackColor);
            borderStyles.SetProperty(f => f.IsVisible = cbVisible.Checked);

            // fire event
            Changed(this, EventArgs.Empty);
        }

        private void cbVisible_CheckedChanged(object sender, EventArgs e)
        {
            UpdateObject();
        }

        private void lbColor_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog() { Color = lbColor.BackColor };
            if (dlg.ShowDialog() == DialogResult.OK)
                lbColor.BackColor = dlg.Color;
        }

        private void cbPattern_DrawItem(object sender, DrawItemEventArgs e)
        {
            var cb = (ComboBox)sender;
            var g = e.Graphics;
            // Draw the background of the item.
            e.DrawBackground();
            var rect = new Rectangle(e.Bounds.X, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1);
            rect.Inflate(-4, 0);
            //if (e.Index == 0)
            //{
            //    using (var textColor = new SolidBrush(e.ForeColor))
            //    {
            //        string showing = cb.Items[e.Index].ToString();
            //        g.DrawString(showing, cb.Font, textColor, rect);
            //    }
            //}
            //else
            //{
                using (var p = new Pen(e.ForeColor))
                {
                    p.Width = 2;
                    p.DashStyle = (DashStyle)(e.Index /* - 1*/);
                    g.DrawLine(p, new Point(rect.Left, rect.Top + rect.Height / 2),
                                  new Point(rect.Right, rect.Top + rect.Height / 2));
                }
            //}
            // Draw the focus rectangle if the mouse hovers over an item.
            e.DrawFocusRectangle();
        }
    }
}
