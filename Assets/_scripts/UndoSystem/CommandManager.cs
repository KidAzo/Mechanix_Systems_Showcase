using FactoryTemplate.EventHandling;
using FactoryTemplate.Utils;
using System.Windows.Input;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace FactoryTemplate.UndoSystem
{
	public class CommandManager : Singleton<CommandManager>, ICommandManager
	{
		[Inject] private IInputManager _inputManager;
		private CustomStack<ICommand> _commands;
		private ICommand _currentCommand;
		private int _maxCommandCount = 20;

		public static void CreateCommand(ICommand command)
		{
			instance._currentCommand = command;
			instance._commands.Push(command);

			if (instance._commands.Count >= instance._maxCommandCount)
			{
				instance._commands.Remove(0);
			}
		}

		public void Undo()
		{
			if (_commands.Count > 0)
			{
				ICommand command = _commands.Pop();
				command.Undo();
			}
		}

		private void Start()
		{
			_commands = new CustomStack<ICommand>();
		}

		private void OnEnable()
		{
			_inputManager.OnUndo += Undo;
		}

		private void OnDisable()
		{
			_inputManager.OnUndo -= Undo;
		}
	}
}
