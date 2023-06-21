namespace Godot.Net.Main;

public partial class MainTimerSync
{
    public record struct MainFrameTime
    {
        public double ProcessStep           { get; set; }
        public int    PhysicsSteps          { get; set; }
        public double InterpolationFraction { get; set; }

        public void ClampProcessStep(double minProcessStep, double maxProcessStep)
        {
            if (this.ProcessStep < minProcessStep)
            {
                this.ProcessStep = minProcessStep;
            }
            else if (this.ProcessStep > maxProcessStep)
            {
                this.ProcessStep = maxProcessStep;
            }
        }
    }
}
