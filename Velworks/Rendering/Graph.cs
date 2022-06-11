using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Velworks.Rendering
{
    [Serializable]
    public class Node
    {
        public float posX;
        public float posY;


    }

    // A graph of nodes, can also be a Subgraph
    public class Graph : Node
    {
        List<Node> nodes = new List<Node>();
        Node exit;


    }

    [Serializable]
    public class GraphData
    {
        List <Node> nodes = new List<Node>();
        int exit;

    }


    // Takes Resources as Inputs/Outputs
    public class InstructionNode : Node { }

    public class InputNode : Node { }
    public class OutputNode : Node { }

    // Something required by nodes
    public class ResourceNode : Node { }


    public class DrawScenePass : InstructionNode
    {
        [Channel(ChannelType.In)]
        SceneQuerry? querry;

        [Channel(ChannelType.Out)]
        IRenderTarget? target;


    }
    public class RenderTarget : ResourceNode { }

    [AttributeUsage(AttributeTargets.Field)]
    public class ChannelAttribute : Attribute
    {
        public ChannelAttribute(ChannelType type)
        {
            Type = type;
        }

        public ChannelType Type { get; }
    }
    public enum ChannelType
    {
        In = 0,
        Out,
        InOut,
    }

    public class SceneQuerry
    {

    }

    public interface IRenderTarget
    {
        public bool Bind(CommandList cl);
    }
}
