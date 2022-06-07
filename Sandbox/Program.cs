using Velworks;
using Velworks.ShaderSystem;

VelworksApp.RunApplication<App>();


class App : VelworksApp
{
    protected override void OnInitialize()
    {

        var shader = VrkSL.Compile(@"
Name = 'Test'
Version = '450'
#input vrt_

#pass vertex vert
void vert()
{

}
#output v2f
vec3 color

", Console.WriteLine);

        shader?.LogData(Console.WriteLine);
    }
}

