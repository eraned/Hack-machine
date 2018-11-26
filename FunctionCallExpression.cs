using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/*
 * Implement!!!!
 */
namespace SimpleCompiler
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; private set; }
        public List<Expression> Args { get; private set; }
        
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
             * funcname '(' exprssion list ')'
             */
            Args = new List<Expression>();
            StackIsNotEmpty(sTokens);
            Token t;
            if(sTokens.Peek() is Identifier){
                Token tFuncCallExp = sTokens.Pop(); //func name
                FunctionName = ((Identifier) tFuncCallExp).Name;
            }
            else
                throw new SyntaxErrorException("Expected Function name, received " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "(")
            {
                t = sTokens.Pop(); //(
            }
            else
                throw new SyntaxErrorException("Expected ( Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            //expression list
            while((sTokens.Count > 0 && !(sTokens.Peek() is Parentheses) || (sTokens.Peek().ToString()!=")")))
            {
                if (sTokens.Count < 3)
                    throw new SyntaxErrorException("Early termination ", t);
                Expression exp = Expression.Create(sTokens);
                exp.Parse(sTokens);
                Args.Add(exp);
                if (sTokens.Count > 0 && sTokens.Peek() is Separator)//,
                    sTokens.Pop(); 
            }
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == ")")
            {
                t = sTokens.Pop(); //)
            }
            else
                throw new SyntaxErrorException("Expected ) Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
        }

        public override string ToString()
        {
            string sFunction = FunctionName + "(";
            for (int i = 0; i < Args.Count - 1; i++)
                sFunction += Args[i] + ",";
            if (Args.Count > 0)
                sFunction += Args[Args.Count - 1];
            sFunction += ")";
            return sFunction;
        }
    }
}