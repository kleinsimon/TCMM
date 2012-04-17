using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TC_Macro_Manager
{
    public partial class M_CurveColor : UserControl
    {
        private CurveColorModule ModObj = new CurveColorModule();

        public M_CurveColor()
        {
            InitializeComponent();
            ModObj.Position = MacroPosition.Post;
            if (ParentForm.GetType() == typeof(MainForm))
            {
                ((MainForm)ParentForm).TCM.Module.Add(ModObj);
            }
        }
    }

    public class CurveColorModule : MacroModule
    {
        public MacroPosition Position
        {
            get;

            set;
        }

        public string getString()
        {
            return "";
        }
    }
}
