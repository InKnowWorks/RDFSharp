﻿/*
   Copyright 2012-2021 Marco De Salvo

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDFSharp.Test.Model
{
    [TestClass]
    public class RDFValidationHelperTest
    {
        #region Properties
        private RDFGraph dataGraph;
        #endregion

        #region Ctors
        [TestInitialize]
        public void Initialize()
        {
            dataGraph = new RDFGraph();
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:Person")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Alice"), RDFVocabulary.FOAF.NAME, new RDFPlainLiteral("Alice")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Alice"), RDFVocabulary.FOAF.AGE, new RDFTypedLiteral("14", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Alice"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Bob")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:Person")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Bob"), RDFVocabulary.FOAF.NAME, new RDFPlainLiteral("Bob", "en")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Bob"), RDFVocabulary.FOAF.AGE, new RDFTypedLiteral("17", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Bob"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Jane")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Jane"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:Person")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Jane"), RDFVocabulary.FOAF.NAME, new RDFTypedLiteral("Jane", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Jane"), RDFVocabulary.FOAF.AGE, new RDFTypedLiteral("11", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Jane"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Steve")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Steve"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:Human")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Steve"), RDFVocabulary.FOAF.AGE, new RDFTypedLiteral("12", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Steve"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Alice")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Steve"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Bob")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Steve"), RDFVocabulary.FOAF.AGENT, new RDFResource("ex:Carl")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Carl"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:Man")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Carl"), RDFVocabulary.FOAF.NAME, new RDFPlainLiteral("Carl", "en-us")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Carl"), RDFVocabulary.FOAF.NICK, new RDFPlainLiteral("Carl", "en")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Carl"), RDFVocabulary.FOAF.OPEN_ID, new RDFPlainLiteral("Carl")));

            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Person"), RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:Human")));
            dataGraph.AddTriple(new RDFTriple(new RDFResource("ex:Guy"), RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:Person")));
        }
        #endregion

        #region Tests
        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithoutTargets()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithoutTargets()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.AGE);
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetClass()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Person")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 3);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetClass()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Person")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 3);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetClassAndReasoning()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Human")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetClassAndReasoning()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Human")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetClassNoInstances()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Guy")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetClassNoInstances()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Guy")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetClassUnexisting()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:People")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetClassUnexisting()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:People")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetNode()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetNode(new RDFResource("ex:Alice")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 1);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetNode()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetNode(new RDFResource("ex:Alice")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 1);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetSubjectsOf()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.AGE));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetSubjectsOf()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.AGE));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetSubjectsOfUnexisting()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.BASED_NEAR));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetSubjectsOfUnexisting()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.BASED_NEAR));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetObjectsOf()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.KNOWS));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetObjectsOf()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.AGE)
                                    .AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.KNOWS));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNodeShapeWithTargetObjectsOfUnexisting()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.MBOX));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfPropertyShapeWithTargetObjectsOfUnexisting()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.MBOX));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNullShape()
        {
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, null);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetFocusNodesOfNullDataGraph()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(null, nShape);

            Assert.IsNotNull(focusNodes);
            Assert.IsTrue(focusNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithoutTargets()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithoutTargets()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.AGE);
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetClass()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Person")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 3);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetClass()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Person")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 3);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetClassAndReasoning()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Human")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetClassAndReasoning()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Human")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 5);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetClassNoInstances()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Guy")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetClassNoInstances()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:Guy")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetClassUnexisting()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:People")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetClassUnexisting()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetClass(new RDFResource("ex:People")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetNode()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetNode(new RDFResource("ex:Alice")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 1);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetNode()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetNode(new RDFResource("ex:Alice")));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 1);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetSubjectsOf()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.AGE));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetSubjectsOf()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.AGE));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 5);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetSubjectsOfUnexisting()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.BASED_NEAR));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetSubjectsOfUnexisting()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.BASED_NEAR));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetObjectsOf()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.KNOWS));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetObjectsOf()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.AGE)
                                    .AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.KNOWS));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 4);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNodeShapeWithTargetObjectsOfUnexisting()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"))
                                    .AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.MBOX));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, nShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfPropertyShapeWithTargetObjectsOfUnexisting()
        {
            RDFShape pShape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS)
                                    .AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.MBOX));
            List<RDFPatternMember> focusNodes = RDFValidationHelper.GetFocusNodesOf(dataGraph, pShape);
            List<RDFPatternMember> valueNodes = new List<RDFPatternMember>();
            focusNodes.ForEach(focusNode => valueNodes.AddRange(RDFValidationHelper.GetValueNodesOf(dataGraph, pShape, focusNode)));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNullShape()
        {
            List<RDFPatternMember> valueNodes = RDFValidationHelper.GetValueNodesOf(dataGraph, null, new RDFResource("ex:Alice"));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNullDataGraph()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"));
            List<RDFPatternMember> valueNodes = RDFValidationHelper.GetValueNodesOf(null, nShape, new RDFResource("ex:Alice"));

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetValueNodesOfNullFocusNode()
        {
            RDFShape nShape = new RDFNodeShape(new RDFResource("ex:nodeShape"));
            List<RDFPatternMember> valueNodes = RDFValidationHelper.GetValueNodesOf(dataGraph, nShape, null);

            Assert.IsNotNull(valueNodes);
            Assert.IsTrue(valueNodes.Count == 0);
        }

        [TestMethod]
        public void ShouldGetInstancesOfClass()
        {
            List<RDFPatternMember> persons = RDFValidationHelper.GetInstancesOfClass(dataGraph, new RDFResource("ex:Person"));

            Assert.IsNotNull(persons);
            Assert.IsTrue(persons.Count == 3);
        }

        [TestMethod]
        public void ShouldGetInstancesOfClassWithReasoning()
        {
            List<RDFPatternMember> persons = RDFValidationHelper.GetInstancesOfClass(dataGraph, new RDFResource("ex:Human"));

            Assert.IsNotNull(persons);
            Assert.IsTrue(persons.Count == 4);
        }

        [TestMethod]
        public void ShouldGetInstancesOfUnreferencedClass()
        {
            List<RDFPatternMember> persons = RDFValidationHelper.GetInstancesOfClass(dataGraph, new RDFResource("ex:Guy"));

            Assert.IsNotNull(persons);
            Assert.IsTrue(persons.Count == 0);
        }

        [TestMethod]
        public void ShouldGetInstancesOfUnexistingClass()
        {
            List<RDFPatternMember> persons = RDFValidationHelper.GetInstancesOfClass(dataGraph, new RDFResource("ex:People"));

            Assert.IsNotNull(persons);
            Assert.IsTrue(persons.Count == 0);
        }

        [TestMethod]
        public void ShouldGetInstancesOfNullClass()
        {
            List<RDFPatternMember> persons = RDFValidationHelper.GetInstancesOfClass(dataGraph, null);

            Assert.IsNotNull(persons);
            Assert.IsTrue(persons.Count == 0);
        }

        [TestMethod]
        public void ShouldGetInstancesOfClassFromNullDataGraph()
        {
            List<RDFPatternMember> persons = RDFValidationHelper.GetInstancesOfClass(null, new RDFResource("ex:Person"));

            Assert.IsNotNull(persons);
            Assert.IsTrue(persons.Count == 0);
        }

        [TestMethod]
        public void ShouldParseFromNullGraph()
        {
            RDFShapesGraph shapesGraph = RDFValidationHelper.FromRDFGraph(null);

            Assert.IsNull(shapesGraph);
        }

        [DataTestMethod]
        [DataRow(RDFValidationEnums.RDFShapeSeverity.Violation)]
        [DataRow(RDFValidationEnums.RDFShapeSeverity.Warning)]
        [DataRow(RDFValidationEnums.RDFShapeSeverity.Info)]
        public void ShouldParseNodeShapeFromGraph(RDFValidationEnums.RDFShapeSeverity severity)
        {
            RDFNodeShape shape = new RDFNodeShape(new RDFResource("ex:nodeShape"));

            //Attributes
            shape.SetSeverity(severity);
            shape.AddMessage(new RDFPlainLiteral("message", "en-US"));

            //Targets
            shape.AddTarget(new RDFTargetClass(new RDFResource("ex:Person")));
            shape.AddTarget(new RDFTargetNode(new RDFResource("ex:Alice")));
            shape.AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.KNOWS));
            shape.AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.KNOWS));

            //Constraints
            shape.AddConstraint(new RDFClassConstraint(new RDFResource("ex:Human")));
            shape.AddConstraint(new RDFAndConstraint().AddShape(shape));
            shape.AddConstraint(new RDFClosedConstraint(true).AddIgnoredProperty(RDFVocabulary.FOAF.KNOWS));
            shape.AddConstraint(new RDFDatatypeConstraint(RDFModelEnums.RDFDatatypes.XSD_INTEGER));
            shape.AddConstraint(new RDFDisjointConstraint(RDFVocabulary.FOAF.KNOWS));
            shape.AddConstraint(new RDFEqualsConstraint(RDFVocabulary.FOAF.KNOWS));
            shape.AddConstraint(new RDFHasValueConstraint(new RDFResource("ex:Alice")));
            shape.AddConstraint(new RDFHasValueConstraint(new RDFPlainLiteral("Alice")));
            shape.AddConstraint(new RDFInConstraint(RDFModelEnums.RDFItemTypes.Resource).AddValue(new RDFResource("ex:Alice")));
            shape.AddConstraint(new RDFInConstraint(RDFModelEnums.RDFItemTypes.Literal).AddValue(new RDFPlainLiteral("Alice")));
            shape.AddConstraint(new RDFLanguageInConstraint(new List<string>() { "en-US" }));
            shape.AddConstraint(new RDFLessThanConstraint(new RDFResource("ex:prop")));
            shape.AddConstraint(new RDFLessThanOrEqualsConstraint(new RDFResource("ex:prop")));
            shape.AddConstraint(new RDFMaxCountConstraint(2));
            shape.AddConstraint(new RDFMaxExclusiveConstraint(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            shape.AddConstraint(new RDFMaxInclusiveConstraint(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            shape.AddConstraint(new RDFMaxLengthConstraint(2));
            shape.AddConstraint(new RDFMinCountConstraint(2));
            shape.AddConstraint(new RDFMinExclusiveConstraint(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            shape.AddConstraint(new RDFMinInclusiveConstraint(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            shape.AddConstraint(new RDFMinLengthConstraint(2));

            //ShapesGraph
            RDFShapesGraph shapesGraph = new RDFShapesGraph(new RDFResource("ex:shapesGraph"));
            shapesGraph.AddShape(shape);
            RDFGraph graph = shapesGraph.ToRDFGraph();
            RDFShapesGraph shapesGraph2 = RDFValidationHelper.FromRDFGraph(graph);

            #region Asserts
            Assert.IsNotNull(shapesGraph2);
            Assert.IsTrue(shapesGraph2.Equals(new RDFResource("ex:shapesGraph")));
            Assert.IsTrue(shapesGraph2.ShapesCount == 1);
            RDFNodeShape shape2 = shapesGraph2.SelectShape("ex:nodeShape") as RDFNodeShape;
            Assert.IsNotNull(shape2);
            
            //Attributes
            Assert.IsFalse(shape2.Deactivated);
            Assert.IsTrue(shape2.Severity == severity);
            Assert.IsTrue(shape2.MessagesCount == 1);
            RDFPlainLiteral shape2Message = shape2.Messages.Single() as RDFPlainLiteral;
            Assert.IsNotNull(shape2Message);
            Assert.IsTrue(shape2Message.Equals(new RDFPlainLiteral("message", "en-US")));

            //Targets
            Assert.IsTrue(shape2.TargetsCount == 4);
            RDFTargetClass shape2TargetClass = shape2.Targets.Single(x => x is RDFTargetClass) as RDFTargetClass;
            Assert.IsNotNull(shape2TargetClass);
            Assert.IsTrue(shape2TargetClass.TargetValue.Equals(new RDFResource("ex:Person")));
            RDFTargetNode shape2TargetNode = shape2.Targets.Single(x => x is RDFTargetNode) as RDFTargetNode;
            Assert.IsNotNull(shape2TargetNode);
            Assert.IsTrue(shape2TargetNode.TargetValue.Equals(new RDFResource("ex:Alice")));
            RDFTargetSubjectsOf shape2TargetSubjectsOf = shape2.Targets.Single(x => x is RDFTargetSubjectsOf) as RDFTargetSubjectsOf;
            Assert.IsNotNull(shape2TargetSubjectsOf);
            Assert.IsTrue(shape2TargetSubjectsOf.TargetValue.Equals(RDFVocabulary.FOAF.KNOWS));
            RDFTargetObjectsOf shape2TargetObjectsOf = shape2.Targets.Single(x => x is RDFTargetObjectsOf) as RDFTargetObjectsOf;
            Assert.IsNotNull(shape2TargetObjectsOf);
            Assert.IsTrue(shape2TargetObjectsOf.TargetValue.Equals(RDFVocabulary.FOAF.KNOWS));

            //Constraints
            Assert.IsTrue(shape2.ConstraintsCount == 21);
            RDFClassConstraint shape2ClassConstraint = shape2.Constraints.Single(x => x is RDFClassConstraint) as RDFClassConstraint;
            Assert.IsNotNull(shape2ClassConstraint);
            Assert.IsTrue(shape2ClassConstraint.ClassType.Equals(new RDFResource("ex:Human")));
            RDFAndConstraint shape2AndConstraint = shape2.Constraints.Single(x => x is RDFAndConstraint) as RDFAndConstraint;
            Assert.IsNotNull(shape2AndConstraint);
            Assert.IsTrue(shape2AndConstraint.AndShapes.ContainsKey(shape2.PatternMemberID));
            RDFClosedConstraint shape2ClosedConstraint = shape2.Constraints.Single(x => x is RDFClosedConstraint) as RDFClosedConstraint;
            Assert.IsNotNull(shape2ClosedConstraint);
            Assert.IsTrue(shape2ClosedConstraint.Closed);
            Assert.IsTrue(shape2ClosedConstraint.IgnoredProperties.ContainsKey(RDFVocabulary.FOAF.KNOWS.PatternMemberID));
            RDFDatatypeConstraint shape2DatatypeConstraint = shape2.Constraints.Single(x => x is RDFDatatypeConstraint) as RDFDatatypeConstraint;
            Assert.IsNotNull(shape2DatatypeConstraint);
            Assert.IsTrue(shape2DatatypeConstraint.Datatype == RDFModelEnums.RDFDatatypes.XSD_INTEGER);
            RDFDisjointConstraint shape2DisjointConstraint = shape2.Constraints.Single(x => x is RDFDisjointConstraint) as RDFDisjointConstraint;
            Assert.IsNotNull(shape2DisjointConstraint);
            Assert.IsTrue(shape2DisjointConstraint.DisjointPredicate.Equals(RDFVocabulary.FOAF.KNOWS));
            RDFEqualsConstraint shape2EqualsConstraint = shape2.Constraints.Single(x => x is RDFEqualsConstraint) as RDFEqualsConstraint;
            Assert.IsNotNull(shape2EqualsConstraint);
            Assert.IsTrue(shape2EqualsConstraint.EqualsPredicate.Equals(RDFVocabulary.FOAF.KNOWS));
            RDFHasValueConstraint shape2HasValueConstraintRes = shape2.Constraints.Single(x => x is RDFHasValueConstraint hvc && hvc.Value is RDFResource) as RDFHasValueConstraint;
            Assert.IsNotNull(shape2HasValueConstraintRes);
            Assert.IsTrue(shape2HasValueConstraintRes.Value.Equals(new RDFResource("ex:Alice")));
            RDFHasValueConstraint shape2HasValueConstraintLit = shape2.Constraints.Single(x => x is RDFHasValueConstraint hvc && hvc.Value is RDFLiteral) as RDFHasValueConstraint;
            Assert.IsNotNull(shape2HasValueConstraintLit);
            Assert.IsTrue(shape2HasValueConstraintLit.Value.Equals(new RDFPlainLiteral("Alice")));
            RDFInConstraint shape2InConstraintRes = shape2.Constraints.Single(x => x is RDFInConstraint ic && ic.ItemType == RDFModelEnums.RDFItemTypes.Resource) as RDFInConstraint;
            Assert.IsNotNull(shape2InConstraintRes);
            Assert.IsTrue(shape2InConstraintRes.InValues.ContainsKey(new RDFResource("ex:Alice").PatternMemberID));
            RDFInConstraint shape2InConstraintLit = shape2.Constraints.Single(x => x is RDFInConstraint ic && ic.ItemType == RDFModelEnums.RDFItemTypes.Literal) as RDFInConstraint;
            Assert.IsNotNull(shape2InConstraintLit);
            Assert.IsTrue(shape2InConstraintLit.InValues.ContainsKey(new RDFPlainLiteral("Alice").PatternMemberID));
            RDFLanguageInConstraint shape2LanguageInConstraint = shape2.Constraints.Single(x => x is RDFLanguageInConstraint) as RDFLanguageInConstraint;
            Assert.IsNotNull(shape2LanguageInConstraint);
            Assert.IsTrue(shape2LanguageInConstraint.LanguageTags.Contains("EN-US"));
            RDFLessThanConstraint shape2LessThanConstraint = shape2.Constraints.Single(x => x is RDFLessThanConstraint) as RDFLessThanConstraint;
            Assert.IsNotNull(shape2LessThanConstraint);
            Assert.IsTrue(shape2LessThanConstraint.LessThanPredicate.Equals(new RDFResource("ex:prop")));
            RDFLessThanOrEqualsConstraint shape2LessThanOrEqualsConstraint = shape2.Constraints.Single(x => x is RDFLessThanOrEqualsConstraint) as RDFLessThanOrEqualsConstraint;
            Assert.IsNotNull(shape2LessThanOrEqualsConstraint);
            Assert.IsTrue(shape2LessThanOrEqualsConstraint.LessThanOrEqualsPredicate.Equals(new RDFResource("ex:prop")));
            RDFMaxCountConstraint shape2MaxCountConstraint = shape2.Constraints.Single(x => x is RDFMaxCountConstraint) as RDFMaxCountConstraint;
            Assert.IsNotNull(shape2MaxCountConstraint);
            Assert.IsTrue(shape2MaxCountConstraint.MaxCount == 2);
            RDFMaxExclusiveConstraint shape2MaxExclusiveConstraint = shape2.Constraints.Single(x => x is RDFMaxExclusiveConstraint) as RDFMaxExclusiveConstraint;
            Assert.IsNotNull(shape2MaxExclusiveConstraint);
            Assert.IsTrue(shape2MaxExclusiveConstraint.Value.Equals(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFMaxInclusiveConstraint shape2MaxInclusiveConstraint = shape2.Constraints.Single(x => x is RDFMaxInclusiveConstraint) as RDFMaxInclusiveConstraint;
            Assert.IsNotNull(shape2MaxInclusiveConstraint);
            Assert.IsTrue(shape2MaxInclusiveConstraint.Value.Equals(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFMaxLengthConstraint shape2MaxLengthConstraint = shape2.Constraints.Single(x => x is RDFMaxLengthConstraint) as RDFMaxLengthConstraint;
            Assert.IsNotNull(shape2MaxLengthConstraint);
            Assert.IsTrue(shape2MaxLengthConstraint.MaxLength == 2);
            RDFMinCountConstraint shape2MinCountConstraint = shape2.Constraints.Single(x => x is RDFMinCountConstraint) as RDFMinCountConstraint;
            Assert.IsNotNull(shape2MinCountConstraint);
            Assert.IsTrue(shape2MinCountConstraint.MinCount == 2);
            RDFMinExclusiveConstraint shape2MinExclusiveConstraint = shape2.Constraints.Single(x => x is RDFMinExclusiveConstraint) as RDFMinExclusiveConstraint;
            Assert.IsNotNull(shape2MinExclusiveConstraint);
            Assert.IsTrue(shape2MinExclusiveConstraint.Value.Equals(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFMinInclusiveConstraint shape2MinInclusiveConstraint = shape2.Constraints.Single(x => x is RDFMinInclusiveConstraint) as RDFMinInclusiveConstraint;
            Assert.IsNotNull(shape2MinInclusiveConstraint);
            Assert.IsTrue(shape2MinInclusiveConstraint.Value.Equals(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFMinLengthConstraint shape2MinLengthConstraint = shape2.Constraints.Single(x => x is RDFMinLengthConstraint) as RDFMinLengthConstraint;
            Assert.IsNotNull(shape2MinLengthConstraint);
            Assert.IsTrue(shape2MinLengthConstraint.MinLength == 2);
            #endregion
        }

        [DataTestMethod]
        [DataRow(RDFValidationEnums.RDFShapeSeverity.Violation)]
        [DataRow(RDFValidationEnums.RDFShapeSeverity.Warning)]
        [DataRow(RDFValidationEnums.RDFShapeSeverity.Info)]
        public void ShouldParsePropertyShapeFromGraph(RDFValidationEnums.RDFShapeSeverity severity)
        {
            RDFPropertyShape shape = new RDFPropertyShape(new RDFResource("ex:propertyShape"), RDFVocabulary.FOAF.KNOWS);

            //Attributes
            shape.SetSeverity(severity);
            shape.AddMessage(new RDFPlainLiteral("message", "en-US"));

            //NonValidating
            shape.AddDescription(new RDFPlainLiteral("description", "en-US"));
            shape.AddName(new RDFPlainLiteral("name", "en-US"));
            shape.SetOrder(1);
            shape.SetGroup(new RDFResource("ex:shapeGroup"));

            //Targets
            shape.AddTarget(new RDFTargetClass(new RDFResource("ex:Person")));
            shape.AddTarget(new RDFTargetNode(new RDFResource("ex:Alice")));
            shape.AddTarget(new RDFTargetSubjectsOf(RDFVocabulary.FOAF.KNOWS));
            shape.AddTarget(new RDFTargetObjectsOf(RDFVocabulary.FOAF.KNOWS));
            
            //Constraints
            shape.AddConstraint(new RDFClassConstraint(new RDFResource("ex:Human")));
            shape.AddConstraint(new RDFAndConstraint().AddShape(shape));
            shape.AddConstraint(new RDFClosedConstraint(true).AddIgnoredProperty(RDFVocabulary.FOAF.KNOWS));
            shape.AddConstraint(new RDFDatatypeConstraint(RDFModelEnums.RDFDatatypes.XSD_INTEGER));
            shape.AddConstraint(new RDFDisjointConstraint(RDFVocabulary.FOAF.KNOWS));
            shape.AddConstraint(new RDFEqualsConstraint(RDFVocabulary.FOAF.KNOWS));
            shape.AddConstraint(new RDFHasValueConstraint(new RDFResource("ex:Alice")));
            shape.AddConstraint(new RDFHasValueConstraint(new RDFPlainLiteral("Alice")));
            shape.AddConstraint(new RDFInConstraint(RDFModelEnums.RDFItemTypes.Resource).AddValue(new RDFResource("ex:Alice")));
            shape.AddConstraint(new RDFInConstraint(RDFModelEnums.RDFItemTypes.Literal).AddValue(new RDFPlainLiteral("Alice")));
            shape.AddConstraint(new RDFLanguageInConstraint(new List<string>() { "en-US" }));
            shape.AddConstraint(new RDFLessThanConstraint(new RDFResource("ex:prop")));
            shape.AddConstraint(new RDFLessThanOrEqualsConstraint(new RDFResource("ex:prop")));
            shape.AddConstraint(new RDFMaxCountConstraint(2));
            shape.AddConstraint(new RDFMaxExclusiveConstraint(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            shape.AddConstraint(new RDFMaxInclusiveConstraint(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            shape.AddConstraint(new RDFMaxLengthConstraint(2));
            shape.AddConstraint(new RDFMinCountConstraint(2));
            shape.AddConstraint(new RDFMinExclusiveConstraint(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            shape.AddConstraint(new RDFMinInclusiveConstraint(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            shape.AddConstraint(new RDFMinLengthConstraint(2));

            //ShapesGraph
            RDFShapesGraph shapesGraph = new RDFShapesGraph(new RDFResource("ex:shapesGraph"));
            shapesGraph.AddShape(shape);
            RDFGraph graph = shapesGraph.ToRDFGraph();
            RDFShapesGraph shapesGraph2 = RDFValidationHelper.FromRDFGraph(graph);

            #region Asserts
            Assert.IsNotNull(shapesGraph2);
            Assert.IsTrue(shapesGraph2.ShapesCount == 1);
            Assert.IsTrue(shapesGraph2.Equals(new RDFResource("ex:shapesGraph")));
            RDFPropertyShape shape2 = shapesGraph2.SelectShape("ex:propertyShape") as RDFPropertyShape;
            Assert.IsNotNull(shape2);

            //Attributes
            Assert.IsFalse(shape2.Deactivated);
            Assert.IsTrue(shape2.Severity == severity);
            Assert.IsTrue(shape2.MessagesCount == 1);
            RDFPlainLiteral shape2Message = shape2.Messages.Single() as RDFPlainLiteral;
            Assert.IsNotNull(shape2Message);
            Assert.IsTrue(shape2Message.Equals(new RDFPlainLiteral("message", "en-US")));

            //NonValidating
            Assert.IsTrue(shape2.Descriptions.Count == 1);
            Assert.IsTrue(shape2.Descriptions.Single().Equals(new RDFPlainLiteral("description", "en-US")));
            Assert.IsTrue(shape2.Names.Count == 1);
            Assert.IsTrue(shape2.Names.Single().Equals(new RDFPlainLiteral("name", "en-US")));
            Assert.IsNotNull(shape2.Order);
            Assert.IsTrue(shape2.Order.Equals(new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            Assert.IsNotNull(shape2.Group);
            Assert.IsTrue(shape2.Group.Equals(new RDFResource("ex:shapeGroup")));

            //Targets
            Assert.IsTrue(shape2.TargetsCount == 4);
            RDFTargetClass shape2TargetClass = shape2.Targets.Single(x => x is RDFTargetClass) as RDFTargetClass;
            Assert.IsNotNull(shape2TargetClass);
            Assert.IsTrue(shape2TargetClass.TargetValue.Equals(new RDFResource("ex:Person")));
            RDFTargetNode shape2TargetNode = shape2.Targets.Single(x => x is RDFTargetNode) as RDFTargetNode;
            Assert.IsNotNull(shape2TargetNode);
            Assert.IsTrue(shape2TargetNode.TargetValue.Equals(new RDFResource("ex:Alice")));
            RDFTargetSubjectsOf shape2TargetSubjectsOf = shape2.Targets.Single(x => x is RDFTargetSubjectsOf) as RDFTargetSubjectsOf;
            Assert.IsNotNull(shape2TargetSubjectsOf);
            Assert.IsTrue(shape2TargetSubjectsOf.TargetValue.Equals(RDFVocabulary.FOAF.KNOWS));
            RDFTargetObjectsOf shape2TargetObjectsOf = shape2.Targets.Single(x => x is RDFTargetObjectsOf) as RDFTargetObjectsOf;
            Assert.IsNotNull(shape2TargetObjectsOf);
            Assert.IsTrue(shape2TargetObjectsOf.TargetValue.Equals(RDFVocabulary.FOAF.KNOWS));

            //Constraints
            Assert.IsTrue(shape2.ConstraintsCount == 21);
            RDFClassConstraint shape2ClassConstraint = shape2.Constraints.Single(x => x is RDFClassConstraint) as RDFClassConstraint;
            Assert.IsNotNull(shape2ClassConstraint);
            Assert.IsTrue(shape2ClassConstraint.ClassType.Equals(new RDFResource("ex:Human")));
            RDFAndConstraint shape2AndConstraint = shape2.Constraints.Single(x => x is RDFAndConstraint) as RDFAndConstraint;
            Assert.IsNotNull(shape2AndConstraint);
            Assert.IsTrue(shape2AndConstraint.AndShapes.ContainsKey(shape2.PatternMemberID));
            RDFClosedConstraint shape2ClosedConstraint = shape2.Constraints.Single(x => x is RDFClosedConstraint) as RDFClosedConstraint;
            Assert.IsNotNull(shape2ClosedConstraint);
            Assert.IsTrue(shape2ClosedConstraint.Closed);
            Assert.IsTrue(shape2ClosedConstraint.IgnoredProperties.ContainsKey(RDFVocabulary.FOAF.KNOWS.PatternMemberID));
            RDFDatatypeConstraint shape2DatatypeConstraint = shape2.Constraints.Single(x => x is RDFDatatypeConstraint) as RDFDatatypeConstraint;
            Assert.IsNotNull(shape2DatatypeConstraint);
            Assert.IsTrue(shape2DatatypeConstraint.Datatype == RDFModelEnums.RDFDatatypes.XSD_INTEGER);
            RDFDisjointConstraint shape2DisjointConstraint = shape2.Constraints.Single(x => x is RDFDisjointConstraint) as RDFDisjointConstraint;
            Assert.IsNotNull(shape2DisjointConstraint);
            Assert.IsTrue(shape2DisjointConstraint.DisjointPredicate.Equals(RDFVocabulary.FOAF.KNOWS));
            RDFEqualsConstraint shape2EqualsConstraint = shape2.Constraints.Single(x => x is RDFEqualsConstraint) as RDFEqualsConstraint;
            Assert.IsNotNull(shape2EqualsConstraint);
            Assert.IsTrue(shape2EqualsConstraint.EqualsPredicate.Equals(RDFVocabulary.FOAF.KNOWS));
            RDFHasValueConstraint shape2HasValueConstraintRes = shape2.Constraints.Single(x => x is RDFHasValueConstraint hvc && hvc.Value is RDFResource) as RDFHasValueConstraint;
            Assert.IsNotNull(shape2HasValueConstraintRes);
            Assert.IsTrue(shape2HasValueConstraintRes.Value.Equals(new RDFResource("ex:Alice")));
            RDFHasValueConstraint shape2HasValueConstraintLit = shape2.Constraints.Single(x => x is RDFHasValueConstraint hvc && hvc.Value is RDFLiteral) as RDFHasValueConstraint;
            Assert.IsNotNull(shape2HasValueConstraintLit);
            Assert.IsTrue(shape2HasValueConstraintLit.Value.Equals(new RDFPlainLiteral("Alice")));
            RDFInConstraint shape2InConstraintRes = shape2.Constraints.Single(x => x is RDFInConstraint ic && ic.ItemType == RDFModelEnums.RDFItemTypes.Resource) as RDFInConstraint;
            Assert.IsNotNull(shape2InConstraintRes);
            Assert.IsTrue(shape2InConstraintRes.InValues.ContainsKey(new RDFResource("ex:Alice").PatternMemberID));
            RDFInConstraint shape2InConstraintLit = shape2.Constraints.Single(x => x is RDFInConstraint ic && ic.ItemType == RDFModelEnums.RDFItemTypes.Literal) as RDFInConstraint;
            Assert.IsNotNull(shape2InConstraintLit);
            Assert.IsTrue(shape2InConstraintLit.InValues.ContainsKey(new RDFPlainLiteral("Alice").PatternMemberID));
            RDFLanguageInConstraint shape2LanguageInConstraint = shape2.Constraints.Single(x => x is RDFLanguageInConstraint) as RDFLanguageInConstraint;
            Assert.IsNotNull(shape2LanguageInConstraint);
            Assert.IsTrue(shape2LanguageInConstraint.LanguageTags.Contains("EN-US"));
            RDFLessThanConstraint shape2LessThanConstraint = shape2.Constraints.Single(x => x is RDFLessThanConstraint) as RDFLessThanConstraint;
            Assert.IsNotNull(shape2LessThanConstraint);
            Assert.IsTrue(shape2LessThanConstraint.LessThanPredicate.Equals(new RDFResource("ex:prop")));
            RDFLessThanOrEqualsConstraint shape2LessThanOrEqualsConstraint = shape2.Constraints.Single(x => x is RDFLessThanOrEqualsConstraint) as RDFLessThanOrEqualsConstraint;
            Assert.IsNotNull(shape2LessThanOrEqualsConstraint);
            Assert.IsTrue(shape2LessThanOrEqualsConstraint.LessThanOrEqualsPredicate.Equals(new RDFResource("ex:prop")));
            RDFMaxCountConstraint shape2MaxCountConstraint = shape2.Constraints.Single(x => x is RDFMaxCountConstraint) as RDFMaxCountConstraint;
            Assert.IsNotNull(shape2MaxCountConstraint);
            Assert.IsTrue(shape2MaxCountConstraint.MaxCount == 2);
            RDFMaxExclusiveConstraint shape2MaxExclusiveConstraint = shape2.Constraints.Single(x => x is RDFMaxExclusiveConstraint) as RDFMaxExclusiveConstraint;
            Assert.IsNotNull(shape2MaxExclusiveConstraint);
            Assert.IsTrue(shape2MaxExclusiveConstraint.Value.Equals(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFMaxInclusiveConstraint shape2MaxInclusiveConstraint = shape2.Constraints.Single(x => x is RDFMaxInclusiveConstraint) as RDFMaxInclusiveConstraint;
            Assert.IsNotNull(shape2MaxInclusiveConstraint);
            Assert.IsTrue(shape2MaxInclusiveConstraint.Value.Equals(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFMaxLengthConstraint shape2MaxLengthConstraint = shape2.Constraints.Single(x => x is RDFMaxLengthConstraint) as RDFMaxLengthConstraint;
            Assert.IsNotNull(shape2MaxLengthConstraint);
            Assert.IsTrue(shape2MaxLengthConstraint.MaxLength == 2);
            RDFMinCountConstraint shape2MinCountConstraint = shape2.Constraints.Single(x => x is RDFMinCountConstraint) as RDFMinCountConstraint;
            Assert.IsNotNull(shape2MinCountConstraint);
            Assert.IsTrue(shape2MinCountConstraint.MinCount == 2);
            RDFMinExclusiveConstraint shape2MinExclusiveConstraint = shape2.Constraints.Single(x => x is RDFMinExclusiveConstraint) as RDFMinExclusiveConstraint;
            Assert.IsNotNull(shape2MinExclusiveConstraint);
            Assert.IsTrue(shape2MinExclusiveConstraint.Value.Equals(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFMinInclusiveConstraint shape2MinInclusiveConstraint = shape2.Constraints.Single(x => x is RDFMinInclusiveConstraint) as RDFMinInclusiveConstraint;
            Assert.IsNotNull(shape2MinInclusiveConstraint);
            Assert.IsTrue(shape2MinInclusiveConstraint.Value.Equals(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFMinLengthConstraint shape2MinLengthConstraint = shape2.Constraints.Single(x => x is RDFMinLengthConstraint) as RDFMinLengthConstraint;
            Assert.IsNotNull(shape2MinLengthConstraint);
            Assert.IsTrue(shape2MinLengthConstraint.MinLength == 2);
            #endregion
        }
        #endregion
    }
}