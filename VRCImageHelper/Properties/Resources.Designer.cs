﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace VRCImageHelper.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VRCImageHelper.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   すべてについて、現在のスレッドの CurrentUICulture プロパティをオーバーライドします
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
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
        ///   FFMpeg is required to compress with this format. 
        ///Do you want to download it? に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string FFMpegDownloadMessage {
            get {
                return ResourceManager.GetString("FFMpegDownloadMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   VRC Image Helper / Download FFMpeg に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string FFMpegDownloadTitle {
            get {
                return ResourceManager.GetString("FFMpegDownloadTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Do you want to retroactively perform processing on past VRChat logs?
        ///OSC messages will discard until complete that. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ScanAllRestartMessage {
            get {
                return ResourceManager.GetString("ScanAllRestartMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   VRC Image Helper / Past logs に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ScanAllRestartTitle {
            get {
                return ResourceManager.GetString("ScanAllRestartTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Exit に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ToolbarExit {
            get {
                return ResourceManager.GetString("ToolbarExit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Process past logs に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ToolbarScanAll {
            get {
                return ResourceManager.GetString("ToolbarScanAll", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Processing {Progress}/{Total} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ToolbarScanAllProgress {
            get {
                return ResourceManager.GetString("ToolbarScanAllProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Settings に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ToolbarSettings {
            get {
                return ResourceManager.GetString("ToolbarSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   VRC Image Helper に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ToolbarTitle {
            get {
                return ResourceManager.GetString("ToolbarTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Run when computer starts に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ToolbarToggleAutoStart {
            get {
                return ResourceManager.GetString("ToolbarToggleAutoStart", resourceCulture);
            }
        }
    }
}
