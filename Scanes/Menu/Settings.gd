extends Control

func _on_Button_pressed():
	visible = false
	if($"..".name != "Container"):
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
