using UnityEngine;

namespace FactoryTemplate.UndoSystem
{
	public interface ICommand
	{
		void Undo();
	}
}