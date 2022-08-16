using MassTransit;
using Sample.Components;

namespace Sample.Api.Controllers;

internal class RegistrationSubmitHandler : IConsumer<RegistrationSubmitRequest>
{
    private readonly IRegistrationService _registrationService;

    public RegistrationSubmitHandler(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    public async Task Consume(ConsumeContext<RegistrationSubmitRequest> context)
    {
        var registration = await _registrationService.SubmitRegistration(context.Message.RegistrationModel.EventId, context.Message.RegistrationModel.MemberId, context.Message.RegistrationModel.Payment);

        await context.RespondAsync(new RegistrationSubmitResponse(registration.Id, registration.RegistrationDate, registration.MemberId, registration.EventId));
    }
}

public record RegistrationSubmitRequest(RegistrationModel RegistrationModel);

public record RegistrationSubmitResponse(int RegistrationId, DateTime RegistrationDate, string MemberId, string EventId);