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

    public class ConflictsHandler
    {
        private readonly CosmosContainer cosmosContainer;
        private const uint ConcurrencyRetires = 3;
        private const string ContainerName = "DemoContainer";
        private const string ContainerPartitionKey = "/pk";

        public ConflictsHandler(CosmosContainer cosmosContainer)
        {
            this.cosmosContainer = cosmosContainer;
        }

        public async Task<CosmosContainer> AutoResolveContainerAsync(CancellationToken cancellationToken)
        {
            CosmosDatabase database = null;

            // Configure last-write wins policy
            return await database
                .Containers.DefineContainer(ConflictsHandler.ContainerName, ConflictsHandler.ContainerPartitionKey)
                .ConflictPolicy()
                    .LastWriterWins("/_ts")
                    .Attach()
                .CreateAsync(cancellationToken: cancellationToken);

        }

        public async Task DemoConflictsHandlerAsync(CancellationToken cancellationToken)
        {
            FeedIterator<CosmosConflictSettings> conflictsIterator = this.cosmosContainer.Conflicts.GetConflictsIterator(10);
            while (conflictsIterator.HasMoreResults)
            {
                FeedResponse<CosmosConflictSettings> conflicts = await conflictsIterator.FetchNextSetAsync(cancellationToken);
                foreach (var entry in conflicts)
                {
                    // Read payload which failed 
                    TwoPersonGame uncommitedItem = entry.GetResource<TwoPersonGame>();

                    // Read the source item which conflicted
                    TwoPersonGame conflictSource = await this.cosmosContainer.Conflicts.ReadConflictSourceItemAsync<TwoPersonGame>(
                                this.GetPartitionKey(uncommitedItem), 
                                entry, 
                                cancellationToken);

                    // Resolve conflict 
                    await this.ResolveConflict(conflictSource, uncommitedItem);

                    // Delete conflict 
                    await this.cosmosContainer.Conflicts.DeleteConflictAsync(this.GetPartitionKey(uncommitedItem), entry.Id);
                }
            }
        }

        private Task ResolveConflict(TwoPersonGame source, TwoPersonGame uncommitted)
        {
            // Application logic of conflict resolution 
            throw new NotImplementedException();
        }

        private object GetPartitionKey(TwoPersonGame game)
        {
            return game.Day;
        }
    }
}
