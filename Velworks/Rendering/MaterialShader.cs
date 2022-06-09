namespace Velworks.Rendering;
using Veldrid;
using Veldrid.SPIRV;
using Velworks.ShaderSystem;

public class MaterialShader
{

    public MaterialShader(GraphicsDevice gd, ShaderCompilerArgs args, string name)
    {
        GraphicsDevice = gd;

        ShaderPasses = args.Compile(gd.ResourceFactory);

        Name = name;
    }
    public MaterialShader(GraphicsDevice gd, ShaderDescription vert, ShaderDescription frag, string name)
    {
        GraphicsDevice = gd;

        ShaderPasses = gd.ResourceFactory.CreateFromSpirv(vert, frag);

        Name = name;
    }

    public GraphicsDevice GraphicsDevice { get; private set; }
    public Shader[] ShaderPasses { get; private set; }
    public string Name { get; private set; }

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

    
}


