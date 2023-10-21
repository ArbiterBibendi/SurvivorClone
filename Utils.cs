using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class Utils
{
    public async static void QueueFree(Node node)
    {
        List<Node> children = GetAllChildren(node);
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
                MoveToRoot(particles);
                await Task.Delay((int)(1000 * particles.Lifetime * (2 - particles.Explosiveness))); // wait until particles stop emitting
                particles.QueueFree();
            }
        }
        node.QueueFree();
    }
    public static List<Node> GetAllChildren(Node node)
    {
        List<Node> children = new List<Node>();
        foreach (Node child in node.GetChildren())
        {
            children.Add(child);
            children.AddRange(GetAllChildren(child));
        }
        return children;
    }
    public static void MoveToRoot(Node node)
    {
        SceneTree tree = node.GetTree();
        node.GetParent().RemoveChild(node);
        tree.Root.AddChild(node);
    }
    public static void OnAudioStreamPlayerFinished()
    {
        
    }
}