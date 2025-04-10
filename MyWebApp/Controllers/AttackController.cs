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
            // Validate the request data
            if (request == null || request.AttackNameValuePairs == null || request.AttackNameValuePairs.Count == 0)
            {
                return BadRequest("Invalid request data. AttackNameValuePairs cannot be null or empty.");
            }

            try
            {
                // Call the service method to calculate damage and generate the graph
                byte[] graphImage = _attackService.CalculateDamageAndGenerateGraph(request);

                // Return the generated graph as a PNG image
                return File(graphImage, "image/png");
            }
            catch (Exception ex)
            {
                // Log the error and return a generic error response
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
        public ActionResult<string[]> GetBasicMethods()
        {
            var damageMethods = typeof(Damages)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => m.Name)
                .Distinct()
                .ToArray();

            return Ok(damageMethods);
        }

        [HttpGet("finisher-methods")]
        public ActionResult<string[]> GetFinisherMethods()
        {
            var finisherMethods = typeof(Finishers)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => m.Name)
                .Distinct()
                .ToArray();

            return Ok(finisherMethods);
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
