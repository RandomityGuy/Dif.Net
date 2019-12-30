using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

namespace Dif.Net
{
    public class InteriorFileVersion : IReadable,IWritable
    {
        public int version;

        public void Read(BinaryReader rb)
        {
            version = rb.ReadInt32();
            IO.interiorFileVersion = version;
        }

        public void Write(BinaryWriter wb)
        {
            IO.interiorFileVersion = version;
            wb.Write(version);
        }

        public static implicit operator InteriorFileVersion(int version)
        {
            return new InteriorFileVersion() { version = version };
        }

    }

    public struct Interior
    {

        public InteriorFileVersion interiorFileVersion;

        public int detailLevel;

        public int minPixels;

        public Box3F boundingBox;

        public SphereF boundingSphere;

        public bool hasAlarmState;

        public int numLightStateEntries;

        public List<Vector3> normals;

        public List<InteriorPlane> planes;

        public List<Vector3> points;

        [Version(Exclusions = new int[] { 4 })]
        public List<byte> pointVisibilities;

        public List<TexGenEQ> texGenEQs;

        public List<BSPNode> bspNodes;

        public List<BSPSolidLeaf> bspSolidLeaves;

        public byte materialListVersion;

        public List<string> materialList;

        public MultiSizeIntList<int, short, IndexListSizeComparer> windings;

        public List<WindingIndex> windingIndices;

        [Version(12)]
        public List<Edge> edges;

        public List<Zone> zones;

        public MultiSizeIntList<short, short> zoneSurfaces;

        [Version(12)]
        public List<int> zoneStaticMeshes;

        public MultiSizeIntList<short, short> zonePortalList;

        public List<Portal> portals;

        public List<Surface> surfaces;

        [Version(2, 5)]
        public List<Edge2> edges2;

        [Version(4, 5)]
        public List<Vector3> normals2;

        [Version(4, 5)]
        public MultiSizeIntList<short, byte, NormalIndexListSizeComparer> normalIndices;

        [Version(0, 12)]
        public List<byte> normalLMapIndices;

        [Version(0, 12, new int[] { 4 })]
        public List<byte> alarmLMapIndices;

        [Version(13)]
        public List<int> normalLMapIndices_v13;

        [Version(13)]
        public List<int> alarmLMapIndices_v13;

        public List<NullSurface> nullSurfaces;

        [Version(Exclusions = new int[] { 4 })]
        public List<Lightmap> lightMaps;

        public MultiSizeIntList<int,short> solidLeafSurfaces;

        public List<AnimatedLight> animatedLights;

        public List<LightState> lightStates;

        [Version(Exclusions = new int[] { 4 })]
        public List<StateData> stateDatas;

        [Version(Exclusions = new int[] { 4 })]
        public StateDataBuffer stateDataBuffer;

        [Version(Exclusions = new int[] { 4 })]
        public List<byte> nameBuffer;

        [Version(Exclusions = new int[] { 4 })]
        public int numSubObjects;

        public List<ConvexHull> convexHulls;

        public List<byte> convexHullEmitStrings;

        public MultiSizeIntList<int,short> hullIndices;

        public MultiSizeIntList<short,short> hullPlaneIndices;

        public MultiSizeIntList<int,short> hullEmitStringIndices;

        public MultiSizeIntList<int,short> hullSurfaceIndices;

        public MultiSizeIntList<short,short> polyListPlanes;

        public MultiSizeIntList<int,short> polyListPoints;

        public List<byte> polyListStrings;

        public CoordBinList coordBins;

        public MultiSizeIntList<short,short> coordBinIndices;

        public int coordBinMode;

        [Version(Exclusions = new int[] { 4 })]
        public ColorF baseAmbientColor;

        [Version(Exclusions = new int[] { 4 })]
        public ColorF alarmAmbientColor;

        [Version(10)]
        public int numStaticMeshes; //Please be 0, this library cant support them yet

        [Version(11)]
        public List<Vector3> texNormals;

        [Version(0, 10, new int[] { 4 })]
        public int numTexNormals;

        [Version(11)]
        public List<TexMatrix> texMatrices;

        [Version(0, 10, new int[] { 4 })]
        public int numTexMatrices;

        [Version(11)]
        public List<int> texMatIndices;

        [Version(0, 10, new int[] { 4 })]
        public int numTexMatIndices;

        [Version(Exclusions = new int[] { 4 })]
        public ExtendedLightmapData extendedLightMapData;

    }
}
