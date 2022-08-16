namespace Sample.Components;

using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachines;


public class RegistrationDbContext :
    SagaDbContext
{
    public RegistrationDbContext(DbContextOptions<RegistrationDbContext> options, IPublishEndpoint publishEndpoint)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new RegistrationStateMap(); }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // FROM: Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.OrderingContext
        // Dispatch Domain Events collection. 
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        //publishEndpoint.DispatchDomainEventsAsync(this, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        MapRegistration(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }

    static void MapRegistration(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<Registration> registration = modelBuilder.Entity<Registration>();

        registration.Property(x => x.Id);
        registration.HasKey(x => x.Id);

        registration.Property(x => x.RegistrationId);
        registration.Property(x => x.RegistrationDate);
        registration.Property(x => x.MemberId).HasMaxLength(64);
        registration.Property(x => x.EventId).HasMaxLength(64);
        registration.Property(x => x.Payment);

        registration.HasIndex(x => new
        {
            x.MemberId,
            x.EventId
        }).IsUnique();
    }
}