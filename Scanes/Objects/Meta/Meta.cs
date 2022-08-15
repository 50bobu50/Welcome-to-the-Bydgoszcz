using Godot;
using System;

public class Meta : Spatial
{
	private Transform RestPose;
	private Sprite3D Sprite;
	public override void _Ready()
	{
		Sprite = GetNode<Sprite3D>("MetaSprite");
		RestPose = Sprite.Transform;
	}

	public override void _PhysicsProcess(float delta)
	{
		Transform Pozycja = Sprite.Transform;
		Pozycja.origin = RestPose.origin + new Vector3(0,(Mathf.Cos(Time.GetTicksMsec() * delta * .25f)) * 0.25f,0);
		Pozycja.basis = Pozycja.basis.Rotated(Vector3.Up,delta);
		Pozycja.origin = RestPose.origin + new Vector3(0,(Mathf.Cos(Time.GetTicksMsec() * delta * .5f)) * 0.5f,0);
		//GD.Print(Pozycja.origin);
		Sprite.Transform = Pozycja;
	}
	public void PickUp()
	{
		Rpc("PickUpSnyc");
	}
	[Sync]
	public void PickUpSnyc()
	{
		(this.GetParent().GetParent() as Game).PointsCollected++;
		(this.GetParent().GetParent() as Game).ShowPoints();
		QueueFree();
	}
}	
