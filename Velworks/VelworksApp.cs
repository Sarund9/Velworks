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
    Sdl2Window window;
    VrkRenderer renderer;

    #region CONSTRUCTOR
    protected VelworksApp(
        string title = "Velworks Application",
        int x = 100, int y = 100,
        int width = 720, int height = 520,
        WindowState windowState = WindowState.Normal)
    {
        window = VeldridStartup.CreateWindow(new WindowCreateInfo
        {
            WindowHeight = height,
            WindowWidth = width,
            WindowTitle = title,
            X = x, Y = y,
            WindowInitialState = windowState,
        });
        renderer = new VrkRenderer(window);
    }
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
        while (window.Exists)
        {
            window.PumpEvents();
            if (!window.Exists)
                break;
            OnUpdate();
            renderer.Draw();
        }
        OnDeinitialize();
    }
    #endregion

    #region EVENTS

    protected virtual void OnInitialize() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDeinitialize() { }

    #endregion
}

