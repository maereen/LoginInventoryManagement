Public Class HomeForm

    Private ReadOnly SideButtonHeight As Integer = 44
    Private ReadOnly SideButtonLeftPadding As New Padding(64, 0, 0, 0)

    Private isDragging As Boolean = False
    Private dragStartPoint As Point

    Private Sub HomeForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ApplyCleanLayout()
        MoveSidePanel(btndashboard)
        LoadPage(New DashboardPage())
    End Sub

    Private Sub HomeForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ApplyCleanLayout()
    End Sub

    Private Sub ApplyCleanLayout()
        Panel1.Width = 250
        Panel1.Dock = DockStyle.Left

        Panel2.Dock = DockStyle.Top
        Panel2.Height = 70

        Panel3.Dock = DockStyle.Fill
        Panel3.BackColor = Color.FromArgb(248, 244, 250)

        Label6.Text = "Welcome!"
        Label6.Location = New Point(18, 20)

        lbladmin.Visible = False
        PictureBox8.Visible = False
        TextBox2.Visible = False
        TextBox3.Visible = False

        PictureBox12.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        PictureBox10.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        PictureBox11.Anchor = AnchorStyles.Top Or AnchorStyles.Right

        PictureBox12.Location = New Point(Panel2.Width - 34, 6)
        PictureBox10.Location = New Point(Panel2.Width - 62, 6)
        PictureBox11.Location = New Point(Panel2.Width - 90, 6)

        FormatSideButton(btndashboard, "Dashboard", 112, PictureBox2)
        FormatSideButton(btnInventory, "Inventory", 162, PictureBox7)
        FormatSideButton(btntransaction, "Transactions", 212, PictureBox6)
        FormatSideButton(btnsuppliers, "Suppliers", 262, PictureBox3)
        FormatSideButton(btnreports, "Reports", 312, PictureBox4)
        FormatSideButton(btnsettings, "Settings", 362, PictureBox5)
        FormatSideButton(btnlogout, "Logout", Panel1.Height - 70, PictureBox9)

        TextBox1.Width = Panel1.Width - 24
        TextBox1.Location = New Point(12, 82)

        SidePanel.Width = 5

        ApplyThemeColors()
    End Sub

    Private Sub ApplyThemeColors()
        Dim linen As Color = Color.FromArgb(253, 241, 226)
        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim amethyst As Color = Color.FromArgb(171, 146, 191)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        Panel1.BackColor = linen

        Label3.ForeColor = dolphin
        Label5.ForeColor = dolphin

        TextBox1.BackColor = amethyst
        TextBox1.ForeColor = amethyst

        SidePanel.BackColor = amethyst

        For Each btn In New Button() {btndashboard, btnInventory, btntransaction, btnsuppliers, btnreports, btnsettings, btnlogout}
            btn.ForeColor = textDark
            btn.BackColor = Color.Transparent
            btn.FlatStyle = FlatStyle.Flat
            btn.FlatAppearance.BorderSize = 0
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 220, 245)
        Next

        Panel2.BackColor = dolphin

        Label6.ForeColor = linen
        Label6.BackColor = Color.Transparent
        Label6.Font = New Font("Segoe UI", 13.5F, FontStyle.Bold)

        PictureBox10.BackColor = dolphin
        PictureBox11.BackColor = dolphin
        PictureBox12.BackColor = dolphin
    End Sub

    Private Sub FormatSideButton(btn As Button, text As String, y As Integer, icon As PictureBox)
        btn.Text = text
        btn.Width = Panel1.Width - 18
        btn.Height = SideButtonHeight
        btn.Location = New Point(8, y)
        btn.TextAlign = ContentAlignment.MiddleLeft
        btn.Padding = SideButtonLeftPadding
        btn.Font = New Font("Segoe UI", 12.0F, FontStyle.Regular)
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0

        icon.SizeMode = PictureBoxSizeMode.Zoom
        icon.Size = New Size(28, 28)
        icon.Location = New Point(28, y + 8)
    End Sub

    Private Sub LoadPage(page As UserControl)
        Panel3.Controls.Clear()
        page.Dock = DockStyle.Fill
        Panel3.Controls.Add(page)
    End Sub

    Private Sub LoadPlaceholderPage(title As String)
        Dim page As New UserControl()
        page.BackColor = Color.FromArgb(248, 244, 250)

        Dim label As New Label With {
            .Text = title & " page will be added next.",
            .Font = New Font("Segoe UI Semilight", 22),
            .ForeColor = Color.FromArgb(101, 90, 124),
            .AutoSize = True,
            .Location = New Point(24, 24)
        }

        page.Controls.Add(label)
        LoadPage(page)
    End Sub

    Private Sub MoveSidePanel(btn As Button)
        SidePanel.Visible = True
        SidePanel.Width = 5
        SidePanel.Height = btn.Height - 10
        SidePanel.Left = 0
        SidePanel.Top = btn.Top + 5
    End Sub

    Private Sub btndashboard_Click(sender As Object, e As EventArgs) Handles btndashboard.Click
        MoveSidePanel(btndashboard)
        LoadPage(New DashboardPage())
    End Sub

    Private Sub btnInventory_Click(sender As Object, e As EventArgs) Handles btnInventory.Click
        MoveSidePanel(btnInventory)
        LoadPage(New InventoryPage())
    End Sub

    Private Sub btntransaction_Click(sender As Object, e As EventArgs) Handles btntransaction.Click
        MoveSidePanel(btntransaction)
        LoadPage(New TransactionsPage())
    End Sub

    Private Sub btnsuppliers_Click(sender As Object, e As EventArgs) Handles btnsuppliers.Click
        MoveSidePanel(btnsuppliers)
        LoadPlaceholderPage("Suppliers")
    End Sub

    Private Sub btnreports_Click(sender As Object, e As EventArgs) Handles btnreports.Click
        MoveSidePanel(btnreports)
        LoadPlaceholderPage("Reports")
    End Sub

    Private Sub btnsettings_Click(sender As Object, e As EventArgs) Handles btnsettings.Click
        MoveSidePanel(btnsettings)
        LoadPlaceholderPage("Settings")
    End Sub

    Private Sub btnlogout_Click(sender As Object, e As EventArgs) Handles btnlogout.Click
        Me.Hide()
        LoginForm.Show()
    End Sub

    Private Sub PictureBox12_Click(sender As Object, e As EventArgs) Handles PictureBox12.Click
        If MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Application.ExitThread()
        End If
    End Sub

    Private Sub PictureBox10_Click(sender As Object, e As EventArgs) Handles PictureBox10.Click
        If Me.WindowState = FormWindowState.Normal Then
            Me.WindowState = FormWindowState.Maximized
        Else
            Me.WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub PictureBox11_Click(sender As Object, e As EventArgs) Handles PictureBox11.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub StartDrag(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        If e.Button = MouseButtons.Left Then
            isDragging = True
            dragStartPoint = e.Location
        End If
    End Sub

    Private Sub DragForm(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        If isDragging Then
            Me.Location = New Point(
                Me.Location.X + e.X - dragStartPoint.X,
                Me.Location.Y + e.Y - dragStartPoint.Y
            )
        End If
    End Sub

    Private Sub StopDrag(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        isDragging = False
    End Sub

    Private Sub Panel2_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel2.MouseDown
        StartDrag(sender, e)
    End Sub

    Private Sub Panel2_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel2.MouseMove
        DragForm(sender, e)
    End Sub

    Private Sub Panel2_MouseUp(sender As Object, e As MouseEventArgs) Handles Panel2.MouseUp
        StopDrag(sender, e)
    End Sub

End Class
