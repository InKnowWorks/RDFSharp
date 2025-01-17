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
using System.Collections;
using System.Data;
using System.Threading.Tasks;

namespace RDFSharp.Query
{
    /// <summary>
    /// RDFDescribeQueryResult is a container for SPARQL "DESCRIBE" query results.
    /// </summary>
    public class RDFDescribeQueryResult
    {
        #region Properties
        /// <summary>
        /// Tabular response of the query
        /// </summary>
        public DataTable DescribeResults { get; internal set; }

        /// <summary>
        /// Gets the number of results produced by the query
        /// </summary>
        public long DescribeResultsCount
            => DescribeResults.Rows.Count;
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty DESCRIBE result
        /// </summary>
        internal RDFDescribeQueryResult()
            => DescribeResults = new DataTable();
        #endregion

        #region Methods
        /// <summary>
        /// Gets a graph corresponding to the query result
        /// </summary>
        public RDFGraph ToRDFGraph()
        {
            RDFGraph result = new RDFGraph();
            RDFPatternMember subj = null;
            RDFPatternMember pred = null;
            RDFPatternMember obj = null;

            //Iterate the datatable rows and generate the corresponding triples to be added to the result graph
            IEnumerator resultRows = DescribeResults.Rows.GetEnumerator();
            while (resultRows.MoveNext())
            {
                subj = RDFQueryUtilities.ParseRDFPatternMember(((DataRow)resultRows.Current)["?SUBJECT"].ToString());
                pred = RDFQueryUtilities.ParseRDFPatternMember(((DataRow)resultRows.Current)["?PREDICATE"].ToString());
                obj = RDFQueryUtilities.ParseRDFPatternMember(((DataRow)resultRows.Current)["?OBJECT"].ToString());
                if (obj is RDFResource)
                    result.AddTriple(new RDFTriple((RDFResource)subj, (RDFResource)pred, (RDFResource)obj));
                else
                    result.AddTriple(new RDFTriple((RDFResource)subj, (RDFResource)pred, (RDFLiteral)obj));
            }

            return result;
        }

        /// <summary>
        /// Asynchronously gets a graph corresponding to the query result
        /// </summary>
        public Task<RDFGraph> ToRDFGraphAsync()
            => Task.Run(() => ToRDFGraph());

        /// <summary>
        /// Gets a memory store corresponding to the query result
        /// </summary>
        public RDFMemoryStore ToRDFMemoryStore()
        {
            RDFMemoryStore result = new RDFMemoryStore();
            RDFPatternMember ctx = null;
            RDFPatternMember subj = null;
            RDFPatternMember pred = null;
            RDFPatternMember obj = null;

            //Prepare context data
            bool hasCtx = DescribeResults.Columns.Contains("?CONTEXT");
            RDFContext defCtx = new RDFContext();

            //Iterate the datatable rows and generate the corresponding triples to be added to the result memory store
            IEnumerator resultRows = DescribeResults.Rows.GetEnumerator();
            while (resultRows.MoveNext())
            {
                //In case the context column is unbound, we can safely apply default context
                if (!hasCtx || string.IsNullOrEmpty(((DataRow)resultRows.Current)["?CONTEXT"].ToString()))
                    ctx = defCtx;
                else
                    ctx = new RDFContext(RDFQueryUtilities.ParseRDFPatternMember(((DataRow)resultRows.Current)["?CONTEXT"].ToString()).ToString());
                subj = RDFQueryUtilities.ParseRDFPatternMember(((DataRow)resultRows.Current)["?SUBJECT"].ToString());
                pred = RDFQueryUtilities.ParseRDFPatternMember(((DataRow)resultRows.Current)["?PREDICATE"].ToString());
                obj = RDFQueryUtilities.ParseRDFPatternMember(((DataRow)resultRows.Current)["?OBJECT"].ToString());
                if (obj is RDFResource)
                    result.AddQuadruple(new RDFQuadruple((RDFContext)ctx, (RDFResource)subj, (RDFResource)pred, (RDFResource)obj));
                else
                    result.AddQuadruple(new RDFQuadruple((RDFContext)ctx, (RDFResource)subj, (RDFResource)pred, (RDFLiteral)obj));
            }

            return result;
        }

        /// <summary>
        /// Asynchronously gets a memory store corresponding to the query result
        /// </summary>
        public Task<RDFMemoryStore> ToRDFMemoryStoreAsync()
            => Task.Run(() => ToRDFMemoryStore());

        /// <summary>
        /// Gets a query result corresponding to the given graph
        /// </summary>
        public static RDFDescribeQueryResult FromRDFGraph(RDFGraph graph)
        {
            RDFDescribeQueryResult result = new RDFDescribeQueryResult();
            if (graph != null)
            {
                //Transform the graph into a datatable and assign it to the query result
                result.DescribeResults = graph.ToDataTable();
            }
            return result;
        }

        /// <summary>
        /// Asynchronously gets a query result corresponding to the given graph
        /// </summary>
        public static Task<RDFDescribeQueryResult> FromRDFGraphAsync(RDFGraph graph)
            => Task.Run(() => FromRDFGraph(graph));

        /// <summary>
        /// Gets a query result corresponding to the given memory store
        /// </summary>
        public static RDFDescribeQueryResult FromRDFMemoryStore(RDFMemoryStore store)
        {
            RDFDescribeQueryResult result = new RDFDescribeQueryResult();
            if (store != null)
            {
                //Transform the memory store into a datatable and assign it to the query result
                result.DescribeResults = store.ToDataTable();
            }
            return result;
        }

        /// <summary>
        /// Asynchronously gets a query result corresponding to the given memory store
        /// </summary>
        public static Task<RDFDescribeQueryResult> FromRDFMemoryStoreAsync(RDFMemoryStore store)
            => Task.Run(() => FromRDFMemoryStore(store));
        #endregion
    }
}