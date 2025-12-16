using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lists.WindowsApplication
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //nop
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearForm();
        }

        private void clearForm()
        {
            txtMessage.Text = "";
            radTypeAnnouncement.Checked = false;
            radTypeInformation.Checked = false;
            radTypeWarning.Checked = false;
            dtpShowFrom.Value = DateTime.Today;
            dtpShowTo.Value = DateTime.Today.AddDays(1);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (validateFields())
            {
                saveMessage();
                clearForm();
                showNotification("Message has been saved", false);
            }
        }

        private void saveMessage()
        {
            var message = new SiteMessage
            {
                ShowFrom = dtpShowFrom.Value,
                ShowTo = dtpShowTo.Value,
                Text = txtMessage.Text,
                Type = radTypeAnnouncement.Checked ? 
                    SiteMessageType.Announcment : 
                    radTypeInformation.Checked ?
                    SiteMessageType.Information : 
                    SiteMessageType.Warning
            };
            message.Save();

        }

        private bool validateFields()
        {
            bool success = true;

            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                showNotification("Enter a valid message");
                lblNotification.ForeColor = System.Drawing.Color.Red;
                success = false;
            }
            else if (!radTypeAnnouncement.Checked && !radTypeInformation.Checked && !radTypeWarning.Checked)
            {
                showNotification("Select a valid type");
                success = false;
            }
            else if (dtpShowFrom.Value > dtpShowTo.Value)
            {
                showNotification("Select a show from date that is the to date");
                success = false;
            }
            else if (dtpShowFrom.Value == dtpShowTo.Value)
            {
                showNotification("Show from and show to cannot be equal");
                success = false;
            }

            return success;
        }

        private void showNotification(string message, bool isError = true)
        {
            lblNotification.Text = message;
            lblNotification.ForeColor = isError ? System.Drawing.Color.Red : System.Drawing.Color.Green;
        }
    }
}
