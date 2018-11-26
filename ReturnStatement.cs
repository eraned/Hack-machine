using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * 
 * learn from here how to create expression
 * 
 */
namespace SimpleCompiler
{
    public class ReturnStatement : StatetmentBase
    {
        public Expression Expression { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            /*
             * 'return' expression ';'
             * 
             */
            Token tRet = sTokens.Pop();//return
            Expression = Expression.Create(sTokens);
            Expression.Parse(sTokens);
            Token tEnd = sTokens.Pop();//;
        }

        public override string ToString()
        {
            return "return " + Expression + ";";
        }
    }
}
