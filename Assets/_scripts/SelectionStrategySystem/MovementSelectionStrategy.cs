using FactoryTemplate.EventHandling;

namespace FactoryTemplate.SelectionSystem
{
	public class MovementSelectionStrategy : ISelectionStrategy
	{
		private ISelectionManager _selectionManager;
		bool _additionalSelection;

		public MovementSelectionStrategy(ISelectionManager selectionManager)
		{
			_selectionManager = selectionManager;
		}

		public void Select()
		{
			Selection();
			Bus<MovementSelectionEvent>.Raise(new MovementSelectionEvent(_selectionManager.CurrentSelected, _additionalSelection));
		}

		private void Selection()
		{
			if (_selectionManager.IsAdditionModeActive)
			{
				_selectionManager.TransformSelector.AdditionalSelection();
				_additionalSelection = true;
				return;
			}

			_additionalSelection = false;
			_selectionManager.TransformSelector.Select();
		}
	}
}
