namespace ModCore.Common.Discord.Rest
{
    public sealed record UploadFile(
        string FileName,
        byte[] Data,
        string? ContentType = null,
        string? FieldName = null
    );

    public sealed class MultipartRequest
    {
        public object? Payload { get; init; }
        public IReadOnlyList<UploadFile> Files { get; init; } = [];
        public IReadOnlyDictionary<string, string> Fields { get; init; } =
            new Dictionary<string, string>();
    }
}
