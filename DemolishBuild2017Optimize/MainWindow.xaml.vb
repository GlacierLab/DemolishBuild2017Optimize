Imports System.IO
Imports System.IO.Compression
Imports System.Windows.Forms
Imports System.Windows.Resources
Imports System.Reflection
Class MainWindow
    Dim gamePath As String
    Dim archive As ZipArchive
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim asm As Assembly = Assembly.GetExecutingAssembly()
        archive = New ZipArchive(asm.GetManifestResourceStream("DemolishBuild2017Optimize.optimizeData"), ZipArchiveMode.Read)
        If File.Exists("demolish.exe") Then
            gamePath = Environment.CurrentDirectory + "\demolish_Data"
            Directory.SetCurrentDirectory(gamePath)
            statusLabel.Foreground = Brushes.Green
            statusLabel.Content = "就绪。" + Environment.NewLine + "Ready."
        Else
            ChooseFolder()
        End If
        If Read("mainData", 4260) = &HFE Then
            Exclusive.Background = New SolidColorBrush(ColorConverter.ConvertFromString("#FF44DD20"))
        Else
            Borderless.Background = New SolidColorBrush(ColorConverter.ConvertFromString("#FF44DD20"))
        End If
    End Sub

    Private Function Read(ByVal TargetFile As String, ByVal FileOffset As Long) As Byte
        Dim mode As Byte
        Try
            Dim br As BinaryReader = New BinaryReader(File.Open(TargetFile, FileMode.Open))
            br.BaseStream.Position = FileOffset
            mode = br.BaseStream.ReadByte()
            br.Close()
        Catch
        End Try
        Return mode
    End Function

    Private Sub Patch(ByVal TargetFile As String, ByVal FileOffset As Long, ByVal NewValue() As Byte)
        Try
            Dim br As BinaryReader = New BinaryReader(File.Open(TargetFile, FileMode.Open))
            br.BaseStream.Position = FileOffset
            Dim byteB As Byte
            For Each byteB In NewValue
                If (byteB.ToString <> String.Empty) Then
                    br.BaseStream.WriteByte(byteB)
                Else
                    Exit For
                End If
            Next byteB
            br.Close()
        Catch
        End Try
    End Sub

    Private Sub About_Click(sender As Object, e As RoutedEventArgs) Handles About.Click
        MsgBox("版本1.0.0 琴梨梨 GlacierLab" + Environment.NewLine + "Ver1.0.0 Qinlili GlacierLab" + Environment.NewLine + "独占全屏性能更好" + Environment.NewLine + "Exclusive fullscreen gives better performance." + Environment.NewLine + "解锁帧率后可使用NV面板、RTSS、SpecialK等工具限制帧率" + Environment.NewLine + "After unlocked framerate, you can use Nvidia Panel/RTSS/SpecialK and other tools to limit framerate.",, "关于与说明 About&Help")
    End Sub

    Private Sub Folder_Click(sender As Object, e As RoutedEventArgs) Handles Folder.Click
        Process.Start("demolish.exe")
        End
    End Sub
    Private Sub ChooseFolder()
        Dim f As New FolderBrowserDialog
        f.Description = "选择游戏目录 Select the directory of game."
        f.ShowNewFolderButton = False
        If f.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Directory.SetCurrentDirectory(f.SelectedPath)
            If File.Exists("demolish.exe") Then
                statusLabel.Foreground = Brushes.Green
                statusLabel.Content = "已找到游戏文件。" + Environment.NewLine + "Game Executable Found."
                Try
                    File.Create(gamePath + "\PermissionTest")
                Catch ex As Exception
                    MsgBox("权限不足，请使用管理员权限运行。" + Environment.NewLine + "Permission denied. Please run in administrator.",, "关键错误 Critical Error")
                End Try
                Directory.SetCurrentDirectory(f.SelectedPath + "\demolish_Data")
                gamePath = f.SelectedPath + "\demolish_Data"
            Else
                statusLabel.Foreground = Brushes.Red
                statusLabel.Content = "选择的文件夹内没有游戏。" + Environment.NewLine + "No game in selected folder."
                MsgBox("未找到游戏主程序。请选择正确的游戏目录或直接放到游戏目录运行。" + Environment.NewLine + "Game executable missing. Please select right game path or run in game directory directly.",, "关键错误 Critical Error")
            End If
        Else
            statusLabel.Foreground = Brushes.Red
            statusLabel.Content = "用户取消了路径选择。" + Environment.NewLine + "User canceled folder selecting."
        End If
    End Sub

    Private Sub Borderless_Click(sender As Object, e As RoutedEventArgs) Handles Borderless.Click
        Patch("mainData", 4260, New Byte() {&H0})
        Borderless.Background = New SolidColorBrush(ColorConverter.ConvertFromString("#FF44DD20"))
        Exclusive.Background = New SolidColorBrush(ColorConverter.ConvertFromString("#FFDDDDDD"))
    End Sub

    Private Sub Exclusive_Click(sender As Object, e As RoutedEventArgs) Handles Exclusive.Click
        Patch("mainData", 4260, New Byte() {&HFE})
        Exclusive.Background = New SolidColorBrush(ColorConverter.ConvertFromString("#FF44DD20"))
        Borderless.Background = New SolidColorBrush(ColorConverter.ConvertFromString("#FFDDDDDD"))
    End Sub

    Private Sub Unlock_Click(sender As Object, e As RoutedEventArgs) Handles Unlock.Click
        File.Delete("Managed\Assembly-CSharp.dll")
        Dim entry As ZipArchiveEntry = archive.GetEntry("Assembly-CSharp.dll")
        entry.ExtractToFile(gamePath + "\Managed\Assembly-CSharp.dll")
        Unlock.Background = New SolidColorBrush(ColorConverter.ConvertFromString("#FF44DD20"))
    End Sub
End Class
