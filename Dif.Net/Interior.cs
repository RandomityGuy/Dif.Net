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
        /// <summary>
        ///      The file version for this Interior
        /// </summary>
        public InteriorFileVersion interiorFileVersion;

        /// <summary>
        /// The detail number that this Interior corresponds to
        /// </summary>
        public int detailLevel;

        /// <summary>
        ///  The minimum pixel size that this Interior should start rendering at if there is more that one LOD in the InteriorInstance
        /// </summary>
        public int minPixels;

        /// <summary>
        ///  An axis-aligned bounding box should exactly encompess all of the geometry
        /// </summary>
        public Box3F boundingBox;

        /// <summary>
        ///  A sphere that completely encloses the geometry
        /// </summary>
        public SphereF boundingSphere;

        /// <summary>
        ///  denotes whether or not the Interior has alarm lighting baked in
        /// </summary>
        [Obsolete]
        public bool hasAlarmState;

        [Obsolete]
        public int numLightStateEntries;

        public List<Vector3> normals;

        /// <summary>
        /// These planes are used by the surfaces, bsp nodes, and hulls for collision and clipping. They are defined  by a normal and a
        /// distance along that normal.This allows the normals to be shared to save on file size.The planes are written out as a list of
        ///  normals and then a list of indices into those normals and a distance along that normal.For most of the data in the Interior 
        /// (bsp node plane indices being the exception), the plane indices can be stored with an |= 0x8000 to indicate that it is an
        /// inverted version of the plane indexed.
        /// </summary>
        public List<InteriorPlane> planes;

        /// <summary>
        /// These are all of the Point3F's that make up the geometry in the Interior
        /// </summary>
        public List<Vector3> points;

        /// <summary>
        /// These aren't used by the engine for anything but they match up 1:1 to the mPoints. A value of 1 indicates
        /// that the point is part of the visible geometry while a value of 0 indicates that the point is only
        /// used as part of the collision hulls.
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        public List<byte> pointVisibilities;

        /// <summary>
        /// The list of texgens that the surfaces index into. Each texgen is comprised of a PlaneF for calculating the u
        /// coordinates and a PlaneF for calculating the v coordinates for each vertex of each surface.
        /// </summary>
        public List<TexGenEQ> texGenEQs;

        /// <summary>
        /// The bsp nodes are used primarily for ray cast collisions. They include an index into the planes as well as an
        /// index to the a front node and a back node.If either of the node indices is masked by 0x8000 then it is a
        /// leaf index.If the leaf index is masked by 0x40000 then it is an empty leaf and you can get its zone by using
        /// index &amp; ~0xC000. If it is not a leaf index then it is an index into the solid bsp leaves.You can retrieve the
        /// solid leaf index with(index &amp; ~0xC000). It is important to note that the engine does not support inverted
        /// plane indices for bsp nodes.
        /// </summary>
        public List<BSPNode> bspNodes;

        /// <summary>
        /// The solid bsp leaves that are used for ray cast collision. They reference a starting point and number of surfaces
        /// in mSolidLeafSurfaces.
        /// </summary>
        public List<BSPSolidLeaf> bspSolidLeaves;

        public byte materialListVersion;

        /// <summary>
        /// A list of the textures used by the surfaces. They should not include path information directly in the material
        /// name.The IO for MaterialList's is handled external to the Interior code and its format can change.
        /// </summary>
        public List<string> materialList;

        /// <summary>
        /// A list of indices into mPoints which are used by the surfaces and portals.
        /// </summary>
        public MultiSizeIntList<int, short, IndexListSizeComparer> windings;

        /// <summary>
        /// The winding indices are pairs of start and count values used by the portals to get at their list of indices from mWindings.
        /// </summary>
        public List<WindingIndex> windingIndices;

        /// <summary>
        /// A list of all of the edges in the Interior. It defines the edge with two indices into mPoints and two indices into mSurfaces.
        /// </summary>
        [Version(12)]
        public List<Edge> edges;

        /// <summary>
        /// The Interior is split into multiple zones which are rendered according to which portals are in view of the camera.
        /// Each zone references a list of surfaces that belong to it and a list of portals which are attached to it.
        /// </summary>
        public List<Zone> zones;

        /// <summary>
        /// A list of the surfaces that belong to the zones in the Interior. These are used to decide which surfaces
        /// are rendered.
        /// </summary>
        public MultiSizeIntList<short, short> zoneSurfaces;

        [Version(12)]
        public List<int> zoneStaticMeshes;

        /// <summary>
        ///  A list of the portals that belong to the zones in the Interior.
        /// </summary>
        public MultiSizeIntList<short, short> zonePortalList;

        /// <summary>
        /// Portals are coplanar (planeIndex) triangle fans that link two zones together. The triangle fans are indexed by using a list
        /// of mWindingIndices(triFanStart and triFanCount) which index into the mWindings array.
        /// </summary>
        public List<Portal> portals;

        /// <summary>
        /// <para>The surfaces are the core geometry of the Interior. The are used by the zones, the bsp solid leaves, and the convex hulls.
        /// They are also the only geometry in the Interior intended for rendering.
        /// </para>
        /// <para>
        /// Each surface is defined by a list of indices(windingStart and windingCount) from mWindings, an index(planeIndex) into mPlanes
        /// (which may be masked as inverted), an index(textureIndex) into the MaterialList for its texture, an index(texGenIndex) into the
        /// mTexGenEqs, a set of surfaceFlags(which indicate whether or not it is an outside surface or a static mesh surface), and a
        /// fanMask(used to determine if a point should be in a collision fan - points inserted to fix t-junctions should not).
        /// </para>
        /// <para>
        /// The lightCount and lightStateInfoStart data are used for animated lights and are depreciated.
        /// The surface's lightmap index can be found in mNormalLMapIndices (1:1 relationship with the surfaces). It actually only uses a small
        /// subset of a larger shared lightmap sheet as defined by mapSizeX, mapSizeY, mapOffsetX, and mapOffsetY.The lightmap texgens
        /// are stored in an encoded form in the surface data (see readLMapTexGen() for more information).
        /// </para>
        /// </summary>
        public List<Surface> surfaces;

        [Version(2, 5)]
        public List<Edge2> edges2;

        [Version(4, 5)]
        public List<Vector3> normals2;

        [Version(4, 5)]
        public MultiSizeIntList<short, byte, NormalIndexListSizeComparer> normalIndices;

        /// <summary>
        /// An index into the lightmaps. There is one for each surface.
        /// </summary>
        [Version(0, 12)]
        public List<byte> normalLMapIndices;

        /// <summary>
        /// An index into the lightmaps. There is one for each surface. A value of 255 indicates that the index is unsued.
        /// </summary>
        [Version(0, 12, new int[] { 4 })]
        [Obsolete]
        public List<byte> alarmLMapIndices;

        /// <summary>
        /// An index into the lightmaps. There is one for each surface.
        /// </summary>
        [Version(13)]
        public List<int> normalLMapIndices_v13;

        /// <summary>
        /// An index into the lightmaps. There is one for each surface. A value of 255 indicates that the index is unsued.
        /// </summary>
        [Version(13)]
        [Obsolete]
        public List<int> alarmLMapIndices_v13;

        /// <summary>
        /// Null surfaces are surfaces that are used for collision but not rendering. The surfaces are defined by a list of
        /// indices and a plane.Note that the null surfaces are not a triangle strip while the surfaces are.
        /// </summary>
        public List<NullSurface> nullSurfaces;

        /// <summary>
        /// The lightmaps used for rendering the surfaces. Multiple lightmaps are packed into a larger sheets. Each lightmap
        /// has a flag to indicate whether or not to keep the lightmap bits after uploading them to the Lightmap Manager.
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        public List<Lightmap> lightMaps;

        /// <summary>
        /// List of indices into surfaces and null surfaces used by the solid bsp leaves. Null surface indices are masked by 0x80000000.
        /// </summary>
        public MultiSizeIntList<int,short> solidLeafSurfaces;

        /// <summary>
        /// Animated lights
        /// </summary>
        [Obsolete]
        public List<AnimatedLight> animatedLights;

        /// <summary>
        /// The color states and info for animated lights
        /// </summary>
        [Obsolete]
        public List<LightState> lightStates;

        /// <summary>
        ///  Used for tracking the various states of animated lights
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        [Obsolete]
        public List<StateData> stateDatas;

        /// <summary>
        /// Used to store intesity maps for animated lights
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        [Obsolete]
        public StateDataBuffer stateDataBuffer;

        /// <summary>
        /// Used to store the name of animated lights
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        [Obsolete]
        public List<byte> nameBuffer;

        /// <summary>
        /// Mirrors are currently the only InteriorSubObject's that are implemented. Please refer to MirrorSubObject::_readISO() for more
        /// information on its file format.
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        public int numSubObjects;

        /// <summary>
        /// <para>
        /// The convex hulls are the closed convex volumes that are used to gather collision data in Torque (getFeatures() and getPolyList()).
        /// Each convex references a list of surfaces(surfaceStart and surfaceCount), a list of indices(hullStart and hullCount) that translates
        /// its local point indices into mPoints, a list of indices(planeStart) that translates its local plane indices into mPlanes, a list of indices
        /// (hullStart) into the emit strings, a list of indices(polyListPlaneStart) that translates its local plane indices into mPlanes for the polylist
        /// system, a list of indices(polyListPointStart) that translates its local point indices into mPoints, and an index in mPolyListStrings for
        /// where its polylist string starts.
        /// </para>
        /// <para> 
        /// The convex hull emit strings and supporting index arrays are used primarily for getFeatures() which is integral to the GJK convex-to-
        /// convex collision system in Torque.Each emit string describes a convex silhouette of a the convex hull from each of the points on the
        /// convex hull.
        /// </para>
        /// <para>
        /// The polylist and supporting index arrays are primarily used for getPolyList() which is used in various other collision subsystems (like
        /// Player) and in the calculation of dynamic shadows.It describes the convex hull in its entirety (points, indices, planes).
        /// </para>
        /// <para>
        /// Also included in the convex hull data are the max and min points of its axis aligned bounding box.
        /// </para>
        /// </summary>
        public List<ConvexHull> convexHulls;

        /// <summary>
        /// <para>
        /// The convex hull emit strings are used to get a convex silhouette of a convex hull for use in Torque's GJK collisions (getFeatures()).
        /// </para>
        /// <para>
        /// The convex hull emit strings can and often are shared by multiple hulls since they use relative indices (a corner on a cube looks like
        /// the corner on another cube no matter what the actual vertex values are when using indices that are local to the convexes).
        /// </para>
        /// </summary>
        public List<byte> convexHullEmitStrings;

        /// <summary>
        /// These remap the local point indices of the convex hulls to the Interior's mPoints array.
        /// </summary>
        public MultiSizeIntList<int,short> hullIndices;

        /// <summary>
        /// These remap the local plane indices of the convex hulls to the Interior's mPlanes array.
        /// </summary>
        public MultiSizeIntList<short,short> hullPlaneIndices;

        /// <summary>
        /// The hull emit string indices are a list of indices into the mConvexHullEmitStrings (which can be shared by multiple hull points). There
        /// should be an emit string index for each point on each hull(as referenced by hullStart and hullCount).
        /// </summary>
        public MultiSizeIntList<int,short> hullEmitStringIndices;

        /// <summary>
        /// A list of indices into mSurfaces which indicates which surfaces belong to which convex collision hull (as referenced by surfaceStart and surfaceCount).
        /// Null surface indices are masked by 0x80000000.
        /// </summary>
        public MultiSizeIntList<int,short> hullSurfaceIndices;

        /// <summary>
        /// These remap the local plane indices of the convex hulls to the Interior's mPlanes array.
        /// </summary>
        public MultiSizeIntList<short,short> polyListPlanes;

        /// <summary>
        /// These remap the local point indices of the convex hulls to the Interior's mPoints array.
        /// </summary>
        public MultiSizeIntList<int,short> polyListPoints;

        /// <summary>
        /// The poly list strings are used for quickly getting at the geometry data of the convex hulls. It must describe a closed convex volume.
        /// This data can easily be generated by processHullPolyLists() if the convex hull surfaces are already set up
        /// </summary>
        public List<byte> polyListStrings;

        /// <summary>
        /// The coord bins are a 2d grid of the volume of the Interior's bounding box. The convex hulls are then sorted into their respective bins according to
        /// to their bounding boxes.
        /// </summary>
        public CoordBinList coordBins;

        /// <summary>
        ///  A list of the indices of the convex brushes that belong in each bin (binStart through binCount).
        /// </summary>
        public MultiSizeIntList<short,short> coordBinIndices;

        /// <summary>
        /// used to indicate which two axii are used for the 2d bins grid...currently only BinsXY is used
        /// </summary>
        [Obsolete]
        public int coordBinMode;

        /// <summary>
        /// the base ambient color for the Interior lightmaps
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        [Obsolete]
        public ColorF baseAmbientColor;

        /// <summary>
        /// the base alarm ambient color for the Interior alarm lightmaps
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        [Obsolete]
        public ColorF alarmAmbientColor;

        /// <summary>
        /// <para>
        /// These are the lightmapped detail meshes that get baked into the Interior. They include simple mesh data (vertices, indices, uvs, textures, and lightmaps).
        /// There is also some extended data(hasSolid, hasTranslucency, bounds, transform, scale) that are used in the rendering of the mesh.</para>
        /// Two things are important to note here:
        /// <para>
        /// 1) Baking the diffuse bitmaps into the Interior file has been depreciated in favor of having the textures external to the Interior file and as such may not
        /// work correctly</para>
        /// 2) The IO for static meshes is implemented externally to the Interior code and can change from the format specified here
        /// </summary>
        [Version(10)]
        public int numStaticMeshes; //Please be 0, this library cant support them yet

        /// <summary>
        /// These are the actual normals used by the texture matrices. They can and often are shared among multiple texture matrices.
        /// </summary>
        [Version(11)]
        public List<Vector3> texNormals;

        [Version(0, 10, new int[] { 4 })]
        public int numTexNormals;

        /// <summary>
        /// The texture matrices are used to properly transform lighting operations into texture space. This is mostly used by shaders and is essential for smooth
        /// shading on Interiors but you can also use it to get point normals by accessing the N normal.Each texture matrix is comprised of three indices into the 
        /// mNormals array. They are mapped to each vertex on each face through mTexMatIndices.
        /// </summary>
        [Version(11)]
        public List<TexMatrix> texMatrices;

        [Version(0, 10, new int[] { 4 })]
        public int numTexMatrices;

        /// <summary>
        /// Each point on each surface should have an index into the mTexMatrices array for its own texture matrix.
        /// </summary>
        [Version(11)]
        public List<int> texMatIndices;

        [Version(0, 10, new int[] { 4 })]
        public int numTexMatIndices;

        /// <summary>
        /// If extendedLightMapData is set to 1 then the next value read will be the padding around the lightmaps. This is used to combat the lightmap
        /// bleeding that can occur when anti-aliasing is enabled on a video card(since it samples outside the uv coords).
        /// </summary>
        [Version(Exclusions = new int[] { 4 })]
        public ExtendedLightmapData extendedLightMapData;

    }
}
