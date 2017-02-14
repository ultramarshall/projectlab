namespace client
{
    partial class FormAddModul
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if ( disposing && (components != null) )
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.simpleButton3);
            this.layoutControl1.Controls.Add(this.simpleButton2);
            this.layoutControl1.Controls.Add(this.simpleButton1);
            this.layoutControl1.Controls.Add(this.textEdit1);
            this.layoutControl1.Controls.Add(this.comboBoxEdit1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(343, 122);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // simpleButton3
            // 
            this.simpleButton3.Location = new System.Drawing.Point(241, 80);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(82, 22);
            this.simpleButton3.StyleController = this.layoutControl1;
            this.simpleButton3.TabIndex = 8;
            this.simpleButton3.Text = "cancel";
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(158, 80);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(73, 22);
            this.simpleButton2.StyleController = this.layoutControl1;
            this.simpleButton2.TabIndex = 7;
            this.simpleButton2.Text = "ok";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(270, 14);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(59, 22);
            this.simpleButton1.StyleController = this.layoutControl1;
            this.simpleButton1.TabIndex = 6;
            this.simpleButton1.Text = "add file";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(70, 15);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Properties.ReadOnly = true;
            this.textEdit1.Size = new System.Drawing.Size(191, 20);
            this.textEdit1.StyleController = this.layoutControl1;
            this.textEdit1.TabIndex = 5;
            // 
            // comboBoxEdit1
            // 
            this.comboBoxEdit1.Location = new System.Drawing.Point(70, 45);
            this.comboBoxEdit1.Name = "comboBoxEdit1";
            this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit1.Properties.Items.AddRange(new object[] {
            "BAB 01",
            "BAB 02",
            "BAB 03",
            "BAB 05",
            "BAB 06",
            "BAB 07",
            "BAB 08",
            "BAB 09",
            "BAB 10",
            "BAB 11",
            "BAB 12",
            "BAB 13",
            "BAB 14",
            "BAB 15",
            "BAB 16"});
            this.comboBoxEdit1.Size = new System.Drawing.Size(258, 20);
            this.comboBoxEdit1.StyleController = this.layoutControl1;
            this.comboBoxEdit1.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(343, 122);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.comboBoxEdit1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 30);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem1.Size = new System.Drawing.Size(323, 30);
            this.layoutControlItem1.Text = "File Modul";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(52, 13);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.textEdit1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(256, 30);
            this.layoutControlItem2.Text = "Source File";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(52, 13);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.simpleButton1;
            this.layoutControlItem3.Location = new System.Drawing.Point(256, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 4, 4, 4);
            this.layoutControlItem3.Size = new System.Drawing.Size(67, 30);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.simpleButton2;
            this.layoutControlItem4.Location = new System.Drawing.Point(138, 60);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 5, 10, 10);
            this.layoutControlItem4.Size = new System.Drawing.Size(88, 42);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.simpleButton3;
            this.layoutControlItem5.Location = new System.Drawing.Point(226, 60);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 10, 10, 10);
            this.layoutControlItem5.Size = new System.Drawing.Size(97, 42);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 60);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(138, 42);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // FormAddModul
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 122);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormAddModul";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormAddModul";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}