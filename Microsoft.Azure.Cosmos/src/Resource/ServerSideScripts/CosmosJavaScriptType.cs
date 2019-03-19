//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the cosmos server side script type <see cref="https://docs.microsoft.com/en-us/azure/cosmos-db/stored-procedures-triggers-udfs"/>
    /// </summary>
    public enum CosmosJavaScriptType
    {
        /// <summary>
        /// Defined a stored procedure <see cref="https://docs.microsoft.com/en-us/rest/api/cosmos-db/stored-procedures"/>
        /// </summary>
        StoredProcedure,

        /// <summary>
        /// Defines a user defined function <see cref="https://docs.microsoft.com/en-us/rest/api/cosmos-db/user-defined-functions"/>
        /// </summary>
        UserDefinedFunction,

        /// <summary>
        /// Defined a Trigger <see cref="https://docs.microsoft.com/en-us/rest/api/cosmos-db/triggers"/>
        /// </summary>
        Trigger,
    }
}
