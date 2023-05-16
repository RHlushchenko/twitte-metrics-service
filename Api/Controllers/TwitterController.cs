using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Twitter;

namespace Api.Controllers
{
    /// <summary>
    /// The Twitter API controller.
    /// </summary>
    [ApiController]
    [Route("api/twitter")]
    public class TwitterController : Controller
    {
        private ITwitterService service;

        public TwitterController(ITwitterService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Metrics
        /// </summary>
        /// <remarks>
        /// Returns metrics of the Twitter storage.
        /// </remarks>
        /// <returns>Metrics.</returns>
        [HttpGet("metrics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TwitterMetrics> Metrics()
        {
            return base.Ok(this.service.GetMetrics());
        }
    }
}
