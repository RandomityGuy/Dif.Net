# Dif.Net
A C# library to read/write/build MBG difs, working obj2difSharp included

# Usage
## Opening DIFs
```cs
using Dif.Net;
var interior = InteriorResource.Load("path/to/interior.dif");
```

## Building DIFs
```cs
using Dif.Net;
using Dif.Net.Builder;
using System.Numerics;

// Create a builder
var builder = new DifBuilder();

// Provide vertices, texture coordinates, etc
var v1 = new Vector3(0,0,0);
var v2 = new Vector3(1,0,0);
var v3 = new Vector3(0,1,0);
var uv1 = new Vector2(0,0);
var uv2 = new Vector2(1,0);
var uv3 = new Vector2(0,1);
var n = new Vector3(0,0,1);
var material = "pattern_cool2";

var polygon = new Polygon() 
{
  Vertices = new List<Vector3>() { v1, v2, v3 },
  Indices = new List<int>() { 0, 1, 2 },
  Normal = n,
  Material = material
};

// Add triangle defined by points and their texture coordinates
builder.AddTriangle(v1,v2,v3,uv1,uv2,uv3);

// Add triangle defined by points and their texture coordinates and the material used
builder.AddTriangle(v1,v2,v3,uv1,uv2,uv3,material);

// Add triangle defined by points and their texture coordinates, a perpendicular from the plane of the triangle and the material used
builder.AddTriangle(v1,v2,v3,uv1,uv2,uv3,n,material);

// Or directly add a previously defined polygon
builder.AddPolygon(polygon);

// Now build our dif
var dif = new InteriorResource();
builder.Build(ref dif);
InteriorResource.Save(dif,"path/to/interior.dif");
```

# obj2difSharp
This is a slightly improved version of the [obj2difPlus](https://github.com/RandomityGuy/obj2difPlus) but in C#

## Usage
```obj2difSharp <filename> [-f] [-r]
filename: the path to the file to convert to dif
(optional) f: flip normals
(optional) r: reverse vertex order
```
# Showcase
![twistedpaths](https://i.imgur.com/NNJp6S9.png "Twisted Paths by RandomityGuy")
