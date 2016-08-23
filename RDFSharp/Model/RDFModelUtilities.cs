﻿/*
   Copyright 2012-2016 Marco De Salvo

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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace RDFSharp.Model 
{

    /// <summary>
    /// RDFModelUtilities is a collector of reusable utility methods for RDF model management
    /// </summary>
    internal static class RDFModelUtilities {

        #region Greta
        /// <summary>
        /// Performs MD5 hash calculation of the given string
        /// </summary>
        internal static Int64 CreateHash(String input) {
            if (input != null) {
                var md5Encryptor   = new MD5CryptoServiceProvider();
                var inputBytes     = Encoding.UTF8.GetBytes(input);
                var hashBytes      = md5Encryptor.ComputeHash(inputBytes);
                return BitConverter.ToInt64(hashBytes, 0);
            }
            throw new RDFModelException("Cannot create hash because given \"input\" string parameter is null.");
        }
        #endregion

        #region Strings
        /// <summary>
        /// Regex to catch 8-byte unicodes
        /// </summary>
        internal static readonly Regex regexU8 = new Regex(@"\\U([0-9A-Fa-f]{8})", RegexOptions.Compiled);
        /// <summary>
        /// Regex to catch 4-byte unicodes
        /// </summary>
        internal static readonly Regex regexU4 = new Regex(@"\\u([0-9A-Fa-f]{4})", RegexOptions.Compiled);

        /// <summary>
        /// Gets the string representation of the given term status
        /// </summary>
        internal static String GetTermStatus(RDFModelEnums.RDFTermStatus termStatus) {
            switch (termStatus) {
                case RDFModelEnums.RDFTermStatus.Stable:
                    return "stable";
                case RDFModelEnums.RDFTermStatus.Unstable:
                    return "unstable";
                case RDFModelEnums.RDFTermStatus.Testing:
                    return "testing";
                case RDFModelEnums.RDFTermStatus.Archaic:
                    return "archaic";

                default:
                    return "stable";
            }
        }

        /// <summary>
        /// Gets the Uri corresponding to the given string
        /// </summary>
        internal static Uri GetUriFromString(String uriString) {
            Uri tempUri       = null;
            if (uriString    != null && uriString.Trim() != String.Empty) {

                // blanks detection
                if (uriString.StartsWith("_:")) {
                    uriString = "bnode:" + uriString.Substring(2);
                }

				Uri.TryCreate(uriString, UriKind.Absolute, out tempUri);
            }
            return tempUri;
        }

        /// <summary>
        /// Generates a new Uri for a blank resource.
        /// It starts by default with "bnode:".
        /// </summary>
        internal static Uri GenerateAnonUri() {
            return new Uri("bnode:" + Guid.NewGuid());
        }

        /// <summary>
        /// Turns back ASCII-encoded Unicodes into Unicodes. 
        /// </summary>
        internal static String ASCII_To_Unicode(String asciiString) {
            if(asciiString != null) {
                asciiString = regexU8.Replace(asciiString, match => ((Char)Int64.Parse(match.Groups[1].Value, NumberStyles.HexNumber)).ToString(CultureInfo.InvariantCulture));
                asciiString = regexU4.Replace(asciiString, match => ((Char)Int32.Parse(match.Groups[1].Value, NumberStyles.HexNumber)).ToString(CultureInfo.InvariantCulture));
            }
            return asciiString;
        }

        /// <summary>
        /// Turns Unicodes into ASCII-encoded Unicodes. 
        /// </summary>
        internal static String Unicode_To_ASCII(String unicodeString) {
            if (unicodeString   != null) {
                StringBuilder b  = new StringBuilder();
                foreach (Char c in unicodeString.ToCharArray()) {
                    if (c       <= 127) {
                        b.Append(c);
                    }
                    else {
                        if (c   <= 65535) {
                            b.Append("\\u" + ((Int32)c).ToString("X4"));
                        }
                        else {
                            b.Append("\\U" + ((Int32)c).ToString("X8"));
                        }
                    }
                }
                unicodeString    = b.ToString();
            }
            return unicodeString;
        }
        #endregion

        #region Graph
        /// <summary>
        /// Rebuild the metadata of the given graph
        /// </summary>
        internal static void RebuildGraph(RDFGraph graph) {
            var triples  = new Dictionary<Int64, RDFTriple>(graph.Triples);
            graph.ClearTriples();
            foreach (var t in triples) {
                graph.AddTriple(t.Value);
            }
        }

        /// <summary>
        /// Selects the triples corresponding to the given pattern from the given graph
        /// </summary>
        internal static List<RDFTriple> SelectTriples(RDFGraph graph,  RDFResource subj, 
                                                                       RDFResource pred, 
                                                                       RDFResource obj, 
                                                                       RDFLiteral  lit) {
            var matchSubj        = new List<RDFTriple>();
            var matchPred        = new List<RDFTriple>();
            var matchObj         = new List<RDFTriple>();
            var matchLit         = new List<RDFTriple>();
            var matchResult      = new List<RDFTriple>();
            if (graph           != null) {
                
                //Filter by Subject
                if (subj        != null) {
                    foreach (var t in graph.GraphIndex.SelectIndexBySubject(subj).Keys) {
                        matchSubj.Add(graph.Triples[t]);
                    }
                }

                //Filter by Predicate
                if (pred        != null) {
                    foreach (var t in graph.GraphIndex.SelectIndexByPredicate(pred).Keys) {
                        matchPred.Add(graph.Triples[t]);
                    }
                }

                //Filter by Object
                if (obj         != null) {
                    foreach (var t in graph.GraphIndex.SelectIndexByObject(obj).Keys) {
                        matchObj.Add(graph.Triples[t]);
                    }
                }

                //Filter by Literal
                if (lit         != null) {
                    foreach (var t in graph.GraphIndex.SelectIndexByLiteral(lit).Keys) {
                        matchLit.Add(graph.Triples[t]);
                    }
                }

                //Intersect the filters
                if (subj                   != null) {
                    if (pred               != null) {
                        if (obj            != null) {
                            //S->P->O
                            matchResult     = matchSubj.Intersect(matchPred)
                                                       .Intersect(matchObj)
                                                       .ToList<RDFTriple>();
                        }
                        else {
                            if (lit        != null) {
                                //S->P->L
                                matchResult = matchSubj.Intersect(matchPred)
                                                       .Intersect(matchLit)
                                                       .ToList<RDFTriple>();
                            }
                            else {
                                //S->P->
                                matchResult = matchSubj.Intersect(matchPred)
                                                       .ToList<RDFTriple>();
                            }
                        }
                    }
                    else {
                        if (obj            != null) {
                            //S->->O
                            matchResult     = matchSubj.Intersect(matchObj)
                                                       .ToList<RDFTriple>();
                        }
                        else {
                            if (lit        != null) {
                                //S->->L
                                matchResult = matchSubj.Intersect(matchLit)
                                                       .ToList<RDFTriple>();
                            }
                            else {
                                //S->->
                                matchResult = matchSubj;
                            }
                        }
                    }
                }
                else {
                    if (pred               != null) {
                        if (obj            != null) {
                            //->P->O
                            matchResult     = matchPred.Intersect(matchObj)
                                                       .ToList<RDFTriple>();
                        }
                        else {
                            if (lit        != null) {
                                //->P->L
                                matchResult = matchPred.Intersect(matchLit)
                                                       .ToList<RDFTriple>();
                            }
                            else {
                                //->P->
                                matchResult = matchPred;
                            }
                        }
                    }
                    else {
                        if (obj            != null) {
                            //->->O
                            matchResult     = matchObj;
                        }
                        else {
                            if (lit        != null) {
                                //->->L
                                matchResult = matchLit;
                            }
                            else {
                                //->->
                                matchResult = graph.Triples.Values.ToList<RDFTriple>();
                            }
                        }
                    }
                }

            }
            return matchResult;
        }
        #endregion

        #region RDFNamespace
        /// <summary>
        /// Looksup the given prefix or namespace into the prefix.cc service
        /// </summary>
        internal static RDFNamespace LookupPrefixCC(String data, Int32 lookupMode) {
            var lookupString       = (lookupMode == 1 ? "http://prefix.cc/" + data + ".file.txt" :
                                                        "http://prefix.cc/reverse?uri=" + data + "&format=txt");

            using (var webclient   = new WebClient()) {
                try {
                    var response   = webclient.DownloadString(lookupString);
                    var new_prefix = response.Split('\t')[0];
                    var new_nspace = response.Split('\t')[1].TrimEnd(new Char[] { '\n' });
                    var result     = new RDFNamespace(new_prefix, new_nspace);

                    //Also add the namespace to the register, to avoid future lookups
                    RDFNamespaceRegister.AddNamespace(result);

                    //Return the found result
                    return result;
                }
                catch  (WebException wex) {
                    if (wex.Message.Contains("404")) {
                        return null;
                    }
                    else {
                        throw new RDFModelException("Cannot retrieve data from prefix.cc service because: " + wex.Message, wex);
                    }
                }
                catch(Exception ex) {
                    throw new RDFModelException("Cannot retrieve data from prefix.cc service because: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Finds if the given token contains a recognizable namespace and, if so, abbreviates it with its prefix.
        /// It also prepares the result in a format useful for serialization (it's used by Turtle writer).
        /// </summary>
        internal static String AbbreviateNamespace(String token) {

            //Null or Space token: it's a trick, give empty result
            if (token == null || token.Trim() == String.Empty) {
                return String.Empty;
            }
            //Blank token: abbreviate it with "_"
            if (token.StartsWith("bnode:")) {
                return token.Replace("bnode:", "_:");
            }
            //Prefixed token: check if it starts with a known prefix, if so just return it
            if (RDFNamespaceRegister.GetByPrefix(token.Split(':')[0]) != null) {
                return token;
            }

            //Uri token: search a known namespace, if found replace it with its prefix
            Boolean abbreviationDone     = false;
            RDFNamespaceRegister.Instance.Register.ForEach(ns => {
                if (!abbreviationDone) {
                    String nS            = ns.ToString();
                    if (token.Contains(nS)) {
                        token            = token.Replace(nS, ns.Prefix + ":").TrimEnd(new Char[] { '/' });
                        abbreviationDone = true;
                    }
                }
            });

            //Search done, let's analyze results:
            if (abbreviationDone) {
                return token; //token is a relative or a blank uri
            }
            if (token.Contains("^^")) { //token is a typedLiteral absolute uri
                return token.Replace("^^", "^^<") + ">";
            }
            return "<" + token + ">"; //token is an absolute uri

        }

        /// <summary>
        /// Generates an automatic prefix for a namespace
        /// </summary>
        internal static RDFNamespace GenerateNamespace(String namespaceString, Boolean isDatatypeNamespace) {
            if (namespaceString    != null && namespaceString.Trim() != String.Empty) {
                
                //Extract the prefixable part from the Uri
                Uri uriNS           = GetUriFromString(namespaceString);
                if (uriNS          == null) {
                    throw new RDFModelException("Cannot create RDFNamespace because given \"namespaceString\" (" + namespaceString + ") parameter cannot be converted to a valid Uri");
                }
                String type         = null;
                String ns           = uriNS.AbsoluteUri;

                // e.g.:  "http://www.w3.org/2001/XMLSchema#integer"
                if (uriNS.Fragment != String.Empty) {
                    type            = uriNS.Fragment.Replace("#", String.Empty);  //"integer"
                    if (type       != String.Empty) {
                        ns          = ns.TrimEnd(type.ToCharArray());             //"http://www.w3.org/2001/XMLSchema#"
                    }
                }
                else {
                    // e.g.:  "http://example.org/integer"
                    if (uriNS.LocalPath != "/") {
                        if (!isDatatypeNamespace) {
                            ns      = ns.TrimEnd(uriNS.Segments[uriNS.Segments.Length-1].ToCharArray());
                        }
                    }
                }

                //Check if a namespace with the extracted Uri is in the register, or generate an automatic one
                return (RDFNamespaceRegister.GetByNamespace(ns) ?? new RDFNamespace("autoNS", ns));

            }
            throw new RDFModelException("Cannot create RDFNamespace because given \"namespaceString\" parameter is null or empty");
        }
        #endregion

        #region RDFDatatype
        /// <summary>
        /// Tries to parse the given string in order to build the corresponding datatype
        /// </summary>
        internal static RDFDatatype GetDatatypeFromString(String datatypeString) {
            if (datatypeString     != null && datatypeString.Trim() != String.Empty) {
                Uri datatypeUri     = GetUriFromString(datatypeString);
                if (datatypeUri    == null) {
                    throw new RDFModelException("Cannot create RDFDatatype because given \"datatypeString\" (" + datatypeString + ") parameter cannot be converted to a valid Uri");
                }
                String type         = null;
                String ns           = null;
                RDFDatatype dt      = null;

                // e.g.:  "http://www.w3.org/2001/XMLSchema#integer"
                if (datatypeUri.Fragment != String.Empty) {
                    type            = datatypeUri.Fragment.TrimStart(new Char[] { '#' });    //"integer"
                    ns              = datatypeUri.AbsoluteUri.TrimEnd(type.ToCharArray());   //"http://www.w3.org/2001/XMLSchema#"
                }
                // e.g.:  "http://example.org/integer" or "ex:integer"
                else {
                    type            = datatypeUri.Segments[datatypeUri.Segments.Length - 1]; //"integer"
                    ns              = datatypeUri.AbsoluteUri.TrimEnd(type.ToCharArray());   //"http://example.org/" or "ex:"
                }

                //First try to search the register for prefix and datatype
                if (ns.EndsWith(":")) {
                    ns              = ns.TrimEnd(':');
                    dt              = RDFDatatypeRegister.GetByPrefixAndDatatype(ns, type);
                }

                //If nothing found, try to search the register for namespace and datatype
                if(dt              == null) {
                    dt              = RDFDatatypeRegister.GetByNamespaceAndDatatype(ns, type);
                    
                    //If nothing found, we must create and register a new datatype
                    if (dt         == null) {

                        //First try to find a namespace to work with
                        var nSpace  = (RDFNamespaceRegister.GetByNamespace(ns) ?? GenerateNamespace(ns, true));

                        //If nothing found, we also have to create a new datatype
                        dt          = new RDFDatatype(nSpace.Prefix, nSpace.Namespace, type, RDFModelEnums.RDFDatatypeCategory.String);

                    }
                }

                return dt;
            }
            throw new RDFModelException("Cannot create RDFDatatype because given \"datatypeString\" parameter is null or empty");
        }

        /// <summary>
        /// Validates the value of the given typed literal against the category of its datatype
        /// </summary>
        internal static Boolean ValidateTypedLiteral(RDFTypedLiteral typedLiteral) {
            if (typedLiteral != null) {
                Boolean validateResponse = true;
                switch (typedLiteral.Datatype.Category) {

                    #region STRING CATEGORY
                    case RDFModelEnums.RDFDatatypeCategory.String:

                        //ANYURI
                        if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "anyURI"))) {
                            Uri outUri;
                            if (!Uri.TryCreate(typedLiteral.Value, UriKind.Absolute, out outUri)) {
                                 validateResponse = false;
                            }
                        }

                        //XML_LITERAL
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.RDF.PREFIX, "XMLLiteral"))) {
                            try {
                                XDocument.Parse(typedLiteral.Value);
                            }
                            catch {
                                validateResponse = false;
                            }
                        }

                        //NAME
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "Name"))) {
                            try {
                                XmlConvert.VerifyName(typedLiteral.Value);
                            }
                            catch {
                                validateResponse = false;
                            }
                        }

                        //NCNAME
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "NCName"))) {
                            try {
                                XmlConvert.VerifyNCName(typedLiteral.Value);
                            }
                            catch {
                                validateResponse = false;
                            }
                        }

                        //TOKEN
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "token"))) {
                            try {
                                XmlConvert.VerifyTOKEN(typedLiteral.Value);
                            }
                            catch {
                                validateResponse = false;
                            }
                        }

                        //NMTOKEN
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "NMToken"))) {
                            try {
                                XmlConvert.VerifyNMTOKEN(typedLiteral.Value);
                            }
                            catch {
                                validateResponse = false;
                            }
                        }

                        //NORMALIZED_STRING
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "normalizedString"))) {
                             if (typedLiteral.Value.Contains('\r') || typedLiteral.Value.Contains('\n') || typedLiteral.Value.Contains('\t')) {
                                 validateResponse = false;
                             }
                        }

                        //LANGUAGE
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "language"))) {
                             if (!Regex.IsMatch(typedLiteral.Value, "^[a-zA-Z]+([\\-][a-zA-Z0-9]+)*$")) {
                                  validateResponse = false;
                             }
                        }

                        //BASE64_BINARY
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "base64Binary"))) {
                             try {
                                 Convert.FromBase64String(typedLiteral.Value);
                             }
                             catch {
                                 validateResponse = false;
                             }
                        }

                        //HEX_BINARY
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "hexBinary"))) {
                             if ((typedLiteral.Value.Length % 2 != 0) || (!Regex.IsMatch(typedLiteral.Value, @"^[a-fA-F0-9]+$"))) {
                                 validateResponse = false;
                             }
                        }

                        break;
                    #endregion

                    #region BOOLEAN CATEGORY
                    case RDFModelEnums.RDFDatatypeCategory.Boolean:

                        Boolean outBool;
                        if (Boolean.TryParse(typedLiteral.Value, out outBool)) {
                            typedLiteral.Value = (outBool ? "true" : "false");
                        }
                        else {

                            //XSD:BOOLEAN MUST ALSO SUPPORT 1/0 BOOLEAN REPRESENTATION
                            //WHICH IS EVENTUALLY CONVERTED TO STANDARD TRUE/FALSE
                            if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "boolean"))) {
                                if (typedLiteral.Value.Equals("1")) {
                                    typedLiteral.Value = "true";
                                }
                                else if(typedLiteral.Value.Equals("0")) {
                                    typedLiteral.Value = "false";
                                }
                                else {
                                    validateResponse = false;
                                }
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        break;
                    #endregion

                    #region DATETIME CATEGORY
                    case RDFModelEnums.RDFDatatypeCategory.DateTime:

                        //DATETIME
                        if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "dateTime"))) {
                            try {
                                DateTime.ParseExact(typedLiteral.Value, "yyyy-MM-ddTHH:mm:ss.FFFK", CultureInfo.InvariantCulture);
                            }
                            catch {
                                try {
                                    DateTime.ParseExact(typedLiteral.Value, "yyyy-MM-ddTHH:mm:ss.FFF", CultureInfo.InvariantCulture);
                                }
                                catch {
                                    try {
                                        DateTime.ParseExact(typedLiteral.Value, "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture);
                                    }
                                    catch {
                                        try {
                                            DateTime.ParseExact(typedLiteral.Value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                                        }
                                        catch {
                                            validateResponse = false;
                                        }
                                    }
                                }
                            }
                        }

                        //DATE
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "date"))) {
                            try {
                                DateTime.ParseExact(typedLiteral.Value, "yyyy-MM-ddK", CultureInfo.InvariantCulture);
                            }
                            catch {
                                try {
                                    DateTime.ParseExact(typedLiteral.Value, "yyyy-MM-dd",  CultureInfo.InvariantCulture);
                                }
                                catch {
                                    validateResponse = false;
                                }
                            }
                        }

                        //TIME
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "time"))) {
                            try {
                                DateTime.ParseExact(typedLiteral.Value, "HH:mm:ss.FFFK", CultureInfo.InvariantCulture);
                            }
                            catch {
                                try {
                                    DateTime.ParseExact(typedLiteral.Value, "HH:mm:ss.FFF", CultureInfo.InvariantCulture);
                                }
                                catch {
                                    try {
                                        DateTime.ParseExact(typedLiteral.Value, "HH:mm:ssK", CultureInfo.InvariantCulture);
                                    }
                                    catch {
                                        try {
                                            DateTime.ParseExact(typedLiteral.Value, "HH:mm:ss", CultureInfo.InvariantCulture);
                                        }
                                        catch {
                                            validateResponse = false;
                                        }
                                    }
                                }
                            }
                        }

                        //G_MONTH_DAY
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "gMonthDay"))) {
                            try {
                                DateTime.ParseExact(typedLiteral.Value, "--MM-ddK", CultureInfo.InvariantCulture);
                            }
                            catch {
                                try {
                                    DateTime.ParseExact(typedLiteral.Value, "--MM-dd",  CultureInfo.InvariantCulture);
                                }
                                catch {
                                    validateResponse = false;
                                }
                            }
                        }

                        //G_YEAR_MONTH
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "gYearMonth"))) {
                            try {
                                DateTime.ParseExact(typedLiteral.Value, "yyyy-MMK", CultureInfo.InvariantCulture);
                            }
                            catch {
                                try {
                                    DateTime.ParseExact(typedLiteral.Value, "yyyy-MM",  CultureInfo.InvariantCulture);
                                }
                                catch {
                                    validateResponse = false;
                                }
                            }
                        }

						//G_YEAR
						else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "gYear"))) {
							try {
                                DateTime.ParseExact(typedLiteral.Value, "yyyyK", CultureInfo.InvariantCulture);
                            }
                            catch {
                                try {
                                    DateTime.ParseExact(typedLiteral.Value, "yyyy",  CultureInfo.InvariantCulture);
                                }
                                catch {
                                    validateResponse = false;
                                }
                            }
						}
						
						//G_MONTH
						else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "gMonth"))) {
							try {
                                DateTime.ParseExact(typedLiteral.Value, "MMK", CultureInfo.InvariantCulture);
                            }
                            catch {
                                try {
                                    DateTime.ParseExact(typedLiteral.Value, "MM",  CultureInfo.InvariantCulture);
                                }
                                catch {
                                    validateResponse = false;
                                }
                            }
						}
						
						//G_DAY
						else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "gDay"))) {
							try {
                                DateTime.ParseExact(typedLiteral.Value, "ddK", CultureInfo.InvariantCulture);
                            }
                            catch {
                                try {
                                    DateTime.ParseExact(typedLiteral.Value, "dd",  CultureInfo.InvariantCulture);
                                }
                                catch {
                                    validateResponse = false;
                                }
                            }
						}

                        //OTHER
                        else {
                            DateTime dateTime;
                            if (DateTime.TryParse(typedLiteral.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
                                typedLiteral.Value = dateTime.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        break;
                    #endregion

                    #region TIMESPAN CATEGORY
                    case RDFModelEnums.RDFDatatypeCategory.TimeSpan:

                        try {
                            XmlConvert.ToTimeSpan(typedLiteral.Value);
                        }
                        catch {
                            validateResponse = false;
                        }

                        break;
                    #endregion

                    #region NUMERIC CATEGORY
                    case RDFModelEnums.RDFDatatypeCategory.Numeric:

                        //DECIMAL
                        if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "decimal"))) {
                            Decimal outDecimal;
                            if (Decimal.TryParse(typedLiteral.Value, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out outDecimal)) {
                                typedLiteral.Value = outDecimal.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //DOUBLE
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "double"))) {
                            Double outDouble;
                            if (Double.TryParse(typedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out outDouble)) {
                                typedLiteral.Value = outDouble.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //FLOAT
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "float"))) {
                            Single outFloat;
                            if (Single.TryParse(typedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out outFloat)) {
                                typedLiteral.Value = outFloat.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //INTEGER
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "integer"))) {
                            Decimal outInteger;
                            if (Decimal.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outInteger)) {
                                typedLiteral.Value = outInteger.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //LONG
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "long"))) {
                            Int64 outLong;
                            if (Int64.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outLong)) {
                                typedLiteral.Value = outLong.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //INT
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "int"))) {
                            Int32 outInt;
                            if (Int32.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outInt)) {
                                typedLiteral.Value = outInt.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //SHORT
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "short"))) {
                            Int16 outShort;
                            if (Int16.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outShort)) {
                                typedLiteral.Value = outShort.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //BYTE
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "byte"))) {
                            SByte outSByte;
                            if (SByte.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outSByte)) {
                                typedLiteral.Value = outSByte.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //UNSIGNED LONG
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "unsignedLong"))) {
                            UInt64 outULong;
                            if (UInt64.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outULong)) {
                                typedLiteral.Value = outULong.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //UNSIGNED INT
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "unsignedInt"))) {
                            UInt32 outUInt;
                            if (UInt32.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outUInt)) {
                                typedLiteral.Value = outUInt.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //UNSIGNED SHORT
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "unsignedShort"))) {
                            UInt16 outUShort;
                            if (UInt16.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outUShort)) {
                                typedLiteral.Value = outUShort.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //UNSIGNED BYTE
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "unsignedByte"))) {
                            Byte outByte;
                            if (Byte.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outByte)) {
                                typedLiteral.Value = outByte.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //NON-POSITIVE INTEGER [Decimal.MinValue, 0]
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "nonPositiveInteger"))) {
                            Decimal outNPInteger;
                            if (Decimal.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outNPInteger)) {
                                typedLiteral.Value = outNPInteger.ToString(CultureInfo.InvariantCulture);
                                if (outNPInteger > 0) {
                                    validateResponse = false;
                                }
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //NEGATIVE INTEGER [Decimal.MinValue, -1]
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "negativeInteger"))) {
                            Decimal outNInteger;
                            if (Decimal.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outNInteger)) {
                                typedLiteral.Value = outNInteger.ToString(CultureInfo.InvariantCulture);
                                if (outNInteger > -1) {
                                    validateResponse = false;
                                }
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //NON-NEGATIVE INTEGER [0, Decimal.MaxValue]
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "nonNegativeInteger"))) {
                            Decimal outNNInteger;
                            if (Decimal.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outNNInteger)) {
                                typedLiteral.Value = outNNInteger.ToString(CultureInfo.InvariantCulture);
                                if (outNNInteger < 0) {
                                    validateResponse = false;
                                }
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //POSITIVE INTEGER [1, Decimal.MaxValue]
                        else if (typedLiteral.Datatype.Equals(RDFDatatypeRegister.GetByPrefixAndDatatype(RDFVocabulary.XSD.PREFIX, "positiveInteger"))) {
                            Decimal outPInteger;
                            if (Decimal.TryParse(typedLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out outPInteger)) {
                                typedLiteral.Value = outPInteger.ToString(CultureInfo.InvariantCulture);
                                if (outPInteger < 1) {
                                    validateResponse = false;
                                }
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        //OTHER
                        else {
                            Decimal outDecimal;
                            if (Decimal.TryParse(typedLiteral.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out outDecimal)) {
                                typedLiteral.Value = outDecimal.ToString(CultureInfo.InvariantCulture);
                            }
                            else {
                                validateResponse = false;
                            }
                        }

                        break;
                    #endregion

                }
                return validateResponse;
            }
            throw new RDFModelException("Cannot validate RDFTypedLiteral because given \"typedLiteral\" parameter is null.");
        }
        #endregion

    }

}