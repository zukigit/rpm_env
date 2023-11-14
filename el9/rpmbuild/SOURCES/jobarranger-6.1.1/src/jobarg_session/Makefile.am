## Process this file with automake to produce Makefile.in

bin_PROGRAMS = jobarg_session

jobarg_session_SOURCES = jobarg_session.c

jobarg_session_LDADD = \
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

jobarg_session_LDADD += @SERVER_LIBS@

jobarg_session_LDFLAGS = @SERVER_LDFLAGS@

jobarg_session_CFLAGS = \
	-I@top_srcdir@/jainclude/ \
	-DSYSCONFDIR="\"@sysconfdir@/jobarranger\""

bindir = @sysconfdir@/jobarranger/extendedjob
