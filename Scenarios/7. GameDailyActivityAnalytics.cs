namespace Scenarios
{
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class GameDailyActivityAnalytics 
    {
        private CosmosContainer cosmosContainer;
        private CosmosContainer leaseContainer;
        private ChangeFeedProcessor changeFeedProcessor;

        public async Task StartAsync()
        {
            this.changeFeedProcessor = this.cosmosContainer
                .CreateChangeFeedProcessorBuilder<TwoPersonGame>("ActivityAnalytics", this.ProcessChanges)
                .WithInstanceName("DemoMachine")
                .WithCosmosLeaseContainer(leaseContainer)
                .Build();

            await this.changeFeedProcessor.StartAsync();
        }

        public async Task StopAsync()
        {
            if (this.changeFeedProcessor != null)
            {
                await this.changeFeedProcessor.StopAsync();
            }
        }

        private async Task ProcessChanges(IReadOnlyCollection<TwoPersonGame> changes, CancellationToken cancellationToken)
        {
            // Process the changes for analytics
            await Task.Yield();

            throw new NotImplementedException();
        }
    }
}
