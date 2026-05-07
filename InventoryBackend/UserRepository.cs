using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace InventoryBackend
{
    public class UserAccount
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Password { get; set; } = "";
        public string Permissions { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public class UserRepository
    {
        private readonly string _connectionString =
            @"Data Source=(localdb)\ProjectModels;Initial Catalog=InventoryManagementDB;Integrated Security=True;TrustServerCertificate=True";

        public bool ValidateLogin(string username, string password)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT COUNT(*)
                FROM Users
                WHERE Username = @Username
                AND Password = @Password
                AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public (bool success, string username, string fullName, string email, string role, string permissions) Login(string username, string password)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT Username, FullName, Email, Role, Permissions
                FROM Users
                WHERE Username = @Username
                AND Password = @Password
                AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return (
                    true,
                    reader["Username"].ToString() ?? "",
                    reader["FullName"].ToString() ?? "",
                    reader["Email"].ToString() ?? "",
                    reader["Role"].ToString() ?? "",
                    reader["Permissions"].ToString() ?? ""
                );
            }

            return (false, "", "", "", "", "");
        }

        public List<UserAccount> GetAllUsers()
        {
            List<UserAccount> users = new List<UserAccount>();

            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT UserId, Username, FullName, Email, Role, Password, Permissions, IsActive
                FROM Users
                WHERE IsActive = 1
                ORDER BY UserId DESC";

            using SqlCommand cmd = new SqlCommand(query, con);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new UserAccount
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    Username = reader["Username"].ToString() ?? "",
                    FullName = reader["FullName"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    Role = reader["Role"].ToString() ?? "",
                    Password = reader["Password"].ToString() ?? "",
                    Permissions = reader["Permissions"].ToString() ?? "",
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }

            return users;
        }

        public bool UsernameExists(string username)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT COUNT(*)
                FROM Users
                WHERE LOWER(Username) = LOWER(@Username)
                AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", username);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public bool EmailExists(string email)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT COUNT(*)
                FROM Users
                WHERE LOWER(Email) = LOWER(@Email)
                AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Email", email);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public void AddUser(string username, string fullName, string email, string password, string role, string permissions)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                INSERT INTO Users (Username, FullName, Email, Password, Role, Permissions, IsActive)
                VALUES (@Username, @FullName, @Email, @Password, @Role, @Permissions, 1)";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@FullName", fullName);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@Role", role);
            cmd.Parameters.AddWithValue("@Permissions", permissions);

            cmd.ExecuteNonQuery();
        }

        public void ResetPassword(string username, string newPassword)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                UPDATE Users
                SET Password = @Password
                WHERE Username = @Username
                AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", newPassword);

            cmd.ExecuteNonQuery();
        }

        public void RemoveUser(string username)
        {
            if (username.ToLower() == "admin")
            {
                throw new Exception("The main System Admin account cannot be removed.");
            }

            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                UPDATE Users
                SET IsActive = 0
                WHERE Username = @Username";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", username);

            cmd.ExecuteNonQuery();
        }
    }
}
