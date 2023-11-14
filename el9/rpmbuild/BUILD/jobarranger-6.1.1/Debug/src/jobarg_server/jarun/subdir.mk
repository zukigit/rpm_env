################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/jobarg_server/jarun/jarun.c \
../src/jobarg_server/jarun/jarunagent.c \
../src/jobarg_server/jarun/jaruniconcalc.c \
../src/jobarg_server/jarun/jaruniconcommon.c \
../src/jobarg_server/jarun/jaruniconend.c \
../src/jobarg_server/jarun/jaruniconextjob.c \
../src/jobarg_server/jarun/jaruniconfcopy.c \
../src/jobarg_server/jarun/jaruniconfwait.c \
../src/jobarg_server/jarun/jaruniconif.c \
../src/jobarg_server/jarun/jaruniconinfo.c \
../src/jobarg_server/jarun/jaruniconjob.c \
../src/jobarg_server/jarun/jaruniconjobnet.c \
../src/jobarg_server/jarun/jaruniconless.c \
../src/jobarg_server/jarun/jaruniconreboot.c \
../src/jobarg_server/jarun/jaruniconrelease.c \
../src/jobarg_server/jarun/jarunicontask.c \
../src/jobarg_server/jarun/jaruniconvalue.c \
../src/jobarg_server/jarun/jaruniconzabbixlink.c \
../src/jobarg_server/jarun/jarunnormal.c \
../src/jobarg_server/jarun/jarunskip.c \
../src/jobarg_server/jarun/jarunvalue.c 

OBJS += \
./src/jobarg_server/jarun/jarun.o \
./src/jobarg_server/jarun/jarunagent.o \
./src/jobarg_server/jarun/jaruniconcalc.o \
./src/jobarg_server/jarun/jaruniconcommon.o \
./src/jobarg_server/jarun/jaruniconend.o \
./src/jobarg_server/jarun/jaruniconextjob.o \
./src/jobarg_server/jarun/jaruniconfcopy.o \
./src/jobarg_server/jarun/jaruniconfwait.o \
./src/jobarg_server/jarun/jaruniconif.o \
./src/jobarg_server/jarun/jaruniconinfo.o \
./src/jobarg_server/jarun/jaruniconjob.o \
./src/jobarg_server/jarun/jaruniconjobnet.o \
./src/jobarg_server/jarun/jaruniconless.o \
./src/jobarg_server/jarun/jaruniconreboot.o \
./src/jobarg_server/jarun/jaruniconrelease.o \
./src/jobarg_server/jarun/jarunicontask.o \
./src/jobarg_server/jarun/jaruniconvalue.o \
./src/jobarg_server/jarun/jaruniconzabbixlink.o \
./src/jobarg_server/jarun/jarunnormal.o \
./src/jobarg_server/jarun/jarunskip.o \
./src/jobarg_server/jarun/jarunvalue.o 

C_DEPS += \
./src/jobarg_server/jarun/jarun.d \
./src/jobarg_server/jarun/jarunagent.d \
./src/jobarg_server/jarun/jaruniconcalc.d \
./src/jobarg_server/jarun/jaruniconcommon.d \
./src/jobarg_server/jarun/jaruniconend.d \
./src/jobarg_server/jarun/jaruniconextjob.d \
./src/jobarg_server/jarun/jaruniconfcopy.d \
./src/jobarg_server/jarun/jaruniconfwait.d \
./src/jobarg_server/jarun/jaruniconif.d \
./src/jobarg_server/jarun/jaruniconinfo.d \
./src/jobarg_server/jarun/jaruniconjob.d \
./src/jobarg_server/jarun/jaruniconjobnet.d \
./src/jobarg_server/jarun/jaruniconless.d \
./src/jobarg_server/jarun/jaruniconreboot.d \
./src/jobarg_server/jarun/jaruniconrelease.d \
./src/jobarg_server/jarun/jarunicontask.d \
./src/jobarg_server/jarun/jaruniconvalue.d \
./src/jobarg_server/jarun/jaruniconzabbixlink.d \
./src/jobarg_server/jarun/jarunnormal.d \
./src/jobarg_server/jarun/jarunskip.d \
./src/jobarg_server/jarun/jarunvalue.d 


# Each subdirectory must supply rules for building sources it contributes
src/jobarg_server/jarun/%.o: ../src/jobarg_server/jarun/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


