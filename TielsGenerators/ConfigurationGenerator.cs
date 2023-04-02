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
		var receiver = (MainSyntaxReceiver)context.SyntaxReceiver;
		foreach (var capture in receiver.ConfigEntries.Captures)
		{
			var key = capture.Key.Replace("__", "");
			
			var output = capture.Namespace.WithMembers(new(
						RecurseClassDeclarations(null, capture.Classes, capture.Fields, capture.Default, key, 0)))
						.NormalizeWhitespace();

			context.AddSource($"{capture.Classes[0].Identifier.Text}_{key}.g.cs", output.GetText(Encoding.UTF8));
		}
	}

	private ClassDeclarationSyntax RecurseClassDeclarations(ClassDeclarationSyntax? clazz, List<ClassDeclarationSyntax> classes, FieldDeclarationSyntax? fieldDeclaration, EqualsValueClauseSyntax fieldDefault, string key, int i)
	{
		if (clazz == null) clazz = classes[i]
									.WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
		if (classes.Count - 1 > i)
		{
			return clazz.WithMembers(
				new(RecurseClassDeclarations(classes[++i]
					.WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia().WithTrailingTrivia()),
					classes, fieldDeclaration, fieldDefault, key, i))
			);
		}
		else
			return clazz.WithMembers(
				new(CreateReqConfigProperty(key, fieldDeclaration, fieldDefault))
			).WithLeadingTrivia().WithTrailingTrivia();
	}

	public PropertyDeclarationSyntax CreateReqConfigProperty(string configName, FieldDeclarationSyntax fields, EqualsValueClauseSyntax? fieldDefault)
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
														MemberAccessExpression(
															SyntaxKind.SimpleMemberAccessExpression,
															InvocationExpression(
																IdentifierName("ReqModel")),
															IdentifierName("Settings")),
														IdentifierName(configName))
													: BinaryExpression(
														SyntaxKind.CoalesceExpression,
														MemberAccessExpression(
															SyntaxKind.SimpleMemberAccessExpression,
															MemberAccessExpression(
																SyntaxKind.SimpleMemberAccessExpression,
																InvocationExpression(
																	IdentifierName("ReqModel")),
																IdentifierName("Settings")),
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