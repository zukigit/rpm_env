################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsysinfo/aix/aix.c \
../src/libs/zbxsysinfo/aix/cpu.c \
../src/libs/zbxsysinfo/aix/diskio.c \
../src/libs/zbxsysinfo/aix/diskspace.c \
../src/libs/zbxsysinfo/aix/inodes.c \
../src/libs/zbxsysinfo/aix/memory.c \
../src/libs/zbxsysinfo/aix/net.c \
../src/libs/zbxsysinfo/aix/proc.c \
../src/libs/zbxsysinfo/aix/uptime.c \
../src/libs/zbxsysinfo/aix/vmstats.c 

OBJS += \
./src/libs/zbxsysinfo/aix/aix.o \
./src/libs/zbxsysinfo/aix/cpu.o \
./src/libs/zbxsysinfo/aix/diskio.o \
./src/libs/zbxsysinfo/aix/diskspace.o \
./src/libs/zbxsysinfo/aix/inodes.o \
./src/libs/zbxsysinfo/aix/memory.o \
./src/libs/zbxsysinfo/aix/net.o \
./src/libs/zbxsysinfo/aix/proc.o \
./src/libs/zbxsysinfo/aix/uptime.o \
./src/libs/zbxsysinfo/aix/vmstats.o 

C_DEPS += \
./src/libs/zbxsysinfo/aix/aix.d \
./src/libs/zbxsysinfo/aix/cpu.d \
./src/libs/zbxsysinfo/aix/diskio.d \
./src/libs/zbxsysinfo/aix/diskspace.d \
./src/libs/zbxsysinfo/aix/inodes.d \
./src/libs/zbxsysinfo/aix/memory.d \
./src/libs/zbxsysinfo/aix/net.d \
./src/libs/zbxsysinfo/aix/proc.d \
./src/libs/zbxsysinfo/aix/uptime.d \
./src/libs/zbxsysinfo/aix/vmstats.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsysinfo/aix/%.o: ../src/libs/zbxsysinfo/aix/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


