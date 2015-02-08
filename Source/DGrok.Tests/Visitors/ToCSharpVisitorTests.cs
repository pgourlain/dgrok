using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using DGrok.Framework;
using DGrok.Visitors;
using NUnit.Framework;

namespace DGrok.Tests.Visitors
{
    [TestFixture]
    public class ToCSharpVisitorTests
    {

        string Convert(string unitFileName, string delphiCode)
        {
            CodeBase codeBase = new CodeBase(CompilerDefines.CreateEmpty(), new MemoryFileLoader());
            codeBase.AddFileExpectingSuccess(unitFileName, delphiCode);
            return new ToCSharpVisitor().Convert(codeBase);
        }

        private void ParseAndCheck(string unitname)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dir = Path.Combine(dir, "Visitors", "CSharpConverter");

            var inputFileName = Path.Combine(dir, unitname + ".pas.txt");
            var expectedFileName = Path.Combine(dir, unitname + ".cs.txt");
            var convertedPas = Convert(unitname + ".pas", File.ReadAllText(inputFileName));
            var expectedContent = File.ReadAllText(expectedFileName);
            Assert.AreEqual(expectedContent, convertedPas);
        }

        [Test]
        public void TestEmptyUnit()
        {
            ParseAndCheck("emptyunit");
        }

        [Test]
        public void TestStaticFunctions()
        {
            ParseAndCheck("unitfunctions");
        }
    }
}
