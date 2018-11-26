using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

/*
 * 
 * Implement!!!!
 */
namespace SimpleCompiler
{
    public class LetStatement : StatetmentBase
    {
        public string Variable { get; set; }
        public Expression Value { get; set; }

        public override string ToString()
        {
            return "let " + Variable + " = " + Value + ";";
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
             * 'let' Variable  '=' Value ';'
             */
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Statement && sTokens.Peek().ToString() == "let")
            {
                Token tStateLet = sTokens.Pop(); //let 
            }
            else
                throw new SyntaxErrorException("Expected let statment received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            if ( sTokens.Peek() is Identifier)
            {
                Token tVariable = sTokens.Pop(); //Variable
                Variable = tVariable.ToString();
            }
            else
                throw new SyntaxErrorException("Expected Variable name received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            
            if ((sTokens.Peek() is Operator) && sTokens.Peek().ToString()=="=")
            {
                Token tEqual = sTokens.Pop(); //=
            }
             else
                throw new SyntaxErrorException("Expected  =  Operator, received " + sTokens.Peek().ToString(), sTokens.Peek());
            Value = Expression.Create(sTokens);//Value
            if (Value == null)
            {
                Token t = new Token();
                throw new SyntaxErrorException("Invalid Exception", t);
            }
            Value.Parse(sTokens);
            StackIsNotEmpty(sTokens);
            if ((sTokens.Peek() is Separator) && sTokens.Peek().ToString()==";")
            {
                Token tEndLet = sTokens.Pop(); //;
            }
            else
                 throw new SyntaxErrorException("Expected ; Separator received " + sTokens.Peek().ToString(), sTokens.Peek());

        }
        

    }
}
