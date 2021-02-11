<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer3 = New System.Windows.Forms.Timer(Me.components)
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.MenuItemTools = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuItemEditorOn = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuView = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuItemShowHideRulers = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Timer1
        '
        '
        'Timer2
        '
        '
        'Timer3
        '
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(0, 31)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(900, 487)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuItemTools, Me.MenuView})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(900, 31)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "Tools"
        '
        'MenuItemTools
        '
        Me.MenuItemTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuItemEditorOn})
        Me.MenuItemTools.Name = "MenuItemTools"
        Me.MenuItemTools.Size = New System.Drawing.Size(64, 27)
        Me.MenuItemTools.Text = "Tools"
        '
        'MenuItemEditorOn
        '
        Me.MenuItemEditorOn.Name = "MenuItemEditorOn"
        Me.MenuItemEditorOn.Size = New System.Drawing.Size(205, 30)
        Me.MenuItemEditorOn.Text = "Editor On/Off"
        '
        'MenuView
        '
        Me.MenuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuItemShowHideRulers})
        Me.MenuView.Name = "MenuView"
        Me.MenuView.Size = New System.Drawing.Size(62, 27)
        Me.MenuView.Text = "View"
        '
        'MenuItemShowHideRulers
        '
        Me.MenuItemShowHideRulers.Name = "MenuItemShowHideRulers"
        Me.MenuItemShowHideRulers.Size = New System.Drawing.Size(194, 30)
        Me.MenuItemShowHideRulers.Text = "Show Rulers"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 23.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 518)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Timer1 As Timer
    Friend WithEvents Timer2 As Timer
    Friend WithEvents Timer3 As Timer
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents MenuItemTools As ToolStripMenuItem
    Friend WithEvents MenuItemEditorOn As ToolStripMenuItem
    Friend WithEvents MenuView As ToolStripMenuItem
    Friend WithEvents MenuItemShowHideRulers As ToolStripMenuItem
End Class
