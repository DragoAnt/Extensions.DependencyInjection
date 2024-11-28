using System.CodeDom.Compiler;

#pragma warning disable RS1035

namespace DragoAnt.Extensions.DependencyInjection;

/// <summary>
/// Base class for this transformation
/// </summary>
[GeneratedCode("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
public abstract class TransformationBase
{
    private StringBuilder? generationEnvironmentField;
    private List<int>? indentLengthsField;
    private string currentIndentField = "";
    private bool endsWithNewline;
    private IDictionary<string, object> sessionField = default!;

    /// <summary>
    /// The string builder that generation-time code is using to assemble generated output
    /// </summary>
    protected StringBuilder GenerationEnvironment
    {
        get
        {
            generationEnvironmentField ??= new StringBuilder();

            return generationEnvironmentField;
        }
        set
        {
            generationEnvironmentField = value;
        }
    }

    /// <summary>
    /// A list of the lengths of each indent that was added with PushIndent
    /// </summary>
    private List<int> indentLengths
    {
        get
        {
            if ((indentLengthsField == null))
            {
                indentLengthsField = new List<int>();
            }

            return indentLengthsField;
        }
    }

    /// <summary>
    /// Gets the current indent we use when adding lines to the output
    /// </summary>
    public string CurrentIndent
    {
        get
        {
            return currentIndentField;
        }
    }

    /// <summary>
    /// Current transformation session
    /// </summary>
    public virtual IDictionary<string, object> Session
    {
        get
        {
            return sessionField;
        }
        set
        {
            sessionField = value;
        }
    }

    /// <summary>
    /// Create the template output
    /// </summary>
    public abstract string TransformText();

    /// <summary>
    /// Write text directly into the generated output
    /// </summary>
    public void Write(string? textToAppend)
    {
        if (textToAppend is null && string.IsNullOrEmpty(textToAppend))
        {
            return;
        }

        // If we're starting off, or if the previous text ended with a newline,
        // we have to append the current indent first.
        if (((GenerationEnvironment.Length == 0)
             || endsWithNewline))
        {
            GenerationEnvironment.Append(currentIndentField);
            endsWithNewline = false;
        }

        // Check if the current text ends with a newline

        if (textToAppend!.EndsWith(Environment.NewLine, StringComparison.CurrentCulture))
        {
            endsWithNewline = true;
        }

        // This is an optimization. If the current indent is "", then we don't have to do any
        // of the more complex stuff further down.
        if ((currentIndentField.Length == 0))
        {
            GenerationEnvironment.Append(textToAppend);
            return;
        }

        // Everywhere there is a newline in the text, add an indent after it
        textToAppend = textToAppend.Replace(Environment.NewLine, (Environment.NewLine + currentIndentField));
        // If the text ends with a newline, then we should strip off the indent added at the very end
        // because the appropriate indent will be added when the next time Write() is called
        if (endsWithNewline)
        {
            GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - currentIndentField.Length));
        }
        else
        {
            GenerationEnvironment.Append(textToAppend);
        }
    }

    /// <summary>
    /// Write text directly into the generated output
    /// </summary>
    public void WriteLine(string? textToAppend)
    {
        Write(textToAppend);
        GenerationEnvironment.AppendLine();
        endsWithNewline = true;
    }

    /// <summary>
    /// Write formatted text directly into the generated output
    /// </summary>
    public void Write(string? format, params object[] args)
    {
        Write(string.Format(System.Globalization.CultureInfo.CurrentCulture, format ?? string.Empty, args));
    }

    /// <summary>
    /// Write formatted text directly into the generated output
    /// </summary>
    public void WriteLine(string? format, params object[] args)
    {
        WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, format ?? string.Empty, args));
    }

    /// <summary>
    /// Increase the indent
    /// </summary>
    public void PushIndent(string indent)
    {
        if ((indent == null))
        {
            throw new ArgumentNullException(nameof(indent));
        }

        currentIndentField = (currentIndentField + indent);
        indentLengths.Add(indent.Length);
    }

    /// <summary>
    /// Remove the last indent that was added with PushIndent
    /// </summary>
    public string PopIndent()
    {
        var returnValue = "";
        if ((indentLengths.Count > 0))
        {
            var indentLength = indentLengths[(indentLengths.Count - 1)];
            indentLengths.RemoveAt((indentLengths.Count - 1));
            if ((indentLength > 0))
            {
                returnValue = currentIndentField.Substring((currentIndentField.Length - indentLength));
                currentIndentField = currentIndentField.Remove((currentIndentField.Length - indentLength));
            }
        }

        return returnValue;
    }

    /// <summary>
    /// Remove any indentation
    /// </summary>
    public void ClearIndent()
    {
        indentLengths.Clear();
        currentIndentField = "";
    }

    /// <summary>
    /// Utility class to produce culture-oriented representation of an object as a string.
    /// </summary>
    public class ToStringInstanceHelper
    {
        private IFormatProvider formatProviderField = System.Globalization.CultureInfo.InvariantCulture;

        /// <summary>
        /// Gets or sets format provider to be used by ToStringWithCulture method.
        /// </summary>
        public IFormatProvider? FormatProvider
        {
            get
            {
                return formatProviderField;
            }
            set
            {
                if ((value != null))
                {
                    formatProviderField = value;
                }
            }
        }

        /// <summary>
        /// This is called from the compile/run appdomain to convert objects within an expression block to a string
        /// </summary>
        public string? ToStringWithCulture(object objectToConvert)
        {
            if ((objectToConvert == null))
            {
                throw new ArgumentNullException(nameof(objectToConvert));
            }

            var t = objectToConvert.GetType();
            var method = t.GetMethod("ToString", [typeof(IFormatProvider)]);
            if ((method == null))
            {
                return objectToConvert.ToString();
            }

            return ((string?)(method.Invoke(objectToConvert, [formatProviderField])));
        }
    }

    private ToStringInstanceHelper toStringHelperField = new();

    /// <summary>
    /// Helper to produce culture-oriented representation of an object as a string
    /// </summary>
    public ToStringInstanceHelper ToStringHelper
    {
        get
        {
            return toStringHelperField;
        }
    }
}
