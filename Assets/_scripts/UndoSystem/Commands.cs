using FactoryTemplate.Concealer;
using FactoryTemplate.ConveyorCreator;
using FactoryTemplate.Destroyer;
using FactoryTemplate.Hide;
using FactoryTemplate.Machines;
using FactoryTemplate.SelectionSystem;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryTemplate.UndoSystem
{
	public struct SelectionCommand : ICommand
	{
		private readonly ISelectionManager _selectionManager;
		public Transform selected;
		public SelectionCommand(Transform t, ISelectionManager selectionManager)
		{
			selected = t;
			_selectionManager = selectionManager;
		}

		public void Undo()
		{
			_selectionManager.Select(selected);
		}
	}

	public struct MoveCommand : ICommand
	{
		public Transform transform;
		public Vector3 startPos;

		public MoveCommand(Transform t, Vector3 startPosition)
		{
			transform = t;
			this.startPos = startPosition;
		}

		public void Undo()
		{
			transform.position = startPos;
		}
	}

	public struct RotationCommand : ICommand
	{
		public Transform transform;
		public Quaternion startRot;

		public RotationCommand(Transform t, Quaternion startRot)
		{
			transform = t;
			this.startRot = startRot;
		}

		public void Undo()
		{
			transform.rotation = startRot;
		}
	}

	public struct RemoveAdditionalObjectCommand : ICommand
	{
		private readonly IAdditionalSelectionService _additionalSelectionService;
		private readonly ISelectionManager _selectionManager;
		public List<Transform> transforms;
		public Transform UndoObjectTransform => transforms[0];

		public RemoveAdditionalObjectCommand(List<Transform> transforms, IAdditionalSelectionService additionalSelectionservice, ISelectionManager selectionManager)
		{
			this.transforms = transforms;
			_additionalSelectionService = additionalSelectionservice;
			_selectionManager = selectionManager;
		}

		public void Undo()
		{
			foreach (Transform t in transforms)
			{
				if (t == _selectionManager.CurrentSelected) continue;
				if (t == null) continue;

				_additionalSelectionService.Remove(t);
			}
		}
	}

	public struct AddAdditionalObjectCommand : ICommand
	{
		private readonly IAdditionalSelectionService _additionalSelectionService;
		private readonly ISelectionManager _selectionManager;
		public Transform transform;
		public Transform parent;

		public AddAdditionalObjectCommand(Transform transform, Transform parent, IAdditionalSelectionService additionalSelectionservice, ISelectionManager selectionManager)
		{
			this.transform = transform;
			this.parent = parent;
			_additionalSelectionService = additionalSelectionservice;
			_selectionManager = selectionManager;
		}

		public void Undo()
		{
			_additionalSelectionService.AddingProcess(transform, parent);
		}
	}

	public struct MachineCreatedCommand : ICommand
	{
		private readonly IRemoveManager _removeManager;
		public List<Transform> machines;

		public MachineCreatedCommand(List<Transform> machines, IRemoveManager removeManager)
		{
			_removeManager = removeManager;
			this.machines = machines;
		}

		public void Undo()
		{
			foreach (Transform machine in machines)
			{
				if (machine == null) continue;

				_removeManager.Remove(machine);
			}
		}
	}

	public struct MachineDuplicatedCommand : ICommand
	{
		private readonly IRemoveManager _removeManager;
		private readonly ISelectionManager _selectionManager;
		private readonly IAdditionalSelectionService _additionalSelection;
		private Transform _lastSelected;
		public HashSet<Transform> _machines;
		public HashSet<Transform> _additionals;

		public MachineDuplicatedCommand(Transform lastSelected, HashSet<Transform> additionals, HashSet<Transform> machines, IRemoveManager removeManager, ISelectionManager selectionManager, IAdditionalSelectionService additionalSelectionService)
		{
			_removeManager = removeManager;
			_machines = machines;
			_selectionManager = selectionManager;
			_lastSelected = lastSelected;
			_additionalSelection = additionalSelectionService;
			_additionals = additionals;
		}

		public void Undo()
		{
			foreach (Transform machine in _machines)
			{
				_removeManager.Remove(machine);
			}

			_selectionManager.Select(_lastSelected);

			foreach (Transform additional in _additionals)
			{
				_additionalSelection.AddingProcess(additional, _lastSelected);
			}
		}
	}

	public struct ConveyorChangedCommand : ICommand
	{
		private readonly IRemoveManager _removeManager;
		private readonly ISelectionManager _selectionManager;
		public Conveyor oldConveyor;
		public Conveyor newConveyor;

		public ConveyorChangedCommand(Conveyor oldConveyor, Conveyor newConveyor, IRemoveManager removeManager, ISelectionManager selectionManager)
		{
			this.oldConveyor = oldConveyor;
			this.newConveyor = newConveyor;
			_removeManager = removeManager;
			_selectionManager = selectionManager;
		}

		public void Undo()
		{
			_removeManager.Remove(newConveyor.transform);

			oldConveyor.gameObject.SetActive(true);
			oldConveyor.OpenUIData();

			_selectionManager.Select(oldConveyor.transform);
		}
	}

	public struct MachineRemovedCommand : ICommand
	{
		private readonly ISelectionManager _selectionManager;
		private readonly IAdditionalSelectionService _additionalSelection;
		private Transform _selectedMachine;
		private List<Transform> _additionals;

		public MachineRemovedCommand(ISelectionManager selectionManager, List<Transform> additionals, IAdditionalSelectionService additionalSelectionService)
		{
			_selectedMachine = additionals[0];
			additionals.Remove(_selectedMachine);
			_selectionManager = selectionManager;
			_additionals = additionals;
			_additionalSelection = additionalSelectionService;
		}

		public void Undo()
		{
			foreach (var machine in _additionals)
			{
				machine.gameObject.SetActive(true);
				var machineC = machine.GetComponent<Machine>();
				machineC.InfoChartViewModel.Open();
				_additionalSelection.AddingProcess(machine.transform, _selectedMachine.transform);
				MachineRTSSelectionController.Add(machineC);
			}

			var selectedMachine = _selectedMachine.GetComponent<Machine>();
			selectedMachine.InfoChartViewModel.Open();
			_selectedMachine.gameObject.SetActive(true);
			MachineRTSSelectionController.Add(selectedMachine);
			_selectionManager.Select(_selectedMachine.transform);
		}
	}

	public struct MachineRemovedFromUICommand : ICommand
	{
		private Machine _selectedMachine;

		public MachineRemovedFromUICommand(Machine selectedMachine)
		{
			_selectedMachine = selectedMachine;
		}

		public void Undo()
		{
			_selectedMachine.InfoChartViewModel.Open();
			_selectedMachine.gameObject.SetActive(true);
			MachineRTSSelectionController.Add(_selectedMachine);
		}
	}

	public struct HideCommand : ICommand
	{
		private IHideable _hideableObject;

		public HideCommand(IHideable hideableObject)
		{
			_hideableObject = hideableObject;
		}

		public void Undo()
		{
			_hideableObject.Unhide();
		}
	}

	public struct UnhideCommand : ICommand
	{
		private IHideable _hideableObject;

		public UnhideCommand(IHideable hideableObject)
		{
			_hideableObject = hideableObject;
		}

		public void Undo()
		{
			_hideableObject.HideWithCommand();
		}
	}

	public struct PivotChangeCommand : ICommand
	{
		private Transform _postPivot;
		private Transform _prePivot;
		private readonly IAdditionalSelectionService _additionalSelectionService;
		private readonly ISelectionManager _selectionManager;

		public PivotChangeCommand(Transform postPivot, Transform prepivot, IAdditionalSelectionService additionalSelectionService, ISelectionManager selectionManager)
		{
			_postPivot = postPivot;
			_prePivot = prepivot;
			_additionalSelectionService = additionalSelectionService;
			_selectionManager = selectionManager;
		}

		public void Undo()
		{
			_additionalSelectionService.RevertPivot(_prePivot);
			_additionalSelectionService.AddingProcess(_postPivot, _prePivot);
			_selectionManager.Select(_prePivot);
		}
	}

	public struct ConcealerCreatedCommand : ICommand
	{
		private ConcealerObject _concealer;

		public ConcealerCreatedCommand(ConcealerObject concealer)
		{
			_concealer = concealer;
		}

		public void Undo()
		{
			_concealer.gameObject.SetActive(false);
			_concealer.ViewModel.gameObject.SetActive(false);
			foreach (var cc in _concealer.ConcealerGroup)
			{
				cc.gameObject.SetActive(false);
				cc.GetComponent<ConcealerObject>().ViewModel.gameObject.SetActive(false);
			}
		}
	}

	public struct ConcealerPivotChangedCommand : ICommand
	{
		private ConcealerObject _prePivot;
		private ISelectionManager _selectionManager;

		public ConcealerPivotChangedCommand(ConcealerObject prepivot, ISelectionManager selectionManager)
		{
			_prePivot = prepivot;
			_selectionManager = selectionManager;
		}

		public void Undo()
		{
			_prePivot.transform.SetParent(null);
			foreach (var cc in _prePivot.ConcealerGroup)
			{
				cc.SetParent(_prePivot.transform);
			}

			_selectionManager.Select(_prePivot.transform);
		}
	}

	public struct ConcealerRemovedCommand : ICommand
	{
		private ConcealerObject _concealer;

		public ConcealerRemovedCommand(ConcealerObject concealer)
		{
			_concealer = concealer;
		}

		public void Undo()
		{
			_concealer.gameObject.SetActive(true);
			_concealer.transform.SetParent(null);
			_concealer.Unhide();

			foreach (var cc in _concealer.ConcealerGroup)
			{
				if (cc.gameObject == _concealer.gameObject) continue;

				cc.gameObject.SetActive(true);
				cc.GetComponent<ConcealerObject>().Unhide();
				cc.transform.SetParent(_concealer.transform);
			}
		}
	}

	public struct ConcealerSeperatodCommand : ICommand
	{
		private ConcealerObject _concealer;
		private List<Transform> _concealers;
		private Transform _scene;

		public ConcealerSeperatodCommand(ConcealerObject concealer, List<Transform> concealers, Transform scene)
		{
			_concealer = concealer;
			_concealers = concealers;
			_scene = scene;
		}

		public void Undo()
		{
			_concealer.transform.SetParent(_scene);
			_concealer.SetGroup(_concealers);

			foreach (var cc in _concealers)
			{
				if (cc.gameObject == _concealer.gameObject) continue;

				cc.gameObject.SetActive(true);
				cc.transform.SetParent(_concealer.transform);
				cc.GetComponent<ConcealerObject>().SetGroup(_concealers);
			}
		}
	}
}
