using System.Collections.Generic;

public interface IActionableItem
{
	List<ItemAction> GetMenuActions(bool held);
}
