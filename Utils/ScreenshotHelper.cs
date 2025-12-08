using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.Utils;

public static class ScreenshotHelper
{
    /// <summary>
    /// 截取主屏幕并保存到 Screenshots 文件夹
    /// </summary>
    /// <param name="lootName">掉落物品名称（用于文件名）</param>
    /// <returns>保存的文件完整路径，失败返回 null</returns>
    public static string? CaptureAndSave(string lootName)
    {
        try
        {
            // 1. 准备目录
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 2. 处理文件名非法字符
            string safeLootName = string.Join("_", lootName.Split(Path.GetInvalidFileNameChars()));
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Loot_{safeLootName}_{timestamp}.png";
            string fullPath = Path.Combine(folderPath, fileName);

            // 3. 执行截屏
            // 获取主屏幕的边界
            Rectangle bounds = Screen.PrimaryScreen!.Bounds;

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // 将屏幕内容复制到位图
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                // 保存图片
                bitmap.Save(fullPath, ImageFormat.Png);
            }

            LogManager.WriteDebugLog("ScreenshotHelper", $"截图已保存: {fullPath}");
            return fullPath;
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("ScreenshotHelper", "截图失败", ex);
            return null;
        }
    }
}