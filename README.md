# Interactive Elements Demo - Unity Project

## Table of Contents
- [Project Overview](#project-overview)
- [Features](#features)
- [Installation & Setup](#installation--setup)
- [How the Code Works](#how-the-code-works)
  - [1. PlayerCubeController](#1-playercubecontroller)
  - [2. CameraFollow](#2-camerafollow)
  - [3. RotatingPlatform](#3-rotatingplatform)
  - [4. ClickableObject](#4-clickableobject)
  - [5. BouncingBall](#5-bouncingball)
  - [6. ObjectSpawner](#6-objectspawner)
  - [7. UIManager](#7-uimanager)
- [System Architecture](#system-architecture)
- [Controls](#controls)
- [Technical Details](#technical-details)
- [Customization](#customization)
- [Troubleshooting](#troubleshooting)

---

## Project Overview

This Unity project demonstrates **7 different interactive elements** showcasing various Unity systems including:
- Player input handling (keyboard and mouse)
- Physics simulation
- Object instantiation and destruction
- Material and color manipulation
- UI interaction
- Camera control
- Timed events

**Input System:** Supports both Legacy Input Manager and new Input System

---

## Features

### Interactive Elements

1. **WASD Cube with Color Trail**
   - Keyboard-controlled movement
   - Dynamic object spawning
   - Color cycling system
   - Timed destruction

2. **Rotating Platform**
   - Continuous rotation
   - Scale pulsing animation
   - Color transitions

3. **Clickable Spheres**
   - Mouse raycast detection
   - Color randomization
   - Scale animation

4. **Physics Bouncing Ball**
   - Rigidbody physics
   - Collision detection
   - Dynamic materials

5. **Object Spawner UI**
   - UI button interaction
   - Runtime object instantiation
   - Physics integration

6. **Camera Follow System**
   - Smooth camera tracking
   - Lerp-based movement

7. **UI Manager**
   - Dynamic instructions
   - Toggle visibility
   - Auto-configuration

---

## Installation & Setup

### Prerequisites
- Unity 2021.3 or later
- Basic understanding of Unity Editor

### Quick Start

1. **Create New Unity Project**
   ```
   - Open Unity Hub
   - New Project → 3D (Core or URP)
   - Name: "Interactive Elements Demo"
   ```

2. **Configure Input System**
   ```
   Edit → Project Settings → Player → Other Settings
   Active Input Handling → "Input Manager (Old)" or "Both"
   Restart Unity
   ```

3. **Create Scripts Folder**
   ```
   Assets → Right Click → Create → Folder → "Scripts"
   ```

4. **Add All Scripts**
   - Copy all 7 scripts into the Scripts folder
   - Wait for compilation

5. **Build Scene Hierarchy**
   - Follow the setup guide to create GameObjects
   - Attach appropriate scripts
   - Configure Inspector values

6. **Press Play!**

---

## How the Code Works

### 1. PlayerCubeController

**File:** `PlayerCubeController.cs`  
**Attached to:** PlayerCube GameObject

#### Purpose
Controls player movement and manages the trail spawning system.

#### How It Works

**A. Movement System**
```csharp
void HandleMovement()
{
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    Vector3 movement = new Vector3(horizontal, 0, vertical);
    
    if (movement.magnitude > 0.1f)
    {
        movement = movement.normalized;
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}
```

**Explanation:**
- `Input.GetAxis()` reads WASD/Arrow keys and returns values between -1 and 1
- Horizontal: A/D or Left/Right arrows (X-axis movement)
- Vertical: W/S or Up/Down arrows (Z-axis movement)
- `movement.normalized` prevents faster diagonal movement by keeping magnitude at 1
- `transform.Translate()` moves the cube in world space
- `Time.deltaTime` ensures frame-rate independent movement

**B. Trail Spawning System**
```csharp
void SpawnTrailSphere()
{
    GameObject sphere = Instantiate(trailSpherePrefab, transform.position, Quaternion.identity);
    sphere.SetActive(true);
    
    if (syncColors)
    {
        sphere.AddComponent<TrailSphere>().Initialize(sphereLifetime, this);
    }
    
    Destroy(sphere, sphereLifetime);
}
```

**Explanation:**
- `Instantiate()` creates a new sphere GameObject at the cube's current position
- `Quaternion.identity` means no rotation
- If `syncColors` is true, adds a `TrailSphere` component that updates its color every frame
- `Destroy(sphere, sphereLifetime)` automatically destroys the sphere after 5 seconds
- This prevents memory buildup from infinite sphere spawning

**C. Color Cycling System**
```csharp
void UpdateTrailColor()
{
    float hue = (Time.time * colorChangeSpeed * 0.1f) % 1f;
    currentTrailColor = Color.HSV(hue, 0.8f, 1f);
}
```

**Explanation:**
- `Time.time` is total seconds since game start
- Multiply by `colorChangeSpeed` to control how fast colors change
- `% 1f` keeps hue value between 0 and 1 (wraps around)
- `Color.HSV()` converts HSV (Hue, Saturation, Value) to RGB
- Hue cycles through rainbow: 0=red, 0.33=green, 0.66=blue, 1=red again
- Saturation at 0.8 (80%) gives vibrant but not oversaturated colors
- Value at 1.0 (100%) gives maximum brightness

**D. TrailSphere Helper Class**
```csharp
public class TrailSphere : MonoBehaviour
{
    void Update()
    {
        if (controller != null)
        {
            sphereRenderer.material.color = controller.GetCurrentTrailColor();
        }
    }
}
```

**Explanation:**
- Each sphere has this component attached
- Every frame, it reads the current color from PlayerCubeController
- Updates its own material color to match
- This creates synchronized color changes across all spheres
- When `controller` is null (syncColors = false), spheres keep their random color

---

### 2. CameraFollow

**File:** `CameraFollow.cs`  
**Attached to:** Main Camera

#### Purpose
Creates smooth camera movement that follows the player cube.

#### How It Works

```csharp
void LateUpdate()
{
    Vector3 desiredPosition = target.position + offset;
    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    transform.position = smoothedPosition;
    
    if (lookAtTarget)
    {
        transform.LookAt(target);
    }
}
```

**Explanation:**

**Why LateUpdate()?**
- `LateUpdate()` runs AFTER `Update()` every frame
- This ensures the player moves first, then the camera follows
- Prevents jittery camera movement that would occur if camera moved before player

**Position Calculation:**
- `desiredPosition = target.position + offset`
  - Gets the player's position
  - Adds offset (0, 5, -10) to position camera above and behind
  - Example: Player at (0,0,0) → Camera wants to be at (0,5,-10)

**Smooth Following with Lerp:**
```csharp
Vector3.Lerp(currentPos, targetPos, smoothSpeed)
```
- Lerp = Linear Interpolation (smooth transition between two values)
- `smoothSpeed` of 0.125 means camera moves 12.5% closer each frame
- This creates smooth, lag-following effect
- Lower smoothSpeed = more lag (cinematic)
- Higher smoothSpeed = less lag (responsive)

**Mathematical behavior:**
- Frame 1: Move 12.5% of distance
- Frame 2: Move 12.5% of remaining distance
- Frame 3: Move 12.5% of remaining distance
- Asymptotically approaches target (never quite reaches, but gets very close)

**Look At Target:**
- `transform.LookAt(target)` rotates camera to face the player
- This keeps player centered in view even as it moves
- Can be disabled for fixed-angle following

---

### 3. RotatingPlatform

**File:** `RotatingPlatform.cs`  
**Attached to:** RotatingPlatform GameObject

#### Purpose
Demonstrates continuous transformation animations (rotation, scale, color).

#### How It Works

**A. Rotation**
```csharp
transform.Rotate(rotationSpeed * Time.deltaTime);
```

**Explanation:**
- `rotationSpeed` is set to (0, 50, 0) = 50 degrees/second on Y-axis
- `Time.deltaTime` is time since last frame (usually ~0.016s for 60fps)
- Multiply: 50 * 0.016 = 0.8 degrees per frame
- 60 frames/second * 0.8 degrees = 48 degrees/second (close to 50)
- Platform makes full 360° rotation every 7.2 seconds
- Uses local rotation, so it spins around its own center

**B. Scale Pulsing**
```csharp
float scale = Mathf.Lerp(minScale, maxScale, 
    (Mathf.Sin(Time.time * scaleSpeed) + 1f) / 2f);
transform.localScale = originalScale * scale;
```

**Explanation - Step by Step:**

1. **Sine Wave Generation:**
   ```csharp
   Mathf.Sin(Time.time * scaleSpeed)
   ```
   - `Sin()` produces wave oscillating between -1 and 1
   - `Time.time * scaleSpeed` (scaleSpeed=2) makes it cycle faster
   - Completes one full sine wave every π seconds (~3.14s)

2. **Normalize to 0-1 Range:**
   ```csharp
   (Mathf.Sin(...) + 1f) / 2f
   ```
   - Add 1: shifts range from [-1,1] to [0,2]
   - Divide by 2: compresses range to [0,1]
   - Now we have smooth 0→1→0 oscillation

3. **Map to Scale Range:**
   ```csharp
   Mathf.Lerp(minScale, maxScale, normalizedValue)
   ```
   - minScale = 0.8, maxScale = 1.2
   - When normalizedValue = 0: scale = 0.8 (80% size)
   - When normalizedValue = 0.5: scale = 1.0 (100% size)
   - When normalizedValue = 1: scale = 1.2 (120% size)

4. **Apply Scale:**
   ```csharp
   transform.localScale = originalScale * scale
   ```
   - Multiply original scale by the calculated multiplier
   - Preserves original proportions while scaling

**Result:** Smooth breathing/pulsing animation

**C. Color Cycling**
```csharp
float hue = (Time.time * colorChangeSpeed * 0.1f) % 1f;
platformRenderer.material.color = Color.HSV(hue, 0.6f, 0.9f);
```

**Explanation:**
- Same HSV color cycling as trail spheres
- `colorChangeSpeed * 0.1` makes it cycle slower than trail
- Saturation at 0.6 (less vibrant for platform)
- Value at 0.9 (slightly dimmer than trail)
- Creates distinct visual identity while maintaining color harmony

---

### 4. ClickableObject

**File:** `ClickableObject.cs`  
**Attached to:** ClickableSphere1, ClickableSphere2, ClickableSphere3

#### Purpose
Detects mouse clicks on 3D objects and triggers visual responses.

#### How It Works

**A. Click Detection with Raycasting**
```csharp
if (Input.GetMouseButtonDown(0))
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    
    if (Physics.Raycast(ray, out hit))
    {
        if (hit.transform == transform)
        {
            OnClicked();
        }
    }
}
```

**Explanation - The Raycasting Process:**

1. **Detect Click:**
   - `Input.GetMouseButtonDown(0)` returns true on left click press
   - Only triggers ONCE per click (not every frame)

2. **Create Ray:**
   ```csharp
   Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition)
   ```
   - Gets mouse position in screen coordinates (pixels)
   - Example: (800, 600) on a 1920x1080 screen
   - Converts to a 3D ray shooting from camera through that screen point
   - Ray has origin (camera position) and direction (toward mouse point)

3. **Cast Ray into Scene:**
   ```csharp
   Physics.Raycast(ray, out hit)
   ```
   - Shoots the ray into the 3D scene
   - Checks for collision with any Collider components
   - `out hit` receives information about what was hit
   - Returns true if ray hit something, false if hit nothing

4. **Check if We Hit THIS Object:**
   ```csharp
   if (hit.transform == transform)
   ```
   - `hit.transform` = the Transform of the object that was hit
   - `transform` = the Transform of THIS script's GameObject
   - Only proceed if we clicked on THIS specific sphere

**Visual Analogy:**
```
Camera → [Screen Point] → Ray → → → [Sphere Collider] = Hit!
                                 ↘ [Nothing] = Miss
```

**B. Scale Animation System**
```csharp
void Update()
{
    if (isAnimating)
    {
        animationTimer += Time.deltaTime;
        float progress = animationTimer / animationDuration;
        
        if (progress < 0.5f)
        {
            float scaleProgress = progress * 2f;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleProgress);
        }
        else
        {
            float scaleProgress = (progress - 0.5f) * 2f;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, scaleProgress);
        }
    }
}
```

**Explanation - Animation Timeline:**

Animation duration = 0.3 seconds

**Phase 1: Scale Up (0.0 → 0.15s)**
- `progress` goes from 0 → 0.5
- `scaleProgress = progress * 2` goes from 0 → 1
- Lerp from original (1.0) to target (1.5)
- Sphere grows to 150% size

**Phase 2: Scale Down (0.15 → 0.3s)**
- `progress` goes from 0.5 → 1.0
- `scaleProgress = (progress - 0.5) * 2` goes from 0 → 1
- Lerp from target (1.5) back to original (1.0)
- Sphere shrinks back to 100% size

**Timeline Diagram:**
```
Time:    0s    0.15s   0.3s
Scale:   1.0 → 1.5 → 1.0
         ___/‾‾‾\___
```

**Why This Approach?**
- Creates symmetric bounce effect
- Smooth acceleration/deceleration
- No external animation system needed
- Lightweight and performant

---

### 5. BouncingBall

**File:** `BouncingBall.cs`  
**Attached to:** BouncingBall GameObject

#### Purpose
Demonstrates Unity's physics system with continuous bouncing and dynamic colors.

#### How It Works

**A. Physics Setup**
```csharp
void Start()
{
    rb = GetComponent<Rigidbody>();
    
    bouncyMaterial = new PhysicMaterial("Bouncy");
    bouncyMaterial.bounciness = 0.9f;
    bouncyMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
    bouncyMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
    
    GetComponent<Collider>().material = bouncyMaterial;
}
```

**Explanation:**

**Rigidbody Component:**
- Unity's physics simulation component
- Adds gravity, collision response, forces
- Mass, drag, and angular drag properties
- Without Rigidbody, object is static

**PhysicMaterial Properties:**
- `bounciness = 0.9` (90% energy retained on bounce)
  - 0.0 = no bounce (absorbs all energy)
  - 1.0 = perfect bounce (impossible in reality)
  - 0.9 = very bouncy (like a superball)

- `frictionCombine = Minimum`
  - How friction is calculated between materials
  - Minimum uses the lower friction value
  - Reduces sliding resistance

- `bounceCombine = Maximum`
  - How bounce is calculated between materials
  - Maximum uses the higher bounce value
  - Ensures ball bounces even on non-bouncy surfaces

**B. Continuous Bouncing System**
```csharp
void Update()
{
    timeSinceLastBounce += Time.deltaTime;
    
    if (timeSinceLastBounce >= bounceInterval)
    {
        ApplyBounceForce();
        timeSinceLastBounce = 0f;
    }
}

void ApplyBounceForce()
{
    rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
    
    Vector3 randomForce = new Vector3(
        Random.Range(-2f, 2f),
        0f,
        Random.Range(-2f, 2f)
    );
    rb.AddForce(randomForce, ForceMode.Impulse);
}
```

**Explanation:**

**Timer System:**
- Tracks time since last manual bounce
- Every 2 seconds (`bounceInterval`), applies new force
- Keeps ball bouncing indefinitely
- Prevents ball from settling and stopping

**Force Application:**
- `Vector3.up * bounceForce` creates upward force
- `bounceForce = 10` in Newton-meters
- `ForceMode.Impulse` = instant force (like hitting ball with bat)
- Alternative would be `ForceMode.Force` (continuous push)

**Random Horizontal Force:**
- Adds unpredictability
- Prevents repetitive bouncing pattern
- X and Z between -2 and 2 (horizontal plane)
- Y is 0 (no vertical component)
- Makes bouncing more interesting

**C. Velocity-Based Color**
```csharp
void Update()
{
    float speed = rb.velocity.magnitude;
    float hue = Mathf.Clamp01(speed / 20f);
    ballRenderer.material.color = Color.HSV(hue, 0.8f, 1f);
}
```

**Explanation:**

**Velocity Magnitude:**
- `rb.velocity` is a Vector3 (x, y, z components)
- `magnitude` calculates total speed: √(x² + y² + z²)
- Example: velocity (3, 4, 0) has magnitude 5

**Speed to Color Mapping:**
- `speed / 20f` normalizes to 0-1 range
  - Speed 0 → hue 0 (red/blue - slow)
  - Speed 20+ → hue 1 (red - fast)
- `Mathf.Clamp01()` keeps result between 0 and 1
- HSV hue interpretation:
  - 0.0 = Red (high speed)
  - 0.16 = Yellow
  - 0.33 = Green
  - 0.5 = Cyan (medium speed)
  - 0.66 = Blue (low speed)

**Visual Feedback:**
- Ball appears blue when moving slowly
- Transitions through cyan, green, yellow
- Turns red when moving fast
- Provides intuitive speed indication

---

### 6. ObjectSpawner

**File:** `ObjectSpawner.cs`  
**Attached to:** SpawnerManager GameObject

#### Purpose
Handles UI button interaction and spawns random physics objects.

#### How It Works

**A. Button Setup**
```csharp
void Start()
{
    if (spawnButton != null)
    {
        spawnButton.onClick.AddListener(SpawnObject);
    }
}
```

**Explanation:**
- `onClick` is a Unity Event (similar to C# events)
- `AddListener()` subscribes a method to the event
- When button is clicked, Unity calls `SpawnObject()`
- This is the Observer pattern in action

**B. Runtime Prefab Creation**
```csharp
if (objectsToSpawn == null || objectsToSpawn.Length == 0)
{
    objectsToSpawn = new GameObject[3];
    objectsToSpawn[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
    objectsToSpawn[1] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    objectsToSpawn[2] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
    
    foreach (GameObject obj in objectsToSpawn)
    {
        obj.SetActive(false);
    }
}
```

**Explanation:**

**Why Create at Runtime?**
- No need to manually create prefabs in editor
- Script is self-contained and portable
- Automatic fallback if no prefabs assigned

**Template Pattern:**
- Creates primitive shapes as templates
- `SetActive(false)` hides them in scene
- They exist but are invisible
- Used as blueprints for instantiation

**C. Spawn Logic**
```csharp
public void SpawnObject()
{
    GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
    
    Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
    randomOffset.y = 0;
    Vector3 spawnPosition = spawnPoint.position + randomOffset + Vector3.up * spawnHeight;
    
    Quaternion spawnRotation = randomRotation ? Random.rotation : Quaternion.identity;
    
    GameObject spawnedObject = Instantiate(prefab, spawnPosition, spawnRotation);
}
```

**Explanation - Step by Step:**

1. **Random Selection:**
   ```csharp
   Random.Range(0, objectsToSpawn.Length)
   ```
   - Returns random integer: 0, 1, or 2
   - Selects one of three shapes randomly

2. **Position Calculation:**
   ```csharp
   Random.insideUnitSphere * spawnRadius
   ```
   - `insideUnitSphere` returns point inside sphere of radius 1
   - Example: (0.5, 0.3, -0.7)
   - Multiply by `spawnRadius` (3) → (1.5, 0.9, -2.1)
   - Set Y to 0 → (1.5, 0, -2.1) (horizontal plane only)
   
   ```csharp
   spawnPoint.position + randomOffset + Vector3.up * spawnHeight
   ```
   - Start at player position
   - Add random horizontal offset (circle around player)
   - Add vertical offset (spawn height)
   - Example: (0,0,0) + (1.5,0,-2.1) + (0,5,0) = (1.5, 5, -2.1)

3. **Rotation:**
   - `Random.rotation` generates completely random rotation
   - Uses quaternions (4D rotation representation)
   - Objects tumble naturally as they fall

**D. Physics and Auto-Destruction**
```csharp
if (addPhysics)
{
    Rigidbody rb = spawnedObject.AddComponent<Rigidbody>();
    
    Vector3 randomForce = new Vector3(
        Random.Range(-3f, 3f),
        Random.Range(0f, 2f),
        Random.Range(-3f, 3f)
    );
    rb.AddForce(randomForce, ForceMode.Impulse);
}

Destroy(spawnedObject, despawnTime);
```

**Explanation:**

**Runtime Rigidbody Addition:**
- `AddComponent<Rigidbody>()` adds physics at runtime
- Template objects don't have Rigidbody (lightweight)
- Only spawned instances get physics
- Memory efficient

**Initial Force:**
- Adds horizontal randomness (X and Z)
- Small upward force (Y: 0 to 2)
- Creates interesting flight paths
- Prevents objects from just dropping straight down

**Auto-Cleanup:**
- `Destroy(object, time)` schedules destruction
- After 10 seconds, object is removed
- Prevents unlimited object accumulation
- Maintains performance

---

### 7. UIManager

**File:** `UIManager.cs`  
**Attached to:** UIManager GameObject

#### Purpose
Manages the instructions UI and provides toggle functionality.

#### How It Works

**A. Auto-Find System**
```csharp
void SetupUIReferences()
{
    if (welcomeText == null)
    {
        GameObject welcomeObj = GameObject.Find("WelcomeText");
        if (welcomeObj != null)
        {
            welcomeText = welcomeObj.GetComponent<Text>();
        }
    }
}
```

**Explanation:**

**Why Auto-Find?**
- Reduces manual setup in Inspector
- Script works out-of-the-box
- Follows convention-over-configuration principle
- Still allows manual override

**GameObject.Find() Process:**
- Searches entire scene hierarchy by name
- Returns first GameObject with matching name
- Returns null if not found
- Relatively slow, so only done once in Start()

**Component Retrieval:**
- `GetComponent<Text>()` finds Text component on GameObject
- Returns null if component doesn't exist
- Type-safe retrieval

**B. Toggle System**
```csharp
void Update()
{
    bool togglePressed = false;
    
    if (Keyboard.current != null)
    {
        togglePressed = Keyboard.current.hKey.wasPressedThisFrame;
    }
    else
    {
        togglePressed = Input.GetKeyDown(toggleKey);
    }
    
    if (togglePressed)
    {
        ToggleInstructions();
    }
}

public void ToggleInstructions()
{
    instructionsVisible = !instructionsVisible;
    instructionsPanel.SetActive(instructionsVisible);
    UpdateToggleHint();
}
```

**Explanation:**

**Input System Compatibility:**
- First tries new Input System (`Keyboard.current`)
- Falls back to legacy Input Manager
- `Keyboard.current != null` checks if new system is available
- Ensures compatibility with both systems

**Boolean Toggle:**
- `instructionsVisible = !instructionsVisible` flips boolean
- true → false, false → true
- Simple state management

**SetActive() Behavior:**
- `SetActive(false)` disables GameObject
- GameObject still exists but doesn't render or update
- All child GameObjects also disabled
- Efficient way to hide/show UI

**C. Dynamic Text Generation**
```csharp
string BuildInstructionsText()
{
    return @"<b>HOW TO INTERACT:</b>

<b> Moving Cube Trail</b>
• WASD or Arrow Keys - Move the cube
• Watch the colorful trail appear!
...";
}
```

**Explanation:**

**Verbatim String (@):**
- `@` before string allows multi-line strings
- Preserves formatting and line breaks
- No need to use `\n` for new lines

**Rich Text Markup:**
- `<b>...</b>` creates bold text
- Requires "Rich Text" enabled on Text component
- Unity parses HTML-like tags
- Also supports `<i>`, `<size>`, `<color>` tags

---

## System Architecture

### Component Interaction Diagram

```
┌─────────────────────────────────────────────────┐
│                    Scene                        │
├─────────────────────────────────────────────────┤
│                                                 │
│  ┌──────────────┐         ┌──────────────┐      │
│  │ PlayerCube   │────────>│ CameraFollow │      │
│  │ - Movement   │  target │ - Tracking   │      │
│  │ - Spawning   │         └──────────────┘      │
│  └──────┬───────┘                               │
│         │ spawns                                │
│         ↓                                       │
│  ┌──────────────┐                               │
│  │ TrailSphere  │ (many instances)              │
│  │ - ColorSync  │                               │
│  │ - Lifetime   │                               │
│  └──────────────┘                               │
│                                                 │
│  ┌──────────────┐         ┌──────────────┐      │
│  │ClickableSphere│<───────│    Mouse     │      │
│  │ - Raycast    │ clicks  │   Input      │      │
│  │ - Animation  │         └──────────────┘      │
│  └──────────────┘                               │
│                                                 │
│  ┌──────────────┐         ┌──────────────┐      │
│  │ BouncingBall │<───────>│   Physics    │      │
│  │ - Forces     │ reacts  │   System     │      │
│  │ - Collisions │         └──────────────┘      │
│  └──────────────┘                               │
│                                                 │
│  ┌──────────────┐         ┌──────────────┐      │
│  │   UIButton   │────────>│ObjectSpawner │      │
│  │   onClick    │ triggers│ - Instantiate│      │
│  └──────────────┘         └──────┬───────┘      │
│                                  │ spawns       │
│                                  ↓              │
│                            ┌──────────────┐     │
│                            │Random Objects│     │
│                            │  - Physics   │     │
│                            └──────────────┘     │
│                                                 │
│  ┌──────────────┐         ┌──────────────┐      │
│  │  UIManager   │────────>│Instructions  │      │
│  │ - Toggle     │ updates │    Panel     │      │
│  └──────────────┘         └──────────────┘      │
└─────────────────────────────────────────────────┘
```

### Data Flow

**Input Flow:**
```
Keyboard/Mouse Input
    ↓
Unity Input System
    ↓
Script Update() Method
    ↓
Transform/Rigidbody Changes
    ↓
Visual Result
```

**Frame Update Cycle:**
```
1. Update() - Input reading, logic updates
2. FixedUpdate() - Physics calculations (not used in our scripts)
3. LateUpdate() - Camera following
4. Render - Unity draws scene
```

---

## Controls

| Input | Action | Element |
|-------|--------|---------|
| **W** / **↑** | Move Forward | Player Cube |
| **S** / **↓** | Move Backward | Player Cube |
| **A** / **←** | Move Left | Player Cube |
| **D** / **→** | Move Right | Player Cube |
| **Left Click** | Change Color | Clickable Spheres |
| **Spacebar** | Extra Bounce | Bouncing Ball |
| **E** | Spawn Object | Object Spawner |
| **H** | Toggle Instructions | UI Manager |
| **Click Button** | Spawn Object | Object Spawner |

---

## Technical Details

### Performance Considerations

**Trail System Optimization:**
- Automatic cleanup via `Destroy(object, time)`
- Maximum ~50 spheres exist simultaneously (5 seconds / 0.1 spawn interval)
- Each sphere is simple primitive with single material
- Low polygon count (20 faces per sphere)

**Memory Management:**
- All spawned objects destroyed automatically
- Material instances created per object (no shared state issues)
- No memory leaks from infinite spawning

**Physics Performance:**
- Only 2 objects with Rigidbody (ball + spawned objects)
- Simple colliders (sphere, box)
- No complex mesh colliders
- Physics calculations minimal

### Design Patterns Used

1. **Component Pattern**
   - Each script is a self-contained component
   - Attached to GameObjects
   - Unity's ECS-like architecture

2. **Observer Pattern**
   - UI Button onClick events
   - Event-driven architecture

3. **Template Method Pattern**
   - Runtime prefab creation
   - Instantiate from templates

4. **State Machine Pattern**
   - Animation states (isAnimating flag)
   - UI visibility states

5. **Singleton Pattern**
   - Camera.main (Unity built-in)
   - Single instance access

### Color Theory Implementation

**HSV Color Space:**
- **Hue (H):** 0-360° on color wheel, normalized to 0-1
- **Saturation (S):** 0-1, color intensity
- **Value (V):** 0-1, brightness

**Why HSV over RGB?**
- Easy to cycle through colors (just change H)
- Consistent saturation and brightness
- Intuitive color relationships
- Smooth rainbow transitions

**Color Synchronization:**
- Master color stored in PlayerCubeController
- TrailSphere components read color each frame
- No message passing needed
- Direct reference communication

---

## 🎨 Customization

### Adjusting Trail Behavior

```csharp
// In PlayerCubeController Inspector:
Trail Spawn Interval: 0.1    // Lower = denser trail
Sphere Lifetime: 5           // Higher = longer trail
Sync Colors: true/false      // Toggle color behavior
Color Change Speed: 1        // Higher = faster color cycling
```

### Modifying Camera Feel

```csharp
// In CameraFollow Inspector:
Offset: (0, 5, -10)         // Change camera position
Smooth Speed: 0.125         // Lower = more cinematic lag
Look At Target: true/false  // Toggle rotation following
```

### Tweaking Physics

```csharp
// In BouncingBall Inspector (Rigidbody):
Mass: 1                     // Higher = harder to move
Drag: 0                     // Air resistance
Angular Drag: 0.05          // Rotation resistance

// In Script:
Bounce Force: 10            // Higher = bigger jumps
Bounciness: 0.9             // 0-1, energy retention
```

### Spawn System Tuning

```csharp
// In ObjectSpawner Inspector:
Spawn Height: 5             // How high objects spawn
Spawn Radius: 3             // Spread around player
Despawn Time: 10            // Lifetime in seconds
Random Color: true          // Colored or default
Add Physics: true           // Enable falling
Random Rotation: true       // Tumbling effect
```

---

## 🎓 Extending the Project

### Suggested Enhancements

1. **Particle Effects**
   - Add ParticleSystem to trail
   - Explosion on sphere click
   - Bounce impact particles

2. **Audio**
   - Movement sounds
   - Click feedback
   - Bounce sound effects
   - Background music

3. **Score System**
   - Count clicks
   - Track distance traveled
   - Sphere collection mechanic

4. **Multiple Levels**
   - Scene management
   - Save/load system
   - Difficulty progression

5. **Polish**
   - Screen shake on impacts
   - Motion blur
   - Post-processing effects
   - Smooth UI transitions