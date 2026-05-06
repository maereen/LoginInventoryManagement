Public Class HomeForm

    Private Sub btndashboard_Click(sender As Object, e As EventArgs) Handles btndashboard.Click
        SidePanel.Height = btndashboard.Height
        SidePanel.Top = btndashboard.Top
    End Sub

    Private Sub btnInventory_Click(sender As Object, e As EventArgs) Handles btnInventory.Click
        SidePanel.Height = btnInventory.Height
        SidePanel.Top = btnInventory.Top
    End Sub

    Private Sub btntransaction_Click(sender As Object, e As EventArgs) Handles btntransaction.Click
        SidePanel.Height = btntransaction.Height
        SidePanel.Top = btntransaction.Top
    End Sub

    Private Sub btnsuppliers_Click(sender As Object, e As EventArgs) Handles btnsuppliers.Click
        SidePanel.Height = btnsuppliers.Height
        SidePanel.Top = btnsuppliers.Top
    End Sub

    Private Sub btnreports_Click(sender As Object, e As EventArgs) Handles btnreports.Click
        SidePanel.Height = btnreports.Height
        SidePanel.Top = btnreports.Top
    End Sub

    Private Sub btnsettings_Click(sender As Object, e As EventArgs) Handles btnsettings.Click
        SidePanel.Height = btnsettings.Height
        SidePanel.Top = btnsettings.Top
    End Sub
    Private Sub btnlogout_Click(sender As Object, e As EventArgs) Handles btnlogout.Click
        Me.Hide()
        LoginForm.Show()
    End Sub

    Private Sub PictureBox12_Click(sender As Object, e As EventArgs) Handles PictureBox12.Click



        Dim response As Integer

        response = MessageBox.Show("Are you sure you want to exit ?", "Exit application",
MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If response = vbYes Then
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
End Class