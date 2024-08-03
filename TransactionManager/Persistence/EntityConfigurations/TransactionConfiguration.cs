using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionManager.Entities;

namespace TransactionManager.Persistence.EntityConfigurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(e => e.TransactionId)
            .HasName("PRIMARY");

        builder.Property(e => e.TransactionDateUtc)
            .HasColumnType("timestamp without time zone");

        builder.Property(e => e.Amount)
            .HasColumnType("money");
    }
}