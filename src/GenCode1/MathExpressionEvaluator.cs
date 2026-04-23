using Lab1_MathEvaluator.Interfaces;
using System;
using System.Globalization;

namespace Lab1_MathEvaluator.Implementations.GenCode1
{
    public class MathExpressionEvaluator : IMathExpressionEvaluator
    {
        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException("Выражение не может быть пустым или состоять только из пробелов.", nameof(expression));
            }

            int pos = 0;
            double result = ParseExpression(expression, ref pos);

            // Проверяем, что не осталось лишних символов после вычисления
            SkipWhitespace(expression, ref pos);
            if (pos < expression.Length)
            {
                throw new ArgumentException($"Некорректный символ '{expression[pos]}' в конце выражения.");
            }

            return result;
        }

        // Парсит выражение уровня сложения и вычитания (+, -)
        private double ParseExpression(string expression, ref int pos)
        {
            double left = ParseTerm(expression, ref pos);

            while (true)
            {
                SkipWhitespace(expression, ref pos);
                if (pos >= expression.Length) break;

                char op = expression[pos];
                if (op != '+' && op != '-')
                {
                    // Если встретили * или / здесь, значит нарушен приоритет или синтаксис
                    // (например, 2 + * 3), так как ParseTerm должен был их обработать
                    throw new ArgumentException($"Неожиданный оператор '{op}'.");
                }

                pos++; // Потребляем оператор
                double right = ParseTerm(expression, ref pos);

                if (op == '+')
                {
                    left += right;
                }
                else
                {
                    left -= right;
                }
            }

            return left;
        }

        // Парсит терм уровня умножения и деления (*, /)
        private double ParseTerm(string expression, ref int pos)
        {
            double left = ParseNumber(expression, ref pos);

            while (true)
            {
                SkipWhitespace(expression, ref pos);
                if (pos >= expression.Length) break;

                char op = expression[pos];
                if (op != '*' && op != '/')
                {
                    break; // Переходим на уровень выше (+, -)
                }

                pos++; // Потребляем оператор
                double right = ParseNumber(expression, ref pos);

                if (op == '*')
                {
                    left *= right;
                }
                else
                {
                    if (right == 0.0)
                    {
                        throw new DivideByZeroException("Деление на ноль недопустимо.");
                    }
                    left /= right;
                }
            }

            return left;
        }

        // Парсит число (целое или с плавающей точкой), включая унарный знак
        private double ParseNumber(string expression, ref int pos)
        {
            SkipWhitespace(expression, ref pos);
            if (pos >= expression.Length)
            {
                throw new ArgumentException("Ожидалось число, но выражение закончилось.");
            }

            int start = pos;
            bool hasSign = false;
	    _ = hasSign;

            // Обработка унарного знака
            if (expression[pos] == '+' || expression[pos] == '-')
            {
                hasSign = true;
                pos++;
                SkipWhitespace(expression, ref pos); // Пробелы между знаком и числом обычно нежелательны, но пропустим
                if (pos >= expression.Length)
                {
                    throw new ArgumentException("Ожидалось число после знака.");
                }
            }

            // Проверка на наличие цифр или точки
            if (!char.IsDigit(expression[pos]) && expression[pos] != '.')
            {
                throw new ArgumentException($"Некорректный символ '{expression[pos]}'. Ожидалось число.");
            }

            // Читаем цифры и точку
            bool hasDigit = false;
            bool hasDot = false;

            while (pos < expression.Length)
            {
                char c = expression[pos];
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    pos++;
                }
                else if (c == '.')
                {
                    if (hasDot)
                    {
                        throw new ArgumentException("В числе не может быть более одной точки.");
                    }
                    hasDot = true;
                    pos++;
                }
                else
                {
                    break; // Конец числа
                }
            }

            if (!hasDigit)
            {
                throw new ArgumentException("Число должно содержать хотя бы одну цифру.");
            }

            string numberStr = expression.Substring(start, pos - start);

            // Используем InvariantCulture для корректной обработки точки как разделителя
            if (!double.TryParse(numberStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            {
                throw new ArgumentException($"Не удалось преобразовать '{numberStr}' в число.");
            }

            return result;
        }

        private void SkipWhitespace(string expression, ref int pos)
        {
            while (pos < expression.Length && char.IsWhiteSpace(expression[pos]))
            {
                pos++;
            }
        }
    }
}