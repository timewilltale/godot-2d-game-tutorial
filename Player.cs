using Godot;

namespace Godot2DGameTutorial;

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();
    
    [Export] 
    public int Speed { get; set; } = 400;

    public Vector2 ScreenSize;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero;

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
        }
    
        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
        }
        
        if (Input.IsActionPressed("move_down"))
        {
            velocity.Y += 1;
        }

        if (Input.IsActionPressed("move_up"))
        {
            velocity.Y -= 1;
        }
        
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed; // stops faster diagonal movement when two inputs pressed at the same time
            animatedSprite2D.Play();
        }
        else
        {
            animatedSprite2D.Stop();
        }
        
        Position += velocity * (float)delta; // delta is the frame length, using it makes movement consistent even if frame rate changes
        
        // restricting bounds
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );

        if (velocity.X != 0)
        {
            animatedSprite2D.Animation = "walk";
            animatedSprite2D.FlipV = false;
            animatedSprite2D.FlipH  = velocity.X < 0;
        }
        else if (velocity.Y != 0)
        {
            animatedSprite2D.Animation = "up";
            animatedSprite2D.FlipV = velocity.Y > 0;
        }
        
    }

    private void OnBodyEntered(object body)
    {
        // player disappears when hit
        Hide(); 
        
        // hit signal
        EmitSignal(SignalName.Hit);
        
        // disable collision so that hit signal isn't triggered more than once
        // set deferred means godot waits to disable the shape (like safely ejecting a usb)
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
    
    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
}
