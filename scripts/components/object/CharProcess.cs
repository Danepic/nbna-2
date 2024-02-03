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

	private bool skipStateFrameChange = false;

	private float RUNNING_COUNT = 4f;
	private float SIDE_DASH_COUNT = 4f;

	protected Timer runningLeftCounter;

	protected Timer runningRightCounter;

	protected Timer sideDashUpCounter;

	protected Timer sideDashDownCounter;

	protected Vector3 GRAVITY = Vector3.Down * 10;
	protected Vector3 velocity = Vector3.Zero;

	public override void _Ready()
	{
		characterBody3D = GetNode<CharacterBody3D>("body");
		sprite3D = characterBody3D.GetNode<Sprite3D>("sprite");
		shadowSprite3D = characterBody3D.GetNode<Sprite3D>("shadow_sprite");
		waitTimer = GetNode<Timer>("wait");
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
		GD.Print(velocity);

		//Input State Handle
		InputStateHandle();

		//Actions
		ActionHandle();

		//State
		StateHandle();

		//Extern Actions
		ExternActionHandle();

		//Opoint
		ExecOppoint();

		//Audio
		ExecAudio();
	}

	public override void _PhysicsProcess(double delta)
	{
		//GD.Print("R:" + frameHelper.hitRight + "|L:" + frameHelper.hitLeft + "|U:" + frameHelper.hitUp + "|D:" + frameHelper.hitDown);
		StateFrameEnum state = currentFrame.state;
		switch (state)
		{
			case StateFrameEnum.STANDING:
				velocity = Vector3.Zero;
				break;
			case StateFrameEnum.WALKING:
				if (frameHelper.hitRight && !frameHelper.hitLeft)
				{
					velocity.X = currentFrame.dvx / DV_VALUE_TO_DIVIDE;
				}
				else if (frameHelper.hitLeft && !frameHelper.hitRight)
				{
					velocity.X = -currentFrame.dvx / DV_VALUE_TO_DIVIDE;
				}
				else
				{
					velocity.X = 0;
				}

				if (frameHelper.hitUp && !frameHelper.hitDown)
				{
					velocity.Z = currentFrame.dvz / DV_VALUE_TO_DIVIDE;
				}
				else if (frameHelper.hitDown && !frameHelper.hitUp)
				{
					velocity.Z = -currentFrame.dvz / DV_VALUE_TO_DIVIDE;
				}
				else
				{
					velocity.Z = 0;
				}
				break;
			case StateFrameEnum.RUNNING:
				if (frameHelper.facingRight)
				{
					velocity.X = objEntity.stats.speed / DV_VALUE_TO_DIVIDE;

				}
				else
				{
					velocity.X = -objEntity.stats.speed / DV_VALUE_TO_DIVIDE;
				}

				if (frameHelper.hitUp && !frameHelper.hitDown)
				{
					velocity.Z = currentFrame.dvz / DV_VALUE_TO_DIVIDE;
				}
				else if (frameHelper.hitDown && !frameHelper.hitUp)
				{
					velocity.Z = -currentFrame.dvz / DV_VALUE_TO_DIVIDE;
				}
				else
				{
					velocity.Z = 0;
				}
				break;
			default:
				if (currentFrame.dvx != 0)
				{
					if (currentFrame.dvx.Equals(DV_VALUE_TO_STOP))
					{
						velocity.X = 0;
					}
					else
					{
						if (frameHelper.facingRight)
						{
							velocity.X = currentFrame.dvx / DV_VALUE_TO_DIVIDE;

						}
						else
						{
							velocity.X = -currentFrame.dvx / DV_VALUE_TO_DIVIDE;
						}
					}
				}

				if (currentFrame.dvy != 0)
				{
					if (currentFrame.dvy == DV_VALUE_TO_STOP)
					{
						velocity.Y = 0;
					}
					else
					{
						velocity.Y = currentFrame.dvy / DV_VALUE_TO_DIVIDE;
					}
				}

				if (currentFrame.dvz != 0)
				{
					if (currentFrame.dvz == DV_VALUE_TO_STOP)
					{
						velocity.Z = 0;
					}
					else
					{
						velocity.Z = currentFrame.dvz / DV_VALUE_TO_DIVIDE;
					}
				}

				break;
		}

		//Gravity
		// velocity += GRAVITY * (float)delta;
		var collision = characterBody3D.MoveAndCollide(velocity);

		if (collision != null)
		{
			GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}
	}

	public override void _Input(InputEvent @event)
	{
		//attack
		if (@event.IsActionPressed(ActionEnum.ATTACK.ToString().ToLower()))
		{
			frameHelper.hitAttack = true;
		}
		else if (@event.IsActionReleased(ActionEnum.ATTACK.ToString().ToLower()))
		{
			frameHelper.hitAttack = false;
		}

		//jump
		if (@event.IsActionPressed(ActionEnum.JUMP.ToString().ToLower()))
		{
			frameHelper.hitJump = true;
		}
		else if (@event.IsActionReleased(ActionEnum.JUMP.ToString().ToLower()))
		{
			frameHelper.hitJump = false;
		}

		//defense
		if (@event.IsActionPressed(ActionEnum.DEFENSE.ToString().ToLower()))
		{
			frameHelper.hitDefense = true;
			frameHelper.holdDefenseAfter = true;
		}
		else if (@event.IsActionReleased(ActionEnum.DEFENSE.ToString().ToLower()))
		{
			frameHelper.hitDefense = false;
			frameHelper.holdDefenseAfter = false;
		}

		//power
		if (@event.IsActionPressed(ActionEnum.POWER.ToString().ToLower()))
		{
			frameHelper.hitPower = true;
			frameHelper.holdPowerAfter = true;
		}
		else if (@event.IsActionReleased(ActionEnum.POWER.ToString().ToLower()))
		{
			frameHelper.hitPower = false;
			frameHelper.holdPowerAfter = false;
		}

		//super power
		if (@event.IsActionPressed(ActionEnum.SUPER_POWER.ToString().ToLower()))
		{
			frameHelper.hitSuperPower = true;
		}
		else if (@event.IsActionReleased(ActionEnum.SUPER_POWER.ToString().ToLower()))
		{
			frameHelper.hitSuperPower = false;
		}

		//taunt
		if (@event.IsActionPressed(ActionEnum.TAUNT.ToString().ToLower()))
		{
			frameHelper.hitTaunt = true;
		}
		else if (@event.IsActionReleased(ActionEnum.TAUNT.ToString().ToLower()))
		{
			frameHelper.hitTaunt = false;
		}

		//left
		if (@event.IsActionPressed(ActionEnum.LEFT.ToString().ToLower()))
		{
			frameHelper.hitLeft = true;
			frameHelper.holdForwardAfter = true;
		}
		else if (@event.IsActionReleased(ActionEnum.LEFT.ToString().ToLower()))
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
		if (@event.IsActionPressed(ActionEnum.RIGHT.ToString().ToLower()))
		{
			frameHelper.hitRight = true;
			frameHelper.holdForwardAfter = true;
		}
		else if (@event.IsActionReleased(ActionEnum.RIGHT.ToString().ToLower()))
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
		if (@event.IsActionPressed(ActionEnum.UP.ToString().ToLower()))
		{
			frameHelper.hitUp = true;
		}
		else if (@event.IsActionReleased(ActionEnum.UP.ToString().ToLower()))
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
		if (@event.IsActionPressed(ActionEnum.DOWN.ToString().ToLower()))
		{
			frameHelper.hitDown = true;
		}
		else if (@event.IsActionReleased(ActionEnum.DOWN.ToString().ToLower()))
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

	private void ExecAudio()
	{
	}


	private void ExecOppoint()
	{
	}


	private void ApplyPhisics()
	{

	}

	private void ExternActionHandle()
	{

	}

	private void StateHandle()
	{
		StateFrameEnum state = currentFrame.state;
		switch (state)
		{
			case StateFrameEnum.STANDING:
				CanWalking();
				CanRunning();
				CanSideDash();
				break;
			case StateFrameEnum.WALKING:
				CanFlip(frameHelper.hitLeft, frameHelper.hitRight);
				CanStanding();
				CanRunning();
				break;
			case StateFrameEnum.RUNNING:
				CanStopRunning();
				break;
			case StateFrameEnum.ATTACKS:
			case StateFrameEnum.JUMPING:
				CanJumpDash();
				break;
			case StateFrameEnum.JUMPING_FALLING:
				CanJumpDash();
				break;
			case StateFrameEnum.JUMPING_CHARGE:
			case StateFrameEnum.DOUBLE_JUMPING_FALLING:
			case StateFrameEnum.DASH_JUMPING:
			case StateFrameEnum.DASH:
			case StateFrameEnum.SIDE_DASH:
			case StateFrameEnum.JUMP_DASH:
			case StateFrameEnum.ROWING:
				break;
			case StateFrameEnum.DEFEND:
				CanHoldDefenseAfter();
				CanCharge();
				break;
			case StateFrameEnum.HIT_DEFEND:
			case StateFrameEnum.JUMP_DEFEND:
				CanHoldDefenseAfter();
				break;
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
				break;
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
			case StateFrameEnum.HOLD_DEFENSE_AFTER:
				CanHoldDefenseAfter();
				break;
			case StateFrameEnum.HOLD_FORWARD_AFTER:
				CanHoldForwardAfter();
				break;
			case StateFrameEnum.CHARGE:
				CanHoldPowerAfter();
				break;
		}
	}

	private void ActionHandle()
	{
		if (frameHelper.hitDefense && currentFrame.hitDefense.HasValue)
		{
			ChangeFrame(currentFrame.hitDefense);
			frameHelper.hitDefense = false;
			return;
		}
		if (frameHelper.hitJump && currentFrame.hitJump.HasValue)
		{
			ChangeFrame(currentFrame.hitJump);
			frameHelper.hitJump = false;
			return;
		}
		if (frameHelper.hitAttack && currentFrame.hitAttack.HasValue)
		{
			ChangeFrame(currentFrame.hitAttack);
			frameHelper.hitAttack = false;
			return;
		}
		if (frameHelper.hitPower && currentFrame.hitPower.HasValue)
		{
			ChangeFrame(currentFrame.hitPower);
			frameHelper.hitPower = false;
			return;
		}
		if (frameHelper.hitSuperPower && currentFrame.hitSuperPower.HasValue)
		{
			ChangeFrame(currentFrame.hitSuperPower);
			frameHelper.hitSuperPower = false;
			return;
		}
		if (frameHelper.hitTaunt && currentFrame.hitTaunt.HasValue)
		{
			ChangeFrame(currentFrame.hitTaunt);
			frameHelper.hitTaunt = false;
			return;
		}
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
			frameHelper.runningLeftEnable = false;
			runningRightCounter.WaitTime = RUNNING_COUNT / 30;
			runningRightCounter.Start();

			runningLeftCounter.Stop();
		}

		if (frameHelper.countLeftEnable && !frameHelper.runningLeftEnable)
		{
			frameHelper.runningLeftEnable = true;
			frameHelper.runningRightEnable = false;
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
			frameHelper.sideDashDownEnable = false;
			sideDashUpCounter.WaitTime = SIDE_DASH_COUNT / 30;
			sideDashUpCounter.Start();

			sideDashDownCounter.Stop();
		}

		if (frameHelper.countSideDashDownEnable && !frameHelper.sideDashDownEnable)
		{
			frameHelper.sideDashDownEnable = true;
			frameHelper.sideDashUpEnable = false;
			sideDashDownCounter.WaitTime = SIDE_DASH_COUNT / 30;
			sideDashDownCounter.Start();

			sideDashUpCounter.Stop();
		}
	}

	private void CanFlip(bool hitLeft, bool hitRight)
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

	private void CanStanding()
	{
		if (!frameHelper.hitLeft && !frameHelper.hitRight && !frameHelper.hitUp && !frameHelper.hitDown)
		{
			ChangeFrame(StateHelper.STANDING);
			frameHelper.inMovement = false;
		}
		else if ((frameHelper.hitLeft && frameHelper.hitRight) || (frameHelper.hitUp && frameHelper.hitDown))
		{
			ChangeFrame(StateHelper.STANDING);
			frameHelper.inMovement = false;
		}
	}

	private void CanWalking()
	{
		if ((frameHelper.hitLeft && frameHelper.hitRight) || (frameHelper.hitUp && frameHelper.hitDown))
		{
			return;
		}
		else if (frameHelper.hitLeft || frameHelper.hitRight || frameHelper.hitUp || frameHelper.hitDown)
		{
			ChangeFrame(StateHelper.WALKING);
			CanFlip(frameHelper.hitLeft, frameHelper.hitRight);
			frameHelper.inMovement = true;
		}
	}

	private void CanHoldDefenseAfter()
	{
		if (frameHelper.holdDefenseAfter && currentFrame.holdDefenseAfter.HasValue)
		{
			ChangeFrame(currentFrame.holdDefenseAfter);
		}
	}

	private void CanCharge()
	{
		if (frameHelper.hitPower && currentFrame.hitPower.HasValue)
		{
			frameHelper.hitPower = false;
			ChangeFrame(currentFrame.hitPower);
		}
	}

	private void CanHoldPowerAfter()
	{
		if (frameHelper.holdPowerAfter && currentFrame.holdPowerAfter.HasValue)
		{
			frameHelper.hitPower = false;
			ChangeFrame(currentFrame.holdPowerAfter);
		}
	}

	private void CanHoldForwardAfter()
	{
		if (frameHelper.holdForwardAfter && currentFrame.holdForwardAfter.HasValue)
		{
			ChangeFrame(currentFrame.holdForwardAfter);
		}
	}

	public void CanRunning()
	{
		// Run Right
		if (frameHelper.runningRightEnable && frameHelper.hitRight)
		{
			frameHelper.runningRightEnable = false;
			frameHelper.runningLeftEnable = false;
			runningLeftCounter.Stop();
			runningRightCounter.Stop();
			frameHelper.inMovement = true;
			ChangeFrame(CharStartFrameEnum.SIMPLE_DASH);
			return;
		}

		// Run Left
		if (frameHelper.runningLeftEnable && frameHelper.hitLeft)
		{
			frameHelper.runningRightEnable = false;
			frameHelper.runningLeftEnable = false;
			runningLeftCounter.Stop();
			runningRightCounter.Stop();
			frameHelper.inMovement = true;
			ChangeFrame(CharStartFrameEnum.SIMPLE_DASH);
			return;
		}
	}

	public void CanJumpDash()
	{
		// Run Right
		if (frameHelper.runningRightEnable && frameHelper.hitRight)
		{
			frameHelper.runningRightEnable = false;
			frameHelper.runningLeftEnable = false;
			runningLeftCounter.Stop();
			runningRightCounter.Stop();
			ChangeFrame(CharStartFrameEnum.JUMP_DASH);
			return;
		}

		// Run Left
		if (frameHelper.runningLeftEnable && frameHelper.hitLeft)
		{
			frameHelper.runningRightEnable = false;
			frameHelper.runningLeftEnable = false;
			runningLeftCounter.Stop();
			runningRightCounter.Stop();
			ChangeFrame(CharStartFrameEnum.JUMP_DASH);
			return;
		}
	}

	private void CanStopRunning()
	{
		if (frameHelper.facingRight && frameHelper.hitLeft)
		{
			ChangeFrame(CharStartFrameEnum.STOP_RUNNING);
		}
		else if (!frameHelper.facingRight && frameHelper.hitRight)
		{
			ChangeFrame(CharStartFrameEnum.STOP_RUNNING);
		}
	}

	public void CanSideDash()
	{
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
