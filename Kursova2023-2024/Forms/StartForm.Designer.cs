namespace Kursova2023_2024
{
    partial class StartForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartForm));
            WelcomeLabel = new Label();
            LoginTextBox = new TextBox();
            PasswordTextBox = new TextBox();
            ForgetLabel = new Label();
            LogInButton = new Button();
            WelcomePicture = new PictureBox();
            RegistrationLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)WelcomePicture).BeginInit();
            SuspendLayout();
            // 
            // WelcomeLabel
            // 
            WelcomeLabel.AutoSize = true;
            WelcomeLabel.BackColor = Color.White;
            WelcomeLabel.Font = new Font("Franklin Gothic Medium", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            WelcomeLabel.Location = new Point(622, 97);
            WelcomeLabel.Name = "WelcomeLabel";
            WelcomeLabel.Size = new Size(185, 43);
            WelcomeLabel.TabIndex = 0;
            WelcomeLabel.Text = "WELCOME";
            // 
            // LoginTextBox
            // 
            LoginTextBox.Font = new Font("Segoe UI", 14.25F);
            LoginTextBox.Location = new Point(575, 179);
            LoginTextBox.Name = "LoginTextBox";
            LoginTextBox.Size = new Size(277, 33);
            LoginTextBox.TabIndex = 1;
            // 
            // PasswordTextBox
            // 
            PasswordTextBox.Font = new Font("Segoe UI", 14.25F);
            PasswordTextBox.Location = new Point(575, 250);
            PasswordTextBox.Name = "PasswordTextBox";
            PasswordTextBox.Size = new Size(277, 33);
            PasswordTextBox.TabIndex = 2;
            // 
            // ForgetLabel
            // 
            ForgetLabel.AutoSize = true;
            ForgetLabel.BackColor = Color.White;
            ForgetLabel.FlatStyle = FlatStyle.Flat;
            ForgetLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ForgetLabel.Location = new Point(726, 297);
            ForgetLabel.Name = "ForgetLabel";
            ForgetLabel.Size = new Size(126, 21);
            ForgetLabel.TabIndex = 3;
            ForgetLabel.Text = "Forget password";
            ForgetLabel.Click += ForgetLabel_Click;
            // 
            // LogInButton
            // 
            LogInButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(224, 224, 224);
            LogInButton.FlatAppearance.MouseOverBackColor = Color.Silver;
            LogInButton.FlatStyle = FlatStyle.Flat;
            LogInButton.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            LogInButton.Location = new Point(648, 345);
            LogInButton.Name = "LogInButton";
            LogInButton.Size = new Size(143, 36);
            LogInButton.TabIndex = 4;
            LogInButton.Text = "Log in";
            LogInButton.UseVisualStyleBackColor = true;
            LogInButton.Click += LogInButton_Click;
            // 
            // WelcomePicture
            // 
            WelcomePicture.Image = (Image)resources.GetObject("WelcomePicture.Image");
            WelcomePicture.Location = new Point(-3, -8);
            WelcomePicture.Name = "WelcomePicture";
            WelcomePicture.Size = new Size(572, 636);
            WelcomePicture.SizeMode = PictureBoxSizeMode.StretchImage;
            WelcomePicture.TabIndex = 5;
            WelcomePicture.TabStop = false;
            // 
            // RegistrationLabel
            // 
            RegistrationLabel.AutoSize = true;
            RegistrationLabel.BackColor = Color.White;
            RegistrationLabel.FlatStyle = FlatStyle.Flat;
            RegistrationLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            RegistrationLabel.Location = new Point(670, 394);
            RegistrationLabel.Name = "RegistrationLabel";
            RegistrationLabel.Size = new Size(94, 21);
            RegistrationLabel.TabIndex = 6;
            RegistrationLabel.Text = "Registration";
            RegistrationLabel.Click += RegistrationLabel_Click;
            // 
            // StartForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(944, 501);
            Controls.Add(RegistrationLabel);
            Controls.Add(WelcomePicture);
            Controls.Add(LogInButton);
            Controls.Add(ForgetLabel);
            Controls.Add(PasswordTextBox);
            Controls.Add(LoginTextBox);
            Controls.Add(WelcomeLabel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "StartForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)WelcomePicture).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label WelcomeLabel;
        private TextBox LoginTextBox;
        private TextBox PasswordTextBox;
        private Label ForgetLabel;
        private Button LogInButton;
        private PictureBox WelcomePicture;
        private Label RegistrationLabel;
    }
}
