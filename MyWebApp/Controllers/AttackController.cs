using Microsoft.AspNetCore.Mvc;
using MyWebApp;
using MyWebApp.Models;
using MyWebApp.Services;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MyWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttackController : ControllerBase
    {
        private readonly AttackService _attackService;

        // Injecting AttackService into the constructor for better testability and flexibility
        public AttackController(AttackService attackService)
        {
            _attackService = attackService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("This is a GET request to /api/attack");
        }


        /// <summary>
        /// Calculates damage based on the provided attack parameters and generates the graph.
        /// </summary>
        /// <param name="request">The attack request containing the attack name and value pairs.</param>
        /// <returns>The generated graph image as a PNG file.</returns>
        [HttpPost("calculate-damage")]
        public IActionResult CalculateDamageAndGenerateGraph([FromBody] AttackRequest request)
        {
            Console.WriteLine("🔥 [POST] /calculate-damage called");
            Console.Out.Flush();

            if (request == null || request.AttackNameValuePairs == null || request.AttackNameValuePairs.Count == 0)
            {
                Console.WriteLine("⚠️ Invalid request: AttackNameValuePairs is null or empty");
                Console.Out.Flush();
                return BadRequest("Invalid request data. AttackNameValuePairs cannot be null or empty.");
            }

            try
            {
                Console.WriteLine($"🔢 Simulating {request.AttackNameValuePairs.Count} attacks...");
                Console.Out.Flush();

                byte[] graphImage = _attackService.CalculateDamageAndGenerateGraph(request);

                Console.WriteLine("✅ Graph image generated successfully");
                Console.Out.Flush();

                return File(graphImage, "image/png");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Exception in /calculate-damage:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.Out.Flush();

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Initializes the decks based on the provided deck information.
        /// </summary>
        /// <param name="deckRequest">The request containing deck initialization data.</param>
        /// <returns>Ok if decks are successfully initialized, BadRequest if there are issues.</returns>
        [HttpPost("initialize-decks")]
        public IActionResult InitializeDecks([FromBody] InitializeDeckRequest deckRequest)
        {
            //Console.WriteLine("Controller Called");
            // Validate the deck initialization request
            if (deckRequest == null || deckRequest.OppDeckInfo == null || deckRequest.SelfDeckInfo == null || deckRequest.Opp2ndDeckInfo == null)
            {
                return BadRequest("Invalid deck data. All deck information must be provided.");
            }

            try
            {
                // Initialize decks based on the provided deck information
                _attackService.InitializeDecks(deckRequest.OppDeckInfo, deckRequest.SelfDeckInfo, deckRequest.Opp2ndDeckInfo);

                // Return a success response
                return Ok("Decks initialized successfully.");
            }
            catch (Exception ex)
            {
                // Log the error and return a generic error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("basic-methods")]
        public ActionResult<object[]> GetBasicMethods()
        {
            var methodData = typeof(Damages)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => new
                {
                    Method = m.Name,
                    Parameters = m.GetParameters()
                                 .Select(p => $"{p.ParameterType.Name} {p.Name}")
                                 .ToArray()
                })
                .ToArray();

            return Ok(methodData);
        }


        [HttpGet("finisher-methods")]
        public ActionResult<object[]> GetFinisherMethods()
        {
            var methodData = typeof(Finishers)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => new
                {
                    Method = m.Name,
                    Parameters = m.GetParameters()
                                 .Select(p => $"{p.ParameterType.Name} {p.Name}")
                                 .ToArray()
                })
                .ToArray();

            return Ok(methodData);
        }


    }

    // Request model to initialize the decks
    public class InitializeDeckRequest
    {
        public DeckInfo OppDeckInfo { get; set; }
        public DeckInfo SelfDeckInfo { get; set; }
        public DeckInfo Opp2ndDeckInfo { get; set; }
    }

}
