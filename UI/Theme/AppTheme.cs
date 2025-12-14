using System.Drawing;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Theme;

public static class AppTheme
{
    // ==========================================
    // 新结构 (The New Structure)
    // ==========================================

    public static class Colors
    {
        // 基础色
        public static Color Background = Color.FromArgb(32, 32, 32); // 深灰背景
        public static Color Surface = Color.FromArgb(45, 45, 48); // 容器背景
        public static Color ControlBackground = Color.FromArgb(45, 45, 48); // 控件背景(同Surface)
        public static Color Border = Color.FromArgb(60, 60, 60);

        // 按钮
        public static Color ButtonBackColor = Color.FromArgb(50, 45, 40);

        // 次要文本颜色
        public static Color TextSecondaryColor = Color.FromArgb(160, 160, 160); // 次要文本

        // 按钮文本
        public static Color ButtonText = Color.FromArgb(240, 240, 240); // 按钮文本

        // 功能色
        public static Color Primary = Color.FromArgb(199, 179, 119); // 暗黑金 (原 AccentColor)
        public static Color Text = Color.FromArgb(240, 240, 240); // 主文本
        public static Color TextSecondary = Color.FromArgb(160, 160, 160); // 次要文本

        // 状态色 (对应暗黑2装备颜色)
        public static Color Success = Color.FromArgb(0, 255, 0); // Set Items
        public static Color Warning = Color.FromArgb(255, 255, 0); // Rare Items
        public static Color Info = Color.FromArgb(100, 100, 255); // Magic Items (原 69,69,255 调亮)
        public static Color Error = Color.FromArgb(255, 80, 80); // Life Red
    }

    public static class Fonts
    {
        // 通用字体
        public static Font Regular { get; private set; } = null!; // 正文 (原 MainFont)
        public static Font Bold { get; private set; } = null!; // 粗体 (原 SmallTitleFont)
        public static Font Title { get; private set; } = null!; // 标题 (原 TitleFont)
        public static Font Large { get; private set; } = null!; // 大号 (原 BigFont)
        public static Font Console { get; private set; } = null!; // 等宽 (原 ConsoleFont)

        // 计时器专用
        public static Font TimerBig { get; private set; } = null!; // (原 BigTimeFont)
        public static Font TimerFull { get; private set; } = null!; // (原 FullTimeFont)
        public static Font BigTitle { get; private set; } = null!; // (原 BigTitleFont)

        // 辅助属性
        public static FontFamily FontFamily => Regular?.FontFamily ?? System.Drawing.FontFamily.GenericSansSerif;

        // 字体大小常量
        private const float BaseMainFontSize = 10F;
        private const float BaseTitleFontSize = 12F;
        private const float BaseBigFontSize = 20F;
        private const float BaseBigTimeFontSize = 18F;
        private const float BaseFullTimeFontSize = 36F;

        public static void Initialize()
        {
            Regular = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseMainFontSize), FontStyle.Regular);
            Bold = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseMainFontSize), FontStyle.Bold);
            Title = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseTitleFontSize), FontStyle.Bold);
            Large = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseBigFontSize), FontStyle.Regular);
            BigTitle = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseBigFontSize), FontStyle.Bold);

            Console = new Font("Consolas", ScaleHelper.ScaleFont(BaseMainFontSize), FontStyle.Regular);

            TimerBig = new Font("Verdana", ScaleHelper.ScaleFont(BaseBigTimeFontSize), FontStyle.Bold);
            TimerFull = new Font("Consolas", BaseFullTimeFontSize, FontStyle.Bold); // 这个通常不缩放或者特殊处理
        }
    }

    // ==========================================
    // 兼容层 (Backward Compatibility)
    // 保持旧属性存在，指向新结构，防止现有代码报错
    // ==========================================

    // Colors Mappings
    public static Color BackColor => Colors.Background;
    public static Color SurfaceColor => Colors.Surface;
    public static Color AccentColor => Colors.Primary;
    public static Color SetColor => Colors.Success;
    public static Color RareColor => Colors.Warning;
    public static Color MagicColor => Colors.Info;
    public static Color ErrorColor => Colors.Error;
    public static Color TextColor => Colors.Text;
    public static Color TextSecondaryColor => Colors.TextSecondary;
    public static Color BorderColor => Colors.Border;

    // Fonts Mappings
    public static Font MainFont => Fonts.Regular;
    public static Font BigFont => Fonts.Large;
    public static Font ArialFont => Fonts.Regular; // 暂时映射到 Regular，或者你需要单独加一个 Arial
    public static Font ConsoleFont => Fonts.Console;
    public static Font SmallTitleFont => Fonts.Bold;
    public static Font BigTitleFont => Fonts.BigTitle;
    public static Font TitleFont => Fonts.Title;
    public static Font BigTimeFont => Fonts.TimerBig;
    public static Font FullTimeFont => Fonts.TimerFull;

    // Initialization Mapping
    public static void InitializeFonts()
    {
        Fonts.Initialize();
    }
}
