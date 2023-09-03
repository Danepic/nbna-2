using Godot;
using System;

public class FrameHelper
{
	private Color originalColor;
	private Color fadeOutColor;

	//Debug
	public bool debugFrameToGo;
	public string frameToStopForDebug = null;
	public int previousId;

	public float repeatAnimationAt;
	public bool repeatAnimationStart;
	public bool repeatAnimationReady;

	public int currentHp;
	public int currentMp;
	public bool facingRight;
	public bool facingUp;

	// Cache
	public bool isCache = false;

	// Extern Interaction
	public int summonAction;
	public bool externAction;
	// public InteractionData externItr;
	public bool enemyFacingRight;

	public bool attacked;
	public bool wasAttacked;

	// Running
	private float RUNNING_COUNT = 4f;
	private float SIDE_DASH_COUNT = 4f;

	public float runningRightCount;
	public bool runningRightEnable;
	public bool countRightEnable;

	public float runningLeftCount;
	public bool runningLeftEnable;
	public bool countLeftEnable;

	// Side Dash
	public float sideDashUpCount;
	public bool sideDashUpEnable;
	public bool countSideDashUpEnable;

	public float sideDashDownCount;
	public bool sideDashDownEnable;
	public bool countSideDashDownEnable;

	//Movement
	public Vector2 inputDirection;

	public bool enableNextIfHit;

	//Hit
	public bool hitJump;
	public bool hitDefense;
	public bool holdDefense;
	public bool hitAttack;
	public bool hitTaunt;
	public bool hitPower;

	//Hold
	public bool holdForwardAfter;
	public bool holdDefenseAfter;
	public bool holdPowerAfter;

	//Team
	public TeamEnum team;
	public int ownerId;
	public ulong selfId;
	// public ObjectPointController ownerOpointController;
	// public ObjectPointCache objectPointCache;

	//Injured
	public int injuredCount;
	public static int INJURED_COUNT_LIMIT = 5;
	public bool injuredCountOneTimePerState;
}
