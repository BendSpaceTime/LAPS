namespace Azure.Automation.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ComputedStyle
    {
        #region Initialization

        private Dictionary<string, object> rawStyles;

        public ComputedStyle(Dictionary<string, object> rawStyles)
        {
            this.rawStyles = rawStyles;
        }

        #endregion

        #region Size and positioning

        public string Height
        {
            get
            {
                return this.GetValue("height");
            }
        }

        public string Width
        {
            get
            {
                return this.GetValue("width");
            }
        }

        public string Position
        {
            get
            {
                return this.GetValue("position");
            }
        }

        public string Top
        {
            get
            {
                return this.GetValue("top");
            }
        }

        public string Bottom
        {
            get
            {
                return this.GetValue("bottom");
            }
        }

        public string Left
        {
            get
            {
                return this.GetValue("left");
            }
        }

        public string Right
        {
            get
            {
                return this.GetValue("right");
            }
        }

        public string Float
        {
            get
            {
                return this.GetValue("float");
            }
        }

        public string Margin
        {
            get
            {
                return this.GetValue("margin");
            }
        }

        public string MarginBottom
        {
            get
            {
                return this.GetValue("marginBottom");
            }
        }

        public string MarginLeft
        {
            get
            {
                return this.GetValue("marginLeft");
            }
        }

        public string MarginRight
        {
            get
            {
                return this.GetValue("marginRight");
            }
        }

        public string MarginTop
        {
            get
            {
                return this.GetValue("marginTop");
            }
        }

        public string Padding
        {
            get
            {
                return this.GetValue("padding");
            }
        }

        public string PaddingBottom
        {
            get
            {
                return this.GetValue("paddingBottom");
            }
        }

        public string PaddingLeft
        {
            get
            {
                return this.GetValue("paddingLeft");
            }
        }

        public string PaddingRight
        {
            get
            {
                return this.GetValue("paddingRight");
            }
        }

        public string PaddingTop
        {
            get
            {
                return this.GetValue("paddingTop");
            }
        }

        #endregion

        #region Background

        public string Background
        {
            get
            {
                return this.GetValue("background");
            }
        }

        public string BackgroundAttachment
        {
            get
            {
                return this.GetValue("backgroundAttachment");
            }
        }

        public string BackgroundClip
        {
            get
            {
                return this.GetValue("backgroundClip");
            }
        }

        public string BackgroundColor
        {
            get
            {
                return this.GetValue("backgroundColor");
            }
        }

        public string BackgroundImage
        {
            get
            {
                return this.GetValue("backgroundImage");
            }
        }

        public string BackgroundOrigin
        {
            get
            {
                return this.GetValue("backgroundOrigin");
            }
        }

        public string BackgroundPosition
        {
            get
            {
                return this.GetValue("backgroundPosition");
            }
        }

        public string BackgroundPositionX
        {
            get
            {
                return this.GetValue("backgroundPositionX");
            }
        }

        public string BackgroundPositionY
        {
            get
            {
                return this.GetValue("backgroundPositionY");
            }
        }

        public string BackgroundRepeat
        {
            get
            {
                return this.GetValue("backgroundRepeat");
            }
        }

        public string BackgroundRepeatX
        {
            get
            {
                return this.GetValue("backgroundRepeatX");
            }
        }

        public string BackgroundRepeatY
        {
            get
            {
                return this.GetValue("backgroundRepeatY");
            }
        }

        public string BackgroundSize
        {
            get
            {
                return this.GetValue("backgroundSize");
            }
        }

        #endregion

        #region Border

        public string Border
        {
            get
            {
                return this.GetValue("border");
            }
        }

        public string BorderBottom
        {
            get
            {
                return this.GetValue("borderBottom");
            }
        }

        public string BorderBottomColor
        {
            get
            {
                return this.GetValue("borderBottomColor");
            }
        }

        public string BorderBottomLeftRadius
        {
            get
            {
                return this.GetValue("borderBottomLeftRadius");
            }
        }

        public string BorderBottomRightRadius
        {
            get
            {
                return this.GetValue("borderBottomRightRadius");
            }
        }

        public string BorderBottomStyle
        {
            get
            {
                return this.GetValue("borderBottomStyle");
            }
        }

        public string BorderBottomWidth
        {
            get
            {
                return this.GetValue("borderBottomWidth");
            }
        }

        public string BorderCollapse
        {
            get
            {
                return this.GetValue("borderCollapse");
            }
        }

        public string BorderColor
        {
            get
            {
                return this.GetValue("borderColor");
            }
        }

        public string BorderImage
        {
            get
            {
                return this.GetValue("borderImage");
            }
        }

        public string BorderImageOutset
        {
            get
            {
                return this.GetValue("borderImageOutset");
            }
        }

        public string BorderImageRepeat
        {
            get
            {
                return this.GetValue("borderImageRepeat");
            }
        }

        public string BorderImageSlice
        {
            get
            {
                return this.GetValue("borderImageSlice");
            }
        }

        public string BorderImageSource
        {
            get
            {
                return this.GetValue("borderImageSource");
            }
        }

        public string BorderImageWidth
        {
            get
            {
                return this.GetValue("borderImageWidth");
            }
        }

        public string BorderLeft
        {
            get
            {
                return this.GetValue("borderLeft");
            }
        }

        public string BorderLeftColor
        {
            get
            {
                return this.GetValue("borderLeftColor");
            }
        }

        public string BorderLeftStyle
        {
            get
            {
                return this.GetValue("borderLeftStyle");
            }
        }

        public string BorderLeftWidth
        {
            get
            {
                return this.GetValue("borderLeftWidth");
            }
        }

        public string BorderRadius
        {
            get
            {
                return this.GetValue("borderRadius");
            }
        }

        public string BorderRight
        {
            get
            {
                return this.GetValue("borderRight");
            }
        }

        public string BorderRightColor
        {
            get
            {
                return this.GetValue("borderRightColor");
            }
        }

        public string BorderRightStyle
        {
            get
            {
                return this.GetValue("borderRightStyle");
            }
        }

        public string BorderRightWidth
        {
            get
            {
                return this.GetValue("borderRightWidth");
            }
        }

        public string BorderSpacing
        {
            get
            {
                return this.GetValue("borderSpacing");
            }
        }

        public string BorderStyle
        {
            get
            {
                return this.GetValue("borderStyle");
            }
        }

        public string BorderTop
        {
            get
            {
                return this.GetValue("borderTop");
            }
        }

        public string BorderTopColor
        {
            get
            {
                return this.GetValue("borderTopColor");
            }
        }

        public string BorderTopLeftRadius
        {
            get
            {
                return this.GetValue("borderTopLeftRadius");
            }
        }

        public string BorderTopRightRadius
        {
            get
            {
                return this.GetValue("borderTopRightRadius");
            }
        }

        public string BorderTopStyle
        {
            get
            {
                return this.GetValue("borderTopStyle");
            }
        }

        public string BorderTopWidth
        {
            get
            {
                return this.GetValue("borderTopWidth");
            }
        }

        public string BorderWidth
        {
            get
            {
                return this.GetValue("borderWidth");
            }
        }

        #endregion

        #region Rendering

        public string Display
        {
            get
            {
                return this.GetValue("display");
            }
        }

        public string Visibility
        {
            get
            {
                return this.GetValue("visibility");
            }
        }

        public string ZIndex
        {
            get
            {
                return this.GetValue("zIndex");
            }
        }

        public string Overflow
        {
            get
            {
                return this.GetValue("overflow");
            }
        }

        public string OverflowY
        {
            get
            {
                return this.GetValue("overflowY");
            }
        }

        public string OverflowX
        {
            get
            {
                return this.GetValue("overflowX");
            }
        }

        #endregion

        public string GetValue(string key)
        {
            return (string)this.rawStyles[key];
        }
    }
}
