#if TUANJIE_1_5_OR_NEWER
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TTSDK.Tool
{
    /// <summary>
    /// 背景图处理器，用于验证、复制和处理自定义背景图
    /// </summary>
    public static class TTBackgroundImageProcessor
    {
        private const int MaxImageSize = 2048;
        private const string DefaultBackgroundImage = "images/background.png";

        /// <summary>
        /// 处理背景图，验证并复制到构建目录
        /// </summary>
        /// <param name="customPath">自定义背景图路径</param>
        /// <param name="buildPath">构建输出路径</param>
        /// <returns>最终使用的背景图相对路径</returns>
        public static string ProcessBackgroundImage(string customPath, string buildPath)
        {
            // 如果未指定自定义路径，使用默认图片
            if (string.IsNullOrEmpty(customPath))
            {
                return DefaultBackgroundImage;
            }

            // 验证源文件是否存在
            if (!File.Exists(customPath))
            {
                Debug.LogWarning($"背景图文件不存在: {customPath}，使用默认图片");
                return DefaultBackgroundImage;
            }

            // 加载并验证纹理
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(customPath);
            if (texture == null)
            {
                Debug.LogWarning($"无法加载背景图: {customPath}，使用默认图片");
                return DefaultBackgroundImage;
            }

            // 验证尺寸（参考微信 SDK 的限制）
            if (texture.width > MaxImageSize || texture.height > MaxImageSize)
            {
                Debug.LogError($"背景图尺寸超过限制 ({MaxImageSize}x{MaxImageSize}): {texture.width}x{texture.height}，使用默认图片");
                return DefaultBackgroundImage;
            }

            // 确保目标目录存在
            var imagesDir = Path.Combine(buildPath, "images");
            if (!Directory.Exists(imagesDir))
            {
                Directory.CreateDirectory(imagesDir);
            }

            // 生成目标文件名（保留原始文件名）
            var filename = Path.GetFileName(customPath);
            var destPath = Path.Combine(imagesDir, filename);

            // 复制图片到构建目录
            try
            {
                File.Copy(customPath, destPath, true);
                Debug.Log($"已使用自定义背景图: {filename} ({texture.width}x{texture.height})");
                return $"images/{filename}";
            }
            catch (Exception ex)
            {
                Debug.LogError($"复制背景图失败: {ex.Message}，使用默认图片");
                return DefaultBackgroundImage;
            }
        }
    }
}
#endif
