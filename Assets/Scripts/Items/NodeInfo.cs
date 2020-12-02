/// <summary>
/// Container for node-index and item.
/// Sent out to all clients at game start to sync loot.
/// </summary>
public class NodeInfo
{
    public string itemID;
    public int nodeIndex;

    public NodeInfo(string i, int n)
    {
        itemID = i;
        nodeIndex = n;
    }
}