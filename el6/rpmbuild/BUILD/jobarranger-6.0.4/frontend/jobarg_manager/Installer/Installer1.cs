using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Installer
{
    // インストーラクラス
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        // インストール時に実行される処理
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        // インストール確定時に実行される処理
        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);

            // インストール先のパス取得(末尾に\\が付いているので削除)
            string w_InstDir = this.Context.Parameters["instdir"];
            w_InstDir = w_InstDir.Substring(0, w_InstDir.Length - 2);

            // コマンドプロンプト起動パラメータ設定
            System.Diagnostics.Process w_Process = new System.Diagnostics.Process();
            w_Process.StartInfo.FileName = "cmd.exe";
            w_Process.StartInfo.CreateNoWindow = true;
            w_Process.StartInfo.UseShellExecute = false;

            // usersにフルコントロール権限を当たるコマンド設定
            w_Process.StartInfo.Arguments = "/c cacls \"" + w_InstDir + "\" /t /e /g users:f";

            // コマンドプロンプト起動
            w_Process.Start();
            w_Process.WaitForExit();
            w_Process.Close();
        }

        // インストール失敗時のロールバック時に実行される処理
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        // アンインストール時に実行される処理
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }
    }
}
