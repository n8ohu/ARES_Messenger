namespace ARES_Messenger
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuItemAutoID = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetupBeaconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMode = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuCall = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuAnswerCQ = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEnd = new System.Windows.Forms.ToolStripButton();
            this.prgThroughput = new System.Windows.Forms.ToolStripProgressBar();
            this.lblThroughput = new System.Windows.Forms.ToolStripLabel();
            this.lblQueue = new System.Windows.Forms.ToolStripLabel();
            this.btnScrollLock = new System.Windows.Forms.ToolStripButton();
            this.btnQueueData = new System.Windows.Forms.ToolStripButton();
            this.mnuUserFiles = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPasteAndSend = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCreate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ipdaemon1 = new nsoftware.IPWorks.Ipdaemon(this.components);
            this.Ftp1 = new nsoftware.IPWorks.Ftp(this.components);
            this.Http1 = new nsoftware.IPWorks.Http(this.components);
            this.objTNCTCPIPPort = new nsoftware.IPWorks.Ipport(this.components);
            this.objTCPData = new nsoftware.IPWorks.Ipport(this.components);
            this.tmrStartup = new System.Windows.Forms.Timer(this.components);
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tmrPoll = new System.Windows.Forms.Timer(this.components);
            this.splHorizontal = new System.Windows.Forms.SplitContainer();
            this.rtbSession = new System.Windows.Forms.RichTextBox();
            this.rtbSend = new System.Windows.Forms.RichTextBox();
            this.ContextMenurtbSend = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextMenurtbSession = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FECToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aRQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fECRBSTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fECFDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStrip1.SuspendLayout();
            this.splHorizontal.Panel1.SuspendLayout();
            this.splHorizontal.Panel2.SuspendLayout();
            this.splHorizontal.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuMode,
            this.mnuCall,
            this.btnEnd,
            this.prgThroughput,
            this.lblThroughput,
            this.lblQueue,
            this.btnScrollLock,
            this.btnQueueData,
            this.mnuUserFiles});
            this.ToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Size = new System.Drawing.Size(784, 25);
            this.ToolStrip1.TabIndex = 2;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemAutoID,
            this.aboutToolStripMenuItem,
            this.HelpContentsToolStripMenuItem,
            this.HelpIndexToolStripMenuItem,
            this.SetupToolStripMenuItem,
            this.SetupBeaconToolStripMenuItem,
            this.ExitToolStripMenuItem1});
            this.mnuFile.Image = ((System.Drawing.Image)(resources.GetObject("mnuFile.Image")));
            this.mnuFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(38, 22);
            this.mnuFile.Text = "File";
            // 
            // mnuItemAutoID
            // 
            this.mnuItemAutoID.Name = "mnuItemAutoID";
            this.mnuItemAutoID.Size = new System.Drawing.Size(150, 22);
            this.mnuItemAutoID.Text = "Auto ID is Off";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // HelpContentsToolStripMenuItem
            // 
            this.HelpContentsToolStripMenuItem.Name = "HelpContentsToolStripMenuItem";
            this.HelpContentsToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.HelpContentsToolStripMenuItem.Text = "Help Contents";
            this.HelpContentsToolStripMenuItem.Click += new System.EventHandler(this.HelpContentsToolStripMenuItem_Click);
            // 
            // HelpIndexToolStripMenuItem
            // 
            this.HelpIndexToolStripMenuItem.Name = "HelpIndexToolStripMenuItem";
            this.HelpIndexToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.HelpIndexToolStripMenuItem.Text = "Help Index";
            // 
            // SetupToolStripMenuItem
            // 
            this.SetupToolStripMenuItem.Name = "SetupToolStripMenuItem";
            this.SetupToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.SetupToolStripMenuItem.Text = "Setup";
            this.SetupToolStripMenuItem.Click += new System.EventHandler(this.SetupToolStripMenuItem_Click);
            // 
            // SetupBeaconToolStripMenuItem
            // 
            this.SetupBeaconToolStripMenuItem.Name = "SetupBeaconToolStripMenuItem";
            this.SetupBeaconToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.SetupBeaconToolStripMenuItem.Text = "Setup Beacon";
            // 
            // ExitToolStripMenuItem1
            // 
            this.ExitToolStripMenuItem1.Name = "ExitToolStripMenuItem1";
            this.ExitToolStripMenuItem1.Size = new System.Drawing.Size(150, 22);
            this.ExitToolStripMenuItem1.Text = "Exit";
            this.ExitToolStripMenuItem1.Click += new System.EventHandler(this.ExitToolStripMenuItem1_Click);
            // 
            // mnuMode
            // 
            this.mnuMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aRQToolStripMenuItem,
            this.FECToolStripMenuItem,
            this.fECRBSTToolStripMenuItem,
            this.fECFDToolStripMenuItem});
            this.mnuMode.Enabled = false;
            this.mnuMode.Image = ((System.Drawing.Image)(resources.GetObject("mnuMode.Image")));
            this.mnuMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuMode.Name = "mnuMode";
            this.mnuMode.Size = new System.Drawing.Size(77, 22);
            this.mnuMode.Text = "Mode: FEC";
            this.mnuMode.ToolTipText = "Select Mode ARQ or FEC";
            // 
            // mnuCall
            // 
            this.mnuCall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuCall.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAnswerCQ});
            this.mnuCall.Image = ((System.Drawing.Image)(resources.GetObject("mnuCall.Image")));
            this.mnuCall.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuCall.Name = "mnuCall";
            this.mnuCall.Size = new System.Drawing.Size(44, 22);
            this.mnuCall.Text = "ARQ";
            // 
            // mnuAnswerCQ
            // 
            this.mnuAnswerCQ.Name = "mnuAnswerCQ";
            this.mnuAnswerCQ.Size = new System.Drawing.Size(155, 22);
            this.mnuAnswerCQ.Text = "mnuAnswerCQ";
            // 
            // btnEnd
            // 
            this.btnEnd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEnd.Image = ((System.Drawing.Image)(resources.GetObject("btnEnd.Image")));
            this.btnEnd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(31, 22);
            this.btnEnd.Text = "End";
            // 
            // prgThroughput
            // 
            this.prgThroughput.Name = "prgThroughput";
            this.prgThroughput.Size = new System.Drawing.Size(23, 22);
            // 
            // lblThroughput
            // 
            this.lblThroughput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblThroughput.Image = ((System.Drawing.Image)(resources.GetObject("lblThroughput.Image")));
            this.lblThroughput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lblThroughput.Name = "lblThroughput";
            this.lblThroughput.Size = new System.Drawing.Size(94, 22);
            this.lblThroughput.Text = "toolStripButton1";
            // 
            // lblQueue
            // 
            this.lblQueue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblQueue.Image = ((System.Drawing.Image)(resources.GetObject("lblQueue.Image")));
            this.lblQueue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lblQueue.Name = "lblQueue";
            this.lblQueue.Size = new System.Drawing.Size(94, 22);
            this.lblQueue.Text = "toolStripButton1";
            // 
            // btnScrollLock
            // 
            this.btnScrollLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnScrollLock.Image = ((System.Drawing.Image)(resources.GetObject("btnScrollLock.Image")));
            this.btnScrollLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScrollLock.Name = "btnScrollLock";
            this.btnScrollLock.Size = new System.Drawing.Size(68, 22);
            this.btnScrollLock.Text = "Scroll Lock";
            // 
            // btnQueueData
            // 
            this.btnQueueData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnQueueData.Image = ((System.Drawing.Image)(resources.GetObject("btnQueueData.Image")));
            this.btnQueueData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnQueueData.Name = "btnQueueData";
            this.btnQueueData.Size = new System.Drawing.Size(73, 22);
            this.btnQueueData.Text = "Queue Data";
            // 
            // mnuUserFiles
            // 
            this.mnuUserFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuUserFiles.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPaste,
            this.mnuPasteAndSend,
            this.mnuCreate,
            this.mnuEdit,
            this.mnuDelete});
            this.mnuUserFiles.Image = ((System.Drawing.Image)(resources.GetObject("mnuUserFiles.Image")));
            this.mnuUserFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuUserFiles.Name = "mnuUserFiles";
            this.mnuUserFiles.Size = new System.Drawing.Size(69, 22);
            this.mnuUserFiles.Text = "User Files";
            // 
            // mnuPaste
            // 
            this.mnuPaste.Name = "mnuPaste";
            this.mnuPaste.Size = new System.Drawing.Size(154, 22);
            this.mnuPaste.Text = "Paste";
            // 
            // mnuPasteAndSend
            // 
            this.mnuPasteAndSend.Name = "mnuPasteAndSend";
            this.mnuPasteAndSend.Size = new System.Drawing.Size(154, 22);
            this.mnuPasteAndSend.Text = "Paste and Send";
            // 
            // mnuCreate
            // 
            this.mnuCreate.Name = "mnuCreate";
            this.mnuCreate.Size = new System.Drawing.Size(154, 22);
            this.mnuCreate.Text = "Create";
            // 
            // mnuEdit
            // 
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(154, 22);
            this.mnuEdit.Text = "Edit";
            // 
            // mnuDelete
            // 
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(154, 22);
            this.mnuDelete.Text = "Delete";
            // 
            // ipdaemon1
            // 
            this.ipdaemon1.About = "IP*Works! V9 [Build 5414]";
            // 
            // Ftp1
            // 
            this.Ftp1.About = "IP*Works! V9 [Build 5414]";
            // 
            // Http1
            // 
            this.Http1.About = "IP*Works! V9 [Build 5414]";
            // 
            // objTNCTCPIPPort
            // 
            this.objTNCTCPIPPort.About = "IP*Works! V9 [Build 5414]";
            // 
            // objTCPData
            // 
            this.objTCPData.About = "IP*Works! V9 [Build 5414]";
            // 
            // splHorizontal
            // 
            this.splHorizontal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splHorizontal.Location = new System.Drawing.Point(0, 25);
            this.splHorizontal.Margin = new System.Windows.Forms.Padding(4);
            this.splHorizontal.Name = "splHorizontal";
            this.splHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splHorizontal.Panel1
            // 
            this.splHorizontal.Panel1.Controls.Add(this.rtbSession);
            // 
            // splHorizontal.Panel2
            // 
            this.splHorizontal.Panel2.Controls.Add(this.rtbSend);
            this.splHorizontal.Size = new System.Drawing.Size(784, 336);
            this.splHorizontal.SplitterDistance = 262;
            this.splHorizontal.TabIndex = 2;
            // 
            // rtbSession
            // 
            this.rtbSession.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(225)))));
            this.rtbSession.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbSession.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbSession.Location = new System.Drawing.Point(0, 0);
            this.rtbSession.Margin = new System.Windows.Forms.Padding(4);
            this.rtbSession.Name = "rtbSession";
            this.rtbSession.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbSession.Size = new System.Drawing.Size(780, 258);
            this.rtbSession.TabIndex = 1;
            this.rtbSession.Text = "";
            // 
            // rtbSend
            // 
            this.rtbSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.rtbSend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbSend.Location = new System.Drawing.Point(0, 0);
            this.rtbSend.Margin = new System.Windows.Forms.Padding(4);
            this.rtbSend.Name = "rtbSend";
            this.rtbSend.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbSend.Size = new System.Drawing.Size(780, 66);
            this.rtbSend.TabIndex = 5;
            this.rtbSend.Text = "";
            // 
            // ContextMenurtbSend
            // 
            this.ContextMenurtbSend.Name = "ContextMenurtbSend";
            this.ContextMenurtbSend.Size = new System.Drawing.Size(61, 4);
            // 
            // ContextMenurtbSession
            // 
            this.ContextMenurtbSession.Name = "ContextMenurtbSession";
            this.ContextMenurtbSession.Size = new System.Drawing.Size(61, 4);
            // 
            // FECToolStripMenuItem
            // 
            this.FECToolStripMenuItem.Name = "FECToolStripMenuItem";
            this.FECToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.FECToolStripMenuItem.Text = "FEC";
            // 
            // aRQToolStripMenuItem
            // 
            this.aRQToolStripMenuItem.Name = "aRQToolStripMenuItem";
            this.aRQToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aRQToolStripMenuItem.Text = "ARQ";
            // 
            // fECRBSTToolStripMenuItem
            // 
            this.fECRBSTToolStripMenuItem.Name = "fECRBSTToolStripMenuItem";
            this.fECRBSTToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fECRBSTToolStripMenuItem.Text = "FEC RBST";
            // 
            // fECFDToolStripMenuItem
            // 
            this.fECFDToolStripMenuItem.Name = "fECFDToolStripMenuItem";
            this.fECFDToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fECFDToolStripMenuItem.Text = "FEC FD";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.splHorizontal);
            this.Controls.Add(this.ToolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "ARES Messenger Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.VisibleChanged += new System.EventHandler(this.Main_VisibleChanged);
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.splHorizontal.Panel1.ResumeLayout(false);
            this.splHorizontal.Panel2.ResumeLayout(false);
            this.splHorizontal.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip ToolStrip1;
        private nsoftware.IPWorks.Ipdaemon ipdaemon1;
        private nsoftware.IPWorks.Ftp Ftp1;
        private nsoftware.IPWorks.Http Http1;
        private nsoftware.IPWorks.Ipport objTNCTCPIPPort;
        private nsoftware.IPWorks.Ipport objTCPData;
        private System.Windows.Forms.Timer tmrStartup;
        private System.Windows.Forms.ToolTip ToolTip1;
        private System.Windows.Forms.Timer tmrPoll;
        private System.Windows.Forms.SplitContainer splHorizontal;
        private System.Windows.Forms.RichTextBox rtbSession;
        private System.Windows.Forms.RichTextBox rtbSend;
        private System.Windows.Forms.ToolStripDropDownButton mnuFile;
        private System.Windows.Forms.ToolStripDropDownButton mnuMode;
        private System.Windows.Forms.ContextMenuStrip ContextMenurtbSend;
        private System.Windows.Forms.ContextMenuStrip ContextMenurtbSession;
        private System.Windows.Forms.ToolStripMenuItem mnuItemAutoID;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpContentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetupBeaconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripDropDownButton mnuCall;
        private System.Windows.Forms.ToolStripButton btnEnd;
        private System.Windows.Forms.ToolStripProgressBar prgThroughput;
        private System.Windows.Forms.ToolStripLabel lblThroughput;
        private System.Windows.Forms.ToolStripLabel lblQueue;
        private System.Windows.Forms.ToolStripMenuItem mnuAnswerCQ;
        private System.Windows.Forms.ToolStripButton btnScrollLock;
        private System.Windows.Forms.ToolStripButton btnQueueData;
        private System.Windows.Forms.ToolStripDropDownButton mnuUserFiles;
        private System.Windows.Forms.ToolStripMenuItem mnuPaste;
        private System.Windows.Forms.ToolStripMenuItem mnuPasteAndSend;
        private System.Windows.Forms.ToolStripMenuItem mnuCreate;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuDelete;
        private System.Windows.Forms.ToolStripMenuItem aRQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FECToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fECRBSTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fECFDToolStripMenuItem;
    }
}

