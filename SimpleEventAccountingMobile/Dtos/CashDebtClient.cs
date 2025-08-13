using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Dtos
{
    public class CashDebtClient
    {
        public CashDebtClient(Client client, decimal cash)
        {
            Client = client;
            Cash = cash;
        }

        public Client Client { get; set; }

        public decimal Cash { get; set; }
    }
}
