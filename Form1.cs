using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace UnityBackUp
{
    public partial class UnityBackUpSystem : Form
    {
        static ProgressDialog pd = new ProgressDialog();
        static ProgressDialog zippd = new ProgressDialog();
        static string fileName;

        public UnityBackUpSystem()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            while (true)
            {
                //ダイアログの作成
                FolderBrowserDialog fdDialog_D = new FolderBrowserDialog();
                FolderBrowserDialog fdDialog = new FolderBrowserDialog();
                DialogResult zipDialog;
                DialogResult zipDialogLog;

                //ダイアログの説明
                fdDialog_D.Description = "Unityファイルを指定";
                //デフォルトフォルダ
                fdDialog_D.SelectedPath = Environment.CurrentDirectory;

                //フォルダ選択ダイアログ表示
                if (fdDialog_D.ShowDialog() == DialogResult.OK)
                {
                    //Unityファイルか確認
                    if (!Directory.Exists($"{fdDialog_D.SelectedPath}/Assets"))
                    {
                        MessageBox.Show("Unityファイルを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        continue;
                    }

                    //ダイアログの説明
                    fdDialog.Description = "コピー先を指定";
                    //デフォルトフォルダ
                    fdDialog.SelectedPath = @"C:";
                    //新しいフォルダ作成のボタン表示
                    fdDialog.ShowNewFolderButton = true;
                    //フォルダ選択ダイアログ表示
                    if (fdDialog.ShowDialog() == DialogResult.OK)
                    {
                        //OKがおされた時



                        //ファイル数を取得
                        int tempCount = Directory.GetFiles($"{fdDialog_D.SelectedPath}/Assets", "*", SearchOption.AllDirectories).Length;
                        int _tempCount = Directory.GetFiles($"{fdDialog_D.SelectedPath}/ProjectSettings", "*", SearchOption.AllDirectories).Length;
                        int tempCurrentCount = Directory.GetFiles(Environment.CurrentDirectory, "*", SearchOption.AllDirectories).Length;
                        int fileCount = tempCount + _tempCount + tempCurrentCount;

                        //ダイアログのタイトルを設定
                        pd.Title = "読込中...";
                        //プログレスバーの最小値を設定
                        pd.Minimum = 0;
                        //プログレスバーの最大値を設定
                        pd.Maximum = fileCount;
                        //プログレスバーの初期値を設定
                        pd.Value = 0;
                        //進行状況ダイアログを表示する
                        pd.Show();

                        //日付取得
                        DateTime dt = DateTime.Now;
                        String result = dt.ToString("yyyy.MM.dd");
                        //連番取得
                        int Directry = 1;
                        var targetPath = $"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA";
                        while (Directory.Exists(targetPath))
                        {
                            targetPath = $"{@fdDialog.SelectedPath}/{result}({++Directry})_UnityBA";
                        }
                        var targetPath_ = $"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA.zip";
                        while (Directory.Exists(targetPath_))
                        {
                            targetPath_ = $"{@fdDialog.SelectedPath}/{result}({++Directry})_UnityBA.zip";
                        }
                        //日付、連番を付けた新規ファイル作成
                        DirectoryInfo di = Directory.CreateDirectory($"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA");
                        //ファイルのパスを保存
                        fileName = $"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA";

                        //ファイルをコピー
                        DirectoryCopy($"{fdDialog_D.SelectedPath}/Assets", $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/Assets");
                        DirectoryCopy($"{fdDialog_D.SelectedPath}/ProjectSettings", $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/ProjectSettings");
                        DirectoryCopy("Seet", $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp/Seet");
                        DirectoryCopy("Seet", $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp");
                        // FileInfoのインスタンスを生成する
                        FileInfo fileInfo = new FileInfo($"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp/System_File.sys");
                        // 移動後のファイル名（パス）
                        string destFileName = $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp/UnityBackUp.exe";
                        // ファイルを移動する
                        fileInfo.MoveTo(destFileName);

                        //進行ダイアログを閉じる
                        pd.Close();

                        MessageBox.Show("コピーが完了しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        zipDialog = MessageBox.Show("続けてzipファイルを生成しますか？", "zipの生成", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        if (zipDialog == DialogResult.OK)
                        {
                            zipDialogLog = MessageBox.Show("高圧縮モードで圧縮します(*いいえを選択すると高速低圧縮モードになります)", "zipの生成", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (zipDialogLog == DialogResult.Yes)
                            {

                                UpdateZipProgress($"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA", $"{ @fdDialog.SelectedPath}/{ result}({ Directry})_UnityBA.zip", false);

                            }
                            else if (zipDialogLog == DialogResult.No)
                            {
                                try
                                {
                                    //Zipに圧縮
                                    UpdateZipProgress($"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA", $"{ @fdDialog.SelectedPath}/{ result}({ Directry})_UnityBA.zip", false);
                                }
                                catch (System.IO.IOException)
                                {
                                    //進行ダイアログを閉じる
                                    zippd.Close();
                                    MessageBox.Show("同じ名前のzipファイルがあります", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    Application.Exit();
                                }
                            }
                            if (zipDialogLog == DialogResult.Cancel)
                            {
                                MessageBox.Show("キャンセルされました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            }
                            else if (zipDialogLog != DialogResult.Cancel)
                            {
                                //Zipの元ファイルを削除
                                DirectoryDelete($"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA");
                                //進行ダイアログを閉じる
                                zippd.Close();
                                MessageBox.Show("Zipが生成されました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("キャンセルされました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    //オブジェクト破棄
                    fdDialog.Dispose();
                }
                else
                {
                    MessageBox.Show("キャンセルされました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    fdDialog_D.Dispose();
                }

                //オブジェクト破棄
                fdDialog_D.Dispose();

                break;
            }
        }
        //ディレクトリのコピー
        public static void DirectoryCopy(string sourcePath, string destinationPath)
        {
            Application.DoEvents();

            DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
            DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);

            //コピー先のディレクトリがなければ作成する
            if (destinationDirectory.Exists == false)
            {
                destinationDirectory.Create();
                destinationDirectory.Attributes = sourceDirectory.Attributes;
            }

            //ファイルのコピー
            foreach (FileInfo fileInfo in sourceDirectory.GetFiles())
            {
                //キャンセルされたら終了(エラー吐く)
                if (pd.Canceled)
                    Application.Exit();

                //同じファイルは常に上書き
                fileInfo.CopyTo(destinationDirectory.FullName + @"\" + fileInfo.Name, true);
            }

            //ディレクトリのコピー（再帰）
            foreach (System.IO.DirectoryInfo directoryInfo in sourceDirectory.GetDirectories())
            {
                DirectoryCopy(directoryInfo.FullName, destinationDirectory.FullName + @"\" + directoryInfo.Name);
            }
            pd.Value = Directory.GetFiles(fileName, "*", SearchOption.AllDirectories).Length;

        }
        //ディレクトリーの削除
        public static void DirectoryDelete(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            //ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                DirectoryDelete(directoryPath);
            }
            zippd.Value = zippd.Maximum - Directory.GetFiles(targetDirectoryPath, "*", SearchOption.AllDirectories).Length;
            //中が空になったらディレクトリ自身も削除
            Directory.Delete(targetDirectoryPath, false);
        }

        //ZipProgress更新
        public static void UpdateZipProgress(string sourcePath, string destinationPath, bool compression)
        {
            Application.DoEvents();

            DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
            DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);

            bool OnZip = false;

            //ファイル数取得
            int fileCount = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories).Length;

            // ダイアログのタイトルを設定
            zippd.Title = "Zipを生成中...";
            //プログレスバーの最小値を設定
            zippd.Minimum = 0;
            //プログレスバーの最大値を設定
            zippd.Maximum = fileCount;
            //プログレスバーの初期値を設定
            zippd.Value = 0;
            //進行状況ダイアログを表示する
            zippd.Show();

            ZipArchive archive = null;

            while (true) 
            {
                if (OnZip)
                {
                    OnZip = false;
                    try
                    {
                        if (compression)
                            //Zipに圧縮
                            ZipFile.CreateFromDirectory(sourcePath, destinationPath, CompressionLevel.Fastest, false);
                        else
                            //Zipに圧縮
                            ZipFile.CreateFromDirectory(sourcePath, destinationPath, CompressionLevel.Optimal, false);

                        archive = ZipFile.OpenRead(destinationPath);
                    }
                    catch (System.IO.IOException)
                    {
                        //進行ダイアログを閉じる
                        zippd.Close();
                        MessageBox.Show("同じ名前のzipファイルがあります", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Application.Exit();
                    }
                }
                else
                {
                    if (archive != null)
                        zippd.Value = archive.Entries.Count;
                }

            }
        }
    }
}
