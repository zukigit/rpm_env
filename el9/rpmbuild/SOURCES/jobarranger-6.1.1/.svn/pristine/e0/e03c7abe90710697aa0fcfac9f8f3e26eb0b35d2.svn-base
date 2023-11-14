## Process this file with automake to produce Makefile.in

DIST_SUBDIRS = \
	libs \
	jalibs \
	jobarg_exec \
	jobarg_get \
	jobarg_release \
	jobarg_joblogput \
	jobarg_session \
	jobarg_monitor \
	jobarg_command \
	jobarg_agent \
	jobarg_server

if AGENT
AGENT_SUBDIRS = \
	jobarg_command \
	jobarg_agent
endif

if SERVER
SERVER_SUBDIRS = \
	jobarg_command \
	jobarg_session \
	jobarg_monitor \
	jobarg_server 
endif

SUBDIRS = \
	libs \
	jalibs \
	jobarg_exec \
	jobarg_get \
	jobarg_release \
	jobarg_joblogput \
	$(AGENT_SUBDIRS) \
	$(SERVER_SUBDIRS)
