extends Control

func _ready():
	$"Container/TabContainer/Settings/FOV/Label".text = str(Save.gameSettings["fov"])
	$"Container/TabContainer/Settings/FOV/HSlider".value = Save.gameSettings["fov"]

func _on_HSlider_value_changed(value):
	Save.gameSettings["fov"] = value
	$"Container/TabContainer/Settings/FOV/Label".text = str(value)


func _on_Button_pressed():
	get_parent().remove_child(self)


func _on_OptionButton_item_selected(index):
	var x = 0
	var y = 0
	match index:
		0:
			x = 640
			y = 360
		1:
			x = 1280
			y = 720
		2:
			x = 1920
			y = 1080
	get_tree().set_screen_stretch(SceneTree.STRETCH_MODE_VIEWPORT,  SceneTree.STRETCH_ASPECT_KEEP, Vector2(x,y),2)


func _on_CheckButton_toggled(button_pressed):
	OS.window_fullscreen = button_pressed
