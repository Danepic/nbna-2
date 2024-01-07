using Godot;
using System;
using System.Collections.Generic;


public static class GetFramesFromDataFile
{
    public static Dictionary<int, FrameEntity> Apply(FileAccess dataFileAccess)
    {
        Dictionary<int, FrameEntity> result = new();
        FrameEntity currentFrameToMap = null;

        foreach (string line in dataFileAccess.GetAsText().Split("\n"))
        {
            string trimLine = line.Trim();

            if (trimLine.StartsWith("frame"))
            {
                currentFrameToMap = new();

                string id = GetValueFromKeyInDataFile.Apply(trimLine, "frame", "id");
                if (id != null)
                    currentFrameToMap.id = int.Parse(id);

                string name = GetValueFromKeyInDataFile.Apply(trimLine, "frame", "name");
                if (name != null)
                    currentFrameToMap.name = name;

                string state = GetValueFromKeyInDataFile.Apply(trimLine, "frame", "state");
                if (state != null)
                    Enum.TryParse(state, true, out currentFrameToMap.state);

                string wait = GetValueFromKeyInDataFile.Apply(trimLine, "frame", "wait");
                if (wait != null)
                    currentFrameToMap.wait = float.Parse(wait);

                continue;
            }

            if (trimLine.StartsWith("img"))
            {
                string index = GetValueFromKeyInDataFile.Apply(trimLine, "img", "index");
                if (index != null)
                    currentFrameToMap.textureIndex = int.Parse(index);
                    
                string pic = GetValueFromKeyInDataFile.Apply(trimLine, "img", "pic");
                if (pic != null)
                    currentFrameToMap.pic = int.Parse(pic);

                string x = GetValueFromKeyInDataFile.Apply(trimLine, "img", "x");
                if (x != null)
                    currentFrameToMap.x_offset = float.Parse(x);

                string y = GetValueFromKeyInDataFile.Apply(trimLine, "img", "y");
                if (y != null)
                    currentFrameToMap.y_offset = float.Parse(y);

                continue;
            }

            if (trimLine.StartsWith("next"))
            {
                string id = GetValueFromKeyInDataFile.Apply(trimLine, "next", "id");
                if (id != null)
                    currentFrameToMap.next = int.Parse(id);
            }

            if (trimLine.StartsWith("movement"))
            {
                string dvx = GetValueFromKeyInDataFile.Apply(trimLine, "movement", "dvx");
                if (dvx != null)
                    currentFrameToMap.dvx = float.Parse(dvx);

                string dvy = GetValueFromKeyInDataFile.Apply(trimLine, "movement", "dvy");
                if (dvy != null)
                    currentFrameToMap.dvy = float.Parse(dvy);

                string dvz = GetValueFromKeyInDataFile.Apply(trimLine, "movement", "dvz");
                if (dvz != null)
                    currentFrameToMap.dvz = float.Parse(dvz);

                continue;
            }

            if (trimLine.StartsWith("hit"))
            {
                string hitTaunt = GetValueFromKeyInDataFile.Apply(trimLine, "hit", "hitTaunt");
                if (hitTaunt != null)
                    currentFrameToMap.hitTaunt = int.Parse(hitTaunt);

                string hitJump = GetValueFromKeyInDataFile.Apply(trimLine, "hit", "hitJump");
                if (hitJump != null)
                    currentFrameToMap.hitJump = int.Parse(hitJump);

                string hitDefense = GetValueFromKeyInDataFile.Apply(trimLine, "hit", "hitDefense");
                if (hitDefense != null)
                    currentFrameToMap.hitDefense = int.Parse(hitDefense);

                string hitAttack = GetValueFromKeyInDataFile.Apply(trimLine, "hit", "hitAttack");
                if (hitAttack != null)
                    currentFrameToMap.hitAttack = int.Parse(hitAttack);

                string hitPower = GetValueFromKeyInDataFile.Apply(trimLine, "hit", "hitPower");
                if (hitPower != null)
                    currentFrameToMap.hitPower = int.Parse(hitPower);

                string holdForwardAfter = GetValueFromKeyInDataFile.Apply(trimLine, "hit", "holdForwardAfter");
                if (holdForwardAfter != null)
                    currentFrameToMap.holdForwardAfter = int.Parse(holdForwardAfter);

                string holdDefenseAfter = GetValueFromKeyInDataFile.Apply(trimLine, "hit", "holdDefenseAfter");
                if (holdDefenseAfter != null)
                    currentFrameToMap.holdDefenseAfter = int.Parse(holdDefenseAfter);

                string holdPowerAfter = GetValueFromKeyInDataFile.Apply(trimLine, "hit", "holdPowerAfter");
                if (holdPowerAfter != null)
                    currentFrameToMap.holdPowerAfter = int.Parse(holdPowerAfter);

                continue;
            }

            if (trimLine.StartsWith("bdy"))
            {
                // bdy: kind="NORMAL" x="1" y="1" z="1" w="1" h="1" zwidth="1" wallCheck="false"
                string kind = GetValueFromKeyInDataFile.Apply(trimLine, "bdy", "kind");
                if (kind != null)
                    Enum.TryParse(kind, true, out currentFrameToMap.kind);

                string x = GetValueFromKeyInDataFile.Apply(trimLine, "bdy", "x");
                if (x != null)
                    currentFrameToMap.x_body = float.Parse(x);

                string y = GetValueFromKeyInDataFile.Apply(trimLine, "bdy", "y");
                if (y != null)
                    currentFrameToMap.y_body = float.Parse(y);

                string z = GetValueFromKeyInDataFile.Apply(trimLine, "bdy", "z");
                if (z != null)
                    currentFrameToMap.z_body = float.Parse(z);

                string h = GetValueFromKeyInDataFile.Apply(trimLine, "bdy", "h");
                if (h != null)
                    currentFrameToMap.h_body = float.Parse(h);

                string w = GetValueFromKeyInDataFile.Apply(trimLine, "bdy", "w");
                if (w != null)
                    currentFrameToMap.w_body = float.Parse(w);

                string zwidth = GetValueFromKeyInDataFile.Apply(trimLine, "bdy", "zwidth");
                if (zwidth != null)
                    currentFrameToMap.zwidth_body = float.Parse(zwidth);

                continue;
            }

            if (trimLine == "" && currentFrameToMap != null)
            {
                result.Add(currentFrameToMap.id, currentFrameToMap);
                currentFrameToMap = null;
            }
        }

        return result;
    }
}
