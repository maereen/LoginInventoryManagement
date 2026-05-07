using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace InventoryBackend
{
    public class SupplierRecord
    {
        public string SupplierID { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public string ContactPerson { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime DateAdded { get; set; }
    }

    public class SupplierRepository
    {
        private readonly string _connectionString =
            @"Data Source=(localdb)\ProjectModels;Initial Catalog=InventoryManagementDB;Integrated Security=True;TrustServerCertificate=True";

        public List<SupplierRecord> GetAll()
        {
            List<SupplierRecord> suppliers = new List<SupplierRecord>();

            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = "SELECT * FROM Suppliers ORDER BY DateAdded DESC, SupplierID DESC";

            using SqlCommand cmd = new SqlCommand(query, con);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                suppliers.Add(new SupplierRecord
                {
                    SupplierID = reader["SupplierID"].ToString() ?? "",
                    SupplierName = reader["SupplierName"].ToString() ?? "",
                    ContactPerson = reader["ContactPerson"].ToString() ?? "",
                    Phone = reader["Phone"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    Address = reader["Address"].ToString() ?? "",
                    Status = reader["Status"].ToString() ?? "",
                    DateAdded = Convert.ToDateTime(reader["DateAdded"])
                });
            }

            return suppliers;
        }

        public string GenerateNextSupplierId()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT ISNULL(MAX(TRY_CAST(REPLACE(SupplierID, 'SUP-', '') AS INT)), 0) + 1
                FROM Suppliers
                WHERE SupplierID LIKE 'SUP-%'";

            using SqlCommand cmd = new SqlCommand(query, con);

            int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());
            return "SUP-" + nextNumber.ToString("000");
        }

        public void Add(SupplierRecord supplier)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                INSERT INTO Suppliers
                (SupplierID, SupplierName, ContactPerson, Phone, Email, Address, Status, DateAdded)
                VALUES
                (@SupplierID, @SupplierName, @ContactPerson, @Phone, @Email, @Address, @Status, @DateAdded)";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@SupplierID", supplier.SupplierID);
            cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
            cmd.Parameters.AddWithValue("@ContactPerson", supplier.ContactPerson);
            cmd.Parameters.AddWithValue("@Phone", supplier.Phone);
            cmd.Parameters.AddWithValue("@Email", supplier.Email);
            cmd.Parameters.AddWithValue("@Address", supplier.Address);
            cmd.Parameters.AddWithValue("@Status", supplier.Status);
            cmd.Parameters.AddWithValue("@DateAdded", supplier.DateAdded);

            cmd.ExecuteNonQuery();
        }

        public void Update(SupplierRecord supplier)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                UPDATE Suppliers
                SET SupplierName = @SupplierName,
                    ContactPerson = @ContactPerson,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address,
                    Status = @Status,
                    DateAdded = @DateAdded
                WHERE SupplierID = @SupplierID";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@SupplierID", supplier.SupplierID);
            cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
            cmd.Parameters.AddWithValue("@ContactPerson", supplier.ContactPerson);
            cmd.Parameters.AddWithValue("@Phone", supplier.Phone);
            cmd.Parameters.AddWithValue("@Email", supplier.Email);
            cmd.Parameters.AddWithValue("@Address", supplier.Address);
            cmd.Parameters.AddWithValue("@Status", supplier.Status);
            cmd.Parameters.AddWithValue("@DateAdded", supplier.DateAdded);

            cmd.ExecuteNonQuery();
        }

        public void Delete(string supplierId)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = "DELETE FROM Suppliers WHERE SupplierID = @SupplierID";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@SupplierID", supplierId);

            cmd.ExecuteNonQuery();
        }
    }
}
