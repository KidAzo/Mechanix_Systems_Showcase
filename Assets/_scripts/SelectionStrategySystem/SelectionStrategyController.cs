using UnityEngine;

namespace FactoryTemplate.SelectionSystem
{
	public class SelectionStrategyController : ISelectionStrategyController
	{
		private ISelectionStrategy _currentStrategy;
		private ISelectionManager _selectionManager;
		private SnappingSelectionStrategy _snappingSelectionStrategy;

		public SelectionStrategyController(ISelectionManager selectionManager)
		{
			_selectionManager = selectionManager;
			_currentStrategy = new MovementSelectionStrategy(_selectionManager);
			_snappingSelectionStrategy = new SnappingSelectionStrategy(_selectionManager);
		}

		public void ChangeStrategy(NavigationTool tool)
		{
			_currentStrategy = tool switch
			{
				NavigationTool.Empty => new EmptySelectionStrategy(),
				NavigationTool.Movement => new MovementSelectionStrategy(_selectionManager),
				NavigationTool.Rotation => new RotationSelectionStrategy(_selectionManager),
				NavigationTool.Snap => new SnappingSelectionStrategy(_selectionManager),
				NavigationTool.Remove => new RemovingSelectionStrategy(_selectionManager),
			};
		}

		public void Select()
		{
			_currentStrategy.Select();
		}

		public void SelectAccessor()
		{
			_snappingSelectionStrategy.SelectAccessor();
		}
	}

	public interface ISelectionStrategyController
	{
		void ChangeStrategy(NavigationTool tool);
		void Select();
		void SelectAccessor();
	}
}
