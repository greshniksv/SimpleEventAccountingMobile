namespace SimpleEventAccountingMobile.Dtos
{
    public class ExportData
    {
        public List<CashWalletDto> CashWallets { get; set; } = new();
        public List<CashWalletHistoryDto> CashWalletHistory { get; set; } = new();
        public List<ClientDto> Clients { get; set; } = new();
        public List<EventDto> Events { get; set; } = new();
        public List<EventClientDto> EventClients { get; set; } = new();
        public List<TrainingDto> Trainings { get; set; } = new();
        public List<TrainingClientDto> TrainingClients { get; set; } = new();
        public List<TrainingWalletDto> TrainingWallets { get; set; } = new();
        public List<TrainingWalletHistoryDto> TrainingWalletHistory { get; set; } = new();
        public List<TrainingChangeSetDto> TrainingChangeSets { get; set; } = new();
        public List<EventChangeSetDto> EventChangeSets { get; set; } = new();
    }

    public class EventChangeSetDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid EventId { get; set; }
        public decimal Cash { get; set; }
    }

    public class TrainingChangeSetDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid TrainingId { get; set; }
        public decimal? Count { get; set; }
        public decimal? Skip { get; set; }
        public decimal? Free { get; set; }
        public bool? Subscription { get; set; }
    }

    public class CashWalletDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public decimal Cash { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class CashWalletHistoryDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; }
        public Guid? EventId { get; set; }
        public decimal Cash { get; set; }
        public string? Comment { get; set; }
    }

    public class ClientDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime Birthday { get; set; }
        public string? Comment { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class EventDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class EventClientDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid ClientId { get; set; }
    }

    public class TrainingDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class TrainingClientDto
    {
        public Guid Id { get; set; }
        public Guid TrainingId { get; set; }
        public Guid ClientId { get; set; }
    }

    public class TrainingWalletDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public decimal Count { get; set; }
        public decimal Skip { get; set; }
        public decimal Free { get; set; }
        public bool Subscription { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class TrainingWalletHistoryDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid? TrainingId { get; set; }
        public DateTime Date { get; set; }
        public decimal Count { get; set; }
        public decimal Skip { get; set; }
        public decimal Free { get; set; }
        public bool Subscription { get; set; }
        public string? Comment { get; set; }
    }
}
