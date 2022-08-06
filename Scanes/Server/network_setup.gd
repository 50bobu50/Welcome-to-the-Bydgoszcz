extends Control

func _ready():
	$"../LOBBY".hide()
	show()

func _on_IP_text_changed(new_text):
	Network.IP_ADRESS = new_text
	print(new_text)

func _on_HOST_pressed():
	Network.create_server()
	$"../LOBBY".show()
	hide()


func _on_JOIN_pressed():
	Network.join_server()
	$"../LOBBY".show()
	hide()
