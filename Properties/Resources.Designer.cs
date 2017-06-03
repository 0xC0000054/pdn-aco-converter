﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SwatchConverter.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SwatchConverter.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The swatch file does not contain any colors..
        /// </summary>
        internal static string EmptySwatchFile {
            get {
                return ResourceManager.GetString("EmptySwatchFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap openHS {
            get {
                object obj = ResourceManager.GetObject("openHS", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to R: {0} G:{1} B: {2}.
        /// </summary>
        internal static string RGBFormat {
            get {
                return ResourceManager.GetString("RGBFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to R: {0} G:{1} B: {2} ({3}).
        /// </summary>
        internal static string RGBNameFormat {
            get {
                return ResourceManager.GetString("RGBNameFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap SavePalette {
            get {
                object obj = ResourceManager.GetObject("SavePalette", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} ({1} colors).
        /// </summary>
        internal static string StatusBarLoadFormat {
            get {
                return ResourceManager.GetString("StatusBarLoadFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The swatch file does not contain any supported color modes..
        /// </summary>
        internal static string UnsupportedColorMode {
            get {
                return ResourceManager.GetString("UnsupportedColorMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The swatch file version is not supported..
        /// </summary>
        internal static string UnsupportedSwatchFileVersion {
            get {
                return ResourceManager.GetString("UnsupportedSwatchFileVersion", resourceCulture);
            }
        }
    }
}
