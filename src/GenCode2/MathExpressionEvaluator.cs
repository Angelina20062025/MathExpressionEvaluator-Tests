using Lab1_MathEvaluator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1_MathEvaluator.Implementations.GenCode2
{
    /// <summary>
    /// Evaluator for simple arithmetic expressions without parentheses.
    /// Supports +, -, *, / operators with proper precedence.
    /// </summary>
    public class MathExpressionEvaluator : IMathExpressionEvaluator
    {
        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression cannot be null or empty.");

            // Remove all whitespace
            string cleanedExpression = RemoveWhitespace(expression);

            if (string.IsNullOrEmpty(cleanedExpression))
                throw new ArgumentException("Expression contains only whitespace.");

            // Parse the expression into tokens
            List<string> tokens = Tokenize(cleanedExpression);

            if (tokens.Count == 0)
                throw new ArgumentException("Invalid expression format.");

            // First pass: handle multiplication and division
            List<string> afterMD = ProcessMD(tokens);

            // Second pass: handle addition and subtraction
            double result = ProcessAS(afterMD);

            return result;
        }

        private string RemoveWhitespace(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (!char.IsWhiteSpace(c))
                    result.Append(c);
            }
            return result.ToString();
        }

        private List<string> Tokenize(string expression)
        {
            List<string> tokens = new List<string>();
            StringBuilder currentNumber = new StringBuilder();

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                // Handle numbers (including decimal points and negative signs at start)
                if (char.IsDigit(c) || c == '.' || (c == '-' && i == 0) ||
                    (c == '-' && i > 0 && IsOperator(expression[i - 1].ToString())))
                {
                    currentNumber.Append(c);
                }
                else if (IsOperator(c.ToString()))
                {
                    // If we have a number in buffer, add it to tokens
                    if (currentNumber.Length > 0)
                    {
                        tokens.Add(currentNumber.ToString());
                        currentNumber.Clear();
                    }

                    // Add the operator
                    tokens.Add(c.ToString());
                }
                else
                {
                    throw new ArgumentException($"Invalid character in expression: '{c}'");
                }
            }

            // Add the last number if exists
            if (currentNumber.Length > 0)
            {
                tokens.Add(currentNumber.ToString());
            }

            // Validate token sequence
            if (!IsValidTokenSequence(tokens))
                throw new ArgumentException("Invalid expression format: incorrect operator/number sequence.");

            return tokens;
        }

        private bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

        private bool IsValidTokenSequence(List<string> tokens)
        {
            if (tokens.Count == 0) return false;

            // Expression should start with a number
            if (IsOperator(tokens[0]) && tokens[0] != "-")
                return false;

            // Handle negative numbers at start
            bool expectNumber = true;

            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];

                if (expectNumber)
                {
                    // Check if it's a valid number (including negative sign)
                    if (token == "-" && i < tokens.Count - 1 && IsNumber(tokens[i + 1]))
                    {
                        // Combine negative sign with next number
                        tokens[i] = "-" + tokens[i + 1];
                        tokens.RemoveAt(i + 1);
                    }
                    else if (!IsNumber(token))
                    {
                        return false;
                    }
                    expectNumber = false;
                }
                else
                {
                    if (!IsOperator(token))
                        return false;
                    expectNumber = true;
                }
            }

            // Expression should end with a number
            return !expectNumber;
        }

        private bool IsNumber(string token)
        {
            return double.TryParse(token, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out _);
        }

        private List<string> ProcessMD(List<string> tokens)
        {
            List<string> result = new List<string>();
            int i = 0;

            while (i < tokens.Count)
            {
                if (i + 2 < tokens.Count && (tokens[i + 1] == "*" || tokens[i + 1] == "/"))
                {
                    // Perform multiplication or division
                    double left = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture);
                    string op = tokens[i + 1];
                    double right = double.Parse(tokens[i + 2], System.Globalization.CultureInfo.InvariantCulture);

                    double value;
                    if (op == "*")
                    {
                        value = left * right;
                    }
                    else // "/"
                    {
                        if (right == 0)
                            throw new DivideByZeroException("Division by zero is not allowed.");
                        value = left / right;
                    }

                    result.Add(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    i += 3;
                }
                else
                {
                    result.Add(tokens[i]);
                    i++;
                }
            }

            return result;
        }

        private double ProcessAS(List<string> tokens)
        {
            if (tokens.Count == 0)
                throw new ArgumentException("Invalid expression format.");

            double result = double.Parse(tokens[0], System.Globalization.CultureInfo.InvariantCulture);

            for (int i = 1; i < tokens.Count; i += 2)
            {
                if (i + 1 >= tokens.Count)
                    throw new ArgumentException("Invalid expression format: incomplete expression.");

                string op = tokens[i];
                double nextNumber = double.Parse(tokens[i + 1], System.Globalization.CultureInfo.InvariantCulture);

                switch (op)
                {
                    case "+":
                        result += nextNumber;
                        break;
                    case "-":
                        result -= nextNumber;
                        break;
                    default:
                        throw new ArgumentException($"Unexpected operator: {op}");
                }
            }

            return result;
        }
    }
}