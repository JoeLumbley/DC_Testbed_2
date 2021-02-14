Option Strict On

'Dungeon Crawl
'A work In progress...
'This Is a simple action role-playing game In which the hero navigates a labyrinth,
'battles various monsters, avoids traps, solves puzzles, And loots any treasure that Is found.
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
    'Public Text As String


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
Public Enum MCI_NOTIFY As Integer
    SUCCESSFUL = &H1
    SUPERSEDED = &H2
    ABORTED = &H4
    FAILURE = &H8
End Enum
Public Class Form1

    Private Editor_On As Boolean = False
    Private Show_Rulers As Boolean = True
    Private Selected_Tool As ToolsEnum = ToolsEnum.Pointer





    Private Selected_Index As Integer = 0
    Private IsSelected As Boolean = False
    Private Selected_Pen As New Pen(Color.Blue, 2)



    Private Level As LevelInfo

    Private OurHero As HeroInfo

    Private OurMonster As MonsterInfo

    Private Wall As WallInfo
    Private Wall_Origin As Point
    'Private Walls() As Rectangle
    Private Walls() As WallInfo




    Private Potion As PotionInfo

    Private Viewport As New Rectangle(0, 0, 640, 480)


    Private Map As Rectangle

    'Private Viewport_Size As New Drawing.Size(640, 480)
    Private _BufferFlag As Boolean = True






    Private Monster As New Rectangle(500, 500, 90, 90)
    'Private Monster_Brush As New SolidBrush(Color.FromArgb(255, 39, 205, 89))
    'Private Monster_Brush As New SolidBrush(Color.FromArgb(255, 7, 49, 19))
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
    'Private Life_Brush As New SolidBrush(Color.FromArgb(255, 170, 0, 0))

    'Private Life_Outline As New SolidBrush(Color.FromArgb(255, 255, 0, 0))

    Private Life_Outline_Pen As New Pen(Color.FromArgb(255, 255, 0, 0), 1)

    Private Life_Frame_Brush As New SolidBrush(Color.FromArgb(255, 83, 6, 11))



    Private Life_Blink_Brush As New SolidBrush(Color.FromArgb(255, 170, 0, 0))
    'Private Life_Blink_Brush As New SolidBrush(Color.FromArgb(255, 113, 9, 14))

    Dim Life_Blink_Counter As Integer = 0



    Private Life_Bar_Frame As Rectangle
    Private Life_Bar_Font As New Font("Arial", 15)









    Private Magic_Brush As New SolidBrush(Color.FromArgb(255, 0, 0, 145))
    Private Magic_Blink_Brush As New SolidBrush(Color.FromArgb(255, 0, 0, 227))


    Dim Magic_Blink_Counter As Integer = 0



    'Private Magic_Outline_Pen As New Pen(Color.FromArgb(255, 0, 0, 255), 1)




    Private Magic_Outline_Pen As New Pen(Color.FromArgb(255, 0, 128, 255), 1)





    'Private Magic_Blink_Brush As New SolidBrush(Color.FromArgb(255, 170, 0, 0))



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

    'Dim music As String = Application.StartupPath & "level_music.wav" ' *.wav file location
    'Dim media As New Media.SoundPlayer(music)

    'Dim sound As String = Application.StartupPath & "machine_gun.wav" ' *.wav file location
    'Dim media2 As New Media.SoundPlayer(sound)

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'Private Level_Background_Color As Color = Color.FromArgb(255, 1, 13, 0)
        Level.BackgroundColor = Color.FromArgb(255, 20, 20, 20)
        Level.Rec.X = 0
        Level.Rec.Y = 0
        Level.Rec.Width = 5300
        Level.Rec.Height = 5300

        MenuItemShowHideRulers.Checked = True



        OurHero.Rec = New Rectangle(0, 0, 90, 90)
        'OurHero.Color = Color.FromArgb(255, 11, 182, 255)
        'OurHero.Color = Color.FromArgb(255, 201, 195, 184)
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
        'Wall.Color = Color.FromArgb(255, 164, 164, 164)
        'Wall.Color = Color.FromArgb(255, 11, 64, 26)


        Wall.Color = Color.FromArgb(255, 72, 82, 67)



        'Wall.Color = Color.SlateGray

        'Wall.OutlineColor = Color.FromArgb(255, 0, 128, 0)



        Wall.OutlineColor = Color.FromArgb(255, 195, 195, 195)





        Wall.MapColor = Color.FromArgb(128, 164, 164, 164)





        ''Init Life Potion
        'Potion.IsLife = True
        'Potion.Rec = New Rectangle(0, 0, 200, 200)
        'Potion.Active = False
        'Potion.Color = Color.FromArgb(255, 95, 7, 12)
        'Potion.OutlineColor = Color.FromArgb(255, 255, 0, 0)

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


        'PictureBox1.



        'Load sound files
        GS.AddSound("Music", Application.StartupPath & "level_music.mp3") 'adds and open a Background music file
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
                'Using Buffer1_BMP As New Bitmap(4000, 4000, Imaging.PixelFormat.Format32bppPArgb)

                Using goBuf1 As Graphics = Graphics.FromImage(Buffer1_BMP)
                    With goBuf1
                        .CompositingMode = Drawing2D.CompositingMode.SourceOver 'Bug Fix
                        'To fix draw string error: "Parameters not valid." Set the compositing mode to source over.
                        .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
                        .InterpolationMode = Drawing2D.InterpolationMode.Low
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
                            'goBuf1.DrawString(index.ToString, Life_Bar_Font, drawBrush, index - Viewport.X, Level.Rec.Y - Viewport.Y)


                            If Show_Rulers = True Then
                                If index <> Level.Rec.Width Then
                                    goBuf1.DrawString(index.ToString, Life_Bar_Font, drawBrush, index - Viewport.X, Level.Rec.Y - Viewport.Y)
                                End If
                            End If


                        Next

                        'Horizontal lines
                        For index = 0 To Level.Rec.Width Step 100
                            'goBuf2.DrawLine(New Pen(Color.Cyan, 1), New Point(0, index), New Point(Level.Rec.Width, index))
                            goBuf1.DrawLine(New Pen(Color.Cyan, 1), New Point(Level.Rec.X - Viewport.X, index - Viewport.Y), New Point(Level.Rec.Width - Viewport.X, index - Viewport.Y))


                            If Show_Rulers = True Then
                                If index <> 0 And index <> Level.Rec.Width Then
                                    goBuf1.DrawString(index.ToString, Life_Bar_Font, drawBrush, Level.Rec.X - Viewport.X, index - Viewport.Y)
                                End If
                            End If




                            'goBuf1.DrawString("Level 1", Life_Bar_Font, drawBrush, Map.X - 3, 6)


                        Next


                    End If






                    If Potion.Active = True Then

                        Draw_Potion(goBuf1, Potion.Rec)

                    End If

                    Draw_Monster(goBuf1, Monster)


                    Draw_Projectile(goBuf1)


                    'If ProjectileInflight = True Then
                    '    goBuf1.FillRectangle(Projectile_Brush, Projectile)
                    'End If







                    If Editor_On = True Then

                        If Mouse_Down = True Then

                            Draw_Wall(goBuf1, Wall.Rec)

                        End If

                    End If



                    Draw_Walls(goBuf1)














                    Draw_Monster_Life_Bar(goBuf1)

                    Draw_Hero(goBuf1, OurHero.Rec)


                    goBuf1.DrawString("Level 1", Life_Bar_Font, drawBrush, Map.X - 3, 6)

                    Draw_Map(goBuf1, Viewport.Width - Map.Width - 10, 40, 9)

                    'Draw_Map(goBuf1, Viewport.Width - Map.Width - 10, 10, 9)

                    If Editor_On = False Then
                        Draw_HeroLife_Bar(goBuf1, Life_Bar_Frame)

                        Draw_Hero_Magic_Bar(goBuf1, Magic_Bar_Frame)

                        goBuf1.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString, Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)

                        If Instructions_On = True Then
                            'goBuf1.DrawString(Instruction_Text, Instruction_Font, drawBrush, 0, Viewport.Height - 30)

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

                    'e.Graphics.DrawImage(Buffer1_BMP, Viewport)
                    'e.Graphics.DrawImage(Buffer1_BMP, 0, 0, Viewport, GraphicsUnit.Pixel)

                End Using
            End Using

        Else

            Using _Buffer2 As New Bitmap(Viewport.Width, Viewport.Height, Imaging.PixelFormat.Format32bppPArgb)
                'Using _Buffer2 As New Bitmap(4000, 4000, Imaging.PixelFormat.Format32bppPArgb)
                Using goBuf2 As Graphics = Graphics.FromImage(_Buffer2)

                    With goBuf2
                        .CompositingMode = Drawing2D.CompositingMode.SourceOver 'Bug Fix
                        'To fix draw string error: "Parameters not valid." Set the compositing mode to source over.
                        .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
                        .InterpolationMode = Drawing2D.InterpolationMode.Low
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
                            'goBuf2.DrawString(index.ToString, Life_Bar_Font, drawBrush, index - Viewport.X, Level.Rec.Y - Viewport.Y)



                            If Show_Rulers = True Then
                                If index <> Level.Rec.Width Then
                                    goBuf2.DrawString(index.ToString, Life_Bar_Font, drawBrush, index - Viewport.X, Level.Rec.Y - Viewport.Y)
                                End If
                            End If


                        Next

                        'Horizontal lines
                        For index = 0 To Level.Rec.Width Step 100
                            'goBuf2.DrawLine(New Pen(Color.Cyan, 1), New Point(0, index), New Point(Level.Rec.Width, index))
                            goBuf2.DrawLine(New Pen(Color.Cyan, 1), New Point(Level.Rec.X - Viewport.X, index - Viewport.Y), New Point(Level.Rec.Width - Viewport.X, index - Viewport.Y))
                            'goBuf2.DrawString(index.ToString, Life_Bar_Font, drawBrush, Level.Rec.X - Viewport.X, index - Viewport.Y)


                            If Show_Rulers = True Then
                                If index <> 0 And index <> Level.Rec.Width Then
                                    goBuf2.DrawString(index.ToString, Life_Bar_Font, drawBrush, Level.Rec.X - Viewport.X, index - Viewport.Y)
                                End If
                            End If

                        Next

                    End If

                    'goBuf2.Clear(Level.BackgroundColor)







                    ''Draw Wall
                    'goBuf2.FillRectangle(New SolidBrush(Wall.Color), Wall.Rec)

                    If Potion.Active = True Then

                        Draw_Potion(goBuf2, Potion.Rec)

                    End If


                    Draw_Monster(goBuf2, Monster)


                    'If Monster_Life > 0 Then


                    '    goBuf2.FillRectangle(Monster_Brush, Monster)

                    '    goBuf2.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), Monster)

                    '    'Monster_Font

                    'End If

                    Draw_Projectile(goBuf2)


                    'Draw_Hero(goBuf2, OurHero.Rec)



                    'Draw Wall
                    'goBuf2.FillRectangle(New SolidBrush(Wall.Color), Wall.Rec)










                    If Editor_On = True Then

                        If Mouse_Down = True Then

                            Draw_Wall(goBuf2, Wall.Rec)

                        End If

                    End If



                    Draw_Walls(goBuf2)







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



                    Draw_Monster_Life_Bar(goBuf2)

                    'If Monster_Life > 0 And Monster_Hit = True Then
                    '    goBuf2.FillRectangle(Life_Frame_Brush, Monster.X, Monster.Y - 10, Monster.Width, 6)
                    '    goBuf2.FillRectangle(Life_Brush, Monster.X, Monster.Y - 10, CInt(Monster.Width / Monster_LifeMAX * Monster_Life), 6)
                    'End If




                    Draw_Hero(goBuf2, OurHero.Rec)

                    'Viewport.Height \ Scale



                    goBuf2.DrawString("Level 1", Life_Bar_Font, drawBrush, Map.X - 3, 6)

                    Draw_Map(goBuf2, Viewport.Width - Map.Width - 10, 40, 9)

                    ''Draw Map************************************************************
                    ''Draw map background.
                    'goBuf2.FillRectangle(New SolidBrush(Color.FromArgb(64, Color.Black)), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 10, 10, Viewport.Width \ 4, Viewport.Height \ 4))

                    ''Wall.MapColor

                    'If Wall.Revealed = True Then
                    '    'Draw wall.
                    '    goBuf2.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 10 + Wall.Rec.X \ 4, 10 + Wall.Rec.Y \ 4, Wall.Rec.Width \ 4, Wall.Rec.Height \ 4))
                    'End If







                    ''Draw hero.
                    'goBuf2.FillRectangle(New SolidBrush(OurHero.Color), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 10 + OurHero.Rec.X \ 4, 10 + OurHero.Rec.Y \ 4, OurHero.Rec.Width \ 4, OurHero.Rec.Height \ 4))

                    ''Draw map border.
                    'goBuf2.DrawRectangle(Map_Border_Pen, New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 10, 10, Viewport.Width \ 4, Viewport.Height \ 4))
                    ''********************************************************************

                    'Draw_HeroLife_Bar(goBuf2, Life_Bar_Frame)




                    'Draw_Hero_Magic_Bar(goBuf2, Magic_Bar_Frame)



                    'goBuf2.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString & "     Undead - Attack:" & Monster_Attack.ToString, Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)
                    'To fix draw string error: "Parameters not valid." I had to set the compositing mode to source over.




                    'goBuf2.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString & "    Level 1 - ", Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)

                    'goBuf2.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString, Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)


                    'If Instructions_On = True Then

                    '    'goBuf2.DrawString(Instruction_Text, Instruction_Font, drawBrush, 0, 0)
                    '    'goBuf2.DrawString(Instruction_Text, Instruction_Font, drawBrush, 0, Viewport.Height - 30)

                    '    Dim Instruction_Rec As New Rectangle(6, Viewport.Height - 60, 940, 200)
                    '    goBuf2.DrawString(Instruction_Text, Instruction_Font, New SolidBrush(Color.White), Instruction_Rec)

                    'End If

                    If Editor_On = False Then
                        Draw_HeroLife_Bar(goBuf2, Life_Bar_Frame)

                        Draw_Hero_Magic_Bar(goBuf2, Magic_Bar_Frame)

                        goBuf2.DrawString("Life " & OurHero.Life.ToString & " / " & OurHero.MaxLife.ToString, Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)

                        If Instructions_On = True Then
                            'goBuf1.DrawString(Instruction_Text, Instruction_Font, drawBrush, 0, Viewport.Height - 30)

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
                        'goBuf2.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        goBuf2.DrawString("Paused", PauseFont, drawBrush, Viewport.Width \ 2, Viewport.Height \ 2, Center_String)
                    End If

                    e.Graphics.DrawImageUnscaled(_Buffer2, 0, 0)
                    'e.Graphics.DrawImage(_Buffer2, 0, 0, Viewport, GraphicsUnit.Pixel)

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






            'Dim Level As Rectangle
            'Level.X = 0
            'Level.Y = 0
            'Level.Width = 7680
            'Level.Height = 5300

            'Dim Scale As Integer = 8

            'Draw Map************************************************************
            'Draw map background.
            'g.FillRectangle(New SolidBrush(Color.FromArgb(64, Color.Black)), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 10, 10, Viewport.Width \ 4, Viewport.Height \ 4))
            'g.FillRectangle(New SolidBrush(Color.FromArgb(64, Color.Black)), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 32, 10, Level.Width \ 8, Level.Height \ 8))
            g.FillRectangle(New SolidBrush(Color.FromArgb(16, Color.White)), New Rectangle(x, y, Level.Rec.Width \ Scale, Level.Rec.Height \ Scale))


            If Walls IsNot Nothing Then
                For index = 0 To UBound(Walls)
                    If Walls(index).Revealed = True Then

                        g.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(x + Walls(index).Rec.X \ Scale, y + Walls(index).Rec.Y \ Scale, CInt(Walls(index).Rec.Width / Scale), CInt(Walls(index).Rec.Height / Scale)))

                    End If
                Next
            End If

            'If Wall.Revealed = True Then
            '    'Draw wall.
            '    'g.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 10 + Wall.Rec.X \ 4, 10 + Wall.Rec.Y \ 4, Wall.Rec.Width \ 4, Wall.Rec.Height \ 4))
            '    'g.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 32 + Wall.Rec.X \ 8, 10 + Wall.Rec.Y \ 8, Wall.Rec.Width \ 8, Wall.Rec.Height \ 8))
            '    'g.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 32 + Wall.Rec.X \ 8, 10 + Wall.Rec.Y \ 8, CInt(Wall.Rec.Width / 8), CInt(Wall.Rec.Height / 8)))
            '    g.FillRectangle(New SolidBrush(Wall.MapColor), New Rectangle(x + Wall.Rec.X \ Scale, y + Wall.Rec.Y \ Scale, CInt(Wall.Rec.Width / Scale), CInt(Wall.Rec.Height / Scale)))
            'End If

            'Draw hero.
            'g.FillRectangle(New SolidBrush(OurHero.Color), New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 32 + OurHero.Rec.X \ 8, 10 + OurHero.Rec.Y \ 8, OurHero.Rec.Width \ 8, OurHero.Rec.Height \ 8))
            g.FillRectangle(New SolidBrush(OurHero.MapColor), New Rectangle(x + OurHero.Rec.X \ Scale, y + OurHero.Rec.Y \ Scale, OurHero.Rec.Width \ Scale, OurHero.Rec.Height \ Scale))





            'Draw map border.
            'g.DrawRectangle(Map_Border_Pen, New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 10, 10, Viewport.Width \ 4, Viewport.Height \ 4))
            'g.DrawRectangle(Map_Border_Pen, New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 32, 10, Level.Width \ 8, Level.Height \ 8))
            g.DrawRectangle(Map_Border_Pen, New Rectangle(x, y, Level.Rec.Width \ Scale, Level.Rec.Height \ Scale))



            'Draw viewport.
            'g.DrawRectangle(Map_Border_Pen, New Rectangle(Viewport.Width - (Viewport.Width \ 4) - 32 + Viewport.X \ Scale, 10 + Viewport.Y \ Scale, Viewport.Width \ Scale, Viewport.Height \ Scale))
            g.DrawRectangle(Map_Border_Pen, New Rectangle(x + Viewport.X \ Scale, y + Viewport.Y \ Scale, Viewport.Width \ Scale, Viewport.Height \ Scale))
            '********************************************************************



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



        Dim test As Double = (Bar.Width - 4) / OurHero.MaxLife * OurHero.Life


        'Is the heros life points critically low?
        If OurHero.Life > OurHero.MaxLife \ 4 Then
            'g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt(((Bar.Width - 4) \ OurHero.LifeMAX) * OurHero.Life), Bar.Height - 4)
            'g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxLife * OurHero.Life), Bar.Height - 4)
            g.FillRectangle(Life_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height)
            g.DrawRectangle(Life_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height - 1)
        Else
            'Yes, the heros life points are critically low?

            Select Case Life_Blink_Counter
                Case 0 To 8
                    'g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxLife * OurHero.Life), Bar.Height - 4)
                    g.FillRectangle(Life_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height)
                    g.DrawRectangle(Life_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height - 1)
                    Life_Blink_Counter += 1
                Case 9 To 18
                    'g.FillRectangle(Life_Blink_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxLife * OurHero.Life), Bar.Height - 4)
                    g.FillRectangle(Life_Blink_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height)
                    g.DrawRectangle(Life_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height - 1)
                    Life_Blink_Counter += 1
                Case Else
                    'g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxLife * OurHero.Life), Bar.Height - 4)
                    g.FillRectangle(Life_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height)
                    g.DrawRectangle(Life_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxLife * OurHero.Life), Bar.Height - 1)
                    Life_Blink_Counter = 0
            End Select
        End If
    End Sub


    Private Sub Draw_Hero_Magic_Bar(g As Graphics, Bar As Rectangle)


        'Draw hero magic bar frame.
        g.FillRectangle(Magic_Frame_Brush, Bar)



        Dim test As Double = (Bar.Width - 4) / OurHero.MaxMagic * OurHero.Magic


        'Is the heros magic points critically low?
        If OurHero.Magic >= OurHero.MaxMagic \ 4 Then
            'g.FillRectangle(Life_Brush, Bar.X + 2, Bar.Y + 2, CInt(((Bar.Width - 4) \ OurHero.LifeMAX) * OurHero.Life), Bar.Height - 4)
            'g.FillRectangle(Magic_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 4)





            g.FillRectangle(Magic_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)
            g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)


        Else
            'Yes, the heros life points are critically low?

            Select Case Magic_Blink_Counter
                Case 0 To 8
                    'g.FillRectangle(Magic_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 4)



                    g.FillRectangle(Magic_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)
                    g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)



                    Magic_Blink_Counter += 1
                Case 9 To 18
                    'g.FillRectangle(Magic_Blink_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 4)



                    g.FillRectangle(Magic_Blink_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)
                    g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)



                    Magic_Blink_Counter += 1
                Case Else
                    'g.FillRectangle(Magic_Brush, Bar.X + 2, Bar.Y + 2, CInt((Bar.Width - 4) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 4)



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
        ' New SolidBrush(Potion.Color)


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
            'g.DrawRectangle(New Pen(Potion.OutlineColor, 1), Rec)



            'Draw shadow.
            Dim MyShadow As Integer

            Dim Distance As Double = Distance_Between_Points(Potion.Rec.Location, OurHero.Rec.Location)

            If Distance <= Viewport.Width / 2 Then
                MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance_Between_Points(Potion.Rec.Location, OurHero.Rec.Location))
            Else

                MyShadow = 255
            End If
            g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), PotionInViewportCoordinates)


            'Draw darkness.
            'g.FillRectangle(New SolidBrush(Color.FromArgb(200, Color.Black)), Rec)

        End If

    End Sub


    Private Sub Draw_Monster(g As Graphics, Rec As Rectangle)

        Dim MonsterInViewportCoordinates As Rectangle
        MonsterInViewportCoordinates = Rec
        MonsterInViewportCoordinates.X = Rec.X - Viewport.X
        MonsterInViewportCoordinates.Y = Rec.Y - Viewport.Y


        'LightRec




        If Monster_Life > 0 Then


            If MonsterInViewportCoordinates.IntersectsWith(LightRec) = True Then

                g.FillRectangle(Monster_Brush, MonsterInViewportCoordinates)
                g.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), MonsterInViewportCoordinates, Center_String)


                'Draw shadow.
                Dim MyShadow As Integer
                Dim Distance As Double = Distance_Between_Points(Monster.Location, OurHero.Rec.Location)
                If Distance <= Viewport.Width / 2 Then
                    MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance_Between_Points(Monster.Location, OurHero.Rec.Location))
                Else
                    MyShadow = 255
                End If
                g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), MonsterInViewportCoordinates)



                g.DrawRectangle(New Pen(Color.Green, 1), MonsterInViewportCoordinates)

            Else

                g.FillRectangle(Monster_Brush, MonsterInViewportCoordinates)




                g.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), MonsterInViewportCoordinates, Center_String)
                'g.DrawRectangle(New Pen(Color.Green, 1), Rec)

                'Draw shadow.
                Dim MyShadow As Integer

                Dim Distance As Double = Distance_Between_Points(Monster.Location, OurHero.Rec.Location)

                If Distance <= Viewport.Width / 2 Then
                    MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance_Between_Points(Monster.Location, OurHero.Rec.Location))
                Else

                    MyShadow = 255
                End If
                g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), MonsterInViewportCoordinates)


            End If








        End If

    End Sub


    Private Sub Draw_Wall(g As Graphics, Rec As Rectangle)

        Dim WallInViewportCoordinates As Rectangle
        WallInViewportCoordinates = Rec
        WallInViewportCoordinates.X = Rec.X - Viewport.X
        WallInViewportCoordinates.Y = Rec.Y - Viewport.Y

        'Draw Wall
        g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
        g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

        'If WallInViewportCoordinates.IntersectsWith(LightRec) = True Then

        '    'Draw Wall
        '    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

        '    'Draw shadow.
        '    Dim MyShadow As Integer
        '    Dim Distance As Double = Distance_Between_Points(Wall.Rec.Location, OurHero.Rec.Location)
        '    If Distance <= Viewport.Width / 2 Then
        '        MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance)
        '    Else
        '        MyShadow = 255
        '    End If
        '    g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), WallInViewportCoordinates)

        '    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

        'Else

        '    'Draw Wall
        '    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)



        '    'Draw shadow.
        '    Dim MyShadow As Integer
        '    Dim Distance As Double = Distance_Between_Points(Wall.Rec.Location, OurHero.Rec.Location)
        '    If Distance <= Viewport.Width / 2 Then
        '        MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance)
        '    Else
        '        MyShadow = 255
        '    End If
        '    g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), WallInViewportCoordinates)

        '    'g.FillRectangle(New SolidBrush(Color.FromArgb(150, Color.Black)), Wall.Rec)

        'End If




    End Sub



    Private Sub Draw_Walls(g As Graphics)

        Dim WallInViewportCoordinates As Rectangle





        'Is the editor on?
        If Editor_On = False Then
            'No, the editor off. The game is running.

            If Walls IsNot Nothing Then
                For index = 0 To UBound(Walls)
                    'Frame_Graphics.DrawRectangle(Drawing_Pen, Walls(index))
                    'Frame_Graphics.FillRectangle(Wall_Brush, Walls(index))
                    WallInViewportCoordinates = Walls(index).Rec
                    WallInViewportCoordinates.X = Walls(index).Rec.X - Viewport.X
                    WallInViewportCoordinates.Y = Walls(index).Rec.Y - Viewport.Y
                    'g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)


                    'Draw Wall
                    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
                    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)


                    'If WallInViewportCoordinates.IntersectsWith(LightRec) = True Then

                    '    'Draw Wall
                    '    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

                    '    ''Draw shadow.
                    '    'Dim MyShadow As Integer
                    '    ''Dim Distance As Double = Distance_Between_Points(Walls(index).Location, OurHero.Rec.Location)
                    '    'Dim Distance As Double = Distance_Between_Points(New Point(Walls(index).X + Walls(index).Width \ 2, Walls(index).Y + Walls(index).Height \ 2), OurHero.Rec.Location)
                    '    'If Distance <= Viewport.Width Then
                    '    '    MyShadow = CInt((255 / (Viewport.Width)) * Distance)
                    '    'Else
                    '    '    MyShadow = 255
                    '    'End If
                    '    'g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), WallInViewportCoordinates)

                    '    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

                    'Else

                    '    'Draw Wall
                    '    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

                    '    'Draw shadow.
                    '    Dim MyShadow As Integer
                    '    'Dim Distance As Double = Distance_Between_Points(Walls(index).Location, OurHero.Rec.Location)
                    '    'Dim Distance As Double = Distance_Between_Points(New Point(Walls(index).X + Walls(index).Width \ 2, Walls(index).Y + Walls(index).Height \ 2), OurHero.Rec.Location)
                    '    'Dim Distance As Double = Horizontal_Distance(Walls(index).X, OurHero.Rec.X)
                    '    'Dim Distance As Double = Vertical_Distance(Walls(index).Y, OurHero.Rec.Y)

                    '    Dim Distance As Double
                    '    If Horizontal_Distance(Walls(index).X + Walls(index).Width \ 2, OurHero.Rec.X) > Vertical_Distance(Walls(index).Y + Walls(index).Height \ 2, OurHero.Rec.Y) Then

                    '        Distance = Vertical_Distance(Walls(index).Y + Walls(index).Height \ 2, OurHero.Rec.Y)
                    '    Else


                    '        Distance = Horizontal_Distance(Walls(index).X + Walls(index).Width \ 2, OurHero.Rec.X)


                    '    End If

                    '    'If Distance <= Viewport.Width / 2 Then
                    '    '    MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance)
                    '    'Else
                    '    '    MyShadow = 255

                    '    'End If
                    '    If Distance <= 500 Then
                    '        MyShadow = CInt((255 / (500)) * Distance)
                    '    Else
                    '        MyShadow = 255

                    '    End If
                    '    g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), WallInViewportCoordinates)

                    '    'g.FillRectangle(New SolidBrush(Color.FromArgb(150, Color.Black)), Wall.Rec)

                    'End If

                Next

            End If





        Else

            'Yes, the editor is on. The game is stopped.

            'Draw Wall
            'g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)


            'g.DrawString("Undead", Monster_Font, New SolidBrush(Color.Black), WallInViewportCoordinates, Center_String)



            If Walls IsNot Nothing Then
                For index = 0 To UBound(Walls)
                    'Frame_Graphics.DrawRectangle(Drawing_Pen, Walls(index))
                    'Frame_Graphics.FillRectangle(Wall_Brush, Walls(index))
                    WallInViewportCoordinates = Walls(index).Rec
                    WallInViewportCoordinates.X = Walls(index).Rec.X - Viewport.X
                    WallInViewportCoordinates.Y = Walls(index).Rec.Y - Viewport.Y
                    'g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)


                    'Draw Wall
                    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
                    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)
                    g.DrawString(index.ToString, Monster_Font, New SolidBrush(Color.Black), WallInViewportCoordinates, Center_String)



                    'If WallInViewportCoordinates.IntersectsWith(LightRec) = True Then

                    '    'Draw Wall
                    '    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

                    '    'Draw shadow.
                    '    Dim MyShadow As Integer
                    '    Dim Distance As Double = Distance_Between_Points(Walls(index).Location, OurHero.Rec.Location)
                    '    If Distance <= Viewport.Width / 2 Then
                    '        MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance)
                    '    Else
                    '        MyShadow = 255
                    '    End If
                    '    g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), WallInViewportCoordinates)

                    '    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

                    'Else

                    '    'Draw Wall
                    '    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

                    '    'Draw shadow.
                    '    Dim MyShadow As Integer
                    '    Dim Distance As Double = Distance_Between_Points(Walls(index).Location, OurHero.Rec.Location)
                    '    If Distance <= Viewport.Width / 2 Then
                    '        MyShadow = CInt((255 / (Viewport.Width / 2)) * Distance)
                    '    Else
                    '        MyShadow = 255
                    '    End If
                    '    g.FillRectangle(New SolidBrush(Color.FromArgb(MyShadow, Color.Black)), WallInViewportCoordinates)

                    '    'g.FillRectangle(New SolidBrush(Color.FromArgb(150, Color.Black)), Wall.Rec)

                    'End If
                Next
            End If


            If IsSelected = True Then

                'Draw selection outline.
                'Frame_Graphics.DrawRectangle(Selected_Pen, Walls(Selected_Index))
                WallInViewportCoordinates = Walls(Selected_Index).Rec
                WallInViewportCoordinates.X = Walls(Selected_Index).Rec.X - Viewport.X
                WallInViewportCoordinates.Y = Walls(Selected_Index).Rec.Y - Viewport.Y
                g.DrawRectangle(Selected_Pen, WallInViewportCoordinates)
                g.FillEllipse(Brushes.Red, WallInViewportCoordinates.X - 15 \ 2, WallInViewportCoordinates.Y - 15 \ 2, 15, 15)
                Dim MyString As String = "X " & WallInViewportCoordinates.X.ToString & ",Y " & WallInViewportCoordinates.Y


                g.DrawString(MyString, Monster_Font, New SolidBrush(Color.White), New Point(WallInViewportCoordinates.X, WallInViewportCoordinates.Y - 20), Center_String)



                ''Draw control points.
                'Frame_Graphics.DrawRectangle(Selected_Pen, Walls(Selected_Index).Left, Walls(Selected_Index).Top, 15, 15)
                'Frame_Graphics.DrawRectangle(Selected_Pen, Walls(Selected_Index).Right - 15, Walls(Selected_Index).Top, 15, 15)

                'Frame_Graphics.DrawRectangle(Selected_Pen, Walls(Selected_Index).Right - 15, Walls(Selected_Index).Bottom - 15, 15, 15)
                'Frame_Graphics.DrawRectangle(Selected_Pen, Walls(Selected_Index).Left, Walls(Selected_Index).Bottom - 15, 15, 15)


            End If

        End If






        'If Mouse_Down = True Then

        '    'Frame_Graphics.FillRectangle(Wall_Brush, Wall)
        '    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

        'End If

        'Frame_Graphics.DrawRectangle(Drawing_Pen, Wall)


    End Sub


    Private Sub Draw_Hero_Light(g As Graphics, Rec As Rectangle)


        Dim HeroInViewportCoordinates As Rectangle
        HeroInViewportCoordinates = Rec
        HeroInViewportCoordinates.X = Rec.X - Viewport.X
        HeroInViewportCoordinates.Y = Rec.Y - Viewport.Y

        'Dim LightRec As New Rectangle

        If ProjectileInflight = True Then

            LightRec = HeroInViewportCoordinates

            LightRec.Inflate(300, 300)


            'Create a path
            Dim path As New GraphicsPath()
            path.AddEllipse(LightRec)


            'Create a path gradient brush
            Dim pgBrush As New PathGradientBrush(path)
            'pgBrush.CenterColor = Color.White

            pgBrush.CenterColor = Color.FromArgb(255, 255, 255, 255)



            'Dim list As Color() = New Color() {Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0)}
            Dim list As Color() = New Color() {Color.FromArgb(0, 255, 255, 255), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0)}



            pgBrush.SurroundColors = list


            g.FillPath(pgBrush, path)


            'g.FillRectangle(New SolidBrush(Color.FromArgb(64, Color.White)), LightRec)
            'g.FillEllipse(New SolidBrush(Color.FromArgb(64, Color.White)), LightRec)

        Else

            LightRec = HeroInViewportCoordinates

            LightRec.Inflate(300, 300)

            'Create a path
            Dim path As New GraphicsPath()
            path.AddEllipse(LightRec)


            'Create a path gradient brush
            Dim pgBrush As New PathGradientBrush(path)
            'pgBrush.CenterColor = Color.White
            pgBrush.CenterColor = Color.FromArgb(90, Color.White)



            Dim list As Color() = New Color() {Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0)}
            pgBrush.SurroundColors = list

            'g.SmoothingMode = SmoothingMode.AntiAlias


            g.FillPath(pgBrush, path)




            'g.FillRectangle(New SolidBrush(Color.FromArgb(64, Color.White)), LightRec)
            'g.FillEllipse(New SolidBrush(Color.FromArgb(32, Color.White)), LightRec)

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

                'Dim LightRec As New Rectangle

                'LightRec = Rec

                'LightRec.Inflate(300, 300)

                'g.FillRectangle(New SolidBrush(Color.FromArgb(64, Color.White)), LightRec)

                g.FillRectangle(New SolidBrush(OurHero.Color), HeroInViewportCoordinates)

                g.DrawString("Hero", Monster_Font, New SolidBrush(Color.Black), HeroInViewportCoordinates, Center_String)

                g.DrawRectangle(New Pen(OurHero.OutlineColor, 1), HeroInViewportCoordinates)


            Else
                g.FillRectangle(New SolidBrush(Color.FromArgb(255, Color.Red)), HeroInViewportCoordinates)

                g.DrawString("Hero", Monster_Font, New SolidBrush(Color.White), HeroInViewportCoordinates, Center_String)

                g.DrawRectangle(New Pen(Color.White, 1), HeroInViewportCoordinates)


                'g.DrawString("-" & CStr(OurHero.LifeBeforeHit - OurHero.Life), Monster_Font, New SolidBrush(Color.White), Rec.X - 10, Rec.Y - 10, Center_String)



                Dim HitTotal As Integer = OurHero.LifeBeforeHit - OurHero.Life
                If HitTotal < 0 Then
                    HitTotal = -1
                End If


                'g.DrawString("-" & CStr(Math.Abs(OurHero.LifeBeforeHit - OurHero.Life)), Monster_Font, New SolidBrush(Color.White), Rec.X - 18, Rec.Y - 35)
                g.DrawString("-" & CStr(HitTotal), Monster_Font, New SolidBrush(Color.White), HeroInViewportCoordinates.X - 18, HeroInViewportCoordinates.Y - 35)

                'OurHero.LifeBeforeHit

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
        If Editor_On = False Then

            If GS.IsPlaying("Music") = False Then

                GS.Play("Music")

            End If

            If Me.Text <> "Dungeon Crawler - Playing" Then

                Me.Text = "Dungeon Crawler - Playing"

            End If

        Else
            GS.Stop("Music")

            If Me.Text <> "Dungeon Crawler - Editing" Then
                Me.Text = "Dungeon Crawler - Editing"

            End If



        End If




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



        If Editor_On = False Then


            Do_Hero_Moves()

            Do_Monster_Moves()





            If Walls IsNot Nothing Then
                For index = 0 To UBound(Walls)
                    If Walls(index).Rec.IntersectsWith(Viewport) Then
                        If Walls(index).Revealed <> True Then

                            Walls(index).Revealed = True

                        End If
                    End If
                Next
            End If




        End If





    End Sub

    Private Sub Do_Hero_Moves()

        If OurHero.Life > 0 Then

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

        Else

            ''Play hero death sound.
            'If GS.IsPlaying("Hero_Death") = False Then
            '    GS.Play("Hero_Death")
            'End If

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


            'OurMonster.WallCollision = True


            'Push monster to the left of wall.
            Monster.X = Wall.Rec.X - Monster.Width - 1

            'If GS.IsPlaying("Undead_Move") = True Then
            '    GS.Pause("Undead_Move")
            'End If

            'Move monster down.
            'Monster.Y += Monster_Speed

            'If Monster.IntersectsWith(Wall.Rec) = True Then

            'Push monster above wall.
            'Monster.Y = Wall.Rec.Y - Monster.Height - 1

            'End If

            'OurMonster.WallCollision = False

        End If

        '************************************************

        'Attack Right**************************************************
        'Is the monster touching the hero?
        If Monster.IntersectsWith(OurHero.Rec) = True Then
            'Yes, the monster is touching the hero.


            'Push the monster to the left of the hero.
            Monster.X = OurHero.Rec.X - Monster.Width - 1


            'Roll for initative.************************
            If RandomNumber.Next(1, 20) > 4 Then 'Chance to hit is 1 in 20
                Monster_Initiative = True
            Else
                Monster_Initiative = False
            End If
            '*******************************************

            If Monster_Initiative = True Then

                'Reset the hit total on the first hit.
                If OurHero.Hit = False Then
                    OurHero.LifeBeforeHit = OurHero.Life
                End If

                'Attack the heros life points directly.
                OurHero.Life -= Monster_Attack



                If OurHero.Life < 1 Then

                    'Play hero death sound.
                    'If GS.IsPlaying("Hero_Death") = False Then
                    GS.Play("Hero_Death")

                    OurHero.Life = 0
                    'End If
                End If


                OurHero.Hit = True


                'If GS.IsPlaying("Undead_Move") = True Then
                '    GS.Pause("Undead_Move")
                'End If

                'Play undead attack sound.
                If GS.IsPlaying("Undead_Attack") = False Then
                    GS.Play("Undead_Attack") 'play the Music
                End If

                'Knock hero to the right of monster.
                OurHero.Rec.X = Monster.X + Monster.Width + 32


                'Wall Collision Handler - Moving Right*****************
                'Is the hero touching a wall?
                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                    'Yes, the hero is touching a wall.

                    'Knock hero to the left of wall.
                    OurHero.Rec.X = Wall.Rec.X - OurHero.Rec.Width - 1

                    'Knock the monster to the left of the hero.
                    Monster.X = OurHero.Rec.X - Monster.Width - 16

                End If
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

        'Move Left************************************************************************************
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

        'Play undead moving sound.
        If GS.IsPlaying("Undead_Move") = False Then
            GS.Play("Undead_Move")
        End If
        '***********************************************************************************************
        'Wall Collision Handler Monster moving left*************************
        'Is the monster touching the wall?
        If Monster.IntersectsWith(Wall.Rec) = True Then
            'Yes, the monster is touching the wall.

            'Push monster to the right of wall.
            Monster.X = Wall.Rec.X + Wall.Rec.Width

            'If GS.IsPlaying("Undead_Move") = True Then
            '    GS.Pause("Undead_Move")
            'End If

        End If
        '************************************************
        'Attack Left************************************************************************************
        'Is the monster touching the hero?
        If Monster.IntersectsWith(OurHero.Rec) = True Then
            'Yes, the monster is touching the hero.

            'Push the monster to the right of the hero.
            Monster.X = OurHero.Rec.X + OurHero.Rec.Width + 1

            'Roll for initative.************************
            If RandomNumber.Next(1, 20) > 5 Then 'Chance to hit is 1 in 20
                Monster_Initiative = True
            Else
                Monster_Initiative = False
            End If
            '*******************************************

            If Monster_Initiative = False Then

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
                    GS.Play("Hero_Death")

                    OurHero.Life = 0

                End If

                OurHero.Hit = True

                'Knock hero to the left of monster.
                OurHero.Rec.X = Monster.X - OurHero.Rec.Width - 32

                'Is the hero touching a wall?
                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                    'Yes, the hero is touching a wall.

                    'Knock hero to the right of wall.
                    OurHero.Rec.X = Wall.Rec.X + Wall.Rec.Width + 1

                    'Knock the monster to the right of the hero.
                    Monster.X = OurHero.Rec.X + OurHero.Rec.Width + 16

                End If
            End If
        End If
        '*********************************************************************
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




        'If OurMonster.WallCollision = False Then


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

        'Wall Collision Handler - Monster moving up *************************
        'Is the monster touching the wall?
        If Monster.IntersectsWith(Wall.Rec) = True Then
            'Yes, the monster is touching the wall.

            'Push the hero below the wall.
            Monster.Y = Wall.Rec.Y + Wall.Rec.Height

            'If GS.IsPlaying("Undead_Move") = True Then
            '    GS.Pause("Undead_Move")
            'End If

        End If
        '************************************************
        'Attack Up************************************************************************************
        If Monster.IntersectsWith(OurHero.Rec) = True Then

            'Push the monster below the hero.
            Monster.Y = OurHero.Rec.Y + OurHero.Rec.Height + 1

            If OurHero.Initiative = False Then


                'Reset the hit total on the first hit.
                If OurHero.Hit = False Then
                    OurHero.LifeBeforeHit = OurHero.Life
                End If

                'Attack the heros life points directly.
                OurHero.Life -= Monster_Attack

                OurHero.Hit = True

                'Play undead attack sound.
                If GS.IsPlaying("Undead_Attack") = False Then
                    GS.Play("Undead_Attack") 'play the Music
                End If

                'Knock hero above the monster.
                OurHero.Rec.Y = Monster.Y - OurHero.Rec.Height - 32

                'Is the hero touching a wall?
                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                    'Yes, the hero is touching a wall.

                    'Knock hero below the wall.
                    OurHero.Rec.Y = Wall.Rec.Y + Wall.Rec.Height + 1

                    'Knock the monster below the hero.
                    Monster.Y = OurHero.Rec.Y + OurHero.Rec.Height + 16

                End If
            End If
        End If


        'End If






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
                                OurHero.Rec.Y = Monster.Y + Monster.Height + 16

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

            'If GS.IsPlaying("Undead_Move") = True Then
            '    GS.Pause("Undead_Move")
            'End If

        End If
        '************************************************


        If Monster.IntersectsWith(OurHero.Rec) = True Then

            'Push to monster above the hero.
            Monster.Y = OurHero.Rec.Y - Monster.Height - 1


            If OurHero.Initiative = False Then

                'Reset the hit total on the first hit.
                If OurHero.Hit = False Then
                    OurHero.LifeBeforeHit = OurHero.Life
                End If

                'Attack the heros life points directly.
                OurHero.Life -= Monster_Attack


                OurHero.Hit = True

                'Play undead attack sound.
                If GS.IsPlaying("Undead_Attack") = False Then
                    GS.Play("Undead_Attack") 'play the Music
                End If

                'Knock hero below monster.
                OurHero.Rec.Y = Monster.Y + Monster.Height + 32

                'Is the hero touching a wall?
                If OurHero.Rec.IntersectsWith(Wall.Rec) = True Then
                    'Yes, the hero is touching a wall.

                    'Knock hero above the wall.
                    OurHero.Rec.Y = Wall.Rec.Y - OurHero.Rec.Height - 1

                    'Knock the monster above the hero.
                    Monster.Y = OurHero.Rec.Y - Monster.Height - 16

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


                ''Init Life Potion
                'Potion.IsLife = True
                'Potion.Rec = New Rectangle(0, 0, 200, 200)
                'Potion.Active = False
                'Potion.Color = Color.FromArgb(255, 95, 7, 12)
                'Potion.OutlineColor = Color.FromArgb(255, 255, 0, 0)

                ''Init Magic Potion
                'Potion.IsLife = False
                'Potion.Rec = New Rectangle(0, 0, 200, 200)
                'Potion.Active = False
                'Potion.Color = Color.FromArgb(255, 0, 0, 255)
                'Potion.OutlineColor = Color.FromArgb(255, 0, 0, 255)



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



                                'Knock monster above the hero.
                                Monster.Y -= CInt(Projectile_Speed / 3)



                                'Wall Collision Handler - Monster moving up *************************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push the hero below the wall.
                                    Monster.Y = Wall.Rec.Y + Wall.Rec.Height

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


                                'ProjectileInflight = False


                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock monster below hero.
                                Monster.Y += CInt(Projectile_Speed / 3)


                                'Wall Collision Handler - Moving Down ********************************************************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push the monster above the wall.
                                    Monster.Y = Wall.Rec.Y - Monster.Height - 1

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
                                'ProjectileInflight = False
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




                                'Knock monster above hero.
                                Monster.Y -= CInt(Projectile_Speed / 3)


                                'Wall Collision Handler - Monster moving up *************************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push the hero below the wall.
                                    Monster.Y = Wall.Rec.Y + Wall.Rec.Height

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
                                'ProjectileInflight = False
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




                                'Knock monster below hero.
                                Monster.Y += CInt(Projectile_Speed / 3)


                                'Wall Collision Handler - Moving Down ********************************************************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push the monster above the wall.
                                    Monster.Y = Wall.Rec.Y - Monster.Height - 1

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
                                'ProjectileInflight = False
                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If




                                'Knock monster to the left of hero.
                                Monster.X -= CInt(Projectile_Speed / 3)


                                'Wall Collision Handler Monster moving left*************************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push monster to the right of wall.
                                    Monster.X = Wall.Rec.X + Wall.Rec.Width

                                End If
                                '************************************************





                                'Knock monster above the hero.
                                Monster.Y -= CInt(Projectile_Speed / 3)


                                'Wall Collision Handler - Monster moving up *************************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push the hero below the wall.
                                    Monster.Y = Wall.Rec.Y + Wall.Rec.Height

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
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push monster to the right of wall.
                                    Monster.X = Wall.Rec.X + Wall.Rec.Width

                                End If
                                '************************************************






                                'Knock the monster below the hero.
                                Monster.Y += CInt(Projectile_Speed / 3)




                                'Wall Collision Handler - Moving Down ********************************************************
                                'Is the monster touching the wall?
                                If Monster.IntersectsWith(Wall.Rec) = True Then
                                    'Yes, the monster is touching the wall.

                                    'Push the monster above the wall.
                                    Monster.Y = Wall.Rec.Y - Monster.Height - 1

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
            'Case Keys.Left
            '    If Editor_On = False Then
            '        MoveLeft = True
            '    Else
            '        MoveViewport(DirectionEnum.Left)
            '    End If
            'Case Keys.Right
            '    If Editor_On = False Then
            '        MoveRight = True
            '    Else
            '        MoveViewport(DirectionEnum.Right)
            '    End If
            'Case Keys.Up
            '    If Editor_On = False Then
            '        MoveUp = True
            '    Else
            '        MoveViewport(DirectionEnum.Up)
            '    End If
            'Case Keys.Down
            '    If Editor_On = False Then
            '        MoveDown = True
            '    Else
            '        MoveViewport(DirectionEnum.Down)
            '    End If

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
        'Map_On = True

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
    Private Sub GS_SoundEnded(ByVal SndName As String)
        'If SndName = "Music" Then GS.Play("Music") 'if the Music has reached the end then you can keep replaying it for a loop effect

        If GS.IsPlaying("Music") = False Then

            GS.Play("Music")

        End If

        'mciSendString("close FirstSound", String.Empty, 0, 0)
    End Sub

    Private Sub MenuItemEditorOn_Click(sender As Object, e As EventArgs) Handles MenuItemEditorOn.Click


        If Editor_On = False Then
            Editor_On = True
            MenuItemEditorOn.Checked = True


        Else

            Editor_On = False
            MenuItemEditorOn.Checked = False


        End If






    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown


        If Editor_On = True Then

            Mouse_Down = True

            If Selected_Tool = ToolsEnum.Pointer Then

            End If
            If Selected_Tool = ToolsEnum.Wall Then

                Wall.Rec.Width = 0
                Wall.Rec.Height = 0

                'Wall_Origin = e.Location
                Wall_Origin.X = e.X + Viewport.X
                Wall_Origin.Y = e.Y + Viewport.Y

            End If
        End If





    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp


        If Editor_On = True Then

            Mouse_Down = False

            If Selected_Tool = ToolsEnum.Pointer Then
                If Walls IsNot Nothing Then
                    Dim rec As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)
                    'IsSelected = False
                    For index = 0 To UBound(Walls)
                        If rec.IntersectsWith(Walls(index).Rec) Then
                            Selected_Index = index
                            IsSelected = True
                            Exit For
                        End If
                        IsSelected = False
                    Next
                End If
            End If

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

        'Create temporary array
        Dim TempWalls(UBound(Walls) - 1) As WallInfo
        Dim TempIndex As Integer = LBound(Walls)

        'Copy the array without the element to the temporary array.
        For index = LBound(Walls) To UBound(Walls)

            If index <> IndexToRemove Then

                TempWalls(TempIndex) = Walls(index)

                TempIndex += 1
            End If


        Next

        'Resize the array.
        ReDim Walls(UBound(TempWalls))

        'Copy the temporary array to the array.
        Walls = TempWalls

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
