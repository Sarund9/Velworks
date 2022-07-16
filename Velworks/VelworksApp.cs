using System;
using System.Collections;
using System.Collections.Generic;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Velworks.Rendering;

namespace Velworks;

public abstract class VelworksApp
{
    static VelworksApp? s_Instance;
    public static VelworksApp Instance => s_Instance!;

    #region CONSTRUCTOR
    protected VelworksApp(
        string title = "Velworks Application",
        int x = 100, int y = 100,
        int width = 720, int height = 520,
        WindowState windowState = WindowState.Normal)
    {
        Window = VeldridStartup.CreateWindow(new WindowCreateInfo
        {
            WindowHeight = height,
            WindowWidth = width,
            WindowTitle = title,
            X = x, Y = y,
            WindowInitialState = windowState,
        });
        Renderer = new VrkRenderer(Window);
    }
    #endregion

    #region PROPS

    public Sdl2Window Window { get; }
    public VrkRenderer Renderer { get; }

    #endregion

    #region STATIC
    public static void RunApplication<T>(params object[] constructorArgs)
        where T : VelworksApp
    {
        if (s_Instance is not null)
        {
            // Error: Application already created
            return;
        }
        s_Instance = Activator.CreateInstance(
            typeof(T), args: constructorArgs)
            as T;
        if (Instance is null)
        {
            throw new Exception("Application Creation Failed!");
        }
        Instance.Run();
    }
    #endregion

    #region RUN
    private void Run()
    {
        OnInitialize();

        Renderer.InitializeRenderPasses();

        while (Window.Exists)
        {
            Window.PumpEvents();
            if (!Window.Exists)
                break;
            OnUpdate();
            Renderer.Draw();
        }
        OnDeinitialize();
    }
    #endregion

    #region EVENTS

    protected virtual void OnInitialize() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDeinitialize() { }

    #endregion

    #region API



    #endregion
}
