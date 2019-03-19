//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines a cosmos Java script
    /// 
    /// TODO: Make it CosmosResource ?? 
    /// </summary>
    public sealed class CosmosJavaScriptSettings 
    {
        /// <summary>
        /// Identfier used to address the resource
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Java script body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Script type (StoreProcedure, UderDefinedFunction, Trigger)
        /// </summary>
        public CosmosJavaScriptType Type { get; set; }
    }
}
