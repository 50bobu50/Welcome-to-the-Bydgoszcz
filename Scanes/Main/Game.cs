using Godot;
using System;

public class Game : Node
{
	Label Collected;
	Label Max;
	CanvasItem PickUpText;
	public int PointsCollected=0;
	private int PointsMax;
	private AnimationPlayer Gate;
	public override void _Ready()
	{
		PointsMax = GetNode("MetaHolder").GetChildCount();
		Max = (Label)GetNode("UI/Max");
		Collected = (Label)GetNode("UI/Collected");
		Collected.Text = PointsCollected.ToString();
		Max.Text = PointsMax.ToString();
		Gate = (AnimationPlayer)GetNode("Navigation/NavigationMeshInstance/Gate/AnimationPlayer");
		PickUpText = (CanvasItem)GetNode("UI/PickUpText");
	}
	
	public void ShowPoints()
	{
		Collected.Text = PointsCollected.ToString();
		PickUpText.Visible = true;
		(PickUpText.GetNode("Timer") as Timer).Start();
		if(PointsCollected==PointsMax)
		{
			CollectedEveryPoint();
		}
	} 
	private void CollectedEveryPoint()
	{
		Gate.Play("Cube001Action");
		Gate.Play("CubeAction001");
	}
	public void _on_Timer_timeout()
	{
		PickUpText.Visible = false;
	}
}

