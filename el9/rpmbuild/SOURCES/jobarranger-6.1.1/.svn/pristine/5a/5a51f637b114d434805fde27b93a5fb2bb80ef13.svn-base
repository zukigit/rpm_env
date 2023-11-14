## Process this file with automake to produce Makefile.in

bin_PROGRAMS = jobarg_joblogput

jobarg_joblogput_SOURCES = jobarg_joblogput.c

jobarg_joblogput_CFLAGS = -I@top_srcdir@/jainclude/

jobarg_joblogput_LDADD = \
	$(top_srcdir)/src/jalibs/libjacommon.a \
	$(top_srcdir)/src/libs/zbxcomms/libzbxcomms.a \
	$(top_srcdir)/src/libs/zbxcommon/libzbxcommon.a \
	$(top_srcdir)/src/libs/zbxlog/libzbxlog.a \
	$(top_srcdir)/src/libs/zbxsys/libzbxsys.a \
	$(top_srcdir)/src/libs/zbxcrypto/libzbxcrypto.a \
	$(top_srcdir)/src/libs/zbxconf/libzbxconf.a \
	$(top_srcdir)/src/libs/zbxjson/libzbxjson.a
