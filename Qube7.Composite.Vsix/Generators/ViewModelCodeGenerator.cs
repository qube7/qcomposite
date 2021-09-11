using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qube7.Composite.Design.Generators
{
    /// <summary>
    /// Generates a partial ViewModel class file that contains implementation of the ViewModel conventions.
    /// </summary>
    public class ViewModelCodeGenerator : BaseCodeGenerator
    {
        #region Fields

        /// <summary>
        /// The command aggregator class name.
        /// </summary>
        private const string ClassName = "CommandAggregator";

        /// <summary>
        /// The command aggregator initialize method name.
        /// </summary>
        private const string Method1Name = "Initialize";

        /// <summary>
        /// The command aggregator initialize parameter name.
        /// </summary>
        private const string Parameter1Name = "model";

        /// <summary>
        /// The command aggregator aggregate method name.
        /// </summary>
        private const string Method2Name = "Aggregate";

        /// <summary>
        /// The command aggregator aggregate parameter name.
        /// </summary>
        private const string Parameter2Name = "commands";

        /// <summary>
        /// The delegate command fully qualified type name.
        /// </summary>
        private const string DelegateCommand = "Qube7.Composite.Presentation.DelegateCommand";

        /// <summary>
        /// The execute method regular expression.
        /// </summary>
        private static readonly Regex Execute = new Regex(@"^Execute([A-Z].*)Command$", RegexOptions.Compiled);

        /// <summary>
        /// The can-execute method regular expression.
        /// </summary>
        private static readonly Regex CanExecute = new Regex(@"^CanExecute([A-Z].*)Command$", RegexOptions.Compiled);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension that is given to the output file name.
        /// </summary>
        /// <value>The file extension.</value>
        protected override string DefaultExtension
        {
            get { return ".Designer.cs"; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCodeGenerator"/> class.
        /// </summary>
        public ViewModelCodeGenerator()
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
            CompilationUnitSyntax unit1 = SyntaxFactory.CompilationUnit();

            CompilationUnitSyntax unit2 = CSharpSyntaxTree.ParseText(inputText).GetCompilationUnitRoot();

            NamespaceDeclarationSyntax[] array = GenerateNamespaces(unit2.ChildNodes().OfType<NamespaceDeclarationSyntax>()).ToArray();

            if (array.Length > 0)
            {
                unit1 = unit1.WithUsings(unit2.Usings).AddMembers(array);
            }

            return unit1.ToFileString();
        }

        /// <summary>
        /// Generates a sequence of <see cref="NamespaceDeclarationSyntax"/> for the specified sequence of input <see cref="NamespaceDeclarationSyntax"/>.
        /// </summary>
        /// <param name="items">The sequence of <see cref="NamespaceDeclarationSyntax"/> for which to generate the code.</param>
        /// <returns>The sequence of generated <see cref="NamespaceDeclarationSyntax"/>.</returns>
        private IEnumerable<NamespaceDeclarationSyntax> GenerateNamespaces(IEnumerable<NamespaceDeclarationSyntax> items)
        {
            foreach (NamespaceDeclarationSyntax syntax in items)
            {
                ClassDeclarationSyntax[] array = GenerateClasses(syntax.ChildNodes().OfType<ClassDeclarationSyntax>()).ToArray();

                if (array.Length > 0)
                {
                    yield return SyntaxFactory.NamespaceDeclaration(syntax.Name).WithUsings(syntax.Usings).AddMembers(array);
                }
            }
        }

        /// <summary>
        /// Generates a sequence of <see cref="ClassDeclarationSyntax"/> for the specified sequence of input <see cref="ClassDeclarationSyntax"/>.
        /// </summary>
        /// <param name="items">The sequence of <see cref="ClassDeclarationSyntax"/> for which to generate the code.</param>
        /// <returns>The sequence of generated <see cref="ClassDeclarationSyntax"/>.</returns>
        private IEnumerable<ClassDeclarationSyntax> GenerateClasses(IEnumerable<ClassDeclarationSyntax> items)
        {
            Type type = typeof(ICollection<ICommand>);

            QualifiedNameSyntax parameter2Type = SyntaxFactory.QualifiedName(SyntaxFactory.ParseName(type.Namespace), SyntaxFactory.GenericName(nameof(ICollection<ICommand>)).AddTypeArgumentListArguments(SyntaxFactory.ParseName(type.GenericTypeArguments[0].FullName)));

            foreach (ClassDeclarationSyntax syntax in items)
            {
                CommandElement[] array = GenerateCommands(syntax).ToArray();

                if (array.Length > 0)
                {
                    SyntaxTokenList modifiers = syntax.Modifiers.Any(SyntaxKind.PartialKeyword) ? syntax.Modifiers : syntax.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

                    yield return SyntaxFactory.ClassDeclaration(syntax.Identifier).WithTypeParameterList(syntax.TypeParameterList).WithModifiers(modifiers).AddMembers(
                        SyntaxFactory.ClassDeclaration(ClassName).AddModifiers(
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                            SyntaxFactory.Token(SyntaxKind.PartialKeyword)
                            ).AddAttributes(
                            SyntaxFactory.Attribute(SyntaxFactory.ParseName(typeof(DebuggerNonUserCodeAttribute).FullName)),
                            SyntaxFactory.Attribute(SyntaxFactory.ParseName(typeof(CompilerGeneratedAttribute).FullName))
                            ).AddMembers(GenerateProperties(array).ToArray()).AddMembers(
                            SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), Method1Name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword)).AddParameterListParameters(
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier(Parameter1Name)).WithType(SyntaxHelper.TypeName(syntax.Identifier, syntax.TypeParameterList))
                                ).AddBodyStatements(GenerateInitializeStatements(array).ToArray()),
                            SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), Method2Name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword)).AddParameterListParameters(
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier(Parameter2Name)).WithType(parameter2Type)
                                ).AddBodyStatements(GenerateAggregateStatements(array).ToArray())
                            )
                        );
                }
            }
        }

        /// <summary>
        /// Generates a sequence of <see cref="PropertyDeclarationSyntax"/> for the specified sequence of <see cref="CommandElement"/>.
        /// </summary>
        /// <param name="commands">The sequence of <see cref="CommandElement"/> for which to generate the code.</param>
        /// <returns>The sequence of generated <see cref="PropertyDeclarationSyntax"/>.</returns>
        private static IEnumerable<PropertyDeclarationSyntax> GenerateProperties(IEnumerable<CommandElement> commands)
        {
            if (SyntaxFactory.ParseTypeName(DelegateCommand) is QualifiedNameSyntax typeName)
            {
                foreach (CommandElement command in commands)
                {
                    TypeSyntax type = typeName.WithRight(SyntaxFactory.GenericName(typeName.Right.Identifier).AddTypeArgumentListArguments(command.ParameterType));

                    yield return SyntaxFactory.PropertyDeclaration(type, command.Identifier).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)).AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)).AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                        ).AddSummary(string.Format(Strings.CommandPropertySummary, command.Identifier));
                }
            }
        }

        /// <summary>
        /// Generates a sequence of initialize <see cref="StatementSyntax"/> for the specified sequence of <see cref="CommandElement"/>.
        /// </summary>
        /// <param name="commands">The sequence of <see cref="CommandElement"/> for which to generate the code.</param>
        /// <returns>The sequence of generated <see cref="StatementSyntax"/>.</returns>
        private static IEnumerable<StatementSyntax> GenerateInitializeStatements(IEnumerable<CommandElement> commands)
        {
            if (SyntaxFactory.ParseTypeName(DelegateCommand) is QualifiedNameSyntax typeName)
            {
                IdentifierNameSyntax parameterName = SyntaxFactory.IdentifierName(Parameter1Name);

                NameSyntax actionLeft = SyntaxFactory.ParseName(typeof(Action<object>).Namespace);
                GenericNameSyntax actionRight = SyntaxFactory.GenericName(nameof(Action<object>));

                NameSyntax funcLeft = SyntaxFactory.ParseName(typeof(Func<object, bool>).Namespace);
                GenericNameSyntax funcRight = SyntaxFactory.GenericName(nameof(Func<object, bool>));

                PredefinedTypeSyntax boolType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));

                InvocationExpressionSyntax name = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(SyntaxFactory.ParseToken(SyntaxFactory.Token(SyntaxKind.NameOfKeyword).ValueText)));

                foreach (CommandElement command in commands)
                {
                    List<ArgumentSyntax> arguments = new List<ArgumentSyntax>();

                    arguments.Add(
                        SyntaxFactory.Argument(
                            SyntaxFactory.ObjectCreationExpression(SyntaxFactory.QualifiedName(actionLeft, actionRight.AddTypeArgumentListArguments(command.ParameterType))).AddArgumentListArguments(
                                SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, parameterName, SyntaxFactory.IdentifierName(command.Execute)))
                                )
                            )
                        );

                    if (command.CanExecute.HasValue)
                    {
                        arguments.Add(
                            SyntaxFactory.Argument(
                                SyntaxFactory.ObjectCreationExpression(SyntaxFactory.QualifiedName(funcLeft, funcRight.AddTypeArgumentListArguments(command.ParameterType, boolType))).AddArgumentListArguments(
                                    SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, parameterName, SyntaxFactory.IdentifierName(command.CanExecute.Value)))
                                    )
                                )
                            );
                    }

                    arguments.Add(SyntaxFactory.Argument(name.AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(command.Identifier)))));

                    TypeSyntax type = typeName.WithRight(SyntaxFactory.GenericName(typeName.Right.Identifier).AddTypeArgumentListArguments(command.ParameterType));

                    yield return SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName(command.Identifier),
                            SyntaxFactory.InvocationExpression(SyntaxFactory.ObjectCreationExpression(type)).AddArgumentListArguments(arguments.ToArray())
                            )
                        );
                }
            }
        }

        /// <summary>
        /// Generates a sequence of aggregate <see cref="StatementSyntax"/> for the specified sequence of <see cref="CommandElement"/>.
        /// </summary>
        /// <param name="commands">The sequence of <see cref="CommandElement"/> for which to generate the code.</param>
        /// <returns>The sequence of generated <see cref="StatementSyntax"/>.</returns>
        private static IEnumerable<StatementSyntax> GenerateAggregateStatements(IEnumerable<CommandElement> commands)
        {
            InvocationExpressionSyntax invocation = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(Parameter2Name), SyntaxFactory.IdentifierName(nameof(ICollection<ICommand>.Add))));

            foreach (CommandElement command in commands)
            {
                yield return SyntaxFactory.ExpressionStatement(invocation.AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(command.Identifier))));
            }
        }

        /// <summary>
        /// Generates a sequence of <see cref="CommandElement"/> for the specified <see cref="ClassDeclarationSyntax"/>.
        /// </summary>
        /// <param name="syntax">The <see cref="ClassDeclarationSyntax"/> for which to generate the commands.</param>
        /// <returns>The sequence of <see cref="CommandElement"/> for the <paramref name="syntax"/>.</returns>
        private IEnumerable<CommandElement> GenerateCommands(ClassDeclarationSyntax syntax)
        {
            List<Tuple<string, MethodDeclarationSyntax>> tuples1 = new List<Tuple<string, MethodDeclarationSyntax>>();
            List<Tuple<string, MethodDeclarationSyntax>> tuples2 = new List<Tuple<string, MethodDeclarationSyntax>>();

            foreach (MethodDeclarationSyntax method in syntax.ChildNodes().OfType<MethodDeclarationSyntax>())
            {
                if (method.TypeParameterList == null && method.ExplicitInterfaceSpecifier == null)
                {
                    if (method.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    if (method.ParameterList.Parameters.Count == 1)
                    {
                        ParameterSyntax parameter = method.ParameterList.Parameters[0];

                        if (parameter.Type != null)
                        {
                            if (method.ReturnType is PredefinedTypeSyntax type)
                            {
                                if (type.Keyword.IsKind(SyntaxKind.VoidKeyword))
                                {
                                    Match match = Execute.Match(method.Identifier.ValueText);

                                    if (match.Success)
                                    {
                                        string command = match.Groups[1].Value;

                                        if (SyntaxFacts.IsValidIdentifier(command))
                                        {
                                            tuples1.Add(Tuple.Create(command, method));
                                        }
                                    }

                                    continue;
                                }

                                if (type.Keyword.IsKind(SyntaxKind.BoolKeyword))
                                {
                                    Match match = CanExecute.Match(method.Identifier.ValueText);

                                    if (match.Success)
                                    {
                                        string command = match.Groups[1].Value;

                                        if (SyntaxFacts.IsValidIdentifier(command))
                                        {
                                            tuples2.Add(Tuple.Create(command, method));
                                        }
                                    }

                                    continue;
                                }
                            }
                        }
                    }
                }
            }

            Dictionary<string, MethodDeclarationSyntax> dictionary1 = new Dictionary<string, MethodDeclarationSyntax>();
            Dictionary<string, MethodDeclarationSyntax> dictionary2 = new Dictionary<string, MethodDeclarationSyntax>();

            foreach (IGrouping<string, MethodDeclarationSyntax> grouping in tuples1.GroupBy(t => t.Item1, t => t.Item2))
            {
                if (grouping.Count() > 1)
                {
                    WarningOverload(grouping.Last(), grouping.Key);

                    continue;
                }

                dictionary1.Add(grouping.Key, grouping.First());
            }

            foreach (IGrouping<string, MethodDeclarationSyntax> grouping in tuples2.GroupBy(t => t.Item1, t => t.Item2))
            {
                if (grouping.Count() > 1)
                {
                    if (dictionary1.Remove(grouping.Key))
                    {
                        WarningOverload(grouping.Last(), grouping.Key);
                    }

                    continue;
                }

                dictionary2.Add(grouping.Key, grouping.First());
            }

            foreach (KeyValuePair<string, MethodDeclarationSyntax> pair in dictionary1.OrderBy(p => p.Key, StringComparer.InvariantCulture))
            {
                string command = pair.Key;

                MethodDeclarationSyntax syntax1 = pair.Value;

                ParameterSyntax parameter1 = syntax1.ParameterList.Parameters[0];

                if (dictionary2.TryGetValue(command, out MethodDeclarationSyntax syntax2))
                {
                    ParameterSyntax parameter2 = syntax2.ParameterList.Parameters[0];

                    if (parameter1.Identifier.ValueText != parameter2.Identifier.ValueText)
                    {
                        WarningParameter(syntax1, syntax2, parameter2.Identifier, command);

                        continue;
                    }

                    if (AreSame(parameter1.Type, parameter2.Type))
                    {
                        yield return new CommandElement(SyntaxFactory.Identifier(command), syntax1.Identifier, syntax2.Identifier, parameter1.Type);

                        continue;
                    }

                    WarningParameter(syntax1, syntax2, parameter2.Type, command);

                    continue;
                }

                yield return new CommandElement(SyntaxFactory.Identifier(command), syntax1.Identifier, parameter1.Type);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="TypeSyntax"/> instances refer to the same type.
        /// </summary>
        /// <param name="syntax1">The first <see cref="TypeSyntax"/> to compare.</param>
        /// <param name="syntax2">The second <see cref="TypeSyntax"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="syntax1"/> refers to the same type as <paramref name="syntax2"/>; otherwise, <c>false</c>.</returns>
        private static bool AreSame(TypeSyntax syntax1, TypeSyntax syntax2)
        {
            return syntax1.GetType() == syntax2.GetType() && syntax1.WithoutTrivia().NormalizeWhitespace().ToString() == syntax2.WithoutTrivia().NormalizeWhitespace().ToString();
        }

        /// <summary>
        /// Generates the warning information indicating overloaded command method.
        /// </summary>
        /// <param name="method">The declaration of the overloaded method.</param>
        /// <param name="command">The identifier of the command.</param>
        private void WarningOverload(MethodDeclarationSyntax method, string command)
        {
            SyntaxToken identifier = method.Identifier;

            FileLinePositionSpan span = identifier.GetLocation().GetLineSpan();

            if (span.IsValid)
            {
                GenerateWarning(string.Format(Strings.MethodOverload, identifier.ValueText, command), span.StartLinePosition.Line + 1, span.StartLinePosition.Character + 1);

                return;
            }

            GenerateWarning(string.Format(Strings.MethodOverload, identifier.ValueText, command));
        }

        /// <summary>
        /// Generates the warning information indicating mismatch in the parameter names of the command methods.
        /// </summary>
        /// <param name="method1">The declaration of the first method.</param>
        /// <param name="method2">The declaration of the second method.</param>
        /// <param name="identifier">The identifier of the mismatched parameter.</param>
        /// <param name="command">The identifier of the command.</param>
        private void WarningParameter(MethodDeclarationSyntax method1, MethodDeclarationSyntax method2, SyntaxToken identifier, string command)
        {
            FileLinePositionSpan span = identifier.GetLocation().GetLineSpan();

            if (span.IsValid)
            {
                GenerateWarning(string.Format(Strings.ParameterNameMismatch, method1.Identifier.ValueText, method2.Identifier.ValueText, command), span.StartLinePosition.Line + 1, span.StartLinePosition.Character + 1);

                return;
            }

            GenerateWarning(string.Format(Strings.ParameterNameMismatch, method1.Identifier.ValueText, method2.Identifier.ValueText, command));
        }

        /// <summary>
        /// Generates the warning information indicating mismatch in the parameter types of the command methods.
        /// </summary>
        /// <param name="method1">The declaration of the first method.</param>
        /// <param name="method2">The declaration of the second method.</param>
        /// <param name="type">The syntax of the mismatched type of the parameter.</param>
        /// <param name="command">The identifier of the command.</param>
        private void WarningParameter(MethodDeclarationSyntax method1, MethodDeclarationSyntax method2, TypeSyntax type, string command)
        {
            FileLinePositionSpan span = type.GetLocation().GetLineSpan();

            if (span.IsValid)
            {
                GenerateWarning(string.Format(Strings.ParameterTypeMismatch, method1.Identifier.ValueText, method2.Identifier.ValueText, command), span.StartLinePosition.Line + 1, span.StartLinePosition.Character + 1);

                return;
            }

            GenerateWarning(string.Format(Strings.ParameterTypeMismatch, method1.Identifier.ValueText, method2.Identifier.ValueText, command));
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents the declaration of the command.
        /// </summary>
        private class CommandElement
        {
            #region Properties

            /// <summary>
            /// Gets the identifier of the command property.
            /// </summary>
            /// <value>The identifier of the command property.</value>
            internal SyntaxToken Identifier { get; private set; }

            /// <summary>
            /// Gets the identifier of the execute method.
            /// </summary>
            /// <value>The identifier of the execute method.</value>
            internal SyntaxToken Execute { get; private set; }

            /// <summary>
            /// Gets the identifier of the can-execute method.
            /// </summary>
            /// <value>The identifier of the can-execute method.</value>
            internal SyntaxToken? CanExecute { get; private set; }

            /// <summary>
            /// Gets the type syntax of the parameter.
            /// </summary>
            /// <value>The type syntax of the parameter.</value>
            internal TypeSyntax ParameterType { get; private set; }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="CommandElement"/> class.
            /// </summary>
            /// <param name="identifier">The identifier of the command property.</param>
            /// <param name="execute">The identifier of the execute method.</param>
            /// <param name="parameterType">The type syntax of the parameter.</param>
            internal CommandElement(SyntaxToken identifier, SyntaxToken execute, TypeSyntax parameterType)
            {
                Identifier = identifier;
                Execute = execute;
                ParameterType = parameterType;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CommandElement"/> class.
            /// </summary>
            /// <param name="identifier">The identifier of the command property.</param>
            /// <param name="execute">The identifier of the execute method.</param>
            /// <param name="canExecute">The identifier of the can-execute method.</param>
            /// <param name="parameterType">The type syntax of the parameter.</param>
            internal CommandElement(SyntaxToken identifier, SyntaxToken execute, SyntaxToken canExecute, TypeSyntax parameterType) : this(identifier, execute, parameterType)
            {
                CanExecute = canExecute;
            }

            #endregion
        }

        #endregion
    }
}
