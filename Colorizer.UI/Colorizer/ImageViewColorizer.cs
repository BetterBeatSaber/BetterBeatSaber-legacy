using System.Linq;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Utilities;

using HMUI;

using UnityEngine;
using UnityEngine.UI;

namespace BetterBeatSaber.Colorizer.UI.Colorizer; 

public partial class UIImageColorizer {

    public sealed class ImageColorizer : MonoBehaviour {

        public int amount = -1;
        
        private Image[] _images = null!;
        
        private void Start() {
            _images = GetComponentsInChildren<Image>();
            if(amount > 0)
                _images = _images.Take(amount).ToArray();
        }

        private void FixedUpdate() {
            foreach (var image in _images) {
                image.color = RGB.Color0;
            }
        }

        private void OnDestroy() {
            _images = null!;
        }
        
    }
    
    internal sealed class ImageViewColorizer : MonoBehaviour {
        
        public int amount = -1;
        public float alpha = 1f;
        
        private ImageView[] _imageViews = null!;
        
        private void Start() {
            ReloadImageViews();
        }

        private void FixedUpdate() {
            foreach (var imageView in _imageViews) {
                if (imageView.name == "Icon") {
                    imageView.color0 = RGB.Color0;
                    imageView.color1 = RGB.Color1;
                } else {
                    imageView.color0 = RGB.Color0.WithAlpha(alpha);
                    imageView.color1 = RGB.Color1.WithAlpha(alpha);
                }
            }
        }

        private void OnDestroy() {
            _imageViews = null!;
        }

        public void ReloadImageViews() {
            
            _imageViews = GetComponentsInChildren<ImageView>();
            if(amount > 0)
                _imageViews = _imageViews.Take(amount).ToArray();
            
            foreach (var imageView in _imageViews) {
                imageView.gradient = true;
            }
            
        }
        
    }

}