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
					capture.Class
						.WithMembers(new(
							CreateReqConfigProperty(key, capture.Fields))
						)
					)
				).NormalizeWhitespace();
			context.AddSource($"{capture.Class.Identifier.Text}_{key}.g.cs", output.GetText(Encoding.UTF8));
		}
	}

	public PropertyDeclarationSyntax CreateReqConfigProperty(string ConfigName, FieldDeclarationSyntax fields)
	{
		return PropertyDeclaration(
				fields.Declaration.Type,
				Identifier(ConfigName))
			.WithModifiers(
				TokenList(
					Token(
						TriviaList(
							Trivia(
								RegionDirectiveTrivia(
										true)
									.WithEndOfDirectiveToken(
										Token(
											TriviaList(
												PreprocessingMessage("Loaders")),
											SyntaxKind.EndOfDirectiveToken,
											TriviaList())))),
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
												MemberAccessExpression(
													SyntaxKind.SimpleMemberAccessExpression,
													PostfixUnaryExpression(
														SyntaxKind.SuppressNullableWarningExpression,
														MemberAccessExpression(
															SyntaxKind.SimpleMemberAccessExpression,
															InvocationExpression(
																IdentifierName("ReqModel")),
															IdentifierName("Settings"))),
													IdentifierName(ConfigName)))))),
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
													IdentifierName(ConfigName)),
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

public static class SyntaxNodeExtensions
{
	public static T GetParent<T>(this SyntaxNode node) where T : SyntaxNode
	{
		var parent = node.Parent;
		while (true)
		{
			if (parent == null) throw new Exception("No parent found");
			
			if (parent is T t) return t;
			parent = parent.Parent;
		}
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

public class ConfigEntryAggregate : ISyntaxReceiver
{
	public List<Capture> Captures { get; } = new();

	public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
	{
		if (syntaxNode is not AttributeSyntax { Name: IdentifierNameSyntax{Identifier.Text: "ConfigEntry"} } attr) return;

		var fields = attr.GetParent<FieldDeclarationSyntax>();
		var clazz = attr.GetParent<ClassDeclarationSyntax>();
		var ns = attr.GetParent<FileScopedNamespaceDeclarationSyntax>();

		foreach (var field in fields.Declaration.Variables)
		{
			var key = field.Identifier.Text;
			Captures.Add(new(key, fields, clazz, ns));
		}
	}

	public record Capture(string Key, FieldDeclarationSyntax Fields, ClassDeclarationSyntax Class, FileScopedNamespaceDeclarationSyntax Namespace);
}