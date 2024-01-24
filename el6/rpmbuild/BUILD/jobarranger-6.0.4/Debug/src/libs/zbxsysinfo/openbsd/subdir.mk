################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsysinfo/openbsd/boottime.c \
../src/libs/zbxsysinfo/openbsd/cpu.c \
../src/libs/zbxsysinfo/openbsd/diskio.c \
../src/libs/zbxsysinfo/openbsd/diskspace.c \
../src/libs/zbxsysinfo/openbsd/inodes.c \
../src/libs/zbxsysinfo/openbsd/kernel.c \
../src/libs/zbxsysinfo/openbsd/memory.c \
../src/libs/zbxsysinfo/openbsd/net.c \
../src/libs/zbxsysinfo/openbsd/openbsd.c \
../src/libs/zbxsysinfo/openbsd/proc.c \
../src/libs/zbxsysinfo/openbsd/sensors.c \
../src/libs/zbxsysinfo/openbsd/swap.c \
../src/libs/zbxsysinfo/openbsd/uptime.c 

OBJS += \
./src/libs/zbxsysinfo/openbsd/boottime.o \
./src/libs/zbxsysinfo/openbsd/cpu.o \
./src/libs/zbxsysinfo/openbsd/diskio.o \
./src/libs/zbxsysinfo/openbsd/diskspace.o \
./src/libs/zbxsysinfo/openbsd/inodes.o \
./src/libs/zbxsysinfo/openbsd/kernel.o \
./src/libs/zbxsysinfo/openbsd/memory.o \
./src/libs/zbxsysinfo/openbsd/net.o \
./src/libs/zbxsysinfo/openbsd/openbsd.o \
./src/libs/zbxsysinfo/openbsd/proc.o \
./src/libs/zbxsysinfo/openbsd/sensors.o \
./src/libs/zbxsysinfo/openbsd/swap.o \
./src/libs/zbxsysinfo/openbsd/uptime.o 

C_DEPS += \
./src/libs/zbxsysinfo/openbsd/boottime.d \
./src/libs/zbxsysinfo/openbsd/cpu.d \
./src/libs/zbxsysinfo/openbsd/diskio.d \
./src/libs/zbxsysinfo/openbsd/diskspace.d \
./src/libs/zbxsysinfo/openbsd/inodes.d \
./src/libs/zbxsysinfo/openbsd/kernel.d \
./src/libs/zbxsysinfo/openbsd/memory.d \
./src/libs/zbxsysinfo/openbsd/net.d \
./src/libs/zbxsysinfo/openbsd/openbsd.d \
./src/libs/zbxsysinfo/openbsd/proc.d \
./src/libs/zbxsysinfo/openbsd/sensors.d \
./src/libs/zbxsysinfo/openbsd/swap.d \
./src/libs/zbxsysinfo/openbsd/uptime.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsysinfo/openbsd/%.o: ../src/libs/zbxsysinfo/openbsd/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


