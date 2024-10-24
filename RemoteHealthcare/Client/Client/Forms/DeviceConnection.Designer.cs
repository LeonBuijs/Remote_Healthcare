namespace Client.Forms
{
    partial class DeviceConnection
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label bikeLabel;
        private System.Windows.Forms.Label bikeNumberLabel;
        private System.Windows.Forms.TextBox bikeNumberTextBox;
        private System.Windows.Forms.Label BikeConnectedStatusLabel;
        private System.Windows.Forms.Label heartRateMonitorLabel;
        private System.Windows.Forms.Label HeartRateConnectedStatusLabel;
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
            this.bikeLabel = new System.Windows.Forms.Label();
            this.bikeNumberLabel = new System.Windows.Forms.Label();
            this.bikeNumberTextBox = new System.Windows.Forms.TextBox();
            this.BikeConnectedStatusLabel = new System.Windows.Forms.Label();
            this.heartRateMonitorLabel = new System.Windows.Forms.Label();
            this.HeartRateConnectedStatusLabel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // titleLabel
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.Location = new System.Drawing.Point(100, 10);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(200, 32);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Connect to Devices";

            // bikeLabel
            this.bikeLabel.AutoSize = true;
            this.bikeLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.bikeLabel.Location = new System.Drawing.Point(100, 60);
            this.bikeLabel.Name = "bikeLabel";
            this.bikeLabel.Size = new System.Drawing.Size(34, 17);
            this.bikeLabel.TabIndex = 1;
            this.bikeLabel.Text = "Bike";

            // bikeNumberLabel
            this.bikeNumberLabel.AutoSize = true;
            this.bikeNumberLabel.Location = new System.Drawing.Point(100, 80);
            this.bikeNumberLabel.Name = "bikeNumberLabel";
            this.bikeNumberLabel.Size = new System.Drawing.Size(90, 17);
            this.bikeNumberLabel.TabIndex = 2;
            this.bikeNumberLabel.Text = "Bike Number:";

            // bikeNumberTextBox
            this.bikeNumberTextBox.Location = new System.Drawing.Point(170, 77);
            this.bikeNumberTextBox.Name = "bikeNumberTextBox";
            this.bikeNumberTextBox.Size = new System.Drawing.Size(40, 22);
            this.bikeNumberTextBox.TabIndex = 3;

            // BikeConnectedStatusLabel
            this.BikeConnectedStatusLabel.AutoSize = true;
            this.BikeConnectedStatusLabel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Italic);
            this.BikeConnectedStatusLabel.Location = new System.Drawing.Point(100, 100);
            this.BikeConnectedStatusLabel.Name = "BikeConnectedStatusLabel";
            this.BikeConnectedStatusLabel.Size = new System.Drawing.Size(130, 17);
            this.BikeConnectedStatusLabel.TabIndex = 4;
            this.BikeConnectedStatusLabel.Text = "No device connected";

            // heartRateMonitorLabel
            this.heartRateMonitorLabel.AutoSize = true;
            this.heartRateMonitorLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.heartRateMonitorLabel.Location = new System.Drawing.Point(100, 140);
            this.heartRateMonitorLabel.Name = "heartRateMonitorLabel";
            this.heartRateMonitorLabel.Size = new System.Drawing.Size(117, 17);
            this.heartRateMonitorLabel.TabIndex = 5;
            this.heartRateMonitorLabel.Text = "Heart Rate Monitor";

            // HeartRateConnectedStatusLabel
            this.HeartRateConnectedStatusLabel.AutoSize = true;
            this.HeartRateConnectedStatusLabel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Italic);
            this.HeartRateConnectedStatusLabel.Location = new System.Drawing.Point(100, 160);
            this.HeartRateConnectedStatusLabel.Name = "HeartRateConnectedStatusLabel";
            this.HeartRateConnectedStatusLabel.Size = new System.Drawing.Size(130, 17);
            this.HeartRateConnectedStatusLabel.TabIndex = 6;
            this.HeartRateConnectedStatusLabel.Text = "No device connected";
            
            // connectButton
            this.connectButton.AutoSize = true;
            this.connectButton.Location = new System.Drawing.Point(100, 190);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(200, 30);
            this.connectButton.TabIndex = 7;
            this.connectButton.Text = "Connect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);

            // DeviceConnection Form settings
            this.ClientSize = new System.Drawing.Size(400, 280);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.HeartRateConnectedStatusLabel);
            this.Controls.Add(this.heartRateMonitorLabel);
            this.Controls.Add(this.BikeConnectedStatusLabel);
            this.Controls.Add(this.bikeNumberTextBox);
            this.Controls.Add(this.bikeNumberLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.bikeLabel);
            this.Controls.Add(this.titleLabel);
            this.Name = "DeviceConnection";
            this.Text = "Device Connection";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
