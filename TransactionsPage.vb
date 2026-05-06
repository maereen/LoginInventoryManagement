Public Class TransactionsPage

    Private dgv As DataGridView
    Private txtSearch As TextBox
    Private cmbStatusFilter As ComboBox
    Private cmbDateFilter As ComboBox
    Private btnAdd As Button
    Private btnEdit As Button
    Private btnDelete As Button

    Private items As New List(Of TransactionItem)
    Private customStartDate As Date? = Nothing
    Private customEndDate As Date? = Nothing

    Private Class TransactionItem
        Public Property TransID As String
        Public Property ItemName As String
        Public Property BorrowerName As String
        Public Property IssuedBy As String
        Public Property ReturnedBy As String
        Public Property DateIssued As Date
        Public Property DueDate As Date
        Public Property IsReturned As Boolean
    End Class

    Private Sub TransactionsPage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(248, 244, 250)
        LoadSample()
        BuildUI()
        RefreshGrid()
    End Sub

    Private Sub BuildUI()
        Me.Controls.Clear()

        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim linen As Color = Color.FromArgb(253, 241, 226)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        Dim title As New Label With {
            .Text = "Transactions",
            .Font = New Font("Segoe UI Semilight", 22),
            .ForeColor = dolphin,
            .Location = New Point(24, 20),
            .AutoSize = True
        }
        Me.Controls.Add(title)

        btnAdd = CreateButton("Add", dolphin)
        btnEdit = CreateButton("Edit", Color.FromArgb(171, 146, 191))
        btnDelete = CreateButton("Delete", Color.FromArgb(180, 85, 95))

        btnAdd.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnEdit.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnDelete.Anchor = AnchorStyles.Top Or AnchorStyles.Right

        AddHandler btnAdd.Click, AddressOf AddClicked
        AddHandler btnEdit.Click, AddressOf EditClicked
        AddHandler btnDelete.Click, AddressOf DeleteClicked

        Me.Controls.AddRange(New Control() {btnAdd, btnEdit, btnDelete})

        txtSearch = New TextBox With {
            .PlaceholderText = "Search ID, item, borrower, staff...",
            .Location = New Point(24, 110),
            .Size = New Size(330, 30),
            .Font = New Font("Segoe UI", 10)
        }

        cmbStatusFilter = CreateCombo(New String() {"All Status", "Borrowed", "For Return", "Completed", "Overdue"})
        cmbDateFilter = CreateCombo(New String() {"All Dates", "Today", "Yesterday", "This Week", "This Month", "Custom Date"})

        cmbStatusFilter.Location = New Point(375, 110)
        cmbDateFilter.Location = New Point(525, 110)

        Me.Controls.AddRange(New Control() {
            New Label With {.Text = "Search", .Location = New Point(24, 88), .ForeColor = textDark},
            txtSearch,
            New Label With {.Text = "Status", .Location = New Point(375, 88), .ForeColor = textDark},
            cmbStatusFilter,
            New Label With {.Text = "Date Issued", .Location = New Point(525, 88), .ForeColor = textDark},
            cmbDateFilter
        })

        AddHandler txtSearch.TextChanged, AddressOf FilterChanged
        AddHandler cmbStatusFilter.SelectedIndexChanged, AddressOf FilterChanged
        AddHandler cmbDateFilter.SelectedIndexChanged, AddressOf DateFilterChanged

        dgv = New DataGridView With {
            .Location = New Point(24, 165),
            .Size = New Size(Me.Width - 48, Me.Height - 205),
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

        LayoutUI()
        AddHandler Me.Resize, Sub() LayoutUI()
    End Sub

    Private Sub LayoutUI()
        If btnAdd Is Nothing Then Return

        If Me.Width >= 1200 Then
            btnAdd.Location = New Point(Me.Width - 378, 95)
            btnEdit.Location = New Point(Me.Width - 256, 95)
            btnDelete.Location = New Point(Me.Width - 134, 95)
        Else
            btnAdd.Location = New Point(Me.Width - 378, 28)
            btnEdit.Location = New Point(Me.Width - 256, 28)
            btnDelete.Location = New Point(Me.Width - 134, 28)
        End If

        If dgv IsNot Nothing Then
            dgv.Size = New Size(Me.Width - 48, Me.Height - 205)
        End If
    End Sub

    Private Function CreateCombo(options As String()) As ComboBox
        Dim cmb As New ComboBox With {
            .Size = New Size(130, 30),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9)
        }
        cmb.Items.AddRange(options)
        cmb.SelectedIndex = 0
        Return cmb
    End Function

    Private Function CreateButton(text As String, color As Color) As Button
        Dim b As New Button With {
            .Text = text,
            .Size = New Size(110, 36),
            .BackColor = color,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }
        b.FlatAppearance.BorderSize = 0
        Return b
    End Function

    Private Sub LoadSample()
        If items.Count > 0 Then Return

        items.Add(New TransactionItem With {
            .TransID = "TR-001",
            .ItemName = "Laptop",
            .BorrowerName = "Juan Dela Cruz",
            .IssuedBy = "Admin User",
            .ReturnedBy = "",
            .DateIssued = Date.Today.AddDays(-2),
            .DueDate = Date.Today.AddDays(2),
            .IsReturned = False
        })

        items.Add(New TransactionItem With {
            .TransID = "TR-002",
            .ItemName = "Projector",
            .BorrowerName = "Maria Santos",
            .IssuedBy = "Admin User",
            .ReturnedBy = "Admin User",
            .DateIssued = Date.Today.AddDays(-4),
            .DueDate = Date.Today.AddDays(-1),
            .IsReturned = True
        })

        items.Add(New TransactionItem With {
            .TransID = "TR-003",
            .ItemName = "Keyboard",
            .BorrowerName = "Carlo Reyes",
            .IssuedBy = "Staff 01",
            .ReturnedBy = "",
            .DateIssued = Date.Today.AddDays(-1),
            .DueDate = Date.Today,
            .IsReturned = False
        })

        items.Add(New TransactionItem With {
            .TransID = "TR-004",
            .ItemName = "Mouse",
            .BorrowerName = "Ana Lopez",
            .IssuedBy = "Staff 01",
            .ReturnedBy = "",
            .DateIssued = Date.Today.AddDays(-5),
            .DueDate = Date.Today.AddDays(-2),
            .IsReturned = False
        })
    End Sub

    Private Function GetTransactionStatus(item As TransactionItem) As String
        If item.IsReturned Then
            Return "Completed"
        End If

        If item.DueDate.Date = Date.Today Then
            Return "For Return"
        End If

        If item.DueDate.Date < Date.Today Then
            Return "Overdue"
        End If

        Return "Borrowed"
    End Function

    Private Sub RefreshGrid()
        If dgv Is Nothing Then Return

        Dim keyword = If(txtSearch Is Nothing, "", txtSearch.Text.Trim().ToLower())
        Dim statusFilter = If(cmbStatusFilter Is Nothing, "All Status", cmbStatusFilter.Text)
        Dim dateFilter = If(cmbDateFilter Is Nothing, "All Dates", cmbDateFilter.Text)

        Dim filtered = items.Where(Function(x)
                                       Dim status = GetTransactionStatus(x).ToLower()

                                       Dim matchSearch = keyword = "" OrElse
                                           x.TransID.ToLower().Contains(keyword) OrElse
                                           x.ItemName.ToLower().Contains(keyword) OrElse
                                           x.BorrowerName.ToLower().Contains(keyword) OrElse
                                           x.IssuedBy.ToLower().Contains(keyword) OrElse
                                           x.ReturnedBy.ToLower().Contains(keyword)

                                       Dim matchStatus = statusFilter = "All Status" OrElse GetTransactionStatus(x) = statusFilter
                                       Dim matchDate = DateMatches(x.DateIssued, dateFilter)

                                       Return matchSearch AndAlso matchStatus AndAlso matchDate
                                   End Function).
            Select(Function(x) New With {
                .TransID = x.TransID,
                .ItemName = x.ItemName,
                .Borrower = x.BorrowerName,
                .IssuedBy = x.IssuedBy,
                .ReturnedBy = If(x.ReturnedBy = "", "-", x.ReturnedBy),
                .DateIssued = x.DateIssued.ToString("yyyy-MM-dd"),
                .DueDate = x.DueDate.ToString("yyyy-MM-dd"),
                .Status = GetTransactionStatus(x)
            }).ToList()

        dgv.DataSource = Nothing
        dgv.DataSource = filtered
    End Sub

    Private Function DateMatches(transactionDate As Date, filter As String) As Boolean
        Dim d = transactionDate.Date

        Select Case filter
            Case "All Dates"
                Return True
            Case "Today"
                Return d = Date.Today
            Case "Yesterday"
                Return d = Date.Today.AddDays(-1)
            Case "This Week"
                Dim startWeek = Date.Today.AddDays(-CInt(Date.Today.DayOfWeek))
                Return d >= startWeek AndAlso d <= startWeek.AddDays(6)
            Case "This Month"
                Return d.Month = Date.Today.Month AndAlso d.Year = Date.Today.Year
            Case "Custom Date"
                If customStartDate.HasValue AndAlso customEndDate.HasValue Then
                    Return d >= customStartDate.Value.Date AndAlso d <= customEndDate.Value.Date
                End If
                Return True
        End Select

        Return True
    End Function

    Private Sub GridReady(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        For Each col As DataGridViewColumn In dgv.Columns
            col.ReadOnly = True
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next

        dgv.ClearSelection()
        dgv.CurrentCell = Nothing
    End Sub

    Private Sub FilterChanged(sender As Object, e As EventArgs)
        RefreshGrid()
    End Sub

    Private Sub DateFilterChanged(sender As Object, e As EventArgs)
        If cmbDateFilter.Text = "Custom Date" Then
            Using dialog As New CustomDateDialog()
                If dialog.ShowDialog() = DialogResult.OK Then
                    customStartDate = dialog.StartDate
                    customEndDate = dialog.EndDate
                Else
                    cmbDateFilter.SelectedIndex = 0
                    customStartDate = Nothing
                    customEndDate = Nothing
                End If
            End Using
        Else
            customStartDate = Nothing
            customEndDate = Nothing
        End If

        RefreshGrid()
    End Sub

    Private Function GetSelectedTransactionID() As String
        If dgv.CurrentRow Is Nothing Then Return ""

        Dim value = dgv.CurrentRow.Cells("TransID").Value
        If value Is Nothing Then Return ""

        Return value.ToString()
    End Function

    Private Sub AddClicked(sender As Object, e As EventArgs)
        Using dialog As New TransactionDialog()
            If dialog.ShowDialog() = DialogResult.OK Then
                items.Add(New TransactionItem With {
                    .TransID = "TR-" & (items.Count + 1).ToString("000"),
                    .ItemName = dialog.ItemName,
                    .BorrowerName = dialog.BorrowerName,
                    .IssuedBy = dialog.IssuedBy,
                    .ReturnedBy = dialog.ReturnedBy,
                    .DateIssued = dialog.DateIssued,
                    .DueDate = dialog.DueDate,
                    .IsReturned = dialog.IsReturned
                })

                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub EditClicked(sender As Object, e As EventArgs)
        Dim selectedId = GetSelectedTransactionID()

        If selectedId = "" Then
            MessageBox.Show("Select one transaction first.")
            Return
        End If

        Dim item = items.FirstOrDefault(Function(x) x.TransID = selectedId)
        If item Is Nothing Then Return

        Using dialog As New TransactionDialog(item)
            If dialog.ShowDialog() = DialogResult.OK Then
                item.ItemName = dialog.ItemName
                item.BorrowerName = dialog.BorrowerName
                item.IssuedBy = dialog.IssuedBy
                item.ReturnedBy = dialog.ReturnedBy
                item.DateIssued = dialog.DateIssued
                item.DueDate = dialog.DueDate
                item.IsReturned = dialog.IsReturned

                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub DeleteClicked(sender As Object, e As EventArgs)
        Dim selectedId = GetSelectedTransactionID()

        If selectedId = "" Then
            MessageBox.Show("Select one transaction first.")
            Return
        End If

        If MessageBox.Show("Delete selected transaction?", "Delete Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            items.RemoveAll(Function(x) x.TransID = selectedId)
            RefreshGrid()
        End If
    End Sub

    Private Class CustomDateDialog
        Inherits Form

        Public Property StartDate As Date
        Public Property EndDate As Date

        Private selectedDates As New List(Of Date)
        Private dayButtons As New Dictionary(Of Date, Button)
        Private calendarControls As New List(Of Control)
        Private calendarTitle As Label
        Private shownMonth As Date = New Date(Date.Today.Year, Date.Today.Month, 1)

        Public Sub New()
            Me.Text = "Choose Date Range"
            Me.Size = New Size(390, 410)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.BackColor = Color.FromArgb(253, 241, 226)
            Me.MaximizeBox = False
            Me.MinimizeBox = False

            BuildStaticCalendarUI()
            RebuildCalendarDays()

            Dim apply As New Button With {
                .Text = "Apply",
                .Location = New Point(180, 330),
                .Size = New Size(80, 35),
                .BackColor = Color.FromArgb(101, 90, 124),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            Dim cancel As New Button With {
                .Text = "Cancel",
                .Location = New Point(270, 330),
                .Size = New Size(80, 35),
                .BackColor = Color.Gray,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            apply.FlatAppearance.BorderSize = 0
            cancel.FlatAppearance.BorderSize = 0

            AddHandler apply.Click, AddressOf ApplyClicked
            AddHandler cancel.Click, Sub()
                                         Me.DialogResult = DialogResult.Cancel
                                         Me.Close()
                                     End Sub

            Me.Controls.AddRange(New Control() {apply, cancel})
        End Sub

        Private Sub BuildStaticCalendarUI()
            Dim dolphin = Color.FromArgb(101, 90, 124)
            Dim linen = Color.FromArgb(253, 241, 226)

            Dim prevBtn As New Button With {
                .Text = "<",
                .Location = New Point(60, 22),
                .Size = New Size(35, 30),
                .BackColor = dolphin,
                .ForeColor = linen,
                .FlatStyle = FlatStyle.Flat
            }

            Dim nextBtn As New Button With {
                .Text = ">",
                .Location = New Point(295, 22),
                .Size = New Size(35, 30),
                .BackColor = dolphin,
                .ForeColor = linen,
                .FlatStyle = FlatStyle.Flat
            }

            prevBtn.FlatAppearance.BorderSize = 0
            nextBtn.FlatAppearance.BorderSize = 0

            calendarTitle = New Label With {
                .Text = shownMonth.ToString("MMMM yyyy"),
                .Location = New Point(105, 22),
                .Size = New Size(180, 30),
                .TextAlign = ContentAlignment.MiddleCenter,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .ForeColor = dolphin
            }

            AddHandler prevBtn.Click, Sub()
                                          shownMonth = shownMonth.AddMonths(-1)
                                          RebuildCalendarDays()
                                      End Sub

            AddHandler nextBtn.Click, Sub()
                                          shownMonth = shownMonth.AddMonths(1)
                                          RebuildCalendarDays()
                                      End Sub

            Me.Controls.AddRange(New Control() {prevBtn, calendarTitle, nextBtn})

            Dim days = {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"}
            For i = 0 To 6
                Me.Controls.Add(New Label With {
                    .Text = days(i),
                    .Location = New Point(35 + (i * 45), 65),
                    .Size = New Size(40, 20),
                    .TextAlign = ContentAlignment.MiddleCenter
                })
            Next
        End Sub

        Private Sub RebuildCalendarDays()
            For Each ctrl In calendarControls
                Me.Controls.Remove(ctrl)
                ctrl.Dispose()
            Next

            calendarControls.Clear()
            dayButtons.Clear()

            If calendarTitle IsNot Nothing Then
                calendarTitle.Text = shownMonth.ToString("MMMM yyyy")
            End If

            Dim firstDay = New Date(shownMonth.Year, shownMonth.Month, 1)
            Dim startOffset = CInt(firstDay.DayOfWeek)
            Dim daysInMonth = Date.DaysInMonth(shownMonth.Year, shownMonth.Month)

            For day = 1 To daysInMonth
                Dim d = New Date(shownMonth.Year, shownMonth.Month, day)
                Dim index = startOffset + day - 1
                Dim row = index \ 7
                Dim col = index Mod 7

                Dim btn As New Button With {
                    .Text = day.ToString(),
                    .Location = New Point(35 + (col * 45), 90 + (row * 35)),
                    .Size = New Size(38, 30),
                    .BackColor = Color.White,
                    .ForeColor = Color.FromArgb(45, 38, 55),
                    .FlatStyle = FlatStyle.Flat,
                    .Tag = d
                }

                btn.FlatAppearance.BorderSize = 0
                AddHandler btn.Click, AddressOf DayClicked

                dayButtons(d) = btn
                calendarControls.Add(btn)
                Me.Controls.Add(btn)
            Next

            UpdateHighlights()
        End Sub

        Private Sub DayClicked(sender As Object, e As EventArgs)
            Dim clickedDate = CDate(CType(sender, Button).Tag)

            If selectedDates.Count = 0 Then
                selectedDates.Add(clickedDate)

            ElseIf selectedDates.Count = 1 Then
                Dim first = selectedDates(0)

                If clickedDate = first Then
                    selectedDates.Clear()
                    selectedDates.Add(clickedDate)
                ElseIf Math.Abs((clickedDate - first).TotalDays) <= 1 Then
                    selectedDates.Add(clickedDate)
                    selectedDates = selectedDates.OrderBy(Function(x) x).ToList()
                Else
                    selectedDates.Clear()
                    selectedDates.Add(clickedDate)
                End If

            Else
                selectedDates.Clear()
                selectedDates.Add(clickedDate)
            End If

            UpdateHighlights()
        End Sub

        Private Sub UpdateHighlights()
            Dim amethyst = Color.FromArgb(171, 146, 191)
            Dim textDark = Color.FromArgb(45, 38, 55)

            For Each pair In dayButtons
                pair.Value.BackColor = Color.White
                pair.Value.ForeColor = textDark
            Next

            For Each d In selectedDates
                If dayButtons.ContainsKey(d) Then
                    dayButtons(d).BackColor = amethyst
                    dayButtons(d).ForeColor = Color.White
                End If
            Next
        End Sub

        Private Sub ApplyClicked(sender As Object, e As EventArgs)
            If selectedDates.Count = 0 Then
                MessageBox.Show("Choose a date first.")
                Return
            End If

            StartDate = selectedDates.Min()
            EndDate = selectedDates.Max()

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class

    Private Class TransactionDialog
        Inherits Form

        Public Property ItemName As String
        Public Property BorrowerName As String
        Public Property IssuedBy As String
        Public Property ReturnedBy As String
        Public Property DateIssued As Date
        Public Property DueDate As Date
        Public Property IsReturned As Boolean

        Private txtItem As TextBox
        Private txtBorrower As TextBox
        Private txtIssuedBy As TextBox
        Private txtReturnedBy As TextBox
        Private dtpIssued As DateTimePicker
        Private dtpDue As DateTimePicker
        Private chkReturned As CheckBox

        Public Sub New(Optional existing As TransactionItem = Nothing)
            Me.Text = If(existing Is Nothing, "Add Borrow Transaction", "Edit Borrow Transaction")
            Me.Size = New Size(430, 465)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.BackColor = Color.FromArgb(253, 241, 226)
            Me.MaximizeBox = False
            Me.MinimizeBox = False

            txtItem = New TextBox With {.Location = New Point(40, 55), .Size = New Size(330, 30)}
            txtBorrower = New TextBox With {.Location = New Point(40, 115), .Size = New Size(330, 30)}
            txtIssuedBy = New TextBox With {.Location = New Point(40, 175), .Size = New Size(330, 30)}
            txtReturnedBy = New TextBox With {.Location = New Point(40, 235), .Size = New Size(330, 30)}

            dtpIssued = New DateTimePicker With {.Location = New Point(40, 295), .Size = New Size(150, 30), .Format = DateTimePickerFormat.Short}
            dtpDue = New DateTimePicker With {.Location = New Point(220, 295), .Size = New Size(150, 30), .Format = DateTimePickerFormat.Short}

            chkReturned = New CheckBox With {.Text = "Item has been returned", .Location = New Point(40, 335), .Size = New Size(250, 25)}

            If existing IsNot Nothing Then
                txtItem.Text = existing.ItemName
                txtBorrower.Text = existing.BorrowerName
                txtIssuedBy.Text = existing.IssuedBy
                txtReturnedBy.Text = existing.ReturnedBy
                dtpIssued.Value = existing.DateIssued
                dtpDue.Value = existing.DueDate
                chkReturned.Checked = existing.IsReturned
            Else
                dtpIssued.Value = Date.Today
                dtpDue.Value = Date.Today.AddDays(1)
            End If

            Dim save As New Button With {
                .Text = "Save",
                .Location = New Point(190, 380),
                .Size = New Size(85, 35),
                .BackColor = Color.FromArgb(101, 90, 124),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            Dim cancel As New Button With {
                .Text = "Cancel",
                .Location = New Point(285, 380),
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
                New Label With {.Text = "Item Name", .Location = New Point(40, 30)},
                txtItem,
                New Label With {.Text = "Borrower Name", .Location = New Point(40, 90)},
                txtBorrower,
                New Label With {.Text = "Issued By", .Location = New Point(40, 150)},
                txtIssuedBy,
                New Label With {.Text = "Returned By", .Location = New Point(40, 210)},
                txtReturnedBy,
                New Label With {.Text = "Date Issued", .Location = New Point(40, 270)},
                dtpIssued,
                New Label With {.Text = "Due Date", .Location = New Point(220, 270)},
                dtpDue,
                chkReturned,
                save,
                cancel
            })
        End Sub

        Private Sub SaveClicked(sender As Object, e As EventArgs)
            If txtItem.Text.Trim() = "" Then
                MessageBox.Show("Item name is required.")
                Return
            End If

            If txtBorrower.Text.Trim() = "" Then
                MessageBox.Show("Borrower name is required.")
                Return
            End If

            If txtIssuedBy.Text.Trim() = "" Then
                MessageBox.Show("Issued by is required.")
                Return
            End If

            If dtpDue.Value.Date < dtpIssued.Value.Date Then
                MessageBox.Show("Due date cannot be earlier than date issued.")
                Return
            End If

            If chkReturned.Checked AndAlso txtReturnedBy.Text.Trim() = "" Then
                MessageBox.Show("Returned by is required if the item has been returned.")
                Return
            End If

            ItemName = txtItem.Text.Trim()
            BorrowerName = txtBorrower.Text.Trim()
            IssuedBy = txtIssuedBy.Text.Trim()
            ReturnedBy = txtReturnedBy.Text.Trim()
            DateIssued = dtpIssued.Value.Date
            DueDate = dtpDue.Value.Date
            IsReturned = chkReturned.Checked

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class

End Class
