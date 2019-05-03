//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace HeroScenarios
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public class Query : BaseScenario
    {
        public async Task<IEnumerable<string>> ListGamesForDayAsync(string day, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(day)) throw new ArgumentNullException(nameof(day));

            string query = $"select Id from r";
            CosmosResultSetIterator<string> resultSet = this.containerItems.CreateItemQuery<string>(query, day);
            return await resultSet.FetchNextSetAsync(cancellationToken);
        }
    }
}
