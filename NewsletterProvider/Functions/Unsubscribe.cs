using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NewsletterProvider.Functions;

public class Unsubscribe(ILogger<Unsubscribe> logger)
{
    private readonly ILogger<Unsubscribe> _logger = logger;
    private readonly DataContext _context;

    [Function("Unsubscribe")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        if (!string.IsNullOrEmpty(body))
        {
            var subscribeEntity = JsonConvert.DeserializeObject<SubscribeEntity>(body);
            if (subscribeEntity != null)
            {
                var existingSubscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == subscribeEntity.Email);
                if (existingSubscriber != null)
                {
                    _context.Remove(existingSubscriber);
                    await _context.SaveChangesAsync();
                    return new OkObjectResult(new { Status = 200, Message = "Subscriber is removed." });
                }
            }
        }
        return new BadRequestObjectResult(new { Status = 400, Message = "Unable to unsubscribe." });
    }
}
