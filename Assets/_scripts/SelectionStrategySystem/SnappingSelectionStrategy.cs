using FactoryTemplate.EventHandling;
using UnityEngine;

namespace FactoryTemplate.SelectionSystem
{
	public class SnappingSelectionStrategy : ISelectionStrategy
	{
		private ISelectionManager _selectionManager;
		private Transform _currentMarkerAccessorTransform;

		public SnappingSelectionStrategy(ISelectionManager selectionManager)
		{
			_selectionManager = selectionManager;
		}

		public void Select()
		{
			Bus<SnappingSelectionEvent>.Raise(new SnappingSelectionEvent(null));
		}

		public void SelectAccessor()
		{
			Transform selected = Selection();
			if (selected == null) return;

			if (!selected.TryGetComponent(out MarkerAccessor accessor)) return;

			Bus<MarkerAccessorSelectedEvent>.Raise(new MarkerAccessorSelectedEvent(accessor));
		}

		private Transform Selection()
		{
			return _selectionManager.TransformSelector.SelectTransform();
		}
	}
}
