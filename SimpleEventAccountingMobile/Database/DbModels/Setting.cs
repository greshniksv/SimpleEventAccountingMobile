using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
    public class Setting : BaseEntity
    {
        public string Key { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<Setting>());

        private static void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
