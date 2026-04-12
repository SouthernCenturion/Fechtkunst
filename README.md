# Fechtkunst

A 2-player side-scrolling dueling game inspired by the art of German sword fighting (Fechtkunst). Players duel one another in a best-of-three format using a timing-based attack and parry system. Attacks come in three directions — high, mid, and low — and must be met with the matching guard to be parried. A single unblocked hit ends the round.

---

## Setup Instructions

### Prerequisites
- Unity 6 (6000.3.4f1 or later)
- Unity Hub
- Git

### Clone and Open
```bash
git clone https://github.com/SouthernCenturion/Fechtkunst.git
cd Fechtkunst
```

1. Open **Unity Hub**
2. Click **Add → Add project from disk**
3. Navigate to the cloned folder and select the **Fechtkunst** subfolder (the one containing the Assets folder)
4. Open the project — Unity will import all packages automatically
5. Once open, load the scene at `Assets/Scenes/DuelScene.unity`

---

## How to Test Multiplayer

This project uses **ParrelSync** to run two editor instances simultaneously, which is the recommended way to test multiplayer locally.

1. Open the project in Unity
2. Go to **ParrelSync → Clones Manager**
3. Click **Open in New Editor** to launch a clone instance
4. In the **main editor**: Press Play, then click **Host**
5. In the **clone editor**: Press Play, then click **Join**
6. Both players will spawn in the DuelScene and can play against each other

### Controls
| Input | Action |
|---|---|
| A / D | Move left / right |
| Up Arrow | Attack high |
| Right Arrow | Attack mid |
| Left Arrow | Attack low |
| Shift + Up Arrow | Guard high |
| Shift + Right Arrow | Guard mid |
| Shift + Left Arrow | Guard low |

### Win Condition
First player to win 2 rounds wins the match. A round ends when one player lands an unblocked hit.

---

## Technical Requirements

### 1. Singleton Pattern
**File:** `Assets/Scripts/GameManager.cs`

`GameManager` implements the Singleton pattern to ensure only one instance exists across all scenes. It tracks player scores, round state, and persists across scene loads via `DontDestroyOnLoad`.

```csharp
public static GameManager Instance { get; private set; }
```

### 2. Delegate Usage
**File:** `Assets/Scripts/GameManager.cs`

A delegate is defined and invoked inside `RegisterHitServerRpc()` to notify other systems when a player scores a hit.

```csharp
public delegate void OnHitDelegate(ulong scoringPlayerId);
public static event OnHitDelegate OnPlayerScored;
```

Invoked in `RegisterHitServerRpc()` when a hit is confirmed server-side.

### 3. Additional Design Pattern — State Pattern
**File:** `Assets/Scripts/PlayerController.cs`

Combat uses a State pattern via the `CombatState` enum to track each player's current action. This state is synced across the network using a `NetworkVariable`, allowing both clients to read each other's combat state for hit and parry resolution.

```csharp
public enum CombatState
{
    Neutral, AttackHigh, AttackMid, AttackLow,
    GuardHigh, GuardMid, GuardLow, Dodge
}
```

---

## Known Bugs / Incomplete Features
- Client player does not spawn at the correct spawn point on connect or after a round reset (planned fix when LobbyScene is wired up)
- No visual feedback for attacks or parries yet — final build will have animations and effects
- Debug combat state text is visible on screen (to be removed)
- No scene transition when the match ends — "Match over!" currently logs to console only
- Scores and match state do not reset between rematches
- Dodge mechanic (Space bar, 3 charges) is designed but not yet implemented

---

## Project Structure
```
Assets/
├── Scripts/
│   ├── GameManager.cs       # Singleton, delegate, score tracking
│   ├── PlayerController.cs  # Input, movement, combat state machine
│   ├── HitDetection.cs      # Attack/parry resolution
│   ├── PlayerSpawner.cs     # Network spawn point management
│   ├── NetworkManagerUI.cs  # Host/Join buttons
│   └── GameUI.cs            # Score display
├── Scenes/
│   ├── DuelScene.unity      # Main playable scene
│   ├── MainMenuScene.unity
│   ├── LobbyScene.unity
│   ├── ShopScene.unity
│   ├── SettingsScene.unity
│   └── GameOverScene.unity
├── Prefabs/
│   └── Player.prefab        # Networked player object
└── Plugins/
    └── ParrelSync/          # Multiplayer testing tool
```
