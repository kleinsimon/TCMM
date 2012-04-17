using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TC_Macro_Manager
{
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class DataGridViewEx : DataGridView
    {
        public DataGridViewEx()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(DataGridViewEx_KeyDown);
            CurrentCellDirtyStateChanged += new EventHandler(DataGridViewEx_CurrentCellDirtyStateChanged);
        }

        void DataGridViewEx_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            EndEdit();
            BeginEdit(false);
        }

        void DataGridViewEx_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                paseHelper(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void paseHelper(KeyEventArgs e)
        {
            if (this.CurrentCellAddress.Y == this.Rows.Count - 1)
            {
                return;
            }
                
            //if user clicked Shift+Ins or Ctrl+V (paste from clipboard)

            if ((e.Shift && e.KeyCode == Keys.Insert) || (e.Control && e.KeyCode == Keys.V))
            {

                char[] rowSplitter = { '\r', '\n' };

                char[] columnSplitter = { '\t' };

                //get the text from clipboard

                IDataObject dataInClipboard = Clipboard.GetDataObject();

                string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);

                //split it into lines

                string[] rowsInClipboard = stringInClipboard.Split(rowSplitter, StringSplitOptions.RemoveEmptyEntries);

                //get the row and column of selected cell in grid

                int r = this.SelectedCells[0].RowIndex;

                int c = this.SelectedCells[0].ColumnIndex;

                //add rows into grid to fit clipboard lines

                if (this.Rows.Count < (r + rowsInClipboard.Length))
                {
                    //int needed = r + rowsInClipboard.Length - this.Rows.Count;
                }

                // loop through the lines, split them into cells and place the values in the corresponding cell.

                for (int iRow = 0; iRow < rowsInClipboard.Length; iRow++)
                {

                    //if (Rows.Count - 1 <= r + iRow)
                    //{
                    //    continue;
                    //}

                    if (this.DataSource.GetType() == typeof(BindingList<Element>))
                    {
                        BindingList<Element> bs = (BindingList<Element>)this.DataSource;
                        if (Rows.Count - 1 <= r + iRow)
                        {
                            bs.AddNew();
                        }
                    }
                    else if (this.DataSource.GetType() == typeof(BindingList<Phase>))
                    {
                        BindingList<Phase> bs = (BindingList<Phase>)this.DataSource;
                        if (Rows.Count - 1 <= r + iRow)
                        {
                            bs.AddNew();
                        }
                    }
                    else
                    {
                        this.Rows.Add();
                    }

                    //split row into cell values

                    string[] valuesInRow = rowsInClipboard[iRow].Split(columnSplitter);

                    //cycle through cell values

                    for (int iCol = 0; iCol < valuesInRow.Length; iCol++)
                    {

                        //assign cell value, only if it within columns of the grid

                        if (this.ColumnCount - 1 >= c + iCol)
                        {
                            DataGridViewCell cell = this.Rows[r + iRow].Cells[c + iCol];


                            try
                            {
                                if (cell.EditType != typeof(DataGridViewComboBoxEditingControl))
                                {
                                    cell.Value = valuesInRow[iCol];
                                }
                                else
                                {
                                    this.CurrentCell = cell;
                                    this.BeginEdit(true);
                                    ((DataGridViewComboBoxEditingControl)this.EditingControl).Text = valuesInRow[iCol];
                                }

                                this.EndEdit();

                                this.Update();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }

                    }

                }

            }
        }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            //base.OnDataError(displayErrorDialogIfNoHandler, e);
        }
    }
}
