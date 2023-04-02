using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TielsGenerators;

public class ConfigEntryAggregate : ISyntaxReceiver
{
	public List<Capture> Captures { get; } = new();

	public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
	{
		if (syntaxNode is not AttributeSyntax { Name: IdentifierNameSyntax{Identifier.Text: "ConfigEntry"} } attr) return;

		var fields = attr.GetParent<FieldDeclarationSyntax>();
		var ns = attr.GetParent<FileScopedNamespaceDeclarationSyntax>();
		string? group = null;
		
		List<ClassDeclarationSyntax> clazz = new List<ClassDeclarationSyntax>();
		GetAllClasses(attr, null, ref clazz);

		if (attr.ArgumentList != null)
			foreach (var argument in attr.ArgumentList.Arguments.Where(argument => argument.NameEquals?.Name.Identifier.Text == "Group"))
				group = ((LiteralExpressionSyntax)argument.Expression).Token.ValueText;

		foreach (var field in fields.Declaration.Variables)
		{
			var key = field.Identifier.Text;
			var @default = field.Initializer;
			Captures.Add(new(key, fields, clazz, ns, @default, group));
		}
	}

	public record Capture(string Key,
		FieldDeclarationSyntax Fields,
		List<ClassDeclarationSyntax> Classes,
		FileScopedNamespaceDeclarationSyntax Namespace,
		EqualsValueClauseSyntax? Default,
		string? Group = null
	);

	public ClassDeclarationSyntax GetAllClasses(SyntaxNode origin, ClassDeclarationSyntax? clazz, ref List<ClassDeclarationSyntax> classes) {
		ClassDeclarationSyntax parent;
		try {
			if (clazz == null) parent = origin.GetParent<ClassDeclarationSyntax>();
			else parent = clazz.GetParent<ClassDeclarationSyntax>();
			classes.Add(GetAllClasses(origin, parent, ref classes));
			return clazz ?? origin.GetParent<ClassDeclarationSyntax>();
		} catch (Exception) {
			return clazz ?? throw new Exception("No class found");
		}
	}
}

public class NoDefaultException : Exception
{
 	public NoDefaultException(string message) : base(message) { }
}