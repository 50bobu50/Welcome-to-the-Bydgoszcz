extends Node

#server config
var IP_ADRESS = "127.0.0.1"
var SERVER_PORT = 27960
var MAX_PLAYERS  = 5
#server setup
var server
var client 
var players = {}
var info = {"name":null, "ready":false, "status":"before game"}


func _ready():
	# warning-ignore:return_value_discarded
	get_tree().connect("connected_to_server", self, "_connected_ok")
	# warning-ignore:return_value_discarded
	get_tree().connect("server_disconnected", self, "_server_disconnected")
	# warning-ignore:return_value_discarded
	get_tree().connect("connection_failed", self, "_connected_fail")
	# warning-ignore:return_value_discarded
	get_tree().connect("network_peer_connected", self ,"_player_connected")
	# warning-ignore:return_value_discarded
	get_tree().connect("network_peer_disconnected", self ,"_player_disconnected")

func create_server():
	print("CREATING SERVER")
	server = NetworkedMultiplayerENet.new()
	server.create_server(SERVER_PORT, MAX_PLAYERS)
	get_tree().network_peer = server
	players[1] = info
	print(players)
	update_lobby()

func join_server():
	print("JOINING SERVER")
	client = NetworkedMultiplayerENet.new()
	var err = client.create_client(IP_ADRESS, SERVER_PORT)
	if(err != OK):
		print(err)
		return false
	get_tree().network_peer = client

func reset_networking_connection():
	if get_tree().has_network_peer() == false:
		get_tree().network.peer = null

func _connected_ok():
	print("Seuccesfully connected to the server")
	players[get_tree().get_network_unique_id()] = info

func _server_disconnected():
	print("Disconnected from to the server")
	get_tree().change_scene("res://Scanes/Menu/Menu.tscn");
	players = {}
	reset_networking_connection()

func _connected_fail():
	print("Connection to the server failed")
	reset_networking_connection()
#player
func _player_connected(id):
	print("PLAYER: %s CONNECTED" % id)
	rpc_id(id, "register_player", info)
	
func _player_disconnected(id):
	print("PLAYER: %s DISCONNECTED" % id)
	players.erase(id)
	rpc("update_lobby")


remote func register_player(player_info):
	var id = get_tree().get_rpc_sender_id()
	players[id] = player_info
	print(players)
	rpc("update_lobby")

sync func update_lobby():
	get_tree().call_group("Lobby","update_lobby",players)
	

remote func set_ready(status):
	var id = get_tree().get_network_unique_id()
	rpc("player_ready",id,status)
	rpc_id(1,"check_ready")

sync func player_ready(id,status):
	players[id]["ready"]=status
	update_lobby()
	
sync func check_ready():
	for i in players:
		if(players[i]["ready"]==false):
			return null
	rpc("load_map")
 
sync func load_map():
	get_tree().set_pause(true)
	if(get_node_or_null("/root/server_gui/LOBBY")==null):
		get_node("/root/LOBBY").visible = false
	elif(get_node_or_null("/root/server_gui/LOBBY")!=null):
		get_node("/root/server_gui/LOBBY").visible = false
	var world = load("res://Scanes/Main/Main.tscn").instance()
	get_tree().get_root().add_child(world)
	for i in players:
		var player = preload("res://Scanes/Objects/Player/Player.tscn").instance()
		player.set_name(str(i))
		player.set_network_master(i)
		get_node("/root/Main/Players").add_child(player)
		Network.players[i]["ready"]=false
		Network.players[i]["status"]="in game"
	if(get_tree().is_network_server()):
		players_done.append(1)
		if players_done.size() == players.size():
			post_configure_game()
	elif(get_tree().is_network_server()==false):
		rpc_id(1,"done_conf")
		
var players_done = []
remote func done_conf():
	var id = get_tree().get_rpc_sender_id()
	assert(get_tree().is_network_server())
	assert(id in players)
	assert(not id in players_done)
	players_done.append(id)
	if players_done.size() == players.size():
		rpc("post_configure_game")

sync func post_configure_game():
	if 1 == get_tree().get_rpc_sender_id():
		get_tree().set_pause(false)
