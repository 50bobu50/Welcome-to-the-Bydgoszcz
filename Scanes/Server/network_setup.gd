extends Control

func _ready():
	$"../LOBBY".hide()
	show()

func _on_IP_text_changed(new_text):
	Network.IP_ADRESS = new_text
	print(new_text)

func _on_HOST_pressed():
	$"../LOBBY".show()
	Network.create_server()
	hide()


func _on_JOIN_pressed():
	$"../LOBBY".show()
	Network.join_server()
	hide()
