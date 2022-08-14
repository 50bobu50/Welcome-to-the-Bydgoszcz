using Godot;
using System;

public class Save : Node
{
	const string SAVEFILE = "user://SAVEFILE.save";
	public Godot.Collections.Dictionary gameSettings = new Godot.Collections.Dictionary();
	
	public override void _Ready()
	{
		gameSettings["fullscreen"] = false;
		gameSettings["displayFps"] = false;
		gameSettings["volume"] = 1;
		gameSettings["fov"] = 70;
		LoadSettings();
	}
	void LoadSettings()
	{
		File file = new File();
		file.Open(SAVEFILE,File.ModeFlags.Read);
		//gameSettings = file.GetVar();
		file.Close();
	}
	
	public void SaveSettings()
	{
		File file = new File();
		file.Open(SAVEFILE,File.ModeFlags.Write);
		file.StoreVar(gameSettings);
		file.Close();
	}
}
