Public Class SuppliersPage

    Private dgv As DataGridView
    Private txtSearch As TextBox
    Private btnAdd As Button
    Private btnEdit As Button
    Private btnDelete As Button

    Private suppliers As New List(Of SupplierItem)

    Private Class SupplierItem
        Public Property SupplierID As String
        Public Property SupplierName As String
        Public Property ContactPerson As String
        Public Property Phone As String
        Public Property Email As String
        Public Property Address As String
        Public Property Status As String
    End Class

    Private Sub SuppliersPage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(248, 244, 250)
        LoadSampleData()
        BuildUI()
        RefreshGrid()
    End Sub

    Private Sub BuildUI()
        Me.Controls.Clear()

        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim amethyst As Color = Color.FromArgb(171, 146, 191)
        Dim linen As Color = Color.FromArgb(253, 241, 226)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        Dim title As New Label With {
            .Text = "Suppliers",
            .Font = New Font("Segoe UI Semilight", 22),
            .ForeColor = dolphin,
            .Location = New Point(24, 20),
            .AutoSize = True
        }
        Me.Controls.Add(title)

        txtSearch = New TextBox With {
            .PlaceholderText = "Search supplier...",
            .Location = New Point(24, 85),
            .Size = New Size(280, 30),
            .Font = New Font("Segoe UI", 10)
        }
        AddHandler txtSearch.TextChanged, AddressOf SearchChanged
        Me.Controls.Add(txtSearch)

        btnAdd = CreateButton("Add Supplier", dolphin)
        btnEdit = CreateButton("Edit", amethyst)
        btnDelete = CreateButton("Delete", Color.FromArgb(180, 85, 95))

        AddHandler btnAdd.Click, AddressOf AddClicked
        AddHandler btnEdit.Click, AddressOf EditClicked
        AddHandler btnDelete.Click, AddressOf DeleteClicked

        Me.Controls.AddRange(New Control() {btnAdd, btnEdit, btnDelete})

        dgv = New DataGridView With {
            .Location = New Point(24, 140),
            .Size = New Size(Me.Width - 48, Me.Height - 200),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right,
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
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dolphin
        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = linen
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)

        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 220, 245)
        dgv.DefaultCellStyle.SelectionForeColor = textDark
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 248, 255)
        dgv.GridColor = Color.FromArgb(220, 210, 225)

        AddHandler dgv.DataBindingComplete, AddressOf GridReady
        Me.Controls.Add(dgv)

        LayoutControls()
        AddHandler Me.Resize, Sub() LayoutControls()
    End Sub

    Private Sub LayoutControls()
        If btnAdd Is Nothing Then Return

        Dim btnWidth As Integer = 120
        Dim gap As Integer = 12
        Dim rightMargin As Integer = 24
        Dim topY As Integer = 85

        ' Delete (rightmost)
        btnDelete.Size = New Size(btnWidth, 36)
        btnDelete.Location = New Point(Me.Width - rightMargin - btnWidth, topY)

        ' Edit
        btnEdit.Size = New Size(btnWidth, 36)
        btnEdit.Location = New Point(btnDelete.Left - gap - btnWidth, topY)

        ' Add
        btnAdd.Size = New Size(btnWidth, 36)
        btnAdd.Location = New Point(btnEdit.Left - gap - btnWidth, topY)

        If dgv IsNot Nothing Then
            dgv.Size = New Size(Me.Width - 48, Me.Height - 200)
        End If
    End Sub

    Private Function CreateButton(text As String, bg As Color) As Button
        Dim btn As New Button With {
            .Text = text,
            .Size = New Size(120, 36),
            .BackColor = bg,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }

        btn.FlatAppearance.BorderSize = 0
        Return btn
    End Function

    Private Sub LoadSampleData()
        If suppliers.Count > 0 Then Return

        suppliers.Add(New SupplierItem With {.SupplierID = "SUP-001", .SupplierName = "TechSource PH", .ContactPerson = "Mark Reyes", .Phone = "0917-123-4567", .Email = "techsource@email.com", .Address = "Quezon City", .Status = "Active"})
        suppliers.Add(New SupplierItem With {.SupplierID = "SUP-002", .SupplierName = "OfficePro Supplies", .ContactPerson = "Ana Santos", .Phone = "0928-222-3344", .Email = "officepro@email.com", .Address = "Manila", .Status = "Active"})
        suppliers.Add(New SupplierItem With {.SupplierID = "SUP-003", .SupplierName = "CompuParts Trading", .ContactPerson = "Leo Cruz", .Phone = "0915-555-6789", .Email = "compuparts@email.com", .Address = "Makati", .Status = "Inactive"})
    End Sub

    Private Sub RefreshGrid()
        If dgv Is Nothing Then Return

        Dim keyword = If(txtSearch Is Nothing, "", txtSearch.Text.Trim().ToLower())

        Dim filtered = suppliers.Where(Function(x)
                                           Return keyword = "" OrElse
                                               x.SupplierID.ToLower().Contains(keyword) OrElse
                                               x.SupplierName.ToLower().Contains(keyword) OrElse
                                               x.ContactPerson.ToLower().Contains(keyword) OrElse
                                               x.Phone.ToLower().Contains(keyword) OrElse
                                               x.Email.ToLower().Contains(keyword) OrElse
                                               x.Address.ToLower().Contains(keyword) OrElse
                                               x.Status.ToLower().Contains(keyword)
                                       End Function).ToList()

        dgv.DataSource = Nothing
        dgv.DataSource = filtered
    End Sub

    Private Sub GridReady(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        For Each col As DataGridViewColumn In dgv.Columns
            col.ReadOnly = True
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next

        dgv.ClearSelection()
        dgv.CurrentCell = Nothing
    End Sub

    Private Sub SearchChanged(sender As Object, e As EventArgs)
        RefreshGrid()
    End Sub

    Private Function GetSelectedSupplierID() As String
        If dgv.CurrentRow Is Nothing Then Return ""

        Dim value = dgv.CurrentRow.Cells("SupplierID").Value
        If value Is Nothing Then Return ""

        Return value.ToString()
    End Function

    Private Sub AddClicked(sender As Object, e As EventArgs)
        Using dialog As New SupplierDialog()
            If dialog.ShowDialog() = DialogResult.OK Then
                suppliers.Add(New SupplierItem With {
                    .SupplierID = "SUP-" & (suppliers.Count + 1).ToString("000"),
                    .SupplierName = dialog.SupplierName,
                    .ContactPerson = dialog.ContactPerson,
                    .Phone = dialog.Phone,
                    .Email = dialog.Email,
                    .Address = dialog.Address,
                    .Status = dialog.Status
                })

                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub EditClicked(sender As Object, e As EventArgs)
        Dim selectedId = GetSelectedSupplierID()

        If selectedId = "" Then
            MessageBox.Show("Select one supplier first.")
            Return
        End If

        Dim supplier = suppliers.FirstOrDefault(Function(x) x.SupplierID = selectedId)
        If supplier Is Nothing Then Return

        Using dialog As New SupplierDialog(supplier)
            If dialog.ShowDialog() = DialogResult.OK Then
                supplier.SupplierName = dialog.SupplierName
                supplier.ContactPerson = dialog.ContactPerson
                supplier.Phone = dialog.Phone
                supplier.Email = dialog.Email
                supplier.Address = dialog.Address
                supplier.Status = dialog.Status

                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub DeleteClicked(sender As Object, e As EventArgs)
        Dim selectedId = GetSelectedSupplierID()

        If selectedId = "" Then
            MessageBox.Show("Select one supplier first.")
            Return
        End If

        If MessageBox.Show("Delete selected supplier?", "Delete Supplier", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            suppliers.RemoveAll(Function(x) x.SupplierID = selectedId)
            RefreshGrid()
        End If
    End Sub

    Private Class SupplierDialog
        Inherits Form

        Public Property SupplierName As String
        Public Property ContactPerson As String
        Public Property Phone As String
        Public Property Email As String
        Public Property Address As String
        Public Property Status As String

        Private txtSupplierName As TextBox
        Private txtContactPerson As TextBox
        Private txtPhone As TextBox
        Private txtEmail As TextBox
        Private txtAddress As TextBox
        Private cmbStatus As ComboBox

        Public Sub New(Optional existing As SupplierItem = Nothing)
            Me.Text = If(existing Is Nothing, "Add Supplier", "Edit Supplier")
            Me.Size = New Size(430, 485)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.BackColor = Color.FromArgb(253, 241, 226)
            Me.MaximizeBox = False
            Me.MinimizeBox = False

            txtSupplierName = New TextBox With {.Location = New Point(40, 55), .Size = New Size(330, 30)}
            txtContactPerson = New TextBox With {.Location = New Point(40, 115), .Size = New Size(330, 30)}
            txtPhone = New TextBox With {.Location = New Point(40, 175), .Size = New Size(330, 30)}
            txtEmail = New TextBox With {.Location = New Point(40, 235), .Size = New Size(330, 30)}
            txtAddress = New TextBox With {.Location = New Point(40, 295), .Size = New Size(330, 30)}

            cmbStatus = New ComboBox With {
                .Location = New Point(40, 355),
                .Size = New Size(330, 30),
                .DropDownStyle = ComboBoxStyle.DropDownList
            }
            cmbStatus.Items.AddRange(New String() {"Active", "Inactive"})

            If existing IsNot Nothing Then
                txtSupplierName.Text = existing.SupplierName
                txtContactPerson.Text = existing.ContactPerson
                txtPhone.Text = existing.Phone
                txtEmail.Text = existing.Email
                txtAddress.Text = existing.Address
                cmbStatus.Text = existing.Status
            Else
                cmbStatus.SelectedIndex = 0
            End If

            Dim save As New Button With {
                .Text = "Save",
                .Location = New Point(190, 405),
                .Size = New Size(85, 35),
                .BackColor = Color.FromArgb(101, 90, 124),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            Dim cancel As New Button With {
                .Text = "Cancel",
                .Location = New Point(285, 405),
                .Size = New Size(85, 35),
                .BackColor = Color.Gray,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            save.FlatAppearance.BorderSize = 0
            cancel.FlatAppearance.BorderSize = 0

            AddHandler save.Click, AddressOf SaveClicked
            AddHandler cancel.Click, Sub()
                                         Me.DialogResult = DialogResult.Cancel
                                         Me.Close()
                                     End Sub

            Me.Controls.AddRange(New Control() {
                New Label With {.Text = "Supplier Name", .Location = New Point(40, 30)},
                txtSupplierName,
                New Label With {.Text = "Contact Person", .Location = New Point(40, 90)},
                txtContactPerson,
                New Label With {.Text = "Phone", .Location = New Point(40, 150)},
                txtPhone,
                New Label With {.Text = "Email", .Location = New Point(40, 210)},
                txtEmail,
                New Label With {.Text = "Address", .Location = New Point(40, 270)},
                txtAddress,
                New Label With {.Text = "Status", .Location = New Point(40, 330)},
                cmbStatus,
                save,
                cancel
            })
        End Sub

        Private Sub SaveClicked(sender As Object, e As EventArgs)
            If txtSupplierName.Text.Trim() = "" Then
                MessageBox.Show("Supplier name is required.")
                Return
            End If

            If txtContactPerson.Text.Trim() = "" Then
                MessageBox.Show("Contact person is required.")
                Return
            End If

            SupplierName = txtSupplierName.Text.Trim()
            ContactPerson = txtContactPerson.Text.Trim()
            Phone = txtPhone.Text.Trim()
            Email = txtEmail.Text.Trim()
            Address = txtAddress.Text.Trim()
            Status = cmbStatus.Text

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class

End Class