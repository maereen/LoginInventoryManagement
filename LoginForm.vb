Public Class LoginForm
    Private Sub btnclose_Click(sender As Object, e As EventArgs) Handles btnclose.Click
        Application.Exit()
    End Sub
    Private Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Me.Hide()
        HomeForm.Show()
    End Sub
End Class