using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
using Conway_s_Game_of_Life.Utility;
using BoardManager;
using BoardManager.Exceptions;

namespace Conway_s_Game_of_Life.Controllers
{
    [ApiController]
    [Route("boards")]
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

        [RequiredScope(JwtConfiguration.ReadScope)]
        [ProducesResponseType(typeof(IEnumerable<BoardManager.DTOs.Board>), 200)]
        [ProducesResponseType(204)]
        [HttpGet()]
        public async Task<IActionResult> GetAll([FromQuery] int pageSize = 100, int pageNumber = 1)
        {
            var boardsCount = await _boardManager.GetBoardsCountAsync();
            var boards = await _boardManager.GetAllBoardsAsync(pageSize, pageNumber);
            if (boards == null)
                return NoContent();
            var result = Ok(boards);

            Response.Headers.Add("X-Total-Count", boardsCount.ToString());
            return Ok(boards);
        }

        [RequiredScope(JwtConfiguration.ReadScope)]
        [ProducesResponseType(typeof(BoardManager.DTOs.Board), 200)]
        [ProducesResponseType(404)]
        [HttpGet($"{{id}}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var board = await _boardManager.GetBoardAsync(id);
            if (board == null)
                return NotFound();
            return Ok(board);
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
    }
}
