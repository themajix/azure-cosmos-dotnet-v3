//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace HeroScenarios
{
    using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public static class DatabaseAndContainerCreate
    {
		public static CosmosClient CreateClient(string connectionString)
        {
			// Default client 
            CosmosClient cosmosClient = new CosmosClient(connectionString);

			// Expected production usage 
            cosmosClient = new CosmosClientBuilder(connectionString)
                .UseCurrentRegion("West US")	// App co-location 
                .UseUserAgentSuffix("DemoApp")	// Telemetry
                .Build();

            return cosmosClient;
        }

		public static async Task<CosmosContainer> DemoCreateContainerAsync(
				CosmosClient client,
				string databaseName,
				string containerName,
				string partitionKey)
        {
			// Default create experiences 
			CosmosDatabase database = await client.Databases.CreateDatabaseIfNotExistsAsync(databaseName);
            CosmosContainer container = await database.Containers.CreateContainerIfNotExistsAsync(containerName, partitionKey);

            // Advanced scenario: Indexing tuning (optimize space)
            container = await database.Containers.DefineContainer(containerName, partitionKey)
                .DefaultTimeToLive(TimeSpan.FromDays(1))
                .UniqueKey()
                    .Path("/players/id")
                    .Attach()
                .IndexingPolicy()
                    .IncludedPaths()
                        .Path("/players")
                        .Path("/querypath")
                        .Attach()
                    .Attach()
                .CreateAsync();

            return client.Databases[databaseName].Containers[containerName];
        }
    }
}
