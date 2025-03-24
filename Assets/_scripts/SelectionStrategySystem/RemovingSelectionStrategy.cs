using FactoryTemplate.EventHandling;

namespace FactoryTemplate.SelectionSystem
{
	public class RemovingSelectionStrategy : ISelectionStrategy
	{
		private ISelectionManager _selectionManager;

		public RemovingSelectionStrategy(ISelectionManager selectionManager)
		{
			_selectionManager = selectionManager;
		}

		public void Select()
		{
			Selection();
			Bus<RemovingSelectionEvent>.Raise(new RemovingSelectionEvent(_selectionManager.CurrentSelected));
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
