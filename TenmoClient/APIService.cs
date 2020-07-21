using System;
using System.Collections.Generic;
using System.Net;

using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient
{
    public class APIService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public void SetToken(string token)
        {
            client.Authenticator = new JwtAuthenticator(token);
        }

        public decimal? GetAccountBalance()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account");
            IRestResponse<decimal> response = client.Get<decimal>(request);

            if (handleErrors(response, "getting your account balance", "The requested account wasn't found.", ""))
            {
                return response.Data;
            }
            else { return null; }
        }

        public List<API_User> GetUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user/all");
            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);

            if (handleErrors(response,"getting the user list", "", ""))
            {
                return response.Data;
            }
            else { return null; }        
        }

        public List<API_User> GetOtherUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user/others");
            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);

            if (handleErrors(response, "getting the user list", "", ""))
            {
                return response.Data;
            }
            else { return null; }
        }

        public bool SubmitTransferRequest(API_Transfer transfer)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transfer/request");
            request.AddJsonBody(transfer);
            IRestResponse<API_Transfer> response = client.Post<API_Transfer>(request);

            return handleErrors(response, "making your transfer", "", "");
        }

        public List<API_Transfer> GetTransfers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transfer");
            IRestResponse<List<API_Transfer>> response = client.Get<List<API_Transfer>>(request);

            if (handleErrors(response,"getting the transfer list", "", ""))
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public bool UpdateTransferStatus(API_Transfer transfer, int approvalOption)
        {
            if(approvalOption == 1)
            {
                transfer.TransferStatus = TransferStatus.Approved;
            }
            else if(approvalOption == 2)
            {
                transfer.TransferStatus = TransferStatus.Rejected;
            }
            else { return false; }
            RestRequest request = new RestRequest(API_BASE_URL + "transfer");
            request.AddJsonBody(transfer);
            IRestResponse<API_Transfer> response = client.Put<API_Transfer>(request);

            return handleErrors(response, "making your transfer", "", "You don't have enough money to approve this request.");
        }

        private bool handleErrors(IRestResponse response, string action, string notFoundMessage, string badRequestMessage)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            }
            else if (!response.IsSuccessful)
            {
                if (notFoundMessage != "" && response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(notFoundMessage);
                }
                else if (badRequestMessage != "" && response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Console.WriteLine(badRequestMessage);
                }
                else
                {
                    Console.WriteLine($"There was an error {action}: Status Code {response.StatusCode}");
                }

                return false;
            }

            return true;
        }


    }
}
