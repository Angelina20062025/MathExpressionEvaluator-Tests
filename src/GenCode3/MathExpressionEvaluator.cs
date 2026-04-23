using Lab1_MathEvaluator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lab1_MathEvaluator.Implementations.GenCode3
{
    public class MathExpressionEvaluator : IMathExpressionEvaluator
    {
        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Выражение не может быть пустым или содержать только пробелы.");

            // Удаляем все пробелы
            expression = Regex.Replace(expression, @"\s+", "");

            if (string.IsNullOrEmpty(expression))
                throw new ArgumentException("Выражение не может состоять только из пробелов.");

            // Проверяем корректность выражения: должны быть только цифры, точки и операторы
            if (!Regex.IsMatch(expression, @"^[\d.+\-*/]+$"))
                throw new ArgumentException("Недопустимые символы в выражении.");

            // Проверяем, что выражение не начинается и не заканчивается оператором (кроме минуса в начале)
            if (expression.Length > 0 && "+-*/".Contains(expression[expression.Length - 1]))
                throw new ArgumentException("Выражение не может заканчиваться оператором.");
            if (expression.Length > 1 && "*/".Contains(expression[0]))
                throw new ArgumentException("Выражение не может начинаться с оператора * или /.");

            // Разбиваем выражение на числа и операторы
            var tokens = Tokenize(expression);

            // Сначала обрабатываем умножение и деление
            tokens = ProcessMultiplicationAndDivision(tokens);

            // Затем обрабатываем сложение и вычитание
            return ProcessAdditionAndSubtraction(tokens);
        }

        private List<object> Tokenize(string expression)
        {
            var tokens = new List<object>();
            var numberBuffer = new System.Text.StringBuilder();

            foreach (char c in expression)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    numberBuffer.Append(c);
                }
                else
                {
                    // Если есть накопленное число, добавляем его
                    if (numberBuffer.Length > 0)
                    {
                        if (!double.TryParse(numberBuffer.ToString(), out double number))
                            throw new ArgumentException($"Некорректное число: {numberBuffer}");
                        tokens.Add(number);
                        numberBuffer.Clear();
                    }

                    // Добавляем оператор
                    tokens.Add(c);
                }
            }

            // Добавляем последнее число, если есть
            if (numberBuffer.Length > 0)
            {
                if (!double.TryParse(numberBuffer.ToString(), out double number))
                    throw new ArgumentException($"Некорректное число: {numberBuffer}");
                tokens.Add(number);
            }

            return tokens;
        }

        private List<object> ProcessMultiplicationAndDivision(List<object> tokens)
        {
            var result = new List<object>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is double num)
                {
                    result.Add(num);
                }
                else if (tokens[i] is char op)
                {
                    if (op == '*' || op == '/')
                    {
                        // Должен быть предыдущий операнд и следующий
                        if (result.Count == 0 || i + 1 >= tokens.Count || !(tokens[i + 1] is double nextNum))
                            throw new ArgumentException("Некорректное выражение: отсутствует операнд.");

                        double leftOperand = (double)result[result.Count - 1];
                        result.RemoveAt(result.Count - 1);

                        if (op == '/' && nextNum == 0)
                            throw new DivideByZeroException("Деление на ноль.");

                        double resultValue = op == '*' ? leftOperand * nextNum : leftOperand / nextNum;
                        result.Add(resultValue);
                        i++; // Пропускаем следующий токен, так как он уже обработан
                    }
                    else
                    {
                        result.Add(op);
                    }
                }
            }

            return result;
        }

        private double ProcessAdditionAndSubtraction(List<object> tokens)
        {
            if (tokens.Count == 0)
                throw new ArgumentException("Пустое выражение после обработки.");

            double result = tokens[0] is double ? (double)tokens[0] : throw new ArgumentException("Выражение должно начинаться с числа.");

            for (int i = 1; i < tokens.Count; i += 2)
            {
                if (i + 1 >= tokens.Count)
                    throw new ArgumentException("Некорректное выражение: отсутствует операнд.");

                if (!(tokens[i] is char op && (op == '+' || op == '-')))
                    throw new ArgumentException($"Недопустимый оператор: {tokens[i]}");

                if (!(tokens[i + 1] is double rightOperand))
                    throw new ArgumentException("Операнд должен быть числом.");

                result = op == '+' ? result + rightOperand : result - rightOperand;
            }

            return result;
        }
    }

    //public interface IMathExpressionEvaluator
    //{
    //    double Evaluate(string expression);
    //}
}