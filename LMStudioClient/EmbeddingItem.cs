namespace LMStudioClient;
public partial class EmbeddingItem
{
   
    internal EmbeddingItem(ReadOnlyMemory<float> embedding, int index)
    {
        Embedding = embedding;
        Index = index;
    }
   
    public ReadOnlyMemory<float> Embedding { get; }
    public int Index { get; }
}