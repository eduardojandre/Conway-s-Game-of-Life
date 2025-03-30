using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
using Conway_s_Game_of_Life.Utility;
using BoardManager;
using BoardManager.Exceptions;

namespace Conway_s_Game_of_Life.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(500)]
    [ProducesResponseType(403)]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    public class BoardsController : ControllerBase
    {
        private readonly ILogger<BoardsController> _logger;
        private readonly IBoardManager _boardManager;
        public BoardsController(IBoardManager boardManager,ILogger<BoardsController> logger)
        {
            _boardManager = boardManager;
            _logger = logger;
        }

        [HttpPost]
        [RequiredScope(JwtConfiguration.WriteScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Board), 201)]
        public async Task<IActionResult> Post(BoardManager.DTOs.NewBoardRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            var result = await _boardManager.InsertBoardAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [RequiredScope(JwtConfiguration.ReadScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Board), 200)]
        [ProducesResponseType(204)]
        [HttpGet($"{{id}}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var board = await _boardManager.GetBoardAsync(id);
            if (board == null)
                return NoContent();
            return Ok(board);
        }

        [RequiredScope(JwtConfiguration.ReadScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Board), 200)]
        [ProducesResponseType(204)]
        [HttpPost($"{{boardId}}/generations")]
        public async Task<IActionResult> CreateGeneration([FromRoute] string boardId)
        {
            try
            {
                var result = await _boardManager.AddGeneration(boardId);
                return CreatedAtAction(nameof(GetGenerationByNumber), new { boardId = result.Board.Id, generationNumber = result.Number }, result);
            }
            catch (BusineesException ex) { 
                return BadRequest(ex.Message);
            }            
        }
        [RequiredScope(JwtConfiguration.ReadScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Generation), 200)]
        [ProducesResponseType(204)]
        [HttpGet($"{{boardId}}/generations/{{generationNumber}}")]
        public async Task<IActionResult> GetGenerationByNumber([FromRoute] string boardId, int generationNumber)
        {
            var generation = await _boardManager.GetGenerationAsync(boardId, generationNumber);
            if (generation == null)
                return NoContent();
            return Ok(generation);
        }
    }
}
