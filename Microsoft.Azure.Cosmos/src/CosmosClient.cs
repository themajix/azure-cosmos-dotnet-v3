﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Handlers;
    using Microsoft.Azure.Cosmos.Query;
    using Microsoft.Azure.Documents;

    /// <summary>
    /// Provides a client-side logical representation of the Azure Cosmos DB database account.
    /// This client can be used to configure and execute requests in the Azure Cosmos DB database service.
    /// 
    /// CosmosClient is thread-safe. Its recommended to maintain a single instance of CosmosClient per lifetime 
    /// of the application which enables efficient connection management and performance.
    /// </summary>
    /// <example>
    /// This example create a <see cref="CosmosClient"/>, <see cref="CosmosDatabase"/>, and a <see cref="CosmosContainer"/>.
    /// The CosmosClient uses the <see cref="CosmosClientOptions"/> to get all the configuration values.
    /// <code language="c#">
    /// <![CDATA[
    /// CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(
    ///     accountEndPoint: "https://testcosmos.documents.azure.com:443/",
    ///     accountKey: "SuperSecretKey")
    ///     .WithApplicationRegion(LocationNames.EastUS2);
    /// 
    /// using (CosmosClient cosmosClient = cosmosClientBuilder.Build())
    /// {
    ///     CosmosDatabase db = await client.CreateDatabaseAsync(Guid.NewGuid().ToString())
    ///     CosmosContainer container = await db.CreateContainerAsync(Guid.NewGuid().ToString());
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// 
    /// <example>
    /// This example create a <see cref="CosmosClient"/>, <see cref="CosmosDatabase"/>, and a <see cref="CosmosContainer"/>.
    /// The CosmosClient is created with the AccountEndpoint and AccountKey.
    /// <code language="c#">
    /// <![CDATA[
    /// using (CosmosClient cosmosClient = new CosmosClient(
    ///     accountEndPoint: "https://testcosmos.documents.azure.com:443/",
    ///     accountKey: "SuperSecretKey"))
    /// {
    ///     CosmosDatabase db = await client.CreateDatabaseAsync(Guid.NewGuid().ToString())
    ///     CosmosContainer container = await db.Containers.CreateContainerAsync(Guid.NewGuid().ToString());
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// 
    /// <example>
    /// This example create a <see cref="CosmosClient"/>, <see cref="CosmosDatabase"/>, and a <see cref="CosmosContainer"/>.
    /// The CosmosClient is created with the connection string.
    /// <code language="c#">
    /// <![CDATA[
    /// using (CosmosClient cosmosClient = new CosmosClient(
    ///     connectionString: "AccountEndpoint=https://testcosmos.documents.azure.com:443/;AccountKey=SuperSecretKey;"))
    /// {
    ///     CosmosDatabase db = await client.CreateDatabaseAsync(Guid.NewGuid().ToString())
    ///     CosmosContainer container = await db.Containers.CreateContainerAsync(Guid.NewGuid().ToString());
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <remarks>
    /// <seealso href="https://docs.microsoft.com/azure/cosmos-db/distribute-data-globally" />
    /// <seealso href="https://docs.microsoft.com/azure/cosmos-db/partitioning-overview" />
    /// <seealso href="https://docs.microsoft.com/azure/cosmos-db/request-units" />
    /// </remarks>
    public class CosmosClient : IDisposable
    {
        private Lazy<CosmosOffers> offerSet;

        static CosmosClient()
        {
            HttpConstants.Versions.CurrentVersion = HttpConstants.Versions.v2018_12_31;
            HttpConstants.Versions.CurrentVersionUTF8 = Encoding.UTF8.GetBytes(HttpConstants.Versions.CurrentVersion);

            // V3 always assumes assemblies exists
            // Shall revisit on feedback
            ServiceInteropWrapper.AssembliesExist = new Lazy<bool>(() => true);
        }

        /// <summary>
        /// Create a new CosmosClient used for mock testing
        /// </summary>
        protected CosmosClient()
        {
        }

        /// <summary>
        /// Create a new CosmosClient with the connection
        /// </summary>
        /// <param name="connectionString">The connection string to the cosmos account. Example: https://mycosmosaccount.documents.azure.com:443/;AccountKey=SuperSecretKey;</param>
        /// <example>
        /// This example creates a CosmosClient
        /// <code language="c#">
        /// <![CDATA[
        /// using (CosmosClient cosmosClient = new CosmosClient(
        ///     connectionString: "https://testcosmos.documents.azure.com:443/;AccountKey=SuperSecretKey;"))
        /// {
        ///     // Create a database and other CosmosClient operations
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public CosmosClient(string connectionString)
            : this(new CosmosClientOptions(connectionString))
        {
        }

        /// <summary>
        /// Create a new CosmosClient with the account endpoint URI string and account key
        /// </summary>
        /// <param name="accountEndPoint">The cosmos service endpoint to use to create the client.</param>
        /// <param name="accountKey">The cosmos account key to use to create the client.</param>
        /// <example>
        /// This example creates a CosmosClient
        /// <code language="c#">
        /// <![CDATA[
        /// using (CosmosClient cosmosClient = new CosmosClient(
        ///     accountEndPoint: "https://testcosmos.documents.azure.com:443/",
        ///     accountKey: "SuperSecretKey"))
        /// {
        ///     // Create a database and other CosmosClient operations
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public CosmosClient(
            string accountEndPoint,
            string accountKey)
            : this(new CosmosClientOptions(accountEndPoint, accountKey))
        {
        }

        /// <summary>
        /// Create a new CosmosClient with the account endpoint URI string and account key
        /// </summary>
        /// <param name="accountEndPoint">The cosmos service endpoint to use to create the client.</param>
        /// <param name="accountKey">The cosmos account key to use to create the client.</param>
        /// <example>
        /// This example creates a CosmosClient
        /// <code language="c#">
        /// <![CDATA[
        /// using (CosmosClient cosmosClient = new CosmosClient(
        ///     accountEndPoint: "https://testcosmos.documents.azure.com:443/",
        ///     accountKey: "SuperSecretKey"))
        /// {
        ///     // Create a database and other CosmosClient operations
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public CosmosClient(
            string accountEndPoint,
            SecureString accountKey)
            : this(new CosmosClientOptions(accountEndPoint, accountKey))
        {
        }

        /// <summary>
        /// Create a new CosmosClient with the cosmosClientOption
        /// </summary>
        /// <param name="clientOptions">The <see cref="CosmosClientOptions"/> used to initialize the cosmos client.</param>
        /// <example>
        /// This example creates a CosmosClient through explicit CosmosClientOptions
        /// <code language="c#">
        /// <![CDATA[
        /// CosmosClientOptions clientOptions = new CosmosClientOptions("accountEndpoint", "accountkey");
        /// clientOptions.ApplicationRegion = "East US 2";
        /// clientOptions.ConnectionMode = ConnectionMode.Direct;
        /// clientOptions.RequestTimeout = TimeSpan.FromSeconds(5);
        /// 
        /// using (CosmosClient client = new CosmosClient(clientOptions))
        /// {
        ///     // Create a database and other CosmosClient operations
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <example>
        /// This example creates a CosmosClient through builder
        /// <code language="c#">
        /// <![CDATA[
        /// CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder("accountEndpoint", "accountkey")
        /// .UseConsistencyLevel(ConsistencyLevel.Eventual)
        /// .WithApplicationRegion("East US 2");
        /// CosmosClient client = cosmosClientBuilder.Build();
        /// ]]>
        /// </code>
        /// </example>
        public CosmosClient(CosmosClientOptions clientOptions)
        {
            if (clientOptions == null)
            {
                throw new ArgumentNullException(nameof(clientOptions));
            }

            CosmosClientOptions clientOptionsClone = clientOptions.Clone();

            DocumentClient documentClient = new DocumentClient(
                clientOptionsClone.EndPoint,
                clientOptionsClone.AccountKey,
                apitype: clientOptionsClone.ApiType,
                sendingRequestEventArgs: clientOptionsClone.SendingRequestEventArgs,
                transportClientHandlerFactory: clientOptionsClone.TransportClientHandlerFactory,
                connectionPolicy: clientOptionsClone.GetConnectionPolicy(),
                enableCpuMonitor: clientOptionsClone.EnableCpuMonitor,
                storeClientFactory: clientOptionsClone.StoreClientFactory);

            this.Init(
                clientOptionsClone,
                documentClient);
        }

        /// <summary>
        /// Used for unit testing only.
        /// </summary>
        internal CosmosClient(
            CosmosClientOptions cosmosClientOptions,
            DocumentClient documentClient)
        {
            if (cosmosClientOptions == null)
            {
                throw new ArgumentNullException(nameof(cosmosClientOptions));
            }

            if (documentClient == null)
            {
                throw new ArgumentNullException(nameof(documentClient));
            }

            this.Init(cosmosClientOptions, documentClient);
        }

        /// <summary>
        /// The <see cref="Cosmos.CosmosClientOptions"/> used initialize CosmosClient
        /// </summary>
        public virtual CosmosClientOptions ClientOptions { get; private set; }

        internal CosmosOffers Offers => this.offerSet.Value;
        internal DocumentClient DocumentClient { get; set; }
        internal RequestInvokerHandler RequestHandler { get; private set; }
        internal ConsistencyLevel AccountConsistencyLevel { get; private set; }
        internal CosmosResponseFactory ResponseFactory { get; private set; }
        internal CosmosClientContext ClientContext { get; private set; }

        /// <summary>
        /// Read the <see cref="Microsoft.Azure.Cosmos.CosmosAccountSettings"/> from the Azure Cosmos DB service as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A <see cref="CosmosAccountSettings"/> wrapped in a <see cref="System.Threading.Tasks.Task"/> object.
        /// </returns>
        public virtual Task<CosmosAccountSettings> GetAccountSettingsAsync()
        {
            return ((IDocumentClientInternal)this.DocumentClient).GetDatabaseAccountInternalAsync(this.ClientOptions.EndPoint);
        }

        /// <summary>
        /// Returns a reference to a database object. 
        /// </summary>
        /// <param name="id">The cosmos database id</param>
        /// <remarks>
        /// Note that the database must be explicitly created, if it does not already exist, before
        /// you can read from it or write to it.
        /// </remarks>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// CosmosDatabase db = this.cosmosClient.GetDatabase("myDatabaseId"];
        /// DatabaseResponse response = await db.ReadAsync();
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Cosmos database proxy</returns>
        public virtual CosmosDatabase GetDatabase(string id)
        {
            return new CosmosDatabaseCore(this.ClientContext, id);
        }

        /// <summary>
        /// Get cosmos container proxy. 
        /// </summary>
        /// <remarks>Proxy existence doesn't guarantee either database or container existence.</remarks>
        /// <param name="databaseId">cosmos database name</param>
        /// <param name="containerId">cosmos container name</param>
        /// <returns>Cosmos container proxy</returns>
        public virtual CosmosContainer GetContainer(string databaseId, string containerId)
        {
            if (string.IsNullOrEmpty(databaseId))
            {
                throw new ArgumentNullException(nameof(databaseId));
            }

            if (string.IsNullOrEmpty(containerId))
            {
                throw new ArgumentNullException(nameof(containerId));
            }

            return this.GetDatabase(databaseId).GetContainer(containerId);
        }

        /// <summary>
        /// Send a request for creating a database.
        ///
        /// A database manages users, permissions and a set of containers.
        /// Each Azure Cosmos DB Database Account is able to support multiple independent named databases,
        /// with the database being the logical container for data.
        ///
        /// Each Database consists of one or more containers, each of which in turn contain one or more
        /// documents. Since databases are an administrative resource, the Service Master Key will be
        /// required in order to access and successfully complete any action using the User APIs.
        /// </summary>
        /// <param name="id">The database id.</param>
        /// <param name="requestUnitsPerSecond">(Optional) The throughput provisioned for a database in measurement of Request Units per second in the Azure Cosmos DB service.</param>
        /// <param name="requestOptions">(Optional) A set of options that can be set.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="DatabaseResponse"/> which wraps a <see cref="CosmosDatabaseSettings"/> containing the resource record.</returns>
        /// <remarks>
        /// <seealso href="https://docs.microsoft.com/azure/cosmos-db/request-units"/> for details on provision throughput.
        /// </remarks>
        public virtual Task<DatabaseResponse> CreateDatabaseAsync(
                string id,
                int? requestUnitsPerSecond = null,
                RequestOptions requestOptions = null,
                CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            CosmosDatabaseSettings databaseSettings = this.PrepareCosmosDatabaseSettings(id);
            return this.CreateDatabaseAsync(
                databaseSettings: databaseSettings,
                requestUnitsPerSecond: requestUnitsPerSecond,
                requestOptions: requestOptions,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check if a database exists, and if it doesn't, create it.
        /// This will make a read operation, and if the database is not found it will do a create operation.
        ///
        /// A database manages users, permissions and a set of containers.
        /// Each Azure Cosmos DB Database Account is able to support multiple independent named databases,
        /// with the database being the logical container for data.
        ///
        /// Each Database consists of one or more containers, each of which in turn contain one or more
        /// documents. Since databases are an administrative resource, the Service Master Key will be
        /// required in order to access and successfully complete any action using the User APIs.
        /// </summary>
        /// <param name="id">The database id.</param>
        /// <param name="requestUnitsPerSecond">(Optional) The throughput provisioned for a database in measurement of Request Units per second in the Azure Cosmos DB service.</param>
        /// <param name="requestOptions">(Optional) A set of additional options that can be set.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="DatabaseResponse"/> which wraps a <see cref="CosmosDatabaseSettings"/> containing the resource record.</returns>
        /// <remarks>
        /// <seealso href="https://docs.microsoft.com/azure/cosmos-db/request-units"/> for details on provision throughput.
        /// </remarks>
        public virtual async Task<DatabaseResponse> CreateDatabaseIfNotExistsAsync(
            string id,
            int? requestUnitsPerSecond = null,
            RequestOptions requestOptions = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            // Doing a Read before Create will give us better latency for existing databases
            CosmosDatabase database = this.GetDatabase(id);
            DatabaseResponse cosmosDatabaseResponse = await database.ReadAsync(cancellationToken: cancellationToken);
            if (cosmosDatabaseResponse.StatusCode != HttpStatusCode.NotFound)
            {
                return cosmosDatabaseResponse;
            }

            cosmosDatabaseResponse = await this.CreateDatabaseAsync(id, requestUnitsPerSecond, requestOptions, cancellationToken: cancellationToken);
            if (cosmosDatabaseResponse.StatusCode != HttpStatusCode.Conflict)
            {
                return cosmosDatabaseResponse;
            }

            // This second Read is to handle the race condition when 2 or more threads have Read the database and only one succeeds with Create
            // so for the remaining ones we should do a Read instead of throwing Conflict exception
            return await database.ReadAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets an iterator to go through all the databases for the account
        /// </summary>
        /// <param name="maxItemCount">The max item count to return as part of the query</param>
        /// <param name="continuationToken">The continuation token in the Azure Cosmos DB service.</param>
        /// <example>
        /// Get an iterator for all the database under the cosmos account
        /// <code language="c#">
        /// <![CDATA[
        /// FeedIterator<CosmosDatabaseSettings> feedIterator = this.cosmosClient.GetDatabasesIterator();
        /// {
        ///     foreach (CosmosDatabaseSettings databaseSettings in  await feedIterator.FetchNextSetAsync())
        ///     {
        ///         Console.WriteLine(setting.Id); 
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>An iterator to go through the databases.</returns>
        public virtual FeedIterator<CosmosDatabaseSettings> GetDatabasesIterator(
            int? maxItemCount = null,
            string continuationToken = null)
        {
            return new FeedIteratorCore<CosmosDatabaseSettings>(
                maxItemCount,
                continuationToken,
                options: null,
                nextDelegate: this.DatabaseFeedRequestExecutorAsync);
        }

        /// <summary>
        /// Send a request for creating a database.
        ///
        /// A database manages users, permissions and a set of containers.
        /// Each Azure Cosmos DB Database Account is able to support multiple independent named databases,
        /// with the database being the logical container for data.
        ///
        /// Each Database consists of one or more containers, each of which in turn contain one or more
        /// documents. Since databases are an administrative resource, the Service Master Key will be
        /// required in order to access and successfully complete any action using the User APIs.
        /// </summary>
        /// <param name="databaseSettings">The database settings</param>
        /// <param name="requestUnitsPerSecond">(Optional) The throughput provisioned for a database in measurement of Request Units per second in the Azure Cosmos DB service.</param>
        /// <param name="requestOptions">(Optional) A set of options that can be set.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="DatabaseResponse"/> which wraps a <see cref="CosmosDatabaseSettings"/> containing the resource record.</returns>
        /// <remarks>
        /// <seealso href="https://docs.microsoft.com/azure/cosmos-db/request-units"/> for details on provision throughput.
        /// </remarks>
        public virtual Task<CosmosResponseMessage> CreateDatabaseStreamAsync(
                CosmosDatabaseSettings databaseSettings,
                int? requestUnitsPerSecond = null,
                RequestOptions requestOptions = null,
                CancellationToken cancellationToken = default(CancellationToken))
        {
            if (databaseSettings == null)
            {
                throw new ArgumentNullException(nameof(databaseSettings));
            }

            this.ClientContext.ValidateResource(databaseSettings.Id);
            Stream streamPayload = this.ClientContext.SettingsSerializer.ToStream<CosmosDatabaseSettings>(databaseSettings);

            return this.CreateDatabaseStreamInternalAsync(streamPayload, requestUnitsPerSecond, requestOptions, cancellationToken);
        }

        /// <summary>
        /// Dispose of cosmos client
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void Init(
            CosmosClientOptions clientOptions,
            DocumentClient documentClient)
        {
            this.ClientOptions = clientOptions;
            this.DocumentClient = documentClient;

            //Request pipeline 
            ClientPipelineBuilder clientPipelineBuilder = new ClientPipelineBuilder(
                this,
                this.DocumentClient.ResetSessionTokenRetryPolicy,
                this.ClientOptions.CustomHandlers);

            // DocumentClient is not initialized with any consistency overrides so default is backend consistency
            this.AccountConsistencyLevel = (ConsistencyLevel)this.DocumentClient.ConsistencyLevel;

            this.RequestHandler = clientPipelineBuilder.Build();

            this.ResponseFactory = new CosmosResponseFactory(
                defaultJsonSerializer: this.ClientOptions.SettingsSerializer,
                userJsonSerializer: this.ClientOptions.CosmosSerializerWithWrapperOrDefault);

            this.ClientContext = new CosmosClientContextCore(
                client: this,
                clientOptions: this.ClientOptions,
                userJsonSerializer: this.ClientOptions.CosmosSerializerWithWrapperOrDefault,
                defaultJsonSerializer: this.ClientOptions.SettingsSerializer,
                cosmosResponseFactory: this.ResponseFactory,
                requestHandler: this.RequestHandler,
                documentClient: this.DocumentClient,
                documentQueryClient: new DocumentQueryClient(this.DocumentClient));

            this.offerSet = new Lazy<CosmosOffers>(() => new CosmosOffers(this.DocumentClient), LazyThreadSafetyMode.PublicationOnly);
        }

        internal CosmosDatabaseSettings PrepareCosmosDatabaseSettings(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            CosmosDatabaseSettings databaseSettings = new CosmosDatabaseSettings()
            {
                Id = id
            };

            this.ClientContext.ValidateResource(databaseSettings.Id);
            return databaseSettings;
        }

        internal Task<DatabaseResponse> CreateDatabaseAsync(
                    CosmosDatabaseSettings databaseSettings,
                    int? requestUnitsPerSecond = null,
                    RequestOptions requestOptions = null,
                    CancellationToken cancellationToken = default(CancellationToken))
        {
            Task<CosmosResponseMessage> response = this.CreateDatabaseStreamInternalAsync(
                streamPayload: this.ClientContext.SettingsSerializer.ToStream<CosmosDatabaseSettings>(databaseSettings),
                requestUnitsPerSecond: requestUnitsPerSecond,
                requestOptions: requestOptions,
                cancellationToken: cancellationToken);

            return this.ClientContext.ResponseFactory.CreateDatabaseResponseAsync(this.GetDatabase(databaseSettings.Id), response);
        }

        private Task<CosmosResponseMessage> CreateDatabaseStreamInternalAsync(
                Stream streamPayload,
                int? requestUnitsPerSecond = null,
                RequestOptions requestOptions = null,
                CancellationToken cancellationToken = default(CancellationToken))
        {
            Uri resourceUri = new Uri(Paths.Databases_Root, UriKind.Relative);
            return this.ClientContext.ProcessResourceOperationStreamAsync(
                resourceUri: resourceUri,
                resourceType: ResourceType.Database,
                operationType: OperationType.Create,
                requestOptions: requestOptions,
                cosmosContainerCore: null,
                partitionKey: null,
                streamPayload: streamPayload,
                requestEnricher: (httpRequestMessage) => httpRequestMessage.AddThroughputHeader(requestUnitsPerSecond),
                cancellationToken: cancellationToken);
        }

        private Task<FeedResponse<CosmosDatabaseSettings>> DatabaseFeedRequestExecutorAsync(
            int? maxItemCount,
            string continuationToken,
            RequestOptions options,
            object state,
            CancellationToken cancellationToken)
        {
            Debug.Assert(state == null);

            Uri resourceUri = new Uri(Paths.Databases_Root, UriKind.Relative);
            return this.ClientContext.ProcessResourceOperationAsync<FeedResponse<CosmosDatabaseSettings>>(
                resourceUri: resourceUri,
                resourceType: ResourceType.Database,
                operationType: OperationType.ReadFeed,
                requestOptions: options,
                cosmosContainerCore: null,
                partitionKey: null,
                streamPayload: null,
                requestEnricher: request =>
                {
                    QueryRequestOptions.FillContinuationToken(request, continuationToken);
                    QueryRequestOptions.FillMaxItemCount(request, maxItemCount);
                },
                responseCreator: response => this.ClientContext.ResponseFactory.CreateResultSetQueryResponse<CosmosDatabaseSettings>(response),
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Dispose of cosmos client
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.DocumentClient != null)
            {
                this.DocumentClient.Dispose();
                this.DocumentClient = null;
            }
        }
    }
}
