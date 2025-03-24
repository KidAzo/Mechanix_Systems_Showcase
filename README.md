# 🧩 FactoryTemplate Systems Showcase

This repository showcases three key architectural systems developed in **Unity C#** using best practices like **Dependency Injection (Zenject)**, **Command Pattern**, and **Strategy Pattern**. The purpose of this repo is to demonstrate clean, modular, and scalable code in a professional Unity project context.

This repo contains only the scripts of some systems of the program. Full video of the program: 
https://www.youtube.com/watch?v=Go8KRYPfkJs&list=TLGGcs1D4ItDK7cyNDAzMjAyNQ

---

## 📦 Project Structure

_assets/ └── _scripts/ ├── RemovingSystem/ ├── SelectionStrategySystem/ └── UndoSystem/
---

## 🗑️ RemovingSystem

Handles the removal of machines or custom objects (e.g., `ConcealerObject`) from the scene. This system is tightly integrated with the selection and undo systems to ensure consistent user interaction and reversibility.

### 🔧 Core: `RemoveManager`

- Removes selected objects via UI or input (e.g., `Delete` key)
- Tracks selection via `ISelectionManager` and `IAdditionalSelectionService`
- Integrates with undo system using `ICommandManager`

### ✅ Features

- Unity UI and input-triggered deletion
- Multi-object removal support
- Object type checking (`Machine`, `ConcealerObject`)
- Undoable commands: `MachineRemovedCommand`, `ConcealerRemovedCommand`

---

## 🧠 SelectionStrategySystem

A highly modular selection architecture that dynamically switches selection behavior based on the active tool. Implements the **Strategy Pattern**.

### 🔧 Core: `SelectionStrategyController`

- Dynamically changes strategy via `NavigationTool` enum
- Strategies implement `ISelectionStrategy`

### 📌 Supported Strategies

| Tool              | Strategy Class                | Behavior                             |
|-------------------|-------------------------------|--------------------------------------|
| Movement          | `MovementSelectionStrategy`    | Selects for movement                 |
| Rotation          | `RotationSelectionStrategy`    | Selects for rotation                 |
| Snap              | `SnappingSelectionStrategy`    | Selects for grid/magnet snapping     |
| Remove            | `RemovingSelectionStrategy`    | Selects for deletion                 |
| Empty             | `EmptySelectionStrategy`       | No selection                         |

### ✅ Features

- Strategy-based selection logic
- Event-driven (e.g., `SnappingSelectionEvent`, `RotationSelectionEvent`)
- Accessor selection support for snapping tools

---

## ♻️ UndoSystem

An extensible undo system built with the **Command Pattern**, allowing every user action to be stored and reversed reliably.

### 🔧 Core: `CommandManager`

- Singleton-based manager for `ICommand` actions
- Supports up to 20 stacked commands (`CustomStack<T>`)
- Binds undo logic to input via `IInputManager`

### 📦 Sample Commands

| Command Type              | Undo Behavior Description                               |
|---------------------------|----------------------------------------------------------|
| `MoveCommand`             | Restores previous position                              |
| `RotationCommand`         | Reverts rotation                                         |
| `MachineCreatedCommand`   | Removes created machines                                 |
| `MachineRemovedCommand`   | Restores removed machines                                |
| `ConveyorChangedCommand`  | Switches back to the previous conveyor                   |
| `ConcealerRemovedCommand` | Re-displays concealer objects and their hierarchy        |
| `PivotChangeCommand`      | Reverts pivot transformation                             |

### ✅ Features

- Command encapsulation for all user actions
- System-agnostic undo logic
- Modular and extendable command set

---

## 🛠️ Tech Stack

- 🎮 Unity Engine (2022+)
- 💉 Zenject (Dependency Injection)
- 🧠 Design Patterns:
  - Strategy Pattern
  - Command Pattern
- 🧪 Event Bus Architecture
- 🧹 Clean Code Practices

---

## 📚 License

This project is for commercial purposes.
---

## 👨‍💻 Author

Developed by [Azizhan Cil] – a Unity developer passionate about scalable architecture and maintainable gameplay systems.


