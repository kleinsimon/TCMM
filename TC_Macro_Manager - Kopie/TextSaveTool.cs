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
    public partial class TextBoxSaveTool : UserControl
    {
        public TextBox RelTextBox;

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

        public TextBoxSaveTool()
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

            if (DataSource.GetType() == typeof(BindingList<TextElement>))
            {
                TextElement el = new TextElement(newName, RelTextBox.Text);

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

            if (DataSource.GetType() == typeof(BindingList<TextElement>))
            {
                if (RelTextBox.Text != null)
                {
                    if (!AskOverWrite())
                    {
                        return;
                    }
                }

                RelTextBox.Text = ((TextElement)comboBoxList.SelectedItem).txt;
            }
        }

        private void deleteCurrent()
        {
            if (DataSource.GetType() == typeof(BindingList<TextElement>))
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
