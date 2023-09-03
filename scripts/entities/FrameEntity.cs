using Godot;
using System;

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

	public int hitTaunt;
	public int hitJump;
	public int hitDefense;
	public int hitAttack;
	public int hitJumpDefense;
	public int hitDefensePower;
	public int hitDefenseAttack;

	public BdyKindEnum kind;
	public float x_body;
	public float y_body;
	public float z_body;
	public float h_body;
	public float w_body;
	public float zwidth_body;

	public override string ToString()
	{
		return string.Format("id={0}, name={1}, state={2}, wait={3}, textureIndex={4}, pic={5}, x_offset={6}, y_offset={7}, next={8}, dvx={9}, dvy={10}, dvz={11}, hitTaunt={12}, hitJump={13}, hitDefense={14}, hitAttack={15}, hitJumpDefense={16}, hitDefensePower={17}, hitDefenseAttack={18}, kind={19}, x_body={20}, y_body={21}, z_body={22}, h_body={23}, w_body={24}, zwidth_body={25}",
		 id, name, state, wait, textureIndex, pic, x_offset, y_offset, next, dvx, dvy, dvz, hitTaunt, hitJump, hitDefense,
		 hitAttack, hitJumpDefense, hitDefensePower, hitDefenseAttack, kind, x_body, y_body, z_body, h_body, w_body, zwidth_body);
	}
}
