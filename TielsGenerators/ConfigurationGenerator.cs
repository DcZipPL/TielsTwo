using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TielsGenerators;

[Generator]
public class ConfigurationGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		context.RegisterForSyntaxNotifications((() => new MainSyntaxReceiver()));
	}

	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (MainSyntaxReceiver)context.SyntaxReceiver; // Get captured syntax nodes
		foreach (var capture in receiver.ConfigEntries.Captures)
		{
			// Cleanup the key
			var key = capture.Key.Replace("__", "");
			
			// Create the output
			var output = capture.Namespace.WithMembers(new(
						RecurseClassDeclarations(null, capture.Classes, capture.Fields, capture.Default, key, capture.Group, 0)))
						.NormalizeWhitespace();

			// Add the output to the compilation
			context.AddSource($"{capture.Classes[0].Identifier.Text}_{key}.g.cs", output.GetText(Encoding.UTF8));
		}
	}

	private ClassDeclarationSyntax RecurseClassDeclarations(ClassDeclarationSyntax? clazz,
		List<ClassDeclarationSyntax> classes,
		FieldDeclarationSyntax fieldDeclaration,
		EqualsValueClauseSyntax? fieldDefault,
		string key,
		string? group,
		int i)
	{
		// If the class is null. This is bottom level class
		clazz ??= classes[i]
			.WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
		
		// If we're not at the top level, recurse through the classes
		if (classes.Count - 1 > i)
		{
			return clazz.WithMembers(
				new(RecurseClassDeclarations(classes[++i]
					.WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia().WithTrailingTrivia()), // Cleanup the trailing trivia and leading trivia
					classes, fieldDeclaration, fieldDefault, key, group, i))
			);
		}
		
		// Finish recursion
		return clazz.WithMembers(
			new(CreateReqConfigProperty(key, group, fieldDeclaration, fieldDefault)) // Create the property in top level class
		).WithLeadingTrivia().WithTrailingTrivia(); // Also, cleanup the trailing trivia and leading trivia
	}

	public PropertyDeclarationSyntax CreateReqConfigProperty(string configName, string? group, FieldDeclarationSyntax fields, EqualsValueClauseSyntax? fieldDefault)
	{
		return PropertyDeclaration(
				fields.Declaration.Type,
				Identifier(configName))
			.WithModifiers(
				TokenList(
					Token(
						TriviaList(),
						SyntaxKind.PublicKeyword,
						TriviaList())))
			.WithAccessorList(
				AccessorList(
					List<AccessorDeclarationSyntax>(
						new AccessorDeclarationSyntax[]
						{
							AccessorDeclaration(
									SyntaxKind.GetAccessorDeclaration)
								.WithBody(
									Block(
										SingletonList<StatementSyntax>(
											ReturnStatement(
												fieldDefault == null
													? MemberAccessExpression(
														SyntaxKind.SimpleMemberAccessExpression,
														group == null
															? InvocationExpression(
																IdentifierName("ReqModel"))
															: MemberAccessExpression(
																SyntaxKind.SimpleMemberAccessExpression,
																InvocationExpression(
																	IdentifierName("ReqModel")),
																IdentifierName(group)),
														IdentifierName(configName))
													: BinaryExpression(
														SyntaxKind.CoalesceExpression,
														MemberAccessExpression(
															SyntaxKind.SimpleMemberAccessExpression,
															group == null
																? InvocationExpression(
																	IdentifierName("ReqModel"))
																: MemberAccessExpression(
																	SyntaxKind.SimpleMemberAccessExpression,
																	InvocationExpression(
																		IdentifierName("ReqModel")),
																	IdentifierName(group)),
															IdentifierName(configName)),
														fieldDefault.Value))))),
							AccessorDeclaration(
									SyntaxKind.SetAccessorDeclaration)
								.WithBody(
									Block(
										LocalDeclarationStatement(
											VariableDeclaration(
													IdentifierName(
														Identifier(
															TriviaList(),
															SyntaxKind.VarKeyword,
															"var",
															"var",
															TriviaList())))
												.WithVariables(
													SingletonSeparatedList<VariableDeclaratorSyntax>(
														VariableDeclarator(
																Identifier("model"))
															.WithInitializer(
																EqualsValueClause(
																	InvocationExpression(
																		IdentifierName("ReqModel"))))))),
										ExpressionStatement(
											AssignmentExpression(
												SyntaxKind.SimpleAssignmentExpression,
												MemberAccessExpression(
													SyntaxKind.SimpleMemberAccessExpression,
													PostfixUnaryExpression(
														SyntaxKind.SuppressNullableWarningExpression,
														MemberAccessExpression(
															SyntaxKind.SimpleMemberAccessExpression,
															IdentifierName("model"),
															IdentifierName("Settings"))),
													IdentifierName(configName)),
												IdentifierName("value"))),
										ExpressionStatement(
											InvocationExpression(
													IdentifierName("SeedModel"))
												.WithArgumentList(
													ArgumentList(
														SingletonSeparatedList<ArgumentSyntax>(
															Argument(
																IdentifierName("model"))))))))
						})));
	}
}

public class MainSyntaxReceiver : ISyntaxReceiver
{
	public ConfigEntryAggregate ConfigEntries { get; } = new();

	public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
	{
		ConfigEntries.OnVisitSyntaxNode(syntaxNode);
	}
}