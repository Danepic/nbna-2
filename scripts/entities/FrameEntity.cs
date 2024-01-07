using Godot;
using System;
using System.Linq;
using System.Text;

public class FrameEntity
{
	public int id;
	public string name;
	public StateFrameEnum state;
	public float wait;

	public int textureIndex;
	public int pic;
	public float x_offset;
	public float y_offset;

	public int next;

	public float dvx;
	public float dvy;
	public float dvz;

	public Nullable<int> hitTaunt;
	public Nullable<int> hitJump;
	public Nullable<int> hitSuperPower;
	public Nullable<int> hitDefense;
	public Nullable<int> hitAttack;
	public Nullable<int> hitPower;
	public Nullable<int> hitJumpDefense;
	public Nullable<int> hitDefensePower;
	public Nullable<int> hitDefenseAttack;
	public Nullable<int> holdForwardAfter;
	public Nullable<int> holdDefenseAfter;
	public Nullable<int> holdPowerAfter;

	public BdyKindEnum kind;
	public float x_body;
	public float y_body;
	public float z_body;
	public float h_body;
	public float w_body;
	public float zwidth_body;

	public override string ToString()
	{
		return Newtonsoft.Json.JsonConvert.SerializeObject(this);
	}
}
