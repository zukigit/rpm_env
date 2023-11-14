#include <stdio.h>
#include <Windows.h>
#include <tchar.h>
#include <Msiquery.h>


#include "Installer.h"

using namespace std;

#define DEF_DIR_PATH		1024
#define DEF_COMMANDO_LEN	2048
#define DEF_SERVICE_NAME	_T( "Job Arranger Agent" )

namespace Installer
{
	// インストールコミット時に動作する処理
	void __stdcall InstallerCommit( MSIHANDLE hInstall )
	{
		TCHAR w_aDirPathBuff[DEF_DIR_PATH]		= { 0 };
		TCHAR w_aDirPath[DEF_DIR_PATH]			= { 0 };
		TCHAR w_aCommandStr[DEF_COMMANDO_LEN]	= { 0 };
		DWORD w_dDirPathSize					= DEF_DIR_PATH;
		SHELLEXECUTEINFO w_ShellExecInfo		= { 0 };

		// インストーラからインストール先ディレクトリのパス取得
		MsiGetProperty( hInstall, _T( "CustomActionData" ), w_aDirPathBuff, &w_dDirPathSize );
		// 末尾の\削除
		_tcsncpy_s( w_aDirPath, DEF_DIR_PATH, w_aDirPathBuff, _tcslen( w_aDirPathBuff ) -1 );

		// コマンドプロンプト実行時のパラメータ設定
		w_ShellExecInfo.cbSize	= sizeof( SHELLEXECUTEINFO );
		w_ShellExecInfo.nShow	= SW_HIDE;
		w_ShellExecInfo.fMask	= SEE_MASK_NOCLOSEPROCESS;
		w_ShellExecInfo.lpFile	= _T( "cmd.exe" );
		// インストール先ディレクトリ配下にフルコントロールのアクセス権限付与
		_sntprintf_s( w_aCommandStr, DEF_COMMANDO_LEN, _TRUNCATE,
			_T( "/c cacls \"%s\" /t /e /g users:f" ), w_aDirPath );
		w_ShellExecInfo.lpParameters = w_aCommandStr;

		// 実行
		ShellExecuteEx( &w_ShellExecInfo );
		// 終了待ち
		WaitForSingleObject( w_ShellExecInfo.hProcess, INFINITE );
	}

	// アンインストール時に動作する処理
	void __stdcall UnInstall( MSIHANDLE hInstall )
	{
		TCHAR w_aCommandStr[DEF_COMMANDO_LEN]	= { 0 };
		SHELLEXECUTEINFO w_ShellExecInfo		= { 0 };

		// コマンドプロンプト実行時のパラメータ設定
		w_ShellExecInfo.cbSize	= sizeof( SHELLEXECUTEINFO );
		w_ShellExecInfo.nShow	= SW_HIDE;
		w_ShellExecInfo.fMask	= SEE_MASK_NOCLOSEPROCESS;
		w_ShellExecInfo.lpFile	= _T( "cmd.exe" );
		// サービスの停止
		_sntprintf_s( w_aCommandStr, DEF_COMMANDO_LEN, _TRUNCATE,
			_T( "/c net stop \"%s\"" ), DEF_SERVICE_NAME );
		w_ShellExecInfo.lpParameters = w_aCommandStr;

		// 実行
		ShellExecuteEx( &w_ShellExecInfo );
		// 終了待ち
		WaitForSingleObject( w_ShellExecInfo.hProcess, INFINITE );
	}
}
