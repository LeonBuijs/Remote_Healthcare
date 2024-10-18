using System.ComponentModel;

namespace Client
{
    partial class Form
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label serverIPLabel;
        private System.Windows.Forms.TextBox serverIPTextBox;
        private System.Windows.Forms.Label bikeNumberLabel;
        private System.Windows.Forms.TextBox bikeNumberTextBox;
        private System.Windows.Forms.Label firstNameLabel;
        private System.Windows.Forms.TextBox firstNameTextBox;
        private System.Windows.Forms.Label lastNameLabel;
        private System.Windows.Forms.TextBox lastNameTextBox;
        private System.Windows.Forms.Label birthDateLabel;
        private System.Windows.Forms.TextBox birthDateTextBox;
        private System.Windows.Forms.Button connectButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.serverIPLabel = new System.Windows.Forms.Label();
            this.serverIPTextBox = new System.Windows.Forms.TextBox();
            this.bikeNumberLabel = new System.Windows.Forms.Label();
            this.bikeNumberTextBox = new System.Windows.Forms.TextBox();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.birthDateLabel = new System.Windows.Forms.Label();
            this.birthDateTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            
            // titleLabel
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.Location = new System.Drawing.Point(150, 10);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(74, 32);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Login";

            // serverIPLabel 
            this.serverIPLabel.AutoSize = true;
            this.serverIPLabel.Location = new System.Drawing.Point(50, 60);
            this.serverIPLabel.Name = "serverIPLabel";
            this.serverIPLabel.Size = new System.Drawing.Size(72, 17);
            this.serverIPLabel.TabIndex = 1;
            this.serverIPLabel.Text = "Server IP:";
            
            // serverIPTextBox
            this.serverIPTextBox.Location = new System.Drawing.Point(150, 60);
            this.serverIPTextBox.Name = "serverIPTextBox";
            this.serverIPTextBox.Size = new System.Drawing.Size(200, 22);
            this.serverIPTextBox.TabIndex = 2;
            
            // bikeNumberLabel
            this.bikeNumberLabel.AutoSize = true;
            this.bikeNumberLabel.Location = new System.Drawing.Point(50, 100);
            this.bikeNumberLabel.Name = "bikeNumberLabel";
            this.bikeNumberLabel.Size = new System.Drawing.Size(90, 17);
            this.bikeNumberLabel.TabIndex = 3;
            this.bikeNumberLabel.Text = "Bike Number:";
            
            // bikeNumberTextBox
            this.bikeNumberTextBox.Location = new System.Drawing.Point(150, 100);
            this.bikeNumberTextBox.Name = "bikeNumberTextBox";
            this.bikeNumberTextBox.Size = new System.Drawing.Size(200, 22);
            this.bikeNumberTextBox.TabIndex = 4;
            
            // firstNameLabel 
            this.firstNameLabel.AutoSize = true;
            this.firstNameLabel.Location = new System.Drawing.Point(50, 140);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(76, 17);
            this.firstNameLabel.TabIndex = 5;
            this.firstNameLabel.Text = "First Name:";
            
            // firstNameTextBox
            this.firstNameTextBox.Location = new System.Drawing.Point(150, 140);
            this.firstNameTextBox.Name = "firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(200, 22);
            this.firstNameTextBox.TabIndex = 6;
            
            // lastNameLabel
            this.lastNameLabel.AutoSize = true;
            this.lastNameLabel.Location = new System.Drawing.Point(50, 180);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(76, 17);
            this.lastNameLabel.TabIndex = 7;
            this.lastNameLabel.Text = "Last Name:";
            
            // lastNameTextBox
            this.lastNameTextBox.Location = new System.Drawing.Point(150, 180);
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(200, 22);
            this.lastNameTextBox.TabIndex = 8;
            
            // birthDateLabel
            this.birthDateLabel.AutoSize = true;
            this.birthDateLabel.Location = new System.Drawing.Point(50, 220);
            this.birthDateLabel.Name = "birthDateLabel";
            this.birthDateLabel.Size = new System.Drawing.Size(76, 17);
            this.birthDateLabel.TabIndex = 9;
            this.birthDateLabel.Text = "Birth Date:";
            
            // birthDateTextBox
            this.birthDateTextBox.Location = new System.Drawing.Point(150, 220);
            this.birthDateTextBox.Name = "birthDateTextBox";
            this.birthDateTextBox.Size = new System.Drawing.Size(200, 22);
            this.birthDateTextBox.TabIndex = 10;
             
            // connectButton
            this.connectButton.Location = new System.Drawing.Point(150, 260);
            this.connectButton.Name = "_connectButton";
            this.connectButton.Size = new System.Drawing.Size(100, 30);
            this.connectButton.TabIndex = 11;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            
            this.ClientSize = new System.Drawing.Size(400, 320);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.birthDateTextBox);
            this.Controls.Add(this.birthDateLabel);
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.lastNameLabel);
            this.Controls.Add(this.firstNameTextBox);
            this.Controls.Add(this.firstNameLabel);
            this.Controls.Add(this.bikeNumberTextBox);
            this.Controls.Add(this.bikeNumberLabel);
            this.Controls.Add(this.serverIPTextBox);
            this.Controls.Add(this.serverIPLabel);
            this.Controls.Add(this.titleLabel);
            this.Name = "Form1";
            this.Text = "Remote Healthcare Client";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

    }
}
