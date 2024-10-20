using System.ComponentModel;

namespace Client
{
    partial class ServerLogin
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label serverIPLabel;
        private System.Windows.Forms.TextBox serverIPTextBox;
        private System.Windows.Forms.Label firstNameLabel;
        private System.Windows.Forms.TextBox firstNameTextBox;
        private System.Windows.Forms.Label lastNameLabel;
        private System.Windows.Forms.TextBox lastNameTextBox;
        private System.Windows.Forms.Label birthDateLabel;
        private System.Windows.Forms.Label dayLabel;
        private System.Windows.Forms.Label monthLabel;
        private System.Windows.Forms.Label yearLabel;
        private System.Windows.Forms.TextBox dayTextBox;
        private System.Windows.Forms.TextBox monthTextBox;
        private System.Windows.Forms.TextBox yearTextBox;
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
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.birthDateLabel = new System.Windows.Forms.Label();
            this.dayLabel = new System.Windows.Forms.Label();
            this.monthLabel = new System.Windows.Forms.Label();
            this.yearLabel = new System.Windows.Forms.Label();
            this.dayTextBox = new System.Windows.Forms.TextBox();
            this.monthTextBox = new System.Windows.Forms.TextBox();
            this.yearTextBox = new System.Windows.Forms.TextBox();
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
            
            // firstNameLabel 
            this.firstNameLabel.AutoSize = true;
            this.firstNameLabel.Location = new System.Drawing.Point(50, 100);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(76, 17);
            this.firstNameLabel.TabIndex = 5;
            this.firstNameLabel.Text = "First Name:";
            
            // firstNameTextBox
            this.firstNameTextBox.Location = new System.Drawing.Point(150, 100);
            this.firstNameTextBox.Name = "firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(200, 22);
            this.firstNameTextBox.TabIndex = 6;
            
            // lastNameLabel
            this.lastNameLabel.AutoSize = true;
            this.lastNameLabel.Location = new System.Drawing.Point(50, 140);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(76, 17);
            this.lastNameLabel.TabIndex = 7;
            this.lastNameLabel.Text = "Last Name:";
            
            // lastNameTextBox
            this.lastNameTextBox.Location = new System.Drawing.Point(150, 140);
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(200, 22);
            this.lastNameTextBox.TabIndex = 8;
            
            // birthDateLabel
            this.birthDateLabel.AutoSize = true;
            this.birthDateLabel.Location = new System.Drawing.Point(50, 180);
            this.birthDateLabel.Name = "birthDateLabel";
            this.birthDateLabel.Size = new System.Drawing.Size(76, 17);
            this.birthDateLabel.TabIndex = 9;
            this.birthDateLabel.Text = "Birth Date:";

            // dayLabel
            this.dayLabel.AutoSize = true;
            this.dayLabel.Location = new System.Drawing.Point(150, 180);
            this.dayLabel.Name = "dayLabel";
            this.dayLabel.Size = new System.Drawing.Size(34, 17);
            this.dayLabel.TabIndex = 10;
            this.dayLabel.Text = "Day";

            // dayTextBox
            this.dayTextBox.Location = new System.Drawing.Point(150, 200);
            this.dayTextBox.Name = "dayTextBox";
            this.dayTextBox.Size = new System.Drawing.Size(50, 22);
            this.dayTextBox.TabIndex = 11;

            // monthLabel
            this.monthLabel.AutoSize = true;
            this.monthLabel.Location = new System.Drawing.Point(210, 180);
            this.monthLabel.Name = "monthLabel";
            this.monthLabel.Size = new System.Drawing.Size(47, 17);
            this.monthLabel.TabIndex = 12;
            this.monthLabel.Text = "Month";

            // monthTextBox
            this.monthTextBox.Location = new System.Drawing.Point(210, 200);
            this.monthTextBox.Name = "monthTextBox";
            this.monthTextBox.Size = new System.Drawing.Size(50, 22);
            this.monthTextBox.TabIndex = 13;

            // yearLabel
            this.yearLabel.AutoSize = true;
            this.yearLabel.Location = new System.Drawing.Point(270, 180);
            this.yearLabel.Name = "yearLabel";
            this.yearLabel.Size = new System.Drawing.Size(38, 17);
            this.yearLabel.TabIndex = 14;
            this.yearLabel.Text = "Year";

            // yearTextBox
            this.yearTextBox.Location = new System.Drawing.Point(270, 200);
            this.yearTextBox.Name = "yearTextBox";
            this.yearTextBox.Size = new System.Drawing.Size(80, 22);
            this.yearTextBox.TabIndex = 15;
             
            // connectButton
            this.connectButton.Location = new System.Drawing.Point(150, 240);
            this.connectButton.Name = "_connectButton";
            this.connectButton.Size = new System.Drawing.Size(100, 30);
            this.connectButton.TabIndex = 16;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.firstNameTextBox);
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.serverIPTextBox);
            this.Controls.Add(this.birthDateLabel);
            this.Controls.Add(this.firstNameLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.lastNameLabel);
            this.Controls.Add(this.serverIPLabel);
            this.Controls.Add(this.monthTextBox);
            this.Controls.Add(this.yearTextBox);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.dayTextBox);
            this.Controls.Add(this.monthLabel);
            this.Controls.Add(this.yearLabel);
            this.Controls.Add(this.dayLabel);
            this.Name = "Connect To Server";
            this.Text = "Remote Healthcare Client";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
