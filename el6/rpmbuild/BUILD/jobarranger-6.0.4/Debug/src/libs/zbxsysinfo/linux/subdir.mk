################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsysinfo/linux/boottime.c \
../src/libs/zbxsysinfo/linux/cpu.c \
../src/libs/zbxsysinfo/linux/diskio.c \
../src/libs/zbxsysinfo/linux/diskspace.c \
../src/libs/zbxsysinfo/linux/hardware.c \
../src/libs/zbxsysinfo/linux/inodes.c \
../src/libs/zbxsysinfo/linux/kernel.c \
../src/libs/zbxsysinfo/linux/linux.c \
../src/libs/zbxsysinfo/linux/memory.c \
../src/libs/zbxsysinfo/linux/net.c \
../src/libs/zbxsysinfo/linux/proc.c \
../src/libs/zbxsysinfo/linux/sensors.c \
../src/libs/zbxsysinfo/linux/software.c \
../src/libs/zbxsysinfo/linux/swap.c \
../src/libs/zbxsysinfo/linux/uptime.c 

OBJS += \
./src/libs/zbxsysinfo/linux/boottime.o \
./src/libs/zbxsysinfo/linux/cpu.o \
./src/libs/zbxsysinfo/linux/diskio.o \
./src/libs/zbxsysinfo/linux/diskspace.o \
./src/libs/zbxsysinfo/linux/hardware.o \
./src/libs/zbxsysinfo/linux/inodes.o \
./src/libs/zbxsysinfo/linux/kernel.o \
./src/libs/zbxsysinfo/linux/linux.o \
./src/libs/zbxsysinfo/linux/memory.o \
./src/libs/zbxsysinfo/linux/net.o \
./src/libs/zbxsysinfo/linux/proc.o \
./src/libs/zbxsysinfo/linux/sensors.o \
./src/libs/zbxsysinfo/linux/software.o \
./src/libs/zbxsysinfo/linux/swap.o \
./src/libs/zbxsysinfo/linux/uptime.o 

C_DEPS += \
./src/libs/zbxsysinfo/linux/boottime.d \
./src/libs/zbxsysinfo/linux/cpu.d \
./src/libs/zbxsysinfo/linux/diskio.d \
./src/libs/zbxsysinfo/linux/diskspace.d \
./src/libs/zbxsysinfo/linux/hardware.d \
./src/libs/zbxsysinfo/linux/inodes.d \
./src/libs/zbxsysinfo/linux/kernel.d \
./src/libs/zbxsysinfo/linux/linux.d \
./src/libs/zbxsysinfo/linux/memory.d \
./src/libs/zbxsysinfo/linux/net.d \
./src/libs/zbxsysinfo/linux/proc.d \
./src/libs/zbxsysinfo/linux/sensors.d \
./src/libs/zbxsysinfo/linux/software.d \
./src/libs/zbxsysinfo/linux/swap.d \
./src/libs/zbxsysinfo/linux/uptime.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsysinfo/linux/%.o: ../src/libs/zbxsysinfo/linux/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


