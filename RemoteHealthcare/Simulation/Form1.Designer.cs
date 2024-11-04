namespace Simulation;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label label4;
    private TextBox field1;
    private TextBox field2;
    private TextBox field3;
    private TextBox field4;
    private Button sendButton;

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
        this.label1 = new Label();
        this.label2 = new Label();
        this.label3 = new Label();
        this.label4 = new Label();
        this.field1 = new TextBox();
        this.field2 = new TextBox();
        this.field3 = new TextBox();
        this.field4 = new TextBox();
        this.sendButton = new Button();
        
        this.label1.Text = "Speed";
        this.label1.Location = new Point(10, 10);
        this.label1.TabIndex = 0;
        
        this.field1.Location = new Point(this.label1.Location.X, this.label1.Bounds.Bottom + 5);
        this.field1.TabIndex = 1;
            
        this.label2.Text = "Wattage";
        this.label2.Location = new Point(150, 10);
        this.label2.TabIndex = 2;
            
        this.field2.Location = new Point(this.label2.Location.X, this.label2.Bounds.Bottom + 5);
        this.field2.TabIndex = 3;
        
        this.label3.Text = "RPM";
        this.label3.Location = new Point(290, 10);
        this.label3.TabIndex = 4;
             
        this.field3.Location = new Point(this.label3.Location.X, this.label3.Bounds.Bottom + 5);
        this.field3.TabIndex = 5;
             
        this.label4.Text = "HeartRate";
        this.label4.Location = new Point(430, 10);
        this.label4.TabIndex = 6;
             
        this.field4.Location = new Point(this.label4.Location.X, this.label4.Bounds.Bottom + 5);
        this.field4.TabIndex = 7;
        
        this.sendButton.Location = new Point(570, 10);
        this.sendButton.TabIndex = 8;
        this.sendButton.Text = "Send";
        this.sendButton.Width = 100;
        this.sendButton.Height = 50;
        
        this.Controls.Add(this.label1);
        this.Controls.Add(this.field1);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.field2);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.field3);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.field4);
        this.Controls.Add(this.sendButton);
        
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "Simulation";
    }

    #endregion
}