using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dif.Net
{
    public class InteriorResource : IReadable, IWritable
    {
        public int interiorResourceFileVersion = 44;
        public bool previewIncluded = false;
        public List<Interior> detailLevels = new List<Interior>();
        public List<Interior> subObjects = new List<Interior>();
        public List<Trigger> triggers = new List<Trigger>();
        public List<InteriorPathFollower> interiorPathFollowers = new List<InteriorPathFollower>();
        public List<ForceField> forceFields = new List<ForceField>();
        public List<AISpecialNode> AISpecialNodes = new List<AISpecialNode>();
        public VehicleCollision vehicleCollisions = VehicleCollision.Default;
        public List<GameEntity> gameEntities = new List<GameEntity>();

        public void Read(BinaryReader rb)
        {
            interiorResourceFileVersion = rb.ReadInt32();
            previewIncluded = rb.ReadBoolean();
            detailLevels = IO.Read<List<Interior>>(rb);
            subObjects = IO.Read<List<Interior>>(rb);
            triggers = IO.Read<List<Trigger>>(rb);
            interiorPathFollowers = IO.Read<List<InteriorPathFollower>>(rb);
            forceFields = IO.Read<List<ForceField>>(rb);
            AISpecialNodes = IO.Read<List<AISpecialNode>>(rb);
            var readVehicleCollision = rb.ReadInt32();
            if (readVehicleCollision == 1)
                vehicleCollisions = IO.Read<VehicleCollision>(rb);
            var readGameEntities = rb.ReadInt32();
            if (readGameEntities == 2)
                gameEntities = IO.Read<List<GameEntity>>(rb);

        }

        public void Write(BinaryWriter wb)
        {
            IO.Write(interiorResourceFileVersion,wb);
            IO.Write(previewIncluded, wb);
            IO.Write(detailLevels, wb);
            IO.Write(subObjects, wb);
            IO.Write(triggers, wb);
            IO.Write(interiorPathFollowers, wb);
            IO.Write(forceFields, wb);
            IO.Write(AISpecialNodes, wb);
            wb.Write(1);
            IO.Write(vehicleCollisions, wb);
            wb.Write(2);
            IO.Write(gameEntities, wb);
            IO.Write(0, wb); //Dummy lol
        }

        public static InteriorResource Load(string path)
        {
            using (var rb = new BinaryReader(File.OpenRead(path)))
                return IO.Read<InteriorResource>(rb);
        }

        public static void Save(InteriorResource ir,string path)
        {
            using (var wb = new BinaryWriter(File.OpenWrite(path)))
                IO.Write<InteriorResource>(ir, wb);
        }
    }
}
