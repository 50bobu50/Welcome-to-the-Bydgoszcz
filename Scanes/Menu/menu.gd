extends Container

func _on_Play_pressed():
	# warning-ignore:return_value_discarded
	get_tree().change_scene("res://Scanes/Server/networkconnection.tscn")

func _on_Options_pressed():
	var settings = load("res://Scanes/Menu/Settings/Settings.tscn").instance()
	get_node("/root/").add_child(settings)

func _on_Exit_pressed():
	get_tree().quit();
