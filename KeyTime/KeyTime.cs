using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyTime
{
    public partial class KeyTime : Form
    {
        private string lastFile = string.Empty; // To hold the currently open file, will be expanded to list as to hold multiple open files.
        private Parser parsed;
        private int startupTime = 1000;
        public KeyTime()
        {
            PercisionTimer.InitHighResolution(); // High resolution clock for this process runtime, will be turned off later.
            InitializeComponent();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            PercisionTimer.DeinitHighResolution();
            base.OnFormClosed(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                menuFileSave_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.S))
            {
                menuFileSaveAs_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.O))
            {
                menuFileOpen_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.P))
            {
                menuParseParse_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.R))
            {
                menuParseRunMacros_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            this.lastFile = ofd.FileName;
            txtMainView.Text = System.IO.File.ReadAllText(this.lastFile);
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.lastFile))
            {
                SaveFileDialog ofd = new SaveFileDialog();
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                this.lastFile = ofd.FileName;
                File.WriteAllText(this.lastFile, txtMainView.Text);
            }
            else
            {
                File.WriteAllText(this.lastFile, txtMainView.Text);
            }
        }

        private void menuFileSaveAs_Click(object sender, EventArgs e)
        {
            this.lastFile = string.Empty;
            menuFileSave_Click(sender, e);
        }

        private void menuParseParse_Click(object sender, EventArgs e)
        {
            try
            {
                parsed = new Parser(new Data(txtMainView.Text));
                resultsLabel.Text = "Parsed Successfuly";
            }
            catch (ParseException ex)
            {
                resultsLabel.Text = $"Parse Failed: {ex.Message}";
            }

        }
        private async void menuParseRunMacros_Click(object sender, EventArgs e)
        {

            Dictionary<String, Action> macros;
            try
            {
                macros = parsed.GetMacros();
            }
            catch
            {
                resultsLabel.Text = "No macros to start.";
                return;
            }
            resultsLabel.Text = "Startup Time Wait...";
            resultsLabel.Refresh();
            PercisionTimer.AccurateDelay(startupTime);
            resultsLabel.Text = "Starting Macros...";
            resultsLabel.Refresh();
            await Task.WhenAll(macros.Values.Select(macro => Task.Run(macro)));
            resultsLabel.Text = "Completed";
        }

        private void menuParseStartup_Click(object sender, EventArgs e)
        {
            string value = InputDialog.Show($"Startup Time currently is: {startupTime}\nTime before macro starts (milliseconds): ");
            if (value != null && int.TryParse(value, out int sTime))
            {
                startupTime = sTime;
                resultsLabel.Text = $"Startup time set to {startupTime}ms.";
            }
            else
            {
                resultsLabel.Text = $"Startup time remains {startupTime}ms.";
            }
        }
    }
}
