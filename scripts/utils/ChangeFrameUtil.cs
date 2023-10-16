using Godot;
using System;
using System.Collections.Generic;


public static class ChangeFrameUtil
{
    public static FrameEntity Apply(int frameToGo, FrameEntity currentFrame, Dictionary<int, FrameEntity> frames, ref FrameHelper frameHelper)
    {
        frameHelper.previousId = currentFrame.id;
        frameHelper.summonAction = -1;
        // GD.Print("Next frame id: " + currentFrame.next);
        return frames[frameToGo];
    }
}
