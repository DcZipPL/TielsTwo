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

		List<ClassDeclarationSyntax> clazz = new List<ClassDeclarationSyntax>();
		GetAllClasses(attr, null, ref clazz);

		foreach (var field in fields.Declaration.Variables)
		{
			var key = field.Identifier.Text;
			Captures.Add(new(key, fields, clazz, ns));
		}
	}

	public record Capture(string Key, FieldDeclarationSyntax Fields, List<ClassDeclarationSyntax> Classes, FileScopedNamespaceDeclarationSyntax Namespace);

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