using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace EmailClient
{
    public partial class Form1 : Form
    {
        #region properties
        BackgroundWorker worker;
        EmailItem item;
        #endregion

        #region init
        public Form1()
        {
            InitializeComponent();
            LoadSettings();

            // worker
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
        }

        private void LoadSettings()
        {
            textBoxHostname.Text = Properties.Settings.Default.Host;
            numericUpDownPort.Value = (string.IsNullOrWhiteSpace(Properties.Settings.Default.Port)) ? 456 : Convert.ToInt32(Properties.Settings.Default.Port);
            textBoxUsername.Text = Properties.Settings.Default.Username;
            textBoxPassword.Text = Properties.Settings.Default.Password;
            textBoxFrom.Text = Properties.Settings.Default.From;
            textBoxTo.Text = Properties.Settings.Default.To;
            textBoxSubject.Text = Properties.Settings.Default.Subject;
            textBoxBody.Text = Properties.Settings.Default.Body;
        }
        #endregion

        #region BG worker
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (string sendTo in item.to)
            {
                Do(sendTo);
                worker.ReportProgress(1, sendTo);
                Thread.CurrentThread.Join(2000);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value += e.ProgressPercentage;
            lblProgresInfo.Text = string.Format("Odesílám na adresu: {0} [{1}/{2}]", e.UserState.ToString(), progressBar1.Value, item.to.Length);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Zpráva byla odeslána na: " +string.Join(",", item.to));
            progressBar1.Value = 0;
            lblProgresInfo.Text = "---";
        }
        #endregion

        private void buttonAttachFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    ListViewItem item = new ListViewItem(file);
                    FileInfo info = new FileInfo(file);
                    item.SubItems.Add(string.Format("{0:N0}KB", info.Length / 1024));
                    listViewFiles.Items.Add(item);
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (listViewFiles.SelectedItems.Count > 0)
                    {
                        List<ListViewItem> deletedItems = new List<ListViewItem>();
                        foreach (ListViewItem item in listViewFiles.SelectedItems)
                        {
                            deletedItems.Add(item);
                        }

                        foreach (ListViewItem item in deletedItems)
                        {
                            item.Remove();
                        }
                    }
                    break;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            List<string> attachments = new List<string>();
            foreach (ListViewItem item in listViewFiles.Items)
            {
                attachments.Add(item.Text);
            }

            try
            {
                SendEmail(textBoxHostname.Text, (int)numericUpDownPort.Value,
                    textBoxUsername.Text, textBoxPassword.Text,
                    textBoxFrom.Text, textBoxTo.Text,
                    textBoxSubject.Text, textBoxBody.Text,
                    attachments);
            }
            catch (SmtpException ex)
            {
                MessageBox.Show("Chyba při odesílání: " + ex.Message);
            }


        }

        private void SendEmail(string host, int port, string username, string password, string from, string to, string subject, string body, ICollection<string> attachedFiles)
        {
            item = new EmailItem(host, port, username, password, from, to, subject, body, attachedFiles);

            // progress bar
            progressBar1.Maximum = item.to.Length;
            progressBar1.Value = 0;

            worker.RunWorkerAsync();
        }

        public void Do(string sendTo)
        {         
            //foreach (string sendTo in item.to)
            //{
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(item.from);
                    message.To.Add(sendTo);
                    message.Subject = item.subject;
                    message.Body = item.body;
                    foreach (string file in item.attachedFiles)
                    {
                        message.Attachments.Add(new Attachment(file));
                    }

                    SmtpClient client = new SmtpClient(item.host, item.port);
                    //if your SMTP server requires a password, this line is important
                    client.Credentials = new NetworkCredential(item.username, item.password);
                    //this send is syncronous. You can also choose to send asyncronously
                    client.Send(message);
                }
            //}
        }

        private void btnContacts_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Host = textBoxHostname.Text;
            Properties.Settings.Default.Port = numericUpDownPort.Value.ToString();
            Properties.Settings.Default.Username = textBoxUsername.Text;
            Properties.Settings.Default.Password = textBoxPassword.Text;
            Properties.Settings.Default.From = textBoxFrom.Text;
            Properties.Settings.Default.To = textBoxTo.Text;
            Properties.Settings.Default.Subject = textBoxSubject.Text;
            Properties.Settings.Default.Body = textBoxBody.Text;

            Properties.Settings.Default.Save();
        }
    }



}




