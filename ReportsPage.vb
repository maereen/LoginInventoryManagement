Public Class ReportsPage

    Private scrollPanel As Panel
    Private contentPanel As Panel

    Private cmbReportType As ComboBox
    Private cmbDateFilter As ComboBox
    Private btnGenerate As Button

    Private summaryPanel As Panel
    Private resultPanel As Panel
    Private lblReportTitle As Label
    Private dgvReport As DataGridView

    Private customStartDate As Date? = Nothing
    Private customEndDate As Date? = Nothing

    Private Sub ReportsPage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(248, 244, 250)
        BuildUI()
        GenerateReport()
    End Sub

    Private Sub ReportsPage_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        LayoutUI()
    End Sub

    Private Sub BuildUI()
        Me.Controls.Clear()

        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim linen As Color = Color.FromArgb(253, 241, 226)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        scrollPanel = New Panel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True,
            .BackColor = Color.FromArgb(248, 244, 250)
        }

        contentPanel = New Panel With {
            .Location = New Point(0, 0),
            .Size = New Size(1000, 780),
            .BackColor = Color.FromArgb(248, 244, 250)
        }

        scrollPanel.Controls.Add(contentPanel)
        Me.Controls.Add(scrollPanel)

        Dim title As New Label With {
            .Text = "Reports",
            .Font = New Font("Segoe UI Semilight", 22),
            .ForeColor = dolphin,
            .Location = New Point(24, 20),
            .AutoSize = True
        }

        Dim lblType As New Label With {
            .Text = "Report Type",
            .Font = New Font("Segoe UI", 9),
            .ForeColor = textDark,
            .Location = New Point(24, 82),
            .AutoSize = True
        }

        cmbReportType = New ComboBox With {
            .Location = New Point(24, 105),
            .Size = New Size(260, 32),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 10)
        }

        cmbReportType.Items.AddRange(New String() {
            "Inventory Summary",
            "Transaction Summary",
            "Supplier Summary"
        })
        cmbReportType.SelectedIndex = 0

        Dim lblDate As New Label With {
            .Text = "Date Filter",
            .Font = New Font("Segoe UI", 9),
            .ForeColor = textDark,
            .Location = New Point(310, 82),
            .AutoSize = True
        }

        cmbDateFilter = New ComboBox With {
            .Location = New Point(310, 105),
            .Size = New Size(190, 32),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 10)
        }

        cmbDateFilter.Items.AddRange(New String() {
            "All Time",
            "This Month",
            "Last 3 Months",
            "Last 6 Months",
            "This Year",
            "Custom Date"
        })
        cmbDateFilter.SelectedIndex = 0

        AddHandler cmbDateFilter.SelectedIndexChanged, AddressOf DateFilterChanged

        btnGenerate = New Button With {
            .Text = "Generate Report",
            .Location = New Point(520, 104),
            .Size = New Size(160, 36),
            .BackColor = dolphin,
            .ForeColor = linen,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }
        btnGenerate.FlatAppearance.BorderSize = 0
        AddHandler btnGenerate.Click, AddressOf GenerateClicked

        summaryPanel = New Panel With {
            .Location = New Point(24, 165),
            .Size = New Size(900, 120),
            .BackColor = Color.Transparent
        }

        resultPanel = New Panel With {
            .Location = New Point(24, 315),
            .Size = New Size(900, 420),
            .BackColor = Color.White
        }

        lblReportTitle = New Label With {
            .Text = "Report Result",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = dolphin,
            .Location = New Point(18, 15),
            .AutoSize = True
        }

        dgvReport = New DataGridView With {
            .Location = New Point(18, 58),
            .Size = New Size(resultPanel.Width - 36, resultPanel.Height - 76),
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .RowHeadersVisible = False,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AllowUserToResizeRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .Font = New Font("Segoe UI", 10)
        }

        dgvReport.EnableHeadersVisualStyles = False
        dgvReport.ColumnHeadersDefaultCellStyle.BackColor = dolphin
        dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = linen
        dgvReport.ColumnHeadersDefaultCellStyle.SelectionBackColor = dolphin
        dgvReport.ColumnHeadersDefaultCellStyle.SelectionForeColor = linen
        dgvReport.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        dgvReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 220, 245)
        dgvReport.DefaultCellStyle.SelectionForeColor = textDark
        dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 248, 255)
        dgvReport.GridColor = Color.FromArgb(220, 210, 225)

        resultPanel.Controls.Add(lblReportTitle)
        resultPanel.Controls.Add(dgvReport)

        contentPanel.Controls.AddRange(New Control() {
            title,
            lblType,
            cmbReportType,
            lblDate,
            cmbDateFilter,
            btnGenerate,
            summaryPanel,
            resultPanel
        })

        LayoutUI()
    End Sub

    Private Sub LayoutUI()
        If contentPanel Is Nothing Then Return

        Dim gap As Integer = 20
        Dim cardWidth As Integer = 220
        Dim cardCount As Integer = If(summaryPanel Is Nothing, 5, Math.Max(5, summaryPanel.Controls.Count))
        Dim neededWidth As Integer = 24 + (cardCount * cardWidth) + ((cardCount - 1) * gap) + 48

        contentPanel.Width = Math.Max(neededWidth, Me.ClientSize.Width - 25)
        contentPanel.Height = 780

        If summaryPanel IsNot Nothing Then
            summaryPanel.Size = New Size(contentPanel.Width - 48, 120)

            For i As Integer = 0 To summaryPanel.Controls.Count - 1
                summaryPanel.Controls(i).Size = New Size(cardWidth, 95)
                summaryPanel.Controls(i).Location = New Point(i * (cardWidth + gap), 0)
            Next
        End If

        If resultPanel IsNot Nothing Then
            resultPanel.Size = New Size(contentPanel.Width - 48, 420)
        End If

        If dgvReport IsNot Nothing Then
            dgvReport.Size = New Size(resultPanel.Width - 36, resultPanel.Height - 76)
        End If
    End Sub

    Private Sub GenerateClicked(sender As Object, e As EventArgs)
        GenerateReport()
    End Sub

    Private Sub DateFilterChanged(sender As Object, e As EventArgs)
        If cmbDateFilter.Text = "Custom Date" Then
            Using dialog As New ReportDateDialog()
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
    End Sub

    Private Sub GenerateReport()
        If cmbReportType Is Nothing Then Return

        Select Case cmbReportType.Text
            Case "Inventory Summary"
                ShowInventoryReport()
            Case "Transaction Summary"
                ShowTransactionReport()
            Case "Supplier Summary"
                ShowSupplierReport()
        End Select

        LayoutUI()
    End Sub

    Private Sub ClearSummary()
        summaryPanel.Controls.Clear()
    End Sub

    Private Function CreateSummaryCard(title As String, value As String) As Panel
        Dim amethyst As Color = Color.FromArgb(171, 146, 191)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        Dim card As New Panel With {
            .Size = New Size(200, 95),
            .BackColor = Color.White
        }

        Dim lblValue As New Label With {
            .Text = value,
            .Font = New Font("Segoe UI", 20, FontStyle.Bold),
            .ForeColor = amethyst,
            .Location = New Point(18, 10),
            .Size = New Size(300, 38),
            .AutoEllipsis = True
        }

        Dim lblTitle As New Label With {
            .Text = title,
            .Font = New Font("Segoe UI", 10),
            .ForeColor = textDark,
            .Location = New Point(20, 58),
            .Size = New Size(300, 25),
            .AutoEllipsis = True
        }

        card.Controls.Add(lblValue)
        card.Controls.Add(lblTitle)

        Return card
    End Function

    Private Function DateMatches(d As Date) As Boolean
        Dim reportDate = d.Date

        Select Case cmbDateFilter.Text
            Case "All Time"
                Return True
            Case "This Month"
                Return reportDate.Month = Date.Today.Month AndAlso reportDate.Year = Date.Today.Year
            Case "Last 3 Months"
                Return reportDate >= Date.Today.AddMonths(-3)
            Case "Last 6 Months"
                Return reportDate >= Date.Today.AddMonths(-6)
            Case "This Year"
                Return reportDate.Year = Date.Today.Year
            Case "Custom Date"
                If customStartDate.HasValue AndAlso customEndDate.HasValue Then
                    Return reportDate >= customStartDate.Value.Date AndAlso reportDate <= customEndDate.Value.Date
                End If
                Return True
        End Select

        Return True
    End Function

    Private Sub ShowInventoryReport()
        Dim inventory = New List(Of InventoryReportItem) From {
            New InventoryReportItem With {.ItemID = "ITM-001", .ItemName = "Laptop", .Category = "Laptop", .Quantity = 12, .Status = "Available", .DateAdded = Date.Today.AddMonths(-1)},
            New InventoryReportItem With {.ItemID = "ITM-002", .ItemName = "Projector", .Category = "Projector", .Quantity = 5, .Status = "Available", .DateAdded = Date.Today.AddMonths(-2)},
            New InventoryReportItem With {.ItemID = "ITM-003", .ItemName = "Keyboard", .Category = "Keyboard", .Quantity = 25, .Status = "Available", .DateAdded = Date.Today.AddMonths(-4)},
            New InventoryReportItem With {.ItemID = "ITM-004", .ItemName = "Mouse", .Category = "Mouse", .Quantity = 2, .Status = "Available", .DateAdded = Date.Today.AddMonths(-7)}
        }

        Dim filtered = inventory.Where(Function(x) DateMatches(x.DateAdded)).ToList()

        ClearSummary()
        summaryPanel.Controls.Add(CreateSummaryCard("Total Items", filtered.Count.ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Available", filtered.Where(Function(x) x.Status = "Available").Count().ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("In Use", filtered.Where(Function(x) x.Status = "In use").Count().ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Defective", filtered.Where(Function(x) x.Status = "Defective").Count().ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Low Stock", filtered.Where(Function(x) x.Quantity <= 5).Count().ToString()))

        lblReportTitle.Text = "Inventory Summary Report"

        dgvReport.DataSource = Nothing
        dgvReport.DataSource = filtered.Select(Function(x) New InventoryReportGridRow With {
            .ItemID = x.ItemID,
            .ItemName = x.ItemName,
            .Category = x.Category,
            .Quantity = x.Quantity,
            .Status = x.Status,
            .DateAddedDisplay = x.DateAdded.ToString("yyyy-MM-dd")
        }).ToList()

        FormatReportGrid()
    End Sub

    Private Sub ShowTransactionReport()
        Dim transactions = New List(Of TransactionReportItem) From {
            New TransactionReportItem With {.TransID = "TR-001", .ItemName = "Laptop", .Borrower = "Juan Dela Cruz", .DateIssued = Date.Today.AddDays(-2), .DueDate = Date.Today.AddDays(2), .IsReturned = False},
            New TransactionReportItem With {.TransID = "TR-002", .ItemName = "Projector", .Borrower = "Maria Santos", .DateIssued = Date.Today.AddMonths(-1), .DueDate = Date.Today.AddDays(-1), .IsReturned = True},
            New TransactionReportItem With {.TransID = "TR-003", .ItemName = "Keyboard", .Borrower = "Carlo Reyes", .DateIssued = Date.Today.AddMonths(-4), .DueDate = Date.Today, .IsReturned = False},
            New TransactionReportItem With {.TransID = "TR-004", .ItemName = "Mouse", .Borrower = "Ana Lopez", .DateIssued = Date.Today.AddMonths(-7), .DueDate = Date.Today.AddDays(-2), .IsReturned = False}
        }

        Dim filtered = transactions.Where(Function(x) DateMatches(x.DateIssued)).ToList()

        ClearSummary()
        summaryPanel.Controls.Add(CreateSummaryCard("Total Transactions", filtered.Count.ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Returned", filtered.Where(Function(x) x.IsReturned).Count().ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Overdue", filtered.Where(Function(x) GetTransactionStatus(x) = "Overdue").Count().ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Most Borrowed", GetMostBorrowedItem(filtered)))

        lblReportTitle.Text = "Transaction Summary Report"

        dgvReport.DataSource = Nothing
        dgvReport.DataSource = filtered.Select(Function(x) New TransactionReportGridRow With {
            .TransID = x.TransID,
            .ItemName = x.ItemName,
            .Borrower = x.Borrower,
            .DateIssuedDisplay = x.DateIssued.ToString("yyyy-MM-dd"),
            .DueDateDisplay = x.DueDate.ToString("yyyy-MM-dd"),
            .Status = GetTransactionStatus(x)
        }).ToList()

        FormatReportGrid()
    End Sub

    Private Sub ShowSupplierReport()
        Dim suppliers = New List(Of SupplierReportItem) From {
            New SupplierReportItem With {.SupplierID = "SUP-001", .SupplierName = "TechSource PH", .ContactPerson = "Mark Reyes", .ContactNumber = "0917-123-4567", .ItemsSupplied = "Laptop, Mouse", .Status = "Active", .DateAdded = Date.Today.AddMonths(-1)},
            New SupplierReportItem With {.SupplierID = "SUP-002", .SupplierName = "OfficePro Supplies", .ContactPerson = "Ana Santos", .ContactNumber = "0928-222-3344", .ItemsSupplied = "Projector", .Status = "Active", .DateAdded = Date.Today.AddMonths(-3)},
            New SupplierReportItem With {.SupplierID = "SUP-003", .SupplierName = "CompuParts Trading", .ContactPerson = "Leo Cruz", .ContactNumber = "0915-555-6789", .ItemsSupplied = "Keyboard", .Status = "Inactive", .DateAdded = Date.Today.AddMonths(-7)}
        }

        Dim filtered = suppliers.Where(Function(x) DateMatches(x.DateAdded)).ToList()

        ClearSummary()
        summaryPanel.Controls.Add(CreateSummaryCard("Total Suppliers", filtered.Count.ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Active", filtered.Where(Function(x) x.Status = "Active").Count().ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Inactive", filtered.Where(Function(x) x.Status = "Inactive").Count().ToString()))
        summaryPanel.Controls.Add(CreateSummaryCard("Top Supplier", GetTopSupplier(filtered)))

        lblReportTitle.Text = "Supplier Summary Report"

        dgvReport.DataSource = Nothing
        dgvReport.DataSource = filtered.Select(Function(x) New SupplierReportGridRow With {
            .SupplierID = x.SupplierID,
            .SupplierName = x.SupplierName,
            .ContactPerson = x.ContactPerson,
            .ContactNumber = x.ContactNumber,
            .ItemsSupplied = x.ItemsSupplied,
            .Status = x.Status,
            .DateAddedDisplay = x.DateAdded.ToString("yyyy-MM-dd")
        }).ToList()

        FormatReportGrid()
    End Sub

    Private Sub FormatReportGrid()
        If dgvReport.Columns.Contains("ItemID") Then dgvReport.Columns("ItemID").HeaderText = "Item ID"
        If dgvReport.Columns.Contains("ItemName") Then dgvReport.Columns("ItemName").HeaderText = "Item Name"
        If dgvReport.Columns.Contains("DateAddedDisplay") Then dgvReport.Columns("DateAddedDisplay").HeaderText = "Date Added"

        If dgvReport.Columns.Contains("TransID") Then dgvReport.Columns("TransID").HeaderText = "Transaction ID"
        If dgvReport.Columns.Contains("DateIssuedDisplay") Then dgvReport.Columns("DateIssuedDisplay").HeaderText = "Date Issued"
        If dgvReport.Columns.Contains("DueDateDisplay") Then dgvReport.Columns("DueDateDisplay").HeaderText = "Due Date"

        If dgvReport.Columns.Contains("SupplierID") Then dgvReport.Columns("SupplierID").HeaderText = "Supplier ID"
        If dgvReport.Columns.Contains("SupplierName") Then dgvReport.Columns("SupplierName").HeaderText = "Supplier Name"
        If dgvReport.Columns.Contains("ContactPerson") Then dgvReport.Columns("ContactPerson").HeaderText = "Contact Person"
        If dgvReport.Columns.Contains("ContactNumber") Then dgvReport.Columns("ContactNumber").HeaderText = "Contact Number"
        If dgvReport.Columns.Contains("ItemsSupplied") Then dgvReport.Columns("ItemsSupplied").HeaderText = "Items Supplied"

        For Each col As DataGridViewColumn In dgvReport.Columns
            col.SortMode = DataGridViewColumnSortMode.NotSortable
            col.ReadOnly = True
        Next

        dgvReport.ClearSelection()
        dgvReport.CurrentCell = Nothing
    End Sub

    Private Function GetTransactionStatus(item As TransactionReportItem) As String
        If item.IsReturned Then Return "Completed"
        If item.DueDate.Date = Date.Today Then Return "For Return"
        If item.DueDate.Date < Date.Today Then Return "Overdue"
        Return "Borrowed"
    End Function

    Private Function GetMostBorrowedItem(items As List(Of TransactionReportItem)) As String
        If items.Count = 0 Then Return "-"

        Return items.GroupBy(Function(x) x.ItemName).OrderByDescending(Function(g) g.Count()).First().Key
    End Function

    Private Function GetTopSupplier(items As List(Of SupplierReportItem)) As String
        If items.Count = 0 Then Return "-"

        Return items.OrderByDescending(Function(x) x.ItemsSupplied.Split(","c).Length).First().SupplierName
    End Function

    Private Class InventoryReportItem
        Public Property ItemID As String
        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String
        Public Property DateAdded As Date
    End Class

    Private Class TransactionReportItem
        Public Property TransID As String
        Public Property ItemName As String
        Public Property Borrower As String
        Public Property DateIssued As Date
        Public Property DueDate As Date
        Public Property IsReturned As Boolean
    End Class

    Private Class SupplierReportItem
        Public Property SupplierID As String
        Public Property SupplierName As String
        Public Property ContactPerson As String
        Public Property ContactNumber As String
        Public Property ItemsSupplied As String
        Public Property Status As String
        Public Property DateAdded As Date
    End Class

    Private Class InventoryReportGridRow
        Public Property ItemID As String
        Public Property ItemName As String
        Public Property Category As String
        Public Property Quantity As Integer
        Public Property Status As String
        Public Property DateAddedDisplay As String
    End Class

    Private Class TransactionReportGridRow
        Public Property TransID As String
        Public Property ItemName As String
        Public Property Borrower As String
        Public Property DateIssuedDisplay As String
        Public Property DueDateDisplay As String
        Public Property Status As String
    End Class

    Private Class SupplierReportGridRow
        Public Property SupplierID As String
        Public Property SupplierName As String
        Public Property ContactPerson As String
        Public Property ContactNumber As String
        Public Property ItemsSupplied As String
        Public Property Status As String
        Public Property DateAddedDisplay As String
    End Class

    Private Class ReportDateDialog
        Inherits Form

        Public Property StartDate As Date
        Public Property EndDate As Date

        Private dtpStart As DateTimePicker
        Private dtpEnd As DateTimePicker

        Public Sub New()
            Me.Text = "Custom Report Date"
            Me.Size = New Size(360, 230)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.BackColor = Color.FromArgb(253, 241, 226)
            Me.MaximizeBox = False
            Me.MinimizeBox = False

            dtpStart = New DateTimePicker With {
                .Location = New Point(35, 55),
                .Size = New Size(270, 30),
                .Format = DateTimePickerFormat.Short
            }

            dtpEnd = New DateTimePicker With {
                .Location = New Point(35, 115),
                .Size = New Size(270, 30),
                .Format = DateTimePickerFormat.Short
            }

            Dim save As New Button With {
                .Text = "Apply",
                .Location = New Point(125, 160),
                .Size = New Size(85, 35),
                .BackColor = Color.FromArgb(101, 90, 124),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            Dim cancel As New Button With {
                .Text = "Cancel",
                .Location = New Point(220, 160),
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
                New Label With {.Text = "Start Date", .Location = New Point(35, 30)},
                dtpStart,
                New Label With {.Text = "End Date", .Location = New Point(35, 90)},
                dtpEnd,
                save,
                cancel
            })
        End Sub

        Private Sub SaveClicked(sender As Object, e As EventArgs)
            If dtpEnd.Value.Date < dtpStart.Value.Date Then
                MessageBox.Show("End date cannot be earlier than start date.")
                Return
            End If

            StartDate = dtpStart.Value.Date
            EndDate = dtpEnd.Value.Date

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class

End Class
