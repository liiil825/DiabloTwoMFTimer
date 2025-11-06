using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

namespace DTwoMFTimerHelper.Resources
{
    public static class LanguageManager
    {
        // 语言常量
        public const string Chinese = "zh-CN";
        public const string English = "en-US";
        
        // 当前语言
        private static CultureInfo currentCulture = CultureInfo.CurrentCulture;
        private static Dictionary<string, string> translations = new Dictionary<string, string>();
        
        // 语言变更事件
        public static event EventHandler? OnLanguageChanged;

        static LanguageManager()
        {
            LoadTranslations(currentCulture);
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="languageCode">语言代码</param>
        public static void SwitchLanguage(string languageCode)
        {
            if (!string.IsNullOrEmpty(languageCode) && languageCode != currentCulture.Name)
            {
                currentCulture = new CultureInfo(languageCode);
                Thread.CurrentThread.CurrentCulture = currentCulture;
                Thread.CurrentThread.CurrentUICulture = currentCulture;
                LoadTranslations(currentCulture);
                OnLanguageChanged?.Invoke(typeof(LanguageManager), EventArgs.Empty);
            }
        }

        private static void LoadTranslations(CultureInfo? culture)
        {
            translations.Clear();
            
            // 确定要加载的语言文件
            string langCode = culture?.Name.StartsWith("zh") ?? false ? "zh" : "en";
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", $"strings_{langCode}.json");
            
            // 如果文件不存在，尝试使用相对路径
            if (!File.Exists(jsonFilePath))
            {
                // 尝试直接在Resources目录中查找
                jsonFilePath = Path.Combine("Resources", $"strings_{langCode}.json");
            }
            
            if (File.Exists(jsonFilePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                    translations = deserialized ?? new Dictionary<string, string>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading translations: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 根据键获取本地化字符串
        /// </summary>
        /// <param name="key">字符串键</param>
        /// <param name="args">格式化参数</param>
        /// <returns>本地化后的字符串</returns>
        public static string GetString(string key, params object[] args)
        {
            if (key == null) return string.Empty;
            string value = string.Empty;
            if (translations != null && translations.TryGetValue(key, out string? tempValue) && tempValue != null)
            {
                value = tempValue;
                if (args != null && args.Length > 0)
                {
                    // 使用正则表达式替换{{0}}, {{1}}等占位符
                    string formattedValue = value;
                    for (int i = 0; i < args.Length; i++)
                    {
                        string replacement = args[i]?.ToString() ?? string.Empty;
                        formattedValue = Regex.Replace(
                            formattedValue,
                            $"\\{{\\{{{i}\\}}\\}}",
                            replacement
                        );
                    }
                    return formattedValue;
                }
                return value;
            }
            return key; // 如果找不到翻译，返回键本身
        }
    }
}