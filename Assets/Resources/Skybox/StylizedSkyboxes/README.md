# StylizedSkyboxes - 3 free sample skies (Skybox Sampler)

These 3 skies are the free sample of Stylized Sky Skyboxes: 16 Day, Sunset, Night and Storm Skies: https://the-knightshift.itch.io/stylized-skyboxes
The paid pack has every sky at 4096 x 2048 with 1024 px cube faces; this sampler has them at the sizes below.

| File stem | Sky |
|---|---|
| `02_golden_hour` | Golden Hour |
| `08_thunderstorm` | Thunderstorm |
| `12_moonlit_night` | Moonlit Night |

## Files

- `panoramas_2048/<sky>.png`: equirectangular panorama, 2048 x 1024 PNG.
- `cubemaps_godot/<sky>/px, nx, py, ny, pz, nz .png`: 512 x 512 cube faces, Godot names.
- `cubemaps_unity/<sky>/Front, Back, Left, Right, Up, Down .png`: 512 x 512 cube faces for Unity's Skybox/6 Sided.
- `unity_panoramas/<sky>.png`: 2048 x 1024 panorama laid out for Unity's Skybox/Panoramic.
- `godot/`: a Godot 4 project. `godot/addons/stylized_skyboxes/` has a PanoramaSkyMaterial (`skies/`) and an Environment
  (`environments/`) per sky; `godot/demo.tscn` cycles the 3 skies.
- `UNITY.md`: Unity import steps. `LICENSE.txt`: the license.

## Godot 4

Copy `godot/addons/stylized_skyboxes/` into your project's `addons/` folder, then set a WorldEnvironment's Environment to
`environments/<sky>.tres`. The sky is the background and also gives ambient light and reflections.

Made with AI assistance: the skies are painted by procedural code written with AI help, then checked by eye and by
numeric seam checks. No AI image generator was used.
