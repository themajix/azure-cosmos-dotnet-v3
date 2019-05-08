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
    [Route("api/GamesQuery")]
    [ApiController]
    public class GamesQueryController : ControllerBase
    {
        private readonly CosmosContainer cosmosContainer;
        private const int MaxConcurrency = 1;
        private const int MaxPageSize = 10;
        private const string SessionHeader = "CosmosSession";
        private const string ContinuationHeader = "CosmosSession";

        // GET: api/GamesQuery/day/userid
        // Return the list of games user played in a day
        [HttpGet("{day}/{userid}", Name = "Get")]
        public async Task<HttpResponseMessage> Query(
            string day, 
            string userId,
            CancellationToken cancellationToken)
        {
            QueryRequestOptions queryRequestOptions = new QueryRequestOptions();
            if (Request.Headers.TryGetValue(GamesQueryController.SessionHeader, out StringValues session))
            {
                queryRequestOptions.SessionToken = session.SingleOrDefault(); 
            }

            if (Request.Headers.TryGetValue(GamesQueryController.ContinuationHeader, out StringValues continuation))
            {
                queryRequestOptions.RequestContinuation = continuation.SingleOrDefault(); 
            }

            // Read a single query page from Azure Cosmos DB as stream
            // LINQ to rescue
            var userGamesQuery = new CosmosSqlQueryDefinition("SELECT * FROM Items i WHERE i.Day = @Day AND ARRAY_CONTAINS(i.Players, @UserId)")
                .UseParameter("Day", day)
                .UseParameter("UserID", userId)
                .ToString();

            var queryIterator = this.cosmosContainer.CreateItemQueryAsStream(
                        sqlQueryText: userGamesQuery,
                        maxConcurrency: GamesQueryController.MaxConcurrency,
                        maxItemCount: GamesQueryController.MaxPageSize,
                        requestOptions: queryRequestOptions);
            CosmosResponseMessage queryResponse = await queryIterator.FetchNextSetAsync(cancellationToken);

            // Pass stream directly to response object, without deserializing
            var result = new HttpResponseMessage();
            result.StatusCode = queryResponse.StatusCode;
            result.Content = new StreamContent(queryResponse.Content);

            // Add session & continuations back 
            result.Headers.Add(GamesQueryController.SessionHeader, queryResponse.Headers.Session);
            result.Headers.Add(GamesQueryController.ContinuationHeader, queryResponse.Headers.Continuation);

            return result;
        }
   }
}