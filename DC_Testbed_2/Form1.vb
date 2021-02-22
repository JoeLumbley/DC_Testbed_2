﻿Option Strict On

'Dungeon Crawl
'A work in progress...
'This is a simple action role-playing game in which the hero navigates a labyrinth,
'battles various monsters, avoids traps, solves puzzles, and loots any treasure that is found.
'Coded by Joseph Lumbley.

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
    Public LifeBeforeHit As Integer

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
    Public WallCollision As Boolean 'If true the monster has collide with a wall.

End Structure

Public Structure WallInfo
    Public Rec As Rectangle
    Public Color As Color
    Public OutlineColor As Color
    Public Revealed As Boolean 'If true the wall will be seen on the map.
    Public MapColor As Color
    Public MapOutlineColor As Color

End Structure

Public Structure PotionInfo

    Public IsLife As Boolean 'If true the potion is a potion of life. If false the potion is a magical potion.
    Public Rec As Rectangle
    Public Color As Color
    Public OutlineColor As Color
    Public Active As Boolean 'If true the potion will be seen and can be used in the level.
    Public Life As Integer
    Public Magic As Integer

End Structure

Public Structure LevelInfo

    Public BackgroundColor As Color
    Public Rec As Rectangle
    Public Text As String

End Structure
Public Enum ToolsEnum As Integer

    Pointer = 0
    Wall = 1

End Enum

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

Public Class Form1

    Private Viewport As New Rectangle(0, 0, 640, 480)

    Private _BufferFlag As Boolean = True

    Private Level As LevelInfo

    Private OurHero As HeroInfo

    'Create and Initialize editor state data.
    Private Editor_On As Boolean = False
    Private Show_Rulers As Boolean = True
    Private Selected_Tool As ToolsEnum = ToolsEnum.Pointer
    Private Pointer_Origin As New Point(0, 0)
    Private Pointer_Offset As New Point(0, 0)
    Private Selected_Index As Integer = 0
    Private IsSelected As Boolean = False
    Private Selected_Pen As New Pen(Color.Blue, 5)

    Private BottomRightControlHandle_Selected As Boolean = False
    Private TopLeftControlHandle_Selected As Boolean = False

    Private Wall As WallInfo
    Private Wall_Origin As Point
    Private Walls() As WallInfo

    Private Potion As PotionInfo

    Private Map As Rectangle

    Private OurMonster As MonsterInfo

    Private Monster As New Rectangle(500, 500, 90, 90)
    Private Monster_Brush As New SolidBrush(Color.FromArgb(255, 14, 88, 34))
    Private Monster_LifeMAX As Integer = 50
    Private Monster_Life As Integer = Monster_LifeMAX
    Private Monster_Attack As Integer = 4
    Private Monster_Hit As Boolean = False
    Private Monster_Speed As Integer = 2
    Private Monster_AttackTimer As Integer = 0
    Private Monster_Font As New Font("Arial", 12)
    Private Monster_Initiative As Boolean = False

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

    Private Mouse_Down As Boolean = False

    Private Life_Brush As New SolidBrush(Color.FromArgb(255, 113, 9, 14))

    Private Life_Outline_Pen As New Pen(Color.FromArgb(255, 255, 0, 0), 1)

    Private Life_Frame_Brush As New SolidBrush(Color.FromArgb(255, 83, 6, 11))

    Private Life_Blink_Brush As New SolidBrush(Color.FromArgb(255, 170, 0, 0))

    Dim Life_Blink_Counter As Integer = 0

    Private Life_Bar_Frame As Rectangle
    Private Life_Bar_Font As New Font("Arial", 15)

    Private Magic_Brush As New SolidBrush(Color.FromArgb(255, 0, 0, 145))
    Private Magic_Blink_Brush As New SolidBrush(Color.FromArgb(255, 0, 0, 227))

    Dim Magic_Blink_Counter As Integer = 0

    Private Magic_Outline_Pen As New Pen(Color.FromArgb(255, 0, 128, 255), 1)

    Private Magic_Frame_Brush As New SolidBrush(Color.FromArgb(255, 0, 0, 119))
    Private Magic_Bar_Frame As Rectangle

    Private Instruction_Text As String = "Use arrow keys to move. Bump to attack. Use Ctrl + arrow keys to cast spells. Press P to pause, M for map and I to hide/show instructions."
    Private Instruction_Font As New Font("Arial", 14)

    Private Instructions_On As Boolean = True

    Private Fifty_Percent_Black_Brush As New SolidBrush(Color.FromArgb(128, Color.Black))
    Private Pause_Screen_Brush As New SolidBrush(Color.FromArgb(128, Color.Black))
    Private PauseFont As New Font("Arial", 50)

    Dim RandomNumber As New Random()

    Dim Center_String As New StringFormat()
    Dim Left_Aline_String As New StringFormat()

    Private Map_Border_Pen As New Pen(Color.FromArgb(16, Color.White), 3)

    Private Map_On As Boolean = False

    Dim LightRec As New Rectangle

    'Set up Game Sound
    Private WithEvents GS As New GameSounds

    ' Create font and brush.
    Private drawFont As New Font("Arial", 16)
    Private drawBrush As New SolidBrush(Color.White)
    Dim drawString As String = "Sample Text"

    Private Blur_BMP1 As New Bitmap(Viewport.Width, Viewport.Height, Imaging.PixelFormat.Format32bppPArgb)
    Private goBlur1 As Graphics = Graphics.FromImage(Blur_BMP1)

    Private Blur_BMP2 As New Bitmap(Viewport.Width, Viewport.Height, Imaging.PixelFormat.Format32bppPArgb)
    Private goBlur2 As Graphics = Graphics.FromImage(Blur_BMP2)

    Dim cm As New Drawing.Imaging.ColorMatrix
    Dim atr As New Drawing.Imaging.ImageAttributes

    Private Intersection_REC As New Rectangle(0, 0, 1, 1)

    Private Const Quarter_Number As Double = 0.25

    Dim x As Single = 0
    Dim y As Single = 0

    Dim drawFormat As New StringFormat

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        CreateSoundFileFromResource()

        Level.BackgroundColor = Color.FromArgb(255, 20, 20, 20)
        Level.Rec.X = 0
        Level.Rec.Y = 0
        Level.Rec.Width = 5300
        Level.Rec.Height = 5300

        MenuItemShowHideRulers.Checked = True

        OurHero.Rec = New Rectangle(0, 0, 90, 90)
        OurHero.Color = Color.FromArgb(255, 157, 150, 0)
        OurHero.OutlineColor = Color.FromArgb(255, 255, 242, 0)
        OurHero.MapColor = Color.FromArgb(64, 255, 242, 0)

        OurHero.Life = 100
        OurHero.MaxLife = 100
        OurHero.Magic = 100
        OurHero.MaxMagic = 100

        OurHero.Attack = 3
        OurHero.Initiative = False
        OurHero.Speed = 4
        OurHero.Hit = False
        OurHero.HitTimer = 0
        OurHero.Life = 100
        OurHero.LifeBeforeHit = 100
        OurMonster.WallCollision = False

        Wall.Rec.X = 2500
        Wall.Rec.Y = 300
        Wall.Rec.Width = 125
        Wall.Rec.Height = 125

        Wall.Color = Color.FromArgb(255, 72, 82, 67)

        Wall.OutlineColor = Color.FromArgb(255, 195, 195, 195)

        Wall.MapColor = Color.FromArgb(128, 164, 164, 164)

        'Init Magic Potion
        Potion.IsLife = False
        Potion.Rec = New Rectangle(0, 0, 200, 200)
        Potion.Active = False
        Potion.Color = Color.FromArgb(255, 0, 0, 255)
        Potion.OutlineColor = Color.FromArgb(255, 0, 0, 255)

        cm(3, 3) = 0.6   'draw with 60% alpha
        atr.SetColorMatrix(cm)

        Center_String.Alignment = StringAlignment.Center
        Center_String.LineAlignment = StringAlignment.Center

        Left_Aline_String.Alignment = StringAlignment.Far
        Left_Aline_String.LineAlignment = StringAlignment.Far

        MinimumSize = New Size(600, 480)

        drawFormat.Alignment = StringAlignment.Center

        SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        SetStyle(ControlStyles.UserPaint, False)

        SetStyle(ControlStyles.FixedHeight, True)
        SetStyle(ControlStyles.FixedWidth, True)

        WindowState = FormWindowState.Maximized

        'Load sound files
        GS.AddSound("Music", Application.StartupPath & "level_music.mp3") 'Worse - The Tower of Light from the YouTube Audio library.
        GS.AddSound("Magic", Application.StartupPath & "magic_sound.mp3")
        GS.AddSound("Monster", Application.StartupPath & "Monster Alien Roar Aggressive.mp3")
        GS.AddSound("Undead_Move", Application.StartupPath & "undead_move.mp3")
        GS.AddSound("Undead_Attack", Application.StartupPath & "undead_attack.mp3")
        GS.AddSound("Potion_Pickup", Application.StartupPath & "potion_pickup.mp3")
        GS.AddSound("Undead_Death", Application.StartupPath & "undead_death.mp3")
        GS.AddSound("Hero_Move", Application.StartupPath & "hero_move.mp3")
        GS.AddSound("Undead_Hit", Application.StartupPath & "undead_hit.mp3")
        GS.AddSound("Hero_Death", Application.StartupPath & "hero_death.mp3")
        GS.AddSound("Not_Enough_Magic", Application.StartupPath & "not_enough_magic.mp3")

        'Set set volume 
        GS.SetVolume("Music", 200)
        GS.SetVolume("Magic", 500)
        GS.SetVolume("Monster", 900)
        GS.SetVolume("Undead_Move", 500)
        GS.SetVolume("Undead_Attack", 400)
        GS.SetVolume("Potion_Pickup", 1000)
        GS.SetVolume("Undead_Death", 300)
        GS.SetVolume("Hero_Move", 200)
        GS.SetVolume("Undead_Hit", 800)
        GS.SetVolume("Hero_Death", 400)
        GS.SetVolume("Not_Enough_Magic", 400)

        'Set frame rate for the display timer.
        Timer1.Interval = 30

        'Set frame rate for the game timer.
        Timer2.Interval = 32

        'Start display timer.
        Timer1.Start()

        'Update the display.
        PictureBox1.Invalidate()

        GS.Play("Music") 'play the Music

        'Start the game timer.
        Timer2.Start()

    End Sub

    Private Shared Sub CreateSoundFileFromResource()

        'Create a level music file in the games start up path.
        Dim file As String = System.IO.Path.Combine(Application.StartupPath, "level_music.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.level_music)
        End If
        'Create a hero move file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "hero_move.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.hero_move)
        End If
        'Create a undead move sound file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "undead_move.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.undead_move)
        End If
        'Create a undead attack sound file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "undead_attack.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.undead_attack)
        End If
        'Create a undead hit sound file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "undead_hit.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.undead_hit)
        End If
        'Create a undead death sound file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "undead_death.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.undead_death)
        End If
        'Create a magic sound file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "magic_sound.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.magic_sound)
        End If
        'Create a not enough magic sound file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "not_enough_magic.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.not_enough_magic)
        End If
        'Create a potion pickup sound file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "potion_pickup.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.potion_pickup)
        End If
        'Create a hero death sound file in the games start up path.
        file = System.IO.Path.Combine(Application.StartupPath, "hero_death.mp3")
        If (Not System.IO.File.Exists(file)) Then
            System.IO.File.WriteAllBytes(file, My.Resources.hero_death)
        End If

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

            Using Buffer1_BMP As New Bitmap(Viewport.Width, Viewport.Height, Imaging.PixelFormat.Format32bppPArgb)

                Using goBuf1 As Graphics = Graphics.FromImage(Buffer1_BMP)
                    With goBuf1
                        .CompositingMode = Drawing2D.CompositingMode.SourceOver 'Bug Fix
                        'To fix draw string error: "Parameters not valid." Set the compositing mode to source over.
                        .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.Half
                    End With

                    With e.Graphics
                        .CompositingMode = Drawing2D.CompositingMode.SourceCopy
                        .SmoothingMode = Drawing2D.SmoothingMode.None
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
                    End With

                    If Editor_On = False Then

                        goBuf1.Clear(Level.BackgroundColor)

                        Draw_Hero_Light(goBuf1, OurHero.Rec)

                    Else

                        goBuf1.Clear(Level.BackgroundColor)

                        'Vertical lines
                        For index = 0 To Level.Rec.Width Step 100
                            goBuf1.DrawLine(New Pen(Color.Cyan, 1), New Point(index - Viewport.X, Level.Rec.Y - Viewport.Y), New Point(index - Viewport.X, Level.Rec.Height - Viewport.Y))

                            If Show_Rulers = True Then
                                If index <> Level.Rec.Width Then
                                    goBuf1.DrawString(index.ToString, Life_Bar_Font, drawBrush, index - Viewport.X, Level.Rec.Y - Viewport.Y)
                                End If
                            End If

                        Next

                        'Horizontal lines
                        For index = 0 To Level.Rec.Width Step 100

                            goBuf1.DrawLine(New Pen(Color.Cyan, 1), New Point(Level.Rec.X - Viewport.X, index - Viewport.Y), New Point(Level.Rec.Width - Viewport.X, index - Viewport.Y))

                            If Show_Rulers = True Then
                                If index <> 0 And index <> Level.Rec.Width Then
                                    goBuf1.DrawString(index.ToString, Life_Bar_Font, drawBrush, Level.Rec.X - Viewport.X, index - Viewport.Y)
                                End If
                            End If

                        Next

                    End If

                    If Potion.Active = True Then

                        Draw_Potion(goBuf1, Potion.Rec)

                    End If

                    Draw_Monster(goBuf1, Monster)

                    Draw_Projectile(goBuf1)

                    Draw_Walls(goBuf1)

                    Draw_Wall(goBuf1, Wall.Rec)

                    Draw_Monster_Life_Bar(goBuf1)

                    Draw_Hero(goBuf1, OurHero.Rec)

                    goBuf1.DrawString("Level 1", Life_Bar_Font, drawBrush, Map.X - 3, 6)

                    Draw_Map(goBuf1, Viewport.Width - Map.Width - 10, 40, 9)

                    If Editor_On = False Then
                        Draw_HeroLife_Bar(goBuf1, Life_Bar_Frame)

                        Draw_Hero_Magic_Bar(goBuf1, Magic_Bar_Frame)

                        If Instructions_On = True Then

                            Dim Instruction_Rec As New Rectangle(6, Viewport.Height - 60, 940, 200)
                            goBuf1.DrawString(Instruction_Text, Instruction_Font, New SolidBrush(Color.White), Instruction_Rec)

                        End If

                    End If

                    'Draw die screen.
                    If OurHero.Life < 1 And Timer2.Enabled = True Then
                        goBuf1.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Red)), 0, 0, Viewport.Width, Viewport.Height)
                        goBuf1.DrawString("Died", PauseFont, drawBrush, Viewport.Width \ 2, Viewport.Height \ 2, Center_String)
                    End If

                    'Draw paused screen.
                    If Timer2.Enabled = False Then
                        goBuf1.FillRectangle(Fifty_Percent_Black_Brush, 0, 0, Viewport.Width, Viewport.Height)
                        goBuf1.DrawString("Paused", PauseFont, drawBrush, Viewport.Width \ 2, Viewport.Height \ 2, Center_String)
                    End If

                    e.Graphics.DrawImageUnscaled(Buffer1_BMP, 0, 0)

                End Using
            End Using

        Else

            Using _Buffer2 As New Bitmap(Viewport.Width, Viewport.Height, Imaging.PixelFormat.Format32bppPArgb)

                Using goBuf2 As Graphics = Graphics.FromImage(_Buffer2)

                    With goBuf2
                        .CompositingMode = Drawing2D.CompositingMode.SourceOver 'Bug Fix
                        'To fix draw string error: "Parameters not valid." Set the compositing mode to source over.
                        .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.Half
                    End With

                    With e.Graphics
                        .CompositingMode = Drawing2D.CompositingMode.SourceCopy
                        .SmoothingMode = Drawing2D.SmoothingMode.None
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
                    End With

                    If Editor_On = False Then

                        goBuf2.Clear(Level.BackgroundColor)

                        Draw_Hero_Light(goBuf2, OurHero.Rec)

                    Else

                        goBuf2.Clear(Level.BackgroundColor)

                        'Vertical lines
                        For index = 0 To Level.Rec.Width Step 100
                            goBuf2.DrawLine(New Pen(Color.Cyan, 1), New Point(index - Viewport.X, Level.Rec.Y - Viewport.Y), New Point(index - Viewport.X, Level.Rec.Height - Viewport.Y))

                            If Show_Rulers = True Then
                                If index <> Level.Rec.Width Then
                                    goBuf2.DrawString(index.ToString, Life_Bar_Font, drawBrush, index - Viewport.X, Level.Rec.Y - Viewport.Y)
                                End If
                            End If

                        Next

                        'Horizontal lines
                        For index = 0 To Level.Rec.Width Step 100

                            goBuf2.DrawLine(New Pen(Color.Cyan, 1), New Point(Level.Rec.X - Viewport.X, index - Viewport.Y), New Point(Level.Rec.Width - Viewport.X, index - Viewport.Y))

                            If Show_Rulers = True Then
                                If index <> 0 And index <> Level.Rec.Width Then
                                    goBuf2.DrawString(index.ToString, Life_Bar_Font, drawBrush, Level.Rec.X - Viewport.X, index - Viewport.Y)
                                End If
                            End If

                        Next

                    End If

                    If Potion.Active = True Then

                        Draw_Potion(goBuf2, Potion.Rec)

                    End If

                    Draw_Monster(goBuf2, Monster)

                    Draw_Projectile(goBuf2)

                    Draw_Walls(goBuf2)

                    Draw_Wall(goBuf2, Wall.Rec)

                    Draw_Monster_Life_Bar(goBuf2)

                    Draw_Hero(goBuf2, OurHero.Rec)

                    goBuf2.DrawString("Level 1", Life_Bar_Font, drawBrush, Map.X - 3, 6)

                    Draw_Map(goBuf2, Viewport.Width - Map.Width - 10, 40, 9)

                    If Editor_On = False Then
                        Draw_HeroLife_Bar(goBuf2, Life_Bar_Frame)

                        Draw_Hero_Magic_Bar(goBuf2, Magic_Bar_Frame)

                        If Instructions_On = True Then

                            Dim Instruction_Rec As New Rectangle(6, Viewport.Height - 60, 940, 200)
                            goBuf2.DrawString(Instruction_Text, Instruction_Font, New SolidBrush(Color.White), Instruction_Rec)

                        End If

                    End If

                    If OurHero.Life < 1 And Timer2.Enabled = True Then
                        goBuf2.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Red)), 0, 0, Viewport.Width, Viewport.Height)
                        goBuf2.DrawString("Died", PauseFont, drawBrush, Viewport.Width \ 2, Viewport.Height \ 2, Center_String)
                    End If

                    If Timer2.Enabled = False Then
                        goBuf2.FillRectangle(Fifty_Percent_Black_Brush, 0, 0, Viewport.Width, Viewport.Height)
                        goBuf2.DrawString("Paused", PauseFont, drawBrush, Viewport.Width \ 2, Viewport.Height \ 2, Center_String)
                    End If

                    e.Graphics.DrawImageUnscaled(_Buffer2, 0, 0)

                End Using
            End Using
        End If

    End Sub

    Private Sub Draw_Map(g As Graphics, x As Integer, y As Integer, Scale As Integer)

        Map.X = x
        Map.Y = y
        Map.Width = Level.Rec.Width \ Scale
        Map.Height = Level.Rec.Height \ Scale

        If Map_On = True Then

            'Draw map background.
            g.FillRectangle(New SolidBrush(Color.FromArgb(16, Color.White)), New Rectangle(x, y, Level.Rec.Width \ Scale, Level.Rec.Height \ Scale))


            If Walls IsNot Nothing Then
                For index = 0 To UBound(Walls)
                    If Walls(index).Revealed = True Then

                        g.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(x + Walls(index).Rec.X \ Scale, y + Walls(index).Rec.Y \ Scale, CInt(Walls(index).Rec.Width / Scale), CInt(Walls(index).Rec.Height / Scale)))

                    End If
                Next
            End If

            'Draw hero.
            g.FillRectangle(New SolidBrush(OurHero.MapColor), New Rectangle(x + OurHero.Rec.X \ Scale, y + OurHero.Rec.Y \ Scale, OurHero.Rec.Width \ Scale, OurHero.Rec.Height \ Scale))

            'Draw map border.
            g.DrawRectangle(Map_Border_Pen, New Rectangle(x, y, Level.Rec.Width \ Scale, Level.Rec.Height \ Scale))

            'Draw viewport.
            g.DrawRectangle(Map_Border_Pen, New Rectangle(x + Viewport.X \ Scale, y + Viewport.Y \ Scale, Viewport.Width \ Scale, Viewport.Height \ Scale))

        End If

    End Sub

    Private Sub Draw_Projectile(g As Graphics)

        Dim ProjectileInViewportCoordinates As Rectangle

        ProjectileInViewportCoordinates = Projectile
        ProjectileInViewportCoordinates.X = Projectile.X - Viewport.X
        ProjectileInViewportCoordinates.Y = Projectile.Y - Viewport.Y

        If ProjectileInflight = True Then
            g.FillRectangle(Projectile_Brush, ProjectileInViewportCoordinates)
        End If

    End Sub

    Private Sub Draw_Monster_Life_Bar(g As Graphics)

        Dim MonsterInViewportCoordinates As Rectangle
        MonsterInViewportCoordinates = Monster
        MonsterInViewportCoordinates.X = Monster.X - Viewport.X
        MonsterInViewportCoordinates.Y = Monster.Y - Viewport.Y

        If Monster_Life > 0 And Monster_Hit = True Then
            g.FillRectangle(Life_Frame_Brush, MonsterInViewportCoordinates.X, MonsterInViewportCoordinates.Y - 10, MonsterInViewportCoordinates.Width, 6)
            g.FillRectangle(Life_Brush, MonsterInViewportCoordinates.X, MonsterInViewportCoordinates.Y - 10, CInt(MonsterInViewportCoordinates.Width / Monster_LifeMAX * Monster_Life), 6)
        End If

    End Sub

    Private Sub Draw_HeroLife_Bar(g As Graphics, Bar As Rectangle)

        'Draw hero life bar frame.
        g.FillRectangle(Life_Frame_Brush, Bar)

        'Is the heros life points critically low?
        If OurHero.Life > OurHero.MaxLife \ 4 Then
            'No, the heros life points are not critically low.

            g.FillRectangle(Life_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height)
            g.DrawRectangle(Life_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height - 1)

        Else
            'Yes, the heros life points are critically low.

            'Make the life bar blink.
            Select Case Life_Blink_Counter
                Case 0 To 8
                    g.FillRectangle(Life_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height)
                    g.DrawRectangle(Life_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height - 1)
                    Life_Blink_Counter += 1
                Case 9 To 18
                    g.FillRectangle(Life_Blink_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height)
                    g.DrawRectangle(Life_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height - 1)
                    Life_Blink_Counter += 1
                Case Else
                    g.FillRectangle(Life_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height)
                    g.DrawRectangle(Life_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height - 1)
                    Life_Blink_Counter = 0
            End Select

        End If

        g.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString, Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)

    End Sub

    Private Sub Draw_Hero_Magic_Bar(g As Graphics, Bar As Rectangle)

        'Draw hero magic bar frame.
        g.FillRectangle(Magic_Frame_Brush, Bar)

        'Is the heros magic points critically low?
        If OurHero.Magic >= OurHero.MaxMagic \ 4 Then
            g.FillRectangle(Magic_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)
            g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)
        Else
            'Yes, the heros life points are critically low?

            Select Case Magic_Blink_Counter
                Case 0 To 8
                    g.FillRectangle(Magic_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)
                    g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)

                    Magic_Blink_Counter += 1

                Case 9 To 18

                    g.FillRectangle(Magic_Blink_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)
                    g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)

                    Magic_Blink_Counter += 1

                Case Else

                    g.FillRectangle(Magic_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)
                    g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)

                    Magic_Blink_Counter = 0

            End Select
        End If

    End Sub

    Private Sub Draw_Potion(g As Graphics, Rec As Rectangle)

        Dim PotionInViewportCoordinates As Rectangle
        PotionInViewportCoordinates = Potion.Rec
        PotionInViewportCoordinates.X = Potion.Rec.X - Viewport.X
        PotionInViewportCoordinates.Y = Potion.Rec.Y - Viewport.Y

        If PotionInViewportCoordinates.IntersectsWith(LightRec) = True Then

            g.FillRectangle(New SolidBrush(Potion.Color), PotionInViewportCoordinates.X, PotionInViewportCoordinates.Y, PotionInViewportCoordinates.Width, Rec.Height)
            g.DrawString("Potion", Monster_Font, New SolidBrush(Color.Black), PotionInViewportCoordinates, Center_String)


            'Draw shadow.
            Dim MyShadow As Integer

            Dim Distance As Double = Distance_Between_Points(Potion.Rec.Location, OurHero.Rec.Location)

            If Distance <= Viewport.Width / 2 Then
                MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance_Between_Points(Potion.Rec.Location, OurHero.Rec.Location))
            Else

                MyShadow = 255
            End If
            g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), PotionInViewportCoordinates)

            g.DrawRectangle(New Pen(Potion.OutlineColor, 1), PotionInViewportCoordinates)

        Else

            g.FillRectangle(New SolidBrush(Potion.Color), PotionInViewportCoordinates.X, PotionInViewportCoordinates.Y, PotionInViewportCoordinates.Width, PotionInViewportCoordinates.Height)
            g.DrawString("Potion", Monster_Font, New SolidBrush(Color.Black), PotionInViewportCoordinates, Center_String)

            'Draw shadow.
            Dim MyShadow As Integer

            Dim Distance As Double = Distance_Between_Points(Potion.Rec.Location, OurHero.Rec.Location)

            If Distance <= Viewport.Width / 2 Then
                MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance_Between_Points(Potion.Rec.Location, OurHero.Rec.Location))
            Else

                MyShadow = 255
            End If
            g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), PotionInViewportCoordinates)

        End If

    End Sub

    Private Sub Draw_Monster(g As Graphics, Rec As Rectangle)

        Dim MonsterInViewportCoordinates As Rectangle
        MonsterInViewportCoordinates = Rec
        MonsterInViewportCoordinates.X = Rec.X - Viewport.X
        MonsterInViewportCoordinates.Y = Rec.Y - Viewport.Y

        'Is the editor on?
        If Editor_On = False Then
            'No, the editor off. The game is running.


            If Monster_Life > 0 Then
                If MonsterInViewportCoordinates.IntersectsWith(LightRec) = True Then

                    g.FillRectangle(Monster_Brush, MonsterInViewportCoordinates)
                    g.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), MonsterInViewportCoordinates, Center_String)

                    g.DrawRectangle(New Pen(Color.Green, 1), MonsterInViewportCoordinates)

                Else

                    g.FillRectangle(Monster_Brush, MonsterInViewportCoordinates)
                    g.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), MonsterInViewportCoordinates, Center_String)

                    g.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Black)), MonsterInViewportCoordinates)

                End If
            End If

        Else
            'Yes, the editor is on. The game is stopped.

            g.FillRectangle(Monster_Brush, MonsterInViewportCoordinates)
            g.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), MonsterInViewportCoordinates, Center_String)
            g.DrawRectangle(New Pen(Color.Green, 1), MonsterInViewportCoordinates)

        End If

    End Sub

    Private Sub Draw_Wall(g As Graphics, Rec As Rectangle)

        Dim WallInViewportCoordinates As Rectangle

        WallInViewportCoordinates = Rec
        WallInViewportCoordinates.X = Rec.X - Viewport.X
        WallInViewportCoordinates.Y = Rec.Y - Viewport.Y

        If Editor_On = True Then
            If Selected_Tool = ToolsEnum.Wall Then
                If Mouse_Down = True Then

                    'Draw Wall
                    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
                    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

                End If
            End If
        End If

    End Sub

    Private Sub Draw_Walls(g As Graphics)

        Dim WallInViewportCoordinates As Rectangle

        'Is the editor on?
        If Editor_On = False Then
            'No, the editor off. The game is running.

            'Do we have at least one wall?
            If Walls IsNot Nothing Then
                'Yes, we have at least one wall.

                'Draw every wall in walls.
                For index = 0 To UBound(Walls)
                    WallInViewportCoordinates = Walls(index).Rec
                    WallInViewportCoordinates.X = Walls(index).Rec.X - Viewport.X
                    WallInViewportCoordinates.Y = Walls(index).Rec.Y - Viewport.Y

                    If Walls(index).Rec.IntersectsWith(Viewport) = True Then

                        If WallInViewportCoordinates.IntersectsWith(LightRec) = True Then

                            'Draw wall.
                            g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
                            'Draw outline.
                            g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

                        Else

                            'Draw wall.
                            g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

                            'Draw shadow.
                            g.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Black)), WallInViewportCoordinates)

                        End If
                    End If
                Next
            End If

        Else
            'Yes, the editor is on. The game is stopped.

            'Draw every wall in walls.
            If Walls IsNot Nothing Then
                For index = 0 To UBound(Walls)
                    WallInViewportCoordinates = Walls(index).Rec
                    WallInViewportCoordinates.X = Walls(index).Rec.X - Viewport.X
                    WallInViewportCoordinates.Y = Walls(index).Rec.Y - Viewport.Y

                    'Draw wall.
                    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

                    'Draw outline.
                    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

                    'Draw wall number.
                    g.DrawString(index.ToString, Monster_Font, New SolidBrush(Color.Black), WallInViewportCoordinates, Center_String)

                Next
            End If


            If IsSelected = True Then

                'Draw selection outline.
                WallInViewportCoordinates = Walls(Selected_Index).Rec
                WallInViewportCoordinates.X = Walls(Selected_Index).Rec.X - Viewport.X
                WallInViewportCoordinates.Y = Walls(Selected_Index).Rec.Y - Viewport.Y

                'Draw selection outline.
                g.DrawRectangle(New Pen(Color.White, 5), WallInViewportCoordinates)

                'Draw outline.
                Dim Outline_Pen As New Pen(Color.Red, 2)
                Outline_Pen.DashStyle = DashStyle.Dash
                g.DrawRectangle(Outline_Pen, WallInViewportCoordinates)

                'Draw top/left control handle.
                g.FillEllipse(Brushes.Red, WallInViewportCoordinates.X - 15 \ 2, WallInViewportCoordinates.Y - 15 \ 2, 15, 15)

                'Draw bottom/right control handle.
                g.FillEllipse(Brushes.Red, WallInViewportCoordinates.Right - 15 \ 2, WallInViewportCoordinates.Bottom - 15 \ 2, 15, 15)

                'Draw X and Y coordinates.
                Dim MyString As String = Walls(Selected_Index).Rec.X.ToString & ", " & Walls(Selected_Index).Rec.Y.ToString
                g.DrawString(MyString, Monster_Font, New SolidBrush(Color.White), New Point(WallInViewportCoordinates.X, WallInViewportCoordinates.Y - 20), Center_String)

            End If
        End If

    End Sub

    Private Sub Draw_Hero_Light(g As Graphics, Rec As Rectangle)

        Dim HeroInViewportCoordinates As Rectangle
        HeroInViewportCoordinates = Rec
        HeroInViewportCoordinates.X = Rec.X - Viewport.X
        HeroInViewportCoordinates.Y = Rec.Y - Viewport.Y

        If ProjectileInflight = True Then

            LightRec = HeroInViewportCoordinates

            LightRec.Inflate(300, 300)

            'Create a path
            Dim path As New GraphicsPath()
            path.AddEllipse(LightRec)

            'Create a path gradient brush
            Dim pgBrush As New PathGradientBrush(path)

            pgBrush.CenterColor = Color.FromArgb(255, 255, 255, 255)

            Dim list As Color() = New Color() {Color.FromArgb(0, 255, 255, 255), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0)}

            pgBrush.SurroundColors = list

            g.FillPath(pgBrush, path)

        Else

            LightRec = HeroInViewportCoordinates

            LightRec.Inflate(300, 300)

            'Create a path
            Dim path As New GraphicsPath()
            path.AddEllipse(LightRec)

            'Create a path gradient brush
            Dim pgBrush As New PathGradientBrush(path)

            pgBrush.CenterColor = Color.FromArgb(90, Color.White)

            Dim list As Color() = New Color() {Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0)}
            pgBrush.SurroundColors = list

            g.FillPath(pgBrush, path)

        End If

    End Sub

    Private Sub Draw_Hero(g As Graphics, Rec As Rectangle)

        Dim HeroInViewportCoordinates As Rectangle
        HeroInViewportCoordinates = Rec
        HeroInViewportCoordinates.X = Rec.X - Viewport.X
        HeroInViewportCoordinates.Y = Rec.Y - Viewport.Y

        If OurHero.Life > 0 Then
            If OurHero.Hit = False Then

                'Draw hero.
                g.FillRectangle(New SolidBrush(OurHero.Color), HeroInViewportCoordinates)

                g.DrawString("Hero", Monster_Font, New SolidBrush(Color.Black), HeroInViewportCoordinates, Center_String)

                g.DrawRectangle(New Pen(OurHero.OutlineColor, 1), HeroInViewportCoordinates)


            Else
                g.FillRectangle(New SolidBrush(Color.FromArgb(255, Color.Red)), HeroInViewportCoordinates)

                g.DrawString("Hero", Monster_Font, New SolidBrush(Color.White), HeroInViewportCoordinates, Center_String)

                g.DrawRectangle(New Pen(Color.White, 1), HeroInViewportCoordinates)

                Dim HitTotal As Integer = OurHero.LifeBeforeHit - OurHero.Life
                If HitTotal < 0 Then
                    HitTotal = -1
                End If

                g.DrawString("-" & CStr(HitTotal), Monster_Font, New SolidBrush(Color.White), HeroInViewportCoordinates.X - 18, HeroInViewportCoordinates.Y - 35)

                OurHero.HitTimer += 1
                If OurHero.HitTimer > 4 Then

                    OurHero.Hit = False
                    OurHero.HitTimer = 0
                    OurHero.LifeBeforeHit = OurHero.Life

                End If
            End If
        End If

    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        'Game timer

        'Is the editor off?
        If Editor_On = False Then
            'Yes, the editor is off. The game is running.

            'Start the level music
            If GS.IsPlaying("Music") = False Then
                GS.Play("Music")
            End If

            'Set the title bar text to playing.
            If Me.Text <> "Dungeon Crawler - Playing" Then
                Me.Text = "Dungeon Crawler - Playing"
            End If

        Else
            'No, the editor is on. The game is stopped.

            'Stop the level music.
            GS.Stop("Music")

            'Set the title bar text to editing.
            If Me.Text <> "Dungeon Crawler - Editing" Then
                Me.Text = "Dungeon Crawler - Editing"
            End If

        End If

        'Set viewport size.
        Viewport.Width = PictureBox1.Width
        Viewport.Height = PictureBox1.Height

        'Set life bar postion and size.
        Life_Bar_Frame.X = 10
        Life_Bar_Frame.Y = 10
        Life_Bar_Frame.Width = Viewport.Width \ 4
        Life_Bar_Frame.Height = 20

        'Set magic bar postion and size.
        Magic_Bar_Frame.X = 10
        Magic_Bar_Frame.Y = 40
        Magic_Bar_Frame.Width = Viewport.Width \ 4
        Magic_Bar_Frame.Height = 20

        'Is the editor off?
        If Editor_On = False Then
            'Yes, the editor is off. The game is running.

            Do_Hero_Moves()

            Do_Monster_Moves()

            Reveal_Walls()

        End If

    End Sub

    Private Sub Reveal_Walls()

        'Do we have at least one wall?
        If Walls IsNot Nothing Then
            'Yes, we have at least one wall.

            'Go thru walls one by one, start to end.
            For index = 0 To UBound(Walls)

                'Has the player seen the wall?
                If Walls(index).Rec.IntersectsWith(Viewport) Then
                    'Yes, the player has seen the wall.

                    'Mark wall as revealed.
                    If Walls(index).Revealed <> True Then
                        Walls(index).Revealed = True
                    End If

                End If
            Next
        End If

    End Sub

    Private Sub Do_Hero_Moves()

        'Is the hero alive?
        If OurHero.Life > 0 Then
            'Yes, the hero is alive.

            'Roll for initative.************************
            If RandomNumber.Next(1, 20) > 9 Then
                OurHero.Initiative = True
            Else
                OurHero.Initiative = False
            End If
            '*******************************************
            If MoveLeft = True Then

                Move_Hero_Left()

            End If

            If MoveRight = True Then

                Move_Hero_Right()

            End If

            If MoveUp = True Then

                Move_Hero_Up()

            End If

            If MoveDown = True Then

                Move_Hero_Down()

            End If

            Do_Hero_Shots()

            Do_Potion_Pickup()

        Else
            'No, the hero is dead.

        End If

    End Sub

    Private Sub Do_Potion_Pickup()

        If Potion.Active = True Then

            If OurHero.Rec.IntersectsWith(Potion.Rec) = True Then

                If Potion.IsLife = True Then

                    If OurHero.Life <> OurHero.MaxLife Then

                        'Play potion pickupsound.
                        If GS.IsPlaying("Potion_Pickup") = False Then
                            GS.Play("Potion_Pickup")
                        End If

                        OurHero.Life = OurHero.MaxLife
                        Potion.Active = False

                    End If

                Else

                    If OurHero.Magic <> OurHero.MaxMagic Then

                        'Play potion pickupsound.
                        If GS.IsPlaying("Potion_Pickup") = False Then
                            GS.Play("Potion_Pickup")
                        End If

                        OurHero.Magic = OurHero.MaxMagic
                        Potion.Active = False

                    End If
                End If
            End If
        End If

    End Sub

    Private Sub Move_Hero_Right()

        MoveHero(DirectionEnum.Right)

        'Play hero moving sound.
        If GS.IsPlaying("Hero_Move") = False Then
            GS.Play("Hero_Move")
        End If

        'Wall Collision Handler - Moving Right*****************
        If Walls IsNot Nothing Then
            For index = 0 To UBound(Walls)
                'Is the hero touching the wall?
                If OurHero.Rec.IntersectsWith(Walls(index).Rec) = True Then
                    'Yes, the hero is touching the wall.

                    'Push hero to the left of wall.
                    OurHero.Rec.X = Walls(index).Rec.X - OurHero.Rec.Width - 1

                End If
            Next
        End If
        '*****************************************************************

        'Is the monster alive?
        If Monster_Life > 0 Then
            'Yes, the monster is alive.

            'Hero Attacks Right *******************************************
            'Is the hero touching the monster?
            If OurHero.Rec.IntersectsWith(Monster) = True Then
                'Yes, the hero is touching the monster.

                'Push hero to the left of monster.
                OurHero.Rec.X = Monster.X - OurHero.Rec.Width - 1

                If OurHero.Initiative = True Then

                    'Play hero attack sound.

                    Monster_Hit = True

                    'Play monster hit sound.
                    If GS.IsPlaying("Undead_Hit") = False Then
                        GS.Play("Undead_Hit")
                    End If


                    'Attack the monsters life points directly.
                    Monster_Life -= OurHero.Attack
                    If Monster_Life < 0 Then
                        Monster_Life = 0
                    End If

                    'Knock monster to the right of hero.
                    Monster.X = OurHero.Rec.X + OurHero.Rec.Width + 32


                    'Wall Collision Handler - Moving Right*****************
                    If Walls IsNot Nothing Then
                        For index = 0 To UBound(Walls)
                            'Is the monster touching a wall?
                            If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                'Yes, the monster is touching a wall.

                                'Knock monster to the left of wall.
                                Monster.X = Walls(index).Rec.X - Monster.Width - 1

                                'Knock the hero to the left of the monster.
                                OurHero.Rec.X = Monster.X - Monster.Width - 16

                            End If
                        Next
                    End If
                    '************************************************
                End If
            End If
        End If

        'Fallow the hero.
        'Is the hero about to walk off screen?
        If OurHero.Rec.X > Viewport.X + Viewport.Width - OurHero.Rec.Width * 4 Then
            'Yes, the hero is about to walk off screen.


            If Viewport.X < Level.Rec.Width - Viewport.Width Then
                'Move viewport to the right.
                Viewport.X += OurHero.Speed
            Else

                Viewport.X = Level.Rec.Width - Viewport.Width

            End If

            If OurHero.Rec.X > Level.Rec.Width - OurHero.Rec.Width Then

                OurHero.Rec.X = Level.Rec.Width - OurHero.Rec.Width

            End If
        End If

    End Sub

    Private Sub Move_Monster_Right()


        Dim Monster_Center_X As Integer = Monster.X + Monster.Width \ 2
        Dim Hero_Center_X As Integer = OurHero.Rec.X + OurHero.Rec.Width \ 2


        'Move Monster Right **************************************************************
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

        'Play undead moving sound.
        If GS.IsPlaying("Undead_Move") = False Then
            GS.Play("Undead_Move")
        End If
        '*********************************************************************************

        'Wall Collision Handler - Moving Right*****************
        If Walls IsNot Nothing Then
            For index = 0 To UBound(Walls)

                'Is the monster touching the wall?
                If Monster.IntersectsWith(Walls(index).Rec) = True Then
                    'Yes, the monster is touching the wall.

                    'Push monster to the left of wall.
                    Monster.X = Walls(index).Rec.X - Monster.Width - 1

                End If
            Next
        End If
        '************************************************

        'Attack Right**************************************************
        'Is the monster touching the hero?
        If Monster.IntersectsWith(OurHero.Rec) = True Then
            'Yes, the monster is touching the hero.

            'Push the monster to the left of the hero.
            Monster.X = OurHero.Rec.X - Monster.Width - 1

            'Roll for initative.************************
            If RandomNumber.Next(1, 20) > 15 Then 'Chance to hit is 1 in 20
                Monster_Initiative = True
            Else
                Monster_Initiative = False
            End If
            '*******************************************

            If Monster_Initiative = True Then

                'Play undead attack sound.
                If GS.IsPlaying("Undead_Attack") = False Then
                    GS.Play("Undead_Attack") 'play the Music
                End If

                'Reset the hit total on the first hit.
                If OurHero.Hit = False Then
                    OurHero.LifeBeforeHit = OurHero.Life
                End If

                'Attack the heros life points directly.
                OurHero.Life -= Monster_Attack

                If OurHero.Life < 1 Then

                    'Play hero death sound.
                    If GS.IsPlaying("Hero_Death") = False Then
                        GS.Play("Hero_Death")
                    End If

                    OurHero.Life = 0

                End If

                OurHero.Hit = True

                'Knock hero to the right of monster.
                OurHero.Rec.X = Monster.X + Monster.Width + 32


                'Wall Collision Handler - Moving Right *******************************
                If Walls IsNot Nothing Then
                    For index = 0 To UBound(Walls)

                        'Is the hero touching a wall?
                        If OurHero.Rec.IntersectsWith(Walls(index).Rec) = True Then
                            'Yes, the hero is touching a wall.

                            'Knock hero to the left of wall.
                            OurHero.Rec.X = Walls(index).Rec.X - OurHero.Rec.Width - 1

                            'Knock the monster to the left of the hero.
                            Monster.X = OurHero.Rec.X - Monster.Width - 16

                        End If
                    Next
                End If
                '*********************************************************************

            End If
        End If

    End Sub

    Private Sub Move_Hero_Left()

        MoveHero(DirectionEnum.Left)

        'Play hero moving sound.
        If GS.IsPlaying("Hero_Move") = False Then
            GS.Play("Hero_Move")
        End If

        'Wall Collision Handler Hero moving left*************************
        If Walls IsNot Nothing Then
            For index = 0 To UBound(Walls)
                'Is the hero touching the wall?
                If OurHero.Rec.IntersectsWith(Walls(index).Rec) = True Then
                    'Yes, the hero is touching the wall.

                    'Push hero to the right of wall.
                    OurHero.Rec.X = Walls(index).Rec.X + Walls(index).Rec.Width

                End If
            Next
        End If
        '*****************************************************************

        'Is the monster alive?
        If Monster_Life > 0 Then
            'Yes, the monster is alive.

            'Hero Attacks Left *******************************************
            'Is the hero touching the monster?
            If OurHero.Rec.IntersectsWith(Monster) = True Then
                'Yes, the hero is touching the monster.

                'Push hero to the right of monster.
                OurHero.Rec.X = Monster.X + Monster.Width + 1

                If OurHero.Initiative = True Then

                    'Play hero attack sound.

                    Monster_Hit = True

                    'Play monster hit sound.
                    If GS.IsPlaying("Undead_Hit") = False Then
                        GS.Play("Undead_Hit")
                    End If

                    'Attack the monsters life points directly.
                    Monster_Life -= OurHero.Attack
                    If Monster_Life < 0 Then
                        Monster_Life = 0
                    End If

                    'Knock monster to the left of hero.
                    Monster.X = OurHero.Rec.X - Monster.Width - 32

                    'Wall Collision Handler Monster moving left*************************
                    If Walls IsNot Nothing Then
                        For index = 0 To UBound(Walls)
                            'Is the monster touching the wall?
                            If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                'Yes, the monster is touching the wall.

                                'Push monster to the right of wall.
                                Monster.X = Walls(index).Rec.X + Walls(index).Rec.Width + 1

                                'Knock the hero to the right of the monster.
                                OurHero.Rec.X = Monster.X + Monster.Width + 16

                            End If
                        Next
                    End If
                    '************************************************

                End If
            End If
        End If

        'Fallow the hero.
        'Is the hero about to walk off screen?
        If OurHero.Rec.X < Viewport.X + OurHero.Rec.Width * 4 Then
            'Yes, the hero is about to walk off screen.

            If Viewport.X > Level.Rec.X Then

                'Move viewport to the right.
                Viewport.X -= OurHero.Speed

            Else

                Viewport.X = Level.Rec.X

            End If

            If OurHero.Rec.X < Level.Rec.X Then

                OurHero.Rec.X = Level.Rec.X

            End If

        End If

    End Sub

    Private Sub Move_Monster_Left()

        Dim Monster_Center_X As Integer = Monster.X + Monster.Width \ 2
        Dim Hero_Center_X As Integer = OurHero.Rec.X + OurHero.Rec.Width \ 2

        'Move Left************************************************************************************
        'Move the monster to the left.
        'Proximity based speed controller. - Fixes Bug: The monster sometimes oscillates.**************
        'Is the monster close to the hero?
        If Horizontal_Distance(Monster_Center_X, Hero_Center_X) > 8 Then
            'No, the monster is not close to the hero.

            'Move the monster to the left fast.
            Monster.X -= Monster_Speed

        Else
            'Yes, the monster is close to the hero.

            'Move the monster to the left slowly.
            Monster.X -= 1

        End If

        'Play undead moving sound.
        If GS.IsPlaying("Undead_Move") = False Then
            GS.Play("Undead_Move")
        End If
        '***********************************************************************************************

        'Wall Collision Handler Monster moving left*************************
        If Walls IsNot Nothing Then
            For index = 0 To UBound(Walls)

                'Is the monster touching the wall?
                If Monster.IntersectsWith(Walls(index).Rec) = True Then
                    'Yes, the monster is touching the wall.

                    'Push monster to the right of wall.
                    Monster.X = Walls(index).Rec.X + Walls(index).Rec.Width

                End If
            Next
        End If
        '************************************************

        'Attack Left************************************************************************************
        'Is the monster touching the hero?
        If Monster.IntersectsWith(OurHero.Rec) = True Then
            'Yes, the monster is touching the hero.

            'Push the monster to the right of the hero.
            Monster.X = OurHero.Rec.X + OurHero.Rec.Width + 1

            'Roll for initative.************************
            If RandomNumber.Next(1, 20) > 15 Then 'Chance to hit is 1 in 20
                Monster_Initiative = True
            Else
                Monster_Initiative = False
            End If
            '*******************************************

            If Monster_Initiative = True Then

                'Play undead attack sound.
                If GS.IsPlaying("Undead_Attack") = False Then
                    GS.Play("Undead_Attack") 'play the Music
                End If

                'Reset the hit total on the first hit.
                If OurHero.Hit = False Then
                    OurHero.LifeBeforeHit = OurHero.Life
                End If

                'Attack the heros life points directly.
                OurHero.Life -= Monster_Attack

                If OurHero.Life < 1 Then

                    'Play hero death sound.
                    If GS.IsPlaying("Hero_Death") = False Then
                        GS.Play("Hero_Death")
                    End If

                    OurHero.Life = 0

                End If

                OurHero.Hit = True

                'Knock hero to the left of monster.
                OurHero.Rec.X = Monster.X - OurHero.Rec.Width - 32

                'Wall Collision Handler - Moving Left *******************************
                If Walls IsNot Nothing Then
                    For index = 0 To UBound(Walls)

                        'Is the hero touching a wall?
                        If OurHero.Rec.IntersectsWith(Walls(index).Rec) = True Then
                            'Yes, the hero is touching a wall.

                            'Knock hero to the right of wall.
                            OurHero.Rec.X = Walls(index).Rec.X + Walls(index).Rec.Width + 1

                            'Knock the monster to the right of the hero.
                            Monster.X = OurHero.Rec.X + OurHero.Rec.Width + 16

                        End If
                    Next
                End If
                '*********************************************************************

            End If
        End If

    End Sub

    Private Sub Move_Hero_Up()

        MoveHero(DirectionEnum.Up)

        'Play hero moving sound.
        If GS.IsPlaying("Hero_Move") = False Then
            GS.Play("Hero_Move")
        End If

        'Wall Collision Handler Hero moving up*************************
        If Walls IsNot Nothing Then
            For index = 0 To UBound(Walls)
                'Is the hero touching the wall?
                If OurHero.Rec.IntersectsWith(Walls(index).Rec) = True Then
                    'Yes, the hero is touching the wall.

                    'Push the hero below the wall.
                    OurHero.Rec.Y = Walls(index).Rec.Y + Walls(index).Rec.Height

                End If
            Next
        End If
        '*****************************************************************

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

                    'Wall Collision Handler - Monster moving up *************************
                    If Walls IsNot Nothing Then
                        For index = 0 To UBound(Walls)
                            'Is the monster touching the wall?
                            If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                'Yes, the monster is touching the wall.

                                'Push the hero below the wall.
                                Monster.Y = Walls(index).Rec.Y + (Walls(index).Rec.Height - 1)

                                'If GS.IsPlaying("Undead_Move") = True Then
                                '    GS.Pause("Undead_Move")
                                'End If

                            End If
                        Next
                    End If
                    '************************************************

                End If
            End If
        End If

        'Fallow the hero.
        'Is the hero about to walk off screen?
        If OurHero.Rec.Y < Viewport.Y + OurHero.Rec.Height * 4 Then
            'Yes, the hero is about to walk off screen.

            If Viewport.Y > Level.Rec.Y Then

                'Move viewport to the up.
                Viewport.Y -= OurHero.Speed

            Else

                Viewport.Y = Level.Rec.Y

            End If

            If OurHero.Rec.Y < Level.Rec.Y Then

                OurHero.Rec.Y = Level.Rec.Y

            End If

        End If
    End Sub

    Private Sub Move_Monster_Up()

        Dim Monster_Center_Y As Integer = Monster.Y + Monster.Height \ 2
        Dim Hero_Center_Y As Integer = OurHero.Rec.Y + OurHero.Rec.Height \ 2

        'Move Monster Up. **************************************************************
        'Proximity based speed controller. - Fixes Bug: The monster sometimes oscillates.
        'Is the monster close to the hero?
        If Vertical_Distance(Monster_Center_Y, Hero_Center_Y) > 8 Then
            'No, the monster is not close to the hero.

            'Move the monster up fast.
            Monster.Y -= Monster_Speed

        Else
            'Yes, the monster is close to the hero.

            'Move the monster up slowly.
            Monster.Y -= 1

        End If

        'Play undead moving sound.
        If GS.IsPlaying("Undead_Move") = False Then
            GS.Play("Undead_Move")
        End If
        '*********************************************************************************

        'Wall Collision Handler - Monster moving up *************************
        If Walls IsNot Nothing Then
            For index = 0 To UBound(Walls)

                'Is the monster touching the wall?
                If Monster.IntersectsWith(Walls(index).Rec) = True Then
                    'Yes, the monster is touching the wall.

                    'Push the hero below the wall.
                    Monster.Y = Walls(index).Rec.Y + Walls(index).Rec.Height

                End If
            Next
        End If
        '************************************************

        'Attack Up************************************************************************************
        If Monster.IntersectsWith(OurHero.Rec) = True Then

            'Push the monster below the hero.
            Monster.Y = OurHero.Rec.Y + OurHero.Rec.Height + 1

            'Roll for initative.************************
            If RandomNumber.Next(1, 20) > 15 Then 'Chance to hit is 1 in 20
                Monster_Initiative = True
            Else
                Monster_Initiative = False
            End If
            '*******************************************

            If Monster_Initiative = True Then

                'Play undead attack sound.
                If GS.IsPlaying("Undead_Attack") = False Then
                    GS.Play("Undead_Attack") 'play the Music
                End If

                'Reset the hit total on the first hit.
                If OurHero.Hit = False Then
                    OurHero.LifeBeforeHit = OurHero.Life
                End If

                'Attack the heros life points directly.
                OurHero.Life -= Monster_Attack

                If OurHero.Life < 1 Then

                    'Play hero death sound.
                    If GS.IsPlaying("Hero_Death") = False Then
                        GS.Play("Hero_Death")
                    End If

                    OurHero.Life = 0

                End If

                OurHero.Hit = True

                'Knock hero above the monster.
                OurHero.Rec.Y = Monster.Y - OurHero.Rec.Height - 32

                'Wall Collision Handler - Moving Up *******************************
                If Walls IsNot Nothing Then
                    For index = 0 To UBound(Walls)

                        'Is the hero touching a wall?
                        If OurHero.Rec.IntersectsWith(Walls(index).Rec) = True Then
                            'Yes, the hero is touching a wall.

                            'Knock hero below the wall.
                            OurHero.Rec.Y = Walls(index).Rec.Y + Walls(index).Rec.Height + 1

                            'Knock the monster below the hero.
                            Monster.Y = OurHero.Rec.Y + OurHero.Rec.Height + 16

                        End If
                    Next
                End If
                '*********************************************************************

            End If
        End If

    End Sub

    Private Sub Move_Hero_Down()

        MoveHero(DirectionEnum.Down)

        'Play hero moving sound.
        If GS.IsPlaying("Hero_Move") = False Then
            GS.Play("Hero_Move")
        End If

        'Wall Collision Handler Hero moving down*************************
        If Walls IsNot Nothing Then
            For index = 0 To UBound(Walls)
                'Is the hero touching the wall?
                If OurHero.Rec.IntersectsWith(Walls(index).Rec) = True Then
                    'Yes, the hero is touching the wall.

                    'Push the hero above the wall.
                    OurHero.Rec.Y = Walls(index).Rec.Y - OurHero.Rec.Height - 1

                End If
            Next
        End If
        '*****************************************************************

        'Is the monster alive?
        If Monster_Life > 0 Then
            'Yes, the monster is alive.

            'Hero Attacks Left *******************************************
            'Is the hero touching the monster?
            If OurHero.Rec.IntersectsWith(Monster) = True Then
                'Yes, the hero is touching the monster.

                'Push hero to the right of monster.
                OurHero.Rec.Y = Monster.Y - OurHero.Rec.Height - 1

                If OurHero.Initiative = True Then

                    'Play hero attack sound.

                    Monster_Hit = True

                    'Play monster hit sound.
                    If GS.IsPlaying("Undead_Hit") = False Then
                        GS.Play("Undead_Hit")
                    End If

                    'Attack the monsters life points directly.
                    Monster_Life -= OurHero.Attack
                    If Monster_Life < 0 Then
                        Monster_Life = 0
                    End If

                    'Knock monster below the hero.
                    Monster.Y = OurHero.Rec.Y + OurHero.Rec.Height + 32

                    'Wall Collision Handler - Moving Down ********************************************************
                    If Walls IsNot Nothing Then
                        For index = 0 To UBound(Walls)
                            'Is the monster touching the wall?
                            If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                'Yes, the monster is touching the wall.

                                'Push the monster above the wall.
                                Monster.Y = Walls(index).Rec.Y - Monster.Height - 1

                                'Knock the hero above the monster.
                                OurHero.Rec.Y = Monster.Y - Monster.Height - 16

                            End If
                        Next
                    End If
                    '************************************************

                End If
            End If
        End If

        'Fallow the hero.
        'Is the hero about to walk off screen?
        If OurHero.Rec.Y > Viewport.Y + Viewport.Height - OurHero.Rec.Height * 4 Then
            'Yes, the hero is about to walk off screen.

            If Viewport.Y < Level.Rec.Height - Viewport.Height Then
                'Move viewport to the right.
                Viewport.Y += OurHero.Speed
            Else

                Viewport.Y = Level.Rec.Height - Viewport.Height

            End If

            If OurHero.Rec.Y > Level.Rec.Height - OurHero.Rec.Height Then

                OurHero.Rec.Y = Level.Rec.Height - OurHero.Rec.Height

            End If

        End If

    End Sub

    Private Sub Move_Monster_Down()

        Dim Monster_Center_Y As Integer = Monster.Y + Monster.Height \ 2
        Dim Hero_Center_Y As Integer = OurHero.Rec.Y + OurHero.Rec.Height \ 2

        'Move Monster Down. **************************************************************
        'Proximity based speed controller. - Fixes Bug: The monster sometimes oscillates.
        'Is the monster close to the hero?
        If Vertical_Distance(Monster_Center_Y, Hero_Center_Y) > 8 Then
            'No, the monster is not close to the hero.

            'Move the monster down fast.
            Monster.Y += Monster_Speed

        Else
            'Yes, the monster is close to the hero.

            'Move the monster down slowly.
            Monster.Y += 1

        End If

        'Play undead moving sound.
        If GS.IsPlaying("Undead_Move") = False Then
            GS.Play("Undead_Move")
        End If
        '***********************************************************************************************

        'Wall Collision Handler - Moving Down ********************************************************
        If Walls IsNot Nothing Then
            For index = 0 To UBound(Walls)

                'Is the monster touching the wall?
                If Monster.IntersectsWith(Walls(index).Rec) = True Then
                    'Yes, the monster is touching the wall.

                    'Push the monster above the wall.
                    Monster.Y = Walls(index).Rec.Y - Monster.Height - 1

                End If
            Next
        End If
        '************************************************

        'Attack Down ************************************************************************************
        If Monster.IntersectsWith(OurHero.Rec) = True Then

            'Push to monster above the hero.
            Monster.Y = OurHero.Rec.Y - Monster.Height - 1

            'Roll for initative.************************
            If RandomNumber.Next(1, 20) > 15 Then 'Chance to hit is 1 in 20
                Monster_Initiative = True
            Else
                Monster_Initiative = False
            End If
            '*******************************************

            If Monster_Initiative = True Then

                'Play undead attack sound.
                If GS.IsPlaying("Undead_Attack") = False Then
                    GS.Play("Undead_Attack") 'play the Music
                End If

                'Reset the hit total on the first hit.
                If OurHero.Hit = False Then
                    OurHero.LifeBeforeHit = OurHero.Life
                End If

                'Attack the heros life points directly.
                OurHero.Life -= Monster_Attack

                If OurHero.Life < 1 Then

                    'Play hero death sound.
                    If GS.IsPlaying("Hero_Death") = False Then
                        GS.Play("Hero_Death")
                    End If

                    OurHero.Life = 0

                End If

                OurHero.Hit = True

                'Knock hero below monster.
                OurHero.Rec.Y = Monster.Y + Monster.Height + 32

                'Wall Collision Handler - Moving Down *******************************
                If Walls IsNot Nothing Then
                    For index = 0 To UBound(Walls)

                        'Is the hero touching a wall?
                        If OurHero.Rec.IntersectsWith(Walls(index).Rec) = True Then
                            'Yes, the hero is touching a wall.

                            'Knock hero above the wall.
                            OurHero.Rec.Y = Walls(index).Rec.Y - OurHero.Rec.Height - 1

                            'Knock the monster above the hero.
                            Monster.Y = OurHero.Rec.Y - Monster.Height - 16

                        End If
                    Next
                End If
                '*********************************************************************

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
            If Distance_Between_Points(Monster.Location, OurHero.Rec.Location) < Viewport.Width \ 3 Then
                'Yes, the monster is near the hero.

                Dim Monster_Center_X As Integer = Monster.X + Monster.Width \ 2
                Dim Hero_Center_X As Integer = OurHero.Rec.X + OurHero.Rec.Width \ 2

                'Is the monster centered with the hero horizontally?
                If Monster_Center_X <> Hero_Center_X Then
                    'No, the monster is not centered horizontally with the hero.

                    'Is the monster to the right of the hero?
                    If Monster_Center_X > Hero_Center_X Then
                        'Yes, the monster is to the right of the hero.

                        Move_Monster_Left()

                    Else
                        'No, the monster is to the left of the hero.

                        Move_Monster_Right()

                    End If
                End If

                'Is the monster centered with the hero vertically?
                If Monster.Y + Monster.Height \ 2 <> OurHero.Rec.Y + OurHero.Rec.Height \ 2 Then
                    'No, the monster is not centered vertically with the hero.

                    'Is the monster below the hero?
                    If Monster.Y + Monster.Height \ 2 > OurHero.Rec.Y + OurHero.Rec.Height \ 2 Then
                        'Yes, the monster is above the hero.

                        Move_Monster_Up()

                    Else
                        'No, the monster is below the hero.

                        Move_Monster_Down()

                    End If
                End If
            End If

        Else
            If Monster_Life = 0 Then

                'Pause undead attack sound.
                If GS.IsPlaying("Undead_Attack") = False Then
                    GS.Pause("Undead_Attack") 'play the Music
                End If

                'Pause undead move sound.
                If GS.IsPlaying("Undead_Move") = True Then
                    GS.Pause("Undead_Move")
                End If

                'Play undead death sound.
                GS.Play("Undead_Death")

                'Loot Drop ************************************************************
                'Roll for loot.
                If RandomNumber.Next(1, 3) < 2 Then 'Chance 50/50
                    'Drop life potion.
                    Potion.IsLife = True
                    Potion.Rec.Size = New Size(85, 85)
                    Potion.Rec.X = Monster.X + (Monster.Width - Potion.Rec.Width) \ 2
                    Potion.Rec.Y = Monster.Y + (Monster.Height - Potion.Rec.Height) \ 2
                    Potion.Active = True

                    Potion.Color = Color.FromArgb(255, 95, 7, 12)
                    Potion.OutlineColor = Color.FromArgb(255, 255, 0, 0)
                Else
                    'Drop magic potion.
                    Potion.IsLife = False
                    Potion.Rec.Size = New Size(85, 85)
                    Potion.Rec.X = Monster.X + (Monster.Width - Potion.Rec.Width) \ 2
                    Potion.Rec.Y = Monster.Y + (Monster.Height - Potion.Rec.Height) \ 2
                    Potion.Active = True

                    Potion.Color = Color.FromArgb(255, 0, 0, 202)
                    Potion.OutlineColor = Color.FromArgb(255, 57, 57, 255)
                End If
                '**********************************************************************

                'Play loot drop sound.

                Monster_Life = -1

            End If

            'Start respawn timer.
            Timer3.Interval = 5000 'Set timer for 5 seconds - 1000 miliseconds = 1 second
            Timer3.Start()
        End If

    End Sub

    Private Sub Do_Hero_Shots()

        'Does the hero have enough magic to cast?
        If OurHero.Magic >= 25 Then
            'Yes, the hero has enough magic to cast.

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

                    'Subtract the cost of magic.
                    OurHero.Magic -= 25
                    If OurHero.Magic < 1 Then
                        OurHero.Magic = 0
                    End If

                    'Play projectile in flight sound.
                    GS.Play("Magic")

                End If
                '******************************************************
            End If
        Else
            'No, the hero doesn't have enough magic to cast.

            'Does the player want to cast a spell?
            If CtrlDown = True Then
                If MoveLeft = True Or MoveRight = True Or MoveUp = True Or MoveDown = True Then
                    'Yes, the player wants to cast a spell.

                    'Play not enough magic dialogue.
                    If GS.IsPlaying("Not_Enough_Magic") = False Then
                        GS.Play("Not_Enough_Magic")
                    End If
                End If
            End If

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
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push monster to the left of wall.
                                            Monster.X = Walls(index).Rec.X - Monster.Width - 1

                                        End If
                                    Next
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

                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock monster to the left of hero.
                                Monster.X -= CInt(Projectile_Speed / 3)

                                'Wall Collision Handler Monster moving left*************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push monster to the right of wall.
                                            Monster.X = Walls(index).Rec.X + Walls(index).Rec.Width

                                        End If
                                    Next
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

                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock monster above the hero.
                                Monster.Y -= CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Monster moving up *************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push the hero below the wall.
                                            Monster.Y = Walls(index).Rec.Y + Walls(index).Rec.Height

                                        End If

                                    Next
                                End If
                                '************************************************

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

                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock monster below hero.
                                Monster.Y += CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Moving Down ********************************************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push the monster above the wall.
                                            Monster.Y = Walls(index).Rec.Y - Monster.Height - 1

                                        End If
                                    Next
                                End If
                                '************************************************

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
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock monster to the right of hero.
                                Monster.X += CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Moving Right*****************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push monster to the left of wall.
                                            Monster.X = Walls(index).Rec.X - Monster.Width - 1

                                        End If
                                    Next
                                End If
                                '************************************************

                                'Knock monster above hero.
                                Monster.Y -= CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Monster moving up *************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push the hero below the wall.
                                            Monster.Y = Walls(index).Rec.Y + Walls(index).Rec.Height

                                        End If
                                    Next
                                End If
                                '************************************************

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

                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock monster to the right of hero.
                                Monster.X += CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Moving Right*****************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push monster to the left of wall.
                                            Monster.X = Walls(index).Rec.X - Monster.Width - 1

                                        End If
                                    Next
                                End If
                                '************************************************

                                'Knock monster below hero.
                                Monster.Y += CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Moving Down ********************************************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push the monster above the wall.
                                            Monster.Y = Walls(index).Rec.Y - Monster.Height - 1

                                        End If
                                    Next
                                End If
                                '************************************************

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

                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock monster to the left of hero.
                                Monster.X -= CInt(Projectile_Speed / 3)

                                'Wall Collision Handler Monster moving left*************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push monster to the right of wall.
                                            Monster.X = Walls(index).Rec.X + Walls(index).Rec.Width

                                        End If
                                    Next
                                End If
                                '************************************************

                                'Knock monster above the hero.
                                Monster.Y -= CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Monster moving up *************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push the hero below the wall.
                                            Monster.Y = Walls(index).Rec.Y + Walls(index).Rec.Height

                                        End If
                                    Next
                                End If
                                '************************************************

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

                                'Knock the monster to the left of hero.
                                Monster.X -= CInt(Projectile_Speed / 3)

                                'Wall Collision Handler Monster moving left*************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push monster to the right of wall.
                                            Monster.X = Walls(index).Rec.X + Walls(index).Rec.Width

                                        End If
                                    Next
                                End If
                                '************************************************

                                'Knock the monster below the hero.
                                Monster.Y += CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Moving Down ********************************************************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push the monster above the wall.
                                            Monster.Y = Walls(index).Rec.Y - Monster.Height - 1

                                        End If
                                    Next
                                End If
                                '************************************************

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

            OurHero.Hit = False
            OurHero.HitTimer = 0
            OurHero.LifeBeforeHit = OurHero.Life

        End If

        Timer3.Stop()

    End Sub
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyValue = Keys.Left Then
            If Editor_On = False Then
                MoveLeft = True
            Else
                MoveViewport(DirectionEnum.Left)
            End If
        End If

        If e.KeyValue = Keys.Right Then
            If Editor_On = False Then
                MoveRight = True
            Else
                MoveViewport(DirectionEnum.Right)
            End If
        End If

        If e.KeyValue = Keys.Up Then
            If Editor_On = False Then
                MoveUp = True
            Else
                MoveViewport(DirectionEnum.Up)
            End If
        End If

        If e.KeyValue = Keys.Down Then
            If Editor_On = False Then
                MoveDown = True
            Else
                MoveViewport(DirectionEnum.Down)
            End If
        End If

        Select Case e.KeyValue

            Case Keys.ControlKey

                CtrlDown = True

            Case Keys.P

                'Toggle game on or off.
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

            Case Keys.Clear

                CtrlDown = True

            Case Keys.M

                'Toggle map on or off.
                If Map_On = False Then
                    Map_On = True
                Else
                    Map_On = False
                End If

            Case Keys.I

                'Toggle instructions on or off.
                If Instructions_On = False Then
                    Instructions_On = True

                Else
                    Instructions_On = False

                End If

            Case Keys.Delete

                If Editor_On = True Then
                    If IsSelected = True Then

                        Remove_Wall(Selected_Index)

                        IsSelected = False

                    End If
                End If

        End Select

    End Sub

    Private Sub MoveViewport(direction As DirectionEnum)

        If direction = DirectionEnum.Left Then
            Viewport.X -= 10
        End If
        If direction = DirectionEnum.Right Then
            Viewport.X += 10
        End If
        If direction = DirectionEnum.Up Then
            Viewport.Y -= 10
        End If
        If direction = DirectionEnum.Down Then
            Viewport.Y += 10
        End If

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

            Case Keys.Clear

                CtrlDown = False

        End Select

    End Sub
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        'viewport
        Viewport.Width = PictureBox1.Width
        Viewport.Height = PictureBox1.Height

        Life_Bar_Frame.X = 10
        Life_Bar_Frame.Y = 10
        Life_Bar_Frame.Width = Viewport.Width \ 4
        Life_Bar_Frame.Height = 20

        Magic_Bar_Frame.X = 10
        Magic_Bar_Frame.Y = 40
        Magic_Bar_Frame.Width = Viewport.Width \ 4
        Magic_Bar_Frame.Height = 20

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

        CtrlDown = False

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

    Private Sub GS_SoundEnded(ByVal SndName As String)

        If GS.IsPlaying("Music") = False Then

            GS.Play("Music")

        End If

    End Sub

    Private Sub MenuItemEditorOn_Click(sender As Object, e As EventArgs) Handles MenuItemEditorOn.Click

        'Toggle Editor On/Off *****************************************
        'Is the editor off?
        If Editor_On = False Then
            'Yes, the editor is off. The game is runnning.

            'Turn editor on.
            Editor_On = True

            'Check the editor in the menu.
            MenuItemEditorOn.Checked = True

        Else
            'No, the editor is on. The game is stopped.

            'Turn editor off.
            Editor_On = False

            'Uncheck the editor in the menu.
            MenuItemEditorOn.Checked = False

            'Center viewport on the hero.
            Viewport.X = OurHero.Rec.X - Viewport.Width \ 2
            Viewport.Y = OurHero.Rec.Y - Viewport.Height \ 2

            'Keep viewport on the level.
            If Viewport.X < Level.Rec.X Then
                Viewport.X = Level.Rec.X
            End If
            If Viewport.X + Viewport.Width > Level.Rec.Width Then
                Viewport.X = Level.Rec.Width - Viewport.Width
            End If
            If Viewport.Y < Level.Rec.Y Then
                Viewport.Y = Level.Rec.Y
            End If
            If Viewport.Y + Viewport.Height > Level.Rec.Height Then
                Viewport.Y = Level.Rec.Height - Viewport.Height
            End If

        End If

    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown

        If Editor_On = True Then

            'Is the pointer the selected tool?
            If Selected_Tool = ToolsEnum.Pointer Then
                'Yes, the pointer is the selected tool.

                'Has a wall been selected?
                If IsSelected <> True Then
                    'No wall is selected.

                    'Set pointer origin to the current mouse location.
                    Pointer_Origin = e.Location

                    'Is there at least one wall?
                    If Walls IsNot Nothing Then
                        'Yes, there is at least one wall.
                        Dim MousePointerInViewPortCooridinates As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)
                        'Go thru every wall in walls in reverse z order.
                        For Index = UBound(Walls) To 0 Step -1
                            'Was a wall selected?
                            If MousePointerInViewPortCooridinates.IntersectsWith(Walls(Index).Rec) Then
                                'Yes, a wall was selected.

                                Selected_Index = Index
                                IsSelected = True
                                Pointer_Offset.X = Pointer_Origin.X - Walls(Selected_Index).Rec.X
                                Pointer_Offset.Y = Pointer_Origin.Y - Walls(Selected_Index).Rec.Y
                                Exit For

                            End If
                            IsSelected = False
                            Selected_Index = -1
                        Next
                    End If

                Else
                    'Yes, a wall is selected.

                    'Is the mouse pointer selecting a control handle?
                    Dim MousePointerRec As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)

                    Dim TopLeftControlHandleRec As New Rectangle(Walls(Selected_Index).Rec.X - 5, Walls(Selected_Index).Rec.Y - 5, 10, 10)

                    Dim BottomRightControlHandleRec As New Rectangle(Walls(Selected_Index).Rec.Right - 5, Walls(Selected_Index).Rec.Bottom - 5, 10, 10)

                    If MousePointerRec.IntersectsWith(TopLeftControlHandleRec) = True Then

                        TopLeftControlHandle_Selected = True

                        Wall_Origin.X = Walls(Selected_Index).Rec.Right
                        Wall_Origin.Y = Walls(Selected_Index).Rec.Bottom

                    Else

                        TopLeftControlHandle_Selected = False

                    End If

                    If MousePointerRec.IntersectsWith(BottomRightControlHandleRec) = True Then

                        BottomRightControlHandle_Selected = True

                        Wall_Origin.X = Walls(Selected_Index).Rec.X
                        Wall_Origin.Y = Walls(Selected_Index).Rec.Y

                    Else

                        BottomRightControlHandle_Selected = False

                    End If

                    If TopLeftControlHandle_Selected = False And BottomRightControlHandle_Selected = False Then

                        'Set pointer origin to the current mouse location.
                        Pointer_Origin = e.Location

                        'Is there at least one wall?
                        If Walls IsNot Nothing Then
                            'Yes, there is at least one wall.
                            Dim MousePointerInViewPortCooridinates As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)
                            'Go thru every wall in walls in reverse z order.
                            For Index = UBound(Walls) To 0 Step -1
                                'Was a wall selected?
                                If MousePointerInViewPortCooridinates.IntersectsWith(Walls(Index).Rec) Then
                                    'Yes, a wall was selected.

                                    Selected_Index = Index
                                    IsSelected = True
                                    Pointer_Offset.X = Pointer_Origin.X - Walls(Selected_Index).Rec.X
                                    Pointer_Offset.Y = Pointer_Origin.Y - Walls(Selected_Index).Rec.Y
                                    Exit For

                                End If
                                IsSelected = False
                                Selected_Index = -1
                            Next
                        End If

                    End If

                End If
            End If

            If Selected_Tool = ToolsEnum.Wall Then

                IsSelected = False
                Selected_Index = -1

                'Define a wall.
                Wall.Rec.Width = 0
                Wall.Rec.Height = 0

                'Wall_Origin = e.Location
                Wall_Origin.X = e.X + Viewport.X
                Wall_Origin.Y = e.Y + Viewport.Y

            End If

            Mouse_Down = True

        End If

    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp

        If Editor_On = True Then

            Mouse_Down = False

            If Selected_Tool = ToolsEnum.Wall Then
                'Has the mouse moved?
                If Wall.Rec.Width > 10 And Wall.Rec.Height > 10 Then
                    'Yes, the mouse moved.

                    Add_Wall(New System.Drawing.Rectangle(Wall.Rec.Location, Wall.Rec.Size))

                    If MenuItemWall.Checked = True Then
                        MenuItemWall.Checked = False
                    End If

                    Selected_Tool = ToolsEnum.Pointer

                    MenuItemPointer.Checked = True

                End If
            End If

        End If

    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove

        If Editor_On = True Then

            If Mouse_Down = True Then
                If Selected_Tool = ToolsEnum.Pointer Then
                    If Walls IsNot Nothing Then
                        If Selected_Index > -1 Then
                            If TopLeftControlHandle_Selected = False And BottomRightControlHandle_Selected = False Then

                                'Move the wall.
                                Walls(Selected_Index).Rec.X = e.X - Pointer_Offset.X
                                Walls(Selected_Index).Rec.Y = e.Y - Pointer_Offset.Y

                            ElseIf TopLeftControlHandle_Selected = True Then

                                'Move point
                                'Which point is the top?
                                If Wall_Origin.Y > e.Y + Viewport.Y Then
                                    Walls(Selected_Index).Rec.Y = e.Y + Viewport.Y
                                Else
                                    Walls(Selected_Index).Rec.Y = Wall_Origin.Y
                                End If

                                'Which point is the left?
                                If Wall_Origin.X > e.X + Viewport.X Then
                                    Walls(Selected_Index).Rec.X = e.X + Viewport.X
                                Else
                                    Walls(Selected_Index).Rec.X = Wall_Origin.X
                                End If

                                Walls(Selected_Index).Rec.Width = Abs(Wall_Origin.X - (e.X + Viewport.X))

                                Walls(Selected_Index).Rec.Height = Abs(Wall_Origin.Y - (e.Y + Viewport.Y))

                            ElseIf BottomRightControlHandle_Selected = True Then

                                'Which point is the top?
                                If Wall_Origin.Y > e.Y + Viewport.Y Then
                                    Walls(Selected_Index).Rec.Y = e.Y + Viewport.Y
                                Else
                                    Walls(Selected_Index).Rec.Y = Wall_Origin.Y
                                End If

                                'Which point is the left?
                                If Wall_Origin.X > e.X + Viewport.X Then
                                    Walls(Selected_Index).Rec.X = e.X + Viewport.X
                                Else
                                    Walls(Selected_Index).Rec.X = Wall_Origin.X
                                End If

                                Walls(Selected_Index).Rec.Width = Abs(Wall_Origin.X - (e.X + Viewport.X))

                                Walls(Selected_Index).Rec.Height = Abs(Wall_Origin.Y - (e.Y + Viewport.Y))

                            End If

                        End If
                    End If
                End If

                If Selected_Tool = ToolsEnum.Wall Then

                    'Which point is the top?
                    If Wall_Origin.Y > e.Y + Viewport.Y Then
                        Wall.Rec.Y = e.Y + Viewport.Y
                    Else
                        Wall.Rec.Y = Wall_Origin.Y
                    End If

                    'Which point is the left?
                    If Wall_Origin.X > e.X + Viewport.X Then
                        Wall.Rec.X = e.X + Viewport.X
                    Else
                        Wall.Rec.X = Wall_Origin.X
                    End If

                    Wall.Rec.Width = Abs(Wall_Origin.X - (e.X + Viewport.X))

                    Wall.Rec.Height = Abs(Wall_Origin.Y - (e.Y + Viewport.Y))

                End If

            End If
        End If

    End Sub

    Private Sub Add_Wall(ByVal Wall As Rectangle)

        If Walls IsNot Nothing Then
            Array.Resize(Walls, Walls.Length + 1)
            Walls(Walls.Length - 1).Rec = Wall
        Else
            ReDim Walls(0)
            Walls(0).Rec = Wall
        End If

    End Sub

    Private Sub Remove_Wall(IndexToRemove As Integer)

        'Is this the last wall?
        If UBound(Walls) > 0 Then 'Arrays start at zero.
            'No, this is not the last wall.

            'Create temporary walls array. Set to the size of the walls array minus one wall.
            Dim TempWalls(UBound(Walls) - 1) As WallInfo

            'Create temporary walls array index. Set the temporary array index to the first wall.
            Dim TempIndex As Integer = LBound(Walls)

            'Copy the array without the element to the temporary array.
            'Go thur the walls array, one by one, start to end.
            For index = LBound(Walls) To UBound(Walls)

                'Is the current wall selected for removal.
                If index <> IndexToRemove Then
                    'No, the current wall is not selected for removal.

                    'Copy the current wall from walls to the temporary walls array.
                    TempWalls(TempIndex) = Walls(index)

                    'Advance the temporary index.
                    TempIndex += 1

                End If
            Next

            'Resize the walls array to match the temporary walls array size.
            ReDim Walls(UBound(TempWalls))

            'Copy the temporary walls array back to the walls array.
            Walls = TempWalls

        Else
            'Yes, this is the last wall.

            Walls = Nothing 'Reset walls to default value.

        End If

    End Sub

    Private Sub MenuItemShowHideRulers_Click(sender As Object, e As EventArgs) Handles MenuItemShowHideRulers.Click

        If Show_Rulers = True Then
            Show_Rulers = False
            MenuItemShowHideRulers.Checked = False
        Else
            Show_Rulers = True
            MenuItemShowHideRulers.Checked = True
        End If

    End Sub

    Private Sub MenuItemPointer_Click(sender As Object, e As EventArgs) Handles MenuItemPointer.Click

        Selected_Tool = ToolsEnum.Pointer
        MenuItemPointer.Checked = True

        If MenuItemWall.Checked = True Then
            MenuItemWall.Checked = False
        End If

    End Sub

    Private Sub MenuItemWall_Click(sender As Object, e As EventArgs) Handles MenuItemWall.Click

        Selected_Tool = ToolsEnum.Wall
        MenuItemWall.Checked = True

        If MenuItemPointer.Checked = True Then
            MenuItemPointer.Checked = False
        End If

    End Sub

End Class

Public Enum MCI_NOTIFY As Integer
    SUCCESSFUL = &H1
    SUPERSEDED = &H2
    ABORTED = &H4
    FAILURE = &H8
End Enum

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
