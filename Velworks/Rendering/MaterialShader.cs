namespace Velworks.Rendering;

using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using Velworks.ShaderSystem;

public sealed class MaterialShader : IDisposable
{

    public MaterialShader(GraphicsDevice gd,
        ShaderDescription vert, ShaderDescription frag, string shaderName,
        DepthStencilStateDescription depthStencilState,
        RasterizerStateDescription rasterizerState)
    {
        GraphicsDevice = gd;
        ShaderPasses = gd.ResourceFactory.CreateFromSpirv(vert, frag);
        Name = shaderName;
        DepthStencilState = depthStencilState;
        RasterizerState = rasterizerState;
    }

    public GraphicsDevice GraphicsDevice { get; private set; }
    public Shader[] ShaderPasses { get; private set; }
    public string Name { get; private set; }
    public DepthStencilStateDescription DepthStencilState { get; }
    public RasterizerStateDescription RasterizerState { get; }

    public void Dispose()
    {
        for (int i = 0; i < ShaderPasses.Length; i++)
        {
            ShaderPasses[i].Dispose();
        }
    }

    public class Builder
    {
        readonly GraphicsDevice gd;
        string shaderName;

        string? vertexCode;
        string vertexEntryPoint = "main";
        string? fragmentCode;
        string fragmentEntryPoint = "main";

        // from MaterialShader
        DepthStencilStateDescription depthStencilState = new(
            depthTestEnabled: true,
            depthWriteEnabled: true,
            comparisonKind: ComparisonKind.LessEqual);

        // from MaterialShader
        RasterizerStateDescription rasterizerState = new(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false);

        public Builder(GraphicsDevice gd, string shaderName)
        {
            this.gd = gd;
            this.shaderName = shaderName;
        }

        #region DEPTH STENCIL
        public Builder DepthTest(bool enabled)
        {
            depthStencilState.DepthTestEnabled = enabled;
            return this;
        }
        public Builder DepthWrite(bool enabled)
        {
            depthStencilState.DepthWriteEnabled = enabled;
            return this;
        }
        public Builder DepthComparison(ComparisonKind comp)
        {
            depthStencilState.DepthComparison = comp;
            return this;
        }
        #endregion

        #region RASTERIZER

        public Builder CullingMode(FaceCullMode cullMode)
        {
            rasterizerState.CullMode = cullMode;
            return this;
        }

        public Builder FillMode(PolygonFillMode fillMode)
        {
            rasterizerState.FillMode = fillMode;
            return this;
        }

        public Builder DrawFace(FrontFace frontFace)
        {
            rasterizerState.FrontFace = frontFace;
            return this;
        }
        public Builder DepthClip(bool enabled)
        {
            rasterizerState.DepthClipEnabled = enabled;
            return this;
        }
        public Builder ScissorTest(bool enabled)
        {
            rasterizerState.ScissorTestEnabled = enabled;
            return this;
        }

        #endregion

        public Builder Vertex(string code, string entryPoint = "main")
        {
            vertexCode = code;
            vertexEntryPoint = entryPoint;
            return this;
        }
        public Builder Fragment(string code, string entryPoint = "main")
        {
            fragmentCode = code;
            fragmentEntryPoint = entryPoint;
            return this;
        }

        public MaterialShader Create()
        {
            if (vertexCode is null)
                throw new ArgumentNullException("No Vertex Code!");
            if (fragmentCode is null)
                throw new ArgumentNullException("No Fragment Code!");
            
            return new MaterialShader(gd,
                new ShaderDescription(
                    ShaderStages.Vertex,
                    Encoding.UTF8.GetBytes(vertexCode),
                    vertexEntryPoint),
                new ShaderDescription(
                    ShaderStages.Fragment,
                    Encoding.UTF8.GetBytes(fragmentCode),
                    fragmentEntryPoint),
                shaderName, depthStencilState, rasterizerState);
        }
    }

    #region OLD
    //public class Builder
    //{
    //    readonly List<Shader> passes = new List<Shader>();
    //    readonly GraphicsDevice gd;
    //    string? name;
    //    public Builder(GraphicsDevice gd)
    //    {
    //        this.gd = gd;
    //    }

    //    public Builder Name(string shadername)
    //    {
    //        name = shadername;
    //        return this;
    //    }

    //    public Builder CompileShader(ShaderDescription description) => CompileShader(ref description);
    //    public Builder CompileShader(ref ShaderDescription description)
    //    {
    //        passes.Add(gd.ResourceFactory.CreateShader(ref description));
    //        return this;
    //    }

    //    public MaterialShader Create()
    //    {
    //        if (name is null)
    //            throw new ArgumentNullException("No Shader Name!");

    //        return new MaterialShader(gd, passes.ToArray(), name);
    //    }

    //}
    #endregion

}


