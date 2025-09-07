namespace SimpleEventAccountingMobile.Dtos
{
    public class FullClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string? Comment { get; set; }

        // Wallet properties
        public bool Subscription { get; set; }
        public int TrainingCount { get; set; }
        public int TrainingSkip { get; set; }
        public int TrainingFree { get; set; }
        public int CashAmount { get; set; }
    }
}
