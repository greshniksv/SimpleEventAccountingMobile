using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
    public class ClientGroup : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<ClientGroupBinding>? ClientGroupBindings { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<ClientGroup>());

        private static void Configure(EntityTypeBuilder<ClientGroup> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasMany(e => e.ClientGroupBindings)
                .WithOne(e => e.ClientGroup)
                .HasForeignKey(e => e.ClientGroupId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
