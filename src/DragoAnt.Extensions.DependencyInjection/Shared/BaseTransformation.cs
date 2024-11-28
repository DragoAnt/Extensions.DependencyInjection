#pragma warning disable S3220,S2325

namespace DragoAnt.Extensions.DependencyInjection
{
    /// <summary>
    ///     Base class for this transformation. Copied from generated class
    /// </summary>
    public abstract class BaseTransformation : TransformationBase
    {
    }

    public abstract class BaseTransformation<T> : BaseTransformation
    {
        /// <summary>
        ///     Object to transform
        /// </summary>
        public T Data { get; set; } = default!;

        protected string ConvertToDocumentationComment(string? value, string ident)
        {
            var comment = value ?? string.Empty;
            var lines = comment.Split('\r', '\n');
            var builder = new StringBuilder();
            foreach (var line in lines)
            {
                builder.Append(ident);
                builder.Append("///");
                builder.Append(' ');
                builder.AppendLine(line.TrimEnd());
            }

            if (builder.Length == 0)
            {
                builder.Append(ident);
                builder.Append("///");
            }

            return builder.ToString().TrimEnd('\r', '\n');
        }
    }
}
