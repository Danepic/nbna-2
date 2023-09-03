using Godot;
using System;
using System.Collections.Generic;


public static class ChangeFrameUtil
{
    public static FrameEntity Apply(int frameToGo, FrameEntity currentFrame, Dictionary<int, FrameEntity> frames, ref Timer waitTimer, ref FrameHelper frameHelper, bool usingNextPattern = true)
    {
        frameHelper.previousId = currentFrame.id;
        frameHelper.summonAction = -1;
        waitTimer.WaitTime = 0;
        if (usingNextPattern)
        {
            return currentFrame.next == (int)FrameSpecialValuesEnum.BACK_TO_STANDING ? frames[0] : frames[frameToGo];
        }
        else
        {
            return frames[frameToGo];
        }
    }
}
