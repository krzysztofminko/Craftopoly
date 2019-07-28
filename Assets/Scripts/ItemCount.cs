[System.Serializable]

public class ItemCount
{
	public ItemType type;
	public int count;

	public ItemCount(ItemType type, int count)
	{
		this.type = type;
		this.count = count;
	}
}