using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Dtos
{
    public class TrainingDebtClient
    {
        public TrainingDebtClient(Client client, decimal count)
        {
            Client = client;
            Count = count;
        }

        public Client Client { get; set; }

        public decimal Count { get; set; }
    }
}
