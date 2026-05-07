Imports InventoryBackend

Public Class LoginForm

    Private isDragging As Boolean = False
    Private dragStartPoint As Point
    Private lblLoginError As Label
    Private isClearingPasswordAfterError As Boolean = False

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ApplyCleanLoginLayout()
    End Sub

    Private Sub LoginForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ApplyCleanLoginLayout()
    End Sub

    Private Sub ApplyCleanLoginLayout()
        Dim linen As Color = Color.FromArgb(253, 241, 226)
        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim amethyst As Color = Color.FromArgb(171, 146, 191)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        Me.MinimumSize = New Size(900, 520)

        Panel1.Dock = DockStyle.Fill
        Panel1.BackgroundImage = Nothing
        Panel1.BackColor = dolphin

        Label1.Visible = False
        Label2.Visible = False
        Label3.Visible = False
        Label4.Visible = False
        TextBox1.Visible = False
        PictureBox1.Visible = False
        PictureBox2.Visible = False

        Dim cardW As Integer = 380
        Dim cardH As Integer = 350
        Dim cardX As Integer = (Panel1.Width - cardW) \ 2
        Dim cardY As Integer = (Panel1.Height - cardH) \ 2

        Dim card As Panel = TryCast(Panel1.Controls("loginCard"), Panel)

        If card Is Nothing Then
            card = New Panel()
            card.Name = "loginCard"
            Panel1.Controls.Add(card)
        End If

        card.BackColor = linen
        card.Size = New Size(cardW, cardH)
        card.Location = New Point(cardX, cardY)

        Dim title As Label = TryCast(card.Controls("lblTitle"), Label)

        If title Is Nothing Then
            title = New Label()
            title.Name = "lblTitle"
            title.TextAlign = ContentAlignment.MiddleCenter
            card.Controls.Add(title)
        End If

        title.Text = "Welcome"
        title.Font = New Font("Segoe UI", 22, FontStyle.Bold)
        title.ForeColor = dolphin
        title.BackColor = linen
        title.Location = New Point(0, 28)
        title.Size = New Size(cardW, 55)

        If lblLoginError Is Nothing Then
            lblLoginError = New Label()
            lblLoginError.Name = "lblLoginError"
            card.Controls.Add(lblLoginError)
        End If

        lblLoginError.ForeColor = Color.FromArgb(200, 60, 60)
        lblLoginError.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        lblLoginError.Location = New Point(55, 84)
        lblLoginError.Size = New Size(270, 25)
        lblLoginError.TextAlign = ContentAlignment.MiddleCenter
        lblLoginError.BackColor = linen
        lblLoginError.Visible = False
        lblLoginError.BringToFront()

        TextBox2.Parent = card
        TextBox2.Location = New Point(55, 120)
        TextBox2.Size = New Size(270, 34)
        TextBox2.Font = New Font("Segoe UI", 10, FontStyle.Regular)
        TextBox2.BorderStyle = BorderStyle.FixedSingle
        TextBox2.ForeColor = textDark
        TextBox2.BackColor = Color.White
        TextBox2.PlaceholderText = "Enter your username"

        TextBox3.Parent = card
        TextBox3.Location = New Point(55, 170)
        TextBox3.Size = New Size(270, 34)
        TextBox3.Font = New Font("Segoe UI", 10, FontStyle.Regular)
        TextBox3.BorderStyle = BorderStyle.FixedSingle
        TextBox3.ForeColor = textDark
        TextBox3.BackColor = Color.White
        TextBox3.PlaceholderText = "Enter your password"
        TextBox3.UseSystemPasswordChar = True

        btnLogin.Parent = card
        btnLogin.Location = New Point(55, 235)
        btnLogin.Size = New Size(270, 42)
        btnLogin.Text = "Login"
        btnLogin.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        btnLogin.BackColor = dolphin
        btnLogin.ForeColor = linen
        btnLogin.FlatStyle = FlatStyle.Flat
        btnLogin.FlatAppearance.BorderSize = 0
        btnLogin.FlatAppearance.MouseOverBackColor = amethyst

        btnclose.Parent = Panel1
        btnclose.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnclose.Location = New Point(Panel1.Width - 45, 10)
        btnclose.Size = New Size(30, 30)
        btnclose.BackColor = dolphin
        btnclose.BringToFront()
    End Sub

    Private Sub ShowLoginError(message As String)
        If lblLoginError Is Nothing Then Return
        lblLoginError.Text = message
        lblLoginError.Visible = True
        lblLoginError.BringToFront()
    End Sub

    Private Sub HideLoginError()
        If lblLoginError Is Nothing Then Return
        lblLoginError.Text = ""
        lblLoginError.Visible = False
    End Sub

    Private Sub HideErrorOnTyping(sender As Object, e As EventArgs) Handles TextBox2.TextChanged, TextBox3.TextChanged
        If isClearingPasswordAfterError Then Return
        HideLoginError()
    End Sub

    Private Sub btnclose_Click(sender As Object, e As EventArgs) Handles btnclose.Click
        Application.Exit()
    End Sub

    Private Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim username As String = TextBox2.Text.Trim()
        Dim password As String = TextBox3.Text.Trim()

        If username = "" OrElse password = "" Then
            ShowLoginError("Please enter username and password")
            Return
        End If

        Try
            Dim repo As New InventoryBackend.UserRepository()
            Dim result = repo.Login(username, password)

            If result.success Then
                InventoryBackend.SessionManager.Username = result.username
                InventoryBackend.SessionManager.FullName = result.fullName
                InventoryBackend.SessionManager.Email = result.email
                InventoryBackend.SessionManager.Role = result.role
                InventoryBackend.SessionManager.Permissions = result.permissions

                HideLoginError()
                Me.Hide()
                HomeForm.Show()
            Else
                isClearingPasswordAfterError = True
                TextBox3.Clear()
                isClearingPasswordAfterError = False

                ShowLoginError("Wrong username or password")
                TextBox3.Focus()
            End If

        Catch ex As Exception
            ShowLoginError("Database error. Please try again.")
        End Try
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

    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        StartDrag(sender, e)
    End Sub

    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove
        DragForm(sender, e)
    End Sub

    Private Sub Panel1_MouseUp(sender As Object, e As MouseEventArgs) Handles Panel1.MouseUp
        StopDrag(sender, e)
    End Sub

End Class
