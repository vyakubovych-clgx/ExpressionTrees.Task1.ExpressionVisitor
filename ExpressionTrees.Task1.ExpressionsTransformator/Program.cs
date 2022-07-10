/*
 * Create a class based on ExpressionVisitor, which makes expression tree transformation:
 * 1. converts expressions like <variable> + 1 to increment operations, <variable> - 1 - into decrement operations.
 * 2. changes parameter values in a lambda expression to constants, taking the following as transformation parameters:
 *    - source expression;
 *    - dictionary: <parameter name: value for replacement>
 * The results could be printed in console or checked via Debugger using any Visualizer.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Expression Visitor for increment/decrement.");
            Console.WriteLine();

            Expression<Func<int, int>> simpleExpression = a => (a + 1) * 5 + 10 * (a - 1);
            OutputBeforeAndAfterTransformation(simpleExpression);

            Expression<Func<double, double, double, double>> complexExpression =
                (a, b, c) => (a + b + 1 - c - 1) * 4 / (c + 1 + 1);
            OutputBeforeAndAfterTransformation(complexExpression);

            var replacements = new Dictionary<string, double>
            {
                {"pi", Math.Round(Math.PI, 2)},
                {"e", Math.Round(Math.E, 2)},
                {"phi", Math.Round((1 + Math.Sqrt(5)) / 2, 2)}
            };

            Expression<Func<double, double, double, double, double>> expressionWithReplacements =
                (a, pi, b, e) => (a + 1) * pi + (b - 1 - e) / pi;
            OutputBeforeAndAfterTransformation(expressionWithReplacements, replacements);

            Expression<Func<IQueryable<double>, double, double, double, double, int, double>> veryComplexExpression =
                (arr, pi, phi, x, e, d) =>
                    Math.Pow(e, arr.Take(d - 1).Where(n => Math.Sqrt(n + 1 + phi) > pi).ToList().IndexOf(e + 1)) +
                    Math.Round(x - 1, d + 1);
            OutputBeforeAndAfterTransformation(veryComplexExpression, replacements);
            Console.ReadLine();
        }

        static void OutputBeforeAndAfterTransformation(Expression expression, Dictionary<string, double> replacements = null)
        {
            Console.WriteLine($"Before transformation: {expression}");
            var transformedExpression = new IncDecExpressionVisitor().Transform(expression, replacements);
            Console.WriteLine(
                $"After transformation: {transformedExpression}");
            Console.WriteLine();
        }
    }
}
