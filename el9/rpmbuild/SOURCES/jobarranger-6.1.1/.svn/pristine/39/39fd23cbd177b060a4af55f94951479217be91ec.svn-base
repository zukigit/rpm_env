## Process this file with automake to produce Makefile.in

sbin_PROGRAMS = jobarg_agentd

jobarg_agentd_SOURCES = \
	jobarg_agentd.c jobarg_agentd.h \
	listener.c listener.h \
	executive.c executive.h \
	jadbbackup.c jadbbackup.h \
	jaagent.c jaagent.h \
	jareboot.c jareboot.h \
	jafcopy.c jafcopy.h \
	jajournal.c jajournal.h \
	jaextjob.c jaextjob.h \
	perfstat.c perfstat.h \
	cpustat.h diskdevices.h stats.h zbxconf.h
	
jobarg_agentd_LDADD = \
	$(top_srcdir)/src/jalibs/libjacommon.a \
	$(top_srcdir)/src/libs/zbxsysinfo/libzbxagentsysinfo.a \
	$(top_srcdir)/src/libs/zbxsysinfo/$(ARCH)/libspecsysinfo.a \
	$(top_srcdir)/src/libs/zbxsysinfo/agent/libagentsysinfo.a \
	$(top_srcdir)/src/libs/zbxsysinfo/common/libcommonsysinfo.a \
	$(top_srcdir)/src/libs/zbxsysinfo/simple/libsimplesysinfo.a \
	$(top_srcdir)/src/libs/zbxlog/libzbxlog.a \
	$(top_srcdir)/src/libs/zbxalgo/libzbxalgo.a \
	$(top_srcdir)/src/libs/zbxsys/libzbxsys.a \
	$(top_srcdir)/src/libs/zbxnix/libzbxnix.a \
	$(top_srcdir)/src/libs/zbxcomms/libzbxcomms.a \
	$(top_srcdir)/src/libs/zbxconf/libzbxconf.a \
	$(top_srcdir)/src/libs/zbxcommon/libzbxcommon.a \
	$(top_srcdir)/src/libs/zbxcrypto/libzbxcrypto.a \
	$(top_srcdir)/src/libs/zbxjson/libzbxjson.a \
	$(top_srcdir)/src/libs/zbxexec/libzbxexec.a

jobarg_agentd_LDADD += @AGENT_LIBS@

jobarg_agentd_LDFLAGS = @AGENT_LDFLAGS@

jobarg_agentd_CFLAGS = \
	-I@top_srcdir@/jainclude/ \
	-DZABBIX_DAEMON \
	-DSYSCONFDIR="\"@sysconfdir@/jobarranger\""

AGENTD_CONFIG = jobarg_agentd.conf
CONFIG_DIR = $(DESTDIR)@sysconfdir@/jobarranger

install-data-hook:
	$(MKDIR_P) "$(CONFIG_DIR)"
	test -f "$(CONFIG_DIR)/$(AGENTD_CONFIG)" || cp "../../jaconf/$(AGENTD_CONFIG)" "$(CONFIG_DIR)"
