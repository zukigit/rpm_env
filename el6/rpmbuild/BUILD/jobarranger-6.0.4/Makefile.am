## Process this file with automake to produce Makefile.in

ACLOCAL_AMFLAGS = -I m4

SUBDIRS = \
	jaconf \
	man \
	src

EXTRA_DIST = \
	frontend \
	database \
	jaconf \
	jainclude \
	include \
	misc \
	build \
	bin \
	m4 \
	3rd_party_licenses

dist-hook:
	@jobarg_revision=`LANG=C; svn info|grep "Last Changed Rev"|awk '{print $$4;}'`; \
	cat $(top_distdir)/include/version.h|sed "s/{ZABBIX_REVISION}/$$jobarg_revision/g" > $(top_distdir)/include/version.h.new; \
	mv $(top_distdir)/include/version.h.new $(top_distdir)/include/version.h; \
	cat $(top_distdir)/jainclude/jacommon.h|sed "s/{JOBARG_REVISION}/$$jobarg_revision/g" > $(top_distdir)/jainclude/jacommon.h.new; \
        mv $(top_distdir)/jainclude/jacommon.h.new $(top_distdir)/jainclude/jacommon.h;

	@jobarg_revdate=`LANG=C; svn info|grep "Last Changed Date"|awk '{print $$4;}'`; \
	cat $(top_distdir)/include/version.h|sed "s/{ZABBIX_REVDATE}/$$jobarg_revdate/g" > $(top_distdir)/include/version.h.new; \
	mv $(top_distdir)/include/version.h.new $(top_distdir)/include/version.h; \
	cat $(top_distdir)/jainclude/jacommon.h|sed "s/{JOBARG_REVDATE}/$$jobarg_revdate/g" > $(top_distdir)/jainclude/jacommon.h.new; \
	mv $(top_distdir)/jainclude/jacommon.h.new $(top_distdir)/jainclude/jacommon.h;
	
	rm -rf `find $(top_distdir) -name ".svn"`
	rm -f $(top_distdir)/include/config.h
