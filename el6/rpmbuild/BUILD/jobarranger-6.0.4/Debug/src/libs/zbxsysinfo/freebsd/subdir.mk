################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsysinfo/freebsd/boottime.c \
../src/libs/zbxsysinfo/freebsd/cpu.c \
../src/libs/zbxsysinfo/freebsd/diskio.c \
../src/libs/zbxsysinfo/freebsd/diskspace.c \
../src/libs/zbxsysinfo/freebsd/freebsd.c \
../src/libs/zbxsysinfo/freebsd/inodes.c \
../src/libs/zbxsysinfo/freebsd/kernel.c \
../src/libs/zbxsysinfo/freebsd/memory.c \
../src/libs/zbxsysinfo/freebsd/net.c \
../src/libs/zbxsysinfo/freebsd/proc.c \
../src/libs/zbxsysinfo/freebsd/swap.c \
../src/libs/zbxsysinfo/freebsd/uptime.c 

OBJS += \
./src/libs/zbxsysinfo/freebsd/boottime.o \
./src/libs/zbxsysinfo/freebsd/cpu.o \
./src/libs/zbxsysinfo/freebsd/diskio.o \
./src/libs/zbxsysinfo/freebsd/diskspace.o \
./src/libs/zbxsysinfo/freebsd/freebsd.o \
./src/libs/zbxsysinfo/freebsd/inodes.o \
./src/libs/zbxsysinfo/freebsd/kernel.o \
./src/libs/zbxsysinfo/freebsd/memory.o \
./src/libs/zbxsysinfo/freebsd/net.o \
./src/libs/zbxsysinfo/freebsd/proc.o \
./src/libs/zbxsysinfo/freebsd/swap.o \
./src/libs/zbxsysinfo/freebsd/uptime.o 

C_DEPS += \
./src/libs/zbxsysinfo/freebsd/boottime.d \
./src/libs/zbxsysinfo/freebsd/cpu.d \
./src/libs/zbxsysinfo/freebsd/diskio.d \
./src/libs/zbxsysinfo/freebsd/diskspace.d \
./src/libs/zbxsysinfo/freebsd/freebsd.d \
./src/libs/zbxsysinfo/freebsd/inodes.d \
./src/libs/zbxsysinfo/freebsd/kernel.d \
./src/libs/zbxsysinfo/freebsd/memory.d \
./src/libs/zbxsysinfo/freebsd/net.d \
./src/libs/zbxsysinfo/freebsd/proc.d \
./src/libs/zbxsysinfo/freebsd/swap.d \
./src/libs/zbxsysinfo/freebsd/uptime.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsysinfo/freebsd/%.o: ../src/libs/zbxsysinfo/freebsd/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


