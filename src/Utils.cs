using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Utils
{
    public static void QueueFree(Godot.Node node)
    {
        if (!Godot.Object.IsInstanceValid(node))
        {
            return;
        }
        List<Godot.Node> children = GetChildrenDeep(node);
        foreach (Godot.Node child in children)
        {
            if (!Godot.Node.IsInstanceValid(child))
                return;
            // let audiostreamplayers finish sound before queueing free
            if (child is AudioStreamPlayer audioStreamPlayer)
            {
                if (!audioStreamPlayer.Playing)
                    audioStreamPlayer.QueueFree();
                MoveToRoot(audioStreamPlayer);
                audioStreamPlayer.Connect("finished", audioStreamPlayer, "queue_free");
            }
            else if (child is Particles2D particles)
            {
                Vector2 position = child.IsInsideTree() ? particles.GlobalPosition : particles.Position;
                MoveToRoot(particles);
                particles.GlobalPosition = position;
                QueueFreeAfterWait(particles, (int)(1000 * particles.Lifetime * (2 - particles.Explosiveness)));
            }
        }
        node.QueueFree();
    }
    private async static void QueueFreeAfterWait(Godot.Node node, int milliseconds)
    {
        await Task.Delay(milliseconds);
        node.QueueFree();
    }
    public static List<Godot.Node> GetChildrenDeep(Godot.Node node)
    {
        List<Godot.Node> children = new List<Godot.Node>();
        foreach (Godot.Node child in node.GetChildren())
        {
            children.Add(child);
            children.AddRange(GetChildrenDeep(child));
        }
        return children;
    }
    public static List<Godot.Node> GetChildrenOfType<Type>(Godot.Node node)
    {
        List<Godot.Node> children = new List<Godot.Node>();
        foreach (Godot.Node child in node.GetChildren())
        {
            if (child is Type)
            {
                children.Add(child);
            }
        }
        return children;
    }
    public static List<Godot.Node> GetChildrenOfTypeDeep<Type>(Godot.Node node)
    {
        List<Godot.Node> children = GetChildrenDeep(node);
        foreach (Godot.Node child in children)
        {
            if (child is Type)
            {
                children.Add(child);
            }
        }
        return children;
    }
    public static Type GetChildOfType<Type>(Godot.Node node)
    {
        foreach (Godot.Node child in node.GetChildren())
        {
            if (child is Type typeChild)
            {
                return typeChild;
            }
        }
        return default(Type);
    }
    public static void MoveToRoot(Godot.Node node)
    {
        if (!Godot.Node.IsInstanceValid(node) || !node.IsInsideTree())
            return;
        SceneTree tree = node.GetTree();
        node.GetParent().RemoveChild(node);
        tree.Root.AddChild(node);
    }
    public delegate void Callback();
    /* PlayAnimation plays the animation then calls the callback (if it exists)
       if the animation does not exist the callback will be called immediately */
    public async static void PlayAnimation(AnimationPlayer animationPlayer, string name, Callback callback = null)
    {
        if (animationPlayer == null)
        {
            callback?.Invoke();
            return;
        }

        if (!animationPlayer.HasAnimation(name))
        {
            GD.PushWarning($"Animation {name} not found");
            callback?.Invoke();
            return;
        }
        animationPlayer.Play(name);

        if (callback != null)
        {
            Animation animation = animationPlayer.GetAnimation(name);
            await Task.Delay((int)(animation.Length * 1000));
            callback();
        }
    }
    public static T Load<T>(string path)
    {
        PackedScene packedScene = ResourceLoader.Load<PackedScene>(path);
        if (packedScene == null || packedScene == default(Resource))
        {
            return default(T);
        }
        Godot.Node instance = packedScene.Instance();
        if (instance is T t)
        {
            return t;
        }
        return default(T);
    }
    public async static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(millisecondsDelay, cancellationToken);
        }
        catch (Exception) { }
        return;
    }
    public async static Task Delay(int millisecondsDelay)
    {
        await Task.Delay(millisecondsDelay);
        return;
    }
}