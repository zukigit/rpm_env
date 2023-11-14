################################################################################
# Automatically-generated file. Do not edit!
################################################################################

-include ../makefile.init

RM := rm -rf

# All of the sources participating in the build are defined here
-include sources.mk
-include tools/sqlite3/tea/win/subdir.mk
-include tools/sqlite3/tea/generic/subdir.mk
-include tools/sqlite3/subdir.mk
-include tools/libtar/libtar/win32/subdir.mk
-include tools/libtar/libtar/listhash/subdir.mk
-include tools/libtar/libtar/lib/subdir.mk
-include tools/libtar/libtar/compat/subdir.mk
-include tools/libtar/alttar/subdir.mk
-include tools/libarchive/tar/test/subdir.mk
-include tools/libarchive/tar/subdir.mk
-include tools/libarchive/libarchive_fe/subdir.mk
-include tools/libarchive/libarchive/test/subdir.mk
-include tools/libarchive/libarchive/subdir.mk
-include tools/libarchive/examples/minitar/subdir.mk
-include tools/libarchive/examples/subdir.mk
-include tools/libarchive/cpio/test/subdir.mk
-include tools/libarchive/cpio/subdir.mk
-include tools/libarchive/contrib/shar/subdir.mk
-include tools/libarchive/contrib/subdir.mk
-include tools/libarchive/build/cmake/subdir.mk
-include tools/json/subdir.mk
-include tools/createprocesswithlogin/subdir.mk
-include src/libs/zbxwin32/subdir.mk
-include src/libs/zbxsysinfo/win32/subdir.mk
-include src/libs/zbxsysinfo/unknown/subdir.mk
-include src/libs/zbxsysinfo/solaris/subdir.mk
-include src/libs/zbxsysinfo/simple/subdir.mk
-include src/libs/zbxsysinfo/osx/subdir.mk
-include src/libs/zbxsysinfo/osf/subdir.mk
-include src/libs/zbxsysinfo/openbsd/subdir.mk
-include src/libs/zbxsysinfo/netbsd/subdir.mk
-include src/libs/zbxsysinfo/linux/subdir.mk
-include src/libs/zbxsysinfo/hpux/subdir.mk
-include src/libs/zbxsysinfo/freebsd/subdir.mk
-include src/libs/zbxsysinfo/common/subdir.mk
-include src/libs/zbxsysinfo/aix/subdir.mk
-include src/libs/zbxsysinfo/agent/subdir.mk
-include src/libs/zbxsysinfo/subdir.mk
-include src/libs/zbxsys/subdir.mk
-include src/libs/zbxserver/subdir.mk
-include src/libs/zbxself/subdir.mk
-include src/libs/zbxnix/subdir.mk
-include src/libs/zbxmemory/subdir.mk
-include src/libs/zbxmedia/subdir.mk
-include src/libs/zbxlog/subdir.mk
-include src/libs/zbxjson/subdir.mk
-include src/libs/zbxicmpping/subdir.mk
-include src/libs/zbxexec/subdir.mk
-include src/libs/zbxdbhigh/subdir.mk
-include src/libs/zbxdbcache/subdir.mk
-include src/libs/zbxdb/subdir.mk
-include src/libs/zbxcrypto/subdir.mk
-include src/libs/zbxconf/subdir.mk
-include src/libs/zbxcommshigh/subdir.mk
-include src/libs/zbxcomms/subdir.mk
-include src/libs/zbxcommon/subdir.mk
-include src/libs/zbxalgo/subdir.mk
-include src/jobarg_session/subdir.mk
-include src/jobarg_server/jatrapper/subdir.mk
-include src/jobarg_server/jaselfmon/subdir.mk
-include src/jobarg_server/jarun/subdir.mk
-include src/jobarg_server/jamsgsnd/subdir.mk
-include src/jobarg_server/jaloader/subdir.mk
-include src/jobarg_server/jajobnet/subdir.mk
-include src/jobarg_server/jajob/subdir.mk
-include src/jobarg_server/jaboot/subdir.mk
-include src/jobarg_server/subdir.mk
-include src/jobarg_release/subdir.mk
-include src/jobarg_monitor/subdir.mk
-include src/jobarg_joblogput/subdir.mk
-include src/jobarg_get/subdir.mk
-include src/jobarg_exec/subdir.mk
-include src/jobarg_command/subdir.mk
-include src/jobarg_agent/subdir.mk
-include src/jalibs/subdir.mk
-include build/project/json2/Release/subdir.mk
-include subdir.mk
-include objects.mk

ifneq ($(MAKECMDGOALS),clean)
ifneq ($(strip $(C_DEPS)),)
-include $(C_DEPS)
endif
endif

-include ../makefile.defs

# Add inputs and outputs from these tool invocations to the build variables 

# All Target
all: job-arranger

# Tool invocations
job-arranger: $(OBJS) $(USER_OBJS)
	@echo 'Building target: $@'
	@echo 'Invoking: Cross GCC Linker'
	gcc  -o "job-arranger" $(OBJS) $(USER_OBJS) $(LIBS)
	@echo 'Finished building target: $@'
	@echo ' '

# Other Targets
clean:
	-$(RM) $(OBJS)$(C_DEPS)$(EXECUTABLES) job-arranger
	-@echo ' '

.PHONY: all clean dependents
.SECONDARY:

-include ../makefile.targets
