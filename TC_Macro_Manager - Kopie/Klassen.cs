using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;

namespace TC_Macro_Manager
{
    [Serializable]
    public class MacroData
    {
        public BindingList<Makro> Makros = new BindingList<Makro>();
        public BindingList<Elemente> Werkstoffe = new BindingList<Elemente>();
        public BindingList<Elemente> Bedingungen = new BindingList<Elemente>();
        public BindingList<Phasen> PhasenSets = new BindingList<Phasen>();

        public BindingList<TextElement> DBText = new BindingList<TextElement>();
        public BindingList<TextElement> PolyText = new BindingList<TextElement>();
        public BindingList<TextElement> PostText = new BindingList<TextElement>();
    }

    public class TCMacro
    {
        public TCMacro()
        {

        }

        public string makeMacro(Makro ws)
        {

            string ret = String.Format(@"@@@ Macro generated with TCMM
@@@ Material-Name: {2}

@@@ Fetching Database
goto_module Database_Retrieval
switch {3}

@@@ Elements
{0}

@@@ Phases
reject phases *
{4}

@@@ Custom Commands DB
{5}

get_data

goto_module Poly_3

@@@ Conditions{1}

@@@ Custom Commands Poly
{6}

@@@ post module
post

@@@ Custom Commands Post
{7}

Set_Interactive
", getElm(ws), getCont(ws), ws.Name, ws.DB, getPhases(ws), ws.DBText, ws.PolyText, ws.PostText);
            return ret;
        }

        private string getPhases(Makro ws)
        {
            string tmp = "";
            int c = 0;

            foreach (Phase p in ws.Phasen)
            {
                if (!p.Enabled || p.Name == null) continue;
                if (c % 5 == 0) tmp += Environment.NewLine + @"restore phases";
                tmp += " " + p.Name.ToString().ToLower();
                c++;
            }

            return tmp;
        }

        private string getElm(Makro ws)
        {
            string tmp = "";
            int c = 0;

            foreach (Element elm in ws.Elemente)
            {
                if (elm.Disabled || elm.Name == null) continue;
                if (c % 5 == 0) tmp += Environment.NewLine + @"Define_Elements";
                tmp += " " + elm.Name.Trim().ToLower();
                c++;
            }

            return tmp;
        }

        private string getCont(Makro ws)
        {
            string tmp = "";

            foreach (Element elm in ws.Elemente)
            {
                if (elm.Disabled) continue;

                if (elm.Value != null && elm.Value != "")
                {
                    tmp += Environment.NewLine + "Set_Condition " + ConvUnit(elm.Kind, elm.Name, elm.Value);
                }
            }

            tmp += Environment.NewLine;

            foreach (Element elm in ws.Conditions)
            {
                if (elm.Disabled) continue;

                if (elm.Value != null && elm.Value != "")
                {
                    tmp += Environment.NewLine + "Set_Condition " + ConvUnit(elm.Kind, elm.Name, elm.Value);
                }
            }

            tmp = tmp.Replace(",", ".");
            return tmp;
        }

        private string ConvUnit(ValKind kind, string Name, string val)
        {
            string tmp = "";
            val = val.Trim();

            if (kind == ValKind.MPC) tmp += @"w(" + Name.ToLower() + @")=" + (DblSaveConvert(val) / 100d);
            else if (kind == ValKind.VPC) tmp += @"v(" + Name.ToLower() + @")=" + (DblSaveConvert(val) / 100d);
            else if (kind == ValKind.VPC) tmp += @"v(" + Name.ToLower() + @")=" + (DblSaveConvert(val) / 100d);
            else if (kind == ValKind.DGC) tmp += @"" + Name.ToLower() + @"=" + (DblSaveConvert(val) + 273.15d);
            else if (kind == ValKind.BAR) tmp += @"" + Name.ToLower() + @"=" + (DblSaveConvert(val) / 1e5d);
            else tmp += @"" + Name.ToLower() + @"=" + (val);

            return tmp;
        }

        public double DblSaveConvert(string input)
        {
            double res;
            try
            {
                res = double.Parse(input);
            }
            catch
            {
                res = 0d;
            }

            return res;
        }
    }

    public enum ValKind
    {
        [EnumDescription("Keine")]
        None,
        [EnumDescription("Mas-%")]
        MPC,
        [EnumDescription("Vol-%")]
        VPC,
        [EnumDescription("°C")]
        DGC,
        [EnumDescription("Kelvin")]
        KLV,
        [EnumDescription("Bar")]
        BAR,
        [EnumDescription("Pascal")]
        PAS
    }

    public enum Phases
    {
        [EnumDescription("GAS:G")]
        GAS,
        [EnumDescription("LIQUID:L")]
        LIQUID,
        [EnumDescription("BCC_A2")]
        BCC_A2,
        [EnumDescription("FCC_A1")]
        FCC_A1,
        [EnumDescription("HCP_A3")]
        HCP_A3,
        [EnumDescription("DIAMOND_FCC_A4")]
        DIAMOND_FCC_A4,
        [EnumDescription("GRAPHITE")]
        GRAPHITE,
        [EnumDescription("CEMENTITE")]
        CEMENTITE,
        [EnumDescription("M23C6")]
        M23C6,
        [EnumDescription("M7C3")]
        M7C3,
        [EnumDescription("M6C")]
        M6C,
        [EnumDescription("M5C2")]
        M5C2,
        [EnumDescription("M3C2")]
        M3C2,
        [EnumDescription("MC_ETA")]
        MC_ETA,
        [EnumDescription("MC_SHP")]
        MC_SHP,
        [EnumDescription("KSI_CARBIDE")]
        KSI_CARBIDE,
        [EnumDescription("A1_KAPPA")]
        A1_KAPPA,
        [EnumDescription("KAPPA")]
        KAPPA,
        [EnumDescription("Z_PHASE")]
        Z_PHASE,
        [EnumDescription("FE4N_LP1")]
        FE4N_LP1,
        [EnumDescription("FECN_CHI")]
        FECN_CHI,
        [EnumDescription("PI")]
        PI,
        [EnumDescription("SIGMA")]
        SIGMA,
        [EnumDescription("MU_PHASE")]
        MU_PHASE,
        [EnumDescription("P_PHASE")]
        P_PHASE,
        [EnumDescription("R_PHASE")]
        R_PHASE,
        [EnumDescription("CHI_A12")]
        CHI_A12,
        [EnumDescription("LAVES_PHASE_C14")]
        LAVES_PHASE_C14,
        [EnumDescription("M3SI")]
        M3SI,
        [EnumDescription("CR3SI")]
        CR3SI,
        [EnumDescription("FE2SI")]
        FE2SI,
        [EnumDescription("MSI")]
        MSI,
        [EnumDescription("M5SI3")]
        M5SI3,
        [EnumDescription("NBNI3")]
        NBNI3,
        [EnumDescription("NI3TI")]
        NI3TI,
        [EnumDescription("AL4C3")]
        AL4C3,
        [EnumDescription("FE8SI2C")]
        FE8SI2C,
        [EnumDescription("SIC")]
        SIC,
        [EnumDescription("TI2N")]
        TI2N
    }

    [Serializable]
    public class Makro : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _DB;
        public string DB
        {
            get
            {
                return _DB;
            }
            set
            {
                _DB = value;
                NotifyPropertyChanged("DB");
            }
        }

        private string _folder;
        public string Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                _folder = value;
                NotifyPropertyChanged("Folder");
            }
        }

        private string _filename;
        public string FileName
        {
            get
            {
                if (_filename == null) return _Name + "_Macro";
                else return _filename;
            }
            set
            {
                _filename = value;
                NotifyPropertyChanged("FileName");
            }
        }

        private string _DBText;
        public string DBText
        {
            get
            {
                return _DBText;
            }
            set
            {
                _DBText = value;
                NotifyPropertyChanged("DBText");
            }
        }

        private string _PolyText;
        public string PolyText
        {
            get
            {
                return _PolyText;
            }
            set
            {
                _PolyText = value;
                NotifyPropertyChanged("_PolyText");
            }
        }

        private string _PostText;
        public string PostText
        {
            get
            {
                return _PostText;
            }
            set
            {
                _PostText = value;
                NotifyPropertyChanged("_PostText");
            }
        }

        private BindingList<Phase> _Phasen = new BindingList<Phase>();
        public BindingList<Phase> Phasen
        {
            get
            {
                return _Phasen;
            }
            set
            {
                _Phasen = value;
                NotifyPropertyChanged("Phasen");
            }
        }

        public BindingList<Element> Elemente = new BindingList<Element>();

        public BindingList<Element> Conditions = new BindingList<Element>();

        public Makro()
        {
            Elemente.ListChanged += new ListChangedEventHandler(Elemente_ListChanged);
            Conditions.ListChanged += new ListChangedEventHandler(Elemente_ListChanged);
            Phasen.ListChanged += new ListChangedEventHandler(Elemente_ListChanged);
        }

        public void initDefaults()
        {
            _DB = "TCFE6.2";
            _folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (_Phasen.Count == 0)
            {
                _Phasen.Clear();
                foreach (Phases p in Enum.GetValues(typeof(Phases)))
                {
                    Phase tmpP = new Phase();
                    tmpP.Name = p.ToString();
                    tmpP.Enabled = false;

                    _Phasen.Add(tmpP);
                }
            }
            Elemente.Add(new Element("Fe", "", ValKind.None, false));

            Conditions.Add(new Element("T", "1000", ValKind.DGC, false));
            Conditions.Add(new Element("P", "1", ValKind.BAR, false));
            Conditions.Add(new Element("N", "1", ValKind.None, false));
        }

        void Elemente_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged("Child changed");
        }

        //public Werkstoff DeepClone ()
        //{
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        BinaryFormatter formatter = new BinaryFormatter();
        //        formatter.Serialize(stream, this);
        //        stream.Position = 0;
        //        return (Werkstoff)formatter.Deserialize(stream);
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public override string ToString()
        {
            return Name;
        }

        static public implicit operator string(Makro ws)
        {
            return ws.Name;
        }
    }

    [Serializable]
    public class TextElement : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _txt;
        public string txt
        {
            get
            {
                return _txt;
            }
            set
            {
                _txt = value;
                NotifyPropertyChanged("Text");
            }
        }

        public TextElement()
        {
            
        }

        public TextElement(string _Name, string _Text)
        {
            _txt = _Text;
            _name = _Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    [Serializable]
    public class Elemente : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public BindingList<Element> ElementListe = new BindingList<Element>();

        public Elemente() { }

        public Elemente(Elemente CopyFrom)
        {
            _Name = CopyFrom.Name;
            foreach (Element p in CopyFrom.ElementListe)
            {
                ElementListe.Add(new Element(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        //public override string ToString()
        //{
        //    return Name;
        //}
    }

    [Serializable]
    public class Phasen : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public BindingList<Phase> PhasenListe = new BindingList<Phase>();

        public Phasen() { }

        public Phasen(Phasen CopyFrom)
        {
            _Name = CopyFrom.Name;
            foreach (Phase p in CopyFrom.PhasenListe)
            {
                PhasenListe.Add(new Phase(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    [Serializable]
    public class Element : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }
        private string _Value;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                NotifyPropertyChanged("Value");
            }
        }

        private ValKind _kind;
        public ValKind Kind
        {
            get
            {
                return _kind;
            }
            set
            {
                _kind = value;
                NotifyPropertyChanged("Kind");
            }
        }

        private bool _disabled;
        public bool Disabled
        {
            get
            {
                return _disabled;
            }
            set
            {
                _disabled = value;
                NotifyPropertyChanged("Disabled");
            }
        }

        public Element()
        {
            _disabled = false;
        }

        public Element(Element CopyFrom)
        {
            _disabled = CopyFrom.Disabled;
            _Name = CopyFrom.Name;
            _Value = CopyFrom.Value;
            _kind = CopyFrom._kind;
        }

        public Element(string Name, string Value, ValKind Einheit, bool Disabled)
        {
            _disabled = Disabled;
            _Name = Name;
            _Value = Value;
            _kind = Einheit;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    [Serializable]
    public class Phase : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private bool _enabled;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                NotifyPropertyChanged("Disabled");
            }
        }

        public Phase()
        {
            _enabled = false;
        }

        public Phase(Phase CopyFrom)
        {
            _enabled = CopyFrom.Enabled;
            _Name = CopyFrom.Name;
        }

        public Phase(string Name, bool Enabled)
        {
            _enabled = Enabled;
            _Name = Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
