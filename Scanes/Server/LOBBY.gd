extends Container

var item_list
var status_list

func _ready():
	update_lobby()
	check()
	
remote func check():
	if(Network.players.empty()):
		return
	if(Network.players[get_tree().get_network_unique_id()]["status"] == "Died"):
		$VideoPlayer.visible = true
		$VideoPlayer.play()
		yield(get_tree().create_timer(7.0), "timeout")
		$VideoPlayer.visible = false

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
	Network.players[id]["status"] = "Escaped"

func player_died(id):
	Network.players[id]["status"] = "Died"

func death():
	print("aaa");
	$VideoPlayer.visible = true
	$VideoPlayer.play()
	
