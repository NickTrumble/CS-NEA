# Procedural World Generator

My A-Level Computer Science NEA: a C# Windows Forms terrain editor that generates a 2D height map from Perlin or Simplex noise and can export the terrain as an OBJ or PLY mesh.

The editor stores terrain in chunks, supports changing generation settings, and uses undo/redo copies while editing elevation. Terrain edits are applied in parallel and update the chunk bitmaps used by the UI.

## Requirements

- Visual Studio with .NET Framework 4.7.2 targeting support

Open `NEA Procedural World Generator.sln`, build it, and run the Windows Forms project.

## Repository layout

- `NEA Procedural World Generator/Noise.cs` contains the noise generation code.
- `World.cs` owns terrain chunks and editing operations.
- `OBJExport.cs` exports mesh data.
- Forms and designer files make up the desktop UI.

This is archived coursework. The later C++ terrain generator is the active continuation of the idea.
