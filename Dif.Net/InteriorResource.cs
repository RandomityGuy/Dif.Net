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
        /// <summary>
        /// Used for storing Interior based subobjects
        /// </summary>
        public List<Interior> subObjects = new List<Interior>();
        /// <summary>
        ///  Allows the InteriorInstance to manager its own set of Trigger objects...mostly used in conjunction with PathedInteriors
        /// </summary>
        public List<Trigger> triggers = new List<Trigger>();
        /// <summary>
        /// Stores a list of PathedInterior object along with their paths and links to triggers
        /// </summary>
        public List<InteriorPathFollower> interiorPathFollowers = new List<InteriorPathFollower>();
        /// <summary>
        /// Special collision brushes designed to block moving objects in the world
        /// </summary>
        [Obsolete]
        public List<ForceField> forceFields = new List<ForceField>();
        /// <summary> 
        /// Nodes used for AI navigation
        /// </summary>
        [Obsolete]
        public List<AISpecialNode> AISpecialNodes = new List<AISpecialNode>();
        /// <summary>
        /// Special convex brushes that are used only for vehicle collision. These are used in place where you don't want the vehicle to be able to procede or in places
        /// where the normal brushes are dense enough to cause collision slow downs.These brushes' emit strings and polylist strings are constructed in the same way
        /// that convex hulls' are.
        /// </summary>
        [Obsolete]
        public VehicleCollision vehicleCollisions = VehicleCollision.Default;
        /// <summary>
        /// Game Entities are placeholders for in-game objects. By running the magicButton() script command on an InteriorInstance you can instantiate these entities
        /// into the current mission.
        /// </summary>
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
