namespace HeroScenarios
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public class BaseScenario
    {
        protected CosmosContainer containerItems;

        /// <summary>
        /// Execute it only once
        /// </summary>
        private async Task Setup()
        {
            string cosmosConnectionString = "{some-connection-string}";
            string gameDatabaseName = "gamedb";
            string gameContainerName = "gamecontainer";
            string gmaeContainerPkPath = "/day";

            CosmosClient client = new CosmosClient(cosmosConnectionString);
            CosmosDatabase database = await client.Databases.CreateDatabaseAsync(gameDatabaseName);
            CosmosContainer container = await database.Containers.CreateContainerAsync(gameContainerName, gmaeContainerPkPath);

            this.containerItems = container;
        }
    }
}
