using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SimpleCompiler
{
    public class Function : JackProgramElement
    {
        public VarDeclaration.VarTypeEnum ReturnType { get; private set; }
        public string Name { get; private set; }
        public List<VarDeclaration> Args { get; private set; }
        public List<VarDeclaration> Locals { get; private set; }
        public List<StatetmentBase> Body { get; private set; }
        public ReturnStatement Return { get; private set; }

        public Function()
        {
            Args = new List<VarDeclaration>();
            Locals = new List<VarDeclaration>();
            Body = new List<StatetmentBase>();
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
             * Function Type ID( args ){
             * varDec*
             * Statments*
             * Return Statment
             * }
             * 
             * check more errors
             */
            Token t;
            StackIsNotEmpty(sTokens);
            Token tFunc = sTokens.Pop();
            if (!(tFunc is Statement) || ((Statement)tFunc).Name != "function")
                throw new SyntaxErrorException("Expected function received: " + tFunc, tFunc);
            StackIsNotEmpty(sTokens);
            Token tType = sTokens.Pop();
            if(!(tType is VarType))
                 throw new SyntaxErrorException("Expected var type, received " + tType, tType);
            ReturnType = VarDeclaration.GetVarType(tType);
            StackIsNotEmpty(sTokens);
            Token tName = sTokens.Pop();
            if(!(tName is Identifier))
                throw new SyntaxErrorException("Expected function name, received " + tType, tType);
            Name = ((Identifier)tName).Name;
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses &&  sTokens.Peek().ToString() == "(")
            {
                t = sTokens.Pop(); //(
            }
            else
                throw new SyntaxErrorException("Expected ( Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            while(sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                if (sTokens.Count < 3)
                    throw new SyntaxErrorException("Early termination ", t);
                Token tArgType = sTokens.Pop();
                Token tArgName = sTokens.Pop();
                VarDeclaration vc = new VarDeclaration(tArgType, tArgName);
                Args.Add(vc);
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
            StackIsNotEmpty(sTokens);
            if (sTokens.Peek() is Parentheses && sTokens.Peek().ToString() == "{")
            {
                t = sTokens.Pop(); //{
            }
            else
                throw new SyntaxErrorException("Expected { Parentheses received: " + sTokens.Peek().ToString(), sTokens.Peek());
            StackIsNotEmpty(sTokens);
            while (sTokens.Count > 0 && (sTokens.Peek() is Statement) && (((Statement)sTokens.Peek()).Name == "var"))
            {
                VarDeclaration local = new VarDeclaration();
                local.Parse(sTokens);
                Locals.Add(local);
            }
            StackIsNotEmpty(sTokens);
            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                StatetmentBase s=  StatetmentBase.Create(sTokens.Peek());
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
            string sFunction = "function " + ReturnType + " " + Name + "(";
            for (int i = 0; i < Args.Count - 1; i++)
                sFunction += Args[i].Type + " " + Args[i].Name + ",";
            if(Args.Count > 0)
                sFunction += Args[Args.Count - 1].Type + " " + Args[Args.Count - 1].Name;
            sFunction += "){\n";
            foreach (VarDeclaration v in Locals)
                sFunction += "\t\t" + v + "\n";
            foreach (StatetmentBase s in Body)
                sFunction += "\t\t" + s + "\n";
            sFunction += "\t}";
            return sFunction;
        }
    }
}
