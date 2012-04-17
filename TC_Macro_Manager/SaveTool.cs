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
    public partial class SaveTool : UserControl
    {
        public Control RelControl;

        public object DataSource
        {
            get
            {
                return comboBoxList.DataSource;
            }
            set
            {
                comboBoxList.DataSource = value;
            }
        }

        public SaveTool()
        {
            InitializeComponent();
            comboBoxList.DisplayMember = "Name";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string newName = "";
            Asker ask = new Asker();
            ask.StartPosition = FormStartPosition.CenterScreen;
            ask.Answer = comboBoxList.Text;
            ask.Question = "Name für neuen Datensatz angeben";
            DialogResult dr = ask.ShowDialog();

            if (dr == DialogResult.OK)
            {
                if (comboBoxList.Text == ask.Answer)
                {
                    DialogResult dr2 = MessageBox.Show("Die Liste mit dem Namen " + ask.Answer + " überschreiben?", "Überschreiben", MessageBoxButtons.YesNo);
                    if (dr2 != DialogResult.Yes) return;
                    deleteCurrent();
                }
                newName = ask.Answer;
            }
            else
            {
                return;
            }

            if (DataSource.GetType() == typeof(BindingList<Elemente>))
            {
                BindingList<Element> newList = new BindingList<Element>();

                foreach (Element elm in (BindingList<Element>)((DataGridView) RelControl).DataSource)
                {
                    newList.Add(new Element(elm));
                }

                Elemente el = new Elemente();

                el.ElementListe = newList;
                el.Name = newName;

                ((BindingList<Elemente>)DataSource).Add(el);
                comboBoxList.SelectedItem = el;
            }
            else if (DataSource.GetType() == typeof(BindingList<Phasen>))
            {
                BindingList<Phase> newList = new BindingList<Phase>();

                foreach (Phase elm in (BindingList<Phase>)((DataGridView)RelControl).DataSource)
                {
                    newList.Add(new Phase(elm));
                }

                Phasen el = new Phasen();

                el.PhasenListe = newList;
                el.Name = newName;

                ((BindingList<Phasen>)DataSource).Add(el);
                comboBoxList.SelectedItem = el;
            }
            else if (DataSource.GetType() == typeof(BindingList<TextElement>))
            {
                TextElement el = new TextElement(newName, ((TextBox)RelControl).Text);

                ((BindingList<TextElement>)DataSource).Add(el);
                comboBoxList.SelectedItem = el;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (comboBoxList.SelectedItem == null) return;
            if (!AskDelete()) return;
            deleteCurrent();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {

            if (DataSource.GetType() == typeof(BindingList<Elemente>))
            {
                BindingList<Element> CurList = (BindingList<Element>)((DataGridView)RelControl).DataSource;

                if (CurList.Count > 0)
                {
                    if (!AskOverWrite())
                    {
                        return;
                    }
                }

                CurList.Clear();
                foreach (Element elm in ((Elemente)comboBoxList.SelectedItem).ElementListe)
                {
                    CurList.Add(new Element(elm));
                }

            }
            else if (DataSource.GetType() == typeof(BindingList<Phasen>))
            {
                BindingList<Phase> CurList = (BindingList<Phase>)((DataGridView)RelControl).DataSource;

                if (CurList.Count > 0)
                {
                    if (!AskOverWrite())
                    {
                        return;
                    }
                }
                CurList.Clear();
                foreach (Phase elm in ((Phasen)comboBoxList.SelectedItem).PhasenListe)
                {
                    CurList.Add(new Phase(elm));
                }
            }
            else if (DataSource.GetType() == typeof(BindingList<TextElement>))
            {
                if (((TextBox)RelControl).Text != null)
                {
                    if (!AskOverWrite())
                    {
                        return;
                    }
                }

                ((TextBox)RelControl).Text = ((TextElement)comboBoxList.SelectedItem).txt;
            }
        }

        private void deleteCurrent()
        {
            if (DataSource.GetType() == typeof(BindingList<Elemente>))
            {
                ((BindingList<Elemente>)DataSource).Remove(((Elemente)comboBoxList.SelectedItem));
            }
            else if (DataSource.GetType() == typeof(BindingList<Phasen>))
            {
                ((BindingList<Phasen>)DataSource).Remove(((Phasen)comboBoxList.SelectedItem));
            }
            else if (DataSource.GetType() == typeof(BindingList<TextElement>))
            {
                ((BindingList<TextElement>)DataSource).Remove(((TextElement)comboBoxList.SelectedItem));
            }
        }

        private bool AskOverWrite()
        {
            DialogResult dr = MessageBox.Show("Die derzeitigen Daten überschrieben?", "Daten öffnen", MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes) return true;
            else return false;
        }

        private bool AskDelete()
        {
            DialogResult dr = MessageBox.Show("Den Datensatz löschen?", "Daten löschen", MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes) return true;
            else return false;
        }
    }
}
