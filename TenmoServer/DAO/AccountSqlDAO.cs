using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {
        private readonly string connectionString;

        public AccountSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Account GetAccount(string username)
        {
            Account returnAccount = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT accounts.account_id, accounts.user_id, balance FROM accounts, users ";
                    query += "WHERE accounts.user_id = users.user_id AND users.username = @username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnAccount = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }

            return returnAccount;
        }

        public decimal? GetBalance(int userID)
        {
            Account returnAccount = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT account_id, user_id, balance FROM accounts ";
                    query += "WHERE user_Id = @user_id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", userID); 
                    SqlDataReader reader = cmd.ExecuteReader();                                   

                    if (reader.HasRows && reader.Read())
                    {
                        returnAccount = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }

            return returnAccount.Balance;
        }

        private Account GetAccountFromReader(SqlDataReader reader)
        {
            Account u = new Account()
            {
                AccountID = Convert.ToInt32(reader["account_id"]),
                UserId = Convert.ToInt32(reader["user_id"]),
                Balance = Convert.ToDecimal(reader["balance"])
            };

            return u;
        }

        public bool DoTransfer(Transfer transfer)
        {
            if (!changeBalance(transfer.ToUserID, transfer.TransferAmount)) return false;
            if (!changeBalance(transfer.FromUserID, -transfer.TransferAmount)) return false;

            return true;
        }

        private bool changeBalance(int userID, decimal change)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "UPDATE accounts SET balance = balance + @amount WHERE user_id = @user_id";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@user_id", userID);
                    cmd.Parameters.AddWithValue("@amount", change);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                return false;
            }

            return true;
        }

    }
}
