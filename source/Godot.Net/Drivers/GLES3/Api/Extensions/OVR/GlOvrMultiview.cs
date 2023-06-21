namespace Godot.Net.Drivers.GLES3.Api.Extensions.OVR;

using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Drivers.GLES3.Api.Interfaces;

public class GlOvrMultiview : IExtension
{
    public static string Name => "GL_OVR_multiview";

    private delegate void GLFramebufferTextureMultiviewOVR(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level, int baseViewIndex, int numViews);

    private readonly GLFramebufferTextureMultiviewOVR glFramebufferTextureMultiviewOVR;

    public GlOvrMultiview(ILoader loader) => this.glFramebufferTextureMultiviewOVR = loader.Load<GLFramebufferTextureMultiviewOVR>("glFramebufferTextureMultiviewOVR");

    public void FramebufferTextureMultiview(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level, int baseViewIndex, int numViews) =>
        this.glFramebufferTextureMultiviewOVR.Invoke(target, attachment, texture, level, baseViewIndex, numViews);

    public static IExtension Create(ILoader loader) => new GlOvrMultiview(loader);
}
