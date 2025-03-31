using BoardManager;
using BoardManager.Exceptions;
using Conway_s_Game_of_Life.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Conway_s_Game_of_Life.Controllers
{
    [ApiController]
    [Route($"boards/{{boardId}}/generations")]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(500)]
    [ProducesResponseType(403)]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]    
    public class GenerationsController :ControllerBase
    {
        private readonly ILogger<BoardsController> _logger;
        private readonly IBoardManager _boardManager;
        public GenerationsController(IBoardManager boardManager, ILogger<BoardsController> logger)
        {
            _boardManager = boardManager;
            _logger = logger;
        }

        [RequiredScope(JwtConfiguration.ReadScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Board), 200)]
        [ProducesResponseType(404)]
        [HttpGet($"current")]
        public async Task<IActionResult> GetCurrentGeneration([FromRoute] string boardId)
        {
            try
            {
                var result = await _boardManager.GetCurrentGeneration(boardId);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (BusineesException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [RequiredScope(JwtConfiguration.WriteScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Board), 201)]
        [HttpPost($"final")]
        public async Task<IActionResult> CalculateFinalGeneration([FromRoute] string boardId)
        {
            try
            {
                var result = await _boardManager.CalculateFinalGeneration(boardId);
                return Ok(result);
            }
            catch (BusineesException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [RequiredScope(JwtConfiguration.WriteScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Board), 201)]
        [HttpPost()]
        public async Task<IActionResult> CreateGeneration([FromRoute] string boardId)
        {
            try
            {
                var result = await _boardManager.AddGeneration(boardId);
                return CreatedAtAction(nameof(GetGenerationByNumber), new { boardId = result.Board.Id, generationNumber = result.Number }, result);
            }
            catch (BusineesException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [RequiredScope(JwtConfiguration.ReadScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Generation), 200)]
        [ProducesResponseType(404)]
        [HttpGet($"{{generationNumber}}")]
        public async Task<IActionResult> GetGenerationByNumber([FromRoute] string boardId, int generationNumber)
        {
            var generation = await _boardManager.GetGenerationAsync(boardId, generationNumber);
            if (generation == null)
                return NotFound();
            return Ok(generation);
        }

        [RequiredScope(JwtConfiguration.WriteScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Generation), 201)]
        [HttpPost($"{{generationNumber}}")]
        public async Task<IActionResult> CalculateGenerationNumber([FromRoute] string boardId, int generationNumber)
        {
            try
            {
                var result = await _boardManager.CalculateGenerationNumber(boardId, generationNumber);
                return CreatedAtAction(nameof(GetGenerationByNumber), new { boardId = result.Board.Id, generationNumber = result.Number }, result);
            }
            catch (BusineesException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
