using System.Runtime.CompilerServices;
using System.Globalization;
using Godot;
using System;
using System.Collections.Generic;
using static Godot.FileAccess;


public partial class ObjDataReader : Node
{
	[Export(PropertyHint.File)]
	private string dataFile;

	private ObjEntity objEntity;
	private FrameEntity currentFrame;

	[Export(PropertyHint.Dir)]
	private string spritePath;
	private Dictionary<int, Dictionary<int, Rect2>> spritePosition;

	private Sprite3D sprite3D;
	private Timer waitTimer;

	private FrameHelper frameHelper;

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
		this.objEntity.frames = GetFramesFromDataFile.Apply(dataFileAccess);
	}

	public override void _Ready()
	{
		if (objEntity.type == ObjTypeEnum.CHARACTER)
			currentFrame = objEntity.frames[0];

		sprite3D = GetNode<Sprite3D>("sprite");
		waitTimer = GetNode<Timer>("wait");
		waitTimer.Timeout += OnWaitTimeout;

		this.frameHelper = new()
		{
			currentHp = this.objEntity.header.startHp,
			currentMp = this.objEntity.header.startMp,
			selfId = this.GetInstanceId()
		};
	}

	public override void _Process(double delta)
	{
		sprite3D.Texture = objEntity.sprites[currentFrame.textureIndex].sprite;
		sprite3D.RegionRect = spritePosition[currentFrame.textureIndex][currentFrame.pic];

		if (waitTimer.WaitTime == 0)
		{
			waitTimer.WaitTime = this.currentFrame.wait / 30;
			waitTimer.Start();

			sprite3D.Texture = objEntity.sprites[currentFrame.textureIndex].sprite;
			sprite3D.RegionRect = spritePosition[currentFrame.textureIndex][currentFrame.pic];
		}

		if (waitTimer.WaitTime <= 0)
		{
			currentFrame = ChangeFrameUtil.Apply(currentFrame.next, currentFrame, objEntity.frames, ref waitTimer, ref frameHelper);
		}

		GD.Print(waitTimer.WaitTime);
	}

	public void OnWaitTimeout() {
		this.QueueFree();
	}
}
