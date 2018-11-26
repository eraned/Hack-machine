using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 
 * Implement!!!!
 */
namespace SimpleCompiler
{
    public class WhileStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> Body { get; private set;
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
             * 'while' '(' expression ')' '{' statement* '}'
             */
            Body = new List<StatetmentBase>();
            Token t;
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Statement && sTokens.Peek().ToString() == "while")
            {
                Token tStartWhile = sTokens.Pop(); //while
            }
            else
                throw new SyntaxErrorException("Expected while statment received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "(")
            {
                 t = sTokens.Pop(); //(
            }
            else
                throw new SyntaxErrorException("Expected ( Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            while ((sTokens.Count > 0 && !(sTokens.Peek() is Parentheses) || (sTokens.Peek().ToString() != ")")))
            {
                if (sTokens.Count < 3)
                    throw new SyntaxErrorException("Early termination ", t);
                Term = Expression.Create(sTokens);
                if (Term == null)
                {
                    t = new Token();
                    throw new SyntaxErrorException("Invalid Exception", t);
                }
                Term.Parse(sTokens);
            }
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == ")")
            {
                t = sTokens.Pop(); //)
            }
            else
                throw new SyntaxErrorException("Expected ) Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "{")
            {
                t = sTokens.Pop(); //{
            }
            else
                throw new SyntaxErrorException("Expected { Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                StatetmentBase s = StatetmentBase.Create(sTokens.Peek());
                s.Parse(sTokens);
                Body.Add(s);
            }
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "}")
            {
                t = sTokens.Pop(); //}
            }
            else
                throw new SyntaxErrorException("Expected } Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
        }

        public override string ToString()
        {
            string sWhile = "while(" + Term + "){\n";
            foreach (StatetmentBase s in Body)
                sWhile += "\t\t\t" + s + "\n";
            sWhile += "\t\t}";
            return sWhile;
        }

    }
}
