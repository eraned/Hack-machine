using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SimpleCompiler
{
    public class Compiler
    {


        public Compiler()
        {

        }
//

        public List<VarDeclaration> ParseVarDeclarations(List<string> lVarLines)
        {
            List<VarDeclaration> lVars = new List<VarDeclaration>();
            for(int i = 0; i < lVarLines.Count; i++)
            {
                List<Token> lTokens = Tokenize(lVarLines[i], i);
                TokensStack stack = new TokensStack(lTokens);
                VarDeclaration var = new VarDeclaration();
                var.Parse(stack);
                lVars.Add(var);
            }
            return lVars;
        }


        public List<LetStatement> ParseAssignments(List<string> lLines)
        {
            List<LetStatement> lParsed = new List<LetStatement>();
            List<Token> lTokens = Tokenize(lLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            while(sTokens.Count > 0)
            {
                LetStatement ls = new LetStatement();
                ls.Parse(sTokens);
                lParsed.Add(ls);

            }
            return lParsed;
        }
/*
 *
 *
 * Impelementtt!!!!
 */
 

        public List<string> GenerateCode(LetStatement aSimple, Dictionary<string, int> dSymbolTable)
        {
            List<string> lAssembly = new List<string>();
  ///////////////NumericExpression        
            if(aSimple.Value is NumericExpression)
            {
                int exp = aSimple.Value;
              
                lAssembly.Add("@"+exp);
               
                lAssembly.Add("D=A");
                
                lAssembly.Add("@RESULT");
            
                lAssembly.Add("M=D");
             
                lAssembly.AddRange(ResultTOtable(aSimple.Variable, dSymbolTable));
            }
//////////////////VariableExpression
            if (aSimple.Value is VariableExpression)
            {
               int exp = ComputeExpression(aSimple.Value, dSymbolTable);
            
                lAssembly.AddRange(VariableAddressToADDRESS(dSymbolTable.ElementAt(exp).Key, dSymbolTable));
              
                lAssembly.Add("@ADDRESS");
                
                lAssembly.Add("A=M");
            
                lAssembly.Add("D=M");
              
                lAssembly.Add("@RESULT");
             
                lAssembly.Add("M=D");
         
                lAssembly.AddRange(ResultTOtable(aSimple.Variable, dSymbolTable));
            }
///////////////////BinaryOperationExpression
            if (aSimple.Value is BinaryOperationExpression)
            {
                int exp = ComputeExpression(aSimple.Value, dSymbolTable);
                BinaryOperationExpression be = (BinaryOperationExpression)aSimple.Value;
                if (be.Operand1 is NumericExpression)
                {
                   
                    lAssembly.Add("@" + be.Operand1);
                 
                    lAssembly.Add("D=A");
                  
                    lAssembly.Add("@OPERAND1");
                 
                    lAssembly.Add("M=D");
                }
                if(be.Operand1 is VariableExpression)
                {
                  
                    lAssembly.AddRange(VariableAddressToADDRESS(be.Operand1.ToString(), dSymbolTable));
                   
                    lAssembly.Add("@ADDRESS");
                  
                    lAssembly.Add("A=M");
             
                    lAssembly.Add("D=M");
                 
                    lAssembly.Add("@OPERAND1");
                   
                    lAssembly.Add("M=D");
                }
                if (be.Operand2 is NumericExpression)
                {
                   
                    lAssembly.Add("@" + be.Operand2);
               
                    lAssembly.Add("D=A");
                
                    lAssembly.Add("@OPERAND2");
                   
                    lAssembly.Add("M=D");
                }
                if (be.Operand2 is VariableExpression)
                {
                    
                    lAssembly.AddRange(VariableAddressToADDRESS(be.Operand2.ToString(), dSymbolTable));
                    
                    lAssembly.Add("@ADDRESS");
                   
                    lAssembly.Add("A=M");
                  
                    lAssembly.Add("D=M");
                 
                    lAssembly.Add("@OPERAND2");
                
                    lAssembly.Add("M=D");
                }
                
                lAssembly.Add("@OPERAND1");
               
                lAssembly.Add("D=M");
             
                lAssembly.Add("@OPERAND2");
               
                lAssembly.Add("D=D"+be.Operator+"M");
            
                lAssembly.Add("@RESULT");
          
                lAssembly.Add("M=D");
            
                lAssembly.AddRange(ResultTOtable(aSimple.Variable, dSymbolTable));
            }
            return lAssembly;
        }

        /*
         *
         *
         *
         * impelementtt!!!!
         */
       
        public Dictionary<string, int> ComputeSymbolTable(List<VarDeclaration> lDeclerations)
        {
            
            Dictionary<string, int> dTable = new Dictionary<string, int>();
            int index= 0;
            for (int i = 0; i < lDeclerations.Count; i++)
            {
                if (!lDeclerations[i].Name.Contains("_") && !dTable.ContainsKey(lDeclerations[i].Name))
                {
                    dTable.Add(lDeclerations[i].Name, index);
                    index++;
                }
            }
            for (int i = 0; i < lDeclerations.Count; i++)
            {
                if (lDeclerations[i].Name.Contains("_") && !dTable.ContainsKey(lDeclerations[i].Name))
                {
                    dTable.Add(lDeclerations[i].Name, index);
                    index++;
                }
            }
            
            return dTable;
        }


        public List<string> GenerateCode(List<LetStatement> lSimpleAssignments, List<VarDeclaration> lVars)
        {
            List<string> lAssembly = new List<string>();
            Dictionary<string, int> dSymbolTable = ComputeSymbolTable(lVars);
            foreach (LetStatement aSimple in lSimpleAssignments)
                lAssembly.AddRange(GenerateCode(aSimple, dSymbolTable));
            return lAssembly;
        }
/*
 *
 *
 * Implement!!!!!
 */
        public List<LetStatement> SimplifyExpressions(LetStatement s, List<VarDeclaration> lVars)
        {
            List<LetStatement> letAfterSimplfy = new List<LetStatement>();
            string str = s.Value.ToString();
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                    count++;
            }
            
            
            int index = 1;
            //complex expression
            while (count>1)
            {
                String substr = "";
                int i;
                for (i = str.IndexOf(')'); str[i] != '('; i--)
                {
                    substr = str[i]+substr;
                }
                substr = "("+substr;

                List<string> lLines = new List<string>();
                lLines.Add(substr);
                List<Token> lTokens = Tokenize(lLines);
                Stack<Token> sTokens = new Stack<Token>();
                for (int j = lTokens.Count - 1; j >= 0; j--)
                    sTokens.Push(lTokens[j]);

                Expression exp = Expression.Create(sTokens);
                exp.Parse(sTokens);
                LetStatement l = new LetStatement();
                l.Value = exp;
                l.Variable ="_"+index;
                letAfterSimplfy.Add(l);
                
                String newexp="";
                newexp = str.Substring(0, i) + "_" + index + str.Substring(str.IndexOf(')') + 1);
                index++;
                str = newexp;
                count--;
             
            }
            
            
            
            List<string> listLines = new List<string>();
            listLines.Add(str);
            List<Token> listTokens = Tokenize(listLines);
            Stack<Token> stackTokens = new Stack<Token>();
            for (int j = listTokens.Count - 1; j >= 0; j--)
                stackTokens.Push(listTokens[j]);

            Expression expi = Expression.Create(stackTokens);
            expi.Parse(stackTokens);
            LetStatement lett = new LetStatement();
            lett.Value = expi;
            lett.Variable = ((LetStatement)s).Variable;
            letAfterSimplfy.Add(lett);
            return letAfterSimplfy;
            
        }
        
        public List<LetStatement> SimplifyExpressions(List<LetStatement> ls, List<VarDeclaration> lVars)
        {
            List<LetStatement> lSimplified = new List<LetStatement>();
            foreach (LetStatement s in ls)
                lSimplified.AddRange(SimplifyExpressions(s, lVars));
            return lSimplified;
        }

 
        public LetStatement ParseStatement(List<Token> lTokens)
        {
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            LetStatement s = new LetStatement();
            s.Parse(sTokens);
            return s;
        }
        
        private bool Contains(string[] a, string s)
        {
            foreach (string s1 in a)
                if (s1 == s)
                    return true;
            return false;
        }



        private bool Contains(char[] a, char c)
        {
            foreach (char c1 in a)
                if (c1 == c)
                    return true;
            return false;
        }




        private string Next(string s, char[] aDelimiters, out string sToken, out int cChars)
        {
            cChars = 1;
            sToken = s[0] + "";
            if (Contains(aDelimiters, s[0]))
                return s.Substring(1);
            int i = 0;
            for (i = 1; i < s.Length; i++)
            {
                if (Contains(aDelimiters, s[i]))
                    return s.Substring(i);
                else
                    sToken += s[i];
                cChars++;
            }
            return null;
        }





        private List<string> Split(string s, char[] aDelimiters)
        {
            List<string> lTokens = new List<string>();
            while (s.Length > 0)
            {
                string sToken = "";
                int i = 0;
                for (i = 0; i < s.Length; i++)
                {
                    if (Contains(aDelimiters, s[i]))
                    {
                        if (sToken.Length > 0)
                            lTokens.Add(sToken);
                        lTokens.Add(s[i] + "");
                        break;
                    }
                    else
                        sToken += s[i];
                }
                if (i == s.Length)
                {
                    lTokens.Add(sToken);
                    s = "";
                }
                else
                    s = s.Substring(i + 1);
            }
            return lTokens;
        }

 
        public List<Token> Tokenize(string sLine, int iLine)
        {
            int Linenumber = iLine;
            int Positionnumber = 0;
            int isnumber;
            char[] Delimiters = { '*', '+', '-', '/', '<', '>', '&', '=', '|', '~', ',', ';' ,' ','(', ')', '[', ']', '{', '}' };
            List<Token> lTokens = new List<Token>();
            
            //tab check reduce substring
            while (true)
            {

                if (sLine.Contains("\t"))
                {
                    Positionnumber++;
                    sLine = sLine.Substring(1);
                }
                else
                    break;
            }
            
                        List<string> TokensToClassified = new List<string>();
                        TokensToClassified = Split(sLine, Delimiters);
                        for (int i=0; i < TokensToClassified.Count; i++)
                        {
                             //if white space ignore else classified
                             if(TokensToClassified[i].Equals(" ")){
                                 continue;
                             }             
                             //statment token
                            if (Token.Statements.Contains(TokensToClassified[i]))
                            {
                                Statement statment = new Statement(TokensToClassified[i], Linenumber, Positionnumber+i);
                        Positionnumber = Positionnumber + TokensToClassified[i].Length-1;
                                lTokens.Add(statment);
                                continue;
                            }
                            //vartype token
                            if (Token.VarTypes.Contains(TokensToClassified[i]))
                            {
                                VarType vartype = new VarType(TokensToClassified[i], Linenumber, Positionnumber+i);
                        Positionnumber = Positionnumber + TokensToClassified[i].Length-1;
                                lTokens.Add(vartype);
                                continue;
                            }
                            //constants token
                            if (Token.Constants.Contains(TokensToClassified[i]))
                            {
                                Constant constnt = new Constant(TokensToClassified[Positionnumber], Linenumber, Positionnumber+i);
                        Positionnumber = Positionnumber + TokensToClassified[i].Length-1;
                                lTokens.Add(constnt);
                                continue;
                            }
                            if (TokensToClassified[i].Length == 1)
                            {
                                char[] charArray = TokensToClassified[i].ToCharArray();
                                char c = charArray[0];
                                //opertors token
                                if (Token.Operators.Contains(c))
                                {
                                    Operator opertor = new Operator(c, Linenumber, Positionnumber+i);

                                    lTokens.Add(opertor);
                                    continue;
                                }
                                //parentheses token
                                if (Token.Parentheses.Contains(c))
                                {
                                    Parentheses parentheses = new Parentheses(c, Linenumber, Positionnumber+i);

                                    lTokens.Add(parentheses);
                                    continue;
                                }
                                //sepertor token
                                if (Token.Separators.Contains(c))
                                {
                                    Separator separator = new Separator(c, Linenumber, Positionnumber+i);
                          
                                    lTokens.Add(separator);
                                    continue;
                                }

                            }
                            //number token
                            if (int.TryParse(TokensToClassified[i], out isnumber))
                            {
                                Number number = new Number(TokensToClassified[i], Linenumber, Positionnumber+i);
                                Positionnumber = Positionnumber + TokensToClassified[i].Length-1;
                                lTokens.Add(number);
                                continue;
                            }
                            //identifier token
                            if ((Regex.IsMatch(TokensToClassified[i], @"^[a-zA-Z0-9_]+$") && (TokensToClassified[i][0] != (0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9))))
                            {
                                Identifier identifier = new Identifier(TokensToClassified[i], Linenumber, Positionnumber+i);
                                Positionnumber = Positionnumber + TokensToClassified[i].Length-1;
                                lTokens.Add(identifier);
                                continue;
                            }
                            else
                            {
                                 Positionnumber = Positionnumber + TokensToClassified[i].Length - 1;
                                 Token problematic = new Token();
                                 problematic.Line = Linenumber;
                                 problematic.Position = Positionnumber;
                                 SyntaxErrorException error = new SyntaxErrorException("syntax token is invalid", problematic);
                                 throw error;
                            }
                        }

            return lTokens;
        }

        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<Token> lTokens = new List<Token>();
            for (int i = 0; i < lCodeLines.Count; i++)
            {
                string sLine = lCodeLines[i];
                //comment check
                if (sLine.Contains("//") || sLine.Contains("/*") || sLine.Contains("*/"))
                {
                    continue;
                }
                List<Token> lLineTokens = Tokenize(sLine, i);
                lTokens.AddRange(lLineTokens);
            }
            return lTokens;
        }

    }
}
