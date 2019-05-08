//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Scenarios.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Primitives;

    [Produces("application/json")]
    [Route("api/gamestyped")]
    [ApiController]
    public class GamesTypedController : ControllerBase
    {
        private readonly CosmosContainer cosmosContainer;
        private const string SessionHeader = "CosmosSession";

        // POST: api/games/day (create new game)
        [HttpPost("{day}")]
        public async Task<JsonResult> Post(
            string day,
            [FromBody]TwoPersonGame newgame,
            CancellationToken cancellationToken)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions();
            if (Request.Headers.TryGetValue(GamesTypedController.SessionHeader, out StringValues session))
            {
                itemRequestOptions.SessionToken = session.SingleOrDefault(); // Assume max one
            }

            try
            {
                ItemResponse<TwoPersonGame> gameCreateResponse = await cosmosContainer
                    .CreateItemAsync<TwoPersonGame>(newgame, itemRequestOptions, cancellationToken);

                // Add session token back 
                Response.Headers.Add(GamesTypedController.SessionHeader, gameCreateResponse.Headers.Session);
                return new JsonResult(gameCreateResponse.Resource);
            }
            catch(CosmosException ex)
            {
                var result = new JsonResult(ex.Message);
                result.StatusCode = (int)ex.StatusCode;

                return result;
            }
        }
    }
}