using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly APIService apiService = new APIService();

        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                            apiService.SetToken(user.Token);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    decimal? balance = apiService.GetAccountBalance();

                    if (balance != null)
                    {
                        decimal balanceAmount = (decimal)balance;
                        Console.WriteLine($"Here's your account balance: {balanceAmount.ToString("C2")}");
                    }
                }
                else if (menuSelection == 2)
                {
                    List<API_Transfer> transfers = apiService.GetTransfers();
                    if (transfers == null) continue;

                    consoleService.PrintAllTransfers(transfers);

                    int chosenID = consoleService.PromptForTransferID("get details");

                    Func<API_Transfer, bool> search = t => t.TransferID == chosenID;

                    if (transfers.Any(search))
                    {
                        API_Transfer transfer = transfers.Single(search);

                        consoleService.PrintTransferDetails(transfer);
                    }
                    else if (chosenID != 0)
                    {
                        Console.WriteLine("The transfer ID you requested does not exist.");
                    }
                }
                else if (menuSelection == 3)
                {
                    List<API_Transfer> transfers = apiService.GetTransfers();
                    if (transfers == null) continue;

                    consoleService.PrintPendingTransfers(transfers);
                    int chosenID = consoleService.PromptForTransferID("approve or reject request");
                    int approvalOption;

                    List<API_Transfer> pendingTransfers = transfers.Where(t => t.TransferStatus == TransferStatus.Pending).ToList();
                    Func<API_Transfer, bool> search = t => t.TransferID == chosenID;

                    if (pendingTransfers.Any(search))
                    {
                        API_Transfer pendingTransfer = pendingTransfers.Single(search);
                        approvalOption = consoleService.PromptForApproval();
                        apiService.UpdateTransferStatus(pendingTransfer, approvalOption);

                    }
                    else if (chosenID != 0)
                    {
                        Console.WriteLine("The transfer ID you requested does not exist.");
                    }

                }
                else if (menuSelection == 4)
                {
                    List<API_User> users = apiService.GetOtherUsers();
                    if (users == null) continue;

                    consoleService.PrintUsers(users);
                    TransferType transferType = TransferType.Send;
                    API_Transfer transfer = consoleService.PromptForTransferRequest(transferType);

                    if (!users.Any(u => u.UserId == transfer.ToUserID))
                    {
                        Console.WriteLine("The user ID you requested does not exist.");
                        continue;
                    }

                    transfer.ToUserName = users.First(u => u.UserId == transfer.ToUserID)?.Username;
                    apiService.SubmitTransferRequest(transfer);

                }
                else if (menuSelection == 5)
                {
                    List<API_User> users = apiService.GetOtherUsers();
                    if (users == null) continue;

                    consoleService.PrintUsers(users);
                    TransferType transferType = TransferType.Request;
                    API_Transfer transfer = consoleService.PromptForTransferRequest(transferType);

                    if (!users.Any(u => u.UserId == transfer.FromUserID))
                    {
                        Console.WriteLine("The user ID you requested does not exist.");
                        continue;
                    }

                    transfer.FromUserName = users.First(u => u.UserId == transfer.FromUserID).Username;
                    apiService.SubmitTransferRequest(transfer);
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
