Public Class InventoryPage

    Private dgvInventory As DataGridView
    Private txtSearch As TextBox
    Private btnAdd As Button
    Private btnEdit As Button
    Private btnDelete As Button
    Private actionPanel As Panel
    Private btnConfirm As Button
    Private btnCancel As Button

    Private mode As String = "View"
    Private items As New List(Of InventoryItem)

    Private Class InventoryItem
        Public Property Selected As Boolean
        Public Property ItemID As String
        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String
    End Class

    Private Sub InventoryPage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(248, 244, 250)
        LoadSampleData()
        BuildUI()
        RefreshGrid()
    End Sub

    Private Sub BuildUI()
        Me.Controls.Clear()

        Dim title As New Label With {
            .Text = "Inventory",
            .Font = New Font("Segoe UI Semilight", 22),
            .ForeColor = Color.FromArgb(101, 90, 124),
            .Location = New Point(24, 20),
            .AutoSize = True
        }
        Me.Controls.Add(title)

        txtSearch = New TextBox With {
            .PlaceholderText = "Search item...",
            .Location = New Point(24, 85),
            .Size = New Size(280, 30),
            .Font = New Font("Segoe UI", 10)
        }
        AddHandler txtSearch.TextChanged, AddressOf SearchChanged
        Me.Controls.Add(txtSearch)

        btnAdd = CreateButton("Add Item", Color.FromArgb(101, 90, 124))
        btnEdit = CreateButton("Edit", Color.FromArgb(171, 146, 191))
        btnDelete = CreateButton("Delete", Color.FromArgb(180, 85, 95))

        AddHandler btnAdd.Click, AddressOf AddClicked
        AddHandler btnEdit.Click, Sub() SetMode("Edit")
        AddHandler btnDelete.Click, Sub() SetMode("Delete")

        Me.Controls.AddRange(New Control() {btnAdd, btnEdit, btnDelete})

        dgvInventory = New DataGridView With {
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
            .ReadOnly = False,
            .MultiSelect = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .Font = New Font("Segoe UI", 10)
        }

        dgvInventory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(101, 90, 124)
        dgvInventory.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(253, 241, 226)
        dgvInventory.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(101, 90, 124)
        dgvInventory.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(253, 241, 226)
        dgvInventory.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        dgvInventory.EnableHeadersVisualStyles = False

        dgvInventory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 220, 245)
        dgvInventory.DefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 38, 55)
        dgvInventory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 248, 255)
        dgvInventory.GridColor = Color.FromArgb(220, 210, 225)

        AddHandler dgvInventory.DataBindingComplete, AddressOf GridReady
        AddHandler dgvInventory.CurrentCellChanged, AddressOf ClearBadSelection

        Me.Controls.Add(dgvInventory)

        actionPanel = New Panel With {.Visible = False, .BackColor = Color.Transparent}
        btnConfirm = CreateButton("Confirm", Color.FromArgb(101, 90, 124))
        btnCancel = CreateButton("Cancel", Color.Gray)

        AddHandler btnConfirm.Click, AddressOf ConfirmClicked
        AddHandler btnCancel.Click, Sub() SetMode("View")

        actionPanel.Controls.AddRange(New Control() {btnConfirm, btnCancel})
        Me.Controls.Add(actionPanel)

        LayoutControls()
        AddHandler Me.Resize, Sub() LayoutControls()
    End Sub

    Private Sub LayoutControls()
        If btnAdd Is Nothing Then Return

        btnDelete.Location = New Point(Me.Width - 134, 85)
        btnEdit.Location = New Point(btnDelete.Left - 122, 85)
        btnAdd.Location = New Point(btnEdit.Left - 132, 85)

        dgvInventory.Size = New Size(Me.Width - 48, Me.Height - 200)

        actionPanel.Size = New Size(270, 45)
        actionPanel.Location = New Point(Me.Width - 294, Me.Height - 55)

        btnConfirm.Size = New Size(145, 36)
        btnCancel.Size = New Size(100, 36)
        btnConfirm.Location = New Point(0, 4)
        btnCancel.Location = New Point(158, 4)
    End Sub

    Private Function CreateButton(text As String, bg As Color) As Button
        Dim btn As New Button With {
            .Text = text,
            .Size = New Size(110, 36),
            .BackColor = bg,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }

        btn.FlatAppearance.BorderSize = 0
        Return btn
    End Function

    Private Sub LoadSampleData()
        If items.Count > 0 Then Return

        items.Add(New InventoryItem With {.ItemID = "ITM-001", .ItemName = "Laptop", .Category = "Laptop", .Quantity = 12, .Status = "Available"})
        items.Add(New InventoryItem With {.ItemID = "ITM-002", .ItemName = "Projector", .Category = "Projector", .Quantity = 5, .Status = "Available"})
        items.Add(New InventoryItem With {.ItemID = "ITM-003", .ItemName = "Keyboard", .Category = "Keyboard", .Quantity = 25, .Status = "Available"})
        items.Add(New InventoryItem With {.ItemID = "ITM-004", .ItemName = "Mouse", .Category = "Mouse", .Quantity = 30, .Status = "Available"})
    End Sub

    Private Sub RefreshGrid()
        If dgvInventory Is Nothing Then Return

        Dim keyword = If(txtSearch Is Nothing, "", txtSearch.Text.Trim().ToLower())

        Dim filtered = items.Where(Function(x)
                                       Return keyword = "" OrElse
                                           x.ItemID.ToLower().Contains(keyword) OrElse
                                           x.ItemName.ToLower().Contains(keyword) OrElse
                                           x.Category.ToLower().Contains(keyword) OrElse
                                           x.Status.ToLower().Contains(keyword)
                                   End Function).ToList()

        dgvInventory.DataSource = Nothing
        dgvInventory.DataSource = filtered
    End Sub

    Private Sub GridReady(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        If dgvInventory.Columns.Contains("Selected") Then
            dgvInventory.Columns("Selected").HeaderText = ""
            dgvInventory.Columns("Selected").Width = 45
            dgvInventory.Columns("Selected").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            dgvInventory.Columns("Selected").Visible = mode <> "View"
            dgvInventory.Columns("Selected").ReadOnly = False
        End If

        For Each col As DataGridViewColumn In dgvInventory.Columns
            If col.Name <> "Selected" Then col.ReadOnly = True
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next

        dgvInventory.ClearSelection()
        dgvInventory.CurrentCell = Nothing
    End Sub

    Private Sub ClearBadSelection(sender As Object, e As EventArgs)
        If dgvInventory Is Nothing Then Return

        If dgvInventory.CurrentCell IsNot Nothing AndAlso dgvInventory.CurrentCell.OwningColumn.Name <> "Selected" Then
            dgvInventory.Rows(dgvInventory.CurrentCell.RowIndex).Selected = True
        End If
    End Sub

    Private Sub SearchChanged(sender As Object, e As EventArgs)
        RefreshGrid()
    End Sub

    Private Sub SetMode(newMode As String)
        mode = newMode

        For Each item In items
            item.Selected = False
        Next

        actionPanel.Visible = mode <> "View"

        If mode = "Edit" Then
            btnConfirm.Text = "Edit Selected"
            btnConfirm.BackColor = Color.FromArgb(101, 90, 124)
        ElseIf mode = "Delete" Then
            btnConfirm.Text = "Delete Selected"
            btnConfirm.BackColor = Color.FromArgb(180, 85, 95)
        End If

        RefreshGrid()
    End Sub

    Private Function CheckedItems() As List(Of InventoryItem)
        dgvInventory.EndEdit()
        Return items.Where(Function(x) x.Selected).ToList()
    End Function

    Private Sub AddClicked(sender As Object, e As EventArgs)
        SetMode("View")

        Using dialog As New ItemDialog()
            If dialog.ShowDialog() = DialogResult.OK Then
                items.Add(New InventoryItem With {
                    .Selected = False,
                    .ItemID = "ITM-" & (items.Count + 1).ToString("000"),
                    .ItemName = dialog.ItemName,
                    .Category = dialog.Category,
                    .Quantity = dialog.Quantity,
                    .Status = dialog.Status
                })

                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub ConfirmClicked(sender As Object, e As EventArgs)
        If mode = "Edit" Then
            ConfirmEdit()
        ElseIf mode = "Delete" Then
            ConfirmDelete()
        End If
    End Sub

    Private Sub ConfirmEdit()
        Dim selected = CheckedItems()

        If selected.Count = 0 Then
            MessageBox.Show("Check at least one item first.", "Edit Item")
            Return
        End If

        Using dialog As New MultiEditDialog(selected)
            If dialog.ShowDialog() = DialogResult.OK Then
                SetMode("View")
            Else
                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub ConfirmDelete()
        Dim selected = CheckedItems()

        If selected.Count = 0 Then
            MessageBox.Show("Check at least one item first.", "Delete Item")
            Return
        End If

        Using dialog As New DeleteConfirmDialog(selected.Count)
            If dialog.ShowDialog() = DialogResult.Yes Then
                For Each item In selected
                    items.Remove(item)
                Next

                SetMode("View")
            End If
        End Using
    End Sub

    Private Class ItemDialog
        Inherits Form

        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String

        Private txtName As TextBox
        Private cmbCategory As ComboBox
        Private qty As NumericUpDown
        Private cmbStatus As ComboBox

        Public Sub New(Optional existing As InventoryItem = Nothing)
            Me.Text = If(existing Is Nothing, "Add Item", "Edit Item")
            Me.Size = New Size(380, 360)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = Color.FromArgb(253, 241, 226)

            txtName = New TextBox With {.Location = New Point(35, 55), .Size = New Size(290, 30)}
            cmbCategory = New ComboBox With {.Location = New Point(35, 115), .Size = New Size(290, 30), .DropDownStyle = ComboBoxStyle.DropDownList}
            cmbCategory.Items.AddRange(New String() {"Laptop", "Desktop Computer", "Monitor", "Keyboard", "Mouse", "Projector", "Printer", "Router / Network Device", "Storage Device", "Cable / Adapter", "Software License", "Other"})

            qty = New NumericUpDown With {.Location = New Point(35, 175), .Size = New Size(290, 30), .Minimum = 0, .Maximum = 100000}
            cmbStatus = New ComboBox With {.Location = New Point(35, 235), .Size = New Size(290, 30), .DropDownStyle = ComboBoxStyle.DropDownList}
            cmbStatus.Items.AddRange(New String() {"Available", "In use", "Defective", "Under Repair", "Missing"})

            If existing IsNot Nothing Then
                txtName.Text = existing.ItemName
                cmbCategory.Text = existing.Category
                qty.Value = existing.Quantity
                cmbStatus.Text = existing.Status
            Else
                cmbCategory.SelectedIndex = 0
                qty.Value = 1
                cmbStatus.SelectedIndex = 0
            End If

            Dim save As New Button With {.Text = "Save", .Location = New Point(145, 285), .Size = New Size(85, 35), .BackColor = Color.FromArgb(101, 90, 124), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
            Dim cancel As New Button With {.Text = "Cancel", .Location = New Point(240, 285), .Size = New Size(85, 35), .BackColor = Color.Gray, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}

            save.FlatAppearance.BorderSize = 0
            cancel.FlatAppearance.BorderSize = 0

            AddHandler save.Click, AddressOf SaveClicked
            AddHandler cancel.Click, Sub()
                                         Me.DialogResult = DialogResult.Cancel
                                         Me.Close()
                                     End Sub

            Me.Controls.AddRange(New Control() {
                New Label With {.Text = "Item Name", .Location = New Point(35, 30)},
                txtName,
                New Label With {.Text = "Category", .Location = New Point(35, 90)},
                cmbCategory,
                New Label With {.Text = "Quantity", .Location = New Point(35, 150)},
                qty,
                New Label With {.Text = "Status", .Location = New Point(35, 210)},
                cmbStatus,
                save,
                cancel
            })
        End Sub

        Private Sub SaveClicked(sender As Object, e As EventArgs)
            If txtName.Text.Trim() = "" Then
                MessageBox.Show("Item name is required.")
                Return
            End If

            ItemName = txtName.Text.Trim()
            Category = cmbCategory.Text
            Quantity = CInt(qty.Value)
            Status = cmbStatus.Text

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class

    Private Class MultiEditDialog
        Inherits Form

        Private selectedItems As List(Of InventoryItem)
        Private itemControls As New Dictionary(Of InventoryItem, Tuple(Of TextBox, ComboBox, NumericUpDown, ComboBox))

        Public Sub New(itemsToEdit As List(Of InventoryItem))
            selectedItems = itemsToEdit

            Me.Text = "Edit Item(s)"
            Me.Size = New Size(520, 500)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = Color.FromArgb(253, 241, 226)

            Dim dolphin As Color = Color.FromArgb(101, 90, 124)
            Dim textDark As Color = Color.FromArgb(45, 38, 55)

            Dim header As New Label With {
                .Text = "Edit Selected Item(s)",
                .Font = New Font("Segoe UI", 14, FontStyle.Bold),
                .ForeColor = dolphin,
                .Location = New Point(25, 18),
                .Size = New Size(350, 30)
            }

            Dim scrollPanel As New Panel With {
                .Location = New Point(25, 60),
                .Size = New Size(450, 330),
                .AutoScroll = True,
                .BackColor = Color.White
            }

            Dim y As Integer = 15

            For Each item In selectedItems
                Dim box As New GroupBox With {
                    .Text = item.ItemID,
                    .Location = New Point(15, y),
                    .Size = New Size(395, 185),
                    .ForeColor = textDark
                }

                Dim txtName As New TextBox With {.Text = item.ItemName, .Location = New Point(20, 35), .Size = New Size(340, 25)}

                Dim cmbCategory As New ComboBox With {.Location = New Point(20, 70), .Size = New Size(340, 25), .DropDownStyle = ComboBoxStyle.DropDownList}
                cmbCategory.Items.AddRange(New String() {"Laptop", "Desktop Computer", "Monitor", "Keyboard", "Mouse", "Projector", "Printer", "Router / Network Device", "Storage Device", "Cable / Adapter", "Software License", "Other"})
                cmbCategory.Text = item.Category

                Dim numQty As New NumericUpDown With {.Location = New Point(20, 105), .Size = New Size(340, 25), .Minimum = 0, .Maximum = 100000, .Value = item.Quantity}

                Dim cmbStatus As New ComboBox With {.Location = New Point(20, 140), .Size = New Size(340, 25), .DropDownStyle = ComboBoxStyle.DropDownList}
                cmbStatus.Items.AddRange(New String() {"Available", "In use", "Defective", "Under Repair", "Missing"})
                cmbStatus.Text = item.Status

                box.Controls.AddRange(New Control() {txtName, cmbCategory, numQty, cmbStatus})
                scrollPanel.Controls.Add(box)

                itemControls(item) = Tuple.Create(txtName, cmbCategory, numQty, cmbStatus)
                y += 200
            Next

            Dim save As New Button With {
                .Text = "Save Changes",
                .Location = New Point(250, 410),
                .Size = New Size(120, 36),
                .BackColor = dolphin,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            Dim cancel As New Button With {
                .Text = "Cancel",
                .Location = New Point(380, 410),
                .Size = New Size(95, 36),
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

            Me.Controls.AddRange(New Control() {header, scrollPanel, save, cancel})
        End Sub

        Private Sub SaveClicked(sender As Object, e As EventArgs)
            For Each item In selectedItems
                Dim controls = itemControls(item)

                Dim txtName = controls.Item1
                Dim cmbCategory = controls.Item2
                Dim numQty = controls.Item3
                Dim cmbStatus = controls.Item4

                If txtName.Text.Trim() = "" Then
                    MessageBox.Show("Item name cannot be blank.")
                    Return
                End If

                item.ItemName = txtName.Text.Trim()
                item.Category = cmbCategory.Text
                item.Quantity = CInt(numQty.Value)
                item.Status = cmbStatus.Text
                item.Selected = False
            Next

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class

    Private Class DeleteConfirmDialog
        Inherits Form

        Public Sub New(count As Integer)
            Dim linen As Color = Color.FromArgb(253, 241, 226)
            Dim dolphin As Color = Color.FromArgb(101, 90, 124)
            Dim danger As Color = Color.FromArgb(180, 85, 95)
            Dim textDark As Color = Color.FromArgb(45, 38, 55)

            Me.Text = "Confirm Delete"
            Me.Size = New Size(370, 220)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = linen

            Dim title As New Label With {
                .Text = "Delete Item(s)?",
                .Font = New Font("Segoe UI", 15, FontStyle.Bold),
                .ForeColor = danger,
                .Location = New Point(30, 25),
                .Size = New Size(280, 35)
            }

            Dim message As New Label With {
                .Text = "You selected " & count.ToString() & " item(s). This action cannot be undone.",
                .Font = New Font("Segoe UI", 10),
                .ForeColor = textDark,
                .Location = New Point(32, 75),
                .Size = New Size(290, 45)
            }

            Dim deleteBtn As New Button With {
                .Text = "Delete",
                .Location = New Point(135, 140),
                .Size = New Size(90, 35),
                .BackColor = danger,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            Dim cancelBtn As New Button With {
                .Text = "Cancel",
                .Location = New Point(235, 140),
                .Size = New Size(90, 35),
                .BackColor = dolphin,
                .ForeColor = linen,
                .FlatStyle = FlatStyle.Flat
            }

            deleteBtn.FlatAppearance.BorderSize = 0
            cancelBtn.FlatAppearance.BorderSize = 0

            AddHandler deleteBtn.Click, Sub()
                                            Me.DialogResult = DialogResult.Yes
                                            Me.Close()
                                        End Sub

            AddHandler cancelBtn.Click, Sub()
                                            Me.DialogResult = DialogResult.No
                                            Me.Close()
                                        End Sub

            Me.Controls.AddRange(New Control() {title, message, deleteBtn, cancelBtn})
        End Sub
    End Class

End Class