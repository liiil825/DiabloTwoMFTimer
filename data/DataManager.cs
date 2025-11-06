using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// AppDomain is already in System namespace
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DTwoMFTimerHelper.Data
{
    public static class DataManager
    {
        // 直接使用用户提到的目录路径进行测试
        private static readonly string ProfilesDirectory = "C:\\Users\\lizhi\\AppData\\Roaming\\mf-time-helper\\profiles\\";
        
        // 静态构造函数，用于验证目录路径
        static DataManager()
        {
            Console.WriteLine($"[目录验证] 角色档案目录路径: {ProfilesDirectory}");
            Console.WriteLine($"[目录验证] 目录是否存在: {Directory.Exists(ProfilesDirectory)}");
            if (Directory.Exists(ProfilesDirectory))
            {
                var files = Directory.GetFiles(ProfilesDirectory, "*.yaml");
                Console.WriteLine($"[目录验证] 目录中找到 {files.Length} 个YAML文件");
                foreach (var file in files)
                {
                    Console.WriteLine($"[目录验证] - {Path.GetFileName(file)}");
                }
            }
        }
        
        // 场景数据文件路径
        private static string FarmingSpotsPath => FindFarmingSpotsFile();
        
        // 智能查找FarmingSpots.yaml文件
        private static string FindFarmingSpotsFile()
        {   
            // 构建可能的路径列表，优先检查Resources文件夹（最佳实践）
            var possiblePaths = new List<string>
            {
                // 相对应用程序基目录的Resources文件夹（推荐的打包位置）
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FarmingSpots.yaml"),
                // 相对应用程序基目录的data文件夹
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "FarmingSpots.yaml"),
                // 相对当前工作目录
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FarmingSpots.yaml"),
                Path.Combine(Directory.GetCurrentDirectory(), "data", "FarmingSpots.yaml")
            };
            
            // 检查哪个路径存在
            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    Console.WriteLine($"找到FarmingSpots.yaml文件：{path}");
                    return path;
                }
            }
            
            // 如果都不存在，返回应用程序基目录下的Resources路径（标准位置）
            string defaultPath = possiblePaths[0];
            Console.WriteLine($"未找到FarmingSpots.yaml文件。尝试使用默认路径：{defaultPath}");
            
            // 只在调试模式下显示MessageBox，避免频繁弹窗影响用户体验
#if DEBUG
            MessageBox.Show($"配置文件未找到，请确保FarmingSpots.yaml位于正确位置。\n\n推荐路径：{defaultPath}", 
                          "配置文件警告", 
                          MessageBoxButtons.OK, 
                          MessageBoxIcon.Warning);
#endif
            
            return defaultPath;
        }
        
        private static readonly ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        
        // 使用CamelCase命名约定，这样可以正确映射yaml中的小写属性到C#的PascalCase属性
        private static readonly IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties() // 忽略不匹配的属性，解决属性名不匹配问题
            .Build();
        
        // 加载所有角色档案
        public static List<CharacterProfile> LoadAllProfiles(bool includeHidden = false)
        {
            // 创建一个简单的文件用于记录调试信息，这样我们可以在文件系统中查看
            string debugLogPath = Path.Combine(Environment.CurrentDirectory, "debug_log.txt");
            
            // 写入日志的辅助方法
            void WriteDebugLog(string message)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(debugLogPath, true))
                    {
                        writer.WriteLine($"[{DateTime.Now}] {message}");
                    }
                }
                catch (Exception logEx)
                {
                    // 避免日志系统本身的异常影响主流程
                    Console.WriteLine($"写入日志失败: {logEx.Message}");
                }
            }
            
            WriteDebugLog($"开始加载所有角色档案，includeHidden={includeHidden}");
            WriteDebugLog($"角色档案目录路径: {ProfilesDirectory}");
            WriteDebugLog($"目录是否存在: {Directory.Exists(ProfilesDirectory)}");
            
            var profiles = new List<CharacterProfile>();
            
            try
            {
                if (!Directory.Exists(ProfilesDirectory))
                {
                    WriteDebugLog("目录不存在");
                    return profiles;
                }
                
                var files = Directory.GetFiles(ProfilesDirectory, "*.yaml");
                WriteDebugLog($"找到 {files.Length} 个角色档案文件");
                foreach (var file in files)
                {
                    WriteDebugLog($"找到文件: {Path.GetFileName(file)}");
                }
                
                foreach (var file in files)
                {
                    try
                    {
                        WriteDebugLog($"正在处理文件: {Path.GetFileName(file)}");
                        
                        // 检查文件是否存在且可读
                        if (!File.Exists(file))
                        {
                            WriteDebugLog($"文件不存在: {file}");
                            continue;
                        }
                        
                        // 使用using确保文件流正确关闭
                        using (var streamReader = new StreamReader(file, Encoding.UTF8))
                        {
                            var yaml = streamReader.ReadToEnd();
                            
                            WriteDebugLog($"读取文件成功，内容长度: {yaml.Length} 字符");
                            // 记录文件的前50个字符用于调试
                            WriteDebugLog($"文件前50字符: {yaml.Substring(0, Math.Min(50, yaml.Length))}");
                            
                            if (string.IsNullOrEmpty(yaml))
                            {
                                WriteDebugLog($"文件内容为空: {file}");
                                continue;
                            }
                            
                            // 反序列化，使用类级别的deserializer（已经配置了IgnoreUnmatchedProperties）
                            CharacterProfile? profile;
                            try
                            {
                                profile = deserializer.Deserialize<CharacterProfile>(yaml);
                            }
                            catch (Exception ex)
                            {
                                WriteDebugLog($"标准反序列化失败，尝试手动解析: {ex.Message}");
                                // 如果标准反序列化失败，尝试手动解析YAML内容
                                profile = ParseYamlManually(yaml, file);
                                
                                if (profile == null)
                                {
                                    WriteDebugLog($"手动解析也失败了: {file}");
                                    continue;
                                }
                            }
                            
                            // 验证反序列化结果
                            if (profile == null)
                            {
                                WriteDebugLog($"反序列化失败，profile为null: {file}");
                                continue;
                            }
                            
                            WriteDebugLog($"反序列化成功，角色名称: {profile.Name}, IsHidden: {profile.IsHidden}");
                            
                            // 确保Records集合已初始化
                            if (profile.Records == null)
                            {
                                profile.Records = new List<MFRecord>();
                                WriteDebugLog($"初始化Records集合: {profile.Name}");
                            }
                            
                            // 根据条件添加到列表
                            WriteDebugLog($"过滤检查: includeHidden={includeHidden}, IsHidden={profile.IsHidden}");
                            if (includeHidden || !profile.IsHidden)
                            {
                                profiles.Add(profile);
                                WriteDebugLog($"成功加载角色: {profile.Name}, 游戏记录数: {profile.Records.Count}");
                            }
                            else
                            {
                                WriteDebugLog($"角色已隐藏，跳过: {profile.Name}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDebugLog($"加载单个角色档案失败 ({file}): {ex.Message}");
                        WriteDebugLog($"异常类型: {ex.GetType().Name}");
                        WriteDebugLog($"异常堆栈: {ex.StackTrace}");
                        // 继续处理其他文件，不中断整个加载过程
                    }
                }
                
                WriteDebugLog($"成功加载 {profiles.Count} 个角色档案");
            }
            catch (Exception ex)
            {
                WriteDebugLog($"加载角色档案失败: {ex.Message}");
                WriteDebugLog($"异常类型: {ex.GetType().Name}");
                WriteDebugLog($"异常堆栈: {ex.StackTrace}");
                
                // 在调试模式下显示错误信息
#if DEBUG
                MessageBox.Show($"加载角色档案失败: {ex.Message}\n目录: {ProfilesDirectory}", "加载错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
            }
            
            return profiles;
        }
        
        // 手动解析YAML内容，处理不同的属性名格式
        private static CharacterProfile? ParseYamlManually(string yamlContent, string filePath)
        {
            var profile = new CharacterProfile();
            string debugLogPath = Path.Combine(Environment.CurrentDirectory, "debug_log.txt");
            
            void WriteDebugLog(string message)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(debugLogPath, true))
                    {
                        writer.WriteLine($"[{DateTime.Now}] 手动解析YAML - {message}");
                    }
                }
                catch {}
            }
            
            try
            {
                WriteDebugLog($"开始手动解析文件: {Path.GetFileName(filePath)}");
                
                // 简单的行解析，处理可能的不同属性名
                foreach (var line in yamlContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length < 2)
                        continue;
                    
                    var key = parts[0].Trim().ToLower();
                    var value = parts[1].Trim();
                    
                    // 处理不同可能的属性名
                    if (key == "name" || key == "character")
                    {
                        profile.Name = value.Trim('"', '\'');
                        WriteDebugLog($"解析到Name: {profile.Name}");
                    }
                    else if (key == "class" || key == "characterclass")
                    {
                        // 尝试解析职业枚举
                        if (Enum.TryParse<CharacterClass>(value, true, out var charClass))
                        {
                            profile.Class = charClass;
                            WriteDebugLog($"解析到Class: {profile.Class}");
                        }
                    }
                    else if (key == "ishidden" || key == "hidden")
                    {
                        if (bool.TryParse(value, out var isHidden))
                        {
                            profile.IsHidden = isHidden;
                            WriteDebugLog($"解析到IsHidden: {profile.IsHidden}");
                        }
                    }
                }
                
                // 确保至少有一个名称
                if (string.IsNullOrEmpty(profile.Name))
                {
                    profile.Name = "未命名角色";  
                    WriteDebugLog($"未找到名称，设置为默认值: {profile.Name}");
                }
                
                // 初始化Records列表
                profile.Records = new List<MFRecord>();
                WriteDebugLog($"手动解析完成，创建了空的Records列表");
                
                return profile;
            }
            catch (Exception ex)
            {
                WriteDebugLog($"手动解析失败: {ex.Message}");
                return new CharacterProfile() { Name = "解析失败角色" };
            }
        }
        
        // 获取安全的文件名（所有地方统一使用）
        private static string GetSafeFileName(string name)
        {
            // 清理文件名，确保安全
            string safeFileName = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
            safeFileName = safeFileName.Replace(" ", "_").ToLower();
            return safeFileName;
        }
        
        // 获取角色档案文件的完整路径
        private static string GetProfileFilePath(string name)
        {
            string safeFileName = GetSafeFileName(name);
            return Path.Combine(ProfilesDirectory, $"{safeFileName}.yaml");
        }
        
        // 保存角色档案
        public static void SaveProfile(CharacterProfile profile)
        {
            try
            {
                // 验证profile对象不为null
                if (profile == null)
                {
                    Console.WriteLine("保存失败: profile对象为null");
                    throw new ArgumentNullException("profile", "保存失败: profile对象为null");
                }
                
                Console.WriteLine($"开始保存角色: {profile.Name}");
                
                // 确保目录存在
                try
                {
                    if (!Directory.Exists(ProfilesDirectory))
                    {
                        Console.WriteLine($"创建目录: {ProfilesDirectory}");
                        Directory.CreateDirectory(ProfilesDirectory);
                        // 验证目录是否创建成功
                        if (!Directory.Exists(ProfilesDirectory))
                        {
                            throw new IOException($"无法创建目录: {ProfilesDirectory}");
                        }
                        Console.WriteLine($"目录创建成功: {ProfilesDirectory}");
                    }
                }
                catch (Exception dirEx)
                {
                    Console.WriteLine($"创建目录失败: {dirEx.Message}");
                    throw new IOException($"创建配置文件目录失败: {dirEx.Message}", dirEx);
                }
                
                // 使用统一的方法获取文件路径
                var filePath = GetProfileFilePath(profile.Name);
                Console.WriteLine($"准备保存到文件: {filePath}");
                
                // 序列化数据
                var yaml = serializer.Serialize(profile);
                if (string.IsNullOrEmpty(yaml))
                {
                    throw new InvalidOperationException("序列化失败，生成了空的YAML数据");
                }
                Console.WriteLine($"序列化成功，数据长度: {yaml.Length} 字符");
                
                // 使用try-finally确保文件流正确关闭
                bool saveSuccess = false;
                int retryCount = 0;
                const int maxRetries = 3;
                
                while (!saveSuccess && retryCount < maxRetries)
                {
                    try
                    {
                        // 使用更安全的文件写入方式
                        using (var streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                        {
                            streamWriter.Write(yaml);
                            streamWriter.Flush();
                            FileInfo fileInfo = new FileInfo(filePath);
                            Console.WriteLine($"文件保存成功，大小: {fileInfo.Length} 字节");
                            saveSuccess = true;
                        }
                        
                        // 验证文件是否真的被写入并包含内容
                        if (File.Exists(filePath))
                        {
                            FileInfo fileInfo = new FileInfo(filePath);
                            if (fileInfo.Length == 0)
                            {
                                throw new IOException("文件已创建但内容为空");
                            }
                            Console.WriteLine($"文件验证成功，实际大小: {fileInfo.Length} 字节");
                        }
                        else
                        {
                            throw new IOException("文件保存后不存在");
                        }
                    }
                    catch (IOException ex) when (retryCount < maxRetries - 1)
                    {
                        retryCount++;
                        Console.WriteLine($"文件保存失败，正在重试 ({retryCount}/{maxRetries}): {ex.Message}");
                        System.Threading.Thread.Sleep(100); // 短暂延迟后重试
                    }
                }
                
                if (!saveSuccess)
                {
                    throw new IOException($"在{maxRetries}次尝试后仍无法保存文件");
                }
                
                Console.WriteLine($"角色 {profile.Name} 保存完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存角色档案失败: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
                
                // 在所有模式下都显示错误信息，确保用户知道保存失败
                string errorMsg = $"保存角色档案失败: {ex.Message}\n文件路径: {ProfilesDirectory}";
                MessageBox.Show(errorMsg, "保存错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // 重新抛出异常，让调用者知道发生了错误
                throw;
            }
        }
        
        // 删除角色档案
        public static void DeleteProfile(CharacterProfile profile)
        {
            try
            {
                // 使用统一的方法获取文件路径
                var filePath = GetProfileFilePath(profile.Name);
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"删除角色档案: {filePath}");
                    File.Delete(filePath);
                    Console.WriteLine($"角色档案删除成功: {profile.Name}");
                }
                else
                {
                    Console.WriteLine($"文件不存在，无法删除: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除角色档案失败: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
                
                // 在所有模式下都显示错误信息
                string errorMsg = $"删除角色档案失败: {ex.Message}";
                MessageBox.Show(errorMsg, "删除错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // 隐藏/显示角色档案
        public static void ToggleProfileVisibility(CharacterProfile profile, bool isHidden)
        {
            profile.IsHidden = isHidden;
            SaveProfile(profile);
        }
        
        // 加载场景数据
        public static List<FarmingScene> LoadFarmingSpots()
        {
            // 确保日志写入到debug_log.txt
            string debugLogPath = Path.Combine(Environment.CurrentDirectory, "debug_log.txt");
            void WriteDebugLog(string message)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(debugLogPath, true))
                    {
                        writer.WriteLine($"[场景加载] {message}");
                    }
                }
                catch (Exception logEx)
                {
                    Console.WriteLine($"写入场景加载日志失败: {logEx.Message}");
                }
            }
            
            WriteDebugLog("===== 开始加载场景数据 =====");
            WriteDebugLog($"尝试加载的文件路径: {FarmingSpotsPath}");
            WriteDebugLog($"当前应用程序基目录: {AppDomain.CurrentDomain.BaseDirectory}");
            WriteDebugLog($"当前工作目录: {Directory.GetCurrentDirectory()}");
            
            try
            {
                bool fileExists = File.Exists(FarmingSpotsPath);
                WriteDebugLog($"文件是否存在: {fileExists}");
                
                if (fileExists)
                {
                    WriteDebugLog("文件存在，开始读取内容...");
                    var yaml = File.ReadAllText(FarmingSpotsPath);
                    WriteDebugLog($"成功读取文件内容，长度: {yaml.Length} 字符");
                    WriteDebugLog($"文件前100字符: {yaml.Substring(0, Math.Min(100, yaml.Length))}");
                    WriteDebugLog("开始反序列化数据...");
                    
                    var data = deserializer.Deserialize<FarmingSpotsData>(yaml);
                    WriteDebugLog($"反序列化成功，场景数量: {data.FarmingSpots.Count}");
                    
                    // 只输出到控制台，不再显示弹窗
                    string keyInfo = $"文件路径: {FarmingSpotsPath}\n文件存在: {fileExists}\n场景数量: {data.FarmingSpots.Count}";
                    Console.WriteLine(keyInfo);
                    
                    return data.FarmingSpots;
                }
                else
                {
                    WriteDebugLog("错误: 文件不存在");
                    // 尝试多个可能的正确路径，优先检查Resources文件夹
                    var possiblePaths = new List<string>
                    {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FarmingSpots.yaml"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "FarmingSpots.yaml"),
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FarmingSpots.yaml"),
                        Path.Combine(Directory.GetCurrentDirectory(), "data", "FarmingSpots.yaml")
                    };
                    
                    foreach (string path in possiblePaths)
                    {
                        bool exists = File.Exists(path);
                        WriteDebugLog($"尝试路径: {path}, 是否存在: {exists}");
                    }
                    
#if DEBUG
                    WriteDebugLog("显示文件加载错误对话框");
                    MessageBox.Show("配置文件未找到。请确保FarmingSpots.yaml位于应用程序的Resources文件夹中。", "文件加载错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    // 生产环境只显示简洁错误信息
                    string errorMsg = "配置文件未找到。请确保FarmingSpots.yaml位于应用程序的Resources文件夹中。";
                    WriteDebugLog($"显示错误信息: {errorMsg}");
                    MessageBox.Show(errorMsg, "文件加载错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                }
            }
            catch (Exception ex)
            {
                WriteDebugLog($"加载场景数据失败: {ex.Message}");
                WriteDebugLog($"异常类型: {ex.GetType().Name}");
                WriteDebugLog($"异常堆栈: {ex.StackTrace}");
                
                #if DEBUG
                    WriteDebugLog("显示加载异常对话框");
                    MessageBox.Show($"加载配置文件时发生错误: {ex.Message}\n\n异常类型: {ex.GetType().Name}\n文件路径: {FarmingSpotsPath}", "加载异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    // 生产环境只显示简洁错误信息
                    string errorMsg = $"加载配置文件时发生错误: {ex.Message}\n\n请确保FarmingSpots.yaml文件格式正确。";
                    WriteDebugLog($"显示错误信息: {errorMsg}");
                    MessageBox.Show(errorMsg, "加载异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
            }
            
            return new List<FarmingScene>();
        }
        
        // 根据名称查找角色档案
        public static CharacterProfile? FindProfileByName(string name, bool includeHidden = false)
        {
            var profiles = LoadAllProfiles(includeHidden);
            return profiles.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        
        // 创建新的角色档案
        public static CharacterProfile CreateNewProfile(string name, DTwoMFTimerHelper.Data.CharacterClass characterClass)
        {
            try
            {
                Console.WriteLine($"开始创建新角色档案: {name}, 职业: {characterClass}");
                
                // 验证输入参数
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("角色名称不能为空", nameof(name));
                }
                
                // 检查角色是否已存在
                var existingProfile = FindProfileByName(name);
                if (existingProfile != null)
                {
                    throw new InvalidOperationException($"角色 '{name}' 已存在");
                }
                
                // 创建新角色
                var profile = new CharacterProfile
                {
                    Name = name,
                    Class = characterClass,
                    IsHidden = false,
                    Records = new List<MFRecord>()
                };
                
                Console.WriteLine($"角色对象创建成功，现在准备保存");
                
                // 目录检查和创建将在SaveProfile方法中完成，避免重复逻辑
                
                // 保存配置文件
                SaveProfile(profile);
                
                Console.WriteLine($"成功创建并保存角色档案: {name}");
                
                // 直接返回创建的profile对象，避免因文件系统缓存导致的验证失败
                // 后续操作会通过正常加载流程确保数据一致性
                return profile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建角色档案失败: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
                // 在所有模式下都显示详细错误信息，确保用户知道具体失败原因
                string errorMsg = ex.InnerException != null
                    ? $"创建角色失败: {ex.Message}\n内部错误: {ex.InnerException.Message}"
                    : $"创建角色失败: {ex.Message}";
                MessageBox.Show(errorMsg, "创建错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; // 重新抛出异常，让调用者知道发生了错误
            }
        }
        
        // 添加MF记录
        public static void AddMFRecord(CharacterProfile profile, MFRecord record)
        {
            profile.Records.Add(record);
            SaveProfile(profile);
        }
        
        // 更新MF记录
        public static void UpdateMFRecord(CharacterProfile profile, MFRecord record)
        {
            var existingRecord = profile.Records.FirstOrDefault(r => 
                r.StartTime == record.StartTime && 
                r.SceneName == record.SceneName);
            
            if (existingRecord != null)
            {
                existingRecord.EndTime = record.EndTime;
                existingRecord.Difficulty = record.Difficulty;
                // 更新其他字段
            }
            
            SaveProfile(profile);
        }
    }
}