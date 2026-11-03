extends Node3D
## Demo: slowly rotating camera; press Left/Right (or A/D) to cycle through the skies.
const ENVS := [
	"res://addons/alien_skyboxes/environments/02_ringed_giant_ocean.tres",
	"res://addons/alien_skyboxes/environments/06_volcanic_ash_world.tres",
	"res://addons/alien_skyboxes/environments/12_tidally_locked_twilight.tres"
]
const NAMES := [
	"Ringed Giant over Ocean",
	"Volcanic Ash World",
	"Tidally-Locked Terminator"
]
@export var rotate_speed := 0.06
var idx := 0
@onready var we: WorldEnvironment = $WorldEnvironment
@onready var cam: Camera3D = $Camera3D
@onready var label: Label = $UI/Label

func _ready() -> void:
	select(0)

func select(i: int) -> void:
	idx = posmod(i, ENVS.size())
	we.environment = load(ENVS[idx])
	label.text = "%s   (%d/%d)   Left/Right: change sky" % [NAMES[idx], idx + 1, ENVS.size()]

func _process(delta: float) -> void:
	cam.rotate_y(rotate_speed * delta)

func _unhandled_input(e: InputEvent) -> void:
	if e.is_action_pressed("ui_right"):
		select(idx + 1)
	elif e.is_action_pressed("ui_left"):
		select(idx - 1)
