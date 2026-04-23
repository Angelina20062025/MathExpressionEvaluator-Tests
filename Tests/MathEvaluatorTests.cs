using NUnit.Framework;
using Lab1_MathEvaluator.Interfaces;
using Lab1_MathEvaluator.Implementations.GenCode3;

namespace Lab1_MathEvaluator.Tests;

[TestFixture]
public class MathEvaluatorTests
{
    private IMathExpressionEvaluator evaluator;

    [SetUp]
    public void Setup()
    {
        evaluator = new MathExpressionEvaluator();
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_SimpleAddition_ReturnsCorrectSum()
    {
        // Проверка: базовое сложение двух чисел
        // Данные: "2+3"
        // Ожидаемый результат: 5
        double result = evaluator.Evaluate("2+3");
        Assert.That(result, Is.EqualTo(5.0).Within(1e-10));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_SimpleSubtraction_ReturnsCorrectDifference()
    {
        // Проверка: базовое вычитание
        // Данные: "10-4"
        // Ожидаемый результат: 6
        double result = evaluator.Evaluate("10-4");
        Assert.That(result, Is.EqualTo(6.0).Within(1e-10));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_SimpleMultiplication_ReturnsCorrectProduct()
    {
        // Проверка: базовое умножение
        // Данные: "6*7"
        // Ожидаемый результат: 42
        double result = evaluator.Evaluate("6*7");
        Assert.That(result, Is.EqualTo(42.0).Within(1e-10));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_SimpleDivision_ReturnsCorrectQuotient()
    {
        // Проверка: базовое деление
        // Данные: "15/3"
        // Ожидаемый результат: 5
        double result = evaluator.Evaluate("15/3");
        Assert.That(result, Is.EqualTo(5.0).Within(1e-10));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_ExpressionWithSpaces_ReturnsCorrectResult()
    {
        // Проверка: игнорирование пробелов
        // Данные: " 5 + 8 "
        // Ожидаемый результат: 13
        double result = evaluator.Evaluate(" 5 + 8 ");
        Assert.That(result, Is.EqualTo(13.0).Within(1e-10));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_ExpressionWithAllOperations_RespectsOperatorPrecedence()
    {
        // Проверка: приоритет операций (* и / перед + и -)
        // Данные: "2+3*4-6/2"
        // Ожидаемый результат: 2 + 12 - 3 = 11
        double result = evaluator.Evaluate("2+3*4-6/2");
        Assert.That(result, Is.EqualTo(11.0).Within(1e-10));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_DivisionByZero_ThrowsDivideByZeroException()
    {
        // Проверка: обработка деления на ноль
        // Данные: "10/0"
        // Ожидаемый результат: DivideByZeroException
        Assert.Throws<DivideByZeroException>(() => evaluator.Evaluate("10/0"));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_EmptyString_ThrowsArgumentException()
    {
        // Проверка: обработка пустой строки
        // Данные: ""
        // Ожидаемый результат: ArgumentException
        Assert.Throws<ArgumentException>(() => evaluator.Evaluate(""));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_NullInput_ThrowsArgumentException()
    {
        // Проверка: обработка null
        // Данные: null
        // Ожидаемый результат: ArgumentException
        Assert.Throws<ArgumentException>(() => evaluator.Evaluate(null!));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_InvalidCharacters_ThrowsArgumentException()
    {
        // Проверка: обработка недопустимых символов
        // Данные: "2+a"
        // Ожидаемый результат: ArgumentException
        Assert.Throws<ArgumentException>(() => evaluator.Evaluate("2+a"));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_ConsecutiveOperators_ThrowsArgumentException()
    {
        // Проверка: два оператора подряд
        // Данные: "2+-3"
        // Ожидаемый результат: ArgumentException
        Assert.Throws<ArgumentException>(() => evaluator.Evaluate("2+-3"));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_MissingNumber_ThrowsArgumentException()
    {
        // Проверка: выражение заканчивается оператором
        // Данные: "5+"
        // Ожидаемый результат: ArgumentException
        Assert.Throws<ArgumentException>(() => evaluator.Evaluate("5+"));
    }

    [Test]
    [Category("BlackBox")]
    public void Evaluate_FloatingPointNumbers_ReturnsCorrectResult()
    {
        // Проверка: работа с числами с плавающей точкой
        // Данные: "2.5*4.2"
        // Ожидаемый результат: 10.5
        double result = evaluator.Evaluate("2.5*4.2");
        Assert.That(result, Is.EqualTo(10.5).Within(1e-10));
    }

    [Test]
    [Category("WhiteBox")]
    public void Evaluate_ComplexExpressionWithMultipleOperations_ReturnsCorrectResult()
    {
        // Проверка: сложное выражение со многими операциями
        // Данные: "10-4*2+12/3-1"
        // Ожидаемый результат: 10 - 8 + 4 - 1 = 5
        double result = evaluator.Evaluate("10-4*2+12/3-1");
        Assert.That(result, Is.EqualTo(5.0).Within(1e-10));
    }

    [Test]
    [Category("WhiteBox")]
    public void Evaluate_ExpressionStartingWithOperator_ThrowsArgumentException()
    {
        // Проверка: выражение начинается с оператора (кроме минуса)
        // Данные: "*5+3"
        // Ожидаемый результат: ArgumentException
        Assert.Throws<ArgumentException>(() => evaluator.Evaluate("*5+3"));
    }

    [Test]
    [Category("WhiteBox")]
    public void Evaluate_OnlyWhitespace_ThrowsArgumentException()
    {
        // Проверка: строка только из пробелов
        // Данные: "   "
        // Ожидаемый результат: ArgumentException
        Assert.Throws<ArgumentException>(() => evaluator.Evaluate("   "));
    }

    [Test]
    [Category("WhiteBox")]
    public void Evaluate_DivisionByZeroInComplexExpression_ThrowsDivideByZeroException()
    {
        // Проверка: деление на ноль в середине выражения
        // Данные: "5+10/0-3"
        // Ожидаемый результат: DivideByZeroException
        Assert.Throws<DivideByZeroException>(() => evaluator.Evaluate("5+10/0-3"));
    }

    [Test]
    [Category("WhiteBox")]
    public void Evaluate_MultipleDecimalPoints_ThrowsArgumentException()
    {
        // Проверка: число с несколькими десятичными точками
        // Данные: "2..5+3"
        // Ожидаемый результат: ArgumentException
        Assert.Throws<ArgumentException>(() => evaluator.Evaluate("2..5+3"));
    }

    [Test]
    [Category("WhiteBox")]
    public void Evaluate_VeryLargeNumbers_HandlesOverflowGracefully()
    {
        // Проверка: обработка очень больших чисел
        try
        {
            double result = evaluator.Evaluate("9999999999999999*9999999999999999");
        }
        catch (Exception ex) when (ex is OverflowException || ex is ArgumentException)
        {
            Assert.Pass("Произошло исключение о переполнении");
        }
    }
}