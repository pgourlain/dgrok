using System;
using System.Collections.Generic;
using System.Text;
using DGrok.DelphiNodes;
using DGrok.Framework;
using System.Linq;

namespace DGrok.Visitors
{
    public class ToCSharpVisitor : Visitor
    {
        #region code management
        StringBuilder _code = new StringBuilder();
        int currentindent = 0;
        public void AddCode(string code)
        {
            _code.Append(code);
        }
        public void AddIndentCode(string code)
        {
            _code.Append(new string(' ', currentindent * 4));
            _code.Append(code);
        }
        private void AddLineCode(string code)
        {
            _code.AppendLine(code);
        }

        private void AddLineCodeFormat(string fmt, params object[] args)
        {
            _code.AppendFormat(fmt, args);
            _code.AppendLine();
        }
        private void AddFullLineCodeFormat(string fmt, params object[] args)
        {
            _code.Append(new string(' ', currentindent * 4));
            _code.AppendFormat(fmt, args);
            _code.AppendLine();
        }

        private void AddFullLineCode(string code)
        {
            _code.Append(new string(' ', currentindent * 4));
            _code.AppendLine(code);
        }

        private void UnIndent()
        {
            currentindent--;
            if (currentindent < 0)
                currentindent = 0;
        }

        private void Indent()
        {
            currentindent++;
        }
        #endregion

        public string Convert(CodeBase codeBase)
        {
            Visit(codeBase);
            return _code.ToString();
        }


        public override void VisitUnitNode(UnitNode node)
        {
            AddLineCodeFormat("namespace {0}", node.UnitNameNode.Text);
            AddFullLineCode("{");
            Indent();
            AddFullLineCode("using System;");
            AddFullLineCode("using System.Collections.Generic;");
            AddLineCode("");
            IEnumerable<UsedUnitNode> uses = new UsedUnitNode[0];
            if (node.InterfaceSectionNode.UsesClauseNode != null)
            {
                uses = uses.Union(node.InterfaceSectionNode.UsesClauseNode.UnitListNode.Items.Select(x => x.ItemNode));
            }
            if (node.ImplementationSectionNode.UsesClauseNode != null)
            {
                uses = uses.Union(node.ImplementationSectionNode.UsesClauseNode.UnitListNode.Items.Select(x => x.ItemNode));
            }

            foreach(var use in uses.Select(x => x.FileNameNode.Text).Distinct())
            {
                AddFullLineCodeFormat("using {0};", use);
            }

            Visit(node.InterfaceSectionNode);
            Visit(node.ImplementationSectionNode);
            Visit(node.InitSectionNode);
            Visit(node.DotNode);

            UnIndent();
            AddFullLineCode("}");
        }

        public override void VisitInterfaceTypeNode(InterfaceTypeNode node)
        {
            base.VisitInterfaceTypeNode(node);
        }

        public override void VisitMethodHeadingNode(MethodHeadingNode node)
        {
            //en C# pas besoin de visiter ce noeuds
            //TODO: reste 
            //base.VisitMethodHeadingNode(node);
        }

        public override void VisitMethodResolutionNode(MethodResolutionNode node)
        {
            base.VisitMethodResolutionNode(node);
        }

        public override void VisitMethodImplementationNode(MethodImplementationNode node)
        {
            //TODO: static only for unit methods
            if (IsPublicUnitMethod(node))
                AddIndentCode("public static ");
            else
            {
                AddIndentCode("private static ");
            }
            //node.MethodHeadingNode.MethodTypeNode.Text

            AddLineCodeFormat("{0} {1}()", ReturnType(node.MethodHeadingNode), ((Token)node.MethodHeadingNode.NameNode).Text);
            AddFullLineCode("{");
            Indent();
            if (node.MethodHeadingNode.ReturnTypeNode != null)
            {
                AddFullLineCodeFormat("{0} result;", ReturnType(node.MethodHeadingNode));
            }
            base.Visit(node.FancyBlockNode);
            if (node.MethodHeadingNode.ReturnTypeNode != null)
            {
                AddFullLineCode("return result;");
            }
            UnIndent();
            AddFullLineCode("}");
        }

        private string ReturnType(MethodHeadingNode node)
        {
            if (node.ReturnTypeNode != null)
            {
                var type = ((Token)node.ReturnTypeNode).Text.ToLowerInvariant();
                switch (type)
                {
                    case "string" :
                        return "string";
                    case "boolean":
                        return "bool";
                    case "integer":
                        return "int";
                    default:
                        return type;
                }
            }
            return "void";
        }

        public override void VisitBlockNode(BlockNode node)
        {
            foreach (AstNode item in node.StatementListNode.Items)
            {
                AddIndentCode("");
                Visit(item);
                AddLineCode(";");
            }
        }

        public override void VisitBinaryOperationNode(BinaryOperationNode node)
        {

            if (node.OperatorNode.Type == TokenType.ColonEquals)
            {
                base.VisitBinaryOperationNode(node);
                //base.Visit(node.RightNode);
                //AddCode(((Token)node.LeftNode).Text);
                //AddCode(" = ");
                //base.Visit(node.RightNode);
            }
            else
                base.VisitBinaryOperationNode(node);
        }

        public override void VisitDelimitedItemNode(AstNode node, AstNode item, Token delimiter)
        {
            base.VisitDelimitedItemNode(node, item, delimiter);
        }

        public override void VisitToken(Token token)
        {
            base.VisitToken(token);
            switch(token.Type)
            {
                case TokenType.ColonEquals:
                    AddCode(" = ");
                    break;
                case TokenType.Semicolon:
                    //AddCode(";");
                    break;
                case TokenType.StringLiteral:
                    AddCode("\"" + token.Text.Trim('\'') +"\"");
                    break;
                case TokenType.Identifier:
                    AddCode(token.Text.ToLowerInvariant());
                    break;
            }
        }

        private bool IsPublicUnitMethod(MethodImplementationNode node)
        {
            //public is present in interface
            var unitNode = node.ParentNodeOfType<UnitNode>();
            var expectedName = ((Token)node.MethodHeadingNode.NameNode).Text;

            var query = node.ParentNodeOfType<UnitNode>().InterfaceSectionNode.ContentListNode.Items.OfType<MethodHeadingNode>().Where(x => ((Token)x.NameNode).Text == expectedName);
            //TODO: equality should be check parameters & overloading
            return query.FirstOrDefault() != null;
        }
    }
}
