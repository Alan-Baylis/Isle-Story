﻿using System.IO;
public enum CommandType {
    MOVE,GETIN,GETOUT, MIGRATE, CHANGEJOB, CHANGEWORK,BUILD
}

public struct Command {
    public CommandType type;
    public TriDirection dir;
    public Entity target;
    public TriCell targetLocation;
    public int flag;

    public override string ToString() {
        return type.ToString() + '\n'+dir+'\n';
    }









    public Command(CommandType type) {
        dir = TriDirection.VERT;
        this.type = type;
        target = null;
        targetLocation = null;
        flag = 0;
    }
    public Command(CommandType type,TriDirection dir,Entity target) {
        this.dir = dir;
        this.type = type;
        this.target = target;
        this.targetLocation = target.Location;
        flag = 1;
    }
    public Command(CommandType type, TriCell targetL) {
        dir = TriDirection.VERT;
        this.type = type;
        this.target = null;
        this.targetLocation = targetL;
        flag = 2;
    }
    public Command(CommandType type, TriDirection dir,Entity target,TriCell targetL) {
        this.dir = dir;
        this.type = type;
        this.target = null;
        this.targetLocation = targetL;
        flag = 3;
    }
    public void Save(BinaryWriter writer) {
        writer.Write((int)type);
        writer.Write(flag);
        if (flag%2==1) {
            target.Location.coordinates.Save(writer);
        }
        else if(flag>=2){
            targetLocation.coordinates.Save(writer);
        }
    }
    public static Command Load(BinaryReader reader) {
        TriGrid grid = TriGrid.Instance;
        CommandType type = (CommandType)reader.ReadInt32();
        int flag = reader.ReadInt32();
        Command t = new Command(type);
        if (flag % 2 == 1) {
            t.target = grid.GetCell(TriCoordinates.Load(reader)).Entity;
        }
        else if (flag >= 2) {
            t.targetLocation = grid.GetCell(TriCoordinates.Load(reader));
        }
        t.flag = flag;
        return t;
    }
}