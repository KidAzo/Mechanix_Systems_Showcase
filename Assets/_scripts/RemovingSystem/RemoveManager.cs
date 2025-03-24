using System.Collections.Generic;
using FactoryTemplate.Concealer;
using FactoryTemplate.EventHandling;
using FactoryTemplate.Removing;
using FactoryTemplate.SelectionSystem;
using FactoryTemplate.UndoSystem;
using UnityEngine;
using Zenject;

namespace FactoryTemplate.Destroyer
{
	public class RemoveManager : MonoBehaviour, IRemoveManager
	{
		[SerializeField] private MachineRemovedEventChannel onMachineRemovedEC;
		[Inject] private IInputManager _inputManager;
		[Inject] private ISelectionManager _selectionManager;
		[Inject] private IAdditionalSelectionService _additionalSelectionService;
		[Inject] private ICommandManager _commandManager;
		private IRemoveController _removeController;

		public void RemoveFromUI(Transform t)
		{
			_removeController.Remove(t, true);
			CommandManager.CreateCommand(new MachineRemovedCommand(_selectionManager, new List<Transform> { t }, _additionalSelectionService));
		}

		public void RemoveFromUI(ConcealerObject concealer)
		{
			_removeController.Remove(concealer.transform, true);
			CommandManager.CreateCommand(new ConcealerRemovedCommand(concealer));
		}

		private void Awake()
		{
			_removeController = new RemoveController(onMachineRemovedEC);
		}

		private void OnEnable()
		{
			_inputManager.OnDelete += Remove;
		}

		private void OnDisable()
		{
			_inputManager.OnDelete -= Remove;
		}

		public void Remove()
		{
			if (_selectionManager.CurrentSelected == null) return;

			var selectedObjects = new List<Transform> { _selectionManager.CurrentSelected };
			selectedObjects.AddRange(_additionalSelectionService.AdditionalObjects);

			bool isConcealer = _selectionManager.CurrentSelected.TryGetComponent(out ConcealerObject concealer);

			_selectionManager.Release();

			_removeController.RemoveAll(selectedObjects);

			ICommand commandToExecute = isConcealer
					? new ConcealerRemovedCommand(concealer)
					: new MachineRemovedCommand(_selectionManager, selectedObjects, _additionalSelectionService);

			CommandManager.CreateCommand(commandToExecute);
		}

		public void Remove(Transform t)
		{
			_selectionManager.Release();
			_removeController.RemoveAll(new List<Transform> { t });
		}
	}

	public interface IRemoveManager
	{
		void RemoveFromUI(Transform t);
		void RemoveFromUI(ConcealerObject concealer);
		void Remove(Transform t);
	}
}
