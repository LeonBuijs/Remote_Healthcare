using System.ComponentModel;

namespace Client
{
    partial class Form
    {
       private System.ComponentModel.IContainer components = null;
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
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.birthDateLabel = new System.Windows.Forms.Label();
            this.birthDateTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            
            // firstNameLabel 
            this.firstNameLabel.AutoSize = true;
            this.firstNameLabel.Location = new System.Drawing.Point(50, 30);
            this.firstNameLabel.Name = "_firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(76, 17);
            this.firstNameLabel.TabIndex = 0;
            this.firstNameLabel.Text = "First Name:";
            
            // firstNameTextBox
            this.firstNameTextBox.Location = new System.Drawing.Point(150, 30);
            this.firstNameTextBox.Name = "_firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(200, 22);
            this.firstNameTextBox.TabIndex = 1;
            
            // lastNameLabel
            this.lastNameLabel.AutoSize = true;
            this.lastNameLabel.Location = new System.Drawing.Point(50, 70);
            this.lastNameLabel.Name = "_lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(76, 17);
            this.lastNameLabel.TabIndex = 2;
            this.lastNameLabel.Text = "Last Name:";
            
            // lastNameTextBox
            this.lastNameTextBox.Location = new System.Drawing.Point(150, 70);
            this.lastNameTextBox.Name = "_lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(200, 22);
            this.lastNameTextBox.TabIndex = 3;
            
            // birthDateLabel
            this.birthDateLabel.AutoSize = true;
            this.birthDateLabel.Location = new System.Drawing.Point(50, 110);
            this.birthDateLabel.Name = "_birthDateLabel";
            this.birthDateLabel.Size = new System.Drawing.Size(76, 17);
            this.birthDateLabel.TabIndex = 4;
            this.birthDateLabel.Text = "Birth Date:";
            
            // birthDateTextBox
            this.birthDateTextBox.Location = new System.Drawing.Point(150, 110);
            this.birthDateTextBox.Name = "_birthDateTextBox";
            this.birthDateTextBox.Size = new System.Drawing.Size(200, 22);
            this.birthDateTextBox.TabIndex = 5;
             
            // connectButton
            this.connectButton.Location = new System.Drawing.Point(150, 150);
            this.connectButton.Name = "_connectButton";
            this.connectButton.Size = new System.Drawing.Size(100, 30);
            this.connectButton.TabIndex = 6;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            
            this.ClientSize = new System.Drawing.Size(400, 220);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.birthDateTextBox);
            this.Controls.Add(this.birthDateLabel);
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.lastNameLabel);
            this.Controls.Add(this.firstNameTextBox);
            this.Controls.Add(this.firstNameLabel);
            this.Name = "Form1";
            this.Text = "Remote Healthcare Client";
            // this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

    }
}