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
        Me.File_Menu = New System.Windows.Forms.ToolStripMenuItem()
        Me.Save_Menu = New System.Windows.Forms.ToolStripMenuItem()
        Me.Open_Menu = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuView = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuItemShowHideRulers = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuItemTools = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuItemEditorOn = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuItemPointer = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuItemWall = New System.Windows.Forms.ToolStripMenuItem()
        Me.Floor_Menu = New System.Windows.Forms.ToolStripMenuItem()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Splitter1 = New System.Windows.Forms.Splitter()
        Me.Timer4 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox1.Location = New System.Drawing.Point(0, 31)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(666, 487)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.File_Menu, Me.MenuView, Me.MenuItemTools})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(900, 31)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "Tools"
        '
        'File_Menu
        '
        Me.File_Menu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Save_Menu, Me.Open_Menu})
        Me.File_Menu.Name = "File_Menu"
        Me.File_Menu.Size = New System.Drawing.Size(51, 27)
        Me.File_Menu.Text = "File"
        '
        'Save_Menu
        '
        Me.Save_Menu.Name = "Save_Menu"
        Me.Save_Menu.Size = New System.Drawing.Size(144, 30)
        Me.Save_Menu.Text = "Save"
        '
        'Open_Menu
        '
        Me.Open_Menu.Name = "Open_Menu"
        Me.Open_Menu.Size = New System.Drawing.Size(144, 30)
        Me.Open_Menu.Text = "Open"
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
        'MenuItemTools
        '
        Me.MenuItemTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuItemEditorOn, Me.MenuItemPointer, Me.MenuItemWall, Me.Floor_Menu})
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
        'MenuItemPointer
        '
        Me.MenuItemPointer.Name = "MenuItemPointer"
        Me.MenuItemPointer.Size = New System.Drawing.Size(205, 30)
        Me.MenuItemPointer.Text = "Pointer"
        '
        'MenuItemWall
        '
        Me.MenuItemWall.Name = "MenuItemWall"
        Me.MenuItemWall.Size = New System.Drawing.Size(205, 30)
        Me.MenuItemWall.Text = "Wall"
        '
        'Floor_Menu
        '
        Me.Floor_Menu.Name = "Floor_Menu"
        Me.Floor_Menu.Size = New System.Drawing.Size(205, 30)
        Me.Floor_Menu.Text = "Floor"
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLight
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(666, 31)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersWidth = 56
        Me.DataGridView1.RowTemplate.Height = 32
        Me.DataGridView1.Size = New System.Drawing.Size(234, 487)
        Me.DataGridView1.TabIndex = 2
        '
        'Splitter1
        '
        Me.Splitter1.Location = New System.Drawing.Point(666, 31)
        Me.Splitter1.Name = "Splitter1"
        Me.Splitter1.Size = New System.Drawing.Size(10, 487)
        Me.Splitter1.TabIndex = 3
        Me.Splitter1.TabStop = False
        '
        'Timer4
        '
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 23.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 518)
        Me.Controls.Add(Me.Splitter1)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents MenuItemPointer As ToolStripMenuItem
    Friend WithEvents MenuItemWall As ToolStripMenuItem
    Friend WithEvents File_Menu As ToolStripMenuItem
    Friend WithEvents Save_Menu As ToolStripMenuItem
    Friend WithEvents Open_Menu As ToolStripMenuItem
    Friend WithEvents Floor_Menu As ToolStripMenuItem
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Splitter1 As Splitter
    Friend WithEvents Timer4 As Timer
End Class
