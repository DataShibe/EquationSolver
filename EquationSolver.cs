// This program is written by DataShibe on 29/01/2022

using System;
using System.Collections.Generic;

namespace EquationSolver
{
    class EquationSolver
    {
        public static void Main(string[] args)
        {
            // get the input from the user
            string equation = Console.ReadLine();

            // figure out, where the x is
            string[] sides = equation.Replace(" = ", "=").Split('=');
            string variableSide = "";
            string otherSide = "";
            foreach(string side in sides)
            {
                if (side.Contains("x")) variableSide = side;
                else otherSide = side;
            }

            // check if the x stands alone
            bool alone = isAlone(variableSide);

            // repeat while the variable is not alone
            char[] operators = new char[] {'+', '-', '*', '/', ' '};
            while (!alone)
            {
                string[] parts = variableSide.Split(' ');

                // figure out where the x is
                int xPosition = 0;
                for (int i = 0; i < parts.Length; i++) if (parts[i][0] == 'x') xPosition = i;

                string nextItem = "";
                int nextItemPosition = 0;

                // get the next operator that is either a '+' or a '-' and sits at the end of the side
                int lowestOperatorPosition = -1;
                for (int i = 0; i < parts.Length; i++)
                {
                    // check if part[i] is a operator
                    if (parts[i].IndexOfAny(operators) != -1)
                    {
                        // figure out which index the operator is in the array
                        int operatorNumber = 4;
                        for (int j = 0; j < operators.Length; j++) if (operators[j] == parts[i][0]) operatorNumber = j;
                  
                        if (operatorNumber <= 1 && i == parts.Length - 2 || i == 1) lowestOperatorPosition = i;
                    }
                }

                // get the next number or operator
                nextItemPosition = ((lowestOperatorPosition > xPosition) ? 1 : -1) + lowestOperatorPosition;
                nextItem = parts[nextItemPosition];

                int number = 0;
                char op = ' ';

                // if the item is on the outer left, get the number and operator and remove it from the side
                if (nextItemPosition < xPosition)
                {
                    number = Convert.ToInt32(nextItem);
                    op = parts[1][0];

                    int length = number.ToString().ToCharArray().Length + 1;
                    variableSide = variableSide.Remove(0, length + 2);
                }
                // if the item is on the outer right, get the number and operator and remove it from the side
                else
                {
                    number = Convert.ToInt32(nextItem);
                    op = parts[parts.Length - 2][0];

                    int length = number.ToString().ToCharArray().Length + 1;
                    variableSide = variableSide.Remove((variableSide.ToCharArray().Length) - (length + 2), length + 2);
                }

                // invert the operator [+ -> -]
                op = invertOperator(op, operators);
                // insert the operator and the number onto the side without the x
                otherSide = "(" + otherSide + ") " + op  + " " + number.ToString();

                alone = isAlone(variableSide);
            }

            // evaluate the side without the x
            double result = eval(otherSide);

            Console.WriteLine(variableSide + " = " + result);
        }

        public static bool isAlone(string side)
        {
            if (side.Equals("x")) return true;
            return false;
        }

        public static char invertOperator(char op, char[] operators)
        {
            char invertedOperator;
            int operatorNumber = 0;

            for (int i = 0; i < operators.Length; i++)
            {
                if (operators[i] == op) operatorNumber = i;
            }

            if (((operatorNumber + 1) % 2) != 0) invertedOperator = operators[operatorNumber + 1];
            else invertedOperator = operators[operatorNumber - 1];

            return invertedOperator;
        }

        public static double eval(string expression)
        {
            string[] parts = expression.Split(' ');

            // remove every parenthese with only one number like (25)
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("(") && parts[i].EndsWith(")"))
                {
                    parts[i] = parts[i].Replace(")", "");
                }
            }

            // thats the array for the order of the execution [for example multiplication comes before subtraction]
            char[] priorities = new char[] { '(', '*', '/', '+', '-' };

            // execute while there is more than one number
            while (parts.Length > 1)
            {
                // get the operator with the highest priority
                int priorityOffset = 0;
                int highestPriority = 5;
                for (int i = 0; i < parts.Length; i++)
                { 
                    for (int j = 0; j < priorities.Length; j++)
                    {
                        if (parts[i].Contains(priorities[j].ToString()) && j < highestPriority)
                        {
                            highestPriority = j;

                            if (priorities[j] == '(') priorityOffset = i;
                            else priorityOffset = i - 1;
                        }
                    }
                }

                // get the number before and after the operator and also the operator itself
                double n1 = Convert.ToDouble(parts[0 + priorityOffset].Replace("(", "").Replace(")", ""));
                double n2 = Convert.ToDouble(parts[2 + priorityOffset].Replace("(", "").Replace(")", ""));
                double res = 0;
                char op = parts[1 + priorityOffset][0];

                // calculate the result with the two numbers and the operator
                switch (op)
                {
                    case '+':
                        res = n1 + n2;
                        break;
                    case '-':
                        res = n1 - n2;
                        break;
                    case '*':
                        res = n1 * n2;
                        break;
                    case '/':
                        res = n1 / n2;
                        break;
                }

                // overwrite the first number with the result and remove the other number and operator
                List<string> p = new List<string>(parts);
                p[0 + priorityOffset] = res.ToString();
                p.RemoveAt(2 + priorityOffset);
                p.RemoveAt(1 + priorityOffset);

                parts = p.ToArray();
            }

            double result = Convert.ToDouble(parts[0]);

            return result;
        }
    }
}
