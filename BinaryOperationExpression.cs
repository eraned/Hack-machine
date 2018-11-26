using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Implement!!!!
 */
namespace SimpleCompiler
{
    public class BinaryOperationExpression : Expression
    {
        public string Operator { get;  set; }
        public Expression Operand1 { get;  set; }
        public Expression Operand2 { get;  set; }

        public override string ToString()
        {
            return "(" + Operator + " " + Operand1 + " " + Operand2 + ")";
        }
        
        public void StackIsNotEmpty(TokensStack sTokens)
        {
            if (sTokens.Count == 0)//check stack is not empty
            {
                Token StackEmpty = new Token();
                throw new SyntaxErrorException("Expected end of Program ", StackEmpty);
            }
          
        }
        
        public override void Parse(TokensStack sTokens)
        {
            /*
             * "(" + Operator + " " + Operand1 + " " + Operand2 + ")"
             */
            Token t;
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "(")
            {
                t = sTokens.Pop(); //(
            }
            else
                throw new SyntaxErrorException("Expected ( Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Operator)
            {
                Token tOperator = sTokens.Pop(); //Operator
                Operator = ((Operator) tOperator).Name.ToString();
            }
            else
                throw new SyntaxErrorException("Expected Operator received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            //Operand1
            Operand1 = Expression.Create(sTokens);
            if (Operand1 == null)
            {
                Token tOp1 = new Token();
                throw new SyntaxErrorException("Invalid Expression Exception", tOp1);
            }
            Operand1.Parse(sTokens);
            //Operand2
            Operand2 = Expression.Create(sTokens);
            if (Operand2 == null)
            {
                Token tOp2 = new Token();
                throw new SyntaxErrorException("Invalid Expression Exception", tOp2);
            }
            Operand2.Parse(sTokens);
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == ")")
            {
                t = sTokens.Pop(); //)
            }
            else
                throw new SyntaxErrorException("Expected ) Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
        }
    }
}
