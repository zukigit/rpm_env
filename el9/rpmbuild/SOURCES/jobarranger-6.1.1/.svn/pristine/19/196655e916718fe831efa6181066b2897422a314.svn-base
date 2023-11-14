## Process this file with automake to produce Makefile.in

client_MANS = \
	jobarg_exec.man \
	jobarg_get.man \
	jobarg_joblogput.man \
	jobarg_release.man

if AGENT
agent_MANS = jobarg_agentd.man
endif

if SERVER
server_MANS = jobarg_server.man
endif

man1_MANS = $(client_MANS)
man8_MANS = $(agent_MANS) $(server_MANS)

EXTRA_DIST = \
	jobarg_agentd.man \
	jobarg_exec.man \
	jobarg_get.man \
	jobarg_joblogput.man \
	jobarg_release.man \
	jobarg_server.man
