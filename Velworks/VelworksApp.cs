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
        var instance = Activator.CreateInstance(
            typeof(T), args: constructorArgs)
            as T;
        if (instance == null)
        {
            throw new Exception("Application Creation Failed!");
        }
        instance.Run();
    }
    #endregion

    #region RUN
    private void Run()
    {
        OnInitialize();

        Renderer.InitializeRenderSystem();

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
