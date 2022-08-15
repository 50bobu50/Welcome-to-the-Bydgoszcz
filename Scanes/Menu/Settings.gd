extends Control

func _on_Button_pressed():
	get_parent().remove_child(self)
