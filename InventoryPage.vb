Imports InventoryBackend

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
    Private repo As New InventoryBackend.InventoryRepository()

    Private Class InventoryItem
        Public Property Selected As Boolean
        Public Property ItemID As String
        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String
        Public Property DateAdded As Date
    End Class

    Private Class InventoryGridRow
        Public Property Selected As Boolean
        Public Property ItemID As String
        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String
        Public Property DateAddedDisplay As String
    End Class

    Private Sub InventoryPage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(248, 244, 250)
        LoadFromDatabase()
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
        AddHandler btnDelete.Click, Sub()
                                        If Not InventoryBackend.SessionManager.CanDeleteItems() Then
                                            MessageBox.Show("You do not have permission to delete inventory items.", "Access Denied")
                                            Return
                                        End If
                                        SetMode("Delete")
                                    End Sub

        Me.Controls.AddRange(New Control() {btnAdd, btnEdit, btnDelete})
        ApplyRolePermissions()

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

        Dim btnWidth As Integer = 110
        Dim gap As Integer = 12
        Dim rightMargin As Integer = 24
        Dim topY As Integer = 85

        btnAdd.Size = New Size(btnWidth, 36)
        btnEdit.Size = New Size(btnWidth, 36)
        btnDelete.Size = New Size(btnWidth, 36)

        btnDelete.Location = New Point(Me.Width - rightMargin - btnWidth, topY)
        btnEdit.Location = New Point(btnDelete.Left - gap - btnWidth, topY)
        btnAdd.Location = New Point(btnEdit.Left - gap - btnWidth, topY)

        If dgvInventory IsNot Nothing Then
            dgvInventory.Size = New Size(Me.Width - 48, Me.Height - 200)
        End If

        If actionPanel IsNot Nothing Then
            actionPanel.Size = New Size(270, 45)
            actionPanel.Location = New Point(Me.Width - 294, Me.Height - 55)

            btnConfirm.Size = New Size(145, 36)
            btnCancel.Size = New Size(100, 36)
            btnConfirm.Location = New Point(0, 4)
            btnCancel.Location = New Point(158, 4)
        End If
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

    Private Sub ApplyRolePermissions()
        If btnDelete Is Nothing Then Return

        btnDelete.Visible = InventoryBackend.SessionManager.CanDeleteItems()
        btnDelete.Enabled = InventoryBackend.SessionManager.CanDeleteItems()
    End Sub

    Private Sub LoadFromDatabase()
        Try
            items.Clear()

            Dim dbItems = repo.GetAll()

            For Each x In dbItems
                items.Add(New InventoryItem With {
                    .Selected = False,
                    .ItemID = x.ItemID,
                    .ItemName = x.ItemName,
                    .Category = x.Category,
                    .Quantity = x.Quantity,
                    .Status = x.Status,
                    .DateAdded = x.DateAdded
                })
            Next

        Catch ex As Exception
            MessageBox.Show("Failed to load inventory: " & ex.Message, "Database Error")
        End Try
    End Sub

    Private Sub RefreshGrid()
        If dgvInventory Is Nothing Then Return

        Dim keyword = If(txtSearch Is Nothing, "", txtSearch.Text.Trim().ToLower())

        Dim filteredItems = items.Where(Function(x)
                                            Return keyword = "" OrElse
                                                x.ItemID.ToLower().Contains(keyword) OrElse
                                                x.ItemName.ToLower().Contains(keyword) OrElse
                                                x.Category.ToLower().Contains(keyword) OrElse
                                                x.Status.ToLower().Contains(keyword) OrElse
                                                x.DateAdded.ToString("yyyy-MM-dd").Contains(keyword)
                                        End Function).ToList()

        Dim displayList = filteredItems.Select(Function(x) New InventoryGridRow With {
            .Selected = x.Selected,
            .ItemID = x.ItemID,
            .ItemName = x.ItemName,
            .Category = x.Category,
            .Quantity = x.Quantity,
            .Status = x.Status,
            .DateAddedDisplay = x.DateAdded.ToString("yyyy-MM-dd")
        }).ToList()

        dgvInventory.DataSource = Nothing
        dgvInventory.DataSource = displayList
    End Sub

    Private Sub GridReady(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        If dgvInventory.Columns.Contains("Selected") Then
            dgvInventory.Columns("Selected").HeaderText = ""
            dgvInventory.Columns("Selected").Width = 45
            dgvInventory.Columns("Selected").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            dgvInventory.Columns("Selected").Visible = mode <> "View"
            dgvInventory.Columns("Selected").ReadOnly = False
        End If

        If dgvInventory.Columns.Contains("ItemID") Then dgvInventory.Columns("ItemID").HeaderText = "Item ID"
        If dgvInventory.Columns.Contains("ItemName") Then dgvInventory.Columns("ItemName").HeaderText = "Item Name"
        If dgvInventory.Columns.Contains("DateAddedDisplay") Then dgvInventory.Columns("DateAddedDisplay").HeaderText = "Date Added"

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

        For Each item In items
            item.Selected = False
        Next

        For Each row As DataGridViewRow In dgvInventory.Rows
            If row.Cells("Selected").Value IsNot Nothing AndAlso CBool(row.Cells("Selected").Value) Then
                Dim itemId = row.Cells("ItemID").Value.ToString()
                Dim selectedItem = items.FirstOrDefault(Function(x) x.ItemID = itemId)
                If selectedItem IsNot Nothing Then selectedItem.Selected = True
            End If
        Next

        Return items.Where(Function(x) x.Selected).ToList()
    End Function

    Private Sub AddClicked(sender As Object, e As EventArgs)
        SetMode("View")

        Using dialog As New ItemDialog()
            If dialog.ShowDialog() = DialogResult.OK Then
                Try
                    Dim newItem As New InventoryBackend.InventoryItem With {
                        .ItemID = repo.GenerateNextItemId(),
                        .ItemName = dialog.ItemName,
                        .Category = dialog.Category,
                        .Quantity = dialog.Quantity,
                        .Status = dialog.Status,
                        .DateAdded = dialog.DateAdded
                    }

                    repo.Add(newItem)
                    LoadFromDatabase()
                    RefreshGrid()

                Catch ex As Exception
                    MessageBox.Show("Failed to add item: " & ex.Message, "Database Error")
                End Try
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
                Try
                    For Each item In selected
                        Dim updatedItem As New InventoryBackend.InventoryItem With {
                            .ItemID = item.ItemID,
                            .ItemName = item.ItemName,
                            .Category = item.Category,
                            .Quantity = item.Quantity,
                            .Status = item.Status,
                            .DateAdded = item.DateAdded
                        }

                        repo.Update(updatedItem)
                    Next

                    LoadFromDatabase()
                    RefreshGrid()
                    SetMode("View")

                Catch ex As Exception
                    MessageBox.Show("Failed to update item: " & ex.Message, "Database Error")
                End Try
            Else
                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub ConfirmDelete()
        If Not InventoryBackend.SessionManager.CanDeleteItems() Then
            MessageBox.Show("You do not have permission to delete inventory items.", "Access Denied")
            SetMode("View")
            Return
        End If

        Dim selected = CheckedItems()

        If selected.Count = 0 Then
            MessageBox.Show("Check at least one item first.", "Delete Item")
            Return
        End If

        Using dialog As New DeleteConfirmDialog(selected.Count)
            If dialog.ShowDialog() = DialogResult.Yes Then
                Try
                    For Each item In selected
                        repo.Delete(item.ItemID)
                    Next

                    LoadFromDatabase()
                    RefreshGrid()
                    SetMode("View")

                Catch ex As Exception
                    MessageBox.Show("Failed to delete item: " & ex.Message, "Database Error")
                End Try
            End If
        End Using
    End Sub

    Private Class ItemDialog
        Inherits Form

        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String
        Public Property DateAdded As Date

        Private txtName As TextBox
        Private cmbCategory As ComboBox
        Private qty As NumericUpDown
        Private cmbStatus As ComboBox
        Private dtpDateAdded As DateTimePicker

        Public Sub New(Optional existing As InventoryItem = Nothing)
            Me.Text = If(existing Is Nothing, "Add Item", "Edit Item")
            Me.Size = New Size(380, 420)
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

            dtpDateAdded = New DateTimePicker With {
                .Location = New Point(35, 295),
                .Size = New Size(290, 30),
                .Format = DateTimePickerFormat.Short
            }

            If existing IsNot Nothing Then
                txtName.Text = existing.ItemName
                cmbCategory.Text = existing.Category
                qty.Value = existing.Quantity
                cmbStatus.Text = existing.Status
                dtpDateAdded.Value = existing.DateAdded
            Else
                cmbCategory.SelectedIndex = 0
                qty.Value = 1
                cmbStatus.SelectedIndex = 0
                dtpDateAdded.Value = Date.Today
            End If

            Dim save As New Button With {.Text = "Save", .Location = New Point(145, 345), .Size = New Size(85, 35), .BackColor = Color.FromArgb(101, 90, 124), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
            Dim cancel As New Button With {.Text = "Cancel", .Location = New Point(240, 345), .Size = New Size(85, 35), .BackColor = Color.Gray, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}

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
                New Label With {.Text = "Date Added", .Location = New Point(35, 270)},
                dtpDateAdded,
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
            DateAdded = dtpDateAdded.Value.Date

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class

    Private Class MultiEditDialog
        Inherits Form

        Private selectedItems As List(Of InventoryItem)
        Private itemControls As New Dictionary(Of InventoryItem, Tuple(Of TextBox, ComboBox, NumericUpDown, ComboBox, DateTimePicker))

        Public Sub New(itemsToEdit As List(Of InventoryItem))
            selectedItems = itemsToEdit

            Me.Text = "Edit Item(s)"
            Me.Size = New Size(540, 540)
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
                .Size = New Size(470, 360),
                .AutoScroll = True,
                .BackColor = Color.White
            }

            Dim y As Integer = 15

            For Each item In selectedItems
                Dim box As New GroupBox With {
                    .Text = item.ItemID,
                    .Location = New Point(15, y),
                    .Size = New Size(410, 220),
                    .ForeColor = textDark
                }

                Dim txtName As New TextBox With {.Text = item.ItemName, .Location = New Point(20, 35), .Size = New Size(350, 25)}

                Dim cmbCategory As New ComboBox With {.Location = New Point(20, 70), .Size = New Size(350, 25), .DropDownStyle = ComboBoxStyle.DropDownList}
                cmbCategory.Items.AddRange(New String() {"Laptop", "Desktop Computer", "Monitor", "Keyboard", "Mouse", "Projector", "Printer", "Router / Network Device", "Storage Device", "Cable / Adapter", "Software License", "Other"})
                cmbCategory.Text = item.Category

                Dim numQty As New NumericUpDown With {.Location = New Point(20, 105), .Size = New Size(350, 25), .Minimum = 0, .Maximum = 100000, .Value = item.Quantity}

                Dim cmbStatus As New ComboBox With {.Location = New Point(20, 140), .Size = New Size(350, 25), .DropDownStyle = ComboBoxStyle.DropDownList}
                cmbStatus.Items.AddRange(New String() {"Available", "In use", "Defective", "Under Repair", "Missing"})
                cmbStatus.Text = item.Status

                Dim dtpDateAdded As New DateTimePicker With {
                    .Location = New Point(20, 175),
                    .Size = New Size(350, 25),
                    .Format = DateTimePickerFormat.Short,
                    .Value = item.DateAdded
                }

                box.Controls.AddRange(New Control() {txtName, cmbCategory, numQty, cmbStatus, dtpDateAdded})
                scrollPanel.Controls.Add(box)

                itemControls(item) = Tuple.Create(txtName, cmbCategory, numQty, cmbStatus, dtpDateAdded)
                y += 235
            Next

            Dim save As New Button With {
                .Text = "Save Changes",
                .Location = New Point(270, 445),
                .Size = New Size(120, 36),
                .BackColor = dolphin,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            Dim cancel As New Button With {
                .Text = "Cancel",
                .Location = New Point(400, 445),
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
                Dim dtpDateAdded = controls.Item5

                If txtName.Text.Trim() = "" Then
                    MessageBox.Show("Item name cannot be blank.")
                    Return
                End If

                item.ItemName = txtName.Text.Trim()
                item.Category = cmbCategory.Text
                item.Quantity = CInt(numQty.Value)
                item.Status = cmbStatus.Text
                item.DateAdded = dtpDateAdded.Value.Date
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
