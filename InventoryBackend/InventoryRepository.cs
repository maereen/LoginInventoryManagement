using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace InventoryBackend
{
    public class InventoryItem
    {
        public string ItemID { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string Category { get; set; } = "";
        public int Quantity { get; set; }
        public string Status { get; set; } = "";
        public DateTime DateAdded { get; set; }
    }

    public class InventoryRepository
    {
        private readonly string _connectionString =
            @"Data Source=(localdb)\ProjectModels;Initial Catalog=InventoryManagementDB;Integrated Security=True;TrustServerCertificate=True";

        public List<InventoryItem> GetAll()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = "SELECT * FROM Inventory ORDER BY DateAdded DESC, ItemID DESC";

            using SqlCommand cmd = new SqlCommand(query, con);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new InventoryItem
                {
                    ItemID = reader["ItemID"].ToString() ?? "",
                    ItemName = reader["ItemName"].ToString() ?? "",
                    Category = reader["Category"].ToString() ?? "",
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Status = reader["Status"].ToString() ?? "",
                    DateAdded = Convert.ToDateTime(reader["DateAdded"])
                });
            }

            return items;
        }

        public string GenerateNextItemId()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT ISNULL(MAX(TRY_CAST(REPLACE(ItemID, 'ITM-', '') AS INT)), 0) + 1
                FROM Inventory
                WHERE ItemID LIKE 'ITM-%'";

            using SqlCommand cmd = new SqlCommand(query, con);

            int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());
            return "ITM-" + nextNumber.ToString("000");
        }

        public void Add(InventoryItem item)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                INSERT INTO Inventory (ItemID, ItemName, Category, Quantity, Status, DateAdded)
                VALUES (@ItemID, @ItemName, @Category, @Quantity, @Status, @DateAdded)";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@Category", item.Category);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@Status", item.Status);
            cmd.Parameters.AddWithValue("@DateAdded", item.DateAdded);

            cmd.ExecuteNonQuery();
        }

        public void Update(InventoryItem item)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                UPDATE Inventory
                SET ItemName = @ItemName,
                    Category = @Category,
                    Quantity = @Quantity,
                    Status = @Status,
                    DateAdded = @DateAdded
                WHERE ItemID = @ItemID";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@Category", item.Category);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@Status", item.Status);
            cmd.Parameters.AddWithValue("@DateAdded", item.DateAdded);

            cmd.ExecuteNonQuery();
        }

        public void Delete(string itemId)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = "DELETE FROM Inventory WHERE ItemID = @ItemID";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ItemID", itemId);

            cmd.ExecuteNonQuery();
        }
    }
}
