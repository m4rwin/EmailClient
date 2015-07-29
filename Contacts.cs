using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmailClient
{
    public partial class Contacts : Form
    {
        private DataGridView DataGridView1 = new DataGridView();
        //private List<ContactItem> Contacst;
        private Form1 form;
            

        public Contacts()
        {
            InitializeComponent();
            SetUpForm();
            this.Text = "Adresář";
            //this.grid.AllowUserToAddRows = true;
        }

        private void GetRowCount()
        {
            lblItemsCount.Text = "Počet kontaktů: " + (grid.Rows.Count-1);
        }

        public Contacts(/*List<ContactItem> c, */Form1 f)
            :this()
        {
            //this.Contacst = c;
            //grid.DataSource = this.Contacst;

            this.form = f;

            grid.ColumnCount = 4;
            grid.Columns[0].Width = 50;
            grid.Columns[1].Width = 130;
            grid.Columns[2].Width = 180;
            grid.Columns[3].Width = 180;

            grid.Columns[1].Name = "Name";
            grid.Columns[2].Name = "Email";
            grid.Columns[3].Name = "Note";

            grid.Columns[0].HeaderText = "";
            grid.Columns[1].HeaderText = "Jméno";
            grid.Columns[2].HeaderText = "Adresa";
            grid.Columns[3].HeaderText = "Poznámka";

            List<ContactItem> list = ContactItem.LoadContacst(Properties.Settings.Default.Contacts);
            foreach (ContactItem item in list)
            {
                AddContact(item.Name, item.Email, item.Note);
            }

            int x = form.Location.X;
            int y = form.Location.Y;
            this.Location = new Point(x, y);

            GetRowCount();
        }

        private void SetUpForm()
        {
            DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn()
            {
                Name = "selected",
                HeaderText = "---",
                Frozen = false
            };
            grid.Columns.Add(col);
        }

        /*
         * CZE: Nacteni kontatku ze souboru
         * ENG: Load contacts from file
         */
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            List<ContactItem> list = new List<ContactItem>();

            if (result == DialogResult.OK)
            {
                form.SourceFile = dialog.FileName;
                try
                {
                    using (StreamReader sr = new StreamReader(dialog.FileName))
                    {
                        String line = sr.ReadToEnd();
                        list = ContactItem.LoadContacst(line);
                    }

                    grid.Rows.Clear();
                    foreach (ContactItem item in list)
                    {
                        AddContact(item.Name, item.Email, item.Note);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načítání souboru.");
                }
                GetRowCount();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<ContactItem> SelectedItems = new List<ContactItem>();
            DataGridViewRow row = new DataGridViewRow();
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                row = grid.Rows[i];
                if (Convert.ToBoolean(row.Cells["selected"].Value) == true &&
                    (grid.Rows[i].Cells["Name"].Value != null && grid.Rows[i].Cells["Email"].Value!= null && grid.Rows[i].Cells["Note"].Value != null))
                {
                    SelectedItems.Add(new ContactItem { Name = grid.Rows[i].Cells["Name"].Value.ToString(), Email = grid.Rows[i].Cells["Email"].Value.ToString(), Note = grid.Rows[i].Cells["Note"].Value.ToString() });
                }
            }
            form.SetRecepient(SelectedItems);
        }

        private void Contacts_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                List<ContactItem> list = new List<ContactItem>();
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    if (grid.Rows[i].Cells["Name"].Value == null &&
                        grid.Rows[i].Cells["Email"].Value == null &&
                        grid.Rows[i].Cells["Note"].Value == null)
                    { continue; }

                    list.Add(new ContactItem() { Name = grid.Rows[i].Cells["Name"].Value.ToString(), Email = grid.Rows[i].Cells["Email"].Value.ToString(), Note = grid.Rows[i].Cells["Note"].Value.ToString() });
                }

                Properties.Settings.Default.Contacts = ContactItem.StoreContacst(list);
                Properties.Settings.Default.Save();

                Hide();
                e.Cancel = true; // this cancels the close event.
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (grid.Rows[i].Cells["Name"].Value != null && grid.Rows[i].Cells["Email"].Value != null && grid.Rows[i].Cells["Note"].Value != null)
                {
                    grid.Rows[i].Cells["selected"].Value = true;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].Cells["selected"].Value = false;
            }
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            /*if (form.SourceFile != null || form.SourceFile != string.Empty)
            {
                try
                {
                    SaveContacst(form.SourceFile);
                    MessageBox.Show("Kontakty byly uloženy do souboru: " + form.SourceFile, "Oznámení", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {*/
                SaveFileDialog dialog = new SaveFileDialog();
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    try
                    {
                        form.SourceFile = dialog.FileName;
                        SaveContacst(form.SourceFile);
                        MessageBox.Show("Kontakty byly uloženy do souboru: " + form.SourceFile, "Oznámení", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            //}
        }

        private void SaveContacst(string file)
        {
            using (StreamWriter sw = new StreamWriter(file, false))
            {
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    if (grid.Rows[i].Cells["Name"].Value == null &&
                        grid.Rows[i].Cells["Email"].Value == null &&
                        grid.Rows[i].Cells["Note"].Value == null)
                    { continue; }

                    sw.WriteLine(string.Format("{0};{1};{2};",
                        grid.Rows[i].Cells["Name"].Value,
                        grid.Rows[i].Cells["Email"].Value,
                        grid.Rows[i].Cells["Note"].Value));

                }
            }
        }

        /*
         * CZE: Pridani kontaktu
         * ENG: Add new contact
         */
        private void button6_Click(object sender, EventArgs e)
        {
            this.grid.Rows.Add();
        }

        public void AddContact(string Name, string Email, string Note)
        {
            string[] row = {"false", Name.Trim(), Email.Trim(), Note.Trim() };
            this.grid.Rows.Add(row);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            List<int> remove = new List<int>();
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (Convert.ToBoolean(grid.Rows[i].Cells["selected"].Value))
                {
                    remove.Add(i);
                }
            }

            
            DialogResult result = MessageBox.Show("Opravdu chceš smazat označené kontakty?", "Info", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                for (int ii = remove.Count - 1; ii >= 0; ii--)
                {
                    grid.Rows.RemoveAt(remove[ii]);
                }

                GetRowCount();
            }
        }
    }
}
