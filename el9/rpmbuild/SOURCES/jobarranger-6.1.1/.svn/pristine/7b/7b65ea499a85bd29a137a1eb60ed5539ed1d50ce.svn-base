## Process this file with automake to produce Makefile.in

sbin_PROGRAMS = jobarg_monitor

jobarg_monitor_SOURCES = jobarg_monitor.c

jobarg_monitor_LDADD = \
	$(top_srcdir)/src/jalibs/libjacommon.a \
	$(top_srcdir)/src/jalibs/libjaserver.a \
	$(top_srcdir)/src/libs/zbxdbhigh/libzbxdbhigh.a \
	$(top_srcdir)/src/libs/zbxdbcache/libzbxdbcache.a \
	$(top_srcdir)/src/libs/zbxdbhigh/libzbxdbhigh.a \
	$(top_srcdir)/src/libs/zbxserver/libzbxserver.a \
	$(top_srcdir)/src/libs/zbxsysinfo/libzbxserversysinfo.a \
	$(top_srcdir)/src/libs/zbxsysinfo/common/libcommonsysinfo.a \
	$(top_srcdir)/src/libs/zbxlog/libzbxlog.a \
	$(top_srcdir)/src/libs/zbxmemory/libzbxmemory.a \
	$(top_srcdir)/src/libs/zbxalgo/libzbxalgo.a \
	$(top_srcdir)/src/libs/zbxnix/libzbxnix.a \
	$(top_srcdir)/src/libs/zbxsys/libzbxsys.a \
	$(top_srcdir)/src/libs/zbxconf/libzbxconf.a \
	$(top_srcdir)/src/libs/zbxsysinfo/simple/libsimplesysinfo.a \
	$(top_srcdir)/src/libs/zbxcomms/libzbxcomms.a \
	$(top_srcdir)/src/libs/zbxcommon/libzbxcommon.a \
	$(top_srcdir)/src/libs/zbxcrypto/libzbxcrypto.a \
	$(top_srcdir)/src/libs/zbxexec/libzbxexec.a \
	$(top_srcdir)/src/libs/zbxjson/libzbxjson.a \
	$(top_srcdir)/src/libs/zbxdb/libzbxdb.a

jobarg_monitor_LDADD += @SERVER_LIBS@

jobarg_monitor_LDFLAGS = @SERVER_LDFLAGS@

jobarg_monitor_CFLAGS = \
	-I@top_srcdir@/jainclude/ \
	-DZABBIX_DAEMON \
	-DSYSCONFDIR="\"@sysconfdir@/jobarranger\""

CONFIG_FILE = jobarg_monitor.conf
CONFIG_DIR = $(DESTDIR)@sysconfdir@/jobarranger

install-data-hook:
	$(MKDIR_P) "$(CONFIG_DIR)"
	test -f "$(CONFIG_DIR)/$(CONFIG_FILE)" || cp "../../jaconf/$(CONFIG_FILE)" "$(CONFIG_DIR)"
