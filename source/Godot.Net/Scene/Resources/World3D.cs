namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.IO;

#pragma warning disable IDE0044,CS0649 // TODO Remove

public class World3D : Resource
{
    private Environment? fallbackEnvironment;

    public Environment? FallbackEnvironment { get => this.fallbackEnvironment; set => this.SetFallbackEnvironment(value); }
    public Guid         Scenario            { get; }

    public World3D() => this.Scenario = RS.Singleton.ScenarioCreate();

    // void World3D::set_fallback_environment(const Ref<Environment> &p_environment)
    private void SetFallbackEnvironment(Environment? value)
    {
        if (this.fallbackEnvironment == value)
        {
            return;
        }

        this.fallbackEnvironment = value;

        if (this.fallbackEnvironment != null)
        {
            RS.Singleton.ScenarioSetFallbackEnvironment(this.Scenario, this.fallbackEnvironment.Id);
        }
        else
        {
            RS.Singleton.ScenarioSetFallbackEnvironment(this.Scenario, default);
        }

        this.EmitChanged();
    }
}
