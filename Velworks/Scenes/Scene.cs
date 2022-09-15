using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefaultEcs;
using MoonSharp.Interpreter;

namespace Velworks.Scenes
{
    [Serializable]
    public class SceneFile
    {
        public List<string> AssetPaths { get; } = new();
        public string RootConstruct { get; set; } = "";



    }

    public struct SceneAssetRef
    {
        string path;

    }

    public class Scene
    {

        Construct root;

        public Scene(SceneFile file)
        {

        }



        class Construct : IDisposable
        {
            private Construct? parent;
            private readonly List<Construct> children = new();
            private readonly HashSet<Entity> entities = new();

            public Construct? Parent
            {
                get => parent;
                set {
                    parent?.children.Remove(this);
                    parent = value;
                    parent?.children.Add(this);
                }
            }

            public void AddEntity(ref Entity e) => entities.Add(e);
            public bool RemoveEntity(ref Entity e) => entities.Remove(e);

            public void Dispose()
            {
                foreach (var child in children)
                {
                    child.Dispose();
                }
                foreach (var entt in entities)
                {
                    entt.Dispose();
                }
                entities.Clear();
            }

        }

    }


    class ConstructEx
    {

        const string StaticMesh = @"
 -- Table keys are ECS Component types
Entity {
    param('transform'),
     -- Table used to fill component parameters
    meshFilter = {
        mesh = param('mesh'),
    },
    meshDraw = {
        material = param('material'),
    },
    collider = param('boxCollider|sphereCollider|capsuleCollider'),
}
";
        /*
        functions:
        - Entity({}) -> EntityInterface
        - param(string) -> DynValue
        objects:
        - EntityInterface
          - add({})
          - [string] get -> ComponentInterface
          - [string] set({}) # Sets Component Type
        - ComponentInterface
          - [string] get -> DynValue
          - [string] set(DynValue)

         */
    }
}
