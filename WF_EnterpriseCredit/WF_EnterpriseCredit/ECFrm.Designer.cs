namespace WF_EnterpriseCredit
{
    partial class ECFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpExcuteTask = new System.Windows.Forms.TabPage();
            this.txtSavepath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTask = new System.Windows.Forms.TextBox();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tpExcuteProcesser = new System.Windows.Forms.TabPage();
            this.lstState = new System.Windows.Forms.ListBox();
            this.btnsave = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExcute = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tpExcuteTask.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tpExcuteProcesser.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpExcuteTask);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(698, 421);
            this.tabControl1.TabIndex = 1;
            // 
            // tpExcuteTask
            // 
            this.tpExcuteTask.Controls.Add(this.txtSavepath);
            this.tpExcuteTask.Controls.Add(this.label1);
            this.tpExcuteTask.Controls.Add(this.txtTask);
            this.tpExcuteTask.Controls.Add(this.tabControl3);
            this.tpExcuteTask.Controls.Add(this.btnsave);
            this.tpExcuteTask.Controls.Add(this.btnStop);
            this.tpExcuteTask.Controls.Add(this.btnExcute);
            this.tpExcuteTask.Location = new System.Drawing.Point(4, 22);
            this.tpExcuteTask.Name = "tpExcuteTask";
            this.tpExcuteTask.Size = new System.Drawing.Size(690, 395);
            this.tpExcuteTask.TabIndex = 2;
            this.tpExcuteTask.Text = "任务执行器";
            this.tpExcuteTask.UseVisualStyleBackColor = true;
            // 
            // txtSavepath
            // 
            this.txtSavepath.Location = new System.Drawing.Point(8, 110);
            this.txtSavepath.Name = "txtSavepath";
            this.txtSavepath.Size = new System.Drawing.Size(593, 21);
            this.txtSavepath.TabIndex = 73;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(10, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(239, 12);
            this.label1.TabIndex = 72;
            this.label1.Text = "注：输入的自定义关键字请用\',\'或\'，\'分隔";
            // 
            // txtTask
            // 
            this.txtTask.Location = new System.Drawing.Point(8, 14);
            this.txtTask.Multiline = true;
            this.txtTask.Name = "txtTask";
            this.txtTask.Size = new System.Drawing.Size(674, 78);
            this.txtTask.TabIndex = 71;
            // 
            // tabControl3
            // 
            this.tabControl3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl3.Controls.Add(this.tpExcuteProcesser);
            this.tabControl3.Location = new System.Drawing.Point(3, 169);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(679, 223);
            this.tabControl3.TabIndex = 70;
            // 
            // tpExcuteProcesser
            // 
            this.tpExcuteProcesser.Controls.Add(this.lstState);
            this.tpExcuteProcesser.Location = new System.Drawing.Point(4, 22);
            this.tpExcuteProcesser.Name = "tpExcuteProcesser";
            this.tpExcuteProcesser.Padding = new System.Windows.Forms.Padding(3);
            this.tpExcuteProcesser.Size = new System.Drawing.Size(671, 197);
            this.tpExcuteProcesser.TabIndex = 0;
            this.tpExcuteProcesser.UseVisualStyleBackColor = true;
            // 
            // lstState
            // 
            this.lstState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstState.FormattingEnabled = true;
            this.lstState.IntegralHeight = false;
            this.lstState.ItemHeight = 12;
            this.lstState.Location = new System.Drawing.Point(3, 3);
            this.lstState.Name = "lstState";
            this.lstState.Size = new System.Drawing.Size(665, 191);
            this.lstState.TabIndex = 3;
            // 
            // btnsave
            // 
            this.btnsave.Location = new System.Drawing.Point(603, 110);
            this.btnsave.Name = "btnsave";
            this.btnsave.Size = new System.Drawing.Size(75, 23);
            this.btnsave.TabIndex = 69;
            this.btnsave.Text = "保存位置";
            this.btnsave.UseVisualStyleBackColor = true;
            this.btnsave.Click += new System.EventHandler(this.btnsave_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(603, 147);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 69;
            this.btnStop.Text = "终止执行";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnExcute
            // 
            this.btnExcute.Location = new System.Drawing.Point(522, 147);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(75, 23);
            this.btnExcute.TabIndex = 68;
            this.btnExcute.Text = "开始执行";
            this.btnExcute.UseVisualStyleBackColor = true;
            this.btnExcute.Click += new System.EventHandler(this.btnExcute_Click);
            // 
            // ECFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 421);
            this.Controls.Add(this.tabControl1);
            this.Name = "ECFrm";
            this.Text = "EC";
            this.Load += new System.EventHandler(this.ECFrm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpExcuteTask.ResumeLayout(false);
            this.tpExcuteTask.PerformLayout();
            this.tabControl3.ResumeLayout(false);
            this.tpExcuteProcesser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpExcuteTask;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tpExcuteProcesser;
        private System.Windows.Forms.ListBox lstState;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExcute;
        private System.Windows.Forms.TextBox txtTask;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSavepath;
        private System.Windows.Forms.Button btnsave;
    }
}

