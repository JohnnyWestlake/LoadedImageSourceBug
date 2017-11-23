using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace LoadedImageSurfaceBugRepro.Brushes
{
    public class LISImageTileBrush : ImageTileBrushBase
    {
        protected override ICompositionSurface CreateSurface()
        {
            return LoadedImageSurface.StartLoadFromUri(this.ImageUri);
        }
    }
}
