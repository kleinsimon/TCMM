using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace TC_Macro_Manager
{
    public partial class MainForm : Form
    {
        public MacroData MData;
        public Makro CurrentMakro= new Makro();
        public BindingList<Makro> Makros;
        public BindingList<Elemente> Werkstoffe;
        public BindingList<Elemente> Bedingungen;
        public BindingList<Phasen> PhasenSets;
        private string dataFile;
        public TCMacro TCM = new TCMacro();
        private const string ext = ".tcm";

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttunAddWS_Click(object sender, EventArgs e)
        {
            Makro n = Makros.AddNew();
            n.initDefaults();
            listBoxMakros.SelectedItem = n;
        }

        private void buttonDelWS_Click(object sender, EventArgs e)
        {
            Makro ws = CurrentMakro;
            Makros.Remove(ws);
            ws = null;
        }

        private void buttonWsSave_Click(object sender, EventArgs e)
        {
            if (CurrentMakro == null) return;
            CurrentMakro.Name = textBoxWsName.Text;
            saveData();
        }

        private void comboBoxWS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxMakros.SelectedItem != null)
            {
                CurrentMakro = (Makro)listBoxMakros.SelectedItem;
                textBoxWsName.Text = CurrentMakro.Name;
                dataGridViewElm.DataSource = CurrentMakro.Elemente;
                dataGridViewCond.DataSource = CurrentMakro.Conditions;
                textBoxDB.DataBindings.Clear();
                textBoxDB.DataBindings.Add(new Binding("Text", CurrentMakro, "DB", false, DataSourceUpdateMode.OnPropertyChanged));
                textBoxFolder.DataBindings.Clear();
                textBoxFolder.DataBindings.Add(new Binding("Text", CurrentMakro, "Folder", false, DataSourceUpdateMode.OnPropertyChanged));
                textBoxFileName.DataBindings.Clear();
                textBoxFileName.DataBindings.Add(new Binding("Text", CurrentMakro, "FileName", false, DataSourceUpdateMode.OnPropertyChanged));

                dataGridViewPhases.DataSource = CurrentMakro.Phasen;

                refreshEditor();
            }
            else
            {
                dataGridViewElm.DataSource = null;
                textBoxWsName.Text = null;
            }
        }

        private void initDataGrid(DataGridView dgv)
        {
            dgv.Columns.Clear();

            dgv.AutoGenerateColumns = false;

            DataGridViewComboBoxColumn cc = new DataGridViewComboBoxColumn();

            cc.DataSource = typeof(ValKind).ToList();
            cc.DisplayMember = "Value";
            cc.ValueMember = "Key";
            cc.DataPropertyName = "Kind";
            cc.Name = "Einheit";

            DataGridViewTextBoxColumn name = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn value = new DataGridViewTextBoxColumn();

            name.DataPropertyName = "Name";
            name.Name = "Name";
            name.SortMode = DataGridViewColumnSortMode.Automatic;

            value.DataPropertyName = "Value";
            value.Name = "Wert";
            value.SortMode = DataGridViewColumnSortMode.Automatic;

            DataGridViewCheckBoxColumn check = new DataGridViewCheckBoxColumn();

            check.DataPropertyName = "Disabled";
            check.Name = "Deaktiviert";

            dgv.Columns.Add(name);
            dgv.Columns.Add(value);
            dgv.Columns.Add(cc);
            dgv.Columns.Add(check);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
            dataFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TCMM\data.xml";

            if (File.Exists(dataFile))
            {
                try
                {
                    this.SuspendLayout();
                    MData = ParseMacroXML(dataFile);

                    this.ResumeLayout();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MData = new MacroData();
                }
            }
            else
            {
                MData = new MacroData();
            }

            Makros = MData.Makros;
            Werkstoffe = MData.Werkstoffe;
            PhasenSets = MData.PhasenSets;
            Bedingungen = MData.Bedingungen;

            initDataGrid(dataGridViewElm);
            initDataGrid(dataGridViewCond);

            dataGridViewPhases.Columns.Clear();

            DataGridViewTextBoxColumn pcn = new DataGridViewTextBoxColumn();
            DataGridViewCheckBoxColumn pcv = new DataGridViewCheckBoxColumn();

            pcn.DataPropertyName = "Name";
            pcn.Name = "Phase";
            pcn.SortMode = DataGridViewColumnSortMode.Automatic;
            pcv.DataPropertyName = "Enabled";
            pcv.Name = "Aktiv";

            dataGridViewPhases.Columns.Add(pcn);
            dataGridViewPhases.Columns.Add(pcv);

            listBoxMakros.DataSource = Makros;
            Makros.AllowNew = true;
            Makros.AllowRemove = true;
            Makros.RaiseListChangedEvents = true;
            Makros.AddingNew += new AddingNewEventHandler(Werkstoffe_AddingNew);
            Makros.ListChanged += new ListChangedEventHandler(Werkstoffe_ListChanged);

            SaveToolElement.DataSource = Werkstoffe;
            SaveToolElement.RelDataGridView = dataGridViewElm;
            SaveToolCondition.DataSource = Bedingungen;
            SaveToolCondition.RelDataGridView = dataGridViewCond;
            SaveToolPhases.DataSource = PhasenSets;
            SaveToolPhases.RelDataGridView = dataGridViewPhases;

            listBoxMakros.Focus();
        }

        void Werkstoffe_ListChanged(object sender, ListChangedEventArgs e)
        {
            refreshEditor();
        }

        public void saveData()
        {
            saveDataToFile(dataFile);
        }

        private void saveDataToFile(string path)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(MacroData));

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            System.Xml.XmlTextWriter w = new System.Xml.XmlTextWriter(path, Encoding.Unicode);
            w.Indentation = 1;
            w.IndentChar = '\t';
            w.Formatting = System.Xml.Formatting.Indented;
            x.Serialize(w, MData);
            w.Close();
        }

        private MacroData ParseMacroXML(string path)
        {
            MacroData res;
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(MacroData));
            System.Xml.XmlTextReader w = new System.Xml.XmlTextReader(path);

            res = (MacroData)x.Deserialize(w);

            w.Close();

            return res;
        }

        public void cloneWS()
        {
            Makro newws;

            using (MemoryStream stream = new MemoryStream())
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Makro));
                x.Serialize(stream, CurrentMakro);
                stream.Position = 0;
                newws = (Makro)x.Deserialize(stream);
            }

            newws.Name += "_Copy";

            Makros.Add(newws);
            listBoxMakros.SelectedItem = newws;
        }

        void Werkstoffe_AddingNew(object sender, AddingNewEventArgs e)
        {
            Makro newItem = new Makro();

            string newName = "Neues Makro ";

            int n = 1;
            while (nameExists(newName + n.ToString()))
            {
                n++;
            }
            newItem.Name = newName + n.ToString();
            e.NewObject = newItem;
            Makros.EndNew(Makros.Count - 1);
        }

        private bool nameExists(string Name)
        {
            foreach (Makro w in Makros)
            {
                if (w.Name == Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = buttonWsSave;
        }

        private void groupBox1_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
        }

        private void buttonCopyWs_Click(object sender, EventArgs e)
        {
            cloneWS();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveData();
            e.Cancel = false;
        }

        private void buttonRefreshMacro_Click(object sender, EventArgs e)
        {
            refreshEditor();
        }

        public void refreshEditor()
        {
            if (CurrentMakro != null)
            {
                textBox1.Text = TCM.makeMacro(CurrentMakro);
            }
        }

        private void dataGridViewElm_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(DataGridView))
            {
                ((DataGridView)sender).EndEdit();
                ((DataGridView)sender).BeginEdit(false);
            }
            else if (sender.GetType() == typeof(TextBox))
            {

            }
        }

        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBoxFolder.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxFolder.Text = folderBrowserDialog1.SelectedPath;
            }

        }

        private void buttonSaveFolder_Click(object sender, EventArgs e)
        {
            saveMacroFile();
        }

        public bool saveMacroFile()
        {
            try
            {
                File.WriteAllText(CurrentMakro.Folder + Path.DirectorySeparatorChar + CurrentMakro.FileName + ext, TCM.makeMacro(CurrentMakro));
                return true;
            }
            catch
            {
                MessageBox.Show("Datei Konnte nicht geschrieben werden");
                return false;
            }
        }

        private void buttonRunMacro_Click(object sender, EventArgs e)
        {
            if (!saveMacroFile()) return;
            Process.Start(CurrentMakro.Folder + Path.DirectorySeparatorChar + CurrentMakro.FileName + ext, TCM.makeMacro(CurrentMakro));
        }

        private void textBoxFolder_Enter(object sender, EventArgs e)
        {
            TextBox tb = ((TextBox)sender);
            tb.Select(tb.Text.Length, 0);
            tb.ScrollToCaret();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            saveFileDialogExport.AddExtension = true;
            saveFileDialogExport.DefaultExt = "xml";
            saveFileDialogExport.Filter = "XML-Datei|*.xml";

            if (saveFileDialogExport.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveDataToFile(saveFileDialogExport.FileName);
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            openFileDialogImport.AddExtension = true;
            openFileDialogImport.DefaultExt = "xml";
            openFileDialogImport.Filter = "XML-Datei|*.xml";

            if (openFileDialogImport.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MacroData NData;
                NData = ParseMacroXML(openFileDialogImport.FileName);

                foreach (Makro m in NData.Makros) MData.Makros.Add(m);
                foreach (Elemente m in NData.Bedingungen) MData.Bedingungen.Add(m);
                foreach (Elemente m in NData.Werkstoffe) MData.Werkstoffe.Add(m);
                foreach (Phasen m in NData.PhasenSets) MData.PhasenSets.Add(m);
            }
        }

    }
}
