namespace ClientGUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label _firstNameLabel;
        private System.Windows.Forms.TextBox _firstNameTextBox;
        private System.Windows.Forms.Label _lastNameLabel;
        private System.Windows.Forms.TextBox _lastNameTextBox;
        private System.Windows.Forms.Label _birthDateLabel;
        private System.Windows.Forms.TextBox _birthDateTextBox;
        private System.Windows.Forms.Button _connectButton;

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
            this._firstNameLabel = new System.Windows.Forms.Label();
            this._firstNameTextBox = new System.Windows.Forms.TextBox();
            this._lastNameLabel = new System.Windows.Forms.Label();
            this._lastNameTextBox = new System.Windows.Forms.TextBox();
            this._birthDateLabel = new System.Windows.Forms.Label();
            this._birthDateTextBox = new System.Windows.Forms.TextBox();
            this._connectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            
             
            this._firstNameLabel.AutoSize = true;
            this._firstNameLabel.Location = new System.Drawing.Point(50, 30);
            this._firstNameLabel.Name = "_firstNameLabel";
            this._firstNameLabel.Size = new System.Drawing.Size(76, 17);
            this._firstNameLabel.TabIndex = 0;
            this._firstNameLabel.Text = "First Name:";
            
            
            this._firstNameTextBox.Location = new System.Drawing.Point(150, 30);
            this._firstNameTextBox.Name = "_firstNameTextBox";
            this._firstNameTextBox.Size = new System.Drawing.Size(200, 22);
            this._firstNameTextBox.TabIndex = 1;
            
            
            this._lastNameLabel.AutoSize = true;
            this._lastNameLabel.Location = new System.Drawing.Point(50, 70);
            this._lastNameLabel.Name = "_lastNameLabel";
            this._lastNameLabel.Size = new System.Drawing.Size(76, 17);
            this._lastNameLabel.TabIndex = 2;
            this._lastNameLabel.Text = "Last Name:";
            
            
            this._lastNameTextBox.Location = new System.Drawing.Point(150, 70);
            this._lastNameTextBox.Name = "_lastNameTextBox";
            this._lastNameTextBox.Size = new System.Drawing.Size(200, 22);
            this._lastNameTextBox.TabIndex = 3;
            
            
            this._birthDateLabel.AutoSize = true;
            this._birthDateLabel.Location = new System.Drawing.Point(50, 110);
            this._birthDateLabel.Name = "_birthDateLabel";
            this._birthDateLabel.Size = new System.Drawing.Size(76, 17);
            this._birthDateLabel.TabIndex = 4;
            this._birthDateLabel.Text = "Birth Date:";
            
            
            this._birthDateTextBox.Location = new System.Drawing.Point(150, 110);
            this._birthDateTextBox.Name = "_birthDateTextBox";
            this._birthDateTextBox.Size = new System.Drawing.Size(200, 22);
            this._birthDateTextBox.TabIndex = 5;
             
            
            this._connectButton.Location = new System.Drawing.Point(150, 150);
            this._connectButton.Name = "_connectButton";
            this._connectButton.Size = new System.Drawing.Size(100, 30);
            this._connectButton.TabIndex = 6;
            this._connectButton.Text = "Connect";
            this._connectButton.UseVisualStyleBackColor = true;
            this._connectButton.Click += new System.EventHandler(this.connectButton_Click);
             
            
            this.ClientSize = new System.Drawing.Size(400, 220);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this._connectButton);
            this.Controls.Add(this._birthDateTextBox);
            this.Controls.Add(this._birthDateLabel);
            this.Controls.Add(this._lastNameTextBox);
            this.Controls.Add(this._lastNameLabel);
            this.Controls.Add(this._firstNameTextBox);
            this.Controls.Add(this._firstNameLabel);
            this.Name = "Form1";
            this.Text = "Remote Healthcare Client";
            // this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
