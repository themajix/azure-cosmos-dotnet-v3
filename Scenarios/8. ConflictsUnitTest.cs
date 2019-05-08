//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Scenarios
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Moq;

    public class ConflictsUnitTest
    {
        public Task TestConflicts()
        {
            var mockContainer = new Mock<CosmosContainer>();
            var mockConflicts = new Mock<CosmosConflicts>();
            var mockIterator = new Mock<FeedIterator<CosmosConflictSettings>>();
            var mockConflictsResponse = new Mock<FeedResponse<CosmosConflictSettings>>();

            List<CosmosConflictSettings> cosmosConflictSettings = new List<CosmosConflictSettings>();
            // cosmosConflictSettings.Add();

            mockContainer.Setup(t => t.Conflicts).Returns(mockConflicts.Object);

            mockConflicts.Setup(t => t.GetConflictsIterator(It.IsAny<int?>(), It.IsAny<string>()))
                .Returns(mockIterator.Object);

            mockIterator.Setup(t => t.FetchNextSetAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mockConflictsResponse.Object));

            throw new NotImplementedException();
        }
    }
}
