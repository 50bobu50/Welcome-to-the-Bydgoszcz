extends Container

var item_list;

func update_lobby(players):
	item_list = $ItemList
	item_list.clear()
	for id in players:
		var line = "{id},{info}".format({"id":id,"info":players[id]})
		item_list.add_item(line)


func _on_CheckBox_toggled(button_pressed):
	Network.set_ready(button_pressed)
