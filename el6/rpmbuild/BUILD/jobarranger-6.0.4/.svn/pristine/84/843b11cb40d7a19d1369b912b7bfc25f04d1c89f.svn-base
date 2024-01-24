## Process this file with automake to produce Makefile.in

SUBDIRS = \
	jaabort \
	jaloader \
	japurge \
	jaboot \
	jajobnet \
	jarun \
	jajob \
	jatrapper \
	jamsgsnd \
	jaselfmon

sbin_PROGRAMS = jobarg_server

jobarg_server_SOURCES = \
	events.c events.h \
	server.c

jobarg_server_LDADD = \
	jaabort/libjaabort.a \
	jaloader/libjaloader.a \
	jaboot/libjaboot.a \
	japurge/libjapurge.a \
	jajobnet/libjajobnet.a \
	jarun/libjarun.a \
	jajob/libjajob.a \
	jatrapper/libjatrapper.a \
	jaselfmon/libjaselfmon.a \
	jamsgsnd/libjamsgsnd.a \
	$(top_srcdir)/src/jalibs/libjacommon.a \
	$(top_srcdir)/src/jalibs/libjaserver.a \
	$(top_srcdir)/src/libs/zbxdbhigh/libzbxdbhigh.a \
	$(top_srcdir)/src/libs/zbxdbcache/libzbxdbcache.a \
	$(top_srcdir)/src/libs/zbxdbhigh/libzbxdbhigh.a \
	$(top_srcdir)/src/libs/zbxserver/libzbxserver.a \
	$(top_srcdir)/src/libs/zbxsysinfo/libzbxserversysinfo.a \
	$(top_srcdir)/src/libs/zbxsysinfo/$(ARCH)/libspecsysinfo.a \
	$(top_srcdir)/src/libs/zbxsysinfo/common/libcommonsysinfo.a \
	$(top_srcdir)/src/libs/zbxlog/libzbxlog.a \
	$(top_srcdir)/src/libs/zbxmemory/libzbxmemory.a \
	$(top_srcdir)/src/libs/zbxalgo/libzbxalgo.a \
	$(top_srcdir)/src/libs/zbxnix/libzbxnix.a \
	$(top_srcdir)/src/libs/zbxsys/libzbxsys.a \
	$(top_srcdir)/src/libs/zbxconf/libzbxconf.a \
	$(top_srcdir)/src/libs/zbxmedia/libzbxmedia.a \
	$(top_srcdir)/src/libs/zbxsysinfo/simple/libsimplesysinfo.a \
	$(top_srcdir)/src/libs/zbxcomms/libzbxcomms.a \
	$(top_srcdir)/src/libs/zbxcommon/libzbxcommon.a \
	$(top_srcdir)/src/libs/zbxcrypto/libzbxcrypto.a \
	$(top_srcdir)/src/libs/zbxcommshigh/libzbxcommshigh.a \
	$(top_srcdir)/src/libs/zbxexec/libzbxexec.a \
	$(top_srcdir)/src/libs/zbxself/libzbxself.a \
	$(top_srcdir)/src/libs/zbxjson/libzbxjson.a \
	$(top_srcdir)/src/libs/zbxicmpping/libzbxicmpping.a \
	$(top_srcdir)/src/libs/zbxdb/libzbxdb.a

jobarg_server_LDADD += @SERVER_LIBS@

jobarg_server_LDFLAGS = @SERVER_LDFLAGS@

jobarg_server_CFLAGS = \
	-I@top_srcdir@/jainclude/ \
	-DZABBIX_DAEMON \
	-DSYSCONFDIR="\"@sysconfdir@/jobarranger\"" \
	-DDATADIR="\"@datadir@\""

CONFIG_FILE = jobarg_server.conf
CONFIG_DIR = $(DESTDIR)@sysconfdir@/jobarranger

install-data-hook:
	$(MKDIR_P) "$(CONFIG_DIR)"
	test -f "$(CONFIG_DIR)/$(CONFIG_FILE)" || cp "../../jaconf/$(CONFIG_FILE)" "$(CONFIG_DIR)"
