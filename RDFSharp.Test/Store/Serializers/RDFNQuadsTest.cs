/*
   Copyright 2012-2022 Marco De Salvo

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace RDFSharp.Test.Store
{
    [TestClass]
    public class RDFNQuadsTest
    {
        #region Tests
        [TestMethod]
        public void ShouldSerializeEmptyStore()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeEmptyStore.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeEmptyStore.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeEmptyStore.nq"));
            Assert.IsTrue(fileContent.Equals(string.Empty));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPOQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"),new RDFResource("http://pred/"),new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> <http://obj/> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPBQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"),new RDFResource("http://pred/"),new RDFResource("bnode:12345")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPBQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPBQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPBQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> _:12345 <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCBPOQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"),new RDFResource("http://pred/"),new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPOQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPOQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPOQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"_:12345 <http://pred/> <http://obj/> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCBPBQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"),new RDFResource("http://pred/"),new RDFResource("bnode:12345")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPBQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPBQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPBQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"_:12345 <http://pred/> _:12345 <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"),new RDFResource("http://pred/"),new RDFPlainLiteral("hello")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"hello\" <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLLQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"),new RDFResource("http://pred/"),new RDFPlainLiteral("hello","en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLLQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLLQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLLQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"hello\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLTQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"),new RDFResource("http://pred/"),new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"25\"^^<{RDFVocabulary.XSD.INTEGER}> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCBPLQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"),new RDFResource("http://pred/"),new RDFPlainLiteral("hello")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"_:12345 <http://pred/> \"hello\" <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCBPLLQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"),new RDFResource("http://pred/"),new RDFPlainLiteral("hello","en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLLQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLLQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLLQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"_:12345 <http://pred/> \"hello\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCBPLTQuadruple()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"),new RDFResource("http://pred/"),new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLTQuadruple.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLTQuadruple.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCBPLTQuadruple.nq"));
            Assert.IsTrue(fileContent.Equals($"_:12345 <http://pred/> \"25\"^^<{RDFVocabulary.XSD.INTEGER}> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInContext()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx😃/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInContext.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInContext.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInContext.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> <http://obj/> <http://ctx\\U0001F603/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInSubject()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj😃/"), new RDFResource("http://pred/"), new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInSubject.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInSubject.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInSubject.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj\\U0001F603/> <http://pred/> <http://obj/> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInPredicate()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred😃/"), new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInPredicate.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInPredicate.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInPredicate.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred\\U0001F603/> <http://obj/> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInObject()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFResource("http://obj😃/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInObject.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInObject.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingLongUnicodeCharInObject.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> <http://obj\\U0001F603/> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLTripleHavingLongUnicodeCharInLiteral()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("Smile!😃","en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTripleHavingLongUnicodeCharInLiteral.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTripleHavingLongUnicodeCharInLiteral.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTripleHavingLongUnicodeCharInLiteral.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"Smile!\\U0001F603\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

                [TestMethod]
        public void ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInContext()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/frag#pageβ2"), new RDFResource("http://subj"), new RDFResource("http://pred/"), new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInContext.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInContext.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInContext.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> <http://obj/> <http://ctx/frag#page\\u03B22> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInSubject()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/frag#pageβ2"), new RDFResource("http://pred/"), new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInSubject.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInSubject.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInSubject.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/frag#page\\u03B22> <http://pred/> <http://obj/> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInPredicate()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/frag#pageβ2"), new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInPredicate.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInPredicate.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInPredicate.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/frag#page\\u03B22> <http://obj/> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInObject()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFResource("http://obj/frag#pageβ2")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInObject.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInObject.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPOTripleHavingShortUnicodeCharInObject.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> <http://obj/frag#page\\u03B22> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLTripleHavingShortUnicodeCharInLiteral()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("Beta!β", "en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTripleHavingShortUnicodeCharInLiteral.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTripleHavingShortUnicodeCharInLiteral.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLTripleHavingShortUnicodeCharInLiteral.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"Beta!\\u03B2\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLQuadrupleHavingCarriageReturnCharInLiteral()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("Return!\r", "en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingCarriageReturnCharInLiteral.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingCarriageReturnCharInLiteral.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingCarriageReturnCharInLiteral.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"Return!\\r\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLQuadrupleHavingNewlineCharInLiteral()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("NewLine!\n", "en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingNewLineCharInLiteral.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingNewLineCharInLiteral.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingNewLineCharInLiteral.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"NewLine!\\n\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLQuadrupleHavingTabCharInLiteral()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("Tab!\t", "en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingTabCharInLiteral.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingTabCharInLiteral.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingTabCharInLiteral.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"Tab!\\t\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLQuadrupleHavingSlashCharInLiteral()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("Slash!\\", "en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingSlashCharInLiteral.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingSlashCharInLiteral.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingSlashCharInLiteral.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"Slash!\\\\\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeStoreWithCSPLQuadrupleHavingDoubleQuotesCharInLiteral()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("DoubleQuotes!\"", "en-US")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingDoubleQuotesCharInLiteral.nq"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingDoubleQuotesCharInLiteral.nq")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldSerializeStoreWithCSPLQuadrupleHavingDoubleQuotesCharInLiteral.nq"));
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> \"DoubleQuotes!\\\"\"@EN-US <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldSerializeEmptyStoreToStream()
        {
            MemoryStream stream = new MemoryStream();
            RDFMemoryStore store = new RDFMemoryStore();
            RDFNQuads.Serialize(store, stream);

            string fileContent;
            using (StreamReader reader = new StreamReader(new MemoryStream(stream.ToArray())))
                fileContent = reader.ReadToEnd();
            Assert.IsTrue(fileContent.Equals(string.Empty));
        }

        [TestMethod]
        public void ShouldSerializeStoreToStream()
        {
            MemoryStream stream = new MemoryStream();
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, stream);

            string fileContent;
            using (StreamReader reader = new StreamReader(new MemoryStream(stream.ToArray())))
                fileContent = reader.ReadToEnd();
            Assert.IsTrue(fileContent.Equals($"<http://subj/> <http://pred/> <http://obj/> <http://ctx/> .{Environment.NewLine}"));
        }

        [TestMethod]
        public void ShouldDeserializeEmptyStoreFromFile()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldDeserializeEmptyStore.nq"));
            RDFMemoryStore store2 = RDFNQuads.Deserialize(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldDeserializeEmptyStore.nq"));

            Assert.IsNotNull(store2);
            Assert.IsTrue(store2.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreFromFile()
        {
            RDFMemoryStore store = new RDFMemoryStore();
            store.AddQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFResource("http://obj/")));
            RDFNQuads.Serialize(store, Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldDeserializeStore.nq"));
            RDFMemoryStore store2 = RDFNQuads.Deserialize(Path.Combine(Environment.CurrentDirectory, $"RDFNQuadsTest_ShouldDeserializeStore.nq"));

            Assert.IsNotNull(store2);
            Assert.IsTrue(store2.QuadruplesCount == 1);
            Assert.IsTrue(store2.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFResource("http://obj/"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCSPOTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"<http://subj/> <http://pred/> <http://obj/> <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFResource("http://obj/"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCSPOTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#<http://subj/> <http://pred/> <http://obj/> <http://ctx/> .");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCSPBTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"<http://subj/> <http://pred/> _:12345 <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFResource("bnode:12345"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCSPBTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#<http://subj/> <http://pred/> _:12345 <http://ctx/> .");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCBPOTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"_:12345 <http://pred/> <http://obj/> <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"), new RDFResource("http://pred/"), new RDFResource("http://obj/"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCBPOTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#_:12345 <http://pred/> <http://obj/> <http://ctx/> .");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCBPBTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"_:12345 <http://pred/> _:12345 <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"), new RDFResource("http://pred/"), new RDFResource("bnode:12345"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCBPBTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#_:12345 <http://pred/> _:12345 <http://ctx/> .");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCSPLTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"<http://subj/> <http://pred/> \"hello\" <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("hello"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCSPLTripleEvenIfEmptyLiteral()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"<http://subj/> <http://pred/> \"\" <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral(string.Empty))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCSPLTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#<http://subj/> <http://pred/> \"hello\" <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCSPLLTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"<http://subj/> <http://pred/> \"hello\"@en-US <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral("hello", "en-US"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCSPLLTripleEvenIfEmptyLiteral()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"<http://subj/> <http://pred/> \"\"@en-US <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFPlainLiteral(string.Empty, "en-US"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCSPLLTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#<http://subj/> <http://pred/> \"hello\"@en-US <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCSPLTTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"<http://subj/> <http://pred/> \"25\"^^<http://www.w3.org/2001/XMLSchema#integer> <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCSPLTTripleEvenIfEmptyLiteral()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"<http://subj/> <http://pred/> \"\"^^<http://www.w3.org/2001/XMLSchema#string> <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("http://subj/"), new RDFResource("http://pred/"), new RDFTypedLiteral(string.Empty, RDFModelEnums.RDFDatatypes.XSD_STRING))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCSPLTTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#<http://subj/> <http://pred/> \"25\"^^<http://www.w3.org/2001/XMLSchema#integer> <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCBPLTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"_:12345 <http://pred/> \"hello\" <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"), new RDFResource("http://pred/"), new RDFPlainLiteral("hello"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCBPLTripleEvenIfEmptyLiteral()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"_:12345 <http://pred/> \"\" <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"), new RDFResource("http://pred/"), new RDFPlainLiteral(string.Empty))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCBPLTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#_:12345 <http://pred/> \"hello\" <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCBPLLTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"_:12345 <http://pred/> \"hello\"@en-US <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"), new RDFResource("http://pred/"), new RDFPlainLiteral("hello", "en-US"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCBPLLTripleEvenIfEmptyLiteral()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"_:12345 <http://pred/> \"\"@en-US <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"), new RDFResource("http://pred/"), new RDFPlainLiteral(string.Empty, "en-US"))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCBPLLTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#_:12345 <http://pred/> \"hello\"@en-US <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCBPLTTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"_:12345 <http://pred/> \"25\"^^<http://www.w3.org/2001/XMLSchema#integer> <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"), new RDFResource("http://pred/"), new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCBPLTTripleEvenIfEmptyLiteral()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"_:12345 <http://pred/> \"\"^^<http://www.w3.org/2001/XMLSchema#string> <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 1);
            Assert.IsTrue(store.ContainsQuadruple(new RDFQuadruple(new RDFContext("http://ctx/"), new RDFResource("bnode:12345"), new RDFResource("http://pred/"), new RDFTypedLiteral(string.Empty, RDFModelEnums.RDFDatatypes.XSD_STRING))));
        }

        [TestMethod]
        public void ShouldDeserializeStoreWithCommentedCBPLTTriple()
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"#_:12345 <http://pred/> \"25\"^^<http://www.w3.org/2001/XMLSchema#integer> <http://ctx/> .{Environment.NewLine}");
            RDFMemoryStore store = RDFNQuads.Deserialize(new MemoryStream(stream.ToArray()));

            Assert.IsNotNull(store);
            Assert.IsTrue(store.QuadruplesCount == 0);
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory, "RDFNQuadsTest_Should*"))
                File.Delete(file);
        }
        #endregion
    }
}