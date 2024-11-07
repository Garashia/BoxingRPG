using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TransformCalculator
{
    // ���C���G���g���[�|�C���g
    public static float EvaluateSingleExpression(string expression, int index, int cloneCount)
    {
        expression = PreprocessExpression(expression, index, cloneCount);
        return EvaluateProcessedExpression(expression, index, cloneCount);
    }

    // �O�����F�萔�ƕϐ��̒u�������A�󔒂̍폜
    private static string PreprocessExpression(string expression, int index, int cloneCount)
    {
        expression = expression.Replace("PI", RoundToThreeDecimalPlaces(Mathf.PI).ToString());
        expression = expression.Replace("C", RoundToThreeDecimalPlaces(cloneCount).ToString());
        expression = expression.Replace("#", RoundToThreeDecimalPlaces(index).ToString());
        return expression.Replace(" ", string.Empty);
    }

    // �����_�ȉ�3���Ɋۂ߂�֐�
    private static float RoundToThreeDecimalPlaces(float value)
    {
        return Mathf.Round(value * 1000f) / 1000f;
    }

    // �O������̐�����]��
    private static float EvaluateProcessedExpression(string expression, int index, int cloneCount)
    {
        try
        {
            expression = ParseConditionalOperators(expression, index, cloneCount);  // �O�����Z�q���ɉ��
            expression = ParseMathFunctions(expression, index, cloneCount);
            expression = EvaluateParentheses(expression, index, cloneCount);
            expression = ParseUnaryMinus(expression);
            expression = ParsePowerOperators(expression);           // �ׂ��Z�̉��
            expression = ParseShiftOperators(expression);           // �V�t�g���Z�̉��
            expression = ParseBitwiseOperators(expression);         // �r�b�g���Z�̉��
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

    // �r�b�g���Z�q�i&�A|�A^�j�̉��
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

    // �O�����Z�q�i�������j���ċA�I�ɉ��
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

    // ���ʂ�D��I�ɕ]��
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

    // �P�����Z�q�Ƃ��Ẵ}�C�i�X�����
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

    // �V�t�g���Z�̉�� (�D�揇�ʂ̒Ⴂ���Z)
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

    // �ׂ��Z�̉�́i^ ���Z�q�j
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

    // ����Z�������ɕϊ����Ċ|���Z�ɕύX
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

    // �񍀉��Z�q�i+�A-�A*�j�����
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

    // �����̕]�����ċA�I�ɍs�����\�b�h
    private static string EvaluateRemainingExpression(string expression)
    {
        expression = ParseBinaryOperators(expression);
        return expression;
    }

    // �蓮�Ŋ֐��Ăяo���̊��ʂ��������郁�\�b�h
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

    // �w�肳�ꂽ�֐��ƈ�����]��
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

    // �J�n���ʂ̃C���f�b�N�X����Ή���������ʂ̃C���f�b�N�X��T��
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
