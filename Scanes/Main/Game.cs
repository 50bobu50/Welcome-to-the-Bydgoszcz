using Godot;
using System;

public class Game : Node
{
	public int PointsLeft;
	public override void _Ready()
	{
		
		Node MetaHolder = GetNode<Node>("MetaHolder");
		Godot.Collections.Array XD = MetaHolder.GetChildren();
		foreach (Meta meta in XD)
		{
			PointsLeft++;
		}
		GD.Print("You have to collect "+PointsLeft+" Things");
		
	}
	//To server stuff
	public void ShowAmountOfPoints()
	{
		Rpc("ShowAmountOfPointsSync");
	}
	//Server stuff
	[Sync]
	public void ShowAmountOfPointsSync()
	{
		GD.Print("bruh");
		//Show everyone amount of points 
		//Points are global
		
		//If PointsLeft == 0 then
		//End game or something
		//Maybe add gate or exit idk
	}
}
