using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class Utils
{
    public static void QueueFree(Node node)
    {
        List<Node> children = GetAllChildrenDeep(node);
        foreach (Node child in children)
        {
            // let audiostreamplayers finish sound before queueing free
            if (child is AudioStreamPlayer audioStreamPlayer)
            {
                MoveToRoot(audioStreamPlayer);
                audioStreamPlayer.Connect("finished", audioStreamPlayer, "queue_free");
            }
            else if (child is Particles2D particles)
            {
                Vector2 position = particles.GlobalPosition;
                MoveToRoot(particles);
                particles.GlobalPosition = position;
                QueueFreeAfterWait(particles, (int)(1000 * particles.Lifetime * (2 - particles.Explosiveness)));
            }
        }
        node.QueueFree();
    }
    private async static void QueueFreeAfterWait(Node node, int milliseconds)
    {
        await Task.Delay(milliseconds);
        node.QueueFree();
    }
    public static List<Node> GetAllChildrenDeep(Node node)
    {
        List<Node> children = new List<Node>();
        foreach (Node child in node.GetChildren())
        {
            children.Add(child);
            children.AddRange(GetAllChildrenDeep(child));
        }
        return children;
    }
    public static List<Node> GetAllChildrenOfType<Type>(Node node)
    {
        List<Node> children = new List<Node>();
        foreach (Node child in node.GetChildren())
        {
            if (child is Type)
            {
                children.Add(child);
            }
        }
        return children;
    }
    public static List<Node> GetAllChildrenOfTypeDeep<Type>(Node node)
    {
        List<Node> children = GetAllChildrenDeep(node);
        foreach (Node child in children)
        {
            if (child is Type)
            {
                children.Add(child);
            }
        }
        return children;
    }
    public static Type FindChildOfType<Type>(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is Type typeChild)
            {
                return typeChild;
            }
        }
        return default(Type);
    }
    public static void MoveToRoot(Node node)
    {
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
            GD.Print((int)(animation.Length * 1000));
            await Task.Delay((int)(animation.Length * 1000));
            callback();
        }
    }
}