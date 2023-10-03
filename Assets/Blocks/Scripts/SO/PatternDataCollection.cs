using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[CreateAssetMenu(
	fileName = "PatternDataCollection.asset",
	menuName = SOArchitecture_Utility.COLLECTION_SUBMENU + "Structs/PatternData",
	order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 13)]
	public class PatternDataCollection : Collection<PatternData>
	{
	} 
}