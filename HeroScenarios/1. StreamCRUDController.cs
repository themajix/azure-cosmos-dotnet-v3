//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CosmosWebAPI.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/TodoItem")]
    // GET: api/TodoItem/userId/continuationToken
    [HttpGet("{userId}/{continuationToken}", Name = "Get")]
    public async Task<HttpResponseMessage> Query(string userId, string continuationToken)
    {
        var result = new HttpResponseMessage();

        // Read a single query page from Azure Cosmos DB as stream
        var query = new CosmosSqlQueryDefinition("SELECT * FROM Items i WHERE i.UserID = @UserId AND i.IsCompleted = 1")
            .UseParameter("UserId", userId);

        var queryIterator = TodoContainer.Items.CreateItemQueryAsStream(query, userId, 10, continuationToken);

        CosmosResponseMessage queryResponse = await queryIterator.FetchNextSetAsync();
        // Pass stream directly to response object, without deserializing
        result.StatusCode = queryResponse.StatusCode;
        result.Content = new StreamContent(queryResponse.Content);
        return result;
    }
}