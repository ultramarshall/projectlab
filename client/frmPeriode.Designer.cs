namespace client
{
    partial class frmPeriode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPeriode));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.dateEdit2 = new DevExpress.XtraEditors.DateEdit();
            this.dateEdit1 = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem2 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem3 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleLabelItem4 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.schedulerStorage1 = new DevExpress.XtraScheduler.SchedulerStorage(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.schedulerStorage1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.comboBoxEdit1);
            this.layoutControl1.Controls.Add(this.simpleButton1);
            this.layoutControl1.Controls.Add(this.dateEdit2);
            this.layoutControl1.Controls.Add(this.dateEdit1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(976, 71, 250, 347);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(443, 387);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(22, 193);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(399, 172);
            this.gridControl1.TabIndex = 9;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            // 
            // comboBoxEdit1
            // 
            this.comboBoxEdit1.Location = new System.Drawing.Point(170, 93);
            this.comboBoxEdit1.Name = "comboBoxEdit1";
            this.comboBoxEdit1.Properties.AllowFocused = false;
            this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit1.Properties.Items.AddRange(new object[] {
            "Ganjil",
            "Genap"});
            this.comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.comboBoxEdit1.Size = new System.Drawing.Size(136, 20);
            this.comboBoxEdit1.StyleController = this.layoutControl1;
            this.comboBoxEdit1.TabIndex = 8;
            // 
            // simpleButton1
            // 
            this.simpleButton1.AllowFocus = false;
            this.simpleButton1.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.Image")));
            this.simpleButton1.Location = new System.Drawing.Point(328, 92);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(93, 22);
            this.simpleButton1.StyleController = this.layoutControl1;
            this.simpleButton1.TabIndex = 7;
            this.simpleButton1.Text = "add";
            this.simpleButton1.Click += new System.EventHandler(this.AddPeriode);
            // 
            // dateEdit2
            // 
            this.dateEdit2.EditValue = null;
            this.dateEdit2.Location = new System.Drawing.Point(170, 153);
            this.dateEdit2.Name = "dateEdit2";
            this.dateEdit2.Properties.AllowFocused = false;
            this.dateEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit2.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit2.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dateEdit2.Size = new System.Drawing.Size(136, 20);
            this.dateEdit2.StyleController = this.layoutControl1;
            this.dateEdit2.TabIndex = 5;
            // 
            // dateEdit1
            // 
            this.dateEdit1.EditValue = null;
            this.dateEdit1.Location = new System.Drawing.Point(170, 123);
            this.dateEdit1.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.dateEdit1.Name = "dateEdit1";
            this.dateEdit1.Properties.AllowFocused = false;
            this.dateEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit1.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dateEdit1.Size = new System.Drawing.Size(136, 20);
            this.dateEdit1.StyleController = this.layoutControl1;
            this.dateEdit1.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.simpleLabelItem1,
            this.simpleLabelItem2,
            this.simpleLabelItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.simpleLabelItem4,
            this.layoutControlItem6});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 20, 20, 20);
            this.layoutControlGroup1.Size = new System.Drawing.Size(443, 387);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.dateEdit1;
            this.layoutControlItem1.Location = new System.Drawing.Point(145, 98);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 20, 5, 5);
            this.layoutControlItem1.Size = new System.Drawing.Size(161, 30);
            this.layoutControlItem1.Text = "Semester awal";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.dateEdit2;
            this.layoutControlItem2.Location = new System.Drawing.Point(145, 128);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 20, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(161, 30);
            this.layoutControlItem2.Text = "Semester akhir";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // simpleLabelItem1
            // 
            this.simpleLabelItem1.AllowHotTrack = false;
            this.simpleLabelItem1.Location = new System.Drawing.Point(0, 98);
            this.simpleLabelItem1.MinSize = new System.Drawing.Size(76, 17);
            this.simpleLabelItem1.Name = "simpleLabelItem1";
            this.simpleLabelItem1.Size = new System.Drawing.Size(145, 30);
            this.simpleLabelItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.simpleLabelItem1.Text = "Tanggal awal semester";
            this.simpleLabelItem1.TextSize = new System.Drawing.Size(156, 13);
            // 
            // simpleLabelItem2
            // 
            this.simpleLabelItem2.AllowHotTrack = false;
            this.simpleLabelItem2.Location = new System.Drawing.Point(0, 128);
            this.simpleLabelItem2.MinSize = new System.Drawing.Size(111, 17);
            this.simpleLabelItem2.Name = "simpleLabelItem2";
            this.simpleLabelItem2.Size = new System.Drawing.Size(145, 30);
            this.simpleLabelItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.simpleLabelItem2.Text = "Tanggal akhir Semester";
            this.simpleLabelItem2.TextSize = new System.Drawing.Size(156, 13);
            // 
            // simpleLabelItem3
            // 
            this.simpleLabelItem3.AllowHotTrack = false;
            this.simpleLabelItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleLabelItem3.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem3.Image = ((System.Drawing.Image)(resources.GetObject("simpleLabelItem3.Image")));
            this.simpleLabelItem3.Location = new System.Drawing.Point(0, 0);
            this.simpleLabelItem3.MinSize = new System.Drawing.Size(111, 17);
            this.simpleLabelItem3.Name = "simpleLabelItem3";
            this.simpleLabelItem3.Size = new System.Drawing.Size(403, 68);
            this.simpleLabelItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.simpleLabelItem3.Text = "Tahun Periode";
            this.simpleLabelItem3.TextSize = new System.Drawing.Size(156, 32);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.simpleButton1;
            this.layoutControlItem4.Location = new System.Drawing.Point(306, 68);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 4, 2);
            this.layoutControlItem4.Size = new System.Drawing.Size(97, 90);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.comboBoxEdit1;
            this.layoutControlItem5.Location = new System.Drawing.Point(145, 68);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 20, 5, 5);
            this.layoutControlItem5.Size = new System.Drawing.Size(161, 30);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // simpleLabelItem4
            // 
            this.simpleLabelItem4.AllowHotTrack = false;
            this.simpleLabelItem4.Location = new System.Drawing.Point(0, 68);
            this.simpleLabelItem4.MinSize = new System.Drawing.Size(116, 17);
            this.simpleLabelItem4.Name = "simpleLabelItem4";
            this.simpleLabelItem4.Size = new System.Drawing.Size(145, 30);
            this.simpleLabelItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.simpleLabelItem4.Text = "Semester";
            this.simpleLabelItem4.TextSize = new System.Drawing.Size(156, 13);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.gridControl1;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 158);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 15, 2);
            this.layoutControlItem6.Size = new System.Drawing.Size(403, 189);
            this.layoutControlItem6.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // frmPeriode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 387);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPeriode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form4";
            this.Load += new System.EventHandler(this.Form4_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.schedulerStorage1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.DateEdit dateEdit1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraScheduler.SchedulerStorage schedulerStorage1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.DateEdit dateEdit2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem1;
        private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem2;
        private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem4;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
    }
}