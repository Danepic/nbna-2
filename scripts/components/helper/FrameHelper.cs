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

	public bool runningRightEnable;
	public bool countRightEnable;

	public bool runningLeftEnable;
	public bool countLeftEnable;

	// Side Dash
	public float sideDashUpCount;
	public bool sideDashUpEnable;
	public bool countSideDashUpEnable;

	public float sideDashDownCount;
	public bool sideDashDownEnable;
	public bool countSideDashDownEnable;

	public bool enableNextIfHit;

	//Hit
	public bool hitJump;
	public bool hitDefense;
	public bool holdDefense;
	public bool hitAttack;
	public bool hitTaunt;
	public bool hitPower;
	public bool hitSuperPower;
	public bool hitUp;
	public bool hitDown;
	public bool hitLeft;
	public bool hitRight;

	//Hit mobile/CPU TODO
	public bool hitCharge;

	//Hit State
	public bool inMovement;

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
