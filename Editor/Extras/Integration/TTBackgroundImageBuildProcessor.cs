#if TUANJIE_1_5_OR_NEWER
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace TTSDK.Tool
{
    /// <summary>
    /// 构建后处理器，用于处理背景图的复制和模板变量替换
    /// </summary>
    public class TTBackgroundImageBuildProcessor
    {
        /// <summary>
        /// 在构建后处理背景图
        /// </summary>
        /// <param name="buildProfile">构建配置</param>
        /// <param name="buildPath">构建输出路径</param>
        public static void ProcessBackgroundImagesAfterBuild(BuildProfile buildProfile, string buildPath)
        {
            var douYinSettings = buildProfile.miniGameSettings as DouYinMiniGameSettings;
            if (douYinSettings == null)
            {
                Debug.LogWarning("未找到抖音小游戏配置，跳过背景图处理");
                return;
            }

            if (string.IsNullOrEmpty(buildPath))
            {
                Debug.LogWarning("构建路径为空，跳过背景图处理");
                return;
            }

            // 根据游戏方向选择对应的背景图
            string customBackgroundPath = douYinSettings.orientation == (int)TTDeviceOrientation.Portrait
                ? douYinSettings.CustomBackgroundPortraitPath
                : douYinSettings.CustomBackgroundLandscapePath;

            // 处理背景图（验证、复制）
            string finalBackgroundPath = TTBackgroundImageProcessor.ProcessBackgroundImage(customBackgroundPath, buildPath);

            // 替换 game.js 中的模板变量
            ReplaceTemplateVariable(buildPath, finalBackgroundPath);
        }

        /// <summary>
        /// 替换 game.js 中的模板变量
        /// </summary>
        /// <param name="buildPath">构建路径</param>
        /// <param name="backgroundPath">背景图路径</param>
        private static void ReplaceTemplateVariable(string buildPath, string backgroundPath)
        {
            string gameJsPath = Path.Combine(buildPath, "game.js");

            if (!File.Exists(gameJsPath))
            {
                Debug.LogWarning($"未找到 game.js 文件: {gameJsPath}");
                return;
            }

            try
            {
                // 读取文件内容
                string content = File.ReadAllText(gameJsPath);

                // 替换模板变量
                string oldContent = content;
                content = content.Replace("$BACKGROUND_IMAGE$", backgroundPath);

                // 如果内容发生了变化，写回文件
                if (content != oldContent)
                {
                    File.WriteAllText(gameJsPath, content);
                    Debug.Log($"已更新背景图配置: {backgroundPath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"替换模板变量失败: {ex.Message}");
            }
        }
    }
}
#endif
