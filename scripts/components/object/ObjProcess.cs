using System.Runtime.CompilerServices;
using System.Globalization;
using Godot;
using System;
using System.Collections.Generic;
using static Godot.FileAccess;


public partial class ObjProcess : Node
{
	[Export(PropertyHint.File)]
	protected string dataFile;

	protected ObjEntity objEntity;
	protected FrameEntity currentFrame;

	protected int nextFrameId;
	protected bool useCustomNextId;

	[Export(PropertyHint.Dir)]
	protected string spritePath;
	protected Dictionary<int, Dictionary<int, Rect2>> spritePosition;

	protected Sprite3D sprite3D;
	protected Sprite3D shadowSprite3D;
	protected Timer waitTimer;
	protected CharacterBody3D characterBody3D;

	protected FrameHelper frameHelper;

	protected float DV_VALUE_TO_DIVIDE = 100;

	protected float DV_VALUE_TO_STOP = 550;

	public override void _EnterTree()
	{
		using DirAccess spriteDir = DirAccess.Open(spritePath);
		FileAccess dataFileAccess = Open(dataFile, ModeFlags.Read);

		this.objEntity = new()
		{
			type = GetTypeFromDataFile.Apply(dataFileAccess),
			sprites = GetSpritesForCacheUtil.Apply(spriteDir, spritePath),
			header = GetHeaderFromDataFile.Apply(dataFileAccess),
			stats = GetStatsFromDataFile.Apply(dataFileAccess)
		};

		this.spritePosition = GetSpritesFromDataFile.Apply(dataFileAccess, ref this.objEntity.sprites);

		foreach (KeyValuePair<int, Dictionary<int, Rect2>> entry in spritePosition)
		{
			foreach (KeyValuePair<int, Rect2> entry2 in entry.Value)
			{
				GD.Print("index: " + entry.Key + " - pos: " + entry2.Key + "/" + entry2.Value);
			}
		}

		this.objEntity.frames = GetFramesFromDataFile.Apply(dataFileAccess);
	}

	public void OnWaitTimeout()
	{
		if (useCustomNextId)
		{
			ChangeFrame(nextFrameId);
			useCustomNextId = false;
		}
		else
		{
			ChangeFrame(currentFrame.next);
		}
	}

	protected void ChangeFrame(CharStartFrameEnum nextFrameEnum)
	{
		this.ChangeFrame((int)nextFrameEnum);
	}

	protected void ChangeFrame(Nullable<int> nextFrameId)
	{
		this.ChangeFrame(nextFrameId.Value);
	}

	protected void ChangeFrame(int nextFrameId)
	{
		// GetTree().Paused = !GetTree().Paused;
		// GD.Print(currentFrame.ToString());

		currentFrame = ChangeFrameUtil.Apply(nextFrameId, currentFrame, objEntity.frames, ref frameHelper);

		waitTimer.WaitTime = this.currentFrame.wait / 30;
		waitTimer.Start();

		sprite3D.Texture = objEntity.sprites[currentFrame.textureIndex].sprite;
		sprite3D.RegionRect = spritePosition[currentFrame.textureIndex][currentFrame.pic];

		//GD.Print("currentFrame.textureIndex: " + currentFrame.textureIndex);
		shadowSprite3D.Texture = objEntity.sprites[currentFrame.textureIndex].sprite;
		shadowSprite3D.RegionRect = spritePosition[currentFrame.textureIndex][currentFrame.pic];

		if (frameHelper.facingRight)
		{
			sprite3D.Offset = new Vector2(currentFrame.x_offset, currentFrame.y_offset);
			shadowSprite3D.Offset = new Vector2(currentFrame.x_offset, currentFrame.y_offset);
		}
		else
		{
			sprite3D.Offset = new Vector2(-currentFrame.x_offset, currentFrame.y_offset);
			shadowSprite3D.Offset = new Vector2(-currentFrame.x_offset, currentFrame.y_offset);
		}
	}
}
