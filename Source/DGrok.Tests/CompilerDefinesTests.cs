// Copyright (c) 2007-2014 Joe White
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Text;
using DGrok.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace DGrok.Tests
{
    [TestFixture]
    //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class CompilerDefinesTests
    {
        private CompilerDefines _defines;

        [SetUp]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestInitialize]
        public void SetUp()
        {
            _defines = CompilerDefines.CreateEmpty();
        }

        private bool DefineIsTrue(string compilerDirective)
        {
            return _defines.IsTrue(compilerDirective, new Location("", "", 0));
        }

        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void FalseIfUndefinedIfDef()
        {
            Assert.That(DefineIsTrue("IFDEF FOO"), Is.False);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void TrueIfUndefinedIfNDef()
        {
            Assert.That(DefineIsTrue("IFNDEF FOO"), Is.True);
        }
        [Test, ExpectedException(typeof(PreprocessorException))]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(PreprocessorException))]
        public void ErrorIfUndefinedIf()
        {
            DefineIsTrue("IF Foo");
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefineDirectiveAsTrue()
        {
            _defines.DefineDirectiveAsTrue("IFDEF FOO");
            Assert.That(DefineIsTrue("IFDEF FOO"), Is.True);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefineDirectiveAsFalse()
        {
            _defines.DefineDirectiveAsFalse("IFDEF FOO");
            Assert.That(DefineIsTrue("IFDEF FOO"), Is.False);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefineSymbol()
        {
            _defines.DefineSymbol("FOO");
            Assert.That(DefineIsTrue("IFDEF FOO"), Is.True);
            Assert.That(DefineIsTrue("IFNDEF FOO"), Is.False);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void UndefineSymbol()
        {
            _defines.UndefineSymbol("FOO");
            Assert.That(DefineIsTrue("IFDEF FOO"), Is.False);
            Assert.That(DefineIsTrue("IFNDEF FOO"), Is.True);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void NotCaseSensitive()
        {
            _defines.DefineDirectiveAsTrue("IFDEF FOO");
            Assert.That(DefineIsTrue("IfDef Foo"), Is.True);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefinedAndTrue()
        {
            _defines.DefineSymbol("FOO");
            _defines.DefineSymbol("BAR");
            Assert.That(DefineIsTrue("if defined(Foo) and defined(Bar)"), Is.True);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefinedAndFalse()
        {
            _defines.DefineSymbol("FOO");
            Assert.That(DefineIsTrue("if defined(Foo) and defined(Bar)"), Is.False);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefinedAndParenthesisTrue()
        {
            _defines.DefineSymbol("FOO");
            _defines.DefineSymbol("BAR");
            Assert.That(DefineIsTrue("IF (DEFINED(Foo) or defined(FOO2)) and defined(Bar)"), Is.True);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefinedAndParenthesisFalse()
        {
            _defines.DefineSymbol("BAR");
            Assert.That(DefineIsTrue("IF (defined(Foo) or DEFINED(FOO2)) and defined(Bar)"), Is.False);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefinedOrFalse()
        {
            Assert.That(DefineIsTrue("if defined(Foo) or defined(Bar)"), Is.False);
        }
        [Test]
        //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DefinedOrTrue()
        {
            _defines.DefineSymbol("FOO");
            Assert.That(DefineIsTrue("if defined(Foo) OR defined(Bar)"), Is.True);
        }
    }
}
