using DGrok.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGrok.DelphiNodes
{
    /// <summary>
    /// this statement is only to parse "defines conditions"
    /// </summary>
    public class IfDefinedStatementNode : NonterminalNode
    {
        private readonly Token _ifKeywordNode;
        private readonly AstNode _conditionNode;
        public IfDefinedStatementNode(Token ifKeywordNode, AstNode conditionNode)
        {
            _ifKeywordNode = ifKeywordNode;
            _conditionNode = conditionNode;
        }

        public AstNode ConditionNode
        {
            get { return _conditionNode; }
        }

        public Token IfKeywordNode
        {
            get { return _ifKeywordNode; }
        }
        public override IEnumerable<AstNode> ChildNodes
        {
            get
            {
                if (IfKeywordNode != null)
                    yield return IfKeywordNode;
                if (ConditionNode != null)
                    yield return ConditionNode;
            }
        }
        public override IEnumerable<KeyValuePair<string, AstNode>> Properties
        {
            get
            {
                yield return new KeyValuePair<string, AstNode>("IfKeywordNode", IfKeywordNode);
                yield return new KeyValuePair<string, AstNode>("ConditionNode", ConditionNode);
            }
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this.IfKeywordNode);
            visitor.Visit(this.ConditionNode);
        }

    }
}