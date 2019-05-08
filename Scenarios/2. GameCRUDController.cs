//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Scenarios.Controllers
{
    using System;
	using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Primitives;

    [Produces("application/json")]
    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly CosmosContainer cosmosContainer;
        private const string SessionHeader = "CosmosSession";

        // POST: api/games/day (create new game)
        [HttpPost("{day}")]
        public async Task<HttpResponseMessage> Post(
            string day,
            CancellationToken cancellationToken)
        {
            using (Request.Body)
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions();
                if (Request.Headers.TryGetValue(GamesController.SessionHeader, out StringValues session))
                {
                    itemRequestOptions.SessionToken = session.SingleOrDefault(); // Assume max one
                }

                CosmosResponseMessage gameCreateResponse = await cosmosContainer
                    .CreateItemStreamAsync(day, Request.Body, itemRequestOptions, cancellationToken);


                var result = new HttpResponseMessage();
                result.StatusCode = gameCreateResponse.StatusCode;
                result.Content = new StreamContent(gameCreateResponse.Content);

                // Add session token back 
                if (gameCreateResponse.Headers.TryGetValue(GamesController.SessionHeader, out string sessionToken))
                {
                    result.Headers.Add(GamesController.SessionHeader, sessionToken);
                }

                return result;
            }
        }

        // GET: api/games/day/id (Read game)
        [HttpGet("{day}/{id}")]
        public async Task<HttpResponseMessage> Get(
            string day, 
            string gameid,
            CancellationToken cancellationToken)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions();
            if (Request.Headers.TryGetValue(GamesController.SessionHeader, out StringValues session))
            {
                itemRequestOptions.SessionToken = session.SingleOrDefault(); // Assume max one
            }

            CosmosResponseMessage gameReadResponse = await cosmosContainer
                .ReadItemStreamAsync(day, gameid, itemRequestOptions, cancellationToken);

            var result = new HttpResponseMessage();
            result.StatusCode = gameReadResponse.StatusCode;
            result.Content = new StreamContent(gameReadResponse.Content);

            // Add session token back 
            if (gameReadResponse.Headers.TryGetValue(GamesController.SessionHeader, out string sessionToken))
            {
                result.Headers.Add(GamesController.SessionHeader, sessionToken);
            }

            return result;
        }
    }
}