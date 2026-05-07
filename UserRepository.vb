Imports Microsoft.Data.SqlClient

Public Class UserAccount
    Public Property UserID As Integer
    Public Property Username As String
    Public Property FullName As String
    Public Property Email As String
    Public Property Role As String
    Public Property Password As String
    Public Property Permissions As String
    Public Property IsActive As Boolean
End Class

Public Module UserRepository

    Public Function GetAllUsers() As List(Of UserAccount)
        Dim users As New List(Of UserAccount)

        Using con As New SqlConnection(ConnectionString)
            con.Open()

            Dim query As String =
                "SELECT UserID, Username, FullName, Email, Role, Password, Permissions, IsActive
                 FROM Users
                 WHERE IsActive = 1
                 ORDER BY UserID"

            Using cmd As New SqlCommand(query, con)
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        users.Add(New UserAccount With {
                            .UserID = CInt(reader("UserID")),
                            .Username = reader("Username").ToString(),
                            .FullName = reader("FullName").ToString(),
                            .Email = reader("Email").ToString(),
                            .Role = reader("Role").ToString(),
                            .Password = reader("Password").ToString(),
                            .Permissions = reader("Permissions").ToString(),
                            .IsActive = CBool(reader("IsActive"))
                        })
                    End While
                End Using
            End Using
        End Using

        Return users
    End Function

    Public Function UsernameExists(username As String) As Boolean
        Using con As New SqlConnection(ConnectionString)
            con.Open()

            Dim query As String =
                "SELECT COUNT(*) FROM Users WHERE Username = @Username"

            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@Username", username)
                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
            End Using
        End Using
    End Function

    Public Function EmailExists(email As String) As Boolean
        Using con As New SqlConnection(ConnectionString)
            con.Open()

            Dim query As String =
                "SELECT COUNT(*) FROM Users WHERE Email = @Email"

            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@Email", email)
                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
            End Using
        End Using
    End Function

    Public Sub AddUser(username As String, fullName As String, email As String, role As String, password As String, permissions As String)
        Using con As New SqlConnection(ConnectionString)
            con.Open()

            Dim query As String =
                "INSERT INTO Users (Username, FullName, Email, Role, Password, Permissions)
                 VALUES (@Username, @FullName, @Email, @Role, @Password, @Permissions)"

            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@Username", username)
                cmd.Parameters.AddWithValue("@FullName", fullName)
                cmd.Parameters.AddWithValue("@Email", email)
                cmd.Parameters.AddWithValue("@Role", role)
                cmd.Parameters.AddWithValue("@Password", password)
                cmd.Parameters.AddWithValue("@Permissions", permissions)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub ResetPassword(username As String, newPassword As String)
        Using con As New SqlConnection(ConnectionString)
            con.Open()

            Dim query As String =
                "UPDATE Users
                 SET Password = @Password
                 WHERE Username = @Username AND IsActive = 1"

            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@Username", username)
                cmd.Parameters.AddWithValue("@Password", newPassword)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

End Module