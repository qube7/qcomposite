using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qube7.Composite.Design.Generators
{
    /// <summary>
    /// Provides helpers for the syntax trees.
    /// </summary>
    internal static class SyntaxHelper
    {
        #region Fields

        /// <summary>
        /// The summary documentation start line.
        /// </summary>
        private const string SummaryStart = "/// <summary>";

        /// <summary>
        /// The summary documentation line format.
        /// </summary>
        private const string SummaryLine = "/// {0}";

        /// <summary>
        /// The summary documentation end line.
        /// </summary>
        private const string SummaryEnd = "/// </summary>";

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified attribute declarations to the specified <see cref="ClassDeclarationSyntax"/>.
        /// </summary>
        /// <param name="syntax">The <see cref="ClassDeclarationSyntax"/> to add attributes to.</param>
        /// <param name="items">The <see cref="AttributeSyntax"/> declarations to add.</param>
        /// <returns>The <see cref="ClassDeclarationSyntax"/> with the <paramref name="items"/> declarations added.</returns>
        internal static ClassDeclarationSyntax AddAttributes(this ClassDeclarationSyntax syntax, params AttributeSyntax[] items)
        {
            return syntax.WithAttributeLists(syntax.AttributeLists.AddRange(items.Select(s => SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(s)))));
        }

        /// <summary>
        /// Adds the summary documentation comment to the specified member.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="member">The member to comment.</param>
        /// <param name="summary">The summary description text.</param>
        /// <returns>The member with the <paramref name="summary"/> comment.</returns>
        internal static TMember AddSummary<TMember>(this TMember member, string summary) where TMember : MemberDeclarationSyntax
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(SummaryStart);

            using (StringReader reader = new StringReader(summary))
            {
                string line = reader.ReadLine();

                while (line != null)
                {
                    builder.AppendLine(string.Format(SummaryLine, line));

                    line = reader.ReadLine();
                }
            }

            builder.AppendLine(SummaryEnd);

            return member.WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia(builder.ToString()));
        }

        /// <summary>
        /// Parses a <see cref="TypeSyntax"/> node using specified type identifier and generic type parameters.
        /// </summary>
        /// <param name="identifier">The type identifier to use.</param>
        /// <param name="list">The generic type parameters to use.</param>
        /// <returns>The parsed <see cref="TypeSyntax"/>.</returns>
        internal static TypeSyntax TypeName(SyntaxToken identifier, TypeParameterListSyntax parameters)
        {
            return parameters == null ? SyntaxFactory.ParseTypeName(identifier.ValueText) : SyntaxFactory.ParseTypeName(string.Concat(identifier.ValueText, parameters.WithoutTrivia().NormalizeWhitespace().ToString()));
        }

        /// <summary>
        /// Formats the specified <see cref="CompilationUnitSyntax"/> and returns its file string representation.
        /// </summary>
        /// <param name="syntax">The <see cref="CompilationUnitSyntax"/> to format.</param>
        /// <param name="rewriter">The <see cref="CSharpSyntaxRewriter"/> to use.</param>
        /// <returns>The formatted file string representation of the <paramref name="syntax"/>.</returns>
        internal static string ToFileString(this CompilationUnitSyntax syntax, CSharpSyntaxRewriter rewriter = null)
        {
            SyntaxTriviaList header = SyntaxFactory.ParseLeadingTrivia(string.Format(Strings.FileHeader, Environment.Version));

            if (syntax.Usings.Count > 0 || syntax.Members.Count > 0)
            {
                header = header.Add(SyntaxFactory.CarriageReturnLineFeed).Add(SyntaxFactory.CarriageReturnLineFeed);
            }

            return syntax.NormalizeWhitespace().Accept(rewriter ?? new Rewriter()).WithLeadingTrivia(header).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed).ToFullString();
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a <see cref="CSharpSyntaxRewriter"/> that does not modify the <see cref="CompilationUnitSyntax"/> graph.
        /// </summary>
        private class Rewriter : CSharpSyntaxRewriter
        {
            #region Methods

            /// <summary>
            /// Called when the visitor visits a <see cref="CompilationUnitSyntax"/> node.
            /// </summary>
            /// <param name="node">The <see cref="CompilationUnitSyntax"/> node to visit.</param>
            /// <returns>The input <paramref name="node"/>.</returns>
            public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
            {
                return node;
            }

            #endregion
        }

        #endregion
    }
}
