Imports InventoryBackend

Public Class SettingsPage

    Private scrollPanel As Panel
    Private contentPanel As Panel

    Private btnChangePassword As Button
    Private btnAddAccount As Button
    Private btnViewAccounts As Button
    Private btnResetPassword As Button
    Private managementCard As Panel

    Private repo As New InventoryBackend.UserRepository()

    Private Sub SettingsPage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(248, 244, 250)
        BuildUI()
    End Sub

    Private Sub SettingsPage_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        LayoutUI()
    End Sub

    Private Sub BuildUI()
        Me.Controls.Clear()

        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim linen As Color = Color.FromArgb(253, 241, 226)
        Dim amethyst As Color = Color.FromArgb(171, 146, 191)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        scrollPanel = New Panel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True,
            .BackColor = Color.FromArgb(248, 244, 250)
        }

        contentPanel = New Panel With {
            .Location = New Point(0, 0),
            .Size = New Size(900, 560),
            .BackColor = Color.FromArgb(248, 244, 250)
        }

        scrollPanel.Controls.Add(contentPanel)
        Me.Controls.Add(scrollPanel)

        Dim title As New Label With {
            .Text = "Settings",
            .Font = New Font("Segoe UI Semilight", 22),
            .ForeColor = dolphin,
            .Location = New Point(24, 20),
            .AutoSize = True
        }

        Dim accountCard = CreateCard("Account Settings", 24, 90, 820, 190)

        accountCard.Controls.Add(CreateInfoLabel("Username:", InventoryBackend.SessionManager.Username, 24, 58))
        accountCard.Controls.Add(CreateInfoLabel("Full Name:", InventoryBackend.SessionManager.FullName, 24, 88))
        accountCard.Controls.Add(CreateInfoLabel("Email:", InventoryBackend.SessionManager.Email, 24, 118))
        accountCard.Controls.Add(CreateInfoLabel("Role:", InventoryBackend.SessionManager.Role, 24, 148))

        btnChangePassword = CreateButton("Change Password", dolphin, linen)
        btnChangePassword.Location = New Point(620, 62)
        AddHandler btnChangePassword.Click, AddressOf ChangePasswordClicked
        accountCard.Controls.Add(btnChangePassword)

        managementCard = CreateCard("Account Management", 24, 305, 820, 190)

        btnAddAccount = CreateButton("+ Add User", dolphin, linen)
        btnViewAccounts = CreateButton("View All Users", amethyst, Color.White)
        btnResetPassword = CreateButton("Reset Password", Color.FromArgb(230, 160, 45), Color.White)

        Dim buttonY As Integer = 62
        Dim buttonGap As Integer = 12

        btnAddAccount.Location = New Point(24, buttonY)
        btnViewAccounts.Location = New Point(btnAddAccount.Right + buttonGap, buttonY)
        btnResetPassword.Location = New Point(btnViewAccounts.Right + buttonGap, buttonY)

        AddHandler btnAddAccount.Click, AddressOf AddAccountClicked
        AddHandler btnViewAccounts.Click, AddressOf ViewAccountsClicked
        AddHandler btnResetPassword.Click, AddressOf ResetPasswordClicked

        Dim desc As New Label With {
            .Text = "Manage accounts, assign roles, remove users, and reset passwords. Visible only to users with permission.",
            .Font = New Font("Segoe UI", 10),
            .ForeColor = textDark,
            .Location = New Point(24, 120),
            .Size = New Size(740, 40)
        }

        managementCard.Controls.AddRange(New Control() {
            btnAddAccount,
            btnViewAccounts,
            btnResetPassword,
            desc
        })

        contentPanel.Controls.AddRange(New Control() {
            title,
            accountCard,
            managementCard
        })

        ApplyRolePermissions()
        LayoutUI()
    End Sub

    Private Sub LayoutUI()
        If contentPanel Is Nothing Then Return
        contentPanel.Width = Math.Max(900, Me.ClientSize.Width - 25)
        contentPanel.Height = 590
    End Sub

    Private Sub ApplyRolePermissions()
        If managementCard Is Nothing Then Return

        If Not InventoryBackend.SessionManager.CanManageUsers() Then
            managementCard.Visible = False
            contentPanel.Height = 340
        Else
            managementCard.Visible = True
        End If
    End Sub

    Private Function CreateCard(headerText As String, x As Integer, y As Integer, w As Integer, h As Integer) As Panel
        Dim card As New Panel With {
            .Location = New Point(x, y),
            .Size = New Size(w, h),
            .BackColor = Color.White
        }

        Dim header As New Label With {
            .Text = headerText,
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(101, 90, 124),
            .Location = New Point(24, 20),
            .AutoSize = True
        }

        card.Controls.Add(header)
        Return card
    End Function

    Private Function CreateInfoLabel(labelText As String, valueText As String, x As Integer, y As Integer) As Label
        Return New Label With {
            .Text = labelText & " " & valueText,
            .Font = New Font("Segoe UI", 10),
            .ForeColor = Color.FromArgb(45, 38, 55),
            .Location = New Point(x, y),
            .AutoSize = True
        }
    End Function

    Private Function CreateButton(text As String, bg As Color, fg As Color) As Button
        Dim btn As New Button With {
            .Text = text,
            .Size = New Size(145, 36),
            .BackColor = bg,
            .ForeColor = fg,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 9, FontStyle.Bold)
        }

        btn.FlatAppearance.BorderSize = 0
        Return btn
    End Function

    Private Sub ChangePasswordClicked(sender As Object, e As EventArgs)
        Using dialog As New ChangePasswordDialog(repo)
            dialog.ShowDialog()
        End Using
    End Sub

    Private Sub AddAccountClicked(sender As Object, e As EventArgs)
        If Not InventoryBackend.SessionManager.CanManageUsers() Then
            MessageBox.Show("You do not have permission to manage users.", "Access Denied")
            Return
        End If

        Using dialog As New AddUserDialog(repo)
            If dialog.ShowDialog() = DialogResult.OK Then
                MessageBox.Show("User added successfully.", "Account Management")
            End If
        End Using
    End Sub

    Private Sub ViewAccountsClicked(sender As Object, e As EventArgs)
        If Not InventoryBackend.SessionManager.CanManageUsers() Then
            MessageBox.Show("You do not have permission to manage users.", "Access Denied")
            Return
        End If

        Using dialog As New ViewUsersDialog(repo)
            dialog.ShowDialog()
        End Using
    End Sub

    Private Sub ResetPasswordClicked(sender As Object, e As EventArgs)
        If Not InventoryBackend.SessionManager.CanManageUsers() Then
            MessageBox.Show("You do not have permission to manage users.", "Access Denied")
            Return
        End If

        Using dialog As New ResetPasswordDialog(repo)
            dialog.ShowDialog()
        End Using
    End Sub

    Private Class ChangePasswordDialog
        Inherits Form

        Private repo As InventoryBackend.UserRepository

        Private txtCurrent As TextBox
        Private txtNew As TextBox
        Private txtConfirm As TextBox

        Public Sub New(userRepo As InventoryBackend.UserRepository)
            repo = userRepo

            Me.Text = "Change Password"
            Me.Size = New Size(420, 350)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = Color.FromArgb(253, 241, 226)
            BuildUI()
        End Sub

        Private Sub BuildUI()
            Dim dolphin As Color = Color.FromArgb(101, 90, 124)
            Dim linen As Color = Color.FromArgb(253, 241, 226)

            Dim title As New Label With {
                .Text = "Change Password",
                .Font = New Font("Segoe UI", 16, FontStyle.Bold),
                .ForeColor = dolphin,
                .Location = New Point(35, 25),
                .AutoSize = True
            }

            txtCurrent = CreatePasswordBox(35, 85)
            txtNew = CreatePasswordBox(35, 150)
            txtConfirm = CreatePasswordBox(35, 215)

            Dim btnCurrentEye = CreateEyeButton(325, 85, txtCurrent)
            Dim btnNewEye = CreateEyeButton(325, 150, txtNew)
            Dim btnConfirmEye = CreateEyeButton(325, 215, txtConfirm)

            Dim btnSave As New Button With {
                .Text = "Save",
                .Location = New Point(185, 265),
                .Size = New Size(85, 35),
                .BackColor = dolphin,
                .ForeColor = linen,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold)
            }
            btnSave.FlatAppearance.BorderSize = 0
            AddHandler btnSave.Click, AddressOf SaveClicked

            Dim btnCancel As New Button With {
                .Text = "Cancel",
                .Location = New Point(280, 265),
                .Size = New Size(85, 35),
                .BackColor = Color.Gray,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold)
            }
            btnCancel.FlatAppearance.BorderSize = 0
            AddHandler btnCancel.Click, Sub() Me.Close()

            Me.Controls.AddRange(New Control() {
                title,
                CreateLabel("Current Password", 35, 63),
                txtCurrent,
                btnCurrentEye,
                CreateLabel("New Password", 35, 128),
                txtNew,
                btnNewEye,
                CreateLabel("Confirm Password", 35, 193),
                txtConfirm,
                btnConfirmEye,
                btnSave,
                btnCancel
            })
        End Sub

        Private Function CreatePasswordBox(x As Integer, y As Integer) As TextBox
            Return New TextBox With {
                .Location = New Point(x, y),
                .Size = New Size(285, 30),
                .Font = New Font("Segoe UI", 10),
                .UseSystemPasswordChar = True
            }
        End Function

        Private Function CreateEyeButton(x As Integer, y As Integer, targetBox As TextBox) As Button
            Dim btn As New Button With {
                .Text = "👁",
                .Location = New Point(x, y),
                .Size = New Size(40, 30),
                .BackColor = Color.FromArgb(171, 146, 191),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 9, FontStyle.Bold),
                .Tag = targetBox
            }

            btn.FlatAppearance.BorderSize = 0
            AddHandler btn.MouseDown, AddressOf EyeMouseDown
            AddHandler btn.MouseUp, AddressOf EyeMouseUp
            AddHandler btn.MouseLeave, AddressOf EyeMouseUp

            Return btn
        End Function

        Private Sub EyeMouseDown(sender As Object, e As MouseEventArgs)
            DirectCast(DirectCast(sender, Button).Tag, TextBox).UseSystemPasswordChar = False
        End Sub

        Private Sub EyeMouseUp(sender As Object, e As EventArgs)
            DirectCast(DirectCast(sender, Button).Tag, TextBox).UseSystemPasswordChar = True
        End Sub

        Private Function CreateLabel(text As String, x As Integer, y As Integer) As Label
            Return New Label With {
                .Text = text,
                .Location = New Point(x, y),
                .AutoSize = True,
                .Font = New Font("Segoe UI", 9),
                .ForeColor = Color.FromArgb(45, 38, 55)
            }
        End Function

        Private Sub SaveClicked(sender As Object, e As EventArgs)
            Try
                If txtCurrent.Text.Trim() = "" OrElse txtNew.Text.Trim() = "" OrElse txtConfirm.Text.Trim() = "" Then
                    MessageBox.Show("Please fill in all password fields.", "Change Password")
                    Return
                End If

                If txtNew.Text.Length < 6 Then
                    MessageBox.Show("New password must be at least 6 characters.", "Change Password")
                    Return
                End If

                If txtNew.Text <> txtConfirm.Text Then
                    MessageBox.Show("New password and confirm password do not match.", "Change Password")
                    Return
                End If

                If txtCurrent.Text = txtNew.Text Then
                    MessageBox.Show("New password must be different from current password.", "Change Password")
                    Return
                End If

                If Not repo.ValidateLogin(InventoryBackend.SessionManager.Username, txtCurrent.Text) Then
                    MessageBox.Show("Current password is incorrect.", "Change Password")
                    Return
                End If

                repo.ResetPassword(InventoryBackend.SessionManager.Username, txtNew.Text)

                MessageBox.Show("Password changed successfully.", "Change Password")
                Me.DialogResult = DialogResult.OK
                Me.Close()

            Catch ex As Exception
                MessageBox.Show("Failed to change password: " & ex.Message, "Database Error")
            End Try
        End Sub
    End Class

    Private Class AddUserDialog
        Inherits Form

        Private repo As InventoryBackend.UserRepository

        Private txtUsername As TextBox
        Private txtFullName As TextBox
        Private txtEmail As TextBox
        Private txtPassword As TextBox
        Private cmbRole As ComboBox
        Private lblPermissions As Label
        Private clbPermissions As CheckedListBox

        Public Sub New(userRepo As InventoryBackend.UserRepository)
            repo = userRepo

            Me.Text = "Add User"
            Me.Size = New Size(850, 530)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = Color.FromArgb(253, 241, 226)
            BuildUI()
        End Sub

        Private Sub BuildUI()
            Dim dolphin As Color = Color.FromArgb(101, 90, 124)
            Dim linen As Color = Color.FromArgb(253, 241, 226)

            Dim title As New Label With {
                .Text = "Add User",
                .Font = New Font("Segoe UI", 17, FontStyle.Bold),
                .ForeColor = dolphin,
                .Location = New Point(35, 25),
                .AutoSize = True
            }

            txtUsername = CreateTextBox(45, 95)
            txtFullName = CreateTextBox(45, 170)
            cmbRole = CreateComboBox(45, 245)

            cmbRole.Items.AddRange(New String() {"System Admin", "Admin", "Staff"})
            cmbRole.SelectedIndex = 2

            txtEmail = CreateTextBox(450, 95)
            txtPassword = CreateTextBox(450, 170)
            txtPassword.UseSystemPasswordChar = True

            lblPermissions = CreateLabel("Additional Permission/s", 450, 223)

            clbPermissions = New CheckedListBox With {
                .Location = New Point(450, 245),
                .Size = New Size(320, 105),
                .Font = New Font("Segoe UI", 10),
                .CheckOnClick = True,
                .BackColor = Color.White,
                .ForeColor = Color.FromArgb(45, 38, 55)
            }

            clbPermissions.Items.AddRange(New String() {
                "Manage Suppliers",
                "Delete Inventory Items",
                "Manage Users"
            })

            AddHandler cmbRole.SelectedIndexChanged, AddressOf RoleChanged

            Dim roleNote As New Label With {
                .Name = "roleNote",
                .Text = "",
                .Location = New Point(450, 245),
                .Size = New Size(320, 70),
                .Font = New Font("Segoe UI", 10),
                .ForeColor = Color.FromArgb(45, 38, 55),
                .Visible = False
            }

            Dim btnConfirm As New Button With {
                .Text = "Confirm",
                .Location = New Point(565, 405),
                .Size = New Size(110, 38),
                .BackColor = dolphin,
                .ForeColor = linen,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold)
            }
            btnConfirm.FlatAppearance.BorderSize = 0
            AddHandler btnConfirm.Click, AddressOf ConfirmClicked

            Dim btnCancel As New Button With {
                .Text = "Cancel",
                .Location = New Point(690, 405),
                .Size = New Size(110, 38),
                .BackColor = Color.FromArgb(180, 85, 95),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold)
            }
            btnCancel.FlatAppearance.BorderSize = 0
            AddHandler btnCancel.Click, Sub() Me.Close()

            Me.Controls.AddRange(New Control() {
                title,
                CreateLabel("Username", 45, 73),
                txtUsername,
                CreateLabel("Full Name", 45, 148),
                txtFullName,
                CreateLabel("Role", 45, 223),
                cmbRole,
                CreateLabel("Email", 450, 73),
                txtEmail,
                CreateLabel("Password", 450, 148),
                txtPassword,
                lblPermissions,
                clbPermissions,
                roleNote,
                btnConfirm,
                btnCancel
            })

            RoleChanged(Nothing, Nothing)
        End Sub

        Private Function CreateLabel(text As String, x As Integer, y As Integer) As Label
            Return New Label With {
                .Text = text,
                .Location = New Point(x, y),
                .AutoSize = True,
                .Font = New Font("Segoe UI", 9),
                .ForeColor = Color.FromArgb(45, 38, 55)
            }
        End Function

        Private Function CreateTextBox(x As Integer, y As Integer) As TextBox
            Return New TextBox With {
                .Location = New Point(x, y),
                .Size = New Size(320, 32),
                .Font = New Font("Segoe UI", 10)
            }
        End Function

        Private Function CreateComboBox(x As Integer, y As Integer) As ComboBox
            Return New ComboBox With {
                .Location = New Point(x, y),
                .Size = New Size(320, 32),
                .Font = New Font("Segoe UI", 10),
                .DropDownStyle = ComboBoxStyle.DropDownList
            }
        End Function

        Private Sub RoleChanged(sender As Object, e As EventArgs)
            Dim roleNote As Label = TryCast(Me.Controls("roleNote"), Label)

            For i As Integer = 0 To clbPermissions.Items.Count - 1
                clbPermissions.SetItemChecked(i, False)
            Next

            If cmbRole.Text = "System Admin" Then
                lblPermissions.Visible = False
                clbPermissions.Visible = False

                If roleNote IsNot Nothing Then
                    roleNote.Text = "System Admin has full access to all features."
                    roleNote.Visible = True
                End If
            Else
                lblPermissions.Visible = True
                clbPermissions.Visible = True

                If roleNote IsNot Nothing Then roleNote.Visible = False

                If cmbRole.Text = "Admin" Then
                    'Admin can be given extra permissions manually by System Admin.
                End If
            End If
        End Sub

        Private Sub CheckPermission(permissionName As String)
            For i As Integer = 0 To clbPermissions.Items.Count - 1
                If clbPermissions.Items(i).ToString() = permissionName Then
                    clbPermissions.SetItemChecked(i, True)
                    Exit For
                End If
            Next
        End Sub

        Private Sub ConfirmClicked(sender As Object, e As EventArgs)
            Try
                If txtUsername.Text.Trim() = "" OrElse txtFullName.Text.Trim() = "" OrElse txtEmail.Text.Trim() = "" OrElse txtPassword.Text.Trim() = "" Then
                    MessageBox.Show("Please fill in all required fields.", "Add User")
                    Return
                End If

                Dim newUsername = txtUsername.Text.Trim()
                Dim newEmail = txtEmail.Text.Trim()

                If repo.UsernameExists(newUsername) Then
                    MessageBox.Show("Username already exists.", "Add User")
                    Return
                End If

                If repo.EmailExists(newEmail) Then
                    MessageBox.Show("Email already exists.", "Add User")
                    Return
                End If

                If Not txtEmail.Text.Contains("@") OrElse Not txtEmail.Text.Contains(".") Then
                    MessageBox.Show("Please enter a valid email address.", "Add User")
                    Return
                End If

                If txtPassword.Text.Length < 6 Then
                    MessageBox.Show("Password must be at least 6 characters.", "Add User")
                    Return
                End If

                Dim finalPermissions As String

                If cmbRole.Text = "System Admin" Then
                    finalPermissions = "Full Access"
                Else
                    Dim selectedPermissions As New List(Of String)

                    For Each item In clbPermissions.CheckedItems
                        selectedPermissions.Add(item.ToString())
                    Next

                    finalPermissions = If(selectedPermissions.Count = 0, "None", String.Join(", ", selectedPermissions))
                End If

                repo.AddUser(
                    newUsername,
                    txtFullName.Text.Trim(),
                    newEmail,
                    txtPassword.Text,
                    cmbRole.Text,
                    finalPermissions
                )

                Me.DialogResult = DialogResult.OK
                Me.Close()

            Catch ex As Exception
                MessageBox.Show("Failed to add user: " & ex.Message, "Database Error")
            End Try
        End Sub
    End Class

    Private Class ViewUsersDialog
        Inherits Form

        Private dgv As DataGridView
        Private btnRemove As Button
        Private btnClose As Button
        Private repo As InventoryBackend.UserRepository

        Public Sub New(userRepo As InventoryBackend.UserRepository)
            repo = userRepo

            Me.Text = "View All Users"
            Me.Size = New Size(760, 430)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = Color.FromArgb(253, 241, 226)

            BuildUI()
            RefreshGrid()
        End Sub

        Private Sub BuildUI()
            Dim dolphin As Color = Color.FromArgb(101, 90, 124)
            Dim linen As Color = Color.FromArgb(253, 241, 226)

            Dim title As New Label With {
                .Text = "All Users",
                .Font = New Font("Segoe UI", 17, FontStyle.Bold),
                .ForeColor = dolphin,
                .Location = New Point(30, 25),
                .AutoSize = True
            }

            dgv = New DataGridView With {
                .Location = New Point(30, 75),
                .Size = New Size(690, 235),
                .BackgroundColor = Color.White,
                .BorderStyle = BorderStyle.None,
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                .RowHeadersVisible = False,
                .AllowUserToAddRows = False,
                .AllowUserToDeleteRows = False,
                .AllowUserToResizeRows = False,
                .AllowUserToResizeColumns = False,
                .ReadOnly = True,
                .MultiSelect = False,
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                .Font = New Font("Segoe UI", 10)
            }

            dgv.EnableHeadersVisualStyles = False
            dgv.ColumnHeadersDefaultCellStyle.BackColor = dolphin
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = linen
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 220, 245)
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 38, 55)

            btnRemove = New Button With {
                .Text = "Remove User",
                .Location = New Point(485, 335),
                .Size = New Size(110, 38),
                .BackColor = Color.FromArgb(180, 85, 95),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold)
            }
            btnRemove.FlatAppearance.BorderSize = 0
            AddHandler btnRemove.Click, AddressOf RemoveClicked

            btnClose = New Button With {
                .Text = "Close",
                .Location = New Point(610, 335),
                .Size = New Size(110, 38),
                .BackColor = Color.Gray,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold)
            }
            btnClose.FlatAppearance.BorderSize = 0
            AddHandler btnClose.Click, Sub() Me.Close()

            Me.Controls.AddRange(New Control() {
                title,
                dgv,
                btnRemove,
                btnClose
            })
        End Sub

        Private Sub RefreshGrid()
            Try
                dgv.DataSource = Nothing

                dgv.DataSource = repo.GetAllUsers().Select(Function(u) New With {
                    .Username = u.Username,
                    .FullName = u.FullName,
                    .Email = u.Email,
                    .Role = u.Role,
                    .Permissions = u.Permissions
                }).ToList()

                dgv.ClearSelection()
                dgv.CurrentCell = Nothing

            Catch ex As Exception
                MessageBox.Show("Failed to load users: " & ex.Message, "Database Error")
            End Try
        End Sub

        Private Sub RemoveClicked(sender As Object, e As EventArgs)
            If dgv.CurrentRow Is Nothing Then
                MessageBox.Show("Select a user first.", "Remove User")
                Return
            End If

            Dim username = dgv.CurrentRow.Cells("Username").Value.ToString()

            If username.ToLower() = "admin" Then
                MessageBox.Show("The main System Admin account cannot be removed.", "Remove User")
                Return
            End If

            If MessageBox.Show("Remove selected user?", "Remove User", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    repo.RemoveUser(username)
                    RefreshGrid()
                    MessageBox.Show("User removed successfully.", "Remove User")
                Catch ex As Exception
                    MessageBox.Show("Failed to remove user: " & ex.Message, "Database Error")
                End Try
            End If
        End Sub
    End Class

    Private Class ResetPasswordDialog
        Inherits Form

        Private repo As InventoryBackend.UserRepository
        Private dgv As DataGridView
        Private txtNewPassword As TextBox
        Private btnReset As Button
        Private btnCancel As Button

        Public Sub New(userRepo As InventoryBackend.UserRepository)
            repo = userRepo

            Me.Text = "Reset Password"
            Me.Size = New Size(760, 470)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = Color.FromArgb(253, 241, 226)

            BuildUI()
            RefreshGrid()
        End Sub

        Private Sub BuildUI()
            Dim dolphin As Color = Color.FromArgb(101, 90, 124)
            Dim linen As Color = Color.FromArgb(253, 241, 226)

            Dim title As New Label With {
                .Text = "Reset Password",
                .Font = New Font("Segoe UI", 17, FontStyle.Bold),
                .ForeColor = dolphin,
                .Location = New Point(30, 25),
                .AutoSize = True
            }

            dgv = New DataGridView With {
                .Location = New Point(30, 75),
                .Size = New Size(690, 205),
                .BackgroundColor = Color.White,
                .BorderStyle = BorderStyle.None,
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                .RowHeadersVisible = False,
                .AllowUserToAddRows = False,
                .AllowUserToDeleteRows = False,
                .ReadOnly = True,
                .MultiSelect = False,
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                .Font = New Font("Segoe UI", 10)
            }

            dgv.EnableHeadersVisualStyles = False
            dgv.ColumnHeadersDefaultCellStyle.BackColor = dolphin
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = linen
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 220, 245)
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 38, 55)

            Dim lblPass As New Label With {
                .Text = "New Temporary Password",
                .Location = New Point(30, 305),
                .AutoSize = True,
                .Font = New Font("Segoe UI", 9),
                .ForeColor = Color.FromArgb(45, 38, 55)
            }

            txtNewPassword = New TextBox With {
                .Location = New Point(30, 328),
                .Size = New Size(300, 32),
                .Font = New Font("Segoe UI", 10),
                .UseSystemPasswordChar = True
            }

            btnReset = New Button With {
                .Text = "Reset Password",
                .Location = New Point(465, 375),
                .Size = New Size(130, 38),
                .BackColor = dolphin,
                .ForeColor = linen,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold)
            }
            btnReset.FlatAppearance.BorderSize = 0
            AddHandler btnReset.Click, AddressOf ResetClicked

            btnCancel = New Button With {
                .Text = "Cancel",
                .Location = New Point(610, 375),
                .Size = New Size(110, 38),
                .BackColor = Color.Gray,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold)
            }
            btnCancel.FlatAppearance.BorderSize = 0
            AddHandler btnCancel.Click, Sub() Me.Close()

            Me.Controls.AddRange(New Control() {
                title,
                dgv,
                lblPass,
                txtNewPassword,
                btnReset,
                btnCancel
            })
        End Sub

        Private Sub RefreshGrid()
            Try
                dgv.DataSource = Nothing

                dgv.DataSource = repo.GetAllUsers().Select(Function(u) New With {
                    .Username = u.Username,
                    .Email = u.Email,
                    .Role = u.Role
                }).ToList()

                dgv.ClearSelection()
                dgv.CurrentCell = Nothing

            Catch ex As Exception
                MessageBox.Show("Failed to load users: " & ex.Message, "Database Error")
            End Try
        End Sub

        Private Sub ResetClicked(sender As Object, e As EventArgs)
            If dgv.CurrentRow Is Nothing Then
                MessageBox.Show("Select a user first.", "Reset Password")
                Return
            End If

            If txtNewPassword.Text.Trim() = "" Then
                MessageBox.Show("Enter a new temporary password.", "Reset Password")
                Return
            End If

            If txtNewPassword.Text.Length < 6 Then
                MessageBox.Show("Temporary password must be at least 6 characters.", "Reset Password")
                Return
            End If

            Dim username = dgv.CurrentRow.Cells("Username").Value.ToString()

            If MessageBox.Show("Reset password for " & username & "?", "Reset Password", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Try
                repo.ResetPassword(username, txtNewPassword.Text)
                MessageBox.Show("Password reset successfully.", "Reset Password")

                txtNewPassword.Clear()
                dgv.ClearSelection()
                dgv.CurrentCell = Nothing

            Catch ex As Exception
                MessageBox.Show("Failed to reset password: " & ex.Message, "Database Error")
            End Try
        End Sub
    End Class

End Class
