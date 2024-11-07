using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TransformCalculator
{
    // メインエントリーポイント
    public static float EvaluateSingleExpression(string expression, int index, int cloneCount)
    {
        expression = PreprocessExpression(expression, index, cloneCount);
        return EvaluateProcessedExpression(expression, index, cloneCount);
    }

    // 前処理：定数と変数の置き換え、空白の削除
    private static string PreprocessExpression(string expression, int index, int cloneCount)
    {
        expression = expression.Replace("PI", RoundToThreeDecimalPlaces(Mathf.PI).ToString());
        expression = expression.Replace("C", RoundToThreeDecimalPlaces(cloneCount).ToString());
        expression = expression.Replace("#", RoundToThreeDecimalPlaces(index).ToString());
        return expression.Replace(" ", string.Empty);
    }

    // 小数点以下3桁に丸める関数
    private static float RoundToThreeDecimalPlaces(float value)
    {
        return Mathf.Round(value * 1000f) / 1000f;
    }

    // 前処理後の数式を評価
    private static float EvaluateProcessedExpression(string expression, int index, int cloneCount)
    {
        try
        {
            expression = ParseConditionalOperators(expression, index, cloneCount);  // 三項演算子を先に解析
            expression = ParseMathFunctions(expression, index, cloneCount);
            expression = EvaluateParentheses(expression, index, cloneCount);
            expression = ParseUnaryMinus(expression);
            expression = ParsePowerOperators(expression);           // べき算の解析
            expression = ParseShiftOperators(expression);           // シフト演算の解析
            expression = ParseBitwiseOperators(expression);         // ビット演算の解析
            expression = ConvertDivisionToMultiplication(expression);
            expression = ParseBinaryOperators(expression);

            if (float.TryParse(expression, out float result))
            {
                return RoundToThreeDecimalPlaces(result);
            }
            else
            {
                expression = EvaluateRemainingExpression(expression);
                if (float.TryParse(expression, out result))
                {
                    return RoundToThreeDecimalPlaces(result);
                }
                throw new ArgumentException($"Unable to parse the final result of the expression '{expression}'");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in evaluating expression '{expression}' at index {index}: {ex.Message}", ex);
        }
    }

    // ビット演算子（&、|、^）の解析
    private static string ParseBitwiseOperators(string expression)
    {
        try
        {
            expression = Regex.Replace(expression, @"(\d+)\s*&\s*(\d+)", match =>
            {
                int leftValue = int.Parse(match.Groups[1].Value);
                int rightValue = int.Parse(match.Groups[2].Value);
                int result = leftValue & rightValue;
                return result.ToString();
            });

            expression = Regex.Replace(expression, @"(\d+)\s*\|\s*(\d+)", match =>
            {
                int leftValue = int.Parse(match.Groups[1].Value);
                int rightValue = int.Parse(match.Groups[2].Value);
                int result = leftValue | rightValue;
                return result.ToString();
            });

            expression = Regex.Replace(expression, @"(\d+)\s*\^\s*(\d+)", match =>
            {
                int leftValue = int.Parse(match.Groups[1].Value);
                int rightValue = int.Parse(match.Groups[2].Value);
                int result = leftValue ^ rightValue;
                return result.ToString();
            });

            return expression;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in parsing bitwise operators.", ex);
        }
    }

    // 三項演算子（条件式）を再帰的に解析
    private static string ParseConditionalOperators(string expression, int index, int cloneCount)
    {
        while (Regex.IsMatch(expression, @"\(([^()]+?)\s*==\s*([^()]+?)\s*\?\s*([^:]+?)\s*:\s*([^)]+?)\)"))
        {
            expression = Regex.Replace(expression, @"\(([^()]+?)\s*==\s*([^()]+?)\s*\?\s*([^:]+?)\s*:\s*([^)]+?)\)", match =>
            {
                string conditionExpr = match.Groups[1].Value + " == " + match.Groups[2].Value;
                string trueResultExpr = match.Groups[3].Value;
                string falseResultExpr = match.Groups[4].Value;

                bool condition = Mathf.Approximately(
                    EvaluateProcessedExpression(match.Groups[1].Value, index, cloneCount),
                    EvaluateProcessedExpression(match.Groups[2].Value, index, cloneCount)
                );

                return RoundToThreeDecimalPlaces(
                    EvaluateProcessedExpression(condition ? trueResultExpr : falseResultExpr, index, cloneCount)
                ).ToString();
            });
        }
        return expression;
    }

    // 括弧を優先的に評価
    private static string EvaluateParentheses(string expression, int index, int cloneCount)
    {
        while (Regex.IsMatch(expression, @"\(([^()]+)\)"))
        {
            expression = Regex.Replace(expression, @"\(([^()]+)\)", match =>
            {
                string innerExpression = match.Groups[1].Value;
                return RoundToThreeDecimalPlaces(EvaluateProcessedExpression(innerExpression, index, cloneCount)).ToString();
            });
        }
        return expression;
    }

    // 単項演算子としてのマイナスを解析
    private static string ParseUnaryMinus(string expression)
    {
        try
        {
            expression = Regex.Replace(expression, @"(?<![\d)])-", "0-");
            return expression;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in parsing unary minus.", ex);
        }
    }

    // シフト演算の解析 (優先順位の低い演算)
    private static string ParseShiftOperators(string expression)
    {
        try
        {
            expression = Regex.Replace(expression, @"(\d+)\s*<<\s*(\d+)", match =>
            {
                int leftValue = int.Parse(match.Groups[1].Value);
                int shiftAmount = int.Parse(match.Groups[2].Value);
                int result = leftValue << shiftAmount;
                return result.ToString();
            });

            expression = Regex.Replace(expression, @"(\d+)\s*>>\s*(\d+)", match =>
            {
                int leftValue = int.Parse(match.Groups[1].Value);
                int shiftAmount = int.Parse(match.Groups[2].Value);
                int result = leftValue >> shiftAmount;
                return result.ToString();
            });

            return expression;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in parsing shift operators.", ex);
        }
    }

    // べき算の解析（^ 演算子）
    private static string ParsePowerOperators(string expression)
    {
        try
        {
            expression = Regex.Replace(expression, @"(\d+(\.\d+)?)\s*\^\s*(\d+(\.\d+)?)", match =>
            {
                float baseValue = float.Parse(match.Groups[1].Value);
                float exponent = float.Parse(match.Groups[3].Value);
                float result = Mathf.Pow(baseValue, exponent);
                return RoundToThreeDecimalPlaces(result).ToString();
            });
            return expression;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in parsing power operators.", ex);
        }
    }

    // 割り算を小数に変換して掛け算に変更
    private static string ConvertDivisionToMultiplication(string expression)
    {
        try
        {
            expression = Regex.Replace(expression, @"(\d+(\.\d+)?)\s*/\s*(\d+(\.\d+)?)", match =>
            {
                float leftValue = float.Parse(match.Groups[1].Value);
                float rightValue = float.Parse(match.Groups[3].Value);

                if (rightValue == 0)
                {
                    throw new DivideByZeroException("Division by zero detected.");
                }

                float result = leftValue * (1 / rightValue);
                return RoundToThreeDecimalPlaces(result).ToString();
            });
            return expression;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in converting division to multiplication.", ex);
        }
    }

    // 二項演算子（+、-、*）を解析
    private static string ParseBinaryOperators(string expression)
    {
        try
        {
            expression = Regex.Replace(expression, @"(\d+(\.\d+)?)\s*\*\s*(\d+(\.\d+)?)", match =>
            {
                float result = float.Parse(match.Groups[1].Value) * float.Parse(match.Groups[3].Value);
                return RoundToThreeDecimalPlaces(result).ToString();
            });

            expression = Regex.Replace(expression, @"(\d+(\.\d+)?)\s*%\s*(\d+(\.\d+)?)", match =>
            {
                float result = float.Parse(match.Groups[1].Value) % float.Parse(match.Groups[3].Value);
                return RoundToThreeDecimalPlaces(result).ToString();
            });

            expression = Regex.Replace(expression, @"(\d+(\.\d+)?)\s*\+\s*(\d+(\.\d+)?)", match =>
            {
                float result = float.Parse(match.Groups[1].Value) + float.Parse(match.Groups[3].Value);
                return RoundToThreeDecimalPlaces(result).ToString();
            });

            expression = Regex.Replace(expression, @"(\d+(\.\d+)?)\s*-\s*(\d+(\.\d+)?)", match =>
            {
                float result = float.Parse(match.Groups[1].Value) - float.Parse(match.Groups[3].Value);
                return RoundToThreeDecimalPlaces(result).ToString();
            });

            return expression;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in parsing binary operators.", ex);
        }
    }

    // 数式の評価を再帰的に行うメソッド
    private static string EvaluateRemainingExpression(string expression)
    {
        expression = ParseBinaryOperators(expression);
        return expression;
    }

    // 手動で関数呼び出しの括弧を処理するメソッド
    private static string ParseMathFunctions(string expression, int index, int cloneCount)
    {
        string[] functions = { "sin", "cos", "sqrt", "pow", "floor", "ceil", "round", "abs", "R" };

        foreach (string func in functions)
        {
            int startIndex = expression.IndexOf(func + "(");
            while (startIndex != -1)
            {
                int openParenIndex = startIndex + func.Length;
                int closeParenIndex = FindMatchingParenthesis(expression, openParenIndex);

                if (closeParenIndex == -1)
                {
                    throw new ArgumentException($"Unmatched parentheses in function '{func}'");
                }

                string innerExpression = expression.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
                float evaluatedResult;

                if (func == "R")
                {
                    string[] args = innerExpression.Split(',');
                    if (args.Length != 2)
                        throw new ArgumentException("R(min, max) requires two arguments.");

                    float minValue = EvaluateProcessedExpression(args[0], index, cloneCount);
                    float maxValue = EvaluateProcessedExpression(args[1], index, cloneCount);
                    evaluatedResult = UnityEngine.Random.Range(minValue, maxValue);
                }
                else
                {
                    evaluatedResult = EvaluateFunction(func, innerExpression, index, cloneCount);
                }

                expression = expression.Substring(0, startIndex) + RoundToThreeDecimalPlaces(evaluatedResult).ToString() + expression.Substring(closeParenIndex + 1);
                startIndex = expression.IndexOf(func + "(");
            }
        }

        return expression;
    }

    // 指定された関数と引数を評価
    private static float EvaluateFunction(string func, string innerExpression, int index, int cloneCount)
    {
        float argument = EvaluateProcessedExpression(innerExpression, index, cloneCount);
        return func switch
        {
            "sin" => Mathf.Sin(argument),
            "cos" => Mathf.Cos(argument),
            "sqrt" => Mathf.Sqrt(argument),
            "floor" => Mathf.Floor(argument),
            "ceil" => Mathf.Ceil(argument),
            "round" => Mathf.Round(argument),
            "abs" => Mathf.Abs(argument),
            _ => throw new ArgumentException($"Unknown function '{func}'")
        };
    }

    // 開始括弧のインデックスから対応する閉じ括弧のインデックスを探す
    private static int FindMatchingParenthesis(string expression, int openParenIndex)
    {
        int depth = 0;
        for (int i = openParenIndex; i < expression.Length; i++)
        {
            if (expression[i] == '(') depth++;
            if (expression[i] == ')') depth--;

            if (depth == 0)
                return i;
        }
        return -1;
    }
}
