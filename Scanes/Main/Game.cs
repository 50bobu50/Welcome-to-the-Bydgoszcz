using Godot;
using System;

public class Game : Node
{
	Label Collected;
	Label Max;
	public int PointsCollected=0;
	private int PointsMax;
	public override void _Ready()
	{
		PointsMax = GetNode("MetaHolder").GetChildCount();
		Max = (Label)GetNode("UI/Max");
		Collected = (Label)GetNode("UI/Collected");
		Collected.Text = PointsCollected.ToString();
		Max.Text = PointsMax.ToString();
	}
	
	public void ShowPoints()
	{
		Collected.Text = PointsCollected.ToString();
	}
}
