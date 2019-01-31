using System.Diagnostics;
using System.Windows.Forms;

namespace EmsTool
{
    partial class MainForm : Form
    {
        bool _isAuthenticated;

        public MainForm()
        {
            InitializeComponent();

            if (Debugger.IsAttached)
            {
                txtUserName.Text = "rubal.walia";
                txtPassword.Text = "dots@123";
            }
        }

        #region properties

        string UserName
        {
            get => txtUserName.Text;
        }

        string Password
        {
            get => txtPassword.Text;
        }

        bool IsAuthenticated
        {
            get => _isAuthenticated;
        }

        #endregion

        private async void btnLogin_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("User name not entered.");
                txtUserName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Password not entered.");
                txtUserName.Focus();
                return;
            }

            btnLogin.Enabled = false;

            var userName = UserName.EndsWith("@dotsquares.com") ? UserName : string.Format("{0}@dotsquares.com", UserName);
            var data = await ApiClient.Instance.Authenticate(userName, Password);
            if (data.Key != null)
            {
                MessageBox.Show(data.Key);
                txtUserName.Focus();
                return;
            }

            _isAuthenticated = data.Value;

            await timesheetFiller.Populate();
            await attendance.Populate();
            tabContainer.Enabled = IsAuthenticated;

            btnLogin.Enabled = true;
        }
    }
}
