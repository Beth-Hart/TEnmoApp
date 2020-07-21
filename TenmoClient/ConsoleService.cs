using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class ConsoleService
    {
        /// <summary>
        /// Prompts for transfer ID to view, approve, or reject
        /// </summary>
        /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
        /// <returns>ID of transfers to view, approve, or reject</returns>

        private const int COLUMN_WIDTH_TRANSFER_ID = 10;
        private const int COLUMN_WIDTH_TRANSFER_OTHER = 20;
        private const int COLUMN_WIDTH_TRANSFER_AMOUNT = 10;

        private const int COLUMN_WIDTH_USER_ID = 10;
        private const int COLUMN_WIDTH_USER_NAME = 20;

        private const int COLUMN_WIDTH_TRANSFER_DETAILS = 20;

        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int transferID))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return transferID;
            }
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }

        public API_Transfer PromptForTransferRequest(TransferType transferType)
        {
            bool succeedID = false;
            bool succeedDollarAmount = false;
            var transfer = new API_Transfer();

            int UserIDInput = 0;
            decimal dollarAmountInput = 0;

            string[] userResponseArray;

            do
            {
                Console.WriteLine("Please enter user id and transfer amount respectively.");
                Console.WriteLine("For Example: 1 100");
                var userResponse = Console.ReadLine();
                userResponseArray = userResponse.Split(" ");

                if (userResponseArray.Length != 2) continue;

                succeedID = Int32.TryParse(userResponseArray[0], out UserIDInput);
                succeedDollarAmount = Decimal.TryParse(userResponseArray[1], out dollarAmountInput);
            }
            while (!succeedID || !succeedDollarAmount);

            if(transferType == TransferType.Send)
            {
                transfer.ToUserID = UserIDInput;
            }
            else { transfer.FromUserID = UserIDInput; }
            transfer.TransferAmount = dollarAmountInput;
            transfer.TransferType = transferType;
            return transfer;
        }

        public void PrintUsers(List<API_User> users)
        {
            WriteDashes(COLUMN_WIDTH_USER_ID + COLUMN_WIDTH_USER_NAME);

            Console.WriteLine("Users");
            Console.Write("ID".PadRight(COLUMN_WIDTH_USER_ID, ' '));
            Console.Write("Name".PadRight(COLUMN_WIDTH_USER_NAME, ' '));
            Console.WriteLine();

            WriteDashes(COLUMN_WIDTH_USER_ID + COLUMN_WIDTH_USER_NAME);

            foreach (API_User user in users)
            {
                Console.Write(user.UserId.ToString().PadRight(COLUMN_WIDTH_USER_ID, ' '));
                Console.Write(user.Username.ToString().PadRight(COLUMN_WIDTH_USER_NAME, ' '));
                Console.WriteLine();
            }

            WriteDashes(COLUMN_WIDTH_USER_ID + COLUMN_WIDTH_USER_NAME);
        }

        public void PrintAllTransfers(List<API_Transfer> transfers)
        {
            WriteDashes(COLUMN_WIDTH_TRANSFER_AMOUNT + COLUMN_WIDTH_TRANSFER_ID + COLUMN_WIDTH_TRANSFER_OTHER);

            Console.WriteLine("Transfers");
            Console.Write("ID".PadRight(COLUMN_WIDTH_TRANSFER_ID, ' '));
            Console.Write("From/To".PadRight(COLUMN_WIDTH_TRANSFER_OTHER, ' '));
            Console.Write("Amount".PadRight(COLUMN_WIDTH_TRANSFER_AMOUNT, ' '));
            Console.WriteLine();

            WriteDashes(COLUMN_WIDTH_TRANSFER_AMOUNT + COLUMN_WIDTH_TRANSFER_ID + COLUMN_WIDTH_TRANSFER_OTHER);

            foreach(API_Transfer transfer in transfers)
            {
                string otherMessage;

                if(transfer.FromUserID == UserService.GetUserId()) otherMessage = $"To: {transfer.ToUserName}";
                else otherMessage = $"From: {transfer.FromUserName}";

                Console.Write(transfer.TransferID.ToString().PadRight(COLUMN_WIDTH_TRANSFER_ID, ' '));
                Console.Write(otherMessage.PadRight(COLUMN_WIDTH_TRANSFER_OTHER, ' '));
                Console.Write(transfer.TransferAmount.ToString("C2").PadRight(COLUMN_WIDTH_TRANSFER_AMOUNT, ' '));
                Console.WriteLine();
            }

            WriteDashes(COLUMN_WIDTH_TRANSFER_AMOUNT + COLUMN_WIDTH_TRANSFER_ID + COLUMN_WIDTH_TRANSFER_OTHER);
        }

        public void PrintPendingTransfers(List<API_Transfer> transfers)
        {
            WriteDashes(COLUMN_WIDTH_TRANSFER_AMOUNT + COLUMN_WIDTH_TRANSFER_ID + COLUMN_WIDTH_TRANSFER_OTHER);

            Console.WriteLine("Transfers");
            Console.Write("ID".PadRight(COLUMN_WIDTH_TRANSFER_ID, ' '));
            Console.Write("To".PadRight(COLUMN_WIDTH_TRANSFER_OTHER, ' '));
            Console.Write("Amount".PadRight(COLUMN_WIDTH_TRANSFER_AMOUNT, ' '));
            Console.WriteLine();

            WriteDashes(COLUMN_WIDTH_TRANSFER_AMOUNT + COLUMN_WIDTH_TRANSFER_ID + COLUMN_WIDTH_TRANSFER_OTHER);

            foreach (API_Transfer transfer in transfers)
            {
                bool isFromMe = transfer.FromUserID == UserService.GetUserId();
                if (transfer.TransferStatus == TransferStatus.Pending && isFromMe)
                {
                    Console.Write(transfer.TransferID.ToString().PadRight(COLUMN_WIDTH_TRANSFER_ID, ' '));
                    Console.Write(transfer.ToUserName.PadRight(COLUMN_WIDTH_TRANSFER_OTHER, ' '));
                    Console.Write(transfer.TransferAmount.ToString("C2").PadRight(COLUMN_WIDTH_TRANSFER_AMOUNT, ' '));
                    Console.WriteLine();
                }
            }

            WriteDashes(COLUMN_WIDTH_TRANSFER_AMOUNT + COLUMN_WIDTH_TRANSFER_ID + COLUMN_WIDTH_TRANSFER_OTHER);
        }


        public void PrintTransferDetails(API_Transfer transfer)
        {
            WriteDashes(COLUMN_WIDTH_TRANSFER_DETAILS);

            Console.WriteLine("Transfer Details");

            WriteDashes(COLUMN_WIDTH_TRANSFER_DETAILS);

            Console.WriteLine($"ID: {transfer.TransferID}");
            Console.WriteLine($"From: {transfer.FromUserName}");
            Console.WriteLine($"To: {transfer.ToUserName}");
            Console.WriteLine($"Type: {transfer.TransferType.ToString()}");
            Console.WriteLine($"Status: {transfer.TransferStatus.ToString()}");
            Console.WriteLine($"Amount: {transfer.TransferAmount.ToString("C2")}");

            WriteDashes(COLUMN_WIDTH_TRANSFER_DETAILS);
        }

        private void WriteDashes(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Console.Write('-');
            }

            Console.WriteLine();
        }

        public int PromptForApproval()
        {
            Console.WriteLine("1: Approve");
            Console.WriteLine("2: Reject");
            Console.WriteLine("0: Don't approve or reject");
            bool optionValid = false;
            int approvalOption = 0;
            while (!optionValid)
            {
                if (!int.TryParse(Console.ReadLine(), out approvalOption))
                {
                    Console.WriteLine("Invalid input. Only input a number.");
                }
                if(approvalOption == 0 || approvalOption == 1 || approvalOption == 2)
                {
                    optionValid = true;
                }
                else
                {
                    Console.WriteLine("Please Enter Number 0, 1, or 2");
                }

            }
            return approvalOption;
        }
    }
}
