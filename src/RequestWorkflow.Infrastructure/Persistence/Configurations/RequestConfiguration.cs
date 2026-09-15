using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RequestWorkflow.Infrastructure.Identity;
using RequestEntity = RequestWorkflow.Domain.Requests.Request;

namespace RequestWorkflow.Infrastructure.Persistence.Configurations;

public sealed class RequestConfiguration
    : IEntityTypeConfiguration<RequestEntity>
{
    public void Configure(EntityTypeBuilder<RequestEntity> builder)
    {
        builder.ToTable("Requests");

        builder.HasKey(request => request.Id);

        builder.Property(request => request.Id)
            .ValueGeneratedNever();

        builder.Property(request => request.Title)
            .IsRequired();

        builder.Property(request => request.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(request => request.Description);

        builder.Property(request => request.Status)
            .IsRequired();

        builder.Property(request => request.AssignedRole)
            .IsRequired();

        builder.Property(request => request.CreatedAt)
            .IsRequired();

        builder.Property(request => request.Metadata)
            .HasColumnType("jsonb");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(request => request.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
