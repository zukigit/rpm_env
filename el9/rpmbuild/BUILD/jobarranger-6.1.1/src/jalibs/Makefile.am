## Process this file with automake to produce Makefile.in

noinst_LIBRARIES = libjacommon.a libjaserver.a

libjacommon_a_CFLAGS = -I@top_srcdir@/jainclude/
libjacommon_a_SOURCES = \
	jastr.c \
	jahostname.c \
	jakill.c \
	jajobobject.c \
	jafile.c \
	jatar.c \
	jajobfile.c \
	jatcp.c \
	jathreads.c \
	jatelegram.c

if SERVER
libjaserver_a_CFLAGS = -I@top_srcdir@/jainclude/
libjaserver_a_SOURCES = \
	jalog.c \
	jajoblog.c \
	javalue.c \
	jatimeout.c \
	jastatus.c \
	jaself.c \
	jaindex.c \
	jaflow.c \
	jaenv.c \
	jahost.c \
	jauser.c \
	jaconnect.c \
	jaschedule.c \
	jajobnet.c \
	jadb.c \
	jazbxmsg.c \
	jajobid.c
endif
