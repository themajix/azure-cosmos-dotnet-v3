//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Scripts
{
    using System;

    /// <summary>
    /// Extensions to interact with Scripts.
    /// </summary>
    /// <seealso cref="CosmosStoredProcedureSettings"/>
    /// <seealso cref="CosmosTriggerSettings"/>
    /// <seealso cref="CosmosUserDefinedFunctionSettings"/>
    public static class ScriptsExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cosmosContainer"></param>
        /// <returns></returns>
        public static CosmosScripts GetScripts(this CosmosContainer cosmosContainer)
        {
            throw new NotImplementedException();
            // return new CosmosScriptsCore((CosmosContainerCore) cosmosContainer);
        }
    }
}
