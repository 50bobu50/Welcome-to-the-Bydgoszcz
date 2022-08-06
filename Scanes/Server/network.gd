extends Node

#server config
var IP_ADRESS = "127.0.0.1"
var SERVER_PORT = 27960
var MAX_PLAYERS  = 5
#server setup
var server
var client 
var players = {}
var info = {name = "name"}

func _ready():
	#debug setup
	get_tree().connect("connected_to_server", self, "_connected_ok")
	get_tree().connect("server_disconnected", self, "_server_disconnected")
	get_tree().connect("connection_failed", self, "_connected_fail")
	get_tree().connect("network_peer_connected", self ,"_player_connected")
	get_tree().connect("network_peer_connected", self ,"_player_disconnected")

func create_server():
	print("CREATING SERVER")
	server = NetworkedMultiplayerENet.new()
	server.create_server(SERVER_PORT, MAX_PLAYERS)
	get_tree().network_peer = server
	players[1] = info
	print(players)



func join_server():
	print("JOINING SERVER")
	client = NetworkedMultiplayerENet.new()
	client.create_client(IP_ADRESS, SERVER_PORT)
	get_tree().network_peer = client

func reset_networking_connection():
	if get_tree().has_network_peer():
		get_tree().network.peer = null

#Debug function
func _connected_ok():
	print("Seuccesfully connected to the server")

func _server_disconnected():
	print("Disconnected from to the server")
	reset_networking_connection()

func _connected_fail():
	print("connection to the server failed")
	reset_networking_connection()
#player
func _player_connected(id):
	print("PLAYER: %s" % id)
	rpc_id(id, "register_player", info)
	rpc("test","test")
	
func _player_disconnected(id):
	players.erase(id)

remote func register_player(info):
	var id = get_tree().get_rpc_sender_id()
	players[id] = info
	print(players)
	
remote func test(test):
	print(test)

