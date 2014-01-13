using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace vsix_AlternatingLineColorTextAdornment
{

    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    class AlternatingLineColorAdornment
    {
        IAdornmentLayer alternatingLineColorLayer;
        Brush brush;

        /// <summary>
        /// Creates a square image and attaches an event handler to the layout changed event that
        /// adds the  square in the upper right-hand corner of the TextView via the adornment layer
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public AlternatingLineColorAdornment(IWpfTextView textView)
        {
            this.brush = new SolidColorBrush(Color.FromArgb(160, 194, 252, 233));
            //this.brush = new SolidColorBrush(Color.FromArgb(160, 40, 80, 60));
            this.brush.Freeze();
            this.alternatingLineColorLayer = textView.GetAdornmentLayer(AlternatingLineColorFactory.LayerName);

            textView.LayoutChanged += OnLayoutChanged;
            textView.ViewportWidthChanged += OnViewportWidthChanged;
            textView.ViewportLeftChanged += OnViewportLeftChanged;
        }

        void OnViewportLeftChanged(object sender, EventArgs e)
        {
            IWpfTextView textView = (IWpfTextView)sender;

            foreach (var r in this.alternatingLineColorLayer.Elements)
            {
                Canvas.SetLeft((Rectangle)r.Adornment, textView.ViewportLeft);
            }
        }

        void OnViewportWidthChanged(object sender, EventArgs e)
        {
            IWpfTextView textView = (IWpfTextView)sender;

            foreach (var r in this.alternatingLineColorLayer.Elements)
            {
                ((Rectangle)r.Adornment).Width = textView.ViewportWidth;
            }
        }

        void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            IWpfTextView textView = (IWpfTextView)sender;
            if (e.OldSnapshot != e.NewSnapshot && e.OldSnapshot.Version.Changes.IncludesLineChanges)
            {
                this.alternatingLineColorLayer.RemoveAllAdornments();
                Refresh(textView, textView.TextViewLines);
            }
            else
            {
                Refresh(textView, e.NewOrReformattedLines);
            }
        }

        protected void Refresh(IWpfTextView textView, IList<ITextViewLine> lines)
        {
            foreach (var line in lines)
            {
                int lineNumber = textView.TextSnapshot.GetLineNumberFromPosition(line.Extent.Start);
                if (lineNumber % 2 == 1)
                {
                    var rect = new Rectangle()
                        {
                            Height = line.Height,
                            Width = textView.ViewportWidth,
                            Fill = this.brush
                        };
                    
                    Canvas.SetLeft(rect, textView.ViewportLeft);
                    Canvas.SetTop(rect, line.Top);
                    this.alternatingLineColorLayer.AddAdornment(line.Extent, null, rect);
                }
            }
        }
    }
}
