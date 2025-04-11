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
        public IActionResult CalculateDamageAndGenerateGraph([FromBody] FullDamageRequest request)
        {
            if (request == null ||
                request.AttackNameValuePairs == null || request.AttackNameValuePairs.Count == 0 ||
                request.SelfDeckInfo == null || request.OppDeckInfo == null || request.Opp2ndDeckInfo == null)
            {
                return BadRequest("Invalid request data. All deck information and AttackNameValuePairs must be provided.");
            }

            try
            {
                // Initialize the decks based on the input
                _attackService.InitializeDecks(request.OppDeckInfo, request.SelfDeckInfo, request.Opp2ndDeckInfo);

                // Create an AttackRequest wrapper from the DTO's attack data
                var attackRequest = new AttackRequest { AttackNameValuePairs = request.AttackNameValuePairs };

                // Generate the damage graph image
                byte[] graphImage = _attackService.CalculateDamageAndGenerateGraph(attackRequest);

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



        /// <summary> Deprecated. 
        /// Initializes the decks based on the provided deck information.
        /// </summary>
        /// <param name="deckRequest">The request containing deck initialization data.</param>
        /// <returns>Ok if decks are successfully initialized, BadRequest if there are issues.</returns>
        //[HttpPost("initialize-decks")]
        //public IActionResult InitializeDecks([FromBody] InitializeDeckRequest deckRequest)
        //{
        //    //Console.WriteLine("Controller Called");
        //    // Validate the deck initialization request
        //    if (deckRequest == null || deckRequest.OppDeckInfo == null || deckRequest.SelfDeckInfo == null || deckRequest.Opp2ndDeckInfo == null)
        //    {
        //        return BadRequest("Invalid deck data. All deck information must be provided.");
        //    }

        //    try
        //    {
        //        // Initialize decks based on the provided deck information
        //        _attackService.InitializeDecks(deckRequest.OppDeckInfo, deckRequest.SelfDeckInfo, deckRequest.Opp2ndDeckInfo);

        //        // Return a success response
        //        return Ok("Decks initialized successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error and return a generic error response
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

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
