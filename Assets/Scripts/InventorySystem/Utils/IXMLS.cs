using System;

namespace InventorySystem.Utils
{
	public interface IXMLS
	{
		void Initialization();

		string Serialize();

		void Deserialize(string data);
	}
}
