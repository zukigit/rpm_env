#include <stdio.h>
#include <Windows.h>
#include <tchar.h>
#include <Msiquery.h>


#include "Installer.h"

using namespace std;

#define DEF_DIR_PATH		1024
#define DEF_COMMANDO_LEN	2048
#define DEF_SERVICE_NAME	_T( "Job Arranger Agent 2" )

namespace Installer
{
	// �C���X�g�[���R�~�b�g���ɓ��삷�鏈��
	void __stdcall InstallerCommit( MSIHANDLE hInstall )
	{
		TCHAR w_aDirPathBuff[DEF_DIR_PATH]		= { 0 };
		TCHAR w_aDirPath[DEF_DIR_PATH]			= { 0 };
		TCHAR w_aCommandStr[DEF_COMMANDO_LEN]	= { 0 };
		DWORD w_dDirPathSize					= DEF_DIR_PATH;
		SHELLEXECUTEINFO w_ShellExecInfo		= { 0 };

		// �C���X�g�[������C���X�g�[����f�B���N�g���̃p�X�擾
		MsiGetProperty( hInstall, _T( "CustomActionData" ), w_aDirPathBuff, &w_dDirPathSize );
		// ������\�폜
		_tcsncpy_s( w_aDirPath, DEF_DIR_PATH, w_aDirPathBuff, _tcslen( w_aDirPathBuff ) -1 );

		// �R�}���h�v�����v�g���s���̃p�����[�^�ݒ�
		w_ShellExecInfo.cbSize	= sizeof( SHELLEXECUTEINFO );
		w_ShellExecInfo.nShow	= SW_HIDE;
		w_ShellExecInfo.fMask	= SEE_MASK_NOCLOSEPROCESS;
		w_ShellExecInfo.lpFile	= _T( "cmd.exe" );
		// �C���X�g�[����f�B���N�g���z���Ƀt���R���g���[���̃A�N�Z�X�����t�^
		_sntprintf_s( w_aCommandStr, DEF_COMMANDO_LEN, _TRUNCATE,
			_T( "/c cacls \"%s\" /t /e /g users:f" ), w_aDirPath );
		w_ShellExecInfo.lpParameters = w_aCommandStr;

		// ���s
		ShellExecuteEx( &w_ShellExecInfo );
		// �I���҂�
		WaitForSingleObject( w_ShellExecInfo.hProcess, INFINITE );
	}

	// �A���C���X�g�[�����ɓ��삷�鏈��
	void __stdcall UnInstall( MSIHANDLE hInstall )
	{
		TCHAR w_aCommandStr[DEF_COMMANDO_LEN]	= { 0 };
		SHELLEXECUTEINFO w_ShellExecInfo		= { 0 };

		// �R�}���h�v�����v�g���s���̃p�����[�^�ݒ�
		w_ShellExecInfo.cbSize	= sizeof( SHELLEXECUTEINFO );
		w_ShellExecInfo.nShow	= SW_HIDE;
		w_ShellExecInfo.fMask	= SEE_MASK_NOCLOSEPROCESS;
		w_ShellExecInfo.lpFile	= _T( "cmd.exe" );
		// �T�[�r�X�̒�~
		_sntprintf_s( w_aCommandStr, DEF_COMMANDO_LEN, _TRUNCATE,
			_T( "/c net stop \"%s\"" ), DEF_SERVICE_NAME );
		w_ShellExecInfo.lpParameters = w_aCommandStr;

		// ���s
		ShellExecuteEx( &w_ShellExecInfo );
		// �I���҂�
		WaitForSingleObject( w_ShellExecInfo.hProcess, INFINITE );
	}
}
