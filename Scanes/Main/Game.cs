using Godot;
using System;

public class Game : Node
{
	Label Collected;
	Label Max;
	CanvasItem PickUpText;
	public Saul SaulCharacter;
	public int PointsCollected=0;
	private int PointsMax;
	private AnimationPlayer Gate;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		PointsMax = GetNode("MetaHolder").GetChildCount();
		Max = (Label)GetNode("UI/Max");
		Collected = (Label)GetNode("UI/Collected");
		Collected.Text = PointsCollected.ToString();
		Max.Text = PointsMax.ToString();
		Gate = (AnimationPlayer)GetNode("Navigation/NavigationMeshInstance/Gate/AnimationPlayer");
		PickUpText = (CanvasItem)GetNode("UI/PickUpText");
		SaulCharacter = GetNode<Saul>("Saul");
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
		SaulCharacter.speed *= 1.25f;
	} 
	private void CollectedEveryPoint()
	{
		Gate.Play("Cube001Action");
		Gate.Play("CubeAction001");
		(PickUpText as Label).Text = "GATE IS OPEN";	
	}
	public void _on_Timer_timeout()
	{
		PickUpText.Visible = false;
	}
	public void _on_Area_body_entered(object body)
	{
		Node bodyNode = body as Node;
		if(bodyNode.GetParent().Name=="Players")
		{
			EscapePlan();
		}
	}
	void EscapePlan()
	{
		Rpc("PlayerEscaped");
		(GetNode("UI") as CanvasItem).Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		foreach(Node child in GetTree().GetRoot().GetChildren())
		{
			if(child.Name!="Network")
			{
				child.QueueFree();
			}
		}
		GetTree().ChangeScene("res://Scanes/Lobby/LOBBY.tscn");
	}
	[Sync]
	public void PlayerEscaped()
	{
		int id = GetTree().GetRpcSenderId();
		GetTree().CallGroup("Lobby","player_escaped",id);
		string path = "Players/"+id;
		GetNode(path).QueueFree();
	}
}
