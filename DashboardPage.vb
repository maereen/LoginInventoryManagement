Public Class DashboardPage

    Private title As Label
    Private cards As New List(Of Panel)
    Private bottomLeft As Panel
    Private bottomRight As Panel

    Private Sub DashboardPage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(248, 244, 250)
        BuildUI()
        LayoutUI()
    End Sub

    Private Sub DashboardPage_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        LayoutUI()
    End Sub

    Private Sub BuildUI()
        Me.Controls.Clear()
        cards.Clear()

        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim amethyst As Color = Color.FromArgb(171, 146, 191)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        title = New Label With {
            .Text = "Dashboard",
            .Font = New Font("Segoe UI Semilight", 22),
            .ForeColor = dolphin,
            .Location = New Point(24, 20),
            .AutoSize = True
        }
        Me.Controls.Add(title)

        cards.Add(CreateCard("Total Items", "4", amethyst, textDark))
        cards.Add(CreateCard("Borrowed", "2", amethyst, textDark))
        cards.Add(CreateCard("For Return", "1", amethyst, textDark))
        cards.Add(CreateCard("Overdue", "1", amethyst, textDark))

        For Each card In cards
            Me.Controls.Add(card)
        Next

        bottomLeft = CreateLargePanel("Recent Activity", "Recent transactions will appear here.")
        bottomRight = CreateLargePanel("Inventory Alerts", "Low stock / defective items will appear here.")

        Me.Controls.Add(bottomLeft)
        Me.Controls.Add(bottomRight)
    End Sub

    Private Function CreateCard(labelText As String, numberText As String, numberColor As Color, textColor As Color) As Panel
        Dim card As New Panel With {
            .BackColor = Color.White
        }

        Dim number As New Label With {
            .Text = numberText,
            .Font = New Font("Segoe UI", 24, FontStyle.Bold),
            .ForeColor = numberColor,
            .AutoSize = True,
            .Location = New Point(18, 12)
        }

        Dim label As New Label With {
            .Text = labelText,
            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .ForeColor = textColor,
            .AutoSize = True,
            .Location = New Point(20, 58)
        }

        card.Controls.Add(number)
        card.Controls.Add(label)

        Return card
    End Function

    Private Function CreateLargePanel(headerText As String, bodyText As String) As Panel
        Dim dolphin As Color = Color.FromArgb(101, 90, 124)
        Dim textDark As Color = Color.FromArgb(45, 38, 55)

        Dim panel As New Panel With {
            .BackColor = Color.White
        }

        Dim header As New Label With {
            .Text = headerText,
            .Font = New Font("Segoe UI", 13, FontStyle.Bold),
            .ForeColor = dolphin,
            .Location = New Point(20, 18),
            .AutoSize = True
        }

        Dim body As New Label With {
            .Text = bodyText,
            .Font = New Font("Segoe UI", 10),
            .ForeColor = textDark,
            .Location = New Point(20, 60),
            .Size = New Size(320, 60)
        }

        panel.Controls.Add(header)
        panel.Controls.Add(body)

        Return panel
    End Function

    Private Sub LayoutUI()
        If title Is Nothing Then Return

        Dim margin As Integer = 24
        Dim gap As Integer = 18
        Dim contentWidth As Integer = Me.ClientSize.Width - (margin * 2)

        Dim cardWidth As Integer = Math.Max(150, (contentWidth - (gap * 3)) \ 4)
        Dim cardHeight As Integer = 90
        Dim topY As Integer = 78

        For i As Integer = 0 To cards.Count - 1
            cards(i).Location = New Point(margin + (i * (cardWidth + gap)), topY)
            cards(i).Size = New Size(cardWidth, cardHeight)
        Next

        Dim bottomY As Integer = topY + cardHeight + 48
        Dim bottomHeight As Integer = Math.Max(180, Me.ClientSize.Height - bottomY - margin)
        Dim bigWidth As Integer = Math.Max(250, (contentWidth - gap) \ 2)

        bottomLeft.Location = New Point(margin, bottomY)
        bottomLeft.Size = New Size(bigWidth, bottomHeight)

        bottomRight.Location = New Point(margin + bigWidth + gap, bottomY)
        bottomRight.Size = New Size(bigWidth, bottomHeight)
    End Sub

End Class
