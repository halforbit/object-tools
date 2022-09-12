using System;
using System.Linq.Expressions;
using System.Text;

namespace Halforbit.ObjectTools.ObjectStringMap.Implementation;

public class StringExpressionConverter
{
    public static string Convert<TKey>(Expression<Func<TKey, string>> expression)
    {
        var body = expression.Body;

        if (body is ConstantExpression c)
        {
            return c.Value.ToString();
        }

        if (body is MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(string) && m.Method.Name == nameof(string.Format))
            {
                var formatString = (m.Arguments[0] as ConstantExpression).Value.ToString();

                var args = new Expression[m.Arguments.Count - 1];

                for (var i = 0; i < args.Length; i++)
                {
                    var src = m.Arguments[i + 1] as UnaryExpression;

                    if (src == null) throw ShapeError();

                    args[i] = src.Operand;
                }

                return ConvertFormatString(formatString, args);
            }
        }

        throw ShapeError();
    }

    static string ConvertFormatString(
        string format, 
        Expression[] args)
    {
        // This method is adapted from the `AppendFormatHelper` method of the `StringBuilder` class in
        // the .NET source code:
        // https://referencesource.microsoft.com/#mscorlib/system/text/stringbuilder.cs,2c3b4c2e7c43f5a4

        if (format == null)
        {
            throw new ArgumentNullException("format");
        }

        var result = new StringBuilder();
        
        int pos = 0;
        int len = format.Length;
        char ch = '\x0';
        
        while (true)
        {
            int p = pos;
            int i = pos;
            while (pos < len)
            {
                ch = format[pos];

                pos++;
                if (ch == '}')
                {
                    if (pos < len && format[pos] == '}') // Treat as escape character for }}
                        pos++;
                    else
                        throw FormatError();
                }

                if (ch == '{')
                {
                    if (pos < len && format[pos] == '{') // Treat as escape character for {{
                        pos++;
                    else
                    {
                        pos--;
                        break;
                    }
                }

                result.Append(ch);
            }

            if (pos == len) break;
            pos++;
            if (pos == len || (ch = format[pos]) < '0' || ch > '9') FormatError();
            int index = 0;
            do
            {
                index = index * 10 + ch - '0';
                pos++;
                if (pos == len) throw FormatError();
                ch = format[pos];
            } while (ch >= '0' && ch <= '9' && index < 1000000);
            if (index >= args.Length) throw new FormatException("Format string is invalid because index is out of range.");
            while (pos < len && (ch = format[pos]) == ' ') pos++;
            int width = 0;
            if (ch == ',')
            {
                pos++;
                while (pos < len && format[pos] == ' ') pos++;

                if (pos == len) throw FormatError();
                ch = format[pos];
                if (ch == '-')
                {
                    pos++;
                    if (pos == len) throw FormatError();
                    ch = format[pos];
                }
                if (ch < '0' || ch > '9') throw FormatError();
                do
                {
                    width = width * 10 + ch - '0';
                    pos++;
                    if (pos == len) throw FormatError();
                    ch = format[pos];
                } while (ch >= '0' && ch <= '9' && width < 1000000);
            }

            while (pos < len && (ch = format[pos]) == ' ') pos++;
            var arg = args[index];
            StringBuilder fmt = null;
            if (ch == ':')
            {
                pos++;
                p = pos;
                i = pos;
                while (true)
                {
                    if (pos == len) throw FormatError();
                    ch = format[pos];
                    pos++;
                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{')  // Treat as escape character for {{
                            pos++;
                        else
                            throw FormatError();
                    }
                    else if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}')  // Treat as escape character for }}
                            pos++;
                        else
                        {
                            pos--;
                            break;
                        }
                    }

                    if (fmt == null)
                    {
                        fmt = new StringBuilder();
                    }
                    fmt.Append(ch);
                }
            }
            if (ch != '}') throw FormatError();
            pos++;

            string s = null;

            if (arg is MemberExpression mex)
            {
                if (fmt != null)
                {
                    s = $"{{{mex.Member.Name}:{fmt}}}";
                }
                else
                {
                    s = $"{{{mex.Member.Name}}}";
                }
            }
            else if (arg is ParameterExpression pex)
            {
                if (fmt != null)
                {
                    s = $"{{this:{fmt}}}";
                }
                else
                {
                    s = "{this}";
                }
            }
            else
            {
                throw ShapeError();
            }

            result.Append(s);
        }

        return result.ToString();
    }

    static Exception FormatError() => new ArgumentException("The format string is formatted incorrectly.");

    static Exception ShapeError() => new ArgumentException(
        "The map expression is not in a recognized shape. " +
        "Map expressions may be a simple string constant, or a string interpolation having nested expressions " +
        "that only reference either a property of the key parameter, or the key parameter itself. Nested " +
        "expressions may have a format string tail separated with a `:`. " +
        "For example: `key => $\"forecasts/{key.PostalCode}/{key.Date:yyyy/MM/dd}\"`");
}

