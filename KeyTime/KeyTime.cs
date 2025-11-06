using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timeline;

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
            UpdateView();
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
                macros = parsed.macros;
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

        private void menuTimelineAddTrack_Click(object sender, EventArgs e)
        {
            timelineControl.AddTrack();
        }

        private void menuTimelineRemoveFirst_Click(object sender, EventArgs e)
        {
            if (timelineControl.Controls.OfType<Panel>().Any())
            {
                timelineControl.RemoveTrack(0);
                resultsLabel.Text = "Removed First Track.";
            }
            else
            {
                resultsLabel.Text = "Could not remove First Track.";
            }
        }

        private void menuTimelineRemoveLast_Click(object sender, EventArgs e)
        {
            if (timelineControl.Controls.OfType<Panel>().Any())
            {
                int lastIndex = timelineControl.Controls.OfType<Panel>().Count() - 1;
                timelineControl.RemoveTrack(lastIndex);
                resultsLabel.Text = "Removed Last Track.";
            }
            else
            {
                resultsLabel.Text = "Could not remove Last Track.";
            }
        }

        private void menuTimelineRemoveIndex_Click(object sender, EventArgs e)
        {
            string value = InputDialog.Show($"Tracks are 0 indexed.\nRemove Track at index:");
            if (value != null && int.TryParse(value, out int trackIndex))
            {
                timelineControl.RemoveTrack(trackIndex);
                resultsLabel.Text = $"Removed track at {trackIndex}.";
            }
            else
            {
                resultsLabel.Text = $"Could not remove track at {value}.";
            }
        }

        private void menuViewToggleCode_Click(object sender, EventArgs e)
        {
            txtMainView.Visible = !txtMainView.Visible;
            UpdateView();
        }

        private void menuViewToggleTimeline_Click(object sender, EventArgs e)
        {
            groupTimeline.Visible = !groupTimeline.Visible;
            UpdateView();
        }

        private void UpdateView()
        {
            int padding = 10;
            int totalWidth = this.ClientSize.Width;

            int leftWidth = (int)(totalWidth * 0.2) - (padding / 2);
            int rightWidth = (int)(totalWidth * 0.8) - (padding / 2);

            if (txtMainView.Visible && groupTimeline.Visible)
            {
                txtMainView.Left = padding;
                txtMainView.Width = leftWidth;

                txtMainView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

                groupTimeline.Left = txtMainView.Right + padding;
                groupTimeline.Width = rightWidth;

                groupTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            }
            else if (txtMainView.Visible && !groupTimeline.Visible)
            {
                txtMainView.Left = padding;
                txtMainView.Width = totalWidth - (2 * padding);

                txtMainView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }
            else if (!txtMainView.Visible && groupTimeline.Visible)
            {
                groupTimeline.Left = padding;
                groupTimeline.Width = totalWidth - (2 * padding);

                groupTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }
        }

        private void menuTimelineChangeWidth_Click(object sender, EventArgs e)
        {
            string value = InputDialog.Show($"New Timeline width: ");
            if (value != null && int.TryParse(value, out int width))
            {
                timelineControl.SetTimelineWidth(width);
                resultsLabel.Text = $"Removed track at {width}.";
            }
            else
            {
                resultsLabel.Text = $"Could not make timeline of length {value}.";
            }

        }

        private void menuTimelineConvert_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtMainView.Text))
            {
                string value = InputDialog.Show($"The current code will be cleared. Is this OK (Y/n):");
                if (value != "Y")
                    return;
            }
            resultsLabel.Text = "Generating Code...";
            resultsLabel.Refresh();
            txtMainView.Text = "";
            List<TimelineControl.TimelineData> timelines = timelineControl.GetTimelineData();
            foreach (TimelineControl.TimelineData data in timelines)
            {
                txtMainView.Text += $"macro {data.TrackIndex}\r\n";
                int currentTime = 0;
                foreach (TimelineControl.ClipData clipData in data.Clips)
                {
                    if (currentTime < clipData.StartTime)
                    {
                        txtMainView.Text += $" sleep {clipData.StartTime - currentTime}\r\n";
                    }
                    else if (currentTime < clipData.StartTime)
                    {
                        resultsLabel.Text = "Error, could not convert!";
                    }
                    txtMainView.Text += $" press {clipData.Character}\r\n sleep {clipData.EndTime - clipData.StartTime}\r\n unpress {clipData.Character}\r\n";
                    currentTime = clipData.EndTime;
                }
                txtMainView.Text += "\r\n";
            }
            menuParseParse_Click(sender, e);
            resultsLabel.Text = "Converted to code, and parsed.";
        }

        private void menuParseConvert_Click(object sender, EventArgs e)
        {
            try
            {
                TimelineParser parsedMacros = new TimelineParser(new Data(txtMainView.Text));
                timelineControl.LoadTimelineData(parsedMacros.timelines);
                resultsLabel.Text = "Converted to a timeline successfully.";
            }
            catch (ParseException ex)
            {
                resultsLabel.Text = $"Conversion Failed: {ex.Message}";
            }
        }
    }
}
