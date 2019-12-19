using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

namespace Dif.Net
{
    public struct Interior
    {
        public int interiorFileVersion;
        public int detailLevel;
        public int minPixels;
        public Box3F boundingBox;
        public SphereF boundingSphere;
        public bool hasAlarmState;
        public int numLightStateEntries;
        public List<Vector3> normals;
        public List<InteriorPlane> planes;
        public List<Vector3> points;
        public List<byte> pointVisibilities;
        public List<TexGenEQ> texGenEQs;
        public List<BSPNode> bspNodes;
        public List<BSPSolidLeaf> bspSolidLeaves;
        public byte materialListVersion;
        public List<string> materialList;
        public List<int> windings;
        public List<WindingIndex> windingIndices;
        public List<Zone> zones;
        public List<short> zoneSurfaces;
        public List<short> zonePortalList;
        public List<Portal> portals;
        public List<Surface> surfaces;
        public List<byte> normalLMapIndices;
        public List<byte> alarmLMapIndices;
        public List<NullSurface> nullSurfaces;
        public List<Lightmap> lightMaps;
        public List<int> solidLeafSurfaces;
        public List<AnimatedLight> animatedLights;
        public List<LightState> lightStates;
        public List<StateData> stateDatas;
        public StateDataBuffer stateDataBuffer;
        public List<byte> nameBuffer;
        public int numSubObjects;
        public List<ConvexHull> convexHulls;
        public List<byte> convexHullEmitStrings;
        public List<int> hullIndices;
        public List<short> hullPlaneIndices;
        public List<int> hullEmitStringIndices;
        public List<int> hullSurfaceIndices;
        public List<short> polyListPlanes;
        public List<int> polyListPoints;
        public List<byte> polyListStrings;
        public CoordBinList coordBins;
        public List<short> coordBinIndices;
        public int coordBinMode;
        public ColorF baseAmbientColor;
        public ColorF alarmAmbientColor;
        public List<Vector3> texNormals;
        public List<TexMatrix> texMatrices;
        public List<int> texMatIndices;
        public int extendedLightMapData;

    }
}
