################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsysinfo/osf/cpu.c \
../src/libs/zbxsysinfo/osf/diskio.c \
../src/libs/zbxsysinfo/osf/diskspace.c \
../src/libs/zbxsysinfo/osf/inodes.c \
../src/libs/zbxsysinfo/osf/kernel.c \
../src/libs/zbxsysinfo/osf/memory.c \
../src/libs/zbxsysinfo/osf/osf.c \
../src/libs/zbxsysinfo/osf/proc.c \
../src/libs/zbxsysinfo/osf/swap.c \
../src/libs/zbxsysinfo/osf/uptime.c 

OBJS += \
./src/libs/zbxsysinfo/osf/cpu.o \
./src/libs/zbxsysinfo/osf/diskio.o \
./src/libs/zbxsysinfo/osf/diskspace.o \
./src/libs/zbxsysinfo/osf/inodes.o \
./src/libs/zbxsysinfo/osf/kernel.o \
./src/libs/zbxsysinfo/osf/memory.o \
./src/libs/zbxsysinfo/osf/osf.o \
./src/libs/zbxsysinfo/osf/proc.o \
./src/libs/zbxsysinfo/osf/swap.o \
./src/libs/zbxsysinfo/osf/uptime.o 

C_DEPS += \
./src/libs/zbxsysinfo/osf/cpu.d \
./src/libs/zbxsysinfo/osf/diskio.d \
./src/libs/zbxsysinfo/osf/diskspace.d \
./src/libs/zbxsysinfo/osf/inodes.d \
./src/libs/zbxsysinfo/osf/kernel.d \
./src/libs/zbxsysinfo/osf/memory.d \
./src/libs/zbxsysinfo/osf/osf.d \
./src/libs/zbxsysinfo/osf/proc.d \
./src/libs/zbxsysinfo/osf/swap.d \
./src/libs/zbxsysinfo/osf/uptime.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsysinfo/osf/%.o: ../src/libs/zbxsysinfo/osf/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


