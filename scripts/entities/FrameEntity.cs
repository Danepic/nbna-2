using Godot;
using System;

public class FrameEntity
{
    public int id;
    public string name;
    public StateFrameEnum state;
    public float wait;

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
}
