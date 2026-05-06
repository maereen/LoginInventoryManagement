Public Class HomeForm

    Private ReadOnly SideButtonHeight As Integer = 44
    Private ReadOnly SideButtonLeftPadding As New Padding(64, 0, 0, 0)

    Private currentView As String = "Dashboard"
    Private inventoryMode As String = "View"

    Private inventoryPanel As Panel
    Private txtSearchInventory As TextBox
    Private dgvInventory As DataGridView
    Private btnAddItem As Button
    Private btnEditItem As Button
    Private btnDeleteItem As Button

    Private actionPanel As Panel
    Private btnConfirmAction As Button
    Private btnCancelAction As Button

    Private inventoryItems As New List(Of InventoryItem)

    Private isDragging As Boolean = False
    Private dragStartPoint As Point

    Private Class InventoryItem
        Public Property Selected As Boolean
        Public Property ItemID As String
        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String
    End Class

    Private Sub HomeForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSampleInventory()
        ApplyCleanLayout()
        ShowDashboard()
    End Sub

    Private Sub HomeForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ApplyCleanLayout()

        If currentView = "Inventory" Then
            LayoutInventoryPanel()
        Else
            LayoutDashboardCards()
        End If
    End Sub

    Private Sub LoadSampleInventory()
        If inventoryItems.Count > 0 Then Return

        inventoryItems.Add(New InventoryItem With {.Selected = False, .ItemID = "ITM-001", .ItemName = "Laptop", .Category = "Laptop", .Quantity = 12, .Status = "Available"})
        inventoryItems.Add(New InventoryItem With {.Selected = False, .ItemID = "ITM-002", .ItemName = "Projector", .Category = "Projector", .Quantity = 5, .Status = "Available"})
        inventoryItems.Add(New InventoryItem With {.Selected = False, .ItemID = "ITM-003", .ItemName = "Keyboard", .Category = "Keyboard", .Quantity = 25, .Status = "Available"})
        inventoryItems.Add(New InventoryItem With {.Selected = False, .ItemID = "ITM-004", .ItemName = "Mouse", .Category = "Mouse", .Quantity = 30, .Status = "Available"})
    End Sub

    Private Sub ApplyCleanLayout()
        Panel1.Width = 250
        Panel1.Dock = DockStyle.Left

        Panel2.Dock = DockStyle.Top
        Panel2.Height = 70

        Panel3.Dock = DockStyle.Fill

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
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 220, 245)
        Next

        Panel2.BackColor = dolphin
        Label6.ForeColor = linen
        Label6.BackColor = Color.Transparent
        Label6.Font = New Font("Segoe UI", 13.5F, FontStyle.Bold)

        PictureBox10.BackColor = dolphin
        PictureBox11.BackColor = dolphin
        PictureBox12.BackColor = dolphin

        Panel3.BackColor = Color.FromArgb(248, 244, 250)
        Label7.ForeColor = dolphin

        For Each p In New Panel() {Panel4, Panel5, Panel6, Panel7, Panel8, Panel9}
            p.BackColor = Color.White
        Next

        For Each lbl In New Label() {Label12, Label13, Label14, Label15}
            lbl.ForeColor = amethyst
        Next

        For Each lbl In New Label() {Label8, Label9, Label10, Label11}
            lbl.ForeColor = textDark
        Next

        Label16.ForeColor = dolphin
        Label17.ForeColor = dolphin
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

    Private Sub ShowDashboard()
        currentView = "Dashboard"
        inventoryMode = "View"
        MoveSidePanel(btndashboard)

        SetDashboardVisible(True)

        If inventoryPanel IsNot Nothing Then
            inventoryPanel.Visible = False
        End If

        Label7.Text = "Dashboard"
        LayoutDashboardCards()
    End Sub

    Private Sub SetDashboardVisible(visible As Boolean)
        pbback.Visible = visible
        Label7.Visible = True

        For Each ctrl In New Control() {Panel4, Panel5, Panel6, Panel7, Panel8, Panel9}
            ctrl.Visible = visible
        Next
    End Sub

    Private Sub LayoutDashboardCards()
        If Panel3.Width <= 0 OrElse Panel3.Height <= 0 Then Return
        If currentView <> "Dashboard" Then Return

        pbback.SizeMode = PictureBoxSizeMode.Zoom
        pbback.Location = New Point(16, 20)
        pbback.Size = New Size(28, 28)

        Label7.Location = New Point(52, 10)
        Label7.Font = New Font("Segoe UI Semilight", 22.0F, FontStyle.Regular)

        Dim margin As Integer = 24
        Dim gap As Integer = 18
        Dim contentWidth As Integer = Panel3.ClientSize.Width - (margin * 2)
        Dim cardWidth As Integer = Math.Max(150, (contentWidth - (gap * 3)) \ 4)
        Dim cardHeight As Integer = 82
        Dim topY As Integer = 78

        Dim cards() As Panel = {Panel4, Panel5, Panel6, Panel7}

        For i As Integer = 0 To cards.Length - 1
            cards(i).Location = New Point(margin + (i * (cardWidth + gap)), topY)
            cards(i).Size = New Size(cardWidth, cardHeight)
        Next

        Dim bottomY As Integer = topY + cardHeight + 48
        Dim bottomHeight As Integer = Math.Max(180, Panel3.ClientSize.Height - bottomY - margin)
        Dim bigWidth As Integer = Math.Max(250, (contentWidth - gap) \ 2)

        Panel8.Location = New Point(margin, bottomY)
        Panel8.Size = New Size(bigWidth, bottomHeight)

        Panel9.Location = New Point(margin + bigWidth + gap, bottomY)
        Panel9.Size = New Size(bigWidth, bottomHeight)

        CenterPlaceholder(TextBox4, Panel8)
        CenterPlaceholder(TextBox5, Panel9)
    End Sub

    Private Sub CenterPlaceholder(tb As TextBox, parent As Panel)
        tb.Width = Math.Min(280, parent.Width - 40)
        tb.Height = 50
        tb.Location = New Point((parent.Width - tb.Width) \ 2, (parent.Height - tb.Height) \ 2)
    End Sub

    Private Sub ShowInventory()
        currentView = "Inventory"
        MoveSidePanel(btnInventory)

        SetDashboardVisible(False)
        Label7.Text = "Inventory"

        EnsureInventoryPanel()
        inventoryPanel.Visible = True
        inventoryPanel.BringToFront()

        SetInventoryMode("View")
        LayoutInventoryPanel()
        RefreshInventoryGrid()
    End Sub

    Private Sub EnsureInventoryPanel()
        If inventoryPanel IsNot Nothing Then Return

        Dim linen As Color = Color.FromArgb(253, 241, 226)
        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim amethyst As Color = Color.FromArgb(171, 146, 191)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        inventoryPanel = New Panel()
        inventoryPanel.Name = "inventoryPanel"
        inventoryPanel.BackColor = Color.Transparent
        Panel3.Controls.Add(inventoryPanel)

        txtSearchInventory = New TextBox()
        txtSearchInventory.Name = "txtSearchInventory"
        txtSearchInventory.Font = New Font("Segoe UI", 10)
        txtSearchInventory.PlaceholderText = "Search item..."
        AddHandler txtSearchInventory.TextChanged, AddressOf SearchInventory_TextChanged
        inventoryPanel.Controls.Add(txtSearchInventory)

        btnAddItem = CreateActionButton("Add Item", dolphin, linen)
        AddHandler btnAddItem.Click, AddressOf AddItem_Click
        inventoryPanel.Controls.Add(btnAddItem)

        btnEditItem = CreateActionButton("Edit", amethyst, Color.White)
        AddHandler btnEditItem.Click, AddressOf EditMode_Click
        inventoryPanel.Controls.Add(btnEditItem)

        btnDeleteItem = CreateActionButton("Delete", Color.FromArgb(180, 85, 95), Color.White)
        AddHandler btnDeleteItem.Click, AddressOf DeleteMode_Click
        inventoryPanel.Controls.Add(btnDeleteItem)

        actionPanel = New Panel()
        actionPanel.BackColor = Color.Transparent
        actionPanel.Visible = False
        inventoryPanel.Controls.Add(actionPanel)

        btnConfirmAction = CreateActionButton("Confirm", dolphin, linen)
        AddHandler btnConfirmAction.Click, AddressOf ConfirmAction_Click
        actionPanel.Controls.Add(btnConfirmAction)

        btnCancelAction = CreateActionButton("Cancel", Color.Gray, Color.White)
        AddHandler btnCancelAction.Click, AddressOf CancelAction_Click
        actionPanel.Controls.Add(btnCancelAction)

        dgvInventory = New DataGridView()
        dgvInventory.Name = "dgvInventory"
        dgvInventory.AllowUserToAddRows = False
        dgvInventory.AllowUserToDeleteRows = False
        dgvInventory.ReadOnly = False
        dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvInventory.MultiSelect = False
        dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvInventory.BackgroundColor = Color.White
        dgvInventory.BorderStyle = BorderStyle.None
        dgvInventory.RowHeadersVisible = False
        dgvInventory.Font = New Font("Segoe UI", 10)
        dgvInventory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 220, 245)
        dgvInventory.DefaultCellStyle.SelectionForeColor = textDark
        dgvInventory.RowsDefaultCellStyle.BackColor = Color.White
        dgvInventory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 248, 255)
        dgvInventory.GridColor = Color.FromArgb(220, 210, 225)

        dgvInventory.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        dgvInventory.ColumnHeadersDefaultCellStyle.BackColor = dolphin
        dgvInventory.ColumnHeadersDefaultCellStyle.ForeColor = linen
        dgvInventory.ColumnHeadersDefaultCellStyle.SelectionBackColor = dolphin
        dgvInventory.ColumnHeadersDefaultCellStyle.SelectionForeColor = linen
        dgvInventory.EnableHeadersVisualStyles = False

        AddHandler dgvInventory.DataBindingComplete, AddressOf DgvInventory_DataBindingComplete
        AddHandler dgvInventory.CellClick, AddressOf DgvInventory_CellClick
        AddHandler dgvInventory.CurrentCellChanged, AddressOf DgvInventory_CurrentCellChanged

        inventoryPanel.Controls.Add(dgvInventory)
    End Sub

    Private Function CreateActionButton(text As String, backColor As Color, foreColor As Color) As Button
        Dim btn As New Button()
        btn.Text = text
        btn.BackColor = backColor
        btn.ForeColor = foreColor
        btn.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        Return btn
    End Function

    Private Sub LayoutInventoryPanel()
        If inventoryPanel Is Nothing Then Return

        Dim margin As Integer = 24
        Dim gap As Integer = 12

        Label7.Location = New Point(24, 12)
        Label7.Font = New Font("Segoe UI Semilight", 22.0F, FontStyle.Regular)

        inventoryPanel.Location = New Point(margin, 78)
        inventoryPanel.Size = New Size(Panel3.ClientSize.Width - (margin * 2), Panel3.ClientSize.Height - 100)

        txtSearchInventory.Location = New Point(0, 0)
        txtSearchInventory.Size = New Size(280, 34)

        btnDeleteItem.Size = New Size(100, 36)
        btnEditItem.Size = New Size(100, 36)
        btnAddItem.Size = New Size(120, 36)

        btnDeleteItem.Location = New Point(inventoryPanel.Width - btnDeleteItem.Width, 0)
        btnEditItem.Location = New Point(btnDeleteItem.Left - btnEditItem.Width - gap, 0)
        btnAddItem.Location = New Point(btnEditItem.Left - btnAddItem.Width - gap, 0)

        dgvInventory.Location = New Point(0, 55)
        dgvInventory.Size = New Size(inventoryPanel.Width, inventoryPanel.Height - 110)

        actionPanel.Size = New Size(260, 44)
        actionPanel.Location = New Point(inventoryPanel.Width - actionPanel.Width, inventoryPanel.Height - actionPanel.Height)

        btnCancelAction.Size = New Size(100, 36)
        btnConfirmAction.Size = New Size(145, 36)

        btnCancelAction.Location = New Point(actionPanel.Width - btnCancelAction.Width, 4)
        btnConfirmAction.Location = New Point(btnCancelAction.Left - btnConfirmAction.Width - gap, 4)
    End Sub

    Private Sub SetInventoryMode(mode As String)
        inventoryMode = mode

        For Each item In inventoryItems
            item.Selected = False
        Next

        If actionPanel IsNot Nothing Then
            actionPanel.Visible = mode <> "View"
        End If

        If btnConfirmAction IsNot Nothing Then
            If mode = "Edit" Then
                btnConfirmAction.Text = "Edit Selected"
                btnConfirmAction.BackColor = Color.FromArgb(101, 90, 124)
            ElseIf mode = "Delete" Then
                btnConfirmAction.Text = "Delete Selected"
                btnConfirmAction.BackColor = Color.FromArgb(180, 85, 95)
            End If
        End If

        RefreshInventoryGrid()
    End Sub

    Private Sub RefreshInventoryGrid()
        If dgvInventory Is Nothing Then Return

        dgvInventory.EndEdit()

        Dim keyword As String = ""

        If txtSearchInventory IsNot Nothing Then
            keyword = txtSearchInventory.Text.Trim().ToLower()
        End If

        Dim filtered = inventoryItems.Where(Function(item)
                                                Return keyword = "" OrElse
                                                    item.ItemID.ToLower().Contains(keyword) OrElse
                                                    item.ItemName.ToLower().Contains(keyword) OrElse
                                                    item.Category.ToLower().Contains(keyword) OrElse
                                                    item.Status.ToLower().Contains(keyword)
                                            End Function).ToList()

        dgvInventory.DataSource = Nothing
        dgvInventory.DataSource = filtered
    End Sub

    Private Sub DgvInventory_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        If dgvInventory.Columns.Contains("Selected") Then
            dgvInventory.Columns("Selected").HeaderText = ""
            dgvInventory.Columns("Selected").Width = 45
            dgvInventory.Columns("Selected").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            dgvInventory.Columns("Selected").ReadOnly = False
            dgvInventory.Columns("Selected").Visible = inventoryMode <> "View"
        End If

        For Each col As DataGridViewColumn In dgvInventory.Columns
            If col.Name <> "Selected" Then
                col.ReadOnly = True
            End If
        Next

        dgvInventory.ClearSelection()
        dgvInventory.CurrentCell = Nothing
    End Sub

    Private Sub DgvInventory_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then
            dgvInventory.ClearSelection()
            dgvInventory.CurrentCell = Nothing
            Return
        End If

        If inventoryMode = "View" Then
            dgvInventory.ClearSelection()
            dgvInventory.CurrentCell = Nothing
        End If
    End Sub

    Private Sub DgvInventory_CurrentCellChanged(sender As Object, e As EventArgs)
        If dgvInventory Is Nothing Then Return

        If dgvInventory.CurrentCell IsNot Nothing AndAlso dgvInventory.CurrentCell.OwningColumn.Name <> "Selected" Then
            dgvInventory.ClearSelection()
        End If
    End Sub

    Private Sub SearchInventory_TextChanged(sender As Object, e As EventArgs)
        RefreshInventoryGrid()
    End Sub

    Private Function GetCheckedItems() As List(Of InventoryItem)
        If dgvInventory IsNot Nothing Then
            dgvInventory.EndEdit()
        End If

        Return inventoryItems.Where(Function(x) x.Selected).ToList()
    End Function

    Private Sub AddItem_Click(sender As Object, e As EventArgs)
        SetInventoryMode("View")

        Using dialog As New AddItemDialog()
            If dialog.ShowDialog() = DialogResult.OK Then
                Dim newId As String = "ITM-" & (inventoryItems.Count + 1).ToString("000")

                inventoryItems.Add(New InventoryItem With {
                    .Selected = False,
                    .ItemID = newId,
                    .ItemName = dialog.ItemName,
                    .Category = dialog.Category,
                    .Quantity = dialog.Quantity,
                    .Status = dialog.Status
                })

                RefreshInventoryGrid()
            End If
        End Using
    End Sub

    Private Sub EditMode_Click(sender As Object, e As EventArgs)
        SetInventoryMode("Edit")
    End Sub

    Private Sub DeleteMode_Click(sender As Object, e As EventArgs)
        SetInventoryMode("Delete")
    End Sub

    Private Sub ConfirmAction_Click(sender As Object, e As EventArgs)
        If inventoryMode = "Edit" Then
            ConfirmEdit()
        ElseIf inventoryMode = "Delete" Then
            ConfirmDelete()
        End If
    End Sub

    Private Sub ConfirmEdit()
        Dim checkedItems = GetCheckedItems()

        If checkedItems.Count = 0 Then
            MessageBox.Show("Check at least one item first.", "Edit Item")
            Return
        End If

        Using dialog As New EditItemsDialog(checkedItems)
            If dialog.ShowDialog() = DialogResult.OK Then
                SetInventoryMode("View")
            Else
                RefreshInventoryGrid()
            End If
        End Using
    End Sub

    Private Sub ConfirmDelete()
        Dim checkedItems = GetCheckedItems()

        If checkedItems.Count = 0 Then
            MessageBox.Show("Check at least one item first.", "Delete Item")
            Return
        End If

        Dim linen As Color = Color.FromArgb(253, 241, 226)
        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim danger As Color = Color.FromArgb(180, 85, 95)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        Using dialog As New Form()
            dialog.Text = "Confirm Delete"
            dialog.Size = New Size(360, 210)
            dialog.StartPosition = FormStartPosition.CenterParent
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog
            dialog.MaximizeBox = False
            dialog.MinimizeBox = False
            dialog.BackColor = linen

            Dim title As New Label With {
            .Text = "Delete Item?",
            .Font = New Font("Segoe UI", 15, FontStyle.Bold),
            .ForeColor = danger,
            .Location = New Point(28, 22),
            .Size = New Size(280, 35)
        }

            Dim message As New Label With {
            .Text = "You selected " & checkedItems.Count.ToString() & " item(s). This action cannot be undone.",
            .Font = New Font("Segoe UI", 10),
            .ForeColor = textDark,
            .Location = New Point(30, 70),
            .Size = New Size(285, 45)
        }

            Dim btnDelete As New Button With {
            .Text = "Delete",
            .Location = New Point(130, 130),
            .Size = New Size(90, 35),
            .BackColor = danger,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }

            Dim btnCancel As New Button With {
            .Text = "Cancel",
            .Location = New Point(230, 130),
            .Size = New Size(90, 35),
            .BackColor = dolphin,
            .ForeColor = linen,
            .FlatStyle = FlatStyle.Flat
        }

            btnDelete.FlatAppearance.BorderSize = 0
            btnCancel.FlatAppearance.BorderSize = 0

            AddHandler btnDelete.Click, Sub()
                                            dialog.DialogResult = DialogResult.Yes
                                            dialog.Close()
                                        End Sub

            AddHandler btnCancel.Click, Sub()
                                            dialog.DialogResult = DialogResult.No
                                            dialog.Close()
                                        End Sub

            dialog.Controls.AddRange(New Control() {title, message, btnDelete, btnCancel})

            If dialog.ShowDialog() = DialogResult.Yes Then
                For Each item In checkedItems
                    inventoryItems.Remove(item)
                Next

                SetInventoryMode("View")
            End If
        End Using
    End Sub

    Private Sub CancelAction_Click(sender As Object, e As EventArgs)
        SetInventoryMode("View")
    End Sub

    Private Sub MoveSidePanel(btn As Button)
        SidePanel.Visible = True
        SidePanel.Width = 5
        SidePanel.Height = btn.Height - 10
        SidePanel.Left = 0
        SidePanel.Top = btn.Top + 5
    End Sub

    Private Sub btndashboard_Click(sender As Object, e As EventArgs) Handles btndashboard.Click
        ShowDashboard()
    End Sub
    Private Sub btnInventory_Click(sender As Object, e As EventArgs) Handles btnInventory.Click
        currentView = "Inventory"
        MoveSidePanel(btnInventory)

        Panel3.Controls.Clear()

        Dim page As New InventoryPage()
        page.Dock = DockStyle.Fill
        Panel3.Controls.Add(page)
    End Sub

    Private Sub btntransaction_Click(sender As Object, e As EventArgs) Handles btntransaction.Click
        currentView = "Transactions"
        MoveSidePanel(btntransaction)

        Panel3.Controls.Clear()

        Dim page As New TransactionsPage()
        page.Dock = DockStyle.Fill
        Panel3.Controls.Add(page)
    End Sub

    Private Sub btnsuppliers_Click(sender As Object, e As EventArgs) Handles btnsuppliers.Click
        MoveSidePanel(btnsuppliers)
        MessageBox.Show("Suppliers page will be added next.", "Frontend")
    End Sub

    Private Sub btnreports_Click(sender As Object, e As EventArgs) Handles btnreports.Click
        MoveSidePanel(btnreports)
        MessageBox.Show("Reports page will be added next.", "Frontend")
    End Sub

    Private Sub btnsettings_Click(sender As Object, e As EventArgs) Handles btnsettings.Click
        MoveSidePanel(btnsettings)
        MessageBox.Show("Settings page will be added next.", "Frontend")
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

    Private Class AddItemDialog
        Inherits Form

        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String

        Private txtItemName As TextBox
        Private cmbCategory As ComboBox
        Private numQuantity As NumericUpDown
        Private cmbStatus As ComboBox

        Public Sub New()
            Me.Text = "Add Item"
            Me.Size = New Size(380, 360)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = Color.FromArgb(253, 241, 226)

            Dim dolphin As Color = Color.FromArgb(101, 90, 124)
            Dim textDark As Color = Color.FromArgb(45, 38, 55)

            Dim lblName As New Label With {.Text = "Item Name", .Location = New Point(35, 30), .Size = New Size(280, 22), .ForeColor = textDark}
            txtItemName = New TextBox With {.Location = New Point(35, 55), .Size = New Size(290, 30)}

            Dim lblCategory As New Label With {.Text = "Category", .Location = New Point(35, 90), .Size = New Size(280, 22), .ForeColor = textDark}
            cmbCategory = New ComboBox With {.Location = New Point(35, 115), .Size = New Size(290, 30), .DropDownStyle = ComboBoxStyle.DropDownList}
            cmbCategory.Items.AddRange(New String() {
                "Laptop", "Desktop Computer", "Monitor", "Keyboard", "Mouse",
                "Projector", "Printer", "Router / Network Device", "Storage Device",
                "Cable / Adapter", "Software License", "Other"
            })
            cmbCategory.SelectedIndex = 0

            Dim lblQty As New Label With {.Text = "Quantity", .Location = New Point(35, 150), .Size = New Size(280, 22), .ForeColor = textDark}
            numQuantity = New NumericUpDown With {.Location = New Point(35, 175), .Size = New Size(290, 30), .Minimum = 0, .Maximum = 100000, .Value = 1}

            Dim lblStatus As New Label With {.Text = "Status", .Location = New Point(35, 210), .Size = New Size(280, 22), .ForeColor = textDark}
            cmbStatus = New ComboBox With {.Location = New Point(35, 235), .Size = New Size(290, 30), .DropDownStyle = ComboBoxStyle.DropDownList}
            cmbStatus.Items.AddRange(New String() {"Available", "In use", "Defective", "Under Repair", "Missing"})
            cmbStatus.SelectedIndex = 0

            Dim btnSave As New Button With {.Text = "Save", .Location = New Point(145, 285), .Size = New Size(85, 35), .BackColor = dolphin, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
            Dim btnCancel As New Button With {.Text = "Cancel", .Location = New Point(240, 285), .Size = New Size(85, 35), .BackColor = Color.Gray, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}

            AddHandler btnSave.Click, AddressOf Save_Click
            AddHandler btnCancel.Click, Sub()
                                            Me.DialogResult = DialogResult.Cancel
                                            Me.Close()
                                        End Sub

            Me.Controls.AddRange(New Control() {
                lblName, txtItemName,
                lblCategory, cmbCategory,
                lblQty, numQuantity,
                lblStatus, cmbStatus,
                btnSave, btnCancel
            })
        End Sub

        Private Sub Save_Click(sender As Object, e As EventArgs)
            If txtItemName.Text.Trim() = "" Then
                MessageBox.Show("Item name is required.", "Missing information")
                Return
            End If

            ItemName = txtItemName.Text.Trim()
            Category = cmbCategory.Text
            Quantity = CInt(numQuantity.Value)
            Status = cmbStatus.Text

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class

    Private Class EditItemsDialog
        Inherits Form

        Private selectedItems As List(Of InventoryItem)
        Private itemControls As New Dictionary(Of InventoryItem, Tuple(Of TextBox, ComboBox, NumericUpDown, ComboBox))

        Public Sub New(items As List(Of InventoryItem))
            selectedItems = items

            Me.Text = "Edit Item(s)"
            Me.Size = New Size(470, 470)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.BackColor = Color.FromArgb(253, 241, 226)

            Dim dolphin As Color = Color.FromArgb(101, 90, 124)
            Dim textDark As Color = Color.FromArgb(45, 38, 55)

            Dim header As New Label With {
                .Text = "Edit checked item(s)",
                .Font = New Font("Segoe UI", 13, FontStyle.Bold),
                .ForeColor = dolphin,
                .Location = New Point(25, 18),
                .Size = New Size(380, 30)
            }

            Dim scrollPanel As New Panel With {
                .Location = New Point(25, 60),
                .Size = New Size(405, 300),
                .AutoScroll = True,
                .BackColor = Color.White
            }

            Dim y As Integer = 15

            For Each item In selectedItems
                Dim group As New GroupBox With {
                    .Text = item.ItemID,
                    .Location = New Point(15, y),
                    .Size = New Size(355, 175),
                    .ForeColor = textDark
                }

                Dim txtName As New TextBox With {.Text = item.ItemName, .Location = New Point(20, 30), .Size = New Size(300, 25)}

                Dim cmbCategory As New ComboBox With {.Location = New Point(20, 65), .Size = New Size(300, 25), .DropDownStyle = ComboBoxStyle.DropDownList}
                cmbCategory.Items.AddRange(New String() {
                    "Laptop", "Desktop Computer", "Monitor", "Keyboard", "Mouse",
                    "Projector", "Printer", "Router / Network Device", "Storage Device",
                    "Cable / Adapter", "Software License", "Other"
                })
                cmbCategory.Text = item.Category

                Dim numQty As New NumericUpDown With {.Location = New Point(20, 100), .Size = New Size(300, 25), .Minimum = 0, .Maximum = 100000, .Value = item.Quantity}

                Dim cmbStatus As New ComboBox With {.Location = New Point(20, 135), .Size = New Size(300, 25), .DropDownStyle = ComboBoxStyle.DropDownList}
                cmbStatus.Items.AddRange(New String() {"Available", "In use", "Defective", "Under Repair", "Missing"})
                cmbStatus.Text = item.Status

                group.Controls.AddRange(New Control() {txtName, cmbCategory, numQty, cmbStatus})
                scrollPanel.Controls.Add(group)

                itemControls(item) = Tuple.Create(txtName, cmbCategory, numQty, cmbStatus)
                y += 190
            Next

            Dim btnSave As New Button With {.Text = "Save Changes", .Location = New Point(225, 380), .Size = New Size(120, 35), .BackColor = dolphin, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
            Dim btnCancel As New Button With {.Text = "Cancel", .Location = New Point(355, 380), .Size = New Size(75, 35), .BackColor = Color.Gray, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}

            AddHandler btnSave.Click, AddressOf Save_Click
            AddHandler btnCancel.Click, Sub()
                                            Me.DialogResult = DialogResult.Cancel
                                            Me.Close()
                                        End Sub

            Me.Controls.AddRange(New Control() {header, scrollPanel, btnSave, btnCancel})
        End Sub

        Private Sub Save_Click(sender As Object, e As EventArgs)
            For Each item In selectedItems
                Dim controls = itemControls(item)
                Dim txtName = controls.Item1
                Dim cmbCategory = controls.Item2
                Dim numQty = controls.Item3
                Dim cmbStatus = controls.Item4

                If txtName.Text.Trim() = "" Then
                    MessageBox.Show("Item name cannot be blank.", "Missing information")
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

End Class