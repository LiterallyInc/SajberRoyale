/// <summary>
/// Container for node-index and item.
/// Sent out to all clients at game start to sync loot.
/// </summary>
public class NodeInfo
{
    public Item item;
    public int nodeIndex;

    public NodeInfo(Item i, int n)
    {
        item = i;
        nodeIndex = n;
    }
}