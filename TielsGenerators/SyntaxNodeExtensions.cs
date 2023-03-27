using Microsoft.CodeAnalysis;

namespace TielsGenerators;

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