using System.Runtime.CompilerServices;
using System.Globalization;
using Godot;
using System;
using System.Collections.Generic;
using static Godot.FileAccess;


public partial class CharProcess : ObjProcess
{
	[Export]
	private bool startFacingRight = true;

	private float RUNNING_COUNT = 4f;
	private float SIDE_DASH_COUNT = 4f;

	protected Timer runningLeftCounter;

	protected Timer runningRightCounter;

	protected Timer sideDashUpCounter;

	protected Timer sideDashDownCounter;

	public override void _Ready()
	{
		sprite3D = GetNode<Sprite3D>("sprite");
		shadowSprite3D = GetNode<Sprite3D>("shadow_sprite");
		waitTimer = GetNode<Timer>("wait");
		characterBody3D = GetNode<CharacterBody3D>("body");
		runningLeftCounter = GetNode<Timer>("runningLeftCounter");
		runningRightCounter = GetNode<Timer>("runningRightCounter");
		sideDashUpCounter = GetNode<Timer>("sideDashUpCounter");
		sideDashDownCounter = GetNode<Timer>("sideDashDownCounter");

		currentFrame = objEntity.frames[0];
		sprite3D.Texture = objEntity.sprites[currentFrame.textureIndex].sprite;
		sprite3D.RegionRect = spritePosition[currentFrame.textureIndex][currentFrame.pic];

		waitTimer.Timeout += OnWaitTimeout;
		waitTimer.WaitTime = 0;
		waitTimer.Start();

		runningLeftCounter.Timeout += OnRunningLeftCounterTimeout;
		runningLeftCounter.WaitTime = 0;
		runningLeftCounter.Start();

		runningRightCounter.Timeout += OnRunningRightCounterTimeout;
		runningRightCounter.WaitTime = 0;
		runningRightCounter.Start();

		sideDashUpCounter.Timeout += OnSideDashUpCounterTimeout;
		sideDashUpCounter.WaitTime = 0;
		sideDashUpCounter.Start();

		sideDashDownCounter.Timeout += OnSideDashDownCounterTimeout;
		sideDashDownCounter.WaitTime = 0;
		sideDashDownCounter.Start();

		this.frameHelper = new()
		{
			currentHp = this.objEntity.header.startHp,
			currentMp = this.objEntity.header.startMp,
			selfId = this.GetInstanceId(),
			facingRight = startFacingRight,
		};
	}

	public override void _Process(double delta)
	{
		//Input
		InputHandle();

		//PassiveActions
		base.useCustomNextId = PassiveActionHandle();

		//Actions
		if (!base.useCustomNextId)
		{
			ActionHandle();
		}

		//State
		StateHandle();

		//Physics
		//Opoint
		//Audio
	}

	private bool PassiveActionHandle()
	{
		if (currentFrame.holdForwardAfter != null && frameHelper.holdForwardAfter)
		{
			base.nextFrameId = currentFrame.holdForwardAfter.Value;
			return true;
		}
		return false;
	}

	private void Flip(bool hitLeft, bool hitRight)
	{
		if (hitLeft && hitRight)
		{
			return;
		}

		if (hitLeft)
		{
			sprite3D.FlipH = true;
			shadowSprite3D.FlipH = true;
			frameHelper.facingRight = false;
		}
		else if (hitRight)
		{
			sprite3D.FlipH = false;
			shadowSprite3D.FlipH = false;
			frameHelper.facingRight = true;
		}
	}

	private void StateHandle()
	{
		StateFrameEnum state = currentFrame.state;
		switch (state)
		{
			case StateFrameEnum.STANDING:
				if (frameHelper.inMovement)
				{
					ChangeFrame(StateHelper.WALKING);
					Flip(frameHelper.hitLeft, frameHelper.hitRight);
				}
				break;
			case StateFrameEnum.WALKING:
				Flip(frameHelper.hitLeft, frameHelper.hitRight);
				if (!frameHelper.inMovement)
				{
					ChangeFrame(StateHelper.STANDING);
				}
				break;
			case StateFrameEnum.RUNNING:
				if (frameHelper.facingRight && frameHelper.hitLeft)
				{
					ChangeFrame(CharStartFrameEnum.STOP_RUNNING);
				}
				if (!frameHelper.facingRight && frameHelper.hitRight)
				{
					ChangeFrame(CharStartFrameEnum.STOP_RUNNING);
				}
				break;
			case StateFrameEnum.ATTACKS:
			case StateFrameEnum.JUMPING:
			case StateFrameEnum.JUMPING_FALLING:
			case StateFrameEnum.JUMPING_CHARGE:
			case StateFrameEnum.DOUBLE_JUMPING_FALLING:
			case StateFrameEnum.DASH_JUMPING:
			case StateFrameEnum.DASH:
			case StateFrameEnum.SIDE_DASH:
			case StateFrameEnum.JUMP_DASH:
			case StateFrameEnum.ROWING:
			case StateFrameEnum.DEFEND:
			case StateFrameEnum.HIT_DEFEND:
			case StateFrameEnum.JUMP_DEFEND:
			case StateFrameEnum.HIT_JUMP_DEFEND:
			case StateFrameEnum.BROKEN_DEFEND:
			case StateFrameEnum.CATCHING:
			case StateFrameEnum.CAUGHT:
			case StateFrameEnum.INJURED:
			case StateFrameEnum.FALLING:
			case StateFrameEnum.ICE:
			case StateFrameEnum.LYING:
			case StateFrameEnum.OTHER:
			case StateFrameEnum.JUMP_OTHER:
			case StateFrameEnum.INJURED_2:
			case StateFrameEnum.DRINKING:
			case StateFrameEnum.BURNING:
			case StateFrameEnum.POISONED:
			case StateFrameEnum.SILENCED:
			case StateFrameEnum.SLOW:
			case StateFrameEnum.CONFUSE:
			case StateFrameEnum.PARALYZED:
			case StateFrameEnum.STOP_RUNNING:
				runningLeftCounter.Stop();
				frameHelper.runningLeftEnable = false;
				frameHelper.countLeftEnable = false;

				runningRightCounter.Stop();
				frameHelper.runningRightEnable = false;
				frameHelper.countRightEnable = false;
				break;
			case StateFrameEnum.TELEPORT_NEAR_ENEMY:
			case StateFrameEnum.TELEPORT_NEAR_ALLY:
			case StateFrameEnum.TELEPORT_MOST_DISTANT_ALLY:
			case StateFrameEnum.TELEPORT_MOST_DISTANT_ENEMY:
				break;
		}
	}

	private void ActionHandle()
	{
		if (frameHelper.hitDefense)
		{
			ChangeFrame(currentFrame.hitDefense);
			frameHelper.hitDefense = false;
			return;
		}
		if (frameHelper.hitJump)
		{
			ChangeFrame(currentFrame.hitJump);
			frameHelper.hitJump = false;
			return;
		}
		if (frameHelper.hitAttack)
		{
			ChangeFrame(currentFrame.hitAttack);
			frameHelper.hitAttack = false;
			return;
		}
		if (frameHelper.hitPower)
		{
			ChangeFrame(currentFrame.hitPower);
			frameHelper.hitPower = false;
			return;
		}
		if (frameHelper.hitSuperPower)
		{
			ChangeFrame(currentFrame.hitSuperPower);
			frameHelper.hitSuperPower = false;
			return;
		}
		if (frameHelper.hitTaunt)
		{
			ChangeFrame(currentFrame.hitTaunt);
			frameHelper.hitTaunt = false;
			return;
		}

		// Run Right
		if (frameHelper.runningRightEnable && frameHelper.hitRight)
		{
			frameHelper.runningRightEnable = false;
			frameHelper.runningLeftEnable = false;
			ChangeFrame(CharStartFrameEnum.SIMPLE_DASH);
			return;
		}

		// Run Left
		if (frameHelper.runningLeftEnable && frameHelper.hitLeft)
		{
			frameHelper.runningRightEnable = false;
			frameHelper.runningLeftEnable = false;
			ChangeFrame(CharStartFrameEnum.SIMPLE_DASH);
			return;
		}

		// Side Dash Up
		if (frameHelper.sideDashUpEnable && frameHelper.hitUp)
		{
			frameHelper.sideDashUpEnable = false;
			frameHelper.sideDashDownEnable = false;
			sideDashUpCounter.Stop();
			sideDashDownCounter.Stop();
			ChangeFrame(CharStartFrameEnum.SIDE_DASH);
			return;
		}

		// Side Dash Down
		if (frameHelper.sideDashDownEnable && frameHelper.hitDown)
		{
			frameHelper.sideDashUpEnable = false;
			frameHelper.sideDashDownEnable = false;
			sideDashUpCounter.Stop();
			sideDashDownCounter.Stop();
			ChangeFrame(CharStartFrameEnum.SIDE_DASH);
			return;
		}
	}

	private void InputHandle()
	{
		//attack
		if (Input.IsActionJustPressed(ActionEnum.ATTACK.ToString().ToLower()))
		{
			frameHelper.hitAttack = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.ATTACK.ToString().ToLower()))
		{
			frameHelper.hitAttack = false;
		}

		//jump
		if (Input.IsActionJustPressed(ActionEnum.JUMP.ToString().ToLower()))
		{
			frameHelper.hitJump = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.JUMP.ToString().ToLower()))
		{
			frameHelper.hitJump = false;
		}

		//defense
		if (Input.IsActionJustPressed(ActionEnum.DEFENSE.ToString().ToLower()))
		{
			frameHelper.hitDefense = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.DEFENSE.ToString().ToLower()))
		{
			frameHelper.hitDefense = false;
		}

		//power
		if (Input.IsActionJustPressed(ActionEnum.POWER.ToString().ToLower()))
		{
			frameHelper.hitPower = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.POWER.ToString().ToLower()))
		{
			frameHelper.hitPower = false;
		}

		//super power
		if (Input.IsActionJustPressed(ActionEnum.SUPER_POWER.ToString().ToLower()))
		{
			frameHelper.hitSuperPower = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.SUPER_POWER.ToString().ToLower()))
		{
			frameHelper.hitSuperPower = false;
		}

		//taunt
		if (Input.IsActionJustPressed(ActionEnum.TAUNT.ToString().ToLower()))
		{
			frameHelper.hitTaunt = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.TAUNT.ToString().ToLower()))
		{
			frameHelper.hitTaunt = false;
		}

		//left
		if (Input.IsActionPressed(ActionEnum.LEFT.ToString().ToLower()))
		{
			frameHelper.hitLeft = true;
			frameHelper.holdForwardAfter = true;
			frameHelper.inMovement = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.LEFT.ToString().ToLower()))
		{
			frameHelper.hitLeft = false;
			frameHelper.holdForwardAfter = false;
			if (!frameHelper.countLeftEnable && !frameHelper.facingRight)
			{
				frameHelper.countLeftEnable = true;
				frameHelper.countRightEnable = false;
				return;
			}
		}

		//right
		if (Input.IsActionPressed(ActionEnum.RIGHT.ToString().ToLower()))
		{
			frameHelper.hitRight = true;
			frameHelper.holdForwardAfter = true;
			frameHelper.inMovement = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.RIGHT.ToString().ToLower()))
		{
			frameHelper.hitRight = false;
			frameHelper.holdForwardAfter = false;
			if (!frameHelper.countRightEnable && frameHelper.facingRight)
			{
				frameHelper.countRightEnable = true;
				frameHelper.countLeftEnable = false;
			}
		}

		//up
		if (Input.IsActionPressed(ActionEnum.UP.ToString().ToLower()))
		{
			frameHelper.hitUp = true;
			frameHelper.inMovement = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.UP.ToString().ToLower()))
		{
			frameHelper.hitUp = false;
			if (!frameHelper.countSideDashUpEnable)
			{
				frameHelper.countSideDashUpEnable = true;
				frameHelper.countSideDashDownEnable = false;
				frameHelper.facingUp = true;
			}
		}

		//down
		if (Input.IsActionPressed(ActionEnum.DOWN.ToString().ToLower()))
		{
			frameHelper.hitDown = true;
			frameHelper.inMovement = true;
		}
		else if (Input.IsActionJustReleased(ActionEnum.DOWN.ToString().ToLower()))
		{
			frameHelper.hitDown = false;
			if (!frameHelper.countSideDashDownEnable)
			{
				frameHelper.countSideDashDownEnable = true;
				frameHelper.countSideDashUpEnable = false;
				frameHelper.facingUp = false;
			}
		}

		InputStateHandle();

		//GD.Print($"ATK: {frameHelper.hitAttack} - JUMP: {frameHelper.hitJump} - DEFEND: {frameHelper.hitDefense} - SUPER: {frameHelper.hitPower} - SUPER_POWER: {frameHelper.hitSuperPower} - TAUNT: {frameHelper.hitTaunt}");
		// GD.Print($"UP: {frameHelper.hitUp} - DOWN: {frameHelper.hitDown} - LEFT: {frameHelper.hitLeft} - RIGHT: {frameHelper.hitRight}");
	}

	private void InputStateHandle()
	{
		if (frameHelper.inMovement)
		{
			if (!frameHelper.hitDown && !frameHelper.hitUp && !frameHelper.hitLeft && !frameHelper.hitRight)
			{
				frameHelper.inMovement = false;
			}
		}

		RunningEnabler();
		SideDashEnabler();
	}

	private void RunningEnabler()
	{
		if (frameHelper.countRightEnable && !frameHelper.runningRightEnable)
		{
			frameHelper.runningRightEnable = true;
			runningRightCounter.WaitTime = RUNNING_COUNT / 30;
			runningRightCounter.Start();
			runningLeftCounter.Stop();
		}

		if (frameHelper.countLeftEnable && !frameHelper.runningLeftEnable)
		{
			frameHelper.runningLeftEnable = true;
			runningLeftCounter.WaitTime = RUNNING_COUNT / 30;
			runningLeftCounter.Start();
			runningRightCounter.Stop();
		}
	}

	private void SideDashEnabler()
	{
		if (frameHelper.countSideDashUpEnable && !frameHelper.sideDashUpEnable)
		{
			frameHelper.sideDashUpEnable = true;
			sideDashUpCounter.WaitTime = SIDE_DASH_COUNT / 30;
			sideDashUpCounter.Start();
			sideDashDownCounter.Stop();
		}

		if (frameHelper.countSideDashDownEnable && !frameHelper.sideDashDownEnable)
		{
			frameHelper.sideDashDownEnable = true;
			sideDashDownCounter.WaitTime = SIDE_DASH_COUNT / 30;
			sideDashDownCounter.Start();
			sideDashUpCounter.Stop();
		}
	}

	public void OnRunningLeftCounterTimeout()
	{
		runningLeftCounter.Stop();
		frameHelper.runningLeftEnable = false;
		frameHelper.countLeftEnable = false;
	}
	public void OnRunningRightCounterTimeout()
	{
		runningRightCounter.Stop();
		frameHelper.runningRightEnable = false;
		frameHelper.countRightEnable = false;
	}

	public void OnSideDashUpCounterTimeout()
	{
		sideDashUpCounter.Stop();
		frameHelper.sideDashUpEnable = false;
		frameHelper.countSideDashUpEnable = false;
	}
	public void OnSideDashDownCounterTimeout()
	{
		sideDashDownCounter.Stop();
		frameHelper.sideDashDownEnable = false;
		frameHelper.countSideDashDownEnable = false;
	}
}
