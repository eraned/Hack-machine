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
    public class IfStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> DoIfTrue { get; private set; }
        public List<StatetmentBase> DoIfFalse { get; private set; }
        
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
          * 'if' '('Expression')' '{' Statment* '}' ('else' '{' Statment* '}' )
          */
            DoIfTrue = new List<StatetmentBase>();
            DoIfFalse = new List<StatetmentBase>();
            Token t;
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Statement && sTokens.Peek().ToString() == "if")
            {
                Token tStartIF = sTokens.Pop(); //if
            }
            else
                throw new SyntaxErrorException("Expected if statment received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "(")
            {
                t = sTokens.Pop(); //(
            }
            else
                throw new SyntaxErrorException("Expected ( Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
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
            //DoIftrue
            StackIsNotEmpty(sTokens);
            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                StatetmentBase sTrue = StatetmentBase.Create(sTokens.Peek());
                sTrue.Parse(sTokens);
                DoIfTrue.Add(sTrue);
            }
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "}")
            {
                t = sTokens.Pop(); //}
            }
            else
                throw new SyntaxErrorException("Expected } Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            //DoIfFalse
            if (sTokens.Peek().ToString() == "else")
            {
                if (sTokens.Peek() is Statement && sTokens.Peek().ToString() == "else")
                {
                    Token tStartELSE = sTokens.Pop(); //else
                }
                else
                    throw new SyntaxErrorException("Expected else statment received: " + sTokens.Peek().ToString(), sTokens.Peek());
                StackIsNotEmpty(sTokens);
                if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "{")
                {
                    t = sTokens.Pop(); //{
                }
                else
                    throw new SyntaxErrorException("Expected { Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
                while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
                {
                    StatetmentBase sFalse = StatetmentBase.Create(sTokens.Peek());
                    sFalse.Parse(sTokens);
                    DoIfFalse.Add(sFalse);
                }
                StackIsNotEmpty(sTokens);
                if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "}")
                {
                    t = sTokens.Pop(); //}
                }
                else
                    throw new SyntaxErrorException("Expected } Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            }
        }

        public override string ToString()
        {
            string sIf = "if(" + Term + "){\n";
            foreach (StatetmentBase s in DoIfTrue)
                sIf += "\t\t\t" + s + "\n";
            sIf += "\t\t}";
            if (DoIfFalse.Count > 0)
            {
                sIf += "else{";
                foreach (StatetmentBase s in DoIfFalse)
                    sIf += "\t\t\t" + s + "\n";
                sIf += "\t\t}";
            }
            return sIf;
        }

    }
}
