# Craftopoly
<img src="Screens/screen03.jpg" width=800px></img>

## Note
[**Go to my C# scripts**](Assets/Scripts)

## Game settings
### Camera
Perspective top-down, following Player position, fixed rotation.
### World
Flat, empty terrain with some test objects.
### Player
Simple character with horizontal movement (WSAD or LStick).


## Implemented features
### Player and NPC
- gathering items from sources (trees, rocks)
- carrying items (wood, stone, ore, tools, crates)
- crafting items (boards, tools, crates)
- <div>skills affecting speed of different actions
<img src="Screens/anim01.gif" width=600px></div>

### Player
- consuming (crates ;) )
- attaching tool to belt

### Player (UI)
<img src="Screens/anim02.gif" width=600px></img>
- <div>assigning NPCs to workplaces (structures)
<img src="Screens/screen05.jpg" width=600px></div>
- <div>setting crafting items and counts
<img src="Screens/screen04.jpg" width=600px></div>
- <div>viewing storage items
<img src="Screens/screen06.jpg" width=600px></div>
- selling/buying
- creating new plots for structures


### NPC
- executing tasks provided by assigned workplace (behavior trees with [Behavior Designer](https://assetstore.unity.com/packages/tools/visual-scripting/behavior-designer-behavior-trees-for-everyone-15277) )
<img src="Screens/screen02.jpg" width=600px></img>

### Other
- keyboard + mouse / controller support
- input hints
- simple localizations
- notifications about missing items needed for crafting
