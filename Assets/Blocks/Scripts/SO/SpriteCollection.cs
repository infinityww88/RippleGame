using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[CreateAssetMenu(
	fileName = "SpriteCollection.asset",
	menuName = SOArchitecture_Utility.COLLECTION_SUBMENU + "Structs/SpriteCollection",
	order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS)]
	public class SpriteCollection : Collection<Sprite>
	{
	} 
}