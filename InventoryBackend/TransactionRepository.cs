using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace InventoryBackend
{
    public class TransactionRecord
    {
        public string TransID { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string BorrowerName { get; set; } = "";
        public string IssuedBy { get; set; } = "";
        public string ReturnedBy { get; set; } = "";
        public DateTime DateIssued { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
    }

    public class TransactionRepository
    {
        private readonly string _connectionString =
            @"Data Source=(localdb)\ProjectModels;Initial Catalog=InventoryManagementDB;Integrated Security=True;TrustServerCertificate=True";

        public List<TransactionRecord> GetAll()
        {
            List<TransactionRecord> transactions = new List<TransactionRecord>();

            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = "SELECT * FROM Transactions ORDER BY DateIssued DESC, TransID DESC";

            using SqlCommand cmd = new SqlCommand(query, con);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(new TransactionRecord
                {
                    TransID = reader["TransID"].ToString() ?? "",
                    ItemName = reader["ItemName"].ToString() ?? "",
                    BorrowerName = reader["BorrowerName"].ToString() ?? "",
                    IssuedBy = reader["IssuedBy"].ToString() ?? "",
                    ReturnedBy = reader["ReturnedBy"].ToString() ?? "",
                    DateIssued = Convert.ToDateTime(reader["DateIssued"]),
                    DueDate = Convert.ToDateTime(reader["DueDate"]),
                    IsReturned = Convert.ToBoolean(reader["IsReturned"])
                });
            }

            return transactions;
        }

        public string GenerateNextTransId()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT ISNULL(MAX(TRY_CAST(REPLACE(TransID, 'TR-', '') AS INT)), 0) + 1
                FROM Transactions
                WHERE TransID LIKE 'TR-%'";

            using SqlCommand cmd = new SqlCommand(query, con);

            int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());
            return "TR-" + nextNumber.ToString("000");
        }

        public void Add(TransactionRecord transaction)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                INSERT INTO Transactions
                (TransID, ItemName, BorrowerName, IssuedBy, ReturnedBy, DateIssued, DueDate, IsReturned)
                VALUES
                (@TransID, @ItemName, @BorrowerName, @IssuedBy, @ReturnedBy, @DateIssued, @DueDate, @IsReturned)";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TransID", transaction.TransID);
            cmd.Parameters.AddWithValue("@ItemName", transaction.ItemName);
            cmd.Parameters.AddWithValue("@BorrowerName", transaction.BorrowerName);
            cmd.Parameters.AddWithValue("@IssuedBy", transaction.IssuedBy);
            cmd.Parameters.AddWithValue("@ReturnedBy", transaction.ReturnedBy);
            cmd.Parameters.AddWithValue("@DateIssued", transaction.DateIssued);
            cmd.Parameters.AddWithValue("@DueDate", transaction.DueDate);
            cmd.Parameters.AddWithValue("@IsReturned", transaction.IsReturned);

            cmd.ExecuteNonQuery();
        }

        public void Update(TransactionRecord transaction)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                UPDATE Transactions
                SET ItemName = @ItemName,
                    BorrowerName = @BorrowerName,
                    IssuedBy = @IssuedBy,
                    ReturnedBy = @ReturnedBy,
                    DateIssued = @DateIssued,
                    DueDate = @DueDate,
                    IsReturned = @IsReturned
                WHERE TransID = @TransID";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TransID", transaction.TransID);
            cmd.Parameters.AddWithValue("@ItemName", transaction.ItemName);
            cmd.Parameters.AddWithValue("@BorrowerName", transaction.BorrowerName);
            cmd.Parameters.AddWithValue("@IssuedBy", transaction.IssuedBy);
            cmd.Parameters.AddWithValue("@ReturnedBy", transaction.ReturnedBy);
            cmd.Parameters.AddWithValue("@DateIssued", transaction.DateIssued);
            cmd.Parameters.AddWithValue("@DueDate", transaction.DueDate);
            cmd.Parameters.AddWithValue("@IsReturned", transaction.IsReturned);

            cmd.ExecuteNonQuery();
        }

        public void Delete(string transId)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string query = "DELETE FROM Transactions WHERE TransID = @TransID";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TransID", transId);

            cmd.ExecuteNonQuery();
        }
    }
}
