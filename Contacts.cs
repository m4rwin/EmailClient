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
        private List<ContactItem> Contacst;
        private Form1 form;

        public Contacts()
        {
            InitializeComponent();
            SetUpForm();
            this.Text = "Adresář";
        }

        public Contacts(List<ContactItem> c, Form1 f)
            :this()
        {
            this.Contacst = c;
            grid.DataSource = this.Contacst;
            this.form = f;

            grid.Columns[0].Width = 50;
            grid.Columns[1].Width = 130;
            grid.Columns[2].Width = 180;
            grid.Columns[3].Width = 180;

            int x = form.Location.X;
            int y = form.Location.Y;
            this.Location = new Point(x, y);
        }

        private void SetUpForm()
        {
            //Size = new Size(800, 600);
            //FlowLayoutPanel flowLayout = new FlowLayoutPanel();
            //flowLayout.FlowDirection = FlowDirection.TopDown;
            //flowLayout.Dock = DockStyle.Fill;
            //Controls.Add(flowLayout);

            //flowLayout.Controls.Add(DataGridView1);
            //DataGridView1.Size = Size;

            DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn()
            {
                Name = "selected",
                HeaderText = "---",
                Frozen = false
            };
            grid.Columns.Add(col);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(dialog.FileName, Encoding.UTF8))
                    {
                        String line = sr.ReadToEnd();
                        this.Contacst = ContactItem.LoadContacst(line);
                    }
                    grid.DataSource = this.Contacst;
                    form.Contacst = this.Contacst;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The file could not be read: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<ContactItem> SelectedItems = new List<ContactItem>();
            DataGridViewRow row = new DataGridViewRow();
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                row = grid.Rows[i];
                if (Convert.ToBoolean(row.Cells["selected"].Value) == true)
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
                Hide();
                e.Cancel = true; // this cancels the close event.
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].Cells["selected"].Value = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].Cells["selected"].Value = false;
            }
        }      
    }
}
