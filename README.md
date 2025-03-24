
# World Builder – Unity 2D Component Package
### A streamlined grid-based drag-and-drop system for building rich 2D environments in Unity.

<br>

What it can do:
- Effortlessly create and design tile-based worlds with an intuitive snapping and placement system.
- Import your own images or use the included demo assets to craft custom tiles and world elements.
- Seamlessly drag, drop, and snap tiles into place. Perfect for prototyping or final-level design.
- Customize tile behaviors with parameters like collision toggling, moveable vs. fixed positioning, and more.

### Whether you're building a top-down RPG, platformer, or puzzle game, World Builder gives you the tools to bring your 2D environments to life—quickly, cleanly, and creatively.

<br><br><br>

# Quick Start Guide
 Build 2D worlds effortlessly with this grid-based drag-and-drop system for Unity.
 


## Installation
1. Download or import the World Builder package into your Unity project.

2. Ensure your project is set up for 2D mode.

##
## Setting Up
#### 1. Add the World Builder Component

- Create an empty GameObject in your scene.

- Attach the WorldBuilder script/component to it.

#### 2. Set Grid Size

- In the inspector, define your grid cell size (e.g., 32x32 or 64x64 pixels).

#### 3. Import Your Tiles

- Upload your own images (sprites) to Unity.

- Drag them into the World Builder interface to register them as tiles.

- Or use the included demo tiles to get started.

##
## Placing Tiles
#### 1. Open the World Builder UI

- A custom editor window or inspector panel should appear with tile options.

#### 2. Select a Tile

- Click on any registered tile image to select it.

#### 3. Drag and Drop

- Click and drag into the scene view to place the tile.

- Tiles will snap to the grid automatically.

##
## Tile Settings
You can configure properties for each tile:

- Collision

    - Enable or disable collider generation.

- Position Lock

    - Set tile as moveable or fixed.

- Custom Tags/Metadata

    - Add custom labels for use in gameplay logic (optional).
    
##
## Editing Tiles
- Select a tile in the scene to modify its settings in the inspector.

- Right-click (or designated shortcut) to remove a tile.

- Use Undo (Ctrl+Z) if needed.


##
## Tips
- Organize tiles into folders (e.g., Ground, Props, Walls) for easier access.

- Combine with Unity’s sorting layers and z-depth for parallax or visual depth.

- Use prefab variants if you want reusable interactive tiles (like doors or switches).
