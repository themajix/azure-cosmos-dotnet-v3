//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Internal;

    /// <summary>
    /// 
    /// </summary>
    public class CosmosJavaScriptResponse : CosmosResponse<CosmosJavaScriptSettings>
    {
        /// <summary>
        /// Create a <see cref="CosmosJavaScriptResponse"/> as a no-op for mock testing
        /// </summary>
        public CosmosJavaScriptResponse() : base()
        {
        }

        /// <summary>
        /// A private constructor to ensure the factory is used to create the object.
        /// This will prevent memory leaks when handling the HttpResponseMessage
        /// </summary>
        internal CosmosJavaScriptResponse(
           HttpStatusCode httpStatusCode,
           CosmosResponseMessageHeaders headers,
           CosmosStoredProcedureSettings cosmosStoredProcedureSettings,
           CosmosJavaScriptSettings scriptSettings) : base(
               httpStatusCode,
               headers,
               cosmosStoredProcedureSettings)
        {
            this.ScriptSettings = scriptSettings;
        }

        /// <summary>
        /// The reference to the cosmos stored procedure.
        /// This allows additional operations for the stored procedure
        /// </summary>
        public virtual CosmosJavaScriptSettings ScriptSettings => this.Resource;
    }
}