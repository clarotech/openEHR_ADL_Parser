using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Support;

internal static class AomHelpers
{
    public static CObject? FindInTree(CObject root, Func<CObject, bool> predicate)
    {
        if (predicate(root)) return root;
        if (root is CComplexObject complex)
            foreach (var attr in complex.Attributes)
            foreach (var child in attr.Children)
            {
                var found = FindInTree(child, predicate);
                if (found != null) return found;
            }
        return null;
    }

    public static CComplexObject? FindElement(CComplexObject root, string nodeId) =>
        FindInTree(root, o => o is CComplexObject c && c.NodeId == nodeId) as CComplexObject;

    public static ArchetypeSlot? FindSlot(CComplexObject root, string nodeId) =>
        FindInTree(root, o => o is ArchetypeSlot s && s.NodeId == nodeId) as ArchetypeSlot;

    public static ArchetypeInternalRef? FindInternalRef(CComplexObject root, string nodeId) =>
        FindInTree(root, o => o is ArchetypeInternalRef r && r.NodeId == nodeId) as ArchetypeInternalRef;
}
