using Veldrid;
using Velworks;
using Velworks.Rendering;
using Velworks.ShaderSystem;

VelworksApp.RunApplication<App>();

class App : VelworksApp
{
    protected override void OnInitialize()
    {
        // Construct Render Pipeline
        Renderer.AddRenderPass(new ClearPass(RgbaFloat.Black));
        Renderer.AddRenderPass(new TestPass(Renderer.Device));



    }
}

