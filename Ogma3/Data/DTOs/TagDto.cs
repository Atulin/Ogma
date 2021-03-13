using Ogma3.Data.Enums;

namespace Ogma3.Data.DTOs
{
    public class TagDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public ETagNamespace? Namespace { get; set; }
        public string NamespaceColor => Namespace.GetColor();
        public uint? NamespaceId => (uint?) Namespace;
    }
}