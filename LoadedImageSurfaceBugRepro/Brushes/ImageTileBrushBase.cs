using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;



namespace LoadedImageSurfaceBugRepro.Brushes
{
    public abstract class ImageTileBrushBase : XamlCompositionBrushBase
    {
        #region Dependency Properties

        #region ImageUri

        public Uri ImageUri
        {
            get { return (Uri)GetValue(ImageUriProperty); }
            set { SetValue(ImageUriProperty, value); }
        }

        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register(nameof(ImageUri), typeof(Uri), typeof(ImageTileBrushBase), new PropertyMetadata(null, (d, e) =>
            {
                ((ImageTileBrushBase)d).SetImageUri();
            }));

        #endregion

        #endregion
        protected Compositor _compositor => Window.Current.Compositor;

        protected CompositionBrush _imageBrush = null;

        protected IDisposable _surfaceSource = null;

        protected override void OnConnected()
        {
            base.OnConnected();
            CreateEffectBrush();
            if (ImageUri != null && _imageBrush == null)
                SetImageUri();
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            this.CompositionBrush?.Dispose();
            this.CompositionBrush = null;

            ClearResources();
        }

        private void ClearResources()
        {
            _imageBrush?.Dispose();
            _imageBrush = null;

            _surfaceSource?.Dispose();
            _surfaceSource = null;
        }

        private void UpdateBrush()
        {
            if (CompositionBrush != null && _imageBrush != null)
            {
                ((CompositionEffectBrush)CompositionBrush).SetSourceParameter(nameof(BorderEffect.Source), _imageBrush);
            }
        }

        protected abstract ICompositionSurface CreateSurface();

        private void SetImageUri()
        {
            ClearResources();

            if (ImageUri == null)
                return;

            var uri = ImageUri;

            try
            {
                var src = CreateSurface();
                _surfaceSource = src as IDisposable;
                var surfaceBrush = _compositor.CreateSurfaceBrush(src);
                surfaceBrush.VerticalAlignmentRatio = 0.0f;
                surfaceBrush.HorizontalAlignmentRatio = 0.0f;
                surfaceBrush.Stretch = CompositionStretch.None;
                _imageBrush = surfaceBrush;

                UpdateBrush();
            }
            catch
            {
                // no image for you, soz.
            }
        }

        private void CreateEffectBrush()
        {
            using (var effect = new BorderEffect
            {
                Name = nameof(BorderEffect),
                ExtendY = CanvasEdgeBehavior.Wrap,
                ExtendX = CanvasEdgeBehavior.Wrap,
                Source = new CompositionEffectSourceParameter(nameof(BorderEffect.Source))
            })
            using (var _effectFactory = _compositor.CreateEffectFactory(effect))
            {
                var brush = _effectFactory.CreateBrush();
                this.CompositionBrush = brush;
                UpdateBrush();
            }
        }
    }
}
