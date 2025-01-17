﻿/*
   Copyright 2012-2023 Marco De Salvo

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using RDFSharp.Model;
using RDFSharp.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using static RDFSharp.Query.RDFQueryUtilities;

namespace RDFSharp.Query
{
    /// <summary>
    /// RDFConstructQuery is the SPARQL "CONSTRUCT" query implementation.
    /// </summary>
    public class RDFConstructQuery : RDFQuery
    {
        #region Properties
        /// <summary>
        /// List of template patterns carried by the query
        /// </summary>
        internal List<RDFPattern> Templates { get; set; }

        /// <summary>
        /// List of variables carried by the template patterns of the query
        /// </summary>
        internal List<RDFVariable> Variables { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty CONSTRUCT query
        /// </summary>
        public RDFConstructQuery()
        {
            Templates = new List<RDFPattern>();
            Variables = new List<RDFVariable>();
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gives the string representation of the CONSTRUCT query
        /// </summary>
        public override string ToString()
            => RDFQueryPrinter.PrintConstructQuery(this);
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given pattern to the templates of the query
        /// </summary>
        public RDFConstructQuery AddTemplate(RDFPattern template)
        {
            if (template != null && !Templates.Any(tp => tp.ToString().Equals(template.ToString())))
            {
                Templates.Add(template);

                //Context
                if (template.Context != null && template.Context is RDFVariable)
                {
                    if (!Variables.Any(v => v.Equals(template.Context)))
                        Variables.Add((RDFVariable)template.Context);
                }

                //Subject
                if (template.Subject is RDFVariable)
                {
                    if (!Variables.Any(v => v.Equals(template.Subject)))
                        Variables.Add((RDFVariable)template.Subject);
                }

                //Predicate
                if (template.Predicate is RDFVariable)
                {
                    if (!Variables.Any(v => v.Equals(template.Predicate)))
                        Variables.Add((RDFVariable)template.Predicate);
                }

                //Object
                if (template.Object is RDFVariable)
                {
                    if (!Variables.Any(v => v.Equals(template.Object)))
                        Variables.Add((RDFVariable)template.Object);
                }
            }
            return this;
        }

        /// <summary>
        /// Adds the given pattern group to the body of the query
        /// </summary>
        public RDFConstructQuery AddPatternGroup(RDFPatternGroup patternGroup)
            => AddPatternGroup<RDFConstructQuery>(patternGroup);

        /// <summary>
        /// Adds the given modifier to the query
        /// </summary>
        public RDFConstructQuery AddModifier(RDFDistinctModifier modifier)
            => AddModifier<RDFConstructQuery>(modifier);

        /// <summary>
        /// Adds the given modifier to the query
        /// </summary>
        public RDFConstructQuery AddModifier(RDFLimitModifier modifier)
            => AddModifier<RDFConstructQuery>(modifier);

        /// <summary>
        /// Adds the given modifier to the query
        /// </summary>
        public RDFConstructQuery AddModifier(RDFOffsetModifier modifier)
            => AddModifier<RDFConstructQuery>(modifier);

        /// <summary>
        /// Adds the given prefix declaration to the query
        /// </summary>
        public RDFConstructQuery AddPrefix(RDFNamespace prefix)
            => AddPrefix<RDFConstructQuery>(prefix);

        /// <summary>
        /// Adds the given subquery to the query
        /// </summary>
        public RDFConstructQuery AddSubQuery(RDFSelectQuery subQuery)
            => AddSubQuery<RDFConstructQuery>(subQuery);

        /// <summary>
        /// Applies the query to the given graph
        /// </summary>
        public RDFConstructQueryResult ApplyToGraph(RDFGraph graph)
            => graph != null ? new RDFQueryEngine().EvaluateConstructQuery(this, graph)
                             : new RDFConstructQueryResult();

        /// <summary>
        /// Applies the query to the given asynchronous graph
        /// </summary>
        public Task<RDFConstructQueryResult> ApplyToGraphAsync(RDFAsyncGraph asyncGraph)
            => Task.Run(() => ApplyToGraph(asyncGraph?.WrappedGraph));

        /// <summary>
        /// Applies the query to the given store
        /// </summary>
        public RDFConstructQueryResult ApplyToStore(RDFStore store)
            => store != null ? new RDFQueryEngine().EvaluateConstructQuery(this, store)
                             : new RDFConstructQueryResult();

        /// <summary>
        /// Applies the query to the given asynchronous store
        /// </summary>
        public Task<RDFConstructQueryResult> ApplyToStoreAsync(RDFAsyncStore asyncStore)
            => Task.Run(() => ApplyToStore(asyncStore?.WrappedStore));

        /// <summary>
        /// Applies the query to the given federation
        /// </summary>
        public RDFConstructQueryResult ApplyToFederation(RDFFederation federation)
            => federation != null ? new RDFQueryEngine().EvaluateConstructQuery(this, federation)
                                  : new RDFConstructQueryResult();

        /// <summary>
        /// Asynchronously applies the query to the given federation
        /// </summary>
        public Task<RDFConstructQueryResult> ApplyToFederationAsync(RDFFederation federation)
            => Task.Run(() => ApplyToFederation(federation));

        /// <summary>
        /// Applies the query to the given SPARQL endpoint
        /// </summary>
        public RDFConstructQueryResult ApplyToSPARQLEndpoint(RDFSPARQLEndpoint sparqlEndpoint)
            => ApplyRawToSPARQLEndpoint(ToString(), sparqlEndpoint, new RDFSPARQLEndpointQueryOptions());

        /// <summary>
        /// Applies the given raw string CONSTRUCT query to the given SPARQL endpoint
        /// </summary>
        public static RDFConstructQueryResult ApplyRawToSPARQLEndpoint(string constructQuery, RDFSPARQLEndpoint sparqlEndpoint)
            => ApplyRawToSPARQLEndpoint(constructQuery, sparqlEndpoint, new RDFSPARQLEndpointQueryOptions());

        /// <summary>
        /// Applies the query to the given SPARQL endpoint
        /// </summary>
        public RDFConstructQueryResult ApplyToSPARQLEndpoint(RDFSPARQLEndpoint sparqlEndpoint, RDFSPARQLEndpointQueryOptions sparqlEndpointQueryOptions)
            => ApplyRawToSPARQLEndpoint(ToString(), sparqlEndpoint, sparqlEndpointQueryOptions);

        /// <summary>
        /// Applies the given raw string CONSTRUCT query to the given SPARQL endpoint
        /// </summary>
        public static RDFConstructQueryResult ApplyRawToSPARQLEndpoint(string constructQuery, RDFSPARQLEndpoint sparqlEndpoint, RDFSPARQLEndpointQueryOptions sparqlEndpointQueryOptions)
        {
            RDFConstructQueryResult constructResult = new RDFConstructQueryResult();
            if (!string.IsNullOrWhiteSpace(constructQuery) && sparqlEndpoint != null)
            {
                if (sparqlEndpointQueryOptions == null)
                    sparqlEndpointQueryOptions = new RDFSPARQLEndpointQueryOptions();

                //Establish a connection to the given SPARQL endpoint
                using (RDFWebClient webClient = new RDFWebClient(sparqlEndpointQueryOptions.TimeoutMilliseconds))
                {
                    //Insert reserved "query" parameter
                    webClient.QueryString.Add("query", HttpUtility.UrlEncode(constructQuery));

                    //Insert user-provided parameters
                    webClient.QueryString.Add(sparqlEndpoint.QueryParams);

                    //Insert request headers
                    webClient.Headers.Add(HttpRequestHeader.Accept, "application/turtle");
                    webClient.Headers.Add(HttpRequestHeader.Accept, "text/turtle");

                    //Insert eventual authorization headers
                    sparqlEndpoint.FillWebClientAuthorization(webClient);

                    //Send querystring to SPARQL endpoint
                    byte[] sparqlResponse = null;
                    try
                    {
                        sparqlResponse = webClient.DownloadData(sparqlEndpoint.BaseAddress);
                    }
                    catch (Exception ex)
                    {
                        if (sparqlEndpointQueryOptions.ErrorBehavior == RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.ThrowException)
                            throw new RDFQueryException($"CONSTRUCT query on SPARQL endpoint failed because: {ex.Message}", ex);
                    }

                    //Parse response from SPARQL endpoint
                    if (sparqlResponse != null)
                    {
                        using (MemoryStream sStream = new MemoryStream(sparqlResponse))
                            constructResult = RDFConstructQueryResult.FromRDFGraph(RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, sStream));
                    }
                }

                //Eventually adjust column names (should start with "?")
                int columnsCount = constructResult.ConstructResults.Columns.Count;
                for (int i = 0; i < columnsCount; i++)
                {
                    if (!constructResult.ConstructResults.Columns[i].ColumnName.StartsWith("?"))
                        constructResult.ConstructResults.Columns[i].ColumnName = string.Concat("?", constructResult.ConstructResults.Columns[i].ColumnName);
                }
            }
            return constructResult;
        }

        /// <summary>
        /// Asynchronously applies the query to the given SPARQL endpoint
        /// </summary>
        public Task<RDFConstructQueryResult> ApplyToSPARQLEndpointAsync(RDFSPARQLEndpoint sparqlEndpoint)
            => ApplyRawToSPARQLEndpointAsync(ToString(), sparqlEndpoint, new RDFSPARQLEndpointQueryOptions());

        /// <summary>
        /// Asynchronously applies the given raw string CONSTRUCT query to the given SPARQL endpoint
        /// </summary>
        public static Task<RDFConstructQueryResult> ApplyRawToSPARQLEndpointAsync(string constructQuery, RDFSPARQLEndpoint sparqlEndpoint)
            => ApplyRawToSPARQLEndpointAsync(constructQuery, sparqlEndpoint, new RDFSPARQLEndpointQueryOptions());

        /// <summary>
        /// Asynchronously applies the query to the given SPARQL endpoint
        /// </summary>
        public Task<RDFConstructQueryResult> ApplyToSPARQLEndpointAsync(RDFSPARQLEndpoint sparqlEndpoint, RDFSPARQLEndpointQueryOptions sparqlEndpointQueryOptions)
            => ApplyRawToSPARQLEndpointAsync(ToString(), sparqlEndpoint, sparqlEndpointQueryOptions);

        /// <summary>
        /// Asynchronously applies the given raw string CONSTRUCT query to the given SPARQL endpoint
        /// </summary>
        public static Task<RDFConstructQueryResult> ApplyRawToSPARQLEndpointAsync(string constructQuery, RDFSPARQLEndpoint sparqlEndpoint, RDFSPARQLEndpointQueryOptions sparqlEndpointQueryOptions)
            => Task.Run(() => ApplyRawToSPARQLEndpoint(constructQuery, sparqlEndpoint, sparqlEndpointQueryOptions));
        #endregion
    }
}