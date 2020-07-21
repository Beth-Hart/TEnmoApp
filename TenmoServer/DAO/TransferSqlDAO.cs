using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public bool AddTransferRequest(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) ";
                    query += "VALUES ((SELECT transfer_type_id FROM transfer_types WHERE transfer_type_desc = @transfertypedesc), ";
                    query += "(SELECT transfer_status_id FROM transfer_statuses WHERE transfer_status_desc = @transferstatusdesc), ";
                    query += "(SELECT account_id FROM accounts WHERE user_id = @user_id_from), ";
                    query += "(SELECT account_id FROM accounts WHERE user_id = @user_id_to), ";
                    query += "@amount)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@transfertypedesc", transfer.TransferType.ToString());
                    cmd.Parameters.AddWithValue("@transferstatusdesc", transfer.TransferStatus.ToString());
                    cmd.Parameters.AddWithValue("@user_id_from", transfer.FromUserID);
                    cmd.Parameters.AddWithValue("@user_id_to", transfer.ToUserID);
                    cmd.Parameters.AddWithValue("@amount", transfer.TransferAmount);
                    cmd.ExecuteNonQuery();

                }
            }
            catch (SqlException e)
            {
                return false;
            }
            return true;
        }


        public List<Transfer> GetTransfers(string username)
        {
            List<Transfer> returnTransfers = new List<Transfer>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT transfer_id, users_from.user_id AS user_id_from, users_to.user_id AS user_id_to, ";
                    query += "users_from.username AS username_from, users_to.username AS username_to, transfers.amount, ";
                    query += "transfer_types.transfer_type_desc, transfer_statuses.transfer_status_desc ";
                    query += "FROM transfers ";
                    query += "JOIN accounts as accounts_from ON accounts_from.account_id = transfers.account_from ";
                    query += "JOIN users AS users_from ON users_from.user_id = accounts_from.account_id ";
                    query += "JOIN accounts as accounts_to ON accounts_to.account_id = transfers.account_to ";
                    query += "JOIN users AS users_to ON users_to.user_id = accounts_to.account_id ";
                    query += "JOIN transfer_statuses ON transfer_statuses.transfer_status_id = transfers.transfer_status_id ";
                    query += "JOIN transfer_types ON transfer_types.transfer_type_id = transfers.transfer_type_id ";
                    query += "WHERE users_from.username = @username or users_to.username = @username";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Transfer t = GetTransferFromReader(reader);
                            returnTransfers.Add(t);
                        }

                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }

            return returnTransfers;
        }


        public Transfer GetTransferByTransferID(int transferID)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT transfer_id, users_from.user_id AS user_id_from, users_to.user_id AS user_id_to, ";
                    query += "users_from.username AS username_from, users_to.username AS username_to, transfers.amount, ";
                    query += "transfer_types.transfer_type_desc, transfer_statuses.transfer_status_desc ";
                    query += "FROM transfers ";
                    query += "JOIN accounts as accounts_from ON accounts_from.account_id = transfers.account_from ";
                    query += "JOIN users AS users_from ON users_from.user_id = accounts_from.account_id ";
                    query += "JOIN accounts as accounts_to ON accounts_to.account_id = transfers.account_to ";
                    query += "JOIN users AS users_to ON users_to.user_id = accounts_to.account_id ";
                    query += "JOIN transfer_statuses ON transfer_statuses.transfer_status_id = transfers.transfer_status_id ";
                    query += "JOIN transfer_types ON transfer_types.transfer_type_id = transfers.transfer_type_id ";
                    query += "WHERE transfers.transfer_id = @transferid";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@transferid", transferID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        Transfer t = new Transfer();
                        while (reader.Read())
                        {
                            t = GetTransferFromReader(reader);
                        }
                        return t;
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }
            return null;
        }


        public bool UpdateTransferStatus(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE transfers " ;
                    query += "SET transfer_status_id = (SELECT transfer_status_id FROM transfer_statuses WHERE transfer_status_desc = @transferstatusdesc) ";
                    query += "WHERE transfer_id = @transferid ";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@transferstatusdesc", transfer.TransferStatus.ToString());
                    cmd.Parameters.AddWithValue("@transferid", transfer.TransferID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                return false;
            }
            return true;
        }



        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                TransferType = (TransferType)Enum.Parse(typeof(TransferType), Convert.ToString(reader["transfer_type_desc"])),
                TransferAmount = Convert.ToDecimal(reader["amount"]),
                TransferStatus = (TransferStatus)Enum.Parse(typeof(TransferStatus), Convert.ToString(reader["transfer_status_desc"])),
                TransferID = Convert.ToInt32(reader["transfer_id"]),
                FromUserName = Convert.ToString(reader["username_from"]),
                ToUserName = Convert.ToString(reader["username_to"]),
                FromUserID = Convert.ToInt32(reader["user_id_from"]),
                ToUserID = Convert.ToInt32(reader["user_id_to"])
            };

            return t; 
        }

    }
}

