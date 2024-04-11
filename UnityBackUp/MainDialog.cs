using System;
using System.IO;
using System.Windows.Forms;
using Ionic.Zip;
using Ionic.Zlib;
using System.Text;

namespace UnityBackUp
{
    public partial class UnityBackUpSystem : Form
    {
        static ProgressDialog pd = new ProgressDialog();
        static ProgressDialog zippd = new ProgressDialog();
        static ProgressDialog AnLockzippd = new ProgressDialog();
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
                        int t_tempCount = Directory.GetFiles($"{fdDialog_D.SelectedPath}/Packages", "*", SearchOption.AllDirectories).Length;
                        int tempCurrentCount = Directory.GetFiles(Environment.CurrentDirectory, "*", SearchOption.AllDirectories).Length;
                        int fileCount = tempCount + _tempCount + tempCurrentCount + t_tempCount;

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

                        pd.Message = "コピーが開始されました";

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
                        DirectoryCopy($"{fdDialog_D.SelectedPath}/Packages", $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/Packages");
                        //DirectoryCopy("Seet", $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp/Seet");
                        //DirectoryCopy("Seet", $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp");
                        DirectoryCopy(Directory.GetCurrentDirectory(), $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp");
                        //// FileInfoのインスタンスを生成する
                        //FileInfo fileInfo = new FileInfo($"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp/System_File.sys");
                        //// 移動後のファイル名（パス）
                        //string destFileName = $"{fdDialog.SelectedPath}/{result}({Directry})_UnityBA/UnityBackUp/UnityBackUp.exe";
                        //// ファイルを移動する
                        //fileInfo.MoveTo(destFileName);

                        //進行ダイアログを閉じる
                        pd.Close();

                        MessageBox.Show("コピーが完了しました。", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        zipDialog = MessageBox.Show("続けてzipファイルを生成しますか？", "zipの生成", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        if (zipDialog == DialogResult.OK)
                        {
                            //Zipに圧縮
                            CreateZip($"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA", $"{ @fdDialog.SelectedPath}/{ result}({ Directry})_UnityBA.zip");

                            zippd.Message = "元データを削除中…";
                            //Zipの元ファイルを削除
                            DirectoryDelete($"{@fdDialog.SelectedPath}/{result}({Directry})_UnityBA");
                            //進行ダイアログを閉じる
                            zippd.Close();
                            MessageBox.Show("Zipが生成されました。", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }
                    else
                    {
                        MessageBox.Show("キャンセルされました。", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    //オブジェクト破棄
                    fdDialog.Dispose();
                }
                else
                {
                    MessageBox.Show("キャンセルされました。", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    fdDialog_D.Dispose();
                }

                //オブジェクト破棄
                fdDialog_D.Dispose();

                break;
            }
            Application.Restart();
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
                //キャンセルされたら終了
                if (pd.Canceled)
                    Application.Restart();

                //同じファイルは常に上書き
                fileInfo.CopyTo(destinationDirectory.FullName + @"\" + fileInfo.Name, true);

                pd.Message = $"{fileInfo.Name}をコピーしています";
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
            //中が空になったらディレクトリ自身も削除
            Directory.Delete(targetDirectoryPath, false);
        }

        //Zipの生成
        public void CreateZip(string sourcePath, string destinationPath)
        {

            // ダイアログのタイトルを設定
            zippd.Title = "Zipを生成中...";
            //プログレスバーの最小値を設定
            zippd.Minimum = 0;
            //プログレスバーの最大値を設定
            zippd.Maximum = 100;
            //プログレスバーの初期値を設定
            zippd.Value = 0;
            //進行状況ダイアログを表示する
            zippd.Show();
            try
            {
                using (var zip = new ZipFile(Encoding.GetEncoding("shift_jis")))
                {
                    //大きなファイル用
                    zip.UseZip64WhenSaving = Zip64Option.AsNecessary;

                    // 圧縮レベル
                    zip.CompressionLevel = CompressionLevel.BestCompression;

                    //SaveProgressイベントハンドラを追加
                    zip.SaveProgress += this.zip_SaveProgress;

                    //ディレクトリの追加
                    zip.AddDirectory(sourcePath);
                    ////ZIPファイルを保存
                    //zip.Save(destinationPath);

                    zip.SaveSelfExtractor(destinationPath,
                        SelfExtractorFlavor.WinFormsApplication);
                }
            }
            catch (IOException)
            {
                //進行ダイアログを閉じる
                //zippd.Close();
                MessageBox.Show("同じ名前のzipファイルがあります", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Application.Restart();
            }

        }
        //SaveProgressイベントハンドラ
        private void zip_SaveProgress(
            object sender, Ionic.Zip.SaveProgressEventArgs e)
        {
            if (e.EventType ==
                ZipProgressEventType.Saving_Started)
            {
                //書庫の作成を開始
                zippd.Message = $"{e.ArchiveName}の作成開始\n";
            }
            else if (e.EventType ==
                ZipProgressEventType.Saving_BeforeWriteEntry)
            {
                //エントリの書き込み開始
                zippd.Message = $"{e.CurrentEntry.FileName}の書き込み開始\n";
                zippd.Maximum = e.EntriesTotal;

                //Zip作成Cancel
                if (zippd.Canceled)
                {
                    e.Cancel = true;
                    CancelMessage();
                }
            }
            else if (e.EventType ==
                ZipProgressEventType.Saving_EntryBytesRead)
            {
                //エントリを書き込み中
                zippd.Message = $"{e.BytesTransferred}/{e.TotalBytesToTransfer} バイト 書き込み完了\n";
            }
            else if (e.EventType ==
                ZipProgressEventType.Saving_AfterWriteEntry)
            {
                //エントリの書き込み終了
                zippd.Message = $"{e.CurrentEntry.FileName} の書き込み終了\n";
                zippd.Message = $"{e.EntriesTotal} 個中 {e.EntriesSaved} 個のエントリの書き込みが完了しました\n";
                zippd.Value = e.EntriesSaved;
            }
            else if (e.EventType ==
                ZipProgressEventType.Saving_Completed)
            {
                //書庫の作成が完了
                zippd.Message = $"{e.ArchiveName} の作成終了\n";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ダイアログの作成
            FolderBrowserDialog LockDialog_D = new FolderBrowserDialog();
            FolderBrowserDialog LockDialog = new FolderBrowserDialog();

                //ダイアログの説明
                LockDialog_D.Description = "圧縮するファイルを指定";
                //デフォルトフォルダ
                LockDialog_D.SelectedPath = @"C:";

            //フォルダ選択ダイアログ表示
            if (LockDialog_D.ShowDialog() == DialogResult.OK)
            {
                //ダイアログの説明
                LockDialog.Description = "圧縮先を指定";
                //デフォルトフォルダ
                LockDialog.SelectedPath = @"C:";
                //新しいフォルダ作成のボタン表示
                LockDialog.ShowNewFolderButton = true;
                //フォルダ選択ダイアログ表示
                if (LockDialog.ShowDialog() == DialogResult.OK)
                {
                    string name = Path.GetFileName(LockDialog_D.SelectedPath);
                    CreateZip(@LockDialog_D.SelectedPath, $"{ @LockDialog.SelectedPath}/{name}.zip");
                    zippd.Close();
                    Application.Restart();
                }
                else
                    CancelMessage();
            }
            else
                CancelMessage();
        }
        public void CancelMessage()
        {
            MessageBox.Show("キャンセルされました", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            Application.Restart();

        }
    }
}