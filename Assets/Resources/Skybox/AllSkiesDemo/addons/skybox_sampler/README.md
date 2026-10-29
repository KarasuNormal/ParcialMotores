# Skybox Sampler - 9 free skies for Godot 4

Nine complete, seamless skyboxes as ready-to-use Godot 4 resources: three space skies (Teal & Magenta Nebula,
Ringed Gas Giant, Supernova Remnant), three outdoor skies (Golden Hour, Thunderstorm, Moonlit Night) and three
alien-world skies (Ringed Giant over Ocean, Volcanic Ash World, Tidally-Locked Terminator).
Each sky is a 2048 x 1024 equirectangular panorama with a PanoramaSkyMaterial and an Environment that uses it
for the background, ambient light and reflections. No seams, no pinching at the poles.

## Install

Copy `addons/skybox_sampler/` into your project's `addons/` folder (or install from the Asset Store). It is a
plain asset pack: there is no plugin to enable.

## Use

* Add a `WorldEnvironment` node and set its Environment to one of
  `res://addons/skybox_sampler/environments/<sky>.tres`.
* Or build your own Environment: set Background to Sky and use `res://addons/skybox_sampler/skies/<sky>.tres`
  as the Sky material.
* Demo: open `res://addons/skybox_sampler/demo/demo.tscn` and run it. A small test scene (floor, a mirror ball, a
  rough ball and a few blocks) shows how each sky lights and reflects on real geometry; the camera orbits slowly.
  Press Left/Right (or A/D) to cycle through the nine skies.

Checked in Godot 4.5: headless import, every material and Environment loads, and the demo scene renders with
the Compatibility renderer. Made for Godot 4.5.

## The skies

| File | Name | From |
|------|------|------|
| `01_teal_magenta_nebula` | Teal & Magenta Nebula | Space Skyboxes: 20 Stylized Skies |
| `09_ringed_gas_giant` | Ringed Gas Giant | Space Skyboxes: 20 Stylized Skies |
| `14_supernova_remnant` | Supernova Remnant | Space Skyboxes: 20 Stylized Skies |
| `02_golden_hour` | Golden Hour | Stylized Sky Skyboxes: 16 Day, Sunset, Night and Storm Skies |
| `08_thunderstorm` | Thunderstorm | Stylized Sky Skyboxes: 16 Day, Sunset, Night and Storm Skies |
| `12_moonlit_night` | Moonlit Night | Stylized Sky Skyboxes: 16 Day, Sunset, Night and Storm Skies |
| `02_ringed_giant_ocean` | Ringed Giant over Ocean | Alien Worlds Skyboxes: 16 Exoplanet Skies |
| `06_volcanic_ash_world` | Volcanic Ash World | Alien Worlds Skyboxes: 16 Exoplanet Skies |
| `12_tidally_locked_twilight` | Tidally-Locked Terminator | Alien Worlds Skyboxes: 16 Exoplanet Skies |

## More skies

Three skies come from each of three paid packs by the same publisher on itch.io, which add the remaining
skies at 4096 x 2048 with 1024 px cube faces and Unity-ready cubemaps:

* Space Skyboxes: 20 Stylized Skies - https://the-knightshift.itch.io/space-skyboxes
* Stylized Sky Skyboxes: 16 Day, Sunset, Night and Storm Skies - https://the-knightshift.itch.io/stylized-skyboxes
* Alien Worlds Skyboxes: 16 Exoplanet Skies - https://the-knightshift.itch.io/alien-skyboxes
* All 52 skies in one bundle - https://itch.io/s/203326/skybox-bundle-52-skies-for-godot-4-and-unity

Want your own skies? They were made with SkyGen, a Godot 4 sky generator:
https://the-knightshift.itch.io/skygen (free LITE: https://store.godotengine.org/asset/the-knightshift/skygen-lite/)

## License

Free for commercial and non-commercial use, no attribution required; not for resale or redistribution as assets.
Full text in LICENSE.md.

## AI disclosure

Made with AI assistance: the skies are painted by procedural code written with AI help, then checked by eye and by
numeric seam checks. No AI image generator was used; every pixel comes from the code.
