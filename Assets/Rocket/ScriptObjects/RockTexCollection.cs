using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[CreateAssetMenu(
	fileName = "RockTexCollection.asset",
	menuName = SOArchitecture_Utility.COLLECTION_SUBMENU + "Structs/RockTexCollection",
	order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS)]
	public class RockTexCollection : Collection<RockTex>
	{
	} 
}