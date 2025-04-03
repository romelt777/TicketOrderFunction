using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace TicketOrderFunction
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run([QueueTrigger("tickethubqueue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            //message.MessageText is the json in the queue
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            string messageJson = message.MessageText;

            //deserialize the message into a contact object
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var ticketOrder = JsonSerializer.Deserialize<TicketOrder>(messageJson, options);

            if (ticketOrder == null)
            {
                _logger.LogError("Unable to deserialize the message into a ticketOrder object");
                return;
            }

            _logger.LogInformation($"ConcertId: {ticketOrder.ConcertId} Name: {ticketOrder.Name} Quantity: {ticketOrder.Quantity}");

            //add the ticket order to the database

            // get connection string from app settings
            string? connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("SQL connection string is not set in the environment variables.");
            }


            //execute the insert
            //create a connection to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //open the connection
                await conn.OpenAsync(); // Note the ASYNC

                //create a query
                var query = "INSERT INTO TicketOrders (ConcertId,Email,Name,Phone,Quantity,CreditCard,ExpirationDate,SecurityCode,Address,City,Province,PostalCode,Country) VALUES(@ConcertId,@Email,@Name,@Phone,@Quantity,@CreditCard,@ExpirationDate,@SecurityCode,@Address,@City,@Province,@PostalCode,@Country);";



                //create a command
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@ConcertId", ticketOrder.ConcertId);
                    cmd.Parameters.AddWithValue("@Email", ticketOrder.Email);
                    cmd.Parameters.AddWithValue("@Name", ticketOrder.Name);
                    cmd.Parameters.AddWithValue("@Phone", ticketOrder.Phone);
                    cmd.Parameters.AddWithValue("@Quantity", ticketOrder.Quantity);
                    cmd.Parameters.AddWithValue("@CreditCard", ticketOrder.CreditCard);
                    cmd.Parameters.AddWithValue("@ExpirationDate", ticketOrder.ExpirationDate);
                    cmd.Parameters.AddWithValue("@SecurityCode", ticketOrder.SecurityCode);
                    cmd.Parameters.AddWithValue("@Address", ticketOrder.Address);
                    cmd.Parameters.AddWithValue("@City", ticketOrder.City);
                    cmd.Parameters.AddWithValue("@Province", ticketOrder.Province);
                    cmd.Parameters.AddWithValue("@PostalCode", ticketOrder.PostalCode);
                    cmd.Parameters.AddWithValue("@Country", ticketOrder.Country);


                    //execute the command
                    await cmd.ExecuteNonQueryAsync();
                }

                //using automatically closes connection
            }
        }
    }
}
