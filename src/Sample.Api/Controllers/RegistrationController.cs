namespace Sample.Api.Controllers;

using Components;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
public class RegistrationController :
    ControllerBase
{
    private readonly IMediator mediator;

    public RegistrationController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] RegistrationModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var requestClient = mediator.CreateRequestClient<RegistrationSubmitRequest>();
            var response = await requestClient.GetResponse<RegistrationSubmitResponse>(new RegistrationSubmitRequest(model));
            return Ok(response.Message);
        }
        catch (DuplicateRegistrationException)
        {
            return Conflict(new
            {
                model.MemberId,
                model.EventId,
            });
        }
    }
}