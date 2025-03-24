using FactoryTemplate.EventHandling;
using UnityEngine;

namespace FactoryTemplate.SelectionSystem
{
	public class RotationSelectionStrategy : ISelectionStrategy
	{
		private readonly ISelectionManager _selectionManager;

		public RotationSelectionStrategy(ISelectionManager selectionManager)
		{
			_selectionManager = selectionManager;
		}

		public void Select()
		{
			Selection();
			Bus<RotationSelectionEvent>.Raise(new RotationSelectionEvent(_selectionManager.CurrentSelected));
		}

		private void Selection()
		{
			if (_selectionManager.IsAdditionModeActive)
			{
				_selectionManager.TransformSelector.AdditionalSelection();
				return;
			}

			_selectionManager.TransformSelector.Select();
		}
	}
}
