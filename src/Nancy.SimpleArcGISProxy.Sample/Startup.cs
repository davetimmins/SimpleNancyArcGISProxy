namespace Nancy.SimpleArcGISProxy.Sample
{
    using global::Owin;
    using Microsoft.Owin.Extensions;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app
                .UseNancy()
                .UseStageMarker(PipelineStage.MapHandler);
        }
    }
}