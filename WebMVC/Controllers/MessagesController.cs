using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        readonly ProducerConfig config;
        public MessagesController()
        {
            config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                ClientId = "Producer",
                Acks = Acks.All
            };
        }

        // POST api/<MessagesController>
        [HttpPost]
        public IActionResult Post([FromForm] string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                    return BadRequest();

                using var producer = new ProducerBuilder<Null, string>(config).Build();

                producer
                     .ProduceAsync("Topic", new Message<Null, string> { Value = value })
                     .ContinueWith((task) =>
                     {
                         Console.WriteLine("Status: {0}, Message: {1}, Offset: {2}", task.Result.Status, task.Result.Value, task.Result.Offset.Value);
                     })
                     .Wait();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
