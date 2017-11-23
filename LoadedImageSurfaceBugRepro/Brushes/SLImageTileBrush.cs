using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Robmikh.CompositionSurfaceFactory;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace LoadedImageSurfaceBugRepro.Brushes
{
    public class SLImageTileBrush : ImageTileBrushBase
    {
        private static CanvasDevice _sharedDevice = null;
        public static CanvasDevice SharedDevice
            => _sharedDevice ?? (_sharedDevice = CanvasDevice.GetSharedDevice());

        private static CompositionGraphicsDevice _compDevice = null;
        public static CompositionGraphicsDevice CompositionGraphicsDevice
            => _compDevice ?? (_compDevice = CanvasComposition.CreateCompositionGraphicsDevice(Window.Current.Compositor, SharedDevice));

        private static SurfaceFactory _surfaceFactory = null;
        public static SurfaceFactory SurfaceFactory
            => _surfaceFactory ?? (_surfaceFactory = SurfaceFactory.CreateFromGraphicsDevice(CompositionGraphicsDevice));


        protected override ICompositionSurface CreateSurface()
        {
            return SurfaceFactory.CreateUriSurface(this.ImageUri).Surface;
        }
    }
}
