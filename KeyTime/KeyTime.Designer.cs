namespace KeyTime
{
    partial class KeyTime
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtMainView = new TextBox();
            menuMain = new MenuStrip();
            menuFile = new ToolStripMenuItem();
            menuFileExit = new ToolStripMenuItem();
            menuFileOpen = new ToolStripMenuItem();
            menuFileSave = new ToolStripMenuItem();
            menuFileSaveAs = new ToolStripMenuItem();
            menuView = new ToolStripMenuItem();
            menuViewToggleCode = new ToolStripMenuItem();
            menuViewToggleTimeline = new ToolStripMenuItem();
            menuParse = new ToolStripMenuItem();
            menuParseParse = new ToolStripMenuItem();
            menuParseRunMacros = new ToolStripMenuItem();
            menuParseStartup = new ToolStripMenuItem();
            menuTimeline = new ToolStripMenuItem();
            menuTimelineConvert = new ToolStripMenuItem();
            menuTimelineAddTrack = new ToolStripMenuItem();
            menuTimelineRemoveFirst = new ToolStripMenuItem();
            menuTimelineRemoveLast = new ToolStripMenuItem();
            menuTimelineRemoveIndex = new ToolStripMenuItem();
            resultsLabel = new Label();
            groupTimeline = new GroupBox();
            timelineControl = new Timeline.TimelineControl();
            menuMain.SuspendLayout();
            groupTimeline.SuspendLayout();
            SuspendLayout();
            // 
            // txtMainView
            // 
            txtMainView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            txtMainView.Location = new Point(8, 27);
            txtMainView.Multiline = true;
            txtMainView.Name = "txtMainView";
            txtMainView.ScrollBars = ScrollBars.Both;
            txtMainView.Size = new Size(243, 407);
            txtMainView.TabIndex = 1;
            // 
            // menuMain
            // 
            menuMain.Items.AddRange(new ToolStripItem[] { menuFile, menuView, menuParse, menuTimeline });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(734, 24);
            menuMain.TabIndex = 2;
            menuMain.Text = "menuStrip1";
            // 
            // menuFile
            // 
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuFileExit, menuFileOpen, menuFileSave, menuFileSaveAs });
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(37, 20);
            menuFile.Text = "File";
            // 
            // menuFileExit
            // 
            menuFileExit.Name = "menuFileExit";
            menuFileExit.Size = new Size(194, 22);
            menuFileExit.Text = "Exit";
            menuFileExit.Click += menuFileExit_Click;
            // 
            // menuFileOpen
            // 
            menuFileOpen.Name = "menuFileOpen";
            menuFileOpen.Size = new Size(194, 22);
            menuFileOpen.Text = "Open Ctrl + O";
            menuFileOpen.Click += menuFileOpen_Click;
            // 
            // menuFileSave
            // 
            menuFileSave.Name = "menuFileSave";
            menuFileSave.Size = new Size(194, 22);
            menuFileSave.Text = "Save Ctrl + S";
            menuFileSave.Click += menuFileSave_Click;
            // 
            // menuFileSaveAs
            // 
            menuFileSaveAs.Name = "menuFileSaveAs";
            menuFileSaveAs.Size = new Size(194, 22);
            menuFileSaveAs.Text = "Save As Ctrl + Shift + S";
            menuFileSaveAs.Click += menuFileSaveAs_Click;
            // 
            // menuView
            // 
            menuView.DropDownItems.AddRange(new ToolStripItem[] { menuViewToggleCode, menuViewToggleTimeline });
            menuView.Name = "menuView";
            menuView.Size = new Size(44, 20);
            menuView.Text = "View";
            // 
            // menuViewToggleCode
            // 
            menuViewToggleCode.Name = "menuViewToggleCode";
            menuViewToggleCode.Size = new Size(159, 22);
            menuViewToggleCode.Text = "Toggle Code";
            menuViewToggleCode.Click += menuViewToggleCode_Click;
            // 
            // menuViewToggleTimeline
            // 
            menuViewToggleTimeline.Name = "menuViewToggleTimeline";
            menuViewToggleTimeline.Size = new Size(159, 22);
            menuViewToggleTimeline.Text = "Toggle Timeline";
            menuViewToggleTimeline.Click += menuViewToggleTimeline_Click;
            // 
            // menuParse
            // 
            menuParse.DropDownItems.AddRange(new ToolStripItem[] { menuParseParse, menuParseRunMacros, menuParseStartup });
            menuParse.Name = "menuParse";
            menuParse.Size = new Size(47, 20);
            menuParse.Text = "Parse";
            // 
            // menuParseParse
            // 
            menuParseParse.Name = "menuParseParse";
            menuParseParse.Size = new Size(186, 22);
            menuParseParse.Text = "Parse Ctrl + P";
            menuParseParse.Click += menuParseParse_Click;
            // 
            // menuParseRunMacros
            // 
            menuParseRunMacros.Name = "menuParseRunMacros";
            menuParseRunMacros.Size = new Size(186, 22);
            menuParseRunMacros.Text = "Run Macros Ctrl + R";
            menuParseRunMacros.Click += menuParseRunMacros_Click;
            // 
            // menuParseStartup
            // 
            menuParseStartup.Name = "menuParseStartup";
            menuParseStartup.Size = new Size(186, 22);
            menuParseStartup.Text = "Change Startup Time";
            menuParseStartup.Click += menuParseStartup_Click;
            // 
            // menuTimeline
            // 
            menuTimeline.DropDownItems.AddRange(new ToolStripItem[] { menuTimelineConvert, menuTimelineAddTrack, menuTimelineRemoveFirst, menuTimelineRemoveLast, menuTimelineRemoveIndex });
            menuTimeline.Name = "menuTimeline";
            menuTimeline.Size = new Size(65, 20);
            menuTimeline.Text = "Timeline";
            // 
            // menuTimelineConvert
            // 
            menuTimelineConvert.Name = "menuTimelineConvert";
            menuTimelineConvert.Size = new Size(192, 22);
            menuTimelineConvert.Text = "Convert To Code";
            menuTimelineConvert.Click += menuTimelineConvert_Click;
            // 
            // menuTimelineAddTrack
            // 
            menuTimelineAddTrack.Name = "menuTimelineAddTrack";
            menuTimelineAddTrack.Size = new Size(192, 22);
            menuTimelineAddTrack.Text = "Add Track";
            menuTimelineAddTrack.Click += menuTimelineAddTrack_Click;
            // 
            // menuTimelineRemoveFirst
            // 
            menuTimelineRemoveFirst.Name = "menuTimelineRemoveFirst";
            menuTimelineRemoveFirst.Size = new Size(192, 22);
            menuTimelineRemoveFirst.Text = "Remove First Track";
            menuTimelineRemoveFirst.Click += menuTimelineRemoveFirst_Click;
            // 
            // menuTimelineRemoveLast
            // 
            menuTimelineRemoveLast.Name = "menuTimelineRemoveLast";
            menuTimelineRemoveLast.Size = new Size(192, 22);
            menuTimelineRemoveLast.Text = "Remove Last Track";
            menuTimelineRemoveLast.Click += menuTimelineRemoveLast_Click;
            // 
            // menuTimelineRemoveIndex
            // 
            menuTimelineRemoveIndex.Name = "menuTimelineRemoveIndex";
            menuTimelineRemoveIndex.Size = new Size(192, 22);
            menuTimelineRemoveIndex.Text = "Remove Track at Index";
            menuTimelineRemoveIndex.Click += menuTimelineRemoveIndex_Click;
            // 
            // resultsLabel
            // 
            resultsLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            resultsLabel.AutoSize = true;
            resultsLabel.Location = new Point(8, 437);
            resultsLabel.Name = "resultsLabel";
            resultsLabel.Size = new Size(257, 15);
            resultsLabel.TabIndex = 3;
            resultsLabel.Text = "The status of commands will be displayed here.";
            // 
            // groupTimeline
            // 
            groupTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupTimeline.Controls.Add(timelineControl);
            groupTimeline.Location = new Point(254, 30);
            groupTimeline.Name = "groupTimeline";
            groupTimeline.Size = new Size(468, 404);
            groupTimeline.TabIndex = 4;
            groupTimeline.TabStop = false;
            groupTimeline.Text = "Tracks";
            // 
            // timelineControl
            // 
            timelineControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            timelineControl.AutoScroll = true;
            timelineControl.BackColor = Color.Black;
            timelineControl.Location = new Point(3, 19);
            timelineControl.Name = "timelineControl";
            timelineControl.Size = new Size(462, 382);
            timelineControl.TabIndex = 0;
            // 
            // KeyTime
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(734, 461);
            Controls.Add(groupTimeline);
            Controls.Add(txtMainView);
            Controls.Add(resultsLabel);
            Controls.Add(menuMain);
            MainMenuStrip = menuMain;
            Name = "KeyTime";
            Text = "KeyTime";
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            groupTimeline.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtMainView;
        private MenuStrip menuMain;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem menuFileOpen;
        private ToolStripMenuItem menuFileSave;
        private ToolStripMenuItem menuFileSaveAs;
        private ToolStripMenuItem menuParse;
        private ToolStripMenuItem menuParseParse;
        private ToolStripMenuItem menuParseRunMacros;
        private Label resultsLabel;
        private ToolStripMenuItem menuParseStartup;
        private ToolStripMenuItem menuTimeline;
        private ToolStripMenuItem menuTimelineAddTrack;
        private GroupBox groupTimeline;
        private Timeline.TimelineControl timelineControl;
        private ToolStripMenuItem menuTimelineRemoveFirst;
        private ToolStripMenuItem menuTimelineRemoveLast;
        private ToolStripMenuItem menuTimelineRemoveIndex;
        private ToolStripMenuItem menuTimelineConvert;
        private ToolStripMenuItem menuView;
        private ToolStripMenuItem menuViewToggleCode;
        private ToolStripMenuItem menuViewToggleTimeline;
    }
}
