using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qube7.Composite.Design.Generators
{
    /// <summary>
    /// Generates a nested, private class file that contains properties that match the string resources in the XML resource file.
    /// </summary>
    public class PrivateResXFileCodeGenerator : BaseCodeGenerator
    {
        #region Fields

        /// <summary>
        /// The nested resources class name.
        /// </summary>
        private const string ClassName = "Resources";

        /// <summary>
        /// The resource manager field name.
        /// </summary>
        private const string FieldName = "resourceManager";

        /// <summary>
        /// The current culture property name.
        /// </summary>
        private const string Current = "Current";

        /// <summary>
        /// The culture fully qualified type name.
        /// </summary>
        private const string Culture = "Qube7.Composite.Culture";

        /// <summary>
        /// The underscore replacement character.
        /// </summary>
        private const char Underscore = '_';

        /// <summary>
        /// The text trimming ellipsis.
        /// </summary>
        private const string Ellipsis = "...";

        /// <summary>
        /// The documentation summary truncation threshold.
        /// </summary>
        private const int SummaryLength = 128;

        /// <summary>
        /// The identifier invalid characters.
        /// </summary>
        private static readonly char[] Invalid = new char[] { ' ', '\u00A0', '.', ',', ';', '|', '~', '!', '@', '#', '$', '%', '^', '&', '*', '+', '-', '=', '/', '\\', '<', '>', '?', '[', ']', '(', ')', '{', '}', '\"', '\'', '`', ':' };

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension that is given to the output file name.
        /// </summary>
        /// <value>The file extension.</value>
        protected override string DefaultExtension
        {
            get { return ".Resources.cs"; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateResXFileCodeGenerator"/> class.
        /// </summary>
        public PrivateResXFileCodeGenerator()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the transformation and returns the contents of the generated output file.
        /// </summary>
        /// <param name="inputText">The contents of the input file.</param>
        /// <param name="filePath">The full path of the input file.</param>
        /// <param name="defaultNamespace">The namespace into which the generated code will be placed.</param>
        /// <returns>The contents of the output file.</returns>
        protected override string GenerateCode(string inputText, string filePath, string defaultNamespace)
        {
            Dictionary<string, ResXData> properties = GenerateDictionary(ReadResX(filePath));

            return GenerateCode(properties, filePath, defaultNamespace);
        }

        /// <summary>
        /// Generates the source code graph for the specified resource property dictionary.
        /// </summary>
        /// <param name="properties">The property dictionary for which to generate the code.</param>
        /// <param name="resxFile">The full path of the input XML resource file.</param>
        /// <param name="defaultNamespace">The namespace into which the generated code will be placed.</param>
        /// <returns>The generated source code.</returns>
        private string GenerateCode(Dictionary<string, ResXData> properties, string resxFile, string defaultNamespace)
        {
            CompilationUnitSyntax unit = SyntaxFactory.CompilationUnit();

            if (properties.Count > 0)
            {
                string fileName = Path.GetFileNameWithoutExtension(resxFile);

                string className = CreateIdentifier(fileName);
                if (className == null)
                {
                    GenerateError(string.Format(Strings.InvalidClassIdentifier, Path.GetFileName(resxFile)));

                    return unit.ToFileString();
                }

                NameSyntax ns = SyntaxFactory.ParseName(defaultNamespace);

                unit = unit.AddMembers(
                    SyntaxFactory.NamespaceDeclaration(ns).AddMembers(
                        SyntaxFactory.ClassDeclaration(className).AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword)).AddMembers(
                            SyntaxFactory.ClassDeclaration(ClassName).AddModifiers(
                                SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                SyntaxFactory.Token(SyntaxKind.PartialKeyword)
                                ).AddAttributes(
                                SyntaxFactory.Attribute(SyntaxFactory.ParseName(typeof(DebuggerNonUserCodeAttribute).FullName)),
                                SyntaxFactory.Attribute(SyntaxFactory.ParseName(typeof(CompilerGeneratedAttribute).FullName))
                                ).AddMembers(
                                SyntaxFactory.FieldDeclaration(
                                    SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseName(typeof(ResourceManager).FullName)).AddVariables(
                                        SyntaxFactory.VariableDeclarator(FieldName).WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseName(typeof(ResourceManager).FullName)).AddArgumentListArguments(
                                                    SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(SyntaxFactory.QualifiedName(ns, SyntaxFactory.IdentifierName(fileName)).ToString()))),
                                                    SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(className)), SyntaxFactory.IdentifierName(nameof(Type.Assembly))))
                                                    )
                                                )
                                            )
                                        )
                                    ).AddModifiers(
                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                    )
                                ).AddMembers(GenerateProperties(properties).ToArray()).AddSummary(Strings.ResourcesClassSummary)
                            )
                        )
                    );
            }

            return unit.ToFileString(new Formatter());
        }

        /// <summary>
        /// Generates a sequence of <see cref="PropertyDeclarationSyntax"/> for the specified property dictionary.
        /// </summary>
        /// <param name="properties">The property dictionary for which to generate the code.</param>
        /// <returns>The sequence of generated <see cref="PropertyDeclarationSyntax"/>.</returns>
        private static IEnumerable<PropertyDeclarationSyntax> GenerateProperties(Dictionary<string, ResXData> properties)
        {
            ArgumentSyntax argument = SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ParseTypeName(Culture), SyntaxFactory.IdentifierName(Current)));

            foreach (KeyValuePair<string, ResXData> pair in properties.OrderBy(p => p.Key, StringComparer.InvariantCulture))
            {
                string name = pair.Key;
                ResXData data = pair.Value;

                yield return SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), name).AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                    ).AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).AddBodyStatements(
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(FieldName), SyntaxFactory.IdentifierName(nameof(ResourceManager.GetString)))
                                ).AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(data.Name))), argument)
                            )
                        )
                    ).AddSummary(PropertySummary(SecurityElement.Escape(data.Value)));
            }
        }

        /// <summary>
        /// Creates the property summary comment based on the specified resource value.
        /// </summary>
        /// <param name="value">The resource value.</param>
        /// <returns>The property summary description text.</returns>
        private static string PropertySummary(string value)
        {
            if (value.Length > SummaryLength)
            {
                value = string.Concat(value.Substring(0, SummaryLength), Ellipsis);
            }

            return string.Format(Strings.ResourcesPropertySummary, value);
        }

        /// <summary>
        /// Generates the property dictionary for the specified sequence of <see cref="ResXData"/>.
        /// </summary>
        /// <param name="resources">The resources for which to generate the dictionary.</param>
        /// <returns>The property dictionary for the <paramref name="resources"/>.</returns>
        private Dictionary<string, ResXData> GenerateDictionary(IEnumerable<ResXData> resources)
        {
            Dictionary<string, ResXData> properties = new Dictionary<string, ResXData>();

            List<Tuple<string, ResXData>> tuples = new List<Tuple<string, ResXData>>();

            foreach (ResXData data in resources)
            {
                string propertyName = CreateIdentifier(data.Name);
                if (propertyName == null)
                {
                    GenerateError(string.Format(Strings.InvalidPropertyIdentifier, data.Name));

                    continue;
                }

                tuples.Add(Tuple.Create(propertyName, data));
            }

            List<IGrouping<string, ResXData>> groupings = new List<IGrouping<string, ResXData>>();

            foreach (IGrouping<string, ResXData> grouping in tuples.GroupBy(t => t.Item1, t => t.Item2))
            {
                if (grouping.Count() > 1)
                {
                    groupings.Add(grouping);

                    continue;
                }

                properties.Add(grouping.Key, grouping.First());
            }

            foreach (IGrouping<string, ResXData> grouping in groupings)
            {
                int count = grouping.Count();

                string key = grouping.Key;

                bool repeat;

                do
                {
                    repeat = false;

                    for (int i = 1; i <= count; i++)
                    {
                        if (properties.ContainsKey(string.Concat(key, i.ToString())))
                        {
                            key = string.Concat(key, Underscore);

                            repeat = true;

                            break;
                        }
                    }
                }
                while (repeat);

                int j = 1;

                foreach (ResXData data in grouping)
                {
                    properties.Add(string.Concat(key, j.ToString()), data);

                    j++;
                }
            }

            return properties;
        }

        /// <summary>
        /// Creates a valid identifier for the specified value.
        /// </summary>
        /// <param name="value">The string for which to generate a valid identifier.</param>
        /// <returns>A valid identifier for the <paramref name="value"/>.</returns>
        private static string CreateIdentifier(string value)
        {
            for (int i = 0; i < Invalid.Length; i++)
            {
                value = value.Replace(Invalid[i], Underscore);
            }

            if (SyntaxFacts.IsValidIdentifier(value))
            {
                return value;
            }

            value = string.Concat(Underscore, value);

            if (SyntaxFacts.IsValidIdentifier(value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Reads the specified XML resource file and returns a sequence of <see cref="ResXData"/>.
        /// </summary>
        /// <param name="resxFile">The path of the file to read.</param>
        /// <returns>The sequence of <see cref="ResXData"/>.</returns>
        private static IEnumerable<ResXData> ReadResX(string resxFile)
        {
            using (ResXResourceReader reader = new ResXResourceReader(resxFile))
            {
                reader.BasePath = Path.GetDirectoryName(resxFile);

                reader.UseResXDataNodes = true;

                foreach (DictionaryEntry entry in reader)
                {
                    ResXDataNode node = (ResXDataNode)entry.Value;

                    Type type = Type.GetType(node.GetValueTypeName((AssemblyName[])null));
                    if (type == typeof(string))
                    {
                        string name = entry.Key.ToString();
                        string value = node.GetValue((AssemblyName[])null).ToString();

                        yield return new ResXData(name, value);
                    }
                }
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents an element in the resource file.
        /// </summary>
        private class ResXData
        {
            #region Properties

            /// <summary>
            /// Gets the name of the resource.
            /// </summary>
            /// <value>The name of the resource.</value>
            internal string Name { get; private set; }

            /// <summary>
            /// Gets the stored value of the resource.
            /// </summary>
            /// <value>The resource value.</value>
            internal string Value { get; private set; }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ResXData"/> class.
            /// </summary>
            /// <param name="name">The name of the resource.</param>
            /// <param name="value">The resource value.</param>
            internal ResXData(string name, string value)
            {
                Name = name;
                Value = value;
            }

            #endregion
        }

        /// <summary>
        /// Formats the output syntax graph.
        /// </summary>
        private class Formatter : CSharpSyntaxRewriter
        {
            #region Methods

            /// <summary>
            /// Called when the visitor visits a <see cref="FieldDeclarationSyntax"/> node.
            /// </summary>
            /// <param name="node">The <see cref="FieldDeclarationSyntax"/> node to visit.</param>
            /// <returns>The modified <paramref name="node"/>.</returns>
            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                return node.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);
            }

            #endregion
        }

        #endregion
    }
}
