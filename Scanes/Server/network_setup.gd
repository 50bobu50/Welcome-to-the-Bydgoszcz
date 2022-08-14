extends Control

func _ready():
	$"../LOBBY".hide()
	show()

func _on_NAME_text_changed(new_text):
	Network.info["name"] = new_text

func _on_IP_text_changed(new_text):
	Network.IP_ADRESS = new_text

func _on_HOST_pressed():
	if(Network.info["name"]==null):
		return
	$"../LOBBY".show()
	Network.create_server()
	hide()


func _on_JOIN_pressed():
	if(Network.info["name"]==null):
		return
	if(Network.join_server()==false):
		return
	$"../LOBBY".show()
	hide()



