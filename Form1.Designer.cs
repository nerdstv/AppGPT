namespace WinFormsApp3
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            prompt_query = new TextBox();
            Btn_search = new Button();
            results = new TextBox();
            Close = new Button();
            img_results = new PictureBox();
            top_panel = new Panel();
            minimise = new Button();
            bot_panel = new Panel();
            ReadButton = new CustomControls.Controls.CustomButton();
            togglelMode = new CustomControls.Controls.ToggleButton();
            Clear = new CustomControls.Controls.CustomButton();
            result_panel = new Panel();
            loadingBox = new PictureBox();
            txtBtn = new CustomControls.Controls.CustomButton();
            imgBtn = new CustomControls.Controls.CustomButton();
            Hard_search = new CustomControls.Controls.CustomButton();
            ((System.ComponentModel.ISupportInitialize)img_results).BeginInit();
            top_panel.SuspendLayout();
            bot_panel.SuspendLayout();
            result_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)loadingBox).BeginInit();
            SuspendLayout();
            // 
            // prompt_query
            // 
            prompt_query.AllowDrop = true;
            prompt_query.AutoCompleteMode = AutoCompleteMode.Suggest;
            prompt_query.AutoCompleteSource = AutoCompleteSource.CustomSource;
            prompt_query.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            prompt_query.Location = new Point(360, 63);
            prompt_query.Name = "prompt_query";
            prompt_query.PlaceholderText = "Enter a prompt here..";
            prompt_query.Size = new Size(475, 30);
            prompt_query.TabIndex = 0;
            prompt_query.TextChanged += prompt_query_TextChanged;
            // 
            // Btn_search
            // 
            Btn_search.BackgroundImage = (Image)resources.GetObject("Btn_search.BackgroundImage");
            Btn_search.BackgroundImageLayout = ImageLayout.Stretch;
            Btn_search.Location = new Point(841, 63);
            Btn_search.Name = "Btn_search";
            Btn_search.Size = new Size(30, 30);
            Btn_search.TabIndex = 1;
            Btn_search.UseVisualStyleBackColor = true;
            Btn_search.Click += Btn_search_Click;
            // 
            // results
            // 
            results.BackColor = Color.Gray;
            results.Font = new Font("Segoe Print", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            results.Location = new Point(18, 13);
            results.Multiline = true;
            results.Name = "results";
            results.PlaceholderText = "Results";
            results.ReadOnly = true;
            results.Size = new Size(773, 357);
            results.TabIndex = 2;
            results.TextChanged += results_TextChanged;
            // 
            // Close
            // 
            Close.BackgroundImage = (Image)resources.GetObject("Close.BackgroundImage");
            Close.BackgroundImageLayout = ImageLayout.Stretch;
            Close.Location = new Point(1213, 6);
            Close.Name = "Close";
            Close.Size = new Size(28, 28);
            Close.TabIndex = 8;
            Close.UseVisualStyleBackColor = true;
            Close.Click += Close_click;
            // 
            // img_results
            // 
            img_results.Location = new Point(214, 35);
            img_results.Name = "img_results";
            img_results.Size = new Size(401, 318);
            img_results.TabIndex = 9;
            img_results.TabStop = false;
            // 
            // top_panel
            // 
            top_panel.BackColor = SystemColors.Control;
            top_panel.Controls.Add(minimise);
            top_panel.Controls.Add(Close);
            top_panel.Location = new Point(1, -4);
            top_panel.Name = "top_panel";
            top_panel.Size = new Size(1244, 37);
            top_panel.TabIndex = 10;
            // 
            // minimise
            // 
            minimise.BackgroundImage = (Image)resources.GetObject("minimise.BackgroundImage");
            minimise.BackgroundImageLayout = ImageLayout.Stretch;
            minimise.Location = new Point(1184, 6);
            minimise.Name = "minimise";
            minimise.Size = new Size(28, 28);
            minimise.TabIndex = 9;
            minimise.UseVisualStyleBackColor = true;
            minimise.Click += minimise_Click;
            // 
            // bot_panel
            // 
            bot_panel.Controls.Add(ReadButton);
            bot_panel.Controls.Add(togglelMode);
            bot_panel.Controls.Add(Clear);
            bot_panel.Location = new Point(1, 530);
            bot_panel.Name = "bot_panel";
            bot_panel.Size = new Size(1244, 35);
            bot_panel.TabIndex = 11;
            // 
            // ReadButton
            // 
            ReadButton.BackColor = SystemColors.Control;
            ReadButton.BackgroundColor = SystemColors.Control;
            ReadButton.BackgroundImage = (Image)resources.GetObject("ReadButton.BackgroundImage");
            ReadButton.BackgroundImageLayout = ImageLayout.Stretch;
            ReadButton.BorderColor = Color.Transparent;
            ReadButton.BorderRadius = 20;
            ReadButton.BorderSize = 0;
            ReadButton.Cursor = Cursors.Hand;
            ReadButton.FlatAppearance.BorderSize = 0;
            ReadButton.FlatStyle = FlatStyle.Flat;
            ReadButton.ForeColor = Color.Transparent;
            ReadButton.HoverColor = Color.Transparent;
            ReadButton.Location = new Point(1077, -1);
            ReadButton.Name = "ReadButton";
            ReadButton.Size = new Size(53, 35);
            ReadButton.TabIndex = 11;
            ReadButton.TextColor = Color.Transparent;
            ReadButton.UseVisualStyleBackColor = true;
            ReadButton.Click += ReadButton_Click_1;
            // 
            // togglelMode
            // 
            togglelMode.AutoSize = true;
            togglelMode.Location = new Point(11, 6);
            togglelMode.MinimumSize = new Size(45, 22);
            togglelMode.Name = "togglelMode";
            togglelMode.OffBackColor = Color.Gray;
            togglelMode.OffToggleColor = Color.Gainsboro;
            togglelMode.OnBackColor = Color.MediumSlateBlue;
            togglelMode.OnToggleColor = Color.WhiteSmoke;
            togglelMode.Size = new Size(45, 22);
            togglelMode.TabIndex = 13;
            togglelMode.UseVisualStyleBackColor = true;
            togglelMode.CheckedChanged += togglelMode_CheckedChanged;
            // 
            // Clear
            // 
            Clear.BackColor = Color.FromArgb(64, 64, 64, 150);
            Clear.BackgroundColor = Color.FromArgb(64, 64, 64, 150);
            Clear.BorderColor = Color.Transparent;
            Clear.BorderRadius = 20;
            Clear.BorderSize = 0;
            Clear.Cursor = Cursors.Hand;
            Clear.FlatAppearance.BorderSize = 0;
            Clear.FlatStyle = FlatStyle.Flat;
            Clear.ForeColor = Color.White;
            Clear.HoverColor = Color.MediumSlateBlue;
            Clear.Location = new Point(1156, 0);
            Clear.Name = "Clear";
            Clear.Size = new Size(85, 32);
            Clear.TabIndex = 12;
            Clear.Text = "Clear";
            Clear.TextColor = Color.White;
            Clear.UseVisualStyleBackColor = false;
            Clear.Click += Clear_Click;
            // 
            // result_panel
            // 
            result_panel.Controls.Add(loadingBox);
            result_panel.Controls.Add(img_results);
            result_panel.Controls.Add(results);
            result_panel.Location = new Point(213, 140);
            result_panel.Name = "result_panel";
            result_panel.Size = new Size(811, 384);
            result_panel.TabIndex = 12;
            // 
            // loadingBox
            // 
            loadingBox.BackgroundImageLayout = ImageLayout.Stretch;
            loadingBox.Image = (Image)resources.GetObject("loadingBox.Image");
            loadingBox.Location = new Point(288, 87);
            loadingBox.Name = "loadingBox";
            loadingBox.Size = new Size(272, 230);
            loadingBox.SizeMode = PictureBoxSizeMode.StretchImage;
            loadingBox.TabIndex = 10;
            loadingBox.TabStop = false;
            // 
            // txtBtn
            // 
            txtBtn.BackColor = Color.FromArgb(64, 64, 64, 125);
            txtBtn.BackgroundColor = Color.FromArgb(64, 64, 64, 125);
            txtBtn.BorderColor = Color.PaleVioletRed;
            txtBtn.BorderRadius = 20;
            txtBtn.BorderSize = 0;
            txtBtn.Cursor = Cursors.Hand;
            txtBtn.FlatAppearance.BorderSize = 0;
            txtBtn.FlatStyle = FlatStyle.Flat;
            txtBtn.ForeColor = Color.White;
            txtBtn.HoverColor = Color.MediumSlateBlue;
            txtBtn.Location = new Point(540, 99);
            txtBtn.Name = "txtBtn";
            txtBtn.Size = new Size(66, 27);
            txtBtn.TabIndex = 13;
            txtBtn.Text = "Text";
            txtBtn.TextColor = Color.White;
            txtBtn.UseVisualStyleBackColor = false;
            txtBtn.Click += txtBtn_Click;
            // 
            // imgBtn
            // 
            imgBtn.BackColor = Color.FromArgb(64, 64, 64, 125);
            imgBtn.BackgroundColor = Color.FromArgb(64, 64, 64, 125);
            imgBtn.BorderColor = Color.PaleVioletRed;
            imgBtn.BorderRadius = 20;
            imgBtn.BorderSize = 0;
            imgBtn.Cursor = Cursors.Hand;
            imgBtn.FlatAppearance.BorderSize = 0;
            imgBtn.FlatStyle = FlatStyle.Flat;
            imgBtn.ForeColor = Color.White;
            imgBtn.HoverColor = Color.MediumSlateBlue;
            imgBtn.Location = new Point(612, 99);
            imgBtn.Name = "imgBtn";
            imgBtn.Size = new Size(73, 27);
            imgBtn.TabIndex = 14;
            imgBtn.Text = "Image";
            imgBtn.TextColor = Color.White;
            imgBtn.UseVisualStyleBackColor = false;
            imgBtn.Click += imgBtn_Click;
            // 
            // Hard_search
            // 
            Hard_search.BackColor = Color.FromArgb(64, 64, 64, 125);
            Hard_search.BackgroundColor = Color.FromArgb(64, 64, 64, 125);
            Hard_search.BorderColor = Color.PaleVioletRed;
            Hard_search.BorderRadius = 20;
            Hard_search.BorderSize = 0;
            Hard_search.FlatAppearance.BorderSize = 0;
            Hard_search.FlatStyle = FlatStyle.Flat;
            Hard_search.ForeColor = Color.White;
            Hard_search.HoverColor = Color.MediumSlateBlue;
            Hard_search.Location = new Point(877, 62);
            Hard_search.Name = "Hard_search";
            Hard_search.Size = new Size(74, 31);
            Hard_search.TabIndex = 15;
            Hard_search.Text = "Hard";
            Hard_search.TextColor = Color.White;
            Hard_search.UseVisualStyleBackColor = false;
            Hard_search.Click += Hard_search_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1246, 566);
            Controls.Add(Hard_search);
            Controls.Add(imgBtn);
            Controls.Add(txtBtn);
            Controls.Add(result_panel);
            Controls.Add(bot_panel);
            Controls.Add(top_panel);
            Controls.Add(Btn_search);
            Controls.Add(prompt_query);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)img_results).EndInit();
            top_panel.ResumeLayout(false);
            bot_panel.ResumeLayout(false);
            bot_panel.PerformLayout();
            result_panel.ResumeLayout(false);
            result_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)loadingBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox prompt_query;
        private Button Btn_search;
        private TextBox results;
        private Button Close;
        private PictureBox img_results;
        private Panel top_panel;
        private Button minimise;
        private Panel bot_panel;
        private CustomControls.Controls.ToggleButton togglelMode;
        private CustomControls.Controls.CustomButton Clear;
        private Panel result_panel;
        private CustomControls.Controls.CustomButton txtBtn;
        private CustomControls.Controls.CustomButton imgBtn;
        private ProgressBar progressBar;
        private PictureBox loadingBox;
        private CustomControls.Controls.CustomButton ReadButton;
        private CustomControls.Controls.CustomButton Hard_search;
    }
}