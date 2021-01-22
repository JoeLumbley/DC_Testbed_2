Option Strict On

Imports System.Math
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.IO

Public Structure HeroInfo
    Public Rec As Rectangle
    Public Color As Color
    Public OutlineColor As Color
    Public MoveLeft As Boolean
    Public MoveRight As Boolean
    Public MoveUp As Boolean
    Public MoveDown As Boolean
    Public Speed As Integer
    Public Life As Integer
    Public MaxLife As Integer
    Public Attack As Integer
    Public Initiative As Boolean
    Public CastMagic As Boolean
    Public Magic As Integer
    Public MaxMagic As Integer
    Public MapColor As Color
    Public MapOutlineColor As Color
    Public Hit As Boolean
    Public HitTimer As Integer
End Structure
Public Structure MonsterInfo
    Public Rec As Rectangle
    Public Color As Color
    Public OutlineColor As Color
    Public Speed As Integer
    Public Life As Integer
    Public MaxLife As Integer
    Public Attack As Integer
    Public Hit As Boolean
End Structure
Public Structure WallInfo
    Public Rec As Rectangle
    Public Color As Color
    Public Revealed As Boolean 'If true the wall will be seen on the map.
    Public MapColor As Color
    Public MapOutlineColor As Color
End Structure
Public Structure PotionInfo
    Public Rec As Rectangle
    Public Color As Color
    Public OutlineColor As Color
    Public Active As Boolean 'If true the potion will be seen and can be used in the level.
    Public Life As Integer
End Structure
Public Enum LootEnum As Integer
    None = 0
    LifePotion = 1 'Restores the heros life points. Keeps the hero alive.
    Elixir = 2 'Adds to max life points of hero. Make the hero tougher.
    MagicPotion = 3 'Restores the heros magic points. Keep the hero casting.
    SpellBook = 4 'Adds to max magic points of hero. Makes casting more powerful.
    Armor = 5 'Adds to armor points of hero. Makes the hero tougher.
    Weapon = 6 'Adds to attack points of hero. Makes the hero more deadly.
    Chest = 7
    Gold = 8
End Enum

Public Enum DirectionEnum As Integer
    None = 0
    Right = 1
    Left = 2
    Up = 3
    Down = 4
    RightUP = 5
    RightDown = 6
    LeftUp = 7
    LeftDown = 8
End Enum
Public Enum MCI_NOTIFY As Integer
    SUCCESSFUL = &H1
    SUPERSEDED = &H2
    ABORTED = &H4
    FAILURE = &H8
End Enum
Public Class Form1

    Private Level_Background_Color As Color = Color.FromArgb(255, 82, 82, 104)

    Private OurHero As HeroInfo

    Private Wall As WallInfo

    Private Potion As PotionInfo

    Private Viewport As New Rectangle(0, 0, 640, 480)

    Private Viewport_Size As New Drawing.Size(640, 480)
    Private _BufferFlag As Boolean = True


    Private Monster As New Rectangle(500, 500, 75, 75)
    Private Monster_Brush As New SolidBrush(Color.FromArgb(255, 39, 205, 89))
    Private Monster_LifeMAX As Integer = 50
    Private Monster_Life As Integer = Monster_LifeMAX
    Private Monster_Attack As Integer = 2
    Private Monster_Hit As Boolean = False
    Private Monster_Speed As Integer = 2
    Private Monster_AttackTimer As Integer = 0
    Private Monster_Font As New Font("Arial", 10)

    Private MoveLeft As Boolean = False
    Private MoveRight As Boolean = False
    Private MoveUp As Boolean = False
    Private MoveDown As Boolean = False

    Private Projectile As New Rectangle(0, 0, 75, 75)
    Private ProjectileInflight As Boolean = False
    Private Projectile_Brush As New SolidBrush(Color.Yellow)
    Private Projectile_Max_Distance As Integer = 300
    Private Projectile_Attack As Integer = 12
    Private Projectile_Speed As Integer = 50
    Private Projectile_Direction As DirectionEnum

    Private ShootLeft As Boolean = False
    Private ShootRight As Boolean = False
    Private ShootUp As Boolean = False
    Private ShootDown As Boolean = False

    Private CtrlDown As Boolean = False

    Private Life_Brush As New SolidBrush(Color.FromArgb(255, 255, 19, 19))
    Private Life_Blink_Brush As New SolidBrush(Color.Red)
    Private Life_Frame_Brush As New SolidBrush(Color.FromArgb(255, 153, 0, 0))
    Private Life_Bar_Frame As Rectangle
    Private Life_Bar_Font As New Font("Arial", 15)

    Private Instruction_Text As String = "Use arrow keys to move hero. Bump monsters to attack. Use Ctrl and use arrow keys to cast spells. Press P to pause game."
    Private Instruction_Font As New Font("Arial", 14)

    Private Fifty_Percent_Black_Brush As New SolidBrush(Color.FromArgb(128, Color.Black))
    Private Pause_Screen_Brush As New SolidBrush(Color.FromArgb(128, Color.Black))
    Private PauseFont As New Font("Arial", 50)

    Dim RandomNumber As New Random()

    Dim Center_String As New StringFormat()

    Dim Blink_Counter As Integer = 0


    Private Map_Border_Pen As New Pen(Color.Black, 3)


    'Set up Game Sound
    Private WithEvents GS As New GameSounds



    ' Create font and brush.
    Private drawFont As New Font("Arial", 16)
    Private drawBrush As New SolidBrush(Color.White)
    Dim drawString As String = "Sample Text"

    Private Blur_BMP1 As New Bitmap(Viewport_Size.Width, Viewport_Size.Height, Imaging.PixelFormat.Format32bppPArgb)
    Private goBlur1 As Graphics = Graphics.FromImage(Blur_BMP1)

    Private Blur_BMP2 As New Bitmap(Viewport_Size.Width, Viewport_Size.Height, Imaging.PixelFormat.Format32bppPArgb)
    Private goBlur2 As Graphics = Graphics.FromImage(Blur_BMP2)

    Dim cm As New Drawing.Imaging.ColorMatrix
    Dim atr As New Drawing.Imaging.ImageAttributes


    Private Intersection_REC As New Rectangle(0, 0, 1, 1)

    Private Const Quarter_Number As Double = 0.25


    Dim x As Single = 0
    Dim y As Single = 0

    Dim drawFormat As New StringFormat

    Dim music As String = Application.StartupPath & "level_music.wav" ' *.wav file location
    Dim media As New Media.SoundPlayer(music)

    Dim sound As String = Application.StartupPath & "machine_gun.wav" ' *.wav file location
    Dim media2 As New Media.SoundPlayer(sound)

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        OurHero.Rec = New Rectangle(0, 0, 75, 75)
        OurHero.Color = Color.FromArgb(255, 11, 182, 255)
        OurHero.Life = 100
        OurHero.MaxLife = 100
        OurHero.Attack = 3
        OurHero.Initiative = False
        OurHero.Speed = 4
        OurHero.Hit = False
        OurHero.HitTimer = 0
        OurHero.Life = 100



        Wall.Rec.X = 900
        Wall.Rec.Y = 300
        Wall.Rec.Width = 100
        Wall.Rec.Height = 100
        Wall.Color = Color.FromArgb(255, 164, 164, 164)
        Wall.MapColor = Color.FromArgb(128, 164, 164, 164)

        Potion.Rec = New Rectangle(0, 0, 50, 50)
        Potion.Active = False
        Potion.Color = Color.FromArgb(255, 255, 19, 19)
        Potion.OutlineColor = Color.FromArgb(255, 255, 255, 255)

        cm(3, 3) = 0.6   'draw with 60% alpha
        atr.SetColorMatrix(cm)


        Center_String.Alignment = StringAlignment.Center
        Center_String.LineAlignment = StringAlignment.Center

        MinimumSize = New Size(600, 480)

        drawFormat.Alignment = StringAlignment.Center

        SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)


        WindowState = FormWindowState.Maximized



        'Load sound files
        GS.AddSound("Music", Application.StartupPath & "level_music.mp3") 'adds and open a Background music file
        GS.AddSound("Magic", Application.StartupPath & "magic_sound.mp3") 'adds and open a Laser Gun sound
        GS.AddSound("Monster", Application.StartupPath & "Monster Alien Roar Aggressive.mp3") 'adds and open a Laser Gun sound

        GS.AddSound("Undead_Move", Application.StartupPath & "undead_move.mp3") 'adds and open a Laser Gun sound

        GS.AddSound("Undead_Attack", Application.StartupPath & "undead_attack.mp3") 'adds and open a Laser Gun sound

        GS.AddSound("Potion_Pickup", Application.StartupPath & "potion_pickup.mp3") 'adds and open a Laser Gun sound

        'Set set volume 
        GS.SetVolume("Music", 200)
        GS.SetVolume("Magic", 600)
        GS.SetVolume("Monster", 900)
        GS.SetVolume("Undead_Move", 900)
        GS.SetVolume("Undead_Attack", 900)
        GS.SetVolume("Potion_Pickup", 1000)


        'Set frame rate for the display timer.
        Timer1.Interval = 33

        'Set frame rate for the game timer.
        Timer2.Interval = 20

        'Start display timer.
        Timer1.Start()

        'Update the display.
        PictureBox1.Invalidate()

        GS.Play("Music") 'play the Music

        'Start the game timer.
        Timer2.Start()


    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Display timer

        'Swap Buffers
        If _BufferFlag = True Then
            _BufferFlag = False
        Else
            _BufferFlag = True
        End If

        'Update the display.
        PictureBox1.Invalidate()

    End Sub
    Private Sub PictureBox1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint

        If _BufferFlag = True Then

            Using Buffer1_BMP As New Bitmap(Viewport_Size.Width, Viewport_Size.Height, Imaging.PixelFormat.Format32bppPArgb)
                Using goBuf1 As Graphics = Graphics.FromImage(Buffer1_BMP)
                    With goBuf1
                        'To fix draw string error: "Parameters not valid." I had to set the compositing mode to source over.
                        .CompositingMode = Drawing2D.CompositingMode.SourceOver
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .SmoothingMode = Drawing2D.SmoothingMode.None
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
                    End With

                    goBuf1.Clear(Level_Background_Color)

                    'Draw Wall
                    goBuf1.FillRectangle(New SolidBrush(Wall.Color), Wall.Rec)

                    If Potion.Active = True Then

                        Draw_Potion(goBuf1, Potion.Rec)

                    End If



                    Draw_Monster(goBuf1, Monster)






                    'If Monster_Life > 0 Then
                    '    goBuf1.FillRectangle(Monster_Brush, Monster)


                    '    goBuf1.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), Monster)




                    'End If

                    If ProjectileInflight = True Then
                        goBuf1.FillRectangle(Projectile_Brush, Projectile)
                    End If







                    Draw_Hero(goBuf1, OurHero.Rec)

                    'If OurHero.Life > 0 Then

                    '    If OurHero.Hit = False Then
                    '        'goBuf1.FillRectangle(New SolidBrush(OurHero.Color), OurHero.Rec)
                    '        Draw_Hero(goBuf1, OurHero.Rec)
                    '    Else
                    '        goBuf1.FillRectangle(New SolidBrush(Color.FromArgb(255, Color.Red)), OurHero.Rec)
                    '        OurHero.HitTimer += 1
                    '    End If
                    'End If


                    If Monster_Life > 0 And Monster_Hit = True Then
                        goBuf1.FillRectangle(Life_Frame_Brush, Monster.X, Monster.Y - 10, Monster.Width, 6)
                        goBuf1.FillRectangle(Life_Brush, Monster.X, Monster.Y - 10, CInt(Monster.Width / Monster_LifeMAX * Monster_Life), 6)
                    End If

                    'Draw Map************************************************************
                    'Draw map background.
                    goBuf1.FillRectangle(New SolidBrush(Color.FromArgb(64, Color.Black)), New Rectangle(Viewport_Size.Width - (Viewport_Size.Width \ 4) - 10, 10, Viewport_Size.Width \ 4, Viewport_Size.Height \ 4))


                    If Wall.Revealed = True Then
                        'Draw wall.
                        goBuf1.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(Viewport_Size.Width - (Viewport_Size.Width \ 4) - 10 + Wall.Rec.X \ 4, 10 + Wall.Rec.Y \ 4, Wall.Rec.Width \ 4, Wall.Rec.Height \ 4))
                    End If


                    'Draw_Hero(goBuf1, OurHero.Rec)






                    'Draw hero.
                    goBuf1.FillRectangle(New SolidBrush(OurHero.Color), New Rectangle(Viewport_Size.Width - (Viewport_Size.Width \ 4) - 10 + OurHero.Rec.X \ 4, 10 + OurHero.Rec.Y \ 4, OurHero.Rec.Width \ 4, OurHero.Rec.Height \ 4))

                    'Draw map border.
                    goBuf1.DrawRectangle(Map_Border_Pen, New Rectangle(Viewport_Size.Width - (Viewport_Size.Width \ 4) - 10, 10, Viewport_Size.Width \ 4, Viewport_Size.Height \ 4))
                    '********************************************************************

                    Draw_HeroLife_Bar(goBuf1, Life_Bar_Frame)


                    'goBuf1.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString & "     Undead - Attack:" & Monster_Attack.ToString, Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)

                    goBuf1.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString & "    Level 1 - 10000 : Gold ", Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)


                    'To fix DrawString error: "Parameters not valid." I had to set the compositing mode to source over.

                    goBuf1.DrawString(Instruction_Text, Instruction_Font, drawBrush, 0, Viewport_Size.Height - 30)

                    If OurHero.Life < 1 And Timer2.Enabled = True Then
                        goBuf1.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Red)), 0, 0, Viewport_Size.Width, Viewport_Size.Height)
                        goBuf1.DrawString("Died", PauseFont, drawBrush, Viewport_Size.Width \ 2, Viewport_Size.Height \ 2, Center_String)
                    End If


                    If Timer2.Enabled = False Then
                        goBuf1.FillRectangle(Fifty_Percent_Black_Brush, 0, 0, Viewport_Size.Width, Viewport_Size.Height)
                        goBuf1.DrawString("Paused", PauseFont, drawBrush, Viewport_Size.Width \ 2, Viewport_Size.Height \ 2, Center_String)
                    End If

                    e.Graphics.DrawImageUnscaled(Buffer1_BMP, 0, 0)

                End Using
            End Using
        Else
            Using _Buffer2 As New Bitmap(Viewport_Size.Width, Viewport_Size.Height, Imaging.PixelFormat.Format32bppPArgb)
                Using goBuf2 As Graphics = Graphics.FromImage(_Buffer2)
                    With goBuf2
                        'To fix draw string error: "Parameters not valid." I had to set the compositing mode to source over.
                        .CompositingMode = Drawing2D.CompositingMode.SourceOver
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .SmoothingMode = Drawing2D.SmoothingMode.None
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
                    End With

                    goBuf2.Clear(Level_Background_Color)

                    'Draw Wall
                    goBuf2.FillRectangle(New SolidBrush(Wall.Color), Wall.Rec)

                    If Potion.Active = True Then

                        Draw_Potion(goBuf2, Potion.Rec)

                    End If


                    Draw_Monster(goBuf2, Monster)


                    'If Monster_Life > 0 Then


                    '    goBuf2.FillRectangle(Monster_Brush, Monster)

                    '    goBuf2.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), Monster)

                    '    'Monster_Font

                    'End If

                    If ProjectileInflight = True Then
                        goBuf2.FillRectangle(Projectile_Brush, Projectile)
                    End If


                    Draw_Hero(goBuf2, OurHero.Rec)

                    'If OurHero.Life > 0 Then
                    '    'goBuf2.FillRectangle(Hero_Brush, Hero)


                    '    If OurHero.Hit = False Then
                    '        'goBuf2.FillRectangle(New SolidBrush(OurHero.Color), OurHero.Rec)
                    '        Draw_Hero(goBuf2, OurHero.Rec)
                    '    Else
                    '        goBuf2.FillRectangle(New SolidBrush(Color.FromArgb(255, Color.Red)), OurHero.Rec)

                    '        If OurHero.HitTimer > 1 Then
                    '            OurHero.HitTimer = 0
                    '            OurHero.Hit = False
                    '        End If



                    '    End If
                    'End If

                    If Monster_Life > 0 And Monster_Hit = True Then
                        goBuf2.FillRectangle(Life_Frame_Brush, Monster.X, Monster.Y - 10, Monster.Width, 6)
                        goBuf2.FillRectangle(Life_Brush, Monster.X, Monster.Y - 10, CInt(Monster.Width / Monster_LifeMAX * Monster_Life), 6)
                    End If

                    'Draw Map************************************************************
                    'Draw map background.
                    goBuf2.FillRectangle(New SolidBrush(Color.FromArgb(64, Color.Black)), New Rectangle(Viewport_Size.Width - (Viewport_Size.Width \ 4) - 10, 10, Viewport_Size.Width \ 4, Viewport_Size.Height \ 4))

                    'Wall.MapColor

                    If Wall.Revealed = True Then
                        'Draw wall.
                        goBuf2.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(Viewport_Size.Width - (Viewport_Size.Width \ 4) - 10 + Wall.Rec.X \ 4, 10 + Wall.Rec.Y \ 4, Wall.Rec.Width \ 4, Wall.Rec.Height \ 4))
                    End If







                    'Draw hero.
                    goBuf2.FillRectangle(New SolidBrush(OurHero.Color), New Rectangle(Viewport_Size.Width - (Viewport_Size.Width \ 4) - 10 + OurHero.Rec.X \ 4, 10 + OurHero.Rec.Y \ 4, OurHero.Rec.Width \ 4, OurHero.Rec.Height \ 4))

                    'Draw map border.
                    goBuf2.DrawRectangle(Map_Border_Pen, New Rectangle(Viewport_Size.Width - (Viewport_Size.Width \ 4) - 10, 10, Viewport_Size.Width \ 4, Viewport_Size.Height \ 4))
                    '********************************************************************

                    Draw_HeroLife_Bar(goBuf2, Life_Bar_Frame)

                    'goBuf2.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString & "     Undead - Attack:" & Monster_Attack.ToString, Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)
                    'To fix draw string error: "Parameters not valid." I had to set the compositing mode to source over.




                    'goBuf2.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString & "    Level 1 - ", Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)

                    goBuf2.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString & "    Level 1 - 10000 : Gold ", Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)



                    'goBuf2.DrawString(Instruction_Text, Instruction_Font, drawBrush, 0, 0)
                    goBuf2.DrawString(Instruction_Text, Instruction_Font, drawBrush, 0, Viewport_Size.Height - 30)


                    If OurHero.Life < 1 And Timer2.Enabled = True Then
                        goBuf2.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Red)), 0, 0, Viewport_Size.Width, Viewport_Size.Height)
                        goBuf2.DrawString("Died", PauseFont, drawBrush, Viewport_Size.Width \ 2, Viewport_Size.Height \ 2, Center_String)
                    End If

                    If Timer2.Enabled = False Then
                        goBuf2.FillRectangle(Fifty_Percent_Black_Brush, 0, 0, Viewport_Size.Width, Viewport_Size.Height)
                        'goBuf2.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        goBuf2.DrawString("Paused", PauseFont, drawBrush, Viewport_Size.Width \ 2, Viewport_Size.Height \ 2, Center_String)
                    End If

                    e.Graphics.DrawImageUnscaled(_Buffer2, 0, 0)

                End Using
            End Using
        End If
    End Sub
    Private Sub Draw_HeroLife_Bar(g As Graphics, Bar As Rectangle)


        'Draw hero life bar frame.
        g.FillRectangle(Life_Frame_Brush, Bar)



        Dim test As Double = (Bar.Width - 4) / OurHero.MaxLife * OurHero.Life


        'Is the heros life points critically low?
        If OurHero.Life > OurHero.MaxLife \ 4 Then
            'g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt(((Bar.Width - 4) \ OurHero.LifeMAX) * OurHero.Life), Bar.Height - 4)
            g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxLife * OurHero.Life), Bar.Height - 4)
        Else
            'Yes, the heros life points are critically low?

            Select Case Blink_Counter
                Case 0 To 1
                    g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxLife * OurHero.Life), Bar.Height - 4)
                    Blink_Counter += 1
                Case 2 To 4
                    g.FillRectangle(Life_Blink_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxLife * OurHero.Life), Bar.Height - 4)
                    Blink_Counter += 1
                Case Else
                    g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxLife * OurHero.Life), Bar.Height - 4)
                    Blink_Counter = 0
            End Select
        End If
    End Sub
    Private Sub Draw_Potion(g As Graphics, Rec As Rectangle)

        ''Create a path
        'Dim path As New GraphicsPath()
        ''path.AddEllipse(Elixir.X, Elixir.Y, Elixir.Width + 25, Elixir.Height + 25)
        'path.AddRectangle(New Rectangle(Rec.X - 10, Rec.Y - 10, Rec.Width + 20, Potion.Rec.Height + 20))

        ''path.AddEllipse(10, 10, 50, 50)
        ''Create a path gradient brush
        'Dim pgBrush As New PathGradientBrush(path)
        'pgBrush.CenterColor = Color.White
        'Dim list As Color() = New Color() {Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0)}
        'pgBrush.SurroundColors = list
        ''Create a Bitmap and ghraphics object
        ''Dim curBitmap As New Bitmap(500, 300)
        ''Dim g As Graphics = Graphics.FromImage(curBitmap)
        ''g.SmoothingMode = SmoothingMode.AntiAlias
        'g.FillPath(pgBrush, path)
        ''g.FillRectangle(lgBrush, 250, 20, 100, 100)


        g.FillRectangle(Life_Brush, Rec.X, Rec.Y, Rec.Width, Rec.Height)

        g.DrawString("Potion", Monster_Font, New SolidBrush(Color.Black), Rec, Center_String)

        g.DrawRectangle(New Pen(Color.DarkRed, 1), Rec)

    End Sub


    Private Sub Draw_Monster(g As Graphics, Rec As Rectangle)

        If Monster_Life > 0 Then
            g.FillRectangle(Monster_Brush, Monster)
            g.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), Monster, Center_String)
            g.DrawRectangle(New Pen(Color.Green, 1), Rec)

        End If

    End Sub


    Private Sub Draw_Hero(g As Graphics, Rec As Rectangle)

        If OurHero.Life > 0 Then
            If OurHero.Hit = False Then
                'Draw hero.
                g.FillRectangle(New SolidBrush(OurHero.Color), Rec)

                g.DrawString("Hero", Monster_Font, New SolidBrush(Color.Black), Rec, Center_String)

                g.DrawRectangle(New Pen(Color.DarkBlue, 1), Rec)


            Else
                g.FillRectangle(New SolidBrush(Color.FromArgb(255, Color.Red)), Rec)

                g.DrawString("Hero", Monster_Font, New SolidBrush(Color.Black), Rec, Center_String)

                g.DrawRectangle(New Pen(Color.Black, 2), Rec)

                OurHero.HitTimer += 1
                If OurHero.HitTimer > 2 Then
                    OurHero.HitTimer = 0
                    OurHero.Hit = False

                End If
            End If
        End If

    End Sub






    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        'Game timer

        Viewport.Width = PictureBox1.Width
        Viewport.Height = PictureBox1.Height

        'If Wall.Rec.IntersectsWith(Viewport) Then

        '    Wall.Revealed = True
        'End If

        'If RandomNumber.Next(1, 3) = 1 Then
        '    OurHero.Initiative = True
        'Else
        '    OurHero.Initiative = False
        'End If


        ''Roll for initative.
        'If RandomNumber.Next(1, 20) > 10 Then
        '    OurHero.Initiative = True
        'Else
        '    OurHero.Initiative = False
        'End If







        Do_Hero_Moves()

        Do_Monster_Moves()




        If Wall.Rec.IntersectsWith(Viewport) Then

            Wall.Revealed = True
        End If

    End Sub

    Private Sub Do_Hero_Moves()

        If OurHero.Life > 0 Then

            'Roll for initative.
            If RandomNumber.Next(1, 20) > 5 Then
                OurHero.Initiative = True
            Else
                OurHero.Initiative = False
            End If

            If MoveLeft = True Then
                'Move hero to the left.
                'OurHero.Rec.X -= OurHero.Speed

                MoveHero(DirectionEnum.Left)

                'Is the hero touching the wall?
                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                    'Yes, the hero is touching the wall.
                    'Push hero to the right of wall.
                    OurHero.Rec.X = Wall.Rec.X + Wall.Rec.Width
                End If

                'Is the monster alive?
                If Monster_Life > 0 Then
                    'Yes, the monster is alive.
                    'Is the hero touching the monster?
                    If OurHero.Rec.IntersectsWith(Monster) = True Then
                        'Yes, the hero is touching the monster.
                        'Push hero to the right of monster.
                        OurHero.Rec.X = Monster.X + Monster.Width + 1




                        If OurHero.Initiative = True Then
                            Monster_Hit = True
                            'Attack the monsters life points directly.
                            Monster_Life -= OurHero.Attack
                            If Monster_Life < 0 Then
                                Monster_Life = 0
                            End If



                            'Knock monster to the left of hero.
                            Monster.X = OurHero.Rec.X - Monster.Width - 32




                            'Wall Collision Handler Monster moing left*************************
                            'Is the monster touching the wall?
                            If Monster.IntersectsWith(Wall.Rec) = True Then
                                'Yes, the monster is touching the wall.

                                'Push monster to the right of wall.
                                Monster.X = Wall.Rec.X + Wall.Rec.Width

                            End If
                            '************************************************







                        End If
                    End If
                End If
            End If

            If MoveRight = True Then
                'Move hero to the right.
                'OurHero.Rec.X += OurHero.Speed

                MoveHero(DirectionEnum.Right)



                'Is the hero touching the wall?
                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                    'Yes, the hero is touching the wall.
                    'Push hero to the left of wall.
                    OurHero.Rec.X = Wall.Rec.X - OurHero.Rec.Width - 1

                End If

                'Is the monster alive?
                If Monster_Life > 0 Then
                    'Yes, the monster is alive.
                    'Is the hero touching the monster?
                    If OurHero.Rec.IntersectsWith(Monster) = True Then
                        'Yes, the hero is touching the monster.
                        'Push hero to the left of monster.
                        OurHero.Rec.X = Monster.X - Monster.Width - 1
                        If OurHero.Initiative = True Then
                            Monster_Hit = True


                            'Attack the monsters life points directly.
                            Monster_Life -= OurHero.Attack
                            If Monster_Life < 0 Then
                                Monster_Life = 0
                            End If


                            'Knock monster to the right of hero.
                            Monster.X = OurHero.Rec.X + OurHero.Rec.Width + 16

                            'Is the monster touching a wall?
                            If Monster.IntersectsWith(Wall.Rec) = True Then
                                'Yes, the monster is touching a wall.

                                'Knock monster to the left of wall.
                                Monster.X = Wall.Rec.X - OurHero.Rec.Width - 16

                                'Knock the hero to the left of the monster.
                                OurHero.Rec.X = Monster.X - Monster.Width - 8

                            End If


                            ''Wall Collision Handler - Moving Right*****************
                            ''Is the monster touching the wall?
                            'If Monster.IntersectsWith(Wall.Rec) = True Then
                            '    'Yes, the monster is touching the wall.

                            '    'Push monster to the left of wall.
                            '    Monster.X = Wall.Rec.X - Monster.Width - 16

                            'End If
                            ''************************************************

                        End If
                    End If
                End If
            End If

            If MoveUp = True Then
                'Move hero up.
                'OurHero.Rec.Y -= OurHero.Speed

                MoveHero(DirectionEnum.Up)

                'Is the hero touching the wall?
                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                    'Yes, the hero is touching the wall.
                    'Push the hero below the wall.
                    OurHero.Rec.Y = Wall.Rec.Y + Wall.Rec.Height
                End If

                'Is the monster alive?
                If Monster_Life > 0 Then
                    'Yes, the monster is alive.
                    'Is the hero touching the monster?
                    If OurHero.Rec.IntersectsWith(Monster) = True Then
                        'Yes, the hero is touching the monster.
                        'Push the hero below the monster.
                        OurHero.Rec.Y = Monster.Y + Monster.Height + 1
                        If OurHero.Initiative = True Then
                            Monster_Hit = True
                            'Attack the monsters life points directly.
                            Monster_Life -= OurHero.Attack

                            If Monster_Life < 0 Then
                                Monster_Life = 0
                            End If
                            'Knock monster above the hero.
                            Monster.Y = OurHero.Rec.Y - Monster.Height - 32
                        End If
                    End If
                End If
            End If

            If MoveDown = True Then
                'OurHero.Rec.Y += OurHero.Speed

                MoveHero(DirectionEnum.Down)


                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                    OurHero.Rec.Y = Wall.Rec.Y - OurHero.Rec.Height - 1
                End If



                If Monster_Life > 0 Then
                    If OurHero.Rec.IntersectsWith(Monster) = True Then
                        OurHero.Rec.Y = Monster.Y - OurHero.Rec.Height - 1
                        If OurHero.Initiative = True Then
                            Monster_Hit = True
                            'Attack the monsters life points directly.
                            Monster_Life -= OurHero.Attack

                            If Monster_Life < 0 Then
                                Monster_Life = 0
                            End If
                            'Knock monster below the hero.
                            Monster.Y = OurHero.Rec.Y + Monster.Height + 32

                            'Wall Collision Handler - Moving Down ********************************************************
                            'Is the monster touching the wall?
                            If Monster.IntersectsWith(Wall.Rec) = True Then
                                'Yes, the monster is touching the wall.

                                'Push the monster above the wall.
                                Monster.Y = Wall.Rec.Y - (Monster.Height - 1)

                            End If
                            '************************************************
                        End If
                    End If
                End If
            End If




            Do_Hero_Shots()


            If Potion.Active = True Then
                If OurHero.Rec.IntersectsWith(Potion.Rec) = True And OurHero.Life <> OurHero.MaxLife Then

                    OurHero.Life = OurHero.MaxLife
                    Potion.Active = False

                    'Play potion pickupsound.
                    If GS.IsPlaying("Potion_Pickup") = False Then
                        GS.Play("Potion_Pickup")
                    End If

                End If
            End If


        End If


    End Sub

    Private Sub Do_Monster_Moves()
        'Monster Moves **********************************************************************************************************
        'Is the monster and the hero alive?
        If Monster_Life > 0 And OurHero.Life > 0 Then
            'Yes, the monster and the hero live.

            'Proximity Based Chase Behavior
            'Is the monster near the hero?
            If Distance_Between_Points(Monster.Location, OurHero.Rec.Location) < Viewport_Size.Width Then
                'Yes, the monster is near the hero.

                Dim Monster_Center_X As Integer = Monster.X + Monster.Width \ 2
                Dim Hero_Center_X As Integer = OurHero.Rec.X + OurHero.Rec.Width \ 2


                'Is the monster centered with the hero horizontally?
                If Monster_Center_X <> Hero_Center_X Then
                    'No, the monster is not centered horizontally with the hero.

                    'Is the monster to the left of the hero?
                    If Monster_Center_X < Hero_Center_X Then
                        'Yes, the monster is to the left of the hero.

                        'Move Monster Right**************************************************************
                        'Proximity based speed controller. - Fixes Bug: The monster sometimes oscillates.
                        'Is the monster close to the hero?
                        If Horizontal_Distance(Monster_Center_X, Hero_Center_X) > 8 Then
                            'No, the monster is not close to the hero.
                            'Move the monster to the right fast.
                            Monster.X += Monster_Speed
                        Else
                            'Yes, the monster is close to the hero.
                            'Move the monster to the right slowly.
                            Monster.X += 1
                        End If

                        'Play undead moveing sound.
                        If GS.IsPlaying("Undead_Move") = False Then
                            GS.Play("Undead_Move")
                        End If
                        '*********************************************************************************


                        'Wall Collision Handler - Moving Right*****************
                        'Is the monster touching the wall?
                        If Monster.IntersectsWith(Wall.Rec) = True Then
                            'Yes, the monster is touching the wall.

                            'Push monster to the left of wall.
                            Monster.X = Wall.Rec.X - Monster.Width - 1

                            If GS.IsPlaying("Undead_Move") = True Then
                                GS.Pause("Undead_Move")
                            End If

                        End If
                        '************************************************

                        'Attack Right**************************************************
                        'Is the monster touching the hero?
                        If Monster.IntersectsWith(OurHero.Rec) = True Then

                            'Yes, the monster is touching the hero.

                            'Push the monster to the left of the hero.
                            Monster.X = OurHero.Rec.X - Monster.Width - 1





                            If OurHero.Initiative = False Then


                                'GS.Play("Monster") 'play the Music

                                'Attack the heros life points directly.
                                OurHero.Life -= Monster_Attack

                                OurHero.Hit = True

                                'Play undead attack sound.
                                If GS.IsPlaying("Undead_Attack") = False Then
                                    GS.Play("Undead_Attack") 'play the Music
                                End If

                                'If Monster_AttackTimer = 0 Then
                                '        GS.Play("Monster") 'play the Music
                                '        Monster_AttackTimer += 1
                                '    ElseIf Monster_AttackTimer < 4 Then
                                '        Monster_AttackTimer += 1
                                '    Else
                                '        Monster_AttackTimer = 0
                                '    End If




                                'Knock hero to the right of monster.
                                OurHero.Rec.X = Monster.X + Monster.Width + 16



                                'Is the hero touching a wall?
                                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the hero is touching a wall.

                                    'Knock hero to the left of wall.
                                    OurHero.Rec.X = Wall.Rec.X - OurHero.Rec.Width - 16

                                    'Knock the monster to the left of the hero.
                                    Monster.X = OurHero.Rec.X - OurHero.Rec.Width - 8

                                End If

                            End If

                        End If

                        '********************************************************************************
                    Else

                        'No, the monster is to the right of the hero.

                        'Move the monster to the left.
                        'Proximity based speed controller. - Fixes Bug: The monster sometimes oscillates.**************
                        'Is the monster close to the hero?
                        If Math.Abs(Monster.X + (Monster.Width \ 2) - (OurHero.Rec.X + OurHero.Rec.Width \ 2)) > 8 Then

                            'No, the monster is not close to the hero.
                            'Move the monster to the left fast.
                            Monster.X -= Monster_Speed
                        Else
                            'Yes, the monster is close to the hero.
                            'Move the monster to the left slowly.
                            Monster.X -= 1
                        End If

                        'Play undead moveing sound.
                        If GS.IsPlaying("Undead_Move") = False Then
                            GS.Play("Undead_Move")
                        End If
                        '***********************************************************************************************



                        'Wall Collision Handler Monster moing left*************************
                        'Is the monster touching the wall?
                        If Monster.IntersectsWith(Wall.Rec) = True Then
                            'Yes, the monster is touching the wall.

                            'Push monster to the right of wall.
                            Monster.X = Wall.Rec.X + Wall.Rec.Width

                            If GS.IsPlaying("Undead_Move") = True Then
                                GS.Pause("Undead_Move")
                            End If

                        End If
                        '************************************************


                        'Attack Left************************************************************************************
                        'Is the monster touching the hero?
                        If Monster.IntersectsWith(OurHero.Rec) = True Then
                            'Yes, the monster is touching the hero.

                            'Push the monster to the right of the hero.
                            Monster.X = OurHero.Rec.X + Monster.Width + 1

                            If OurHero.Initiative = False Then

                                'Attack the heros life points directly.
                                OurHero.Life -= Monster_Attack

                                OurHero.Hit = True

                                'Play undead attack sound.
                                If GS.IsPlaying("Undead_Attack") = False Then
                                    GS.Play("Undead_Attack") 'play the Music
                                End If

                                'Knock hero to the left of monster.
                                OurHero.Rec.X = Monster.X - Monster.Width - 16

                                'Is the hero touching a wall?
                                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the hero is touching a wall.

                                    'Knock hero to the right of wall.
                                    OurHero.Rec.X = Wall.Rec.X + Wall.Rec.Width + 16

                                    'Knock the monster to the right of the hero.
                                    Monster.X = OurHero.Rec.X + OurHero.Rec.Width + 8

                                End If
                            End If
                        End If
                        '*********************************************************************
                    End If
                End If
                'Is the monster centered with the hero vertically?
                If Monster.Y + Monster.Height \ 2 <> OurHero.Rec.Y + OurHero.Rec.Height \ 2 Then
                    'No, the monster is not centered vertically with the hero.

                    'Is the monster above the hero?
                    If Monster.Y + Monster.Height \ 2 < OurHero.Rec.Y + OurHero.Rec.Height \ 2 Then
                        'Yes, the monster is above the hero.


                        'Move monster down.
                        'Proximity based speed controller. - Fixes Bug: The monster sometimes oscillates.**************
                        'Is the monster close to the hero?
                        If Math.Abs(Monster.Y + (Monster.Height \ 2) - (OurHero.Rec.Y + OurHero.Rec.Height \ 2)) > 8 Then
                            'No, the monster is not close to the hero.

                            'Move the monster down fast.
                            Monster.Y += Monster_Speed
                        Else
                            'Yes, the monster is close to the hero.
                            'Move the monster down slowly.
                            Monster.Y += 1

                        End If

                        'Play undead moveing sound.
                        If GS.IsPlaying("Undead_Move") = False Then
                            GS.Play("Undead_Move")
                        End If
                        '***********************************************************************************************

                        'Wall Collision Handler - Moving Down ********************************************************
                        'Is the monster touching the wall?
                        If Monster.IntersectsWith(Wall.Rec) = True Then
                            'Yes, the monster is touching the wall.

                            'Push the monster above the wall.
                            Monster.Y = Wall.Rec.Y - Monster.Height - 1

                            If GS.IsPlaying("Undead_Move") = True Then
                                GS.Pause("Undead_Move")
                            End If

                        End If
                        '************************************************


                        If Monster.IntersectsWith(OurHero.Rec) = True Then
                            Monster.Y = OurHero.Rec.Y - Monster.Height - 1
                        End If

                    Else
                        'No, the monster is below the hero.



                        'Move the monster up.
                        'Is the monster close to the hero?
                        If Math.Abs(Monster.Y + (Monster.Height \ 2) - (OurHero.Rec.Y + OurHero.Rec.Height \ 2)) > 8 Then
                            'No, the monster is not close to the hero.
                            'Move the monster up fast.
                            Monster.Y -= Monster_Speed
                        Else
                            'Yes, the monster is close to the hero.
                            'Move the monster up slowly.
                            Monster.Y -= 1
                        End If

                        'Play undead moveing sound.
                        If GS.IsPlaying("Undead_Move") = False Then
                            GS.Play("Undead_Move")
                        End If

                        'Wall Collision Handler*************************
                        'Is the monster touching the wall?
                        If Monster.IntersectsWith(Wall.Rec) = True Then
                            'Yes, the monster is touching the wall.

                            'Push the hero below the wall.
                            Monster.Y = Wall.Rec.Y + Wall.Rec.Height

                            If GS.IsPlaying("Undead_Move") = True Then
                                GS.Pause("Undead_Move")
                            End If

                        End If
                        '************************************************




                        If Monster.IntersectsWith(OurHero.Rec) = True Then
                            Monster.Y = OurHero.Rec.Y + OurHero.Rec.Height + 1
                        End If

                    End If
                End If


            End If

        Else
            If Monster_Life = 0 Then

                'Drop loot
                'Potion.Rec.Location = Monster.Location
                Potion.Rec.Size = New Size(65, 65)
                Potion.Rec.X = Monster.X + (Monster.Width - Potion.Rec.Width) \ 2
                Potion.Rec.Y = Monster.Y + (Monster.Height - Potion.Rec.Height) \ 2
                Potion.Active = True

                'Play loot drop sound.

                Monster_Life = -1

            End If

            'Start respawn timer.
            Timer3.Interval = 5000 'Set timer for 5 seconds - 1000 miliseconds = 1 second
            Timer3.Start()
        End If



    End Sub



    Private Sub Do_Hero_Shots()


        'Does the player want to cast a spell and isn't already casting a spell.
        If CtrlDown = True And ProjectileInflight = False Then
            'Yes, the player wants to cast a spell and isn't already casting a spell.
            'Determine the hero's direction of fire.**************************
            If MoveRight = True Then
                If MoveUp = True Then
                    Projectile_Direction = DirectionEnum.RightUP
                ElseIf MoveDown = True Then
                    Projectile_Direction = DirectionEnum.RightDown
                Else
                    Projectile_Direction = DirectionEnum.Right
                End If
            ElseIf MoveLeft = True Then
                If MoveUp = True Then
                    Projectile_Direction = DirectionEnum.LeftUp
                ElseIf MoveDown = True Then
                    Projectile_Direction = DirectionEnum.LeftDown
                Else
                    Projectile_Direction = DirectionEnum.Left
                End If
            ElseIf MoveUp = True Then
                Projectile_Direction = DirectionEnum.Up
            ElseIf MoveDown = True Then
                Projectile_Direction = DirectionEnum.Down
            Else
                Projectile_Direction = DirectionEnum.None
            End If
            '*************************************************************
            'Fire**************************************************
            If Projectile_Direction <> DirectionEnum.None Then

                'Position the projectile under the hero. Make the projectile the same size as the hero.
                Projectile = OurHero.Rec

                'Loose the projectile.
                ProjectileInflight = True

                'Play projectile in flight sound.
                GS.Play("Magic")

            End If
            '******************************************************
        End If


        'Move projectile and attack the monster when hit.*****************************************************************************
        'Has the player cast a spell?
        If ProjectileInflight = True Then
            'Yes the player has cast a spell.
            'Is the projectile within it's range?
            If Distance_Between_Points(Projectile.Location, OurHero.Rec.Location) < Projectile_Max_Distance Then
                'Yes, the projectile is within it's range.
                'What direction is the player casting in?
                Select Case Projectile_Direction
                    Case DirectionEnum.Right
                        'The player is casting to the right.
                        'Move the projectile to the right.
                        Projectile.X += Projectile_Speed
                        'Is the monster alive?
                        If Monster_Life > 0 Then
                            'Yes, the monster is alive.
                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then
                                'Yes, the hero is touching the monster.
                                Monster_Hit = True
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If
                                'Knock monster to the right of hero.
                                Monster.X += CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Moving Right*****************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push monster to the left of wall.
                                    Monster.X = Wall.Rec.X - Monster.Width - 1

                                End If
                                '************************************************
                            End If
                        End If
                    Case DirectionEnum.Left
                        'Move projectile to the left.
                        Projectile.X -= Projectile_Speed
                        'Is the monster alive?
                        If Monster_Life > 0 Then
                            'Yes, the monster is alive.
                            'Is the hero touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then
                                'Yes, the hero is touching the monster.
                                Monster_Hit = True
                                'ProjectileInflight = False
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If
                                'Knock monster to the left of hero.
                                Monster.X -= CInt(Projectile_Speed / 3)


                                'Wall Collision Handler Monster moing left*************************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push monster to the right of wall.
                                    Monster.X = Wall.Rec.X + Wall.Rec.Width

                                End If
                                '************************************************






                            End If
                        End If
                    Case DirectionEnum.Up
                        'If Horizontal_Distance(Projectile.X, OurHero.Rec.X) < Projectile_Max_Distance Then
                        Projectile.Y -= Projectile_Speed
                        'Is the monster alive?
                        If Monster_Life > 0 Then
                            'Yes, the monster is alive.
                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then
                                'Yes, the hero is touching the monster.
                                Monster_Hit = True
                                'ProjectileInflight = False
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If
                                'Knock monster to the right of hero.
                                Monster.Y -= CInt(Projectile_Speed / 3)
                            End If
                        End If
                    Case DirectionEnum.Down
                        'If Horizontal_Distance(Projectile.X, OurHero.Rec.X) < Projectile_Max_Distance Then
                        Projectile.Y += Projectile_Speed
                        'Is the monster alive?
                        If Monster_Life > 0 Then
                            'Yes, the monster is alive.
                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then
                                'Yes, the hero is touching the monster.
                                Monster_Hit = True
                                'ProjectileInflight = False
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If
                                'Knock monster to the right of hero.
                                Monster.Y += CInt(Projectile_Speed / 3)
                            End If
                        End If
                    Case DirectionEnum.RightUP
                        'If Horizontal_Distance(Projectile.X, OurHero.Rec.X) < Projectile_Max_Distance Then
                        Projectile.X += Projectile_Speed
                        Projectile.Y -= Projectile_Speed
                        'Is the monster alive?
                        If Monster_Life > 0 Then
                            'Yes, the monster is alive.
                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then
                                'Yes, the hero is touching the monster.
                                Monster_Hit = True
                                'ProjectileInflight = False
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If
                                'Knock monster to the right of hero.
                                Monster.X += CInt(Projectile_Speed / 3)
                                Monster.Y -= CInt(Projectile_Speed / 3)
                            End If
                        End If
                    Case DirectionEnum.RightDown
                        'If Horizontal_Distance(Projectile.X, OurHero.Rec.X) < Projectile_Max_Distance Then
                        Projectile.X += Projectile_Speed
                        Projectile.Y += Projectile_Speed
                        'Is the monster alive?
                        If Monster_Life > 0 Then
                            'Yes, the monster is alive.
                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then
                                'Yes, the hero is touching the monster.
                                Monster_Hit = True
                                'ProjectileInflight = False
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If
                                'Knock monster to the right of hero.
                                Monster.X += CInt(Projectile_Speed / 3)
                                Monster.Y += CInt(Projectile_Speed / 3)
                            End If
                        End If
                    Case DirectionEnum.LeftUp
                        'If Horizontal_Distance(Projectile.X, OurHero.Rec.X) < Projectile_Max_Distance Then
                        Projectile.X -= Projectile_Speed
                        Projectile.Y -= Projectile_Speed
                        'Is the monster alive?
                        If Monster_Life > 0 Then
                            'Yes, the monster is alive.
                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then
                                'Yes, the hero is touching the monster.
                                Monster_Hit = True
                                'ProjectileInflight = False
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If
                                'Knock monster to the right of hero.
                                Monster.X -= CInt(Projectile_Speed / 3)
                                Monster.Y -= CInt(Projectile_Speed / 3)
                            End If
                        End If
                    Case DirectionEnum.LeftDown
                        'The player is casting to the left and down.

                        'Move projectile to the left.
                        Projectile.X -= Projectile_Speed
                        'Move projectile down.
                        Projectile.Y += Projectile_Speed

                        'Is the monster alive?
                        If Monster_Life > 0 Then
                            'Yes, the monster is alive.
                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then
                                'Yes, the hero is touching the monster.

                                Monster_Hit = True

                                'Play monster hit sound

                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock the monster to the right of hero.
                                Monster.X -= CInt(Projectile_Speed / 3)
                                'Knock the monster below the hero.
                                Monster.Y += CInt(Projectile_Speed / 3)

                            End If
                        End If
                End Select
            Else
                'No, the projectile is outside it's range.

                'Stop the projectile.
                ProjectileInflight = False

            End If
        End If



    End Sub
    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick

        If Monster_Life = -1 Then

            'Repawn monster.
            Monster_Life = Monster_LifeMAX

            Monster_Hit = False

        End If

        If OurHero.Life = 0 Then

            'Repawn hero.
            OurHero.Life = OurHero.MaxLife


        End If

        Timer3.Stop()


    End Sub
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        Select Case e.KeyValue
            Case Keys.Left
                MoveLeft = True

            Case Keys.Right

                MoveRight = True

            Case Keys.Up
                MoveUp = True

            Case Keys.Down
                MoveDown = True

            Case Keys.ControlKey

                CtrlDown = True

            Case Keys.P
                'Pause game timer.
                If Timer2.Enabled = True Then
                    Timer2.Stop()
                    If Timer3.Enabled = True Then
                        Timer3.Stop()
                    End If
                Else
                    Timer2.Start()
                    If Monster_Life = -1 Then
                        Timer3.Start()
                    End If
                End If
        End Select

    End Sub
    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp

        Select Case e.KeyValue
            Case Keys.Left
                MoveLeft = False

            Case Keys.Right
                MoveRight = False

            Case Keys.Up
                MoveUp = False

            Case Keys.Down
                MoveDown = False

            Case Keys.ControlKey


                CtrlDown = False

            Case Keys.P


        End Select

    End Sub
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        Viewport_Size.Width = PictureBox1.Width
        Viewport_Size.Height = PictureBox1.Height

        Life_Bar_Frame.X = 10
        Life_Bar_Frame.Y = 10
        Life_Bar_Frame.Width = Viewport_Size.Width \ 4
        Life_Bar_Frame.Height = 20

    End Sub
    Private Function Distance_Between_Points(Point1 As Point, Point2 As Point) As Double

        'Returns the distance between two points.
        Distance_Between_Points = Sqrt((Abs(Point2.X - Point1.X) ^ 2) + (Abs(Point2.Y - Point1.Y) ^ 2))


        'd = √((x2 - x1)² + (y2 - y1)²) Distance Formula - khanacademy.org
        'Distance = Sqr((Abs(x2 - x1) ^ 2) + (Abs(y2 - y1) ^ 2)) Killer42 - bytes.com
        'Application of the Pythagorean Theorem. - Analytic Geometry

    End Function
    Private Function Horizontal_Distance(X1 As Integer, X2 As Integer) As Double

        Horizontal_Distance = Abs(X1 - X2)

    End Function
    Private Function Vertical_Distance(Y1 As Integer, Y2 As Integer) As Double

        Vertical_Distance = Abs(Y1 - Y2)

    End Function
    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        Me.Form1_Resize(sender, e)

    End Sub
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GS.Dispose() 'make sure you call dispose on the new GameSounds class when closing form
    End Sub
    Private Function Get_Hero_Direction() As DirectionEnum


        If MoveRight = True Then
            If MoveUp = True Then
                Get_Hero_Direction = DirectionEnum.RightUP
                Exit Function
            ElseIf MoveDown = True Then
                Get_Hero_Direction = DirectionEnum.RightDown
                Exit Function
            Else
                Get_Hero_Direction = DirectionEnum.Right
                Exit Function
            End If
        ElseIf MoveLeft = True Then
            If MoveUp = True Then
                Get_Hero_Direction = DirectionEnum.LeftUp
                Exit Function
            ElseIf MoveDown = True Then
                Get_Hero_Direction = DirectionEnum.LeftDown
                Exit Function
            Else
                Get_Hero_Direction = DirectionEnum.Left
                Exit Function
            End If
        ElseIf MoveUp = True Then
            Get_Hero_Direction = DirectionEnum.Up
            Exit Function
        ElseIf MoveDown = True Then
            Get_Hero_Direction = DirectionEnum.Down
            Exit Function
        Else
            Get_Hero_Direction = DirectionEnum.None
            Exit Function
        End If


    End Function
    Private Sub MoveHero(Direction As DirectionEnum)

        Select Case Direction
            Case DirectionEnum.Right
                'Move hero to the right.
                OurHero.Rec.X += OurHero.Speed
                Exit Sub
            Case DirectionEnum.Left
                'Move hero to the left.
                OurHero.Rec.X -= OurHero.Speed
                Exit Sub
            Case DirectionEnum.Up
                'Move hero up.
                OurHero.Rec.Y -= OurHero.Speed
                Exit Sub
            Case DirectionEnum.Down
                'Move hero down.
                OurHero.Rec.Y += OurHero.Speed
                Exit Sub
            Case DirectionEnum.RightUP
                'Move hero to the right.
                OurHero.Rec.X += OurHero.Speed
                'Move hero up.
                OurHero.Rec.Y -= OurHero.Speed
                Exit Sub
            Case DirectionEnum.RightDown
                'Move hero to the right.
                OurHero.Rec.X += OurHero.Speed
                'Move hero down.
                OurHero.Rec.Y += OurHero.Speed
                Exit Sub
            Case DirectionEnum.LeftUp
                'Move hero to the left.
                OurHero.Rec.X -= OurHero.Speed
                'Move hero up.
                OurHero.Rec.Y -= OurHero.Speed
                Exit Sub
            Case DirectionEnum.LeftDown
                'Move hero to the left.
                OurHero.Rec.X -= OurHero.Speed
                'Move hero down.
                OurHero.Rec.Y += OurHero.Speed
                Exit Sub
        End Select

    End Sub
    Private Sub Write_AudioFile_to_Path(MStream As MemoryStream, Path As String)
        'This sub procedure takes a memory stream from an audio resource
        'and saves it as an audio file at the path.
        '
        'A example call would look like this.
        '
        'Dim ms As New MemoryStream 'Create memory stream.
        'My.Resources.level_music.CopyTo(ms) 'Copy audio resource into memory stream.
        'Write_AudioFile_to_Path(ms, Application.StartupPath & "\level_music.wav")

        Dim AudioFile() As Byte = MStream.ToArray 'Copy memory stream into byte array.
        File.WriteAllBytes(Path, AudioFile) 'Write the byte array to the drive path.

        'Create a level music file in the apps start up path.
        'Dim ms As New MemoryStream
        'My.Resources.level_music.CopyTo(ms)
        'Dim AudioFile() As Byte = ms.ToArray
        'File.WriteAllBytes(Application.StartupPath & "\level_music.wav", AudioFile)
        'ms.Dispose()

    End Sub
    Sub PlayLoopingBackgroundSoundResource()
        'My.Computer.Audio.Play(My.Resources.level_music, AudioPlayMode.BackgroundLoop)
        Dim LoopPlayer As New System.Media.SoundPlayer(My.Resources.level_music)
        LoopPlayer.PlayLooping()
    End Sub
    Private Sub GS_SoundEnded(ByVal SndName As String) Handles GS.SoundEnded
        If SndName = "Music" Then GS.Play("Music") 'if the Music has reached the end then you can keep replaying it for a loop effect

        'If GS.IsPlaying("Music") = False Then

        'GS.Play("Music" )
        'GS.


        'End If

        'mciSendString("close FirstSound", String.Empty, 0, 0)
    End Sub
End Class
Public Class GameSounds
    Inherits NativeWindow

    Public Event SoundEnded(ByVal SndName As String)
    Public Event SoundStopped(ByVal SndName As String)
    Private Snds As New Dictionary(Of String, String)
    Private sndcnt As Integer = 0
    Private Const MM_MCINOTIFY As Integer = &H3B9

    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
    Private Shared Function mciSendStringW(<MarshalAs(UnmanagedType.LPTStr)> ByVal lpszCommand As String, <MarshalAs(UnmanagedType.LPWStr)> ByVal lpszReturnString As System.Text.StringBuilder, ByVal cchReturn As UInteger, ByVal hwndCallback As IntPtr) As Integer
    End Function

    Public Sub New()
        Me.CreateHandle(New CreateParams)
    End Sub

    ''' <summary>Adds and opens a (.wav) or (.mp3) sound file to the sound collection.</summary>
    ''' <param name="SoundName">A name to refer to the sound being added.</param>
    ''' <param name="SndFilePath">The full path and name to the sound file.</param>
    ''' <returns>True if the sound is successfully added.</returns>
    ''' <remarks>Name can only be used once. If another sound has the same name the file will not be added and the function returns False.</remarks>
    Public Function AddSound(ByVal SoundName As String, ByVal SndFilePath As String) As Boolean
        If SoundName.Trim = "" Or Not IO.File.Exists(SndFilePath) Then Return False
        If Snds.ContainsKey(SoundName) Then Return False
        If mciSendStringW("open " & Chr(34) & SndFilePath & Chr(34) & " alias " & "Snd_" & sndcnt.ToString, Nothing, 0, IntPtr.Zero) <> 0 Then Return False
        Snds.Add(SoundName, "Snd_" & sndcnt.ToString)
        sndcnt += 1
        Return True
    End Function

    ''' <summary>Closes the sound file and removes it from the sound collection.</summary>
    Public Sub RemoveSound(ByVal SoundName As String)
        If Not Snds.ContainsKey(SoundName) Then Exit Sub
        mciSendStringW("close " & Snds.Item(SoundName), Nothing, 0, IntPtr.Zero)
        Snds.Remove(SoundName)
    End Sub

    ''' <summary>Closes all sound files and removes them from the sound collection.</summary>
    Public Sub ClearSounds()
        For Each aliasname As String In Snds.Values
            mciSendStringW("close " & aliasname, Nothing, 0, IntPtr.Zero)
        Next
        Snds.Clear()
    End Sub

    ''' <summary>Plays the sound.</summary>
    ''' <param name="SoundName">The Name of the sound to play.</param>
    Public Function Play(ByVal SoundName As String) As Boolean
        If Not Snds.ContainsKey(SoundName) Then Return False
        mciSendStringW("seek " & Snds.Item(SoundName) & " to start", Nothing, 0, IntPtr.Zero)
        If mciSendStringW("play " & Snds.Item(SoundName) & " notify", Nothing, 0, Me.Handle) <> 0 Then Return False
        Return True
    End Function

    ''' <summary>Stops the sound.</summary>
    ''' <param name="SoundName">The Name of the sound to stop.</param>
    Public Function [Stop](ByVal SoundName As String) As Boolean
        If Not Snds.ContainsKey(SoundName) Then Return False
        If mciSendStringW("stop " & Snds.Item(SoundName), Nothing, 0, IntPtr.Zero) <> 0 Then Return False
        mciSendStringW("seek " & Snds.Item(SoundName) & " to start", Nothing, 0, IntPtr.Zero)
        Return True
    End Function

    ''' <summary>Pauses the sound.</summary>
    ''' <param name="SoundName">The Name of the sound to pause.</param>
    Public Function Pause(ByVal SoundName As String) As Boolean
        If Not Snds.ContainsKey(SoundName) Then Return False
        If IsPlaying(SoundName) Then
            If mciSendStringW("pause " & Snds.Item(SoundName), Nothing, 0, IntPtr.Zero) <> 0 Then Return False
            Return True
        End If
        Return False
    End Function

    ''' <summary>Resumes a paused sound.</summary>
    ''' <param name="SoundName">The Name of the sound to resume.</param>
    Public Function [Resume](ByVal SoundName As String) As Boolean
        If Not Snds.ContainsKey(SoundName) Then Return False
        If IsPaused(SoundName) Then
            If mciSendStringW("resume " & Snds.Item(SoundName), Nothing, 0, IntPtr.Zero) <> 0 Then Return False
            Return True
        End If
        Return False
    End Function

    ''' <summary>Checks the sounds playing status.</summary>
    ''' <param name="SoundName">The Name used to add and refer to the sound.</param>
    Public Function IsPlaying(ByVal SoundName As String) As Boolean
        Return (GetStatusString(SoundName, "mode") = "playing")
    End Function

    ''' <summary>Checks the sounds stopped status.</summary>
    ''' <param name="SoundName">The Name used to add and refer to the sound.</param>
    Public Function IsStopped(ByVal SoundName As String) As Boolean
        Return (GetStatusString(SoundName, "mode") = "stopped")
    End Function

    ''' <summary>Checks the sounds paused status.</summary>
    ''' <param name="SoundName">The Name used to add and refer to the sound.</param>
    Public Function IsPaused(ByVal SoundName As String) As Boolean
        Return (GetStatusString(SoundName, "mode") = "paused")
    End Function

    Private Function GetStatusString(ByVal sName As String, ByVal statustype As String) As String
        If Not Snds.ContainsKey(sName) Then Return String.Empty
        Dim buff As New System.Text.StringBuilder(128)
        mciSendStringW("status " & Snds.Item(sName) & " " & statustype, buff, 128, IntPtr.Zero)
        Return buff.ToString.Trim.ToLower
    End Function

    ''' <summary>Sets the Volume. Does not seem to work for (.wav) files. Works on mp3 though.</summary>
    ''' <param name="sName">The name of the sound.</param>
    ''' <param name="value">An integer value from 0 to 1000 to set the volume to.</param>
    Public Function SetVolume(ByVal sName As String, ByVal value As Integer) As Boolean
        If Not Snds.ContainsKey(sName) Then Return False
        If value < 0 Or value > 1000 Then Return False
        If mciSendStringW("setaudio " & Snds.Item(sName) & " volume to " & value.ToString, Nothing, 0, IntPtr.Zero) <> 0 Then Return False
        Return True
    End Function

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        MyBase.WndProc(m)
        If m.Msg = MM_MCINOTIFY Then
            Dim sn As String = ""
            Dim indx As Integer = 0
            For Each s As KeyValuePair(Of String, String) In Snds
                indx += 1
                If m.LParam.ToInt32 = indx Then
                    sn = s.Key
                    Exit For
                End If
            Next
            If CType(m.WParam.ToInt32, MCI_NOTIFY) = MCI_NOTIFY.ABORTED Then
                RaiseEvent SoundStopped(sn)
            End If
            If CType(m.WParam.ToInt32, MCI_NOTIFY) = MCI_NOTIFY.SUCCESSFUL Then
                RaiseEvent SoundEnded(sn)
            End If
        End If
    End Sub
    '
    Public Sub Dispose()
        ClearSounds()
        Me.DestroyHandle()
    End Sub

End Class
