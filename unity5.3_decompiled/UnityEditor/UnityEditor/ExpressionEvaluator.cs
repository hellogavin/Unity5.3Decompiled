namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class ExpressionEvaluator
    {
        private static readonly Operator[] s_Operators = new Operator[] { new Operator('-', 2, 2, Associativity.Left), new Operator('+', 2, 2, Associativity.Left), new Operator('/', 3, 2, Associativity.Left), new Operator('*', 3, 2, Associativity.Left), new Operator('%', 3, 2, Associativity.Left), new Operator('^', 4, 2, Associativity.Right), new Operator('u', 4, 1, Associativity.Left) };

        private static Operator CharToOperator(char character)
        {
            foreach (Operator @operator in s_Operators)
            {
                if (@operator.character == character)
                {
                    return @operator;
                }
            }
            return new Operator();
        }

        public static T Evaluate<T>(string expression)
        {
            T result = default(T);
            if (!TryParse<T>(expression, out result))
            {
                expression = PreFormatExpression(expression);
                result = Evaluate<T>(InfixToRPN(FixUnaryOperators(ExpressionToTokens(expression))));
            }
            return result;
        }

        private static T Evaluate<T>(string[] tokens)
        {
            T local2;
            Stack<string> source = new Stack<string>();
            foreach (string str in tokens)
            {
                if (IsOperator(str))
                {
                    Operator @operator = CharToOperator(str[0]);
                    List<T> list = new List<T>();
                    bool flag = true;
                    while (((source.LongCount<string>() > 0L) && !IsCommand(source.Peek())) && (list.Count < @operator.inputs))
                    {
                        T local;
                        flag &= TryParse<T>(source.Pop(), out local);
                        list.Add(local);
                    }
                    list.Reverse();
                    if (!flag || (list.Count != @operator.inputs))
                    {
                        return default(T);
                    }
                    source.Push(Evaluate<T>(list.ToArray(), str[0]).ToString());
                }
                else
                {
                    source.Push(str);
                }
            }
            if ((source.LongCount<string>() == 1L) && TryParse<T>(source.Pop(), out local2))
            {
                return local2;
            }
            return default(T);
        }

        private static T Evaluate<T>(T[] values, char oper)
        {
            char ch;
            if (typeof(T) != typeof(float))
            {
                if (typeof(T) != typeof(int))
                {
                    goto Label_0366;
                }
                if (values.Length == 1)
                {
                    ch = oper;
                    if (ch == 'u')
                    {
                        return (T) (((int) values[0]) * -1);
                    }
                    goto Label_0366;
                }
                if (values.Length != 2)
                {
                    goto Label_0366;
                }
                ch = oper;
                switch (ch)
                {
                    case '*':
                        return (T) (((int) values[0]) * ((int) values[1]));

                    case '+':
                        return (T) (((int) values[0]) + ((int) values[1]));

                    case '-':
                        return (T) (((int) values[0]) - ((int) values[1]));

                    case '/':
                        return (T) (((int) values[0]) / ((int) values[1]));
                }
            }
            else
            {
                if (values.Length == 1)
                {
                    ch = oper;
                    if (ch == 'u')
                    {
                        return (T) (((float) values[0]) * -1f);
                    }
                    goto Label_0366;
                }
                if (values.Length != 2)
                {
                    goto Label_0366;
                }
                ch = oper;
                switch (ch)
                {
                    case '*':
                        return (T) (((float) values[0]) * ((float) values[1]));

                    case '+':
                        return (T) (((float) values[0]) + ((float) values[1]));

                    case '-':
                        return (T) (((float) values[0]) - ((float) values[1]));

                    case '/':
                        return (T) (((float) values[0]) / ((float) values[1]));
                }
                switch (ch)
                {
                    case '%':
                        return (T) (((float) values[0]) % ((float) values[1]));

                    case '^':
                        return (T) Mathf.Pow((float) values[0], (float) values[1]);

                    default:
                        goto Label_0366;
                }
            }
            switch (ch)
            {
                case '%':
                    return (T) (((int) values[0]) % ((int) values[1]));

                case '^':
                    return (T) ((int) Math.Pow((double) ((int) values[0]), (double) ((int) values[1])));
            }
        Label_0366:
            if (typeof(T) != typeof(double))
            {
                if (typeof(T) != typeof(long))
                {
                    goto Label_06D1;
                }
                if (values.Length == 1)
                {
                    ch = oper;
                    if (ch == 'u')
                    {
                        return (T) (((long) values[0]) * -1L);
                    }
                    goto Label_06D1;
                }
                if (values.Length != 2)
                {
                    goto Label_06D1;
                }
                ch = oper;
                switch (ch)
                {
                    case '*':
                        return (T) (((long) values[0]) * ((long) values[1]));

                    case '+':
                        return (T) (((long) values[0]) + ((long) values[1]));

                    case '-':
                        return (T) (((long) values[0]) - ((long) values[1]));

                    case '/':
                        return (T) (((long) values[0]) / ((long) values[1]));
                }
            }
            else
            {
                if (values.Length == 1)
                {
                    ch = oper;
                    if (ch == 'u')
                    {
                        return (T) (((double) values[0]) * -1.0);
                    }
                    goto Label_06D1;
                }
                if (values.Length != 2)
                {
                    goto Label_06D1;
                }
                ch = oper;
                switch (ch)
                {
                    case '*':
                        return (T) (((double) values[0]) * ((double) values[1]));

                    case '+':
                        return (T) (((double) values[0]) + ((double) values[1]));

                    case '-':
                        return (T) (((double) values[0]) - ((double) values[1]));

                    case '/':
                        return (T) (((double) values[0]) / ((double) values[1]));
                }
                switch (ch)
                {
                    case '%':
                        return (T) (((double) values[0]) % ((double) values[1]));

                    case '^':
                        return (T) Math.Pow((double) values[0], (double) values[1]);

                    default:
                        goto Label_06D1;
                }
            }
            switch (ch)
            {
                case '%':
                    return (T) (((long) values[0]) % ((long) values[1]));

                case '^':
                    return (T) ((long) Math.Pow((double) ((long) values[0]), (double) ((long) values[1])));
            }
        Label_06D1:
            return default(T);
        }

        private static string[] ExpressionToTokens(string expression)
        {
            List<string> list = new List<string>();
            string item = string.Empty;
            for (int i = 0; i < expression.Length; i++)
            {
                char character = expression[i];
                if (IsCommand(character))
                {
                    if (item.Length > 0)
                    {
                        list.Add(item);
                    }
                    list.Add(character.ToString());
                    item = string.Empty;
                }
                else if (character != ' ')
                {
                    item = item + character;
                }
            }
            if (item.Length > 0)
            {
                list.Add(item);
            }
            return list.ToArray();
        }

        private static string[] FixUnaryOperators(string[] tokens)
        {
            if (tokens.Length != 0)
            {
                if (tokens[0] == "-")
                {
                    tokens[0] = "u";
                }
                for (int i = 1; i < (tokens.Length - 1); i++)
                {
                    string str = tokens[i];
                    string token = tokens[i - 1];
                    string str3 = tokens[i - 1];
                    if ((str == "-") && ((IsCommand(token) || (str3 == "(")) || (str3 == ")")))
                    {
                        tokens[i] = "u";
                    }
                }
            }
            return tokens;
        }

        private static string[] InfixToRPN(string[] tokens)
        {
            Stack<char> source = new Stack<char>();
            Stack<string> stack2 = new Stack<string>();
            foreach (string str in tokens)
            {
                if (IsCommand(str))
                {
                    char item = str[0];
                    switch (item)
                    {
                        case '(':
                        {
                            source.Push(item);
                            continue;
                        }
                        case ')':
                        {
                            while ((source.LongCount<char>() > 0L) && (source.Peek() != '('))
                            {
                                stack2.Push(source.Pop().ToString());
                            }
                            if (source.LongCount<char>() > 0L)
                            {
                                source.Pop();
                            }
                            continue;
                        }
                    }
                    Operator newOperator = CharToOperator(item);
                    while (NeedToPop(source, newOperator))
                    {
                        stack2.Push(source.Pop().ToString());
                    }
                    source.Push(item);
                }
                else
                {
                    stack2.Push(str);
                }
            }
            while (source.LongCount<char>() > 0L)
            {
                stack2.Push(source.Pop().ToString());
            }
            return stack2.Reverse<string>().ToArray<string>();
        }

        private static bool IsCommand(char character)
        {
            if ((character != '(') && (character != ')'))
            {
                return IsOperator(character);
            }
            return true;
        }

        private static bool IsCommand(string token)
        {
            if (token.Length != 1)
            {
                return false;
            }
            return IsCommand(token[0]);
        }

        private static bool IsOperator(char character)
        {
            foreach (Operator @operator in s_Operators)
            {
                if (@operator.character == character)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsOperator(string token)
        {
            if (token.Length != 1)
            {
                return false;
            }
            return IsOperator(token[0]);
        }

        private static bool NeedToPop(Stack<char> operatorStack, Operator newOperator)
        {
            if (operatorStack.LongCount<char>() > 0L)
            {
                Operator @operator = CharToOperator(operatorStack.Peek());
                if (IsOperator(@operator.character) && (((newOperator.associativity == Associativity.Left) && (newOperator.presedence <= @operator.presedence)) || ((newOperator.associativity == Associativity.Right) && (newOperator.presedence < @operator.presedence))))
                {
                    return true;
                }
            }
            return false;
        }

        private static string PreFormatExpression(string expression)
        {
            string str = expression;
            str = str.Trim();
            if (str.Length != 0)
            {
                char character = str[str.Length - 1];
                if (IsOperator(character))
                {
                    char[] trimChars = new char[] { character };
                    str = str.TrimEnd(trimChars);
                }
            }
            return str;
        }

        private static bool TryParse<T>(string expression, out T result)
        {
            expression = expression.Replace(',', '.');
            bool flag = false;
            result = default(T);
            if (typeof(T) == typeof(float))
            {
                float num = 0f;
                flag = float.TryParse(expression, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                result = (T) num;
                return flag;
            }
            if (typeof(T) == typeof(int))
            {
                int num2 = 0;
                flag = int.TryParse(expression, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num2);
                result = (T) num2;
                return flag;
            }
            if (typeof(T) == typeof(double))
            {
                double num3 = 0.0;
                flag = double.TryParse(expression, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num3);
                result = (T) num3;
                return flag;
            }
            if (typeof(T) == typeof(long))
            {
                long num4 = 0L;
                flag = long.TryParse(expression, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num4);
                result = (T) num4;
            }
            return flag;
        }

        private enum Associativity
        {
            Left,
            Right
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Operator
        {
            public char character;
            public int presedence;
            public ExpressionEvaluator.Associativity associativity;
            public int inputs;
            public Operator(char character, int presedence, int inputs, ExpressionEvaluator.Associativity associativity)
            {
                this.character = character;
                this.presedence = presedence;
                this.inputs = inputs;
                this.associativity = associativity;
            }
        }
    }
}

