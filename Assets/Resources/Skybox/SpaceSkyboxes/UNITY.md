# Using the skies in Unity

There is no .unitypackage in this pack. You import plain images, so it works in any Unity version
(Built-in, URP, HDRP) with the steps below.

> Status: checked in Unity 6000.0.84f1 (Built-in and URP): the six faces line up with no seams,
> the horizon is level and the sky is upright. The mapping was worked out from Unity's own
> documentation and the built-in `Skybox-Panoramic.shader` source and is also checked numerically
> (cube faces vs. panorama, seams between faces). The HDRP steps follow Unity's documentation and
> were not checked in the editor. The troubleshooting section says what to do if anything is off.

## Option A (recommended): Skybox/6 Sided

Files: `cubemaps_unity/<sky>/Front.png, Back.png, Left.png, Right.png, Up.png, Down.png` (512 x 512 each in this sampler; 1024 x 1024 in the paid pack).

Unity's manual defines the six slots by world axis:

| Slot  | World direction | File          |
|-------|-----------------|---------------|
| Front | +Z              | `Front.png`   |
| Back  | -Z              | `Back.png`    |
| Left  | +X              | `Left.png`    |
| Right | -X              | `Right.png`   |
| Up    | +Y              | `Up.png`      |
| Down  | -Y              | `Down.png`    |

Note that Unity's "Left" slot is the +X face, which is to the *right* of the default camera. The
files are named after the slot they go into, so just put each file into the slot with the same name.
All six images are upright "inside view" images (the same layout as an unfolded cross: the bottom edge of
`Up` touches the top edge of `Front`).

Steps:

1. Copy the six PNGs of one sky into your project, e.g. `Assets/Skies/TealMagenta/`.
2. Select all six in the Project window and set in the Inspector:
   * Texture Type: Default, Texture Shape: 2D
   * sRGB (Color Texture): on
   * **Wrap Mode: Clamp** (this prevents thin seam lines at the cube edges)
   * Generate Mip Maps: off, Filter Mode: Bilinear
   * Max Size: 512 (or higher), Compression: None or High Quality (low-quality compression causes banding in dark gradients)
3. Create a material: Assets > Create > Material, shader `Skybox/6 Sided`. Drag each file into the slot of the same name.
4. Window > Rendering > Lighting > Environment: set Skybox Material to your new material. For lighting from the sky set
   Environment Lighting > Source to Skybox, then Generate Lighting (or enable auto-generate).
5. In URP the same material works. HDRP does not use skybox materials: use a Cubemap with the HDRI Sky override instead (import the six images as a Cubemap in the 2D/Cube texture workflow).

If a face is rotated or two sides are swapped, see Troubleshooting.

## Option B: Skybox/Panoramic

File: `unity_panoramas/<sky>.png` (2048 x 1024 in this sampler; the paid pack has 4096 x 2048 JPGs).

These panoramas are *not* the same image as the ones in `panoramas_2048/` or the Godot addon: they are
mirrored and rolled so that, under Unity's `Skybox/Panoramic` shader (which reads `u = 0.5 - atan2(z, x) / 2pi`),
the sky matches the 6-sided set exactly.

1. Import with Texture Shape 2D, Wrap Mode Repeat, **Generate Mip Maps off** (mip maps cause a faint line at the left/right seam of lat-long images), Filter Bilinear, Compression None/High Quality, Max Size 4096.
2. Material shader `Skybox/Panoramic`, Mapping: Latitude Longitude Layout, Image Type: 360 Degrees. Assign the image to Spherical (HDR).
3. Assign the material as the scene skybox as in step 4 above.

## Troubleshooting

* Left/right look swapped across the seam between Front and Left/Right: swap the `Left.png` and `Right.png` slots.
* `Up` or `Down` looks rotated: rotate that single image by 90 degrees (and re-check the seam with Front).
* A thin line at the cube edges: set Wrap Mode to Clamp on all six textures.
* Sky looks too dark/bright: change Exposure on the material. The images are ordinary 8-bit sRGB (LDR), not HDR.
