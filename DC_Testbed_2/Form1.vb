Option Strict On

'Dungeon Crawler
'A work in progress...
'This is a simple action role-playing game in which the hero navigates a labyrinth,
'battles various monsters, avoids traps, solves puzzles, and loots any treasure that is found.

'MIT License
'Copyright(c) 2021 Joseph Lumbley

'Permission Is hereby granted, free Of charge, to any person obtaining a copy
'of this software And associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
'copies of the Software, And to permit persons to whom the Software Is
'furnished to do so, subject to the following conditions:

'The above copyright notice And this permission notice shall be included In all
'copies Or substantial portions of the Software.

'THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
'IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
'LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
'OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
'SOFTWARE.

Imports System.Math
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports System.IO
'Imports System.Runtime.Serialization.Formatters.Binary

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

Public Structure Floor_Struct

    Public Rec As Rectangle
    Public Color As Color
    Public OutlineColor As Color
    Public Revealed As Boolean 'If true the floor will be seen on the map.
    Public MapColor As Color
    Public MapOutlineColor As Color

End Structure

Structure Game_Objects_Struct

    Public ID As Integer

    Public X As Integer
    Public Y As Integer
    Public Width As Integer
    Public Height As Integer

    Public Color As Integer
    Public OutlineColor As Integer

    Public MapColor As Integer
    Public MapOutlineColor As Integer

    Public Revealed As Boolean

    Public Text As String

    Public IsOpen As Boolean

    '<VBFixedString(256)> Public Text As String

End Structure


Public Structure DoorInfo
    Public Rec As Rectangle
    Public Color As Color
    Public OutlineColor As Color
    Public Revealed As Boolean 'If true the door will be seen on the map.
    Public MapColor As Color
    Public MapOutlineColor As Color
    Public IsOpen As Boolean

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
    Floor = 2

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

Public Enum Object_ID_Enum As Integer

    Level = 0
    Hero = 1
    Undead = 2
    Wall = 3
    Floor = 4
    Floor_Light = 5
    Life_Potion = 6
    Magic_Potion = 7
    Door = 8

End Enum

Public Class Form1



    Private Update_Properties As Boolean = False

    Private Game_Objects() As Game_Objects_Struct

    Const Rad2Deg As Double = 180.0 / Math.PI

    Private Viewport As New Rectangle(0, 0, 640, 480)

    Private Swap_Buffer As Boolean = True

    Private Level As LevelInfo

    Private OurHero As HeroInfo

    Private Door As DoorInfo

    Private Movement_Target As Point = New Point(0, 0)

    Private Magic_Target As Point = New Point(0, 0)

    'Create and Initialize editor state data.
    Private Editor_On As Boolean = False
    Private Show_Rulers As Boolean = True
    Private Selected_Tool As ToolsEnum = ToolsEnum.Pointer
    Private Pointer_Origin As New Point(0, 0)
    Private Pointer_Offset As New Point(0, 0)
    Private Selected_Wall_Index As Integer = 0
    Private IsWallSelected As Boolean = False
    Private Selected_Floor_Index As Integer = 0
    Private IsFloorSelected As Boolean = False
    Private Selected_Pen As New Pen(Color.Blue, 5)

    Private BottomRightControlHandle_Selected As Boolean = False
    Private TopLeftControlHandle_Selected As Boolean = False

    Private Wall As WallInfo
    Private Wall_Origin As Point
    Private Walls() As WallInfo

    Private Floor As Floor_Struct
    Private Floor_Origin As Point
    Private Floors() As Floor_Struct


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
    Private Projectile_Origin As New Point(0, 0)
    Private ProjectileInflight As Boolean = False
    Private Projectile_Brush As New SolidBrush(Color.Yellow)
    Private Projectile_Max_Distance As Integer = 300
    Private Projectile_Attack As Integer = 12
    Private Projectile_Speed As Integer = 30
    Private Projectile_Direction As DirectionEnum

    Private ShootLeft As Boolean = False
    Private ShootRight As Boolean = False
    Private ShootUp As Boolean = False
    Private ShootDown As Boolean = False

    Private CtrlDown As Boolean = False

    Private Mouse_Down_Left As Boolean = False
    Private Mouse_Down_Right As Boolean = False

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

    Private Instruction_Text As String = "Use left mouse button to move and attack. Use right mouse button to cast spells. Press P to pause, M for map and I for instructions."
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

    Dim cm As New Drawing.Imaging.ColorMatrix
    Dim atr As New Drawing.Imaging.ImageAttributes

    Private Intersection_REC As New Rectangle(0, 0, 1, 1)

    Private Const Quarter_Number As Double = 0.25

    'Dim x As Single = 0
    'Dim y As Single = 0

    Dim drawFormat As New StringFormat

    Dim DistanceToHero As Integer


    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        CreateSoundFileFromResource()





        'Display_Wall_Properties
        'Display_Wall_Properties()

        Door.Rec.X = 2000
        Door.Rec.Y = 2000
        Door.Rec.Width = 100
        Door.Rec.Height = 35
        Door.Color = Color.BurlyWood
        Door.OutlineColor = Color.Beige
        Door.IsOpen = False
        Door.MapColor = Color.BurlyWood
        Door.MapOutlineColor = Color.Beige
        Door.Revealed = False

        Level.BackgroundColor = Color.FromArgb(255, 0, 0, 0)
        Level.Rec.X = 0
        Level.Rec.Y = 0
        'Level.Rec.Width = 5300


        'Level.Rec.Width = 6500
        'Level.Rec.Height = 5490


        Level.Rec.Width = 17100
        Level.Rec.Height = 8000

        MenuItemShowHideRulers.Checked = True

        'OurHero.Rec = New Rectangle(14000, 6000, 90, 90)
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
        OurHero.Speed = 9
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


        Floor.Color = Color.FromArgb(128, 36, 0, 36)
        Floor.MapColor = Color.Purple


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

        'SetStyle(ControlStyles.FixedHeight, True)
        'SetStyle(ControlStyles.FixedWidth, True)

        WindowState = FormWindowState.Maximized

        'Load sound files
        GS.AddSound("Music", Application.StartupPath & "level_music.mp3") 'Worse - The Tower of Light from the YouTube Audio library.
        GS.AddSound("Magic", Application.StartupPath & "magic_sound.mp3")
        'GS.AddSound("Monster", Application.StartupPath & "Monster Alien Roar Aggressive.mp3")
        GS.AddSound("Undead_Move", Application.StartupPath & "undead_move.mp3")
        GS.AddSound("Undead_Attack", Application.StartupPath & "undead_attack.mp3")
        GS.AddSound("Potion_Pickup", Application.StartupPath & "potion_pickup.mp3")
        GS.AddSound("Undead_Death", Application.StartupPath & "undead_death.mp3")
        GS.AddSound("Hero_Move", Application.StartupPath & "hero_move.mp3")
        GS.AddSound("Undead_Hit", Application.StartupPath & "undead_hit.mp3")
        GS.AddSound("Hero_Death", Application.StartupPath & "hero_death.mp3")
        GS.AddSound("Not_Enough_Magic", Application.StartupPath & "not_enough_magic.mp3")

        'Set set volume 
        GS.SetVolume("Music", 150)
        GS.SetVolume("Magic", 500)
        'GS.SetVolume("Monster", 900)
        GS.SetVolume("Undead_Move", 500)
        GS.SetVolume("Undead_Attack", 400)
        GS.SetVolume("Potion_Pickup", 500)
        GS.SetVolume("Undead_Death", 300)
        GS.SetVolume("Hero_Move", 300)
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

        Timer4.Interval = 300
        Timer4.Start()

    End Sub

    Private Sub Display_Wall_Properties()

        If Walls IsNot Nothing Then

            With DataGridView1

                'create datatable And columns,
                Dim dtable = New DataTable()

                'dtable.Columns.Add(New DataColumn("Property Name"))
                'dtable.Columns.Add(New DataColumn("Property Value"))

                dtable.Columns.Add(New DataColumn("Properties"))
                dtable.Columns.Add(New DataColumn("Wall " & Selected_Wall_Index.ToString))

                'dtable.Columns.IsReadOnly = True
                dtable.Columns("Properties").ReadOnly = True

                'simple way create object for rowvalues here i have given only 2 add as per your requirement
                Dim PropertyNames(5) As String
                Dim PropertyValues(5) As String

                'assign values into row object
                PropertyNames(0) = "X"
                PropertyValues(0) = Walls(Selected_Wall_Index).Rec.X.ToString

                PropertyNames(1) = "Y"
                PropertyValues(1) = Walls(Selected_Wall_Index).Rec.Y.ToString

                PropertyNames(2) = "Width"
                PropertyValues(2) = Walls(Selected_Wall_Index).Rec.Width.ToString

                PropertyNames(3) = "Height"
                PropertyValues(3) = Walls(Selected_Wall_Index).Rec.Height.ToString

                PropertyNames(4) = "Color"
                'PropertyValues(4) = Color.Blue.ToArgb.ToString
                PropertyValues(4) = Walls(Selected_Wall_Index).Color.ToArgb.ToString

                dtable.Rows.Add(PropertyNames(0), PropertyValues(0))
                dtable.Rows.Add(PropertyNames(1), PropertyValues(1))
                dtable.Rows.Add(PropertyNames(2), PropertyValues(2))
                dtable.Rows.Add(PropertyNames(3), PropertyValues(3))
                dtable.Rows.Add(PropertyNames(4), PropertyValues(4))
                dtable.AcceptChanges()

                .DataSource = dtable

                .ColumnHeadersVisible = True




            End With

        End If

    End Sub
    Private Sub Display_Blank_Properties()

        With DataGridView1

            'create datatable And columns,
            Dim dtable = New DataTable()

            dtable.Columns.Add(New DataColumn("Properties"))
            dtable.Columns.Add(New DataColumn(" "))

            dtable.AcceptChanges()

            .DataSource = dtable

            .ColumnHeadersVisible = True

        End With

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Display timer

        'Swap Buffers
        If Swap_Buffer = True Then
            Swap_Buffer = False
        Else
            Swap_Buffer = True
        End If

        'Update the display.
        PictureBox1.Invalidate()



    End Sub

    Private Sub PictureBox1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint

        'Draw to buffer one or two?
        If Swap_Buffer = True Then
            'Draw to buffer one.

            'Create a bitmap for buffer one.
            Using Buffer1_Bitmap As New Bitmap(Viewport.Width, Viewport.Height, Imaging.PixelFormat.Format32bppPArgb)

                'Create a graphics object for the bitmap.
                Using goBuf1 As Graphics = Graphics.FromImage(Buffer1_Bitmap)

                    'Use these settings when drawing to the backbuffer.
                    With goBuf1
                        .CompositingMode = Drawing2D.CompositingMode.SourceOver 'Bug fix don't change.
                        'To fix draw string error: "Parameters not valid." I set the compositing mode to source over.
                        .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.Half
                    End With

                    'Use these settings when drawing to the screen.
                    With e.Graphics
                        .CompositingMode = Drawing2D.CompositingMode.SourceCopy
                        .SmoothingMode = Drawing2D.SmoothingMode.None
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
                    End With

                    goBuf1.Clear(Level.BackgroundColor)

                    Draw_Floors(goBuf1)

                    Draw_Floor(goBuf1, Floor.Rec)

                    Draw_Floor_Light(goBuf1)

                    Draw_Hero_Light(goBuf1, OurHero.Rec)

                    Draw_Grid(goBuf1)

                    Draw_Door(goBuf1)

                    Draw_Potion(goBuf1, Potion.Rec)

                    Draw_Walls(goBuf1)

                    Draw_Wall(goBuf1, Wall.Rec)

                    Draw_Monster(goBuf1, Monster)

                    Draw_Projectile(goBuf1)

                    Draw_Monster_Life_Bar(goBuf1)

                    Draw_Hero(goBuf1, OurHero.Rec)

                    goBuf1.DrawString("Level 1", Life_Bar_Font, drawBrush, Viewport.Width - 100, 6)

                    Draw_Map(goBuf1, 10, 68, 9)

                    Draw_HeroLife_Bar(goBuf1, Life_Bar_Frame)

                    Draw_Hero_Magic_Bar(goBuf1, Magic_Bar_Frame)

                    Draw_Instructions(goBuf1)

                    Draw_Dead_Screen(goBuf1)

                    Draw_Paused_Screen(goBuf1)

                    Draw_Movement_Target(goBuf1)

                    Draw_Magic_Target(goBuf1)

                    Draw_Projectile_Origin(goBuf1)

                    'Draw buffer to the screen.
                    e.Graphics.DrawImageUnscaled(Buffer1_Bitmap, 0, 0)

                End Using

            End Using

        Else
            'Draw to buffer two.

            'Create a bitmap for buffer two.
            Using Buffer2_Bitmap As New Bitmap(Viewport.Width, Viewport.Height, Imaging.PixelFormat.Format32bppPArgb)

                'Create a graphics object for the bitmap.
                Using goBuf2 As Graphics = Graphics.FromImage(Buffer2_Bitmap)

                    'Use these settings when drawing to the backbuffer.
                    With goBuf2
                        .CompositingMode = Drawing2D.CompositingMode.SourceOver 'Bug fix don't change.
                        'To fix draw string error: "Parameters not valid." I set the compositing mode to source over.
                        .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.Half
                    End With

                    'Use these settings when drawing to the screen.
                    With e.Graphics
                        .CompositingMode = Drawing2D.CompositingMode.SourceCopy
                        .SmoothingMode = Drawing2D.SmoothingMode.None
                        .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                        .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
                        .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
                    End With

                    goBuf2.Clear(Level.BackgroundColor)

                    Draw_Floors(goBuf2)

                    Draw_Floor(goBuf2, Floor.Rec)

                    Draw_Floor_Light(goBuf2)

                    Draw_Hero_Light(goBuf2, OurHero.Rec)

                    Draw_Grid(goBuf2)

                    Draw_Door(goBuf2)

                    Draw_Potion(goBuf2, Potion.Rec)

                    Draw_Walls(goBuf2)

                    Draw_Wall(goBuf2, Wall.Rec)

                    Draw_Monster(goBuf2, Monster)

                    Draw_Projectile(goBuf2)

                    Draw_Monster_Life_Bar(goBuf2)

                    Draw_Hero(goBuf2, OurHero.Rec)

                    goBuf2.DrawString("Level 1", Life_Bar_Font, drawBrush, Viewport.Width - 100, 6)

                    Draw_Map(goBuf2, 10, 68, 9)

                    Draw_HeroLife_Bar(goBuf2, Life_Bar_Frame)

                    Draw_Hero_Magic_Bar(goBuf2, Magic_Bar_Frame)

                    Draw_Instructions(goBuf2)

                    Draw_Dead_Screen(goBuf2)

                    Draw_Paused_Screen(goBuf2)

                    Draw_Movement_Target(goBuf2)

                    Draw_Magic_Target(goBuf2)

                    Draw_Projectile_Origin(goBuf2)

                    'Draw the buffer to the screen.
                    e.Graphics.DrawImageUnscaled(Buffer2_Bitmap, 0, 0)

                End Using

            End Using

        End If

    End Sub

    Private Sub Draw_Movement_Target(g As Graphics)

        'Transform the movement targets level coordinates into viewport coordinates.
        Dim MovementTargetInViewportCordinates As Point
        MovementTargetInViewportCordinates.X = Movement_Target.X - Viewport.X
        MovementTargetInViewportCordinates.Y = Movement_Target.Y - Viewport.Y

        'Draw Movement Target
        g.DrawRectangle(Pens.Red, New Rectangle(MovementTargetInViewportCordinates, New Drawing.Size(10, 10)))


    End Sub

    Private Sub Draw_Magic_Target(g As Graphics)

        'Transform the magic targets level coordinates into viewport coordinates.
        Dim MagicTargetInViewportCordinates As Point
        MagicTargetInViewportCordinates.X = Magic_Target.X - Viewport.X
        MagicTargetInViewportCordinates.Y = Magic_Target.Y - Viewport.Y

        'Draw magic Target
        g.DrawRectangle(Pens.Blue, New Rectangle(MagicTargetInViewportCordinates, New Drawing.Size(10, 10)))


    End Sub

    Private Sub Draw_Projectile_Center(g As Graphics)


        Dim ProjectileCenter As Point
        ProjectileCenter.X = Projectile.X + Projectile.Width \ 2
        ProjectileCenter.Y = Projectile.Y + Projectile.Height \ 2

        'Transform the magic targets level coordinates into viewport coordinates.
        Dim ProjectileCenterInViewportCordinates As Point
        ProjectileCenterInViewportCordinates.X = ProjectileCenter.X - Viewport.X
        ProjectileCenterInViewportCordinates.Y = ProjectileCenter.Y - Viewport.Y

        'Draw magic Target
        g.DrawRectangle(Pens.Green, New Rectangle(ProjectileCenterInViewportCordinates, New Drawing.Size(10, 10)))


    End Sub

    Private Sub Draw_Projectile_Origin(g As Graphics)

        'Transform the magic targets level coordinates into viewport coordinates.
        Dim ProjectileOriginInViewportCordinates As Point
        ProjectileOriginInViewportCordinates.X = Projectile_Origin.X - Viewport.X
        ProjectileOriginInViewportCordinates.Y = Projectile_Origin.Y - Viewport.Y

        'Draw magic Target
        g.DrawRectangle(Pens.YellowGreen, New Rectangle(ProjectileOriginInViewportCordinates, New Drawing.Size(10, 10)))


    End Sub

    Private Sub Draw_Paused_Screen(g As Graphics)

        If Timer2.Enabled = False Then

            g.FillRectangle(Fifty_Percent_Black_Brush, 0, 0, Viewport.Width, Viewport.Height)
            g.DrawString("Paused", PauseFont, drawBrush, Viewport.Width \ 2, Viewport.Height \ 2, Center_String)

        End If

    End Sub

    Private Sub Draw_Dead_Screen(g As Graphics)

        If OurHero.Life < 1 And Timer2.Enabled = True Then

            g.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Red)), 0, 0, Viewport.Width, Viewport.Height)
            g.DrawString("Died", PauseFont, drawBrush, Viewport.Width \ 2, Viewport.Height \ 2, Center_String)

        End If

    End Sub

    Private Sub Draw_Instructions(goBuf2 As Graphics)

        If Editor_On = False Then

            If Instructions_On = True Then

                Dim Instruction_Rec As New Rectangle(6, Viewport.Height - 60, 1000, 200)
                goBuf2.DrawString(Instruction_Text, Instruction_Font, New SolidBrush(Color.White), Instruction_Rec)

            End If

        End If

    End Sub

    Private Sub Draw_Grid(g As Graphics)

        If Editor_On = True Then

            'Draw Vertical Lines
            'Go thru the width of the level 100 pixels at a time.
            For index = 0 To Level.Rec.Width Step 100

                If index >= Viewport.X And index <= Viewport.X + Viewport.Width Then

                    'Draw grid line.
                    g.DrawLine(New Pen(Color.Cyan, 1), New Point(index - Viewport.X, Level.Rec.Y - Viewport.Y), New Point(index - Viewport.X, Level.Rec.Height - Viewport.Y))

                    If Show_Rulers = True Then

                        If index <> Level.Rec.Width Then

                            'Draw vertical position text.
                            g.DrawString(index.ToString, Life_Bar_Font, drawBrush, index - Viewport.X, Level.Rec.Y - Viewport.Y)

                        End If

                    End If

                End If

            Next

            'Draw Horizontal Lines
            'Go thru the height of the level 100 pixels at a time.
            For index = 0 To Level.Rec.Height Step 100

                If index >= Viewport.Y And index <= Viewport.Y + Viewport.Height Then

                    'Draw grid line.
                    g.DrawLine(New Pen(Color.Cyan, 1), New Point(Level.Rec.X - Viewport.X, index - Viewport.Y), New Point(Level.Rec.Width - Viewport.X, index - Viewport.Y))

                    If Show_Rulers = True Then

                        If index <> 0 And index <> Level.Rec.Height Then

                            'Draw horizontal position text.
                            g.DrawString(index.ToString, Life_Bar_Font, drawBrush, Level.Rec.X - Viewport.X, index - Viewport.Y)

                        End If

                    End If

                End If

            Next

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
            g.FillRectangle(Brushes.Black, MonsterInViewportCoordinates.X - 1, MonsterInViewportCoordinates.Y - 11, MonsterInViewportCoordinates.Width + 2, 8)
            g.FillRectangle(Brushes.Red, MonsterInViewportCoordinates.X, MonsterInViewportCoordinates.Y - 10, CInt(MonsterInViewportCoordinates.Width / Monster_LifeMAX * Monster_Life), 6)
        End If

    End Sub

    Private Sub Draw_HeroLife_Bar(g As Graphics, Bar As Rectangle)

        If Editor_On = False Then

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

            g.DrawString(OurHero.Life.ToString & "/" & OurHero.MaxLife.ToString & " Life", Life_Bar_Font, drawBrush, Life_Bar_Frame.X + Life_Bar_Frame.Width + 5, Life_Bar_Frame.Y - 4)

        End If

    End Sub

    Private Sub Draw_Hero_Magic_Bar(g As Graphics, Bar As Rectangle)

        If Editor_On = False Then

            'Draw hero magic bar frame.
            g.FillRectangle(Magic_Frame_Brush, Bar)

            'Is the heros magic points critically low?
            If OurHero.Magic >= OurHero.MaxMagic \ 4 Then
                'No, the heros magic points are not critically low.

                'Draw magic bar.
                g.FillRectangle(Magic_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)

                'Draw magic bar outline.
                g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)

            Else
                'Yes, the heros magic points are critically low.

                'Make the magic bar blink to indicate to the player that their life points are critically low.
                Select Case Magic_Blink_Counter
                'For frames 0 to 8
                    Case 0 To 8

                        'Draw magic bar.
                        g.FillRectangle(Magic_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)

                        'Draw magic bar outline.
                        g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)

                        'Advance the frame counter by one frame.
                        Magic_Blink_Counter += 1

                    'For frames 9 to 18
                    Case 9 To 18

                        'Draw magic bar with the blink color.
                        g.FillRectangle(Magic_Blink_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)

                        'Draw magic bar outline.
                        g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)

                        'Advance the frame counter by one frame.
                        Magic_Blink_Counter += 1


                    Case Else

                        'Draw magic bar with the blink color.
                        g.FillRectangle(Magic_Blink_Brush, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height)

                        'Draw magic bar outline.
                        g.DrawRectangle(Magic_Outline_Pen, Bar.X, Bar.Y, CInt((Bar.Width) / OurHero.MaxMagic * OurHero.Magic), Bar.Height - 1)

                        'Reset the frame counter.
                        Magic_Blink_Counter = 0

                End Select
            End If

            g.DrawString(OurHero.Magic.ToString & "/" & OurHero.MaxMagic.ToString & " Magic", Life_Bar_Font, drawBrush, Magic_Bar_Frame.X + Magic_Bar_Frame.Width + 5, Magic_Bar_Frame.Y - 4)

        End If

    End Sub

    Private Sub Draw_Potion(g As Graphics, Rec As Rectangle)

        If Potion.Active = True Then

            'Transform the potions level coorinates into viewport coordinates.
            Dim PotionInViewportCoordinates As Rectangle
            PotionInViewportCoordinates = Potion.Rec
            PotionInViewportCoordinates.X = Potion.Rec.X - Viewport.X
            PotionInViewportCoordinates.Y = Potion.Rec.Y - Viewport.Y

            'Is the editor off?
            If Editor_On = False Then
                'Yes, the editor is off and the game is running. - Game On

                'Is the potion in the viewport?
                If Potion.Rec.IntersectsWith(Viewport) = True Then
                    'Yes, the potion is in the viewport.

                    'Is the potion in the heros light radius?
                    If PotionInViewportCoordinates.IntersectsWith(LightRec) = True Then
                        'Yes, the potion is in the heros light radius.

                        'Draw potion hit box.
                        g.FillRectangle(New SolidBrush(Potion.Color), PotionInViewportCoordinates.X, PotionInViewportCoordinates.Y, PotionInViewportCoordinates.Width, Rec.Height)

                        'Draw Potion text.
                        g.DrawString("Potion", Monster_Font, New SolidBrush(Color.Black), PotionInViewportCoordinates, Center_String)

                        'Draw Potion outline.
                        g.DrawRectangle(New Pen(Potion.OutlineColor, 1), PotionInViewportCoordinates)

                    Else
                        'No, the potion is not in the heros light radius.

                        'Draw potion hit box.
                        g.FillRectangle(New SolidBrush(Potion.Color), PotionInViewportCoordinates.X, PotionInViewportCoordinates.Y, PotionInViewportCoordinates.Width, PotionInViewportCoordinates.Height)

                        'Draw Potion text.
                        g.DrawString("Potion", Monster_Font, New SolidBrush(Color.Black), PotionInViewportCoordinates, Center_String)

                        'Draw shadow.
                        g.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Black)), PotionInViewportCoordinates)

                    End If

                End If

            Else
                'No, the editor is on. The game is stopped. - Editor On

                'Draw potion hit box.
                g.FillRectangle(New SolidBrush(Potion.Color), PotionInViewportCoordinates.X, PotionInViewportCoordinates.Y, PotionInViewportCoordinates.Width, Rec.Height)

                'Draw Potion text.
                g.DrawString("Potion", Monster_Font, New SolidBrush(Color.Black), PotionInViewportCoordinates, Center_String)

                'Draw Potion outline.
                g.DrawRectangle(New Pen(Potion.OutlineColor, 1), PotionInViewportCoordinates)

            End If

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

        If Editor_On = True Then
            If Selected_Tool = ToolsEnum.Wall Then
                If Mouse_Down_Left = True Then

                    Dim WallInViewportCoordinates As Rectangle
                    WallInViewportCoordinates = Rec
                    WallInViewportCoordinates.X = Rec.X - Viewport.X
                    WallInViewportCoordinates.Y = Rec.Y - Viewport.Y

                    'Draw Wall
                    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
                    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

                End If
            End If
        End If

    End Sub

    Private Sub Draw_Door(g As Graphics)

        'Is the editor on?
        If Editor_On = False Then
            'No, the editor off. The game is running. - Game On

            'Transform the doors level coordinates to viewport coordinates.
            Dim DoorInViewportCoordinates As Rectangle
            DoorInViewportCoordinates = Door.Rec
            DoorInViewportCoordinates.X = Door.Rec.X - Viewport.X
            DoorInViewportCoordinates.Y = Door.Rec.Y - Viewport.Y

            'Is the door in the heros light radius?
            If DoorInViewportCoordinates.IntersectsWith(LightRec) = True Then
                'Yes, the door is in the heros light radius.

                'Is the door open?
                If Door.IsOpen = False Then
                    'No the door is closed.

                    'Draw door hit box.
                    g.FillRectangle(New SolidBrush(Door.Color), DoorInViewportCoordinates)

                    'Draw door text.
                    g.DrawString("Door", Monster_Font, New SolidBrush(Color.Black), DoorInViewportCoordinates, Center_String)

                    'Draw door outline.
                    g.DrawRectangle(New Pen(Door.OutlineColor, 1), DoorInViewportCoordinates)

                Else
                    'Yes, the door is open.

                    'Draw door hit box.
                    g.FillRectangle(New SolidBrush(Color.FromArgb(128, Door.Color)), DoorInViewportCoordinates)

                    'Draw door text.
                    'g.DrawString("Door", Monster_Font, New SolidBrush(Color.Black), DoorInViewportCoordinates, Center_String)

                    'Draw door outline.
                    'g.DrawRectangle(New Pen(Door.OutlineColor, 1), DoorInViewportCoordinates)


                End If


            Else
                'No, the door is not in the heros light radius.

                'Is the door open?
                If Door.IsOpen = False Then
                    'No the door is closed.

                    'Draw door hit box.
                    g.FillRectangle(New SolidBrush(Door.Color), DoorInViewportCoordinates)

                    'Draw door text.
                    g.DrawString("Door", Monster_Font, New SolidBrush(Color.Black), DoorInViewportCoordinates, Center_String)

                    'Draw door shadow.
                    g.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Black)), DoorInViewportCoordinates)


                Else
                    'Yes, the door is open.

                    'Draw door hit box.
                    g.FillRectangle(New SolidBrush(Color.FromArgb(128, Door.Color)), DoorInViewportCoordinates)

                    'Draw door text.
                    'g.DrawString("Door", Monster_Font, New SolidBrush(Color.Black), DoorInViewportCoordinates, Center_String)

                    'Draw door shadow.
                    'g.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.Black)), DoorInViewportCoordinates)

                End If

            End If

        Else
            'Yes, the editor is on. The game is not running. - Game Off

            'Transform the doors level coordinates to viewport coordinates.
            Dim DoorInViewportCoordinates As Rectangle
            DoorInViewportCoordinates = Door.Rec
            DoorInViewportCoordinates.X = Door.Rec.X - Viewport.X
            DoorInViewportCoordinates.Y = Door.Rec.Y - Viewport.Y

            'Draw door hit box.
            g.FillRectangle(New SolidBrush(Door.Color), DoorInViewportCoordinates)

            'Draw door text.
            g.DrawString("Door", Monster_Font, New SolidBrush(Color.Black), DoorInViewportCoordinates, Center_String)

            'Draw door outline.
            g.DrawRectangle(New Pen(Door.OutlineColor, 1), DoorInViewportCoordinates)

        End If

    End Sub

    Private Sub Draw_Walls(g As Graphics)

        'Is the editor on?
        If Editor_On = False Then
            'No, the editor off. The game is running. - Game On

            'Is there at least one wall?
            If Walls IsNot Nothing Then
                'Yes, we have at least one wall.

                'Go thur every wall in the walls array. One by one. Start to end.
                For index = 0 To UBound(Walls)

                    'Transform the wall level coorinates into viewport coordinates.
                    Dim WallInViewportCoordinates As Rectangle
                    WallInViewportCoordinates = Walls(index).Rec
                    WallInViewportCoordinates.X = Walls(index).Rec.X - Viewport.X
                    WallInViewportCoordinates.Y = Walls(index).Rec.Y - Viewport.Y

                    'Is the wall in the viewport?
                    If Walls(index).Rec.IntersectsWith(Viewport) = True Then
                        'Yes, the wall is in the viewport.

                        'Is the wall in the heros light radius?
                        If WallInViewportCoordinates.IntersectsWith(LightRec) = True Then
                            'Yes, the wall is in the heros light radius.

                            'Draw wall.
                            'g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
                            g.FillRectangle(New SolidBrush(Walls(index).Color), WallInViewportCoordinates)

                            'Draw outline.
                            g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

                        Else

                            'Draw wall.
                            'g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
                            g.FillRectangle(New SolidBrush(Walls(index).Color), WallInViewportCoordinates)

                            'Draw shadow.
                            g.FillRectangle(New SolidBrush(Color.FromArgb(200, Color.Black)), WallInViewportCoordinates)

                        End If

                    End If

                Next

            End If

        Else
            'Yes, the editor is on. The game is stopped. - Editor On

            'Is there at least one wall?
            If Walls IsNot Nothing Then
                'Yes, there is at least one wall.

                'Go thru every wall in the walls array. One by one. Start to end.
                For index = 0 To UBound(Walls)

                    'Transform the wall level coorinates into viewport coordinates.
                    Dim WallInViewportCoordinates As Rectangle
                    WallInViewportCoordinates = Walls(index).Rec
                    WallInViewportCoordinates.X = Walls(index).Rec.X - Viewport.X
                    WallInViewportCoordinates.Y = Walls(index).Rec.Y - Viewport.Y

                    'Draw wall hit box.
                    g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)

                    'Draw wall outline.
                    g.DrawRectangle(New Pen(Wall.OutlineColor, 1), WallInViewportCoordinates)

                    'Draw wall number.
                    g.DrawString(index.ToString, Monster_Font, New SolidBrush(Color.Black), WallInViewportCoordinates, Center_String)

                Next

            End If


            If IsWallSelected = True Then

                'Transform the walls level coorinates into viewport coordinates.
                Dim WallInViewportCoordinates As Rectangle
                WallInViewportCoordinates = Walls(Selected_Wall_Index).Rec
                WallInViewportCoordinates.X = Walls(Selected_Wall_Index).Rec.X - Viewport.X
                WallInViewportCoordinates.Y = Walls(Selected_Wall_Index).Rec.Y - Viewport.Y

                'Draw selection outline.
                g.DrawRectangle(New Pen(Color.White, 5), WallInViewportCoordinates)

                'Draw the walls outline.
                Dim Outline_Pen As New Pen(Color.Red, 2)
                Outline_Pen.DashStyle = DashStyle.Dash
                g.DrawRectangle(Outline_Pen, WallInViewportCoordinates)

                'Draw the walls top-left control handle.
                g.FillEllipse(Brushes.Red, WallInViewportCoordinates.X - 20 \ 2, WallInViewportCoordinates.Y - 20 \ 2, 20, 20)

                'Draw the walls bottom-right control handle.
                g.FillEllipse(Brushes.Red, WallInViewportCoordinates.Right - 20 \ 2, WallInViewportCoordinates.Bottom - 20 \ 2, 20, 20)

                'Draw the walls X and Y coordinates text.
                Dim PositionString As String = Walls(Selected_Wall_Index).Rec.X.ToString & ", " & Walls(Selected_Wall_Index).Rec.Y.ToString
                g.DrawString(PositionString, Monster_Font, New SolidBrush(Color.White), New Point(WallInViewportCoordinates.X, WallInViewportCoordinates.Y - 20), Center_String)

                'Draw the walls width and height text.
                Dim SizeString As String = Walls(Selected_Wall_Index).Rec.Width.ToString & ", " & Walls(Selected_Wall_Index).Rec.Height.ToString
                g.DrawString(SizeString, Monster_Font, New SolidBrush(Color.White), New Point(WallInViewportCoordinates.Right, WallInViewportCoordinates.Bottom + 45), Center_String)

            End If

        End If

    End Sub

    Private Sub Draw_Floor_Light(g As Graphics)

        'Is the editor off?
        'If Editor_On = False Then
        'Yes, the editor is off.

        'Transform the floor level coorinates into viewport coordinates.
        Dim FloorLightInViewportCoordinates As Rectangle

        FloorLightInViewportCoordinates.X = 150 - Viewport.X

        FloorLightInViewportCoordinates.Y = 150 - Viewport.Y

        FloorLightInViewportCoordinates.Width = 300

        FloorLightInViewportCoordinates.Height = 300


        Dim LightColor As Color = Color.FromArgb(64, Color.White)


        'Create a graphics path.
        Dim path As New GraphicsPath()

        'Add an ellipse the size and position of the heros light rectangle to the path.
        path.AddEllipse(FloorLightInViewportCoordinates)

        'Create a path gradient brush
        Dim pgBrush As New PathGradientBrush(path)

        'Set the center color of the path gradient brush.
        pgBrush.CenterColor = LightColor

        'Set the surrounding colors of the path gradient brush.
        Dim list As Color() = New Color() {Color.FromArgb(0, Color.White), Color.FromArgb(0, Color.White), Color.FromArgb(0, Color.White)}
        pgBrush.SurroundColors = list

        'Draw the heros light radius.
        g.FillPath(pgBrush, path)



        'End If

    End Sub

    Private Sub Draw_Floor(g As Graphics, Rec As Rectangle)

        'Is the editor on?
        If Editor_On = True Then
            'Yes, the editor is on. The game is stopped. - Editor On

            If Selected_Tool = ToolsEnum.Floor Then
                If Mouse_Down_Left = True Then

                    'Transform the floors level coorinates into viewport coordinates.
                    Dim FloorInViewportCoordinates As Rectangle
                    FloorInViewportCoordinates = Rec
                    FloorInViewportCoordinates.X = Rec.X - Viewport.X
                    FloorInViewportCoordinates.Y = Rec.Y - Viewport.Y

                    'Draw floor.
                    g.FillRectangle(New SolidBrush(Floor.Color), FloorInViewportCoordinates)

                End If
            End If

        End If

    End Sub

    Private Sub Draw_Floors(g As Graphics)

        'Is the editor on?
        If Editor_On = False Then
            'No, the editor off. The game is running. - Game On

            'Is there at least one floor?
            If Floors IsNot Nothing Then
                'Yes, we have at least one floor.

                'Go thur every floor in the floors array. One by one. Start to end.
                For index = 0 To UBound(Floors)

                    'Transform the floor level coorinates into viewport coordinates.
                    Dim FloorInViewportCoordinates As Rectangle
                    FloorInViewportCoordinates.X = Floors(index).Rec.X - Viewport.X
                    FloorInViewportCoordinates.Y = Floors(index).Rec.Y - Viewport.Y
                    FloorInViewportCoordinates.Width = Floors(index).Rec.Width
                    FloorInViewportCoordinates.Height = Floors(index).Rec.Height

                    'Is the floor in the viewport?
                    If Floors(index).Rec.IntersectsWith(Viewport) = True Then
                        'Yes, the floor is in the viewport.

                        'Draw floor.
                        'g.FillRectangle(New SolidBrush(Wall.Color), WallInViewportCoordinates)
                        g.FillRectangle(New SolidBrush(Floors(index).Color), FloorInViewportCoordinates)

                    End If

                Next

            End If

        Else
            'Yes, the editor is on. The game is stopped. - Editor On

            'Is there at least one floor?
            If Floors IsNot Nothing Then
                'Yes, there is at least one floor.

                'Go thru every floor in the floors array. One by one. Start to end.
                For index = 0 To UBound(Floors)

                    'Transform the floor level coorinates into viewport coordinates.
                    Dim FloorInViewportCoordinates As Rectangle
                    FloorInViewportCoordinates = Floors(index).Rec
                    FloorInViewportCoordinates.X = Floors(index).Rec.X - Viewport.X
                    FloorInViewportCoordinates.Y = Floors(index).Rec.Y - Viewport.Y

                    'Draw floor hit box.
                    g.FillRectangle(New SolidBrush(Floor.Color), FloorInViewportCoordinates)

                    'Draw floor outline.
                    g.DrawRectangle(New Pen(Floor.OutlineColor, 1), FloorInViewportCoordinates)

                    'Draw floor number.
                    g.DrawString(index.ToString, Monster_Font, New SolidBrush(Color.Black), FloorInViewportCoordinates, Center_String)

                Next

            End If

            If IsFloorSelected = True Then

                'Transform the floor level coorinates into viewport coordinates.
                Dim FloorInViewportCoordinates As Rectangle
                FloorInViewportCoordinates = Floors(Selected_Floor_Index).Rec
                FloorInViewportCoordinates.X = Floors(Selected_Floor_Index).Rec.X - Viewport.X
                FloorInViewportCoordinates.Y = Floors(Selected_Floor_Index).Rec.Y - Viewport.Y

                'Draw selection outline.
                g.DrawRectangle(New Pen(Color.White, 5), FloorInViewportCoordinates)

                'Draw the floors outline.
                Dim Outline_Pen As New Pen(Color.Red, 2)
                Outline_Pen.DashStyle = DashStyle.Dash
                g.DrawRectangle(Outline_Pen, FloorInViewportCoordinates)

                'Draw the floors top-left control handle.
                g.FillEllipse(Brushes.Red, FloorInViewportCoordinates.X - 20 \ 2, FloorInViewportCoordinates.Y - 20 \ 2, 20, 20)

                'Draw the floors bottom-right control handle.
                g.FillEllipse(Brushes.Red, FloorInViewportCoordinates.Right - 20 \ 2, FloorInViewportCoordinates.Bottom - 20 \ 2, 20, 20)

                'Draw the floors X and Y coordinates text.
                Dim PositionString As String = Floors(Selected_Floor_Index).Rec.X.ToString & ", " & Floors(Selected_Floor_Index).Rec.Y.ToString
                g.DrawString(PositionString, Monster_Font, New SolidBrush(Color.White), New Point(FloorInViewportCoordinates.X, FloorInViewportCoordinates.Y - 20), Center_String)

                'Draw the floors width and height text.
                Dim SizeString As String = Floors(Selected_Floor_Index).Rec.Width.ToString & ", " & Floors(Selected_Floor_Index).Rec.Height.ToString
                g.DrawString(SizeString, Monster_Font, New SolidBrush(Color.White), New Point(FloorInViewportCoordinates.Right, FloorInViewportCoordinates.Bottom + 45), Center_String)

            End If

        End If

    End Sub

    Private Sub Draw_Hero_Light(g As Graphics, Rec As Rectangle)

        'Is the editor off?
        If Editor_On = False Then
            'Yes, the editor is off.

            'Transform the heros level coorinates into viewport coordinates.
            Dim HeroInViewportCoordinates As Rectangle
            HeroInViewportCoordinates = Rec
            HeroInViewportCoordinates.X = Rec.X - Viewport.X
            HeroInViewportCoordinates.Y = Rec.Y - Viewport.Y

            'Is the player casting magic?
            If ProjectileInflight = True Then
                'Yes, the hero is casting magic.

                'Set the heros light rectangle to the size and position of the heros rectangle.
                LightRec = HeroInViewportCoordinates

                'Set the heros light rectangle to the size and position of the heros rectangle.
                LightRec.Inflate(500, 500)

                'Create a graphics path.
                Dim path As New GraphicsPath()

                'Add an ellipse the size and position of the heros light rectangle to the path.
                path.AddEllipse(LightRec)

                'Create a path gradient brush
                Dim pgBrush As New PathGradientBrush(path)

                'Set the center color of the path gradient brush.
                pgBrush.CenterColor = Color.FromArgb(255, Color.White)

                'Set the surrounding colors of the path gradient brush.
                Dim list As Color() = New Color() {Color.FromArgb(0, Color.White), Color.FromArgb(0, Color.White), Color.FromArgb(0, Color.White)}
                pgBrush.SurroundColors = list

                'Draw the heros light radius.
                g.FillPath(pgBrush, path)

            Else
                'No, the player is not casting magic.

                'Set the heros light rectangle to the size and position of the heros rectangle.
                LightRec = HeroInViewportCoordinates

                'Set the heros light rectangle to the size and position of the heros rectangle.
                LightRec.Inflate(400, 400)

                'Create a graphics path.
                Dim path As New GraphicsPath()

                'Add an ellipse the size and position of the heros light rectangle to the path.
                path.AddEllipse(LightRec)

                'Create a path gradient brush.
                Dim pgBrush As New PathGradientBrush(path)

                'Set the center color of the path gradient brush.
                pgBrush.CenterColor = Color.FromArgb(90, Color.White)

                'Set the surrounding colors of the path gradient brush.
                Dim list As Color() = New Color() {Color.FromArgb(0, Color.White), Color.FromArgb(0, Color.White), Color.FromArgb(0, Color.White)}
                pgBrush.SurroundColors = list

                'Draw the heros light radius.
                g.FillPath(pgBrush, path)

            End If

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


            If Splitter1.SplitPosition < Me.ClientSize.Width - 35 Then

                Splitter1.SplitPosition = Me.ClientSize.Width - 35

            End If

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

            'Is the left mouse button down?
            If Mouse_Down_Left = True Then
                'MousePosition.Offset(Me.Location.X - PictureBox1.Location.X, Me.Location.Y - PictureBox1.Location.Y)

                Dim MouseInClientCoordinates As Point = PictureBox1.PointToClient(MousePosition)
                Dim MouseXInVieportCoordinates As Integer = MouseInClientCoordinates.X + Viewport.X
                Dim MouseYInVieportCoordinates As Integer = MouseInClientCoordinates.Y + Viewport.Y


                Movement_Target.X = MouseXInVieportCoordinates
                Movement_Target.Y = MouseYInVieportCoordinates

                'Keep target on the level.
                If Movement_Target.X < Level.Rec.X Then
                    Movement_Target.X = Level.Rec.X
                End If
                If Movement_Target.X > Level.Rec.Width Then
                    Movement_Target.X = Level.Rec.Width
                End If
                If Movement_Target.Y < Level.Rec.Y Then
                    Movement_Target.Y = Level.Rec.Y
                End If
                If Movement_Target.Y > Level.Rec.Height Then
                    Movement_Target.Y = Level.Rec.Height
                End If

            End If

            Dim Hero_Center_X As Integer = OurHero.Rec.X + OurHero.Rec.Width \ 2

            'Is the hero centered with the movement target horizontally?
            If Hero_Center_X <> Movement_Target.X Then
                'No, the hero is not centered horizontally with the movement target.

                'Is the hero to the right of the movement target?
                If Hero_Center_X > Movement_Target.X Then
                    'Yes, the hero is to the right of the movement target.

                    Move_Hero_Left()

                Else
                    'No, the hero is to the left of the movement target.

                    Move_Hero_Right()

                End If

            End If

            Dim Hero_Center_Y As Integer = OurHero.Rec.Y + OurHero.Rec.Height \ 2

            'Is the hero centered with the movement target vertically?
            If Hero_Center_Y <> Movement_Target.Y Then
                'No, the hero is not centered vertically with the movement target.

                'Is the hero below the movement target?
                If Hero_Center_Y > Movement_Target.Y Then
                    'Yes, the hero is below the movement target.

                    Move_Hero_Up()

                Else
                    'No, the hero is above the movement target.

                    Move_Hero_Down()

                End If

            End If


            'If MoveLeft = True Then

            '    Move_Hero_Left()

            'End If

            'If MoveRight = True Then

            '    Move_Hero_Right()

            'End If

            'If MoveUp = True Then

            '    Move_Hero_Up()

            'End If

            'If MoveDown = True Then

            '    Move_Hero_Down()

            'End If

            Do_Hero_Shots()

            Do_Potion_Pickup()

            If OurHero.Rec.IntersectsWith(Door.Rec) = True Then

                Door.IsOpen = True

            End If

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

        'Dim Monster_Center_X As Integer = Monster.X + Monster.Width \ 2
        Dim Hero_Center_X As Integer = OurHero.Rec.X + OurHero.Rec.Width \ 2

        'Move Hero Right **************************************************************
        'Proximity based speed controller. - Fixes Bug: The hero sometimes oscillates.
        'Is the hero close to the movement target?
        If Horizontal_Distance(Hero_Center_X, Movement_Target.X) > 8 Then
            'No, the hero is not close to the movement target.

            'Move the hero to the right fast.
            OurHero.Rec.X += OurHero.Speed

        Else
            'Yes, the hero is close to the movement target.

            'Move the hero to the right slowly.
            OurHero.Rec.X += 1

        End If

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

        ''Fallow the hero.
        ''Is the hero about to walk off screen?
        'If OurHero.Rec.X > Viewport.X + Viewport.Width - OurHero.Rec.Width * 4 Then
        '    'Yes, the hero is about to walk off screen.


        '    If Viewport.X < Level.Rec.Width - Viewport.Width Then
        '        'Move viewport to the right.
        '        Viewport.X += OurHero.Speed
        '    Else

        '        Viewport.X = Level.Rec.Width - Viewport.Width

        '    End If

        '    If OurHero.Rec.X > Level.Rec.Width - OurHero.Rec.Width Then

        '        OurHero.Rec.X = Level.Rec.Width - OurHero.Rec.Width

        '    End If
        'End If


        Center_Viewport_on_the_Hero()

        'Center viewport on the hero.
        'Viewport.X = OurHero.Rec.X - Viewport.Width \ 2
        'Viewport.Y = OurHero.Rec.Y - Viewport.Height \ 2

        ''Keep viewport on the level.
        'If Viewport.X < Level.Rec.X Then
        '    Viewport.X = Level.Rec.X
        'End If
        'If Viewport.X + Viewport.Width > Level.Rec.Width Then
        '    Viewport.X = Level.Rec.Width - Viewport.Width
        'End If
        'If Viewport.Y < Level.Rec.Y Then
        '    Viewport.Y = Level.Rec.Y
        'End If
        'If Viewport.Y + Viewport.Height > Level.Rec.Height Then
        '    Viewport.Y = Level.Rec.Height - Viewport.Height
        'End If

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

        'MoveHero(DirectionEnum.Left)

        Dim Hero_Center_X As Integer = OurHero.Rec.X + OurHero.Rec.Width \ 2

        'Move Hero Left **************************************************************
        'Proximity based speed controller. - Fixes Bug: The hero sometimes oscillates.
        'Is the hero close to the movement target?
        If Horizontal_Distance(Hero_Center_X, Movement_Target.X) > 8 Then
            'No, the hero is not close to the movement target.

            'Move the hero to the left fast.
            OurHero.Rec.X -= OurHero.Speed

        Else
            'Yes, the hero is close to the movement target.

            'Move the hero to the left slowly.
            OurHero.Rec.X -= 1

        End If

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

        ''Fallow the hero.
        ''Is the hero about to walk off screen?
        'If OurHero.Rec.X < Viewport.X + OurHero.Rec.Width * 4 Then
        '    'Yes, the hero is about to walk off screen.

        '    If Viewport.X > Level.Rec.X Then

        '        'Move viewport to the right.
        '        Viewport.X -= OurHero.Speed

        '    Else

        '        Viewport.X = Level.Rec.X

        '    End If

        '    If OurHero.Rec.X < Level.Rec.X Then

        '        OurHero.Rec.X = Level.Rec.X

        '    End If

        'End If

        'Center viewport on the hero.

        Center_Viewport_on_the_Hero()


        'Viewport.X = OurHero.Rec.X - Viewport.Width \ 2
        'Viewport.Y = OurHero.Rec.Y - Viewport.Height \ 2

        ''Keep viewport on the level.
        'If Viewport.X < Level.Rec.X Then
        '    Viewport.X = Level.Rec.X
        'End If
        'If Viewport.X + Viewport.Width > Level.Rec.Width Then
        '    Viewport.X = Level.Rec.Width - Viewport.Width
        'End If
        'If Viewport.Y < Level.Rec.Y Then
        '    Viewport.Y = Level.Rec.Y
        'End If
        'If Viewport.Y + Viewport.Height > Level.Rec.Height Then
        '    Viewport.Y = Level.Rec.Height - Viewport.Height
        'End If

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

        'MoveHero(DirectionEnum.Up)

        Dim Hero_Center_Y As Integer = OurHero.Rec.Y + OurHero.Rec.Height \ 2

        'Move Hero Up **************************************************************
        'Proximity based speed controller. - Fixes Bug: The hero sometimes oscillates.
        'Is the hero close to the movement target?
        If Vertical_Distance(Hero_Center_Y, Movement_Target.Y) > 8 Then
            'No, the hero is not close to the movement target.

            'Move the hero up fast.
            OurHero.Rec.Y -= OurHero.Speed

        Else
            'Yes, the hero is close to the movement target.

            'Move the hero up slowly.
            OurHero.Rec.Y -= 1

        End If



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

                    'Play monster hit sound.
                    If GS.IsPlaying("Undead_Hit") = False Then
                        GS.Play("Undead_Hit")
                    End If

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

                                'Push the monster below the wall.
                                Monster.Y = Walls(index).Rec.Y + (Walls(index).Rec.Height + 1)


                                'Knock the hero below the monster.
                                OurHero.Rec.Y = Monster.Y + Monster.Height + 16

                                'Movement_Target.Y = Monster.Y + Monster.Height - 16


                            End If

                        Next
                    End If
                    '************************************************

                End If
            End If
        End If

        ''Fallow the hero.
        ''Is the hero about to walk off screen?
        'If OurHero.Rec.Y < Viewport.Y + OurHero.Rec.Height * 2 Then
        '    'Yes, the hero is about to walk off screen.

        '    If Viewport.Y > Level.Rec.Y Then

        '        'Move viewport up.
        '        Viewport.Y -= OurHero.Speed

        '    Else

        '        Viewport.Y = Level.Rec.Y

        '    End If

        '    If OurHero.Rec.Y < Level.Rec.Y Then

        '        OurHero.Rec.Y = Level.Rec.Y

        '    End If

        'End If

        'Center viewport on the hero.

        Center_Viewport_on_the_Hero()

        ''Keep viewport on the level.
        'If Viewport.X < Level.Rec.X Then
        '    Viewport.X = Level.Rec.X
        'End If
        'If Viewport.X + Viewport.Width > Level.Rec.Width Then
        '    Viewport.X = Level.Rec.Width - Viewport.Width
        'End If
        'If Viewport.Y < Level.Rec.Y Then
        '    Viewport.Y = Level.Rec.Y
        'End If
        'If Viewport.Y + Viewport.Height > Level.Rec.Height Then
        '    Viewport.Y = Level.Rec.Height - Viewport.Height
        'End If

    End Sub

    Private Sub Center_Viewport_on_the_Hero()

        Dim HeroCenterX As Integer = OurHero.Rec.X + (OurHero.Rec.Width \ 2)
        Dim HeroCenterY As Integer = OurHero.Rec.Y + (OurHero.Rec.Height \ 2)

        Viewport.X = HeroCenterX - (Viewport.Width \ 2)
        Viewport.Y = HeroCenterY - (Viewport.Height \ 2)


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

        'MoveHero(DirectionEnum.Down)

        Dim Hero_Center_Y As Integer = OurHero.Rec.Y + OurHero.Rec.Height \ 2

        'Move Hero Down **************************************************************
        'Proximity based speed controller. - Fixes Bug: The hero sometimes oscillates.
        'Is the hero close to the movement target?
        If Vertical_Distance(Hero_Center_Y, Movement_Target.Y) > 8 Then
            'No, the hero is not close to the movement target.

            'Move the hero down fast.
            OurHero.Rec.Y += OurHero.Speed

        Else
            'Yes, the hero is close to the movement target.

            'Move the hero down slowly.
            OurHero.Rec.Y += 1

        End If

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

        ''Fallow the hero.
        ''Is the hero about to walk off screen?
        'If OurHero.Rec.Y > Viewport.Y + Viewport.Height - OurHero.Rec.Height * 3 Then
        '    'Yes, the hero is about to walk off screen.

        '    If Viewport.Y < Level.Rec.Height - Viewport.Height Then
        '        'Move viewport to the right.
        '        Viewport.Y += OurHero.Speed
        '    Else

        '        Viewport.Y = Level.Rec.Height - Viewport.Height

        '    End If

        '    If OurHero.Rec.Y > Level.Rec.Height - OurHero.Rec.Height Then

        '        OurHero.Rec.Y = Level.Rec.Height - OurHero.Rec.Height

        '    End If

        'End If

        'Center viewport on the hero.

        Center_Viewport_on_the_Hero()


        'Viewport.X = OurHero.Rec.X - Viewport.Width \ 2
        'Viewport.Y = OurHero.Rec.Y - Viewport.Height \ 2

        ''Keep viewport on the level.
        'If Viewport.X < Level.Rec.X Then
        '    Viewport.X = Level.Rec.X
        'End If
        'If Viewport.X + Viewport.Width > Level.Rec.Width Then
        '    Viewport.X = Level.Rec.Width - Viewport.Width
        'End If
        'If Viewport.Y < Level.Rec.Y Then
        '    Viewport.Y = Level.Rec.Y
        'End If
        'If Viewport.Y + Viewport.Height > Level.Rec.Height Then
        '    Viewport.Y = Level.Rec.Height - Viewport.Height
        'End If

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


            DistanceToHero = CInt(Distance_Between_Points(Monster.Location, OurHero.Rec.Location))

            'Proximity Based Chase Behavior
            'Is the monster near the hero?
            If Distance_Between_Points(Monster.Location, OurHero.Rec.Location) < 1100 Then
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

                    Potion.Color = Color.FromArgb(255, 119, 9, 14)
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
        If OurHero.Magic >= 15 Then
            'Yes, the hero has enough magic to cast.

            'Does the player want to cast a spell and isn't already casting a spell.
            If Mouse_Down_Right = True And ProjectileInflight = False Then
                'Yes, the player wants to cast a spell and isn't already casting a spell.

                Dim HeroCenter As Point
                HeroCenter.X = OurHero.Rec.X + OurHero.Rec.Width \ 2
                HeroCenter.Y = OurHero.Rec.Y + OurHero.Rec.Height \ 2

                'Position the projectile under the hero. Make the projectile the same size as the hero.
                Projectile = OurHero.Rec

                Projectile_Origin = HeroCenter

                'Find casting angle.
                Dim radians As Double = Math.Atan2((Projectile_Origin.Y - Magic_Target.Y), (Magic_Target.X - Projectile_Origin.X))
                Dim Degrees As Integer = CInt(radians * 180 / Math.PI) 'Angles on the unit circle.
                If Degrees < 0 Then                                    '     90
                    Degrees += 360                                     '180     0/360
                End If                                                 '    270

                'Determine the hero's direction of fire.**************************
                'Fire Right - 0° {0° - 22°, 338° - 360°}
                If Degrees >= 0 And Degrees <= 22 Or Degrees >= 338 And Degrees <= 360 Then
                    Projectile_Direction = DirectionEnum.Right
                End If
                'Fire Right Up - 45° {23° - 67°}
                If Degrees >= 23 And Degrees <= 67 Then
                    Projectile_Direction = DirectionEnum.RightUP
                End If
                'Fire Up - 90° {68° - 112°}
                If Degrees >= 68 And Degrees <= 112 Then
                    Projectile_Direction = DirectionEnum.Up
                End If
                'Fire Left Up - 135° {113° - 157°}
                If Degrees >= 113 And Degrees <= 157 Then
                    Projectile_Direction = DirectionEnum.LeftUp
                End If
                'Fire Left - 180° {158° - 202°}
                If Degrees >= 158 And Degrees <= 202 Then
                    Projectile_Direction = DirectionEnum.Left
                End If
                'Fire Left Down - 225° {203° - 247°}
                If Degrees >= 203 And Degrees <= 247 Then
                    Projectile_Direction = DirectionEnum.LeftDown
                End If
                'Fire Down - 270° {248° - 292°}
                If Degrees >= 248 And Degrees <= 292 Then
                    Projectile_Direction = DirectionEnum.Down
                End If
                'Fire Right Down - 315° {293° - 337°}
                If Degrees >= 293 And Degrees <= 337 Then
                    Projectile_Direction = DirectionEnum.RightDown
                End If

                'Loose the projectile.
                ProjectileInflight = True

                'Subtract the cost of magic.
                OurHero.Magic -= 15
                If OurHero.Magic < 1 Then
                    OurHero.Magic = 0
                End If

                'Play projectile in flight sound.
                GS.Play("Magic")

            End If
        Else
            'No, the hero doesn't have enough magic to cast.

            'Does the player want to cast a spell?
            If Mouse_Down_Right = True Then
                'If MoveLeft = True Or MoveRight = True Or MoveUp = True Or MoveDown = True Then
                'Yes, the player wants to cast a spell.

                'Play not enough magic dialogue.
                If GS.IsPlaying("Not_Enough_Magic") = False Then
                    GS.Play("Not_Enough_Magic")
                End If

            End If

        End If

        'Move projectile and attack the monster when hit.*****************************************************************************
        'Has the player cast a spell?
        If ProjectileInflight = True Then
            'Yes the player has cast a spell.

            Dim ProjectileCenter As Point
            ProjectileCenter.X = Projectile.X + Projectile.Width \ 2
            ProjectileCenter.Y = Projectile.Y + Projectile.Height \ 2

            'Is the projectile within it's range?
            If Distance_Between_Points(ProjectileCenter, Projectile_Origin) < Projectile_Max_Distance Then
                'Yes, the projectile is within it's range.

                'What direction is the player casting in?
                Select Case Projectile_Direction
                    Case DirectionEnum.Right
                        'The player is casting to the right.

                        'Move the projectile to the right.
                        Projectile.X += Projectile_Speed

                        'ToDo: 'Go thur the monsters one by one, start to end.
                        'ToDo: If Monsters IsNot Nothing Then
                        'ToDo: For MonIndex = 0 To UBound(Monsters)

                        'Is the monster alive?
                        If Monster_Life > 0 Then 'ToDo: If Monsters(MonIndex).Life > 0 Then
                            'Yes, the monster is alive.

                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then 'ToDo: If Projectile.IntersectsWith(Monsters(MonIndex).Rec) = True Then
                                'Yes, the hero is touching the monster.

                                Monster_Hit = True 'ToDo: Monsters(MonIndex).Hit = True

                                'Play monster hit sound.
                                If GS.IsPlaying("Undead_Hit") = False Then
                                    GS.Play("Undead_Hit")
                                End If

                                'Attack the monsters life points directly.
                                Monster_Life -= Projectile_Attack 'ToDo: Monsters(MonIndex).Life -= Projectile_Attack
                                If Monster_Life < 0 Then
                                    Monster_Life = 0
                                End If

                                'Knock monster to the right of hero.
                                Monster.X += CInt(Projectile_Speed / 3) 'ToDo: Monsters(MonIndex).X += CInt(Projectile_Speed / 3)

                                'Wall Collision Handler - Moving Right*****************
                                If Walls IsNot Nothing Then
                                    For index = 0 To UBound(Walls)

                                        'Is the monster touching the wall?
                                        If Monster.IntersectsWith(Walls(index).Rec) = True Then 'ToDo: If Monsters(MonIndex).IntersectsWith(Walls(index).Rec) = True Then
                                            'Yes, the monster is touching the wall.

                                            'Push monster to the left of wall.
                                            Monster.X = Walls(index).Rec.X - Monster.Width - 1 'ToDo: Monsters(MonIndex).X = Walls(index).Rec.X - Monsters(MonIndex).Width - 1

                                        End If
                                    Next
                                End If
                                '************************************************

                            End If
                        End If
                    'ToDo: Next
                    'ToDo: End if

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

                                'Play monster hit sound.
                                If GS.IsPlaying("Undead_Hit") = False Then
                                    GS.Play("Undead_Hit")
                                End If

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

                                'Play monster hit sound.
                                If GS.IsPlaying("Undead_Hit") = False Then
                                    GS.Play("Undead_Hit")
                                End If

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

                                'Play monster hit sound.
                                If GS.IsPlaying("Undead_Hit") = False Then
                                    GS.Play("Undead_Hit")
                                End If

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

                                'Play monster hit sound.
                                If GS.IsPlaying("Undead_Hit") = False Then
                                    GS.Play("Undead_Hit")
                                End If

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

                                'Play monster hit sound.
                                If GS.IsPlaying("Undead_Hit") = False Then
                                    GS.Play("Undead_Hit")
                                End If


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

                                'Play monster hit sound.
                                If GS.IsPlaying("Undead_Hit") = False Then
                                    GS.Play("Undead_Hit")
                                End If

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

                        'ToDo: 'Go thur the monsters one by one, start to end.
                        'ToDo: If Monsters IsNot Nothing Then
                        'ToDo: For MonIndex = 0 To UBound(Monsters)

                        'Is the monster alive?
                        If Monster_Life > 0 Then 'ToDo: If Monsters(MonIndex).Life > 0 Then
                            'Yes, the monster is alive.

                            'Is the projectile touching the monster?
                            If Projectile.IntersectsWith(Monster) = True Then 'ToDo: If Projectile.IntersectsWith(Monsters(MonIndex).Rec) = True Then
                                'Yes, the hero is touching the monster.

                                Monster_Hit = True 'ToDo: Monsters(MonIndex).Hit = True

                                'Play monster hit sound.
                                If GS.IsPlaying("Undead_Hit") = False Then
                                    GS.Play("Undead_Hit")
                                End If

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
                'No, the projectile is beyond it's range.

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

                If IsWallSelected = True Then

                    Walls(Selected_Wall_Index).Rec.X -= 1

                    'Display_Wall_Properties()

                    e.Handled = True

                    Update_Properties = True

                Else

                    MoveViewport(DirectionEnum.Left)

                    e.Handled = True

                End If



            End If

        End If

        If e.KeyValue = Keys.Right Then

            If Editor_On = False Then

                MoveRight = True


            Else

                If IsWallSelected = True Then

                    Walls(Selected_Wall_Index).Rec.X += 1

                    e.Handled = True

                    'Display_Wall_Properties()

                    Update_Properties = True

                Else

                    MoveViewport(DirectionEnum.Right)

                    e.Handled = True

                End If

            End If

        End If

        If e.KeyValue = Keys.Up Then
            If Editor_On = False Then
                MoveUp = True
            Else

                If IsWallSelected = True Then

                    Walls(Selected_Wall_Index).Rec.Y -= 1

                    e.Handled = True

                    'Display_Wall_Properties()

                    Update_Properties = True

                Else
                    MoveViewport(DirectionEnum.Up)

                    e.Handled = True

                End If

            End If
        End If

        If e.KeyValue = Keys.Down Then
            If Editor_On = False Then
                MoveDown = True
            Else
                If IsWallSelected = True Then

                    Walls(Selected_Wall_Index).Rec.Y += 1

                    e.Handled = True

                    'Display_Wall_Properties()

                    Update_Properties = True

                Else

                    MoveViewport(DirectionEnum.Down)

                    e.Handled = True

                End If

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
                    If IsWallSelected = True Then

                        Remove_Wall(Selected_Wall_Index)

                        IsWallSelected = False

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

        Center_Viewport_on_the_Hero()


    End Sub

    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        Me.Form1_Resize(sender, e)

        CtrlDown = False

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

            ''Keep viewport on the level.
            'If Viewport.X < Level.Rec.X Then
            '    Viewport.X = Level.Rec.X
            'End If
            'If Viewport.X + Viewport.Width > Level.Rec.Width Then
            '    Viewport.X = Level.Rec.Width - Viewport.Width
            'End If
            'If Viewport.Y < Level.Rec.Y Then
            '    Viewport.Y = Level.Rec.Y
            'End If
            'If Viewport.Y + Viewport.Height > Level.Rec.Height Then
            '    Viewport.Y = Level.Rec.Height - Viewport.Height
            'End If

        End If

    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown



        'PictureBox1.Focus()

        'Me.Focus()

        'Me.DataGridView1.ClearSelection()

        '.ClearSelection()

        If Editor_On = True Then

            'Did the user push down the left mouse button?
            If e.Button = MouseButtons.Left Then
                'Yes, the user pushed the left mouse button down.

                'Pointer**********************************************************************************************
                'Is the pointer the selected tool?
                If Selected_Tool = ToolsEnum.Pointer Then
                    'Yes, the pointer is the selected tool.

                    'Wall Selection**********************************************************************************************
                    'Has a wall been selected?
                    If IsWallSelected = False Then
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

                                    Selected_Wall_Index = Index
                                    IsWallSelected = True
                                    Pointer_Offset.X = Pointer_Origin.X - Walls(Selected_Wall_Index).Rec.X
                                    Pointer_Offset.Y = Pointer_Origin.Y - Walls(Selected_Wall_Index).Rec.Y

                                    Update_Properties = True

                                    Exit For

                                End If
                                IsWallSelected = False
                                Selected_Wall_Index = -1
                            Next
                        End If

                    Else
                        'Yes, a wall is selected.

                        'Control Handle Selection
                        Dim MousePointerRec As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)
                        Dim TopLeftControlHandleRec As New Rectangle(Walls(Selected_Wall_Index).Rec.X - 10, Walls(Selected_Wall_Index).Rec.Y - 10, 20, 20)
                        Dim BottomRightControlHandleRec As New Rectangle(Walls(Selected_Wall_Index).Rec.Right - 10, Walls(Selected_Wall_Index).Rec.Bottom - 10, 20, 20)

                        'Is the user selecting the top-left control handle?
                        If MousePointerRec.IntersectsWith(TopLeftControlHandleRec) = True Then
                            'Yes, the user is selecting the top-left control handle.

                            TopLeftControlHandle_Selected = True

                            Wall_Origin.X = Walls(Selected_Wall_Index).Rec.Right
                            Wall_Origin.Y = Walls(Selected_Wall_Index).Rec.Bottom

                        Else
                            'No, the user is not selecting the top-left control handle.

                            TopLeftControlHandle_Selected = False

                        End If

                        'Is the user selecting the botton-right control handle?
                        If MousePointerRec.IntersectsWith(BottomRightControlHandleRec) = True Then
                            'Yes, the user is selecting the botton-right control handle.

                            BottomRightControlHandle_Selected = True

                            Wall_Origin.X = Walls(Selected_Wall_Index).Rec.X
                            Wall_Origin.Y = Walls(Selected_Wall_Index).Rec.Y

                        Else
                            'No, the user is not selecting the botton-right control handle.

                            BottomRightControlHandle_Selected = False

                        End If

                        If TopLeftControlHandle_Selected = False And BottomRightControlHandle_Selected = False Then

                            'Set pointer origin to the current mouse location.
                            Pointer_Origin = e.Location

                            'Is there at least one wall?
                            If Walls IsNot Nothing Then
                                'Yes, there is at least one wall.

                                'Transform the mouses viewport coordinates into level coordinates.
                                Dim MousePointerInLevelCoordinates As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)

                                'Go thru every wall in walls in reverse order.
                                For Index = UBound(Walls) To 0 Step -1

                                    'Was a wall selected?
                                    If MousePointerInLevelCoordinates.IntersectsWith(Walls(Index).Rec) Then
                                        'Yes, a wall was selected.

                                        Selected_Wall_Index = Index
                                        IsWallSelected = True
                                        Pointer_Offset.X = Pointer_Origin.X - Walls(Selected_Wall_Index).Rec.X
                                        Pointer_Offset.Y = Pointer_Origin.Y - Walls(Selected_Wall_Index).Rec.Y

                                        Update_Properties = True

                                        Exit For

                                    End If
                                    IsWallSelected = False
                                    Selected_Wall_Index = -1

                                    Update_Properties = True
                                    'DataGridView1
                                Next
                            End If
                        End If
                    End If
                    '*************************************************************************************************

                    'Floor Selection**********************************************************************************************

                    If IsWallSelected = False Then


                        'Has a floor been selected?
                        If IsFloorSelected = False Then
                            'No floor is selected.

                            'Set pointer origin to the current mouse location.
                            Pointer_Origin = e.Location

                            'Is there at least one floor?
                            If Floors IsNot Nothing Then
                                'Yes, there is at least one floor.
                                Dim MousePointerInViewPortCooridinates As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)
                                'Go thru every floor in floor in reverse z order.
                                For Index = UBound(Floors) To 0 Step -1
                                    'Was a floor selected?
                                    If MousePointerInViewPortCooridinates.IntersectsWith(Floors(Index).Rec) Then
                                        'Yes, a wall was selected.

                                        Selected_Floor_Index = Index
                                        IsFloorSelected = True
                                        Pointer_Offset.X = Pointer_Origin.X - Floors(Selected_Floor_Index).Rec.X
                                        Pointer_Offset.Y = Pointer_Origin.Y - Floors(Selected_Floor_Index).Rec.Y
                                        Exit For

                                    End If
                                    IsFloorSelected = False
                                    Selected_Floor_Index = -1
                                Next
                            End If

                        Else
                            'Yes, a floor is selected.

                            'Control Handle Selection
                            Dim MousePointerRec As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)
                            Dim TopLeftControlHandleRec As New Rectangle(Floors(Selected_Floor_Index).Rec.X - 10, Floors(Selected_Floor_Index).Rec.Y - 10, 20, 20)
                            Dim BottomRightControlHandleRec As New Rectangle(Floors(Selected_Floor_Index).Rec.Right - 10, Floors(Selected_Floor_Index).Rec.Bottom - 10, 20, 20)

                            'Is the user selecting the top-left control handle?
                            If MousePointerRec.IntersectsWith(TopLeftControlHandleRec) = True Then
                                'Yes, the user is selecting the top-left control handle.

                                TopLeftControlHandle_Selected = True

                                Floor_Origin.X = Floors(Selected_Floor_Index).Rec.Right
                                Floor_Origin.Y = Floors(Selected_Floor_Index).Rec.Bottom

                            Else
                                'No, the user is not selecting the top-left control handle.

                                TopLeftControlHandle_Selected = False

                            End If

                            'Is the user selecting the botton-right control handle?
                            If MousePointerRec.IntersectsWith(BottomRightControlHandleRec) = True Then
                                'Yes, the user is selecting the botton-right control handle.

                                BottomRightControlHandle_Selected = True

                                Floor_Origin.X = Floors(Selected_Floor_Index).Rec.X
                                Floor_Origin.Y = Floors(Selected_Floor_Index).Rec.Y

                            Else
                                'No, the user is not selecting the botton-right control handle.

                                BottomRightControlHandle_Selected = False

                            End If

                            If TopLeftControlHandle_Selected = False And BottomRightControlHandle_Selected = False Then

                                'Set pointer origin to the current mouse location.
                                Pointer_Origin = e.Location

                                'Is there at least one floor?
                                If Floors IsNot Nothing Then
                                    'Yes, there is at least one floor.

                                    'Transform the mouses viewport coordinates into level coordinates.
                                    Dim MousePointerInLevelCoordinates As New Rectangle(e.X + Viewport.X, e.Y + Viewport.Y, 1, 1)

                                    'Go thru every floor in floors in reverse order.
                                    For Index = UBound(Floors) To 0 Step -1

                                        'Was a floor selected?
                                        If MousePointerInLevelCoordinates.IntersectsWith(Floors(Index).Rec) Then
                                            'Yes, a floor was selected.

                                            Selected_Floor_Index = Index
                                            IsFloorSelected = True
                                            Pointer_Offset.X = Pointer_Origin.X - Floors(Selected_Floor_Index).Rec.X
                                            Pointer_Offset.Y = Pointer_Origin.Y - Floors(Selected_Floor_Index).Rec.Y
                                            Exit For

                                        End If
                                        IsFloorSelected = False
                                        Selected_Floor_Index = -1
                                    Next
                                End If
                            End If
                        End If

                    End If
                    '*************************************************************************************************

                End If
                '*****************************************************************************************************

                'Wall*************************************************************************************************
                If Selected_Tool = ToolsEnum.Wall Then

                    IsWallSelected = False
                    Selected_Wall_Index = -1

                    'Define a wall.
                    Wall.Rec.Width = 0
                    Wall.Rec.Height = 0

                    'Wall_Origin = e.Location
                    Wall_Origin.X = e.X + Viewport.X
                    Wall_Origin.Y = e.Y + Viewport.Y

                End If
                '*****************************************************************************************************

                'Floor************************************************************************************************
                If Selected_Tool = ToolsEnum.Floor Then

                    IsWallSelected = False
                    Selected_Wall_Index = -1

                    'Define a wall.
                    Floor.Rec.Width = 0
                    Floor.Rec.Height = 0

                    'Set the floor origin.
                    Floor_Origin.X = e.X + Viewport.X
                    Floor_Origin.Y = e.Y + Viewport.Y

                End If
                '*****************************************************************************************************

                Mouse_Down_Left = True

            End If

        Else
            'No, the editor is off. The game is running. - Game On

            'Did the player push down the left mouse button?
            If e.Button = MouseButtons.Left Then
                'Yes, the player pushed the left mouse button down.

                'Set the movement target to the mouse postion in level coordinates.
                Movement_Target.X = e.X + Viewport.X
                Movement_Target.Y = e.Y + Viewport.Y

                'Keep target on the level.
                If Movement_Target.X < Level.Rec.X Then
                    Movement_Target.X = Level.Rec.X
                End If
                If Movement_Target.X > Level.Rec.Width Then
                    Movement_Target.X = Level.Rec.Width
                End If
                If Movement_Target.Y < Level.Rec.Y Then
                    Movement_Target.Y = Level.Rec.Y
                End If
                If Movement_Target.Y > Level.Rec.Height Then
                    Movement_Target.Y = Level.Rec.Height
                End If

                Mouse_Down_Left = True

            End If

            'Did the player push down the right mouse button?
            If e.Button = MouseButtons.Right Then
                'Yes, the player pushed the right mouse button down.

                If ProjectileInflight = False Then


                    'Set the magic target to the mouse postion in level coordinates.
                    Magic_Target.X = e.X + Viewport.X
                    Magic_Target.Y = e.Y + Viewport.Y

                    'Keep target on the level.
                    If Magic_Target.X < Level.Rec.X Then
                        Magic_Target.X = Level.Rec.X
                    End If
                    If Magic_Target.X > Level.Rec.Width Then
                        Magic_Target.X = Level.Rec.Width
                    End If
                    If Magic_Target.Y < Level.Rec.Y Then
                        Magic_Target.Y = Level.Rec.Y
                    End If
                    If Magic_Target.Y > Level.Rec.Height Then
                        Magic_Target.Y = Level.Rec.Height
                    End If

                    Mouse_Down_Right = True

                End If

            End If

        End If

        'If IsWallSelected = False And IsFloorSelected = False Then

        '    If Me.DataGridView1.Visible = True Then
        '        Me.DataGridView1.Visible = False
        '    End If

        'Else

        '    If Me.DataGridView1.Visible = False Then
        '        Me.DataGridView1.Visible = True
        '    End If

        'End If

    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp

        If Editor_On = True Then

            Mouse_Down_Left = False

            'Wall*************************************************************************************************
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
            '*****************************************************************************************************

            'Floor************************************************************************************************
            If Selected_Tool = ToolsEnum.Floor Then
                'Has the mouse moved?
                If Floor.Rec.Width > 10 And Floor.Rec.Height > 10 Then
                    'Yes, the mouse moved.

                    Add_Floor(New Rectangle(Floor.Rec.Location, Floor.Rec.Size))

                    If Floor_Menu.Checked = True Then
                        Floor_Menu.Checked = False
                    End If

                    Selected_Tool = ToolsEnum.Pointer

                    MenuItemPointer.Checked = True

                End If
            End If
            '*****************************************************************************************************


        Else


            'Did the player let the left mouse button up?
            If e.Button = MouseButtons.Left Then
                'Yes, the player let the left mouse button up.

                Mouse_Down_Left = False

            End If

            'Did the player let the right mouse button up?
            If e.Button = MouseButtons.Right Then
                'Yes, the player let the right mouse button up.

                Mouse_Down_Right = False

            End If

        End If

    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove

        If Editor_On = True Then

            If Mouse_Down_Left = True Then

                'Pointer**********************************************************************************************
                If Selected_Tool = ToolsEnum.Pointer Then
                    If Walls IsNot Nothing Then
                        If Selected_Wall_Index > -1 Then
                            If TopLeftControlHandle_Selected = False And BottomRightControlHandle_Selected = False Then

                                'Move the wall.
                                Walls(Selected_Wall_Index).Rec.X = e.X - Pointer_Offset.X
                                Walls(Selected_Wall_Index).Rec.Y = e.Y - Pointer_Offset.Y

                                Update_Properties = True

                            ElseIf TopLeftControlHandle_Selected = True Then

                                'Move point
                                'Which point is the top?
                                If Wall_Origin.Y > e.Y + Viewport.Y Then
                                    Walls(Selected_Wall_Index).Rec.Y = e.Y + Viewport.Y
                                Else
                                    Walls(Selected_Wall_Index).Rec.Y = Wall_Origin.Y
                                End If

                                'Which point is the left?
                                If Wall_Origin.X > e.X + Viewport.X Then
                                    Walls(Selected_Wall_Index).Rec.X = e.X + Viewport.X
                                Else
                                    Walls(Selected_Wall_Index).Rec.X = Wall_Origin.X
                                End If

                                Walls(Selected_Wall_Index).Rec.Width = Abs(Wall_Origin.X - (e.X + Viewport.X))

                                Walls(Selected_Wall_Index).Rec.Height = Abs(Wall_Origin.Y - (e.Y + Viewport.Y))

                                Update_Properties = True

                            ElseIf BottomRightControlHandle_Selected = True Then

                                'Which point is the top?
                                If Wall_Origin.Y > e.Y + Viewport.Y Then
                                    Walls(Selected_Wall_Index).Rec.Y = e.Y + Viewport.Y
                                Else
                                    Walls(Selected_Wall_Index).Rec.Y = Wall_Origin.Y
                                End If

                                'Which point is the left?
                                If Wall_Origin.X > e.X + Viewport.X Then
                                    Walls(Selected_Wall_Index).Rec.X = e.X + Viewport.X
                                Else
                                    Walls(Selected_Wall_Index).Rec.X = Wall_Origin.X
                                End If

                                Walls(Selected_Wall_Index).Rec.Width = Abs(Wall_Origin.X - (e.X + Viewport.X))

                                Walls(Selected_Wall_Index).Rec.Height = Abs(Wall_Origin.Y - (e.Y + Viewport.Y))

                                Update_Properties = True

                            End If

                        End If
                    End If
                End If
                '*****************************************************************************************************

                'Wall*************************************************************************************************
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
                '*****************************************************************************************************

                'Floor************************************************************************************************
                If Selected_Tool = ToolsEnum.Floor Then

                    'Which point is the top?
                    If Floor_Origin.Y > e.Y + Viewport.Y Then
                        Floor.Rec.Y = e.Y + Viewport.Y
                    Else
                        Floor.Rec.Y = Floor_Origin.Y
                    End If

                    'Which point is the left?
                    If Floor_Origin.X > e.X + Viewport.X Then
                        Floor.Rec.X = e.X + Viewport.X
                    Else
                        Floor.Rec.X = Floor_Origin.X
                    End If

                    Floor.Rec.Width = Abs(Floor_Origin.X - (e.X + Viewport.X))

                    Floor.Rec.Height = Abs(Floor_Origin.Y - (e.Y + Viewport.Y))

                End If
                '*****************************************************************************************************

            End If
        End If

        'If IsWallSelected = True Then

        'Display_Wall_Properties()

        'End If

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

        If Floor_Menu.Checked = True Then
            Floor_Menu.Checked = False
        End If

    End Sub

    Private Sub MenuItemWall_Click(sender As Object, e As EventArgs) Handles MenuItemWall.Click

        Selected_Tool = ToolsEnum.Wall

        MenuItemWall.Checked = True

        If MenuItemPointer.Checked = True Then
            MenuItemPointer.Checked = False
        End If

        If Floor_Menu.Checked = True Then
            Floor_Menu.Checked = False
        End If

    End Sub

    Private Sub Floor_Menu_Click(sender As Object, e As EventArgs) Handles Floor_Menu.Click

        Selected_Tool = ToolsEnum.Floor

        Floor_Menu.Checked = True

        If MenuItemPointer.Checked = True Then
            MenuItemPointer.Checked = False
        End If

        If MenuItemWall.Checked = True Then
            MenuItemWall.Checked = False
        End If

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

    Private Sub Add_Wall(ByVal WallRec As Rectangle)

        If Walls IsNot Nothing Then
            Array.Resize(Walls, Walls.Length + 1)
            Walls(Walls.Length - 1).Rec = WallRec

            Walls(Walls.Length - 1).Color = Wall.Color
            Walls(Walls.Length - 1).OutlineColor = Wall.OutlineColor

        Else
            ReDim Walls(0)
            Walls(0).Rec = WallRec

            Walls(0).Color = Wall.Color
            Walls(0).OutlineColor = Wall.OutlineColor

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

    Private Sub Add_Floor(ByVal Rec As Rectangle)

        If Floors IsNot Nothing Then
            Array.Resize(Floors, Floors.Length + 1)
            Floors(Floors.Length - 1).Rec = Rec
            Floors(Floors.Length - 1).Color = Floor.Color

        Else
            ReDim Floors(0)
            Floors(0).Rec = Rec
            Floors(Floors.Length - 1).Color = Floor.Color

        End If

    End Sub

    Private Sub Remove_Floor(IndexToRemove As Integer)

        'Is this the last floor in the floors array?
        If UBound(Floors) > 0 Then 'Arrays start at zero.
            'No, this is not the last floor in the floors array.

            'Create temporary floors array. Set to the size of the floors array minus one floor.
            Dim TempFloors(UBound(Floors) - 1) As Floor_Struct

            'Create temporary floor array index. Set the temporary array index to the first floor.
            Dim TempIndex As Integer = LBound(Floors)

            'Copy the array without the element to the temporary array.
            'Go thur the floor array, one by one, start to end.
            For index = LBound(Floors) To UBound(Floors)

                'Is the current floor selected for removal.
                If index <> IndexToRemove Then
                    'No, the current floor is not selected for removal.

                    'Copy the current floor from walls to the temporary walls array.
                    TempFloors(TempIndex) = Floors(index)

                    'Advance the temporary index.
                    TempIndex += 1

                End If
            Next

            'Resize the floors array to match the temporary floors array size.
            ReDim Floors(UBound(TempFloors))

            'Copy the temporary floors array back to the floors array.
            Floors = TempFloors

        Else
            'Yes, this is the last floor in the floors array.

            Floors = Nothing 'Reset floors to default value.

        End If

    End Sub

    Private Sub Save_Level_File()

        Dim File_Path As String = Application.StartupPath & "Level.txt"
        Dim File_Number As Integer = FreeFile()

        FileOpen(File_Number, File_Path, OpenMode.Output)

        'Write Walls*************************************************************
        If Walls IsNot Nothing Then

            'Go thur every wall in the walls array. One by one. Start to end.
            For Wall_Index = 0 To UBound(Walls)

                'Write wall data in game object format.
                Write(File_Number, Object_ID_Enum.Wall)

                Write(File_Number, Walls(Wall_Index).Rec.X)
                Write(File_Number, Walls(Wall_Index).Rec.Y)
                Write(File_Number, Walls(Wall_Index).Rec.Width)
                Write(File_Number, Walls(Wall_Index).Rec.Height)

                Write(File_Number, Walls(Wall_Index).Color.ToArgb) 'Convert ARGB color to integer color.
                Write(File_Number, Walls(Wall_Index).OutlineColor.ToArgb) 'Convert ARGB color to integer color.

                Write(File_Number, Walls(Wall_Index).MapColor.ToArgb) 'Convert ARGB color to integer color.
                Write(File_Number, Walls(Wall_Index).MapOutlineColor.ToArgb) 'Convert ARGB color to integer color.

                Write(File_Number, False) 'Revealed - Boolean

                Write(File_Number, "") 'Text - String

                Write(File_Number, False) 'IsOpen - Boolean

            Next

        End If
        '*************************************************************************

        'Write Floors*************************************************************
        If Floors IsNot Nothing Then

            'Go thur every floor in the floors array. One by one. Start to end.
            For Floor_Index = 0 To UBound(Floors)

                'Write floor data in game object format.
                Write(File_Number, Object_ID_Enum.Floor)

                Write(File_Number, Floors(Floor_Index).Rec.X)
                Write(File_Number, Floors(Floor_Index).Rec.Y)
                Write(File_Number, Floors(Floor_Index).Rec.Width)
                Write(File_Number, Floors(Floor_Index).Rec.Height)

                Write(File_Number, Floors(Floor_Index).Color.ToArgb) 'Convert ARGB color to integer color.
                'Write(File_Number, Floor.Color.ToArgb) 'Convert ARGB color to integer color.
                Write(File_Number, Floors(Floor_Index).OutlineColor.ToArgb) 'Convert ARGB color to integer color.

                Write(File_Number, Floors(Floor_Index).MapColor.ToArgb) 'Convert ARGB color to integer color.
                Write(File_Number, Floors(Floor_Index).MapOutlineColor.ToArgb) 'Convert ARGB color to integer color.

                Write(File_Number, False) 'Revealed - Boolean

                Write(File_Number, "") 'Text - String

                Write(File_Number, False) 'IsOpen - Boolean

            Next

        End If
        '*************************************************************************

        FileClose(File_Number)

    End Sub

    Private Sub Open_Level_File()

        Dim File_Path As String = Application.StartupPath & "Level.txt"
        Dim File_Number As Integer = FreeFile()

        'Does the file exists?
        If My.Computer.FileSystem.FileExists(File_Path) = True Then
            'Yes, the file exists.

            FileOpen(File_Number, File_Path, OpenMode.Input)

            Game_Objects = Nothing 'Reset game objects to default value.

            Dim Object_Index As Integer = -1 'Arrays start at zero so one before zero is -1.

            'Go thur every game object in the level file. One by one. Start to end.
            Do Until EOF(File_Number)

                Object_Index += 1 'Add a object to the game objects array.

                ReDim Preserve Game_Objects(Object_Index) 'Resize the game objects array.

                'Copy object data from file.
                With Game_Objects(Object_Index)

                    Input(File_Number, .ID)

                    Input(File_Number, .X)
                    Input(File_Number, .Y)
                    Input(File_Number, .Width)
                    Input(File_Number, .Height)

                    Input(File_Number, .Color)
                    Input(File_Number, .OutlineColor)

                    Input(File_Number, .MapColor)
                    Input(File_Number, .MapOutlineColor)

                    Input(File_Number, .Revealed)

                    Input(File_Number, .Text)

                    Input(File_Number, .IsOpen)

                End With

            Loop

            FileClose(File_Number)

            'Load Level***********************************************************************************
            'Is there at least one game object?
            If Game_Objects IsNot Nothing Then
                'Yes, we have at least one game object.

                Walls = Nothing 'Reset walls array to default value.

                Dim Wall_Index As Integer = -1 'Arrays start at zero so one before zero is -1.

                Floors = Nothing 'Reset walls array to default value.

                Dim Floor_Index As Integer = -1 'Arrays start at zero so one before zero is -1.

                'Go thur every object in the game objects array. One by one. Start to end.
                For Index = 0 To UBound(Game_Objects)

                    'Load Walls*************************************************************
                    'Is the game object a wall?
                    If Game_Objects(Index).ID = Object_ID_Enum.Wall Then
                        'Yes, the game object is a wall.

                        Wall_Index += 1 'Add a wall to the walls array.
                        ReDim Preserve Walls(Wall_Index) 'Resize the walls array.

                        'Copy the wall from the game object.
                        Walls(Wall_Index).Rec.X = Game_Objects(Index).X 'Convert coordinate to rectangle.
                        Walls(Wall_Index).Rec.Y = Game_Objects(Index).Y 'Convert coordinate to rectangle.
                        Walls(Wall_Index).Rec.Width = Game_Objects(Index).Width 'Convert size to rectangle.
                        Walls(Wall_Index).Rec.Height = Game_Objects(Index).Height 'Convert size to rectangle.

                        Walls(Wall_Index).Color = Color.FromArgb(Game_Objects(Index).Color) 'Convert integer color to ARGB color.
                        Walls(Wall_Index).OutlineColor = Color.FromArgb(Game_Objects(Index).OutlineColor) 'Convert integer color to ARGB color.

                        Walls(Wall_Index).MapColor = Color.FromArgb(Game_Objects(Index).MapColor) 'Convert integer color to ARGB color.
                        Walls(Wall_Index).MapOutlineColor = Color.FromArgb(Game_Objects(Index).MapOutlineColor) 'Convert integer color to ARGB color.

                        Walls(Wall_Index).Revealed = Game_Objects(Index).Revealed

                    End If
                    '*************************************************************************

                    'Load Floors*************************************************************
                    'Is the game object a floor?
                    If Game_Objects(Index).ID = Object_ID_Enum.Floor Then
                        'Yes, the game object is a floor.

                        Floor_Index += 1 'Add a floor to the floors array.
                        ReDim Preserve Floors(Floor_Index) 'Resize the floors array.

                        'Copy the floor from the game object.
                        Floors(Floor_Index).Rec.X = Game_Objects(Index).X 'Convert coordinate to rectangle.
                        Floors(Floor_Index).Rec.Y = Game_Objects(Index).Y 'Convert coordinate to rectangle.
                        Floors(Floor_Index).Rec.Width = Game_Objects(Index).Width 'Convert size to rectangle.
                        Floors(Floor_Index).Rec.Height = Game_Objects(Index).Height 'Convert size to rectangle.

                        Floors(Floor_Index).Color = Color.FromArgb(Game_Objects(Index).Color) 'Convert integer color to ARGB color.
                        Floors(Floor_Index).OutlineColor = Color.FromArgb(Game_Objects(Index).OutlineColor) 'Convert integer color to ARGB color.

                        Floors(Floor_Index).MapColor = Color.FromArgb(Game_Objects(Index).MapColor) 'Convert integer color to ARGB color.
                        Floors(Floor_Index).MapOutlineColor = Color.FromArgb(Game_Objects(Index).MapOutlineColor) 'Convert integer color to ARGB color.

                        Floors(Floor_Index).Revealed = Game_Objects(Index).Revealed

                    End If
                    '*************************************************************************

                Next

            End If
            '*********************************************************************************************
        Else
            'No, the file doesn't exists.

            MsgBox("File Not Found. Try saving before opening.", MsgBoxStyle.Critical, "Error")

        End If

    End Sub

    Private Sub Open_Test_Level_File()

        'Dim Object_Record As New Object_Record_Info
        'Dim WallIndex As Integer
        'Dim RecordIndex As Integer = 1
        Dim File_Number As Integer = FreeFile()
        Dim AppPath As String = Application.StartupPath
        'Dim WallRec As Rectangle

        Dim File_Path As String = AppPath & "TESTFILE.txt"
        Dim Index As Integer = -1
        Walls = Nothing 'Reset walls to default value.

        FileOpen(File_Number, File_Path, OpenMode.Input)

        Do Until EOF(File_Number)

            Index += 1

            ReDim Preserve Walls(Index)

            With Walls(Index)

                Input(File_Number, .Rec.X)

                Input(File_Number, .Rec.Y)

                Input(File_Number, .Rec.Width)

                Input(File_Number, .Rec.Height)

            End With

        Loop

        FileClose(File_Number)

    End Sub


    Private Sub Save_Test_Level_File()

        'Dim Object_Record As Object_Record_Info
        Dim WallIndex As Integer = -1
        Dim RecordIndex As Integer = 1
        Dim File_Number As Integer = FreeFile()
        Dim AppPath As String = Application.StartupPath

        Dim File_Path As String = AppPath & "TESTFILE.txt"

        If Walls IsNot Nothing Then

            FileOpen(File_Number, File_Path, OpenMode.Output)

            'Go thur every wall in the walls array. One by one. Start to end.
            For WallIndex = 0 To UBound(Walls)

                Write(File_Number, Walls(WallIndex).Rec.X)
                Write(File_Number, Walls(WallIndex).Rec.Y)
                Write(File_Number, Walls(WallIndex).Rec.Width)
                Write(File_Number, Walls(WallIndex).Rec.Height)

            Next

            FileClose(File_Number)

        End If

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

    Private Sub GS_SoundEnded(ByVal SndName As String)

        If GS.IsPlaying("Music") = False Then

            GS.Play("Music")

        End If

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GS.Dispose() 'make sure you call dispose on the new GameSounds class when closing form
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

    Private Sub Save_Menu_Click(sender As Object, e As EventArgs) Handles Save_Menu.Click

        'Save_Test_Level_File()

        Save_Level_File()



    End Sub

    Private Sub Open_Menu_Click(sender As Object, e As EventArgs) Handles Open_Menu.Click

        'Open_Test_Level_File()

        Open_Level_File()


    End Sub

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick

        If Update_Properties = True Then


            If IsWallSelected = True Then

                Display_Wall_Properties()

                Me.DataGridView1.ClearSelection()

            Else

                Display_Blank_Properties()


            End If

            Update_Properties = False

        End If

        'If PictureBox1.Focus = True Then

        '    Me.DataGridView1.ClearSelection()

        'End If

    End Sub

    Private Sub Splitter1_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles Splitter1.SplitterMoved

        Center_Viewport_on_the_Hero()

    End Sub

    Private Sub PictureBox1_Resize(sender As Object, e As EventArgs) Handles PictureBox1.Resize

    End Sub
End Class

Public Enum MCI_NOTIFY As Integer
    SUCCESSFUL = &H1
    SUPERSEDED = &H2
    ABORTED = &H4
    FAILURE = &H8
End Enum

Public Class GameSounds
    'Game sounds class By IronRazer - https://www.dreamincode.net/forums/topic/378353-how-to-play-2-sounds-simultaneously/

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
