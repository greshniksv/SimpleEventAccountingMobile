namespace SimpleEventAccountingMobile.Dtos
{
    public class ClientRelationsDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public DateTime Birthday { get; set; }

        public string? Comment { get; set; }

        public decimal? CashAmount { get; set; }

        public bool? TrainingIsSubscription { get; set; }

        public decimal? TrainingCount { get; set; }

        public decimal? TrainingSkip { get; set; }
    }
}
