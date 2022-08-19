extends Container

var item_list
var status_list

func _ready():
	update_lobby()

func update_lobby():
	item_list = $ItemList
	item_list.clear()
	status_list = $Status
	status_list.clear()
	for id in Network.players:
		item_list.add_item(Network.players[id]["name"],null,false)
		status_list.add_item(Network.players[id]["status"],null,false)


func _on_CheckBox_toggled(button_pressed):
	Network.set_ready(button_pressed)

func player_escaped(id):
	print(id)
	Network.players[id]["status"] = "Escaped"
