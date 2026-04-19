# Fechtkunst

A 2-player side-scrolling dueling game inspired by the art of German sword fighting (Fechtkunst). 
Players duel one another in a best-of-three format using a timing-based attack and parry system. 
Attacks come in three directions — high, mid, and low — and must be met with the matching guard 
to be parried. A single unblocked hit ends the round.

---

## Setup Instructions

### Prerequisites
- Unity 6 (6000.3.4f1 or later)
- Unity Hub
- Git

### Clone and Open
```
git clone https://github.com/SouthernCenturion/Fechtkunst.git
cd Fechtkunst
```

1. Open Unity Hub
2. Click Add → Add project from disk
3. Navigate to the cloned folder and select the Fechtkunst subfolder (the one containing the Assets folder)
4. Open the project — Unity will import all packages automatically
5. Once open, load the scene at `Assets/Scenes/MainMenuScene.unity`

---

## How to Play

### Controls
| Input | Action |
| --- | --- |
| A / D | Move left / right |
| Up Arrow | Attack high |
| Right Arrow | Attack mid |
| Left Arrow | Attack low |
| Shift + Up Arrow | Guard high |
| Shift + Right Arrow | Guard mid |
| Shift + Left Arrow | Guard low |
| Escape | Pause / Unpause |

### Objective
First player to win 2 rounds wins the match. A round ends when one player lands an unblocked hit. 
A hit is blocked only if the defender is holding the matching guard direction.

---

## How to Test Multiplayer

This project uses ParrelSync to run two editor instances simultaneously, 
which is the recommended way to test multiplayer locally.

1. Open the project in Unity
2. Go to ParrelSync → Clones Manager
3. Click Open in New Editor to launch a clone instance
4. In both editors: Press Play — both will start at the Main Menu
5. In both editors: Click Play to navigate to the Lobby
6. In the main editor: Click Host
7. In the clone editor: Click Join
8. In the main editor: Click Arena 1 or Arena 2 to load a scene
9. Both players will spawn in the selected scene and can play against each other

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── GameManager.cs         # Singleton, delegate, score tracking
│   ├── PlayerController.cs    # Input, movement, combat state machine
│   ├── HitDetection.cs        # Server authoritative attack/parry resolution
│   ├── PlayerSpawner.cs       # Network spawn point management
│   ├── AttackVisual.cs        # Swoosh visual for attacks and guards
│   ├── LobbyManager.cs        # Host/Join/Start buttons, scene selection
│   ├── MainMenuManager.cs     # Main menu navigation
│   ├── GameOverManager.cs     # Winner display, win tracking
│   ├── PauseManager.cs        # Pause/resume, volume controls
│   ├── AudioManager.cs        # Central audio management
│   ├── DatabaseManager.cs     # SQLite win tracking
│   ├── SaveLoadManager.cs     # JSON settings persistence
│   └── GameUI.cs              # Score display
├── Scenes/
│   ├── MainMenuScene.unity    # Main menu
│   ├── LobbyScene.unity       # Host/Join lobby with scene selection
│   ├── DuelScene.unity        # Daytime arena
│   ├── DuelScene2.unity       # Nighttime arena
│   ├── GameOverScene.unity    # Winner display and win count
│   └── SettingsScene.unity    # Unused
├── Prefabs/
│   └── Player.prefab          # Networked player object
└── Plugins/
    └── ParrelSync/            # Multiplayer testing tool
```

---

## Technical Implementation

### 1. Singleton Pattern
**File:** `Assets/Scripts/GameManager.cs`

GameManager implements the Singleton pattern to ensure only one instance exists per scene.
It tracks player scores and round state.

```csharp
public static GameManager Instance { get; private set; }
```

Additional singletons: `AudioManager`, `DatabaseManager`, `SaveLoadManager`.

### 2. Delegate Usage
**File:** `Assets/Scripts/GameManager.cs`

A delegate is defined and invoked inside `RegisterHitServerRpc()` to notify other systems 
when a player scores a hit.

```csharp
public delegate void OnHitDelegate(ulong scoringPlayerId);
public static event OnHitDelegate OnPlayerScored;
```

### 3. Additional Design Pattern — State Pattern
**File:** `Assets/Scripts/PlayerController.cs`

Combat uses a State pattern via the `CombatState` enum to track each player's current action. 
This state is synced across the network using a `NetworkVariable`.

```csharp
public enum CombatState
{
    Neutral, AttackHigh, AttackMid, AttackLow,
    GuardHigh, GuardMid, GuardLow, Dodge
}
```

### 4. Database Integration
**File:** `Assets/Scripts/DatabaseManager.cs`

SQLite database stores player win counts persistently using sqlite-net-pcl. 
Win totals are displayed on the GameOver screen and accumulate across sessions.

### 5. Save/Load System
**File:** `Assets/Scripts/SaveLoadManager.cs`

Volume settings (master, music, SFX) are saved to a JSON file using `JsonUtility` 
and loaded on startup so player preferences persist between sessions.

### 6. Audio and Lighting
**File:** `Assets/Scripts/AudioManager.cs`

Central AudioManager handles all music and SFX with separate volume controls. 
Both DuelScene and DuelScene2 include a Global Light 2D and a Spot Light 2D sun source.

---

## Known Issues
- Dodge mechanic is designed but not implemented
- Parry sound plays multiple times if attack state persists across frames

---

## Technologies Used
- Unity 6 (URP)
- Unity Netcode for GameObjects (NGO)
- Unity Relay / UnityTransport
- ParrelSync
- SQLite (sqlite-net-pcl via NuGetForUnity)
- TextMeshPro
