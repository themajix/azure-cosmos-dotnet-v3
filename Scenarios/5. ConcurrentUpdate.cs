//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Scenarios
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public class ConcurrentUpdate
    {
        private readonly CosmosContainer cosmosContainer;
        private const uint ConcurrencyRetires = 3;

        public async Task<TwoPersonGame> IncrementUser1ScoreAsync(
            string gameDay, 
            string gameId, 
            uint incrementScore,
            CancellationToken cancellationToken)
        {
            ItemResponse<TwoPersonGame> gameReadResponse = await this.cosmosContainer
                        .ReadItemAsync<TwoPersonGame>(gameDay, gameId, cancellationToken: cancellationToken);
            if (gameReadResponse.StatusCode == HttpStatusCode.NotFound)
            {
                gameReadResponse = await this.CreateGameIfNotExists(gameDay, gameId, incrementScore, cancellationToken);
            }

            int retryCount = 0;
            while(true)
            {
                retryCount++;

                TwoPersonGame game = gameReadResponse;
                game.User1Score += incrementScore;

                // Pre-condition
                ItemRequestOptions options = new ItemRequestOptions();
                options.IfMatchEtag = gameReadResponse.ETag;

                try
                {
                    ItemResponse<TwoPersonGame> gameReplaceResponse = await this.cosmosContainer
                        .UpsertItemAsync<TwoPersonGame>(game, options, cancellationToken: cancellationToken);
                    return gameReplaceResponse.Resource;
                }
                catch (CosmosException ex) 
                    when (retryCount < ConcurrentUpdate.ConcurrencyRetires         // Max retries
                          && ex.StatusCode == HttpStatusCode.PreconditionFailed) // Concurrent update
                {
                    // Retry by reading again
                    gameReadResponse = await this.cosmosContainer.ReadItemAsync<TwoPersonGame>(gameId, gameId, cancellationToken: cancellationToken);
                }
            }
        }

        public async Task<ItemResponse<TwoPersonGame>> CreateGameIfNotExists(
            string gameDay,
            string gameId,
            uint user1Score,
            CancellationToken cancellationToken)
        {
            TwoPersonGame newGame = new TwoPersonGame(gameDay, gameId);
            newGame.User1Score = user1Score;

            try
            {
                return await this.cosmosContainer.CreateItemAsync<TwoPersonGame>(newGame, cancellationToken: cancellationToken);
            }
            // Concurrent write conflict
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                // Assume game never gets deleted :-)
                return await this.cosmosContainer.ReadItemAsync<TwoPersonGame>(gameDay, gameId, cancellationToken: cancellationToken);
            }
        }
    }
}
