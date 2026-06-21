# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

A Unity prototype for a top-down 2D "spin" game: the player builds up speed while moving and spins faster the more speed they have, dodging/dashing away from bullets spawned around the screen edges. Built with Unity 6000.3.8f1, URP, the new Input System (`UnityEngine.InputSystem`), and Rigidbody2D physics.

## Development

This is a Unity Editor project — there is no CLI build/test/lint pipeline checked into the repo. Common work is done by opening the project in Unity Editor (6000.3.8f1) and pressing Play on `Assets/Scenes/SampleScene.unity`.

- `com.unity.test-framework` is a dependency but no test assemblies exist yet under `Assets/`. If adding tests, use Unity's Test Runner (EditMode/PlayMode) conventions (`*.Tests.asmdef`, `[Test]`/`[UnityTest]`).
- There is no command-line build script; builds/playmode runs go through the Unity Editor or `Unity.exe -batchmode -projectPath . -executeMethod ...` if needed.

## Architecture

### Player (`Assets/Scripts/Spiner.cs`, class `Spin`)
The core player controller. Note the file name (`Spiner.cs`) does not match the class name (`Spin`).
- Movement is ratio-based, not raw velocity: `speedRatio` (0–1) lerps between `minMoveSpeed`/`maxMoveSpeed`, driven by WASD/arrow keys read directly from the new Input System (`Keyboard.current`).
- Changing direction doesn't snap instantly — pressing a new heading first bleeds `speedRatio` down via `deceleration` until it crosses `reverseThreshold`, only then does `direction` actually change. This is what gives the "momentum" feel.
- The visual spin (`visual.RotateAround(...)`) is decoupled from movement direction and only reacts to `speedRatio`, via `minSpinSpeed`/`maxSpinSpeed`.
- Dash ("skill") is triggered by spacebar, fires toward the mouse cursor (`GetMouseDirection`), has its own cooldown with a color fade on `visualRenderer` as a readiness indicator.
- Damage comes from trigger/collision with anything tagged `Bullet`; taking damage triggers a camera screen-shake coroutine relative to `Camera.main`. `Die()` is `protected virtual` and just disables the object — override it in subclasses for death effects/animations.
- There's a separate, unrelated `Movement.cs` (`Assets/Scripts/Controller/`) and `EnemySpinner.cs` (`Assets/Scripts/Enemy/`) that implement an older/simpler velocity-based movement+spin pattern (Rigidbody2D acceleration, `transform.Rotate`). These predate/duplicate parts of `Spin` — check which one a given GameObject in the scene actually uses before editing.

### Bullets (`Assets/Scripts/Bullet/`)
Pooling pattern, not Instantiate/Destroy at runtime:
- `BulletPool` is a singleton (`BulletPool.Instance`) that pre-instantiates `poolSize` bullets, all inactive, in `Start()`. `GetPoolObject()` linear-scans for an inactive instance.
- `SpawnBullet` computes screen-edge spawn bounds from `Camera.main.orthographicSize`/`aspect` and spawns bullets in batches (`batchSize` every `spawnIntervalSeconds`) from a random side (top/bottom/left/right), aimed at `playerPosition` at spawn time only (no homing).
- `Bullet.Initialize(targetPosition)` sets a fixed normalized direction once; movement is constant-velocity in `FixedUpdate` via `Rigidbody2D.linearVelocity`.
- Bullets deactivate themselves (return to pool) on `OnBecameInvisible`, on colliding with something tagged `Player`, or on trigger-exiting something tagged `Bounder` — there's no explicit pool "release" call from outside the bullet itself.

### Tags relied on by code
`Player`, `Bullet`, `Bounder` — these must exist in `ProjectSettings/TagManager.asset` and be assigned on the relevant GameObjects/prefabs for collision logic to fire.

### Mockup/ (`Mockup/`)
Static HTML/CSS/JS mockups (`main-menu.html`, `gameplay.html`, `lose-screen.html`, `styles.css`, `transition.js`) prototyping UI flow and screen transitions outside of Unity — useful as a design reference for HUD/menu work but not wired into the Unity project.
