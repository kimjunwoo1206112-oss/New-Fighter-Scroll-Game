# Fighter Scroll Game (파이터 스크롤 게임)

## 🎯 Project Overview
A classic **Vertical Scrolling Shooter** developed using **Unity** and **C#**. The game features classic arcade-style gameplay with player upgrades, enemy patterns, and intense boss fights.

### 🛠 Main Technologies
- **Game Engine:** Unity 2022.3.x or later
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Input System:** Unity Input System (New)
- **2D Support:** 2D Animation, 2D Sprite, SpriteShape, Tilemap
- **Testing:** Unity Test Framework (TDD approach)

---

## 📂 Directory Structure
- `Assets/Scripts/`: Core game logic.
  - `Player/`: `PlayerController.cs`, `Enemy.cs`, `Boss.cs`, `Bullet.cs`.
  - `Common/`: Shared utilities and constants.
- `Assets/Documents/`: Project documentation.
  - `Design/`: Game design documents (`GameDesign.md`).
  - `Implementation/`: Implementation plans (`ImplementationPlan.md`, `Milestone.md`).
- `Assets/Tests/`: Automated test suite organized by development phase.
  - `Phase1/` to `Phase5/`: Test files for each milestone.
- `Assets/Settings/`: URP and Renderer configuration.

---

## 🚀 Building and Running
1.  **Open Project:** Open the root directory in **Unity Hub**.
2.  **Scene:** Open `Assets/Scenes/SampleScene.unity`.
3.  **Run:** Click the **Play** button in the Unity Editor.
4.  **Build:** Go to `File -> Build Settings...` to build for your target platform (primarily PC).

---

## 🧪 Testing Guidelines
This project follows a **Phase-based TDD (Test-Driven Development)** workflow.

### How to Run Tests:
1.  Open **Window** -> **General** -> **Test Runner** (`Ctrl + Shift + T`).
2.  Select the **PlayMode** tab.
3.  Choose the test suite for the current phase (e.g., `Phase1/PlayerControllerTests.cs`).
4.  Click **Run All** or **Run Selected**.

Refer to `Assets/Tests/TestRunner.cs` for more detailed testing instructions.

---

## 🛠 Development Conventions
- **Coding Style:**
  - Use `[SerializeField]` to expose private fields to the Unity Inspector.
  - Prefix private fields with `camelCase` (e.g., `moveSpeed`).
  - Use `PascalCase` for public methods and properties.
  - Logic separation: Keep physics logic in `FixedUpdate` and input/timing in `Update`.
- **Phased Implementation:**
  - **Phase 1:** Player movement, combat, and life systems.
  - **Phase 2:** Enemy and Boss systems.
  - **Phase 3:** Item drop and basic upgrades.
  - **Phase 4:** Advanced upgrade levels and fighter selection.
  - **Phase 5:** Game flow, UI, and stage progression.
- **Workflow:** Research -> Plan (with Tests) -> Implement -> Validate (Tests) -> Review.

---

## 🗒 Key Files
- `Assets/Documents/Design/GameDesign.md`: Detailed game mechanics and features.
- `Assets/Documents/Implementation/ImplementationPlan.md`: Detailed development roadmap.
- `Assets/Scripts/Player/PlayerController.cs`: Central player logic including movement, combat, and stats.
- `Packages/manifest.json`: Project dependencies and package versions.
