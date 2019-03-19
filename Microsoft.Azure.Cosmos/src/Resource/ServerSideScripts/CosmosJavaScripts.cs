//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Internal;

    /// <summary>
    /// 
    /// </summary>
    public class CosmosJavaScripts
    {
        private readonly CosmosContainer container;
        private readonly CosmosClient client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public CosmosJavaScripts(CosmosContainer container)
        {
            this.container = container;
            this.client = container.Client;
        }

        public virtual Task<CosmosJavaScriptResponse> CreateAsync(
                    CosmosJavaScriptSettings scriptSettings,
                    CosmosJavaScriptRequestOptions requestOptions = null,
                    CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplmentedException();
        }

        public virtual Task<CosmosJavaScriptResponse> ReadAsync(
                    string id,
                    CosmosJavaScriptType scriptType,
                    CosmosJavaScriptRequestOptions requestOptions = null,
                    CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplmentedException();
        }

        public virtual Task<CosmosJavaScriptResponse> ReplaceAsync(
                    CosmosJavaScriptSettings scriptSettings,
                    CosmosJavaScriptRequestOptions requestOptions = null,
                    CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplmentedException();
        }

        public virtual Task<CosmosJavaScriptResponse> DeleteAsync(
                    string id,
                    CosmosJavaScriptType scriptType,
                    CosmosJavaScriptRequestOptions requestOptions = null,
                    CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplmentedException();
        }

        public virtual Task<CosmosStoredProcedureResponse<TOutput>> ExecuteStoredProcedureAsync<TInput, TOutput>(
            string id,
            object partitionKey,
            TInput input,
            CosmosJavaScriptExecuteRequestOptions requestOptions = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplmentedException();
        }

        public virtual CosmosResultSetIterator<CosmosJavaScriptSettings> GetStoredProcedureIterator(
            int? maxItemCount = null,
            string continuationToken = null)
        {
            throw new NotImplmentedException();
        }
    }
}
